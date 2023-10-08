using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Intersect.Logging;
using Intersect.Server.Web.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.Server.Web;

internal partial class ApiService
{
    private const string SelfSignedCertificateName = "self-signed.crt";
    private const string SelfSignedKeyName = "self-signed.key";

    private static void UnpackAppSettings()
    {
        Log.Info("Unpacking appsettings...");
        var hostBuilder = Host.CreateApplicationBuilder();

        var names = new[] { "appsettings.json", $"appsettings.{hostBuilder.Environment.EnvironmentName}.json" };
        var manifestResourceNamePairs = Assembly.GetManifestResourceNames()
            .Where(mrn => names.Any(mrn.EndsWith))
            .Select(mrn => (mrn, names.First(mrn.EndsWith)))
            .ToArray();

        foreach (var (mrn, name) in manifestResourceNamePairs)
        {
            if (string.IsNullOrWhiteSpace(mrn) || string.IsNullOrWhiteSpace(name))
            {
                Log.Warn($"Manifest resource name or file name is null/empty: ({mrn}, {name})");
                continue;
            }

            if (File.Exists(name))
            {
                Log.Debug($"'{name}' already exists, not unpacking '{mrn}'");
                continue;
            }

            using var mrs = Assembly.GetManifestResourceStream(mrn);
            if (mrs == default)
            {
                Log.Warn($"Unable to open stream for embedded content: '{mrn}'");
                continue;
            }

            Log.Info($"Unpacking '{name}' in {Environment.CurrentDirectory}");

            using var fs = File.OpenWrite(name);
            mrs.CopyTo(fs);
        }
    }

    private static void ValidateConfiguration()
    {
        var builder = Host.CreateApplicationBuilder();

        var environmentAppSettingsFileName = $"appsettings.{builder.Environment.EnvironmentName}.json";
        var rawConfiguration = File.ReadAllText(environmentAppSettingsFileName);
        if (string.IsNullOrWhiteSpace(rawConfiguration) || rawConfiguration.Trim().Length < 2)
        {
            rawConfiguration = "{}";
        }

        var configurationJObject = JObject.Parse(rawConfiguration);
        if (!configurationJObject.TryGetValue("Api", out var apiSectionJToken))
        {
            apiSectionJToken = JObject.FromObject(new ApiConfiguration());
        }

        var apiConfiguration = apiSectionJToken.ToObject<ApiConfiguration>();
        apiConfiguration.TokenGenerationOptions ??= new TokenGenerationOptions();

        if (apiConfiguration.TokenGenerationOptions.AccessTokenLifetime < 1)
        {
            apiConfiguration.TokenGenerationOptions.AccessTokenLifetime =
                TokenGenerationOptions.DefaultAccessTokenLifetime;
        }

        if (apiConfiguration.TokenGenerationOptions.RefreshTokenLifetime < 1)
        {
            apiConfiguration.TokenGenerationOptions.RefreshTokenLifetime =
                TokenGenerationOptions.DefaultRefreshTokenLifetime;
        }

        if (apiConfiguration.TokenGenerationOptions.Secret.Length < 256)
        {
            apiConfiguration.TokenGenerationOptions.Secret = default;
            if (apiConfiguration.TokenGenerationOptions.Secret == default)
            {
                throw new UnreachableException("This should be automatically re-generated.");
            }
        }

        if (string.IsNullOrWhiteSpace(apiConfiguration.TokenGenerationOptions.Audience))
        {
            apiConfiguration.TokenGenerationOptions.Audience = TokenGenerationOptions.DefaultAudience;
            apiConfiguration.TokenGenerationOptions.Issuer = TokenGenerationOptions.DefaultIssuer;
        }

        var updatedApiConfigurationJObject = JObject.FromObject(apiConfiguration);
        configurationJObject["Api"] = updatedApiConfigurationJObject;

        updatedApiConfigurationJObject[nameof(JwtBearerOptions)] = apiSectionJToken[nameof(JwtBearerOptions)];
        var jwtBearerOptionsToken = updatedApiConfigurationJObject[nameof(JwtBearerOptions)];
        var jwtBearerOptions =
            jwtBearerOptionsToken?.Type == JTokenType.Object ? (JObject)jwtBearerOptionsToken : default;
        if (jwtBearerOptions == default)
        {
            jwtBearerOptions = new JObject();
            updatedApiConfigurationJObject[nameof(JwtBearerOptions)] = jwtBearerOptions;
        }

        if (!jwtBearerOptions.TryGetValue(
                nameof(JwtBearerOptions.TokenValidationParameters),
                out var tokenValidationParametersToken
            ) ||
            tokenValidationParametersToken.Type != JTokenType.Object)
        {
            tokenValidationParametersToken = new JObject();
            jwtBearerOptions[nameof(JwtBearerOptions.TokenValidationParameters)] = tokenValidationParametersToken;
        }

        var tokenValidationParameters = (JObject)tokenValidationParametersToken;
        if (!tokenValidationParameters.TryGetValue(
                nameof(TokenValidationParameters.ValidAudience),
                out var validAudienceToken
            ) ||
            tokenValidationParametersToken.Type != JTokenType.String ||
            string.IsNullOrWhiteSpace((string)validAudienceToken))
        {
            validAudienceToken = apiConfiguration.TokenGenerationOptions.Audience;
            tokenValidationParameters[nameof(TokenValidationParameters.ValidAudience)] = validAudienceToken;
        }

        if (!tokenValidationParameters.TryGetValue(
                nameof(TokenValidationParameters.ValidIssuer),
                out var validIssuerToken
            ) ||
            tokenValidationParametersToken.Type != JTokenType.String ||
            string.IsNullOrWhiteSpace((string)validIssuerToken))
        {
            validIssuerToken = apiConfiguration.TokenGenerationOptions.Issuer;
            tokenValidationParameters[nameof(TokenValidationParameters.ValidIssuer)] = validIssuerToken;
        }

        var updatedAppSettingsJson = configurationJObject.ToString();
        File.WriteAllText(environmentAppSettingsFileName, updatedAppSettingsJson);

        if (!configurationJObject.TryGetValue("Kestrel", out var kestrelToken) || kestrelToken is not JObject kestrel)
        {
            return;
        }

        if (!kestrel.TryGetValue("Endpoints", out var endpointsToken) || endpointsToken is not JObject endpoints)
        {
            return;
        }

        foreach (var endpointToken in endpoints.PropertyValues())
        {
            if (endpointToken is not JObject endpoint)
            {
                continue;
            }

            if (!endpoint.TryGetValue("Certificate", out var certificateToken) ||
                certificateToken is not JObject certificate)
            {
                continue;
            }

            try
            {
                var certificatePath = certificate.Value<string>("Path");
                var keyPath = certificate.Value<string>("KeyPath");

                if (!string.Equals(certificatePath, SelfSignedCertificateName) ||
                    !string.Equals(keyPath, SelfSignedKeyName))
                {
                    continue;
                }

                using var ecdsa = ECDsa.Create();
                CertificateRequest request = new("cn=self-signed", ecdsa, HashAlgorithmName.SHA256);
                var selfSignedCertificate = request.CreateSelfSigned(
                    DateTimeOffset.Now,
                    DateTimeOffset.Now.AddDays(30)
                );

                var certificatePem = selfSignedCertificate.ExportCertificatePem();
                var keyPem = selfSignedCertificate.GetECDsaPrivateKey().ExportECPrivateKeyPem();
                File.WriteAllText(SelfSignedCertificateName, certificatePem);
                File.WriteAllText(SelfSignedKeyName, keyPem);
            }
            catch (Exception exception)
            {
                Log.Warn(exception);
            }
        }
    }
}