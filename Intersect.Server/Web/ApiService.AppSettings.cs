using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Intersect.Core;
using Intersect.Framework.Reflection;
using Intersect.Server.Web.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intersect.Server.Web;

internal partial class ApiService
{
    private const string SelfSignedCertificateName = "self-signed.crt";
    private const string SelfSignedKeyName = "self-signed.key";

    private const string FileNameAppsettingsJson = "appsettings.json";
    private static readonly string FileNameAppsettingsEnvironmentJson;

    private static readonly JObject? AppsettingsToken;
    private static readonly JObject? AppsettingsEnvironmentToken;

    static ApiService()
    {
        var dummyHostBuilder = Host.CreateApplicationBuilder();
        FileNameAppsettingsEnvironmentJson = $"appsettings.{dummyHostBuilder.Environment.EnvironmentName}.json";

        AppsettingsToken = ParseEmbeddedAppsettings(FileNameAppsettingsJson);
        AppsettingsEnvironmentToken = ParseEmbeddedAppsettings(FileNameAppsettingsEnvironmentJson);
    }

    private static JObject? ParseEmbeddedAppsettings(string name)
    {
        try
        {
            if (!Assembly.TryFindResource(name, out var resourceName))
            {
                ApplicationContext.CurrentContext.Logger.LogWarning("Failed to find '{AppsettingsName}'", name);
                return null;
            }

            using var resourceStream = Assembly.GetManifestResourceStream(resourceName);
            if (resourceStream == default)
            {
                ApplicationContext.Context.Value?.Logger.LogError(
                    "Failed to open resource stream for '{ResourceName}'",
                    resourceName
                );
                return null;
            }

            using var reader = new StreamReader(resourceStream, Encoding.UTF8);
            var rawJson = reader.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(rawJson))
            {
                return JObject.Parse(rawJson);
            }

            ApplicationContext.Context.Value?.Logger.LogError(
                "Invalid JSON, embedded resouce was empty or whitespace: '{ResourceName}'",
                resourceName
            );
            return null;
        }
        catch (Exception exception)
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                exception,
                "Exception thrown while loading and parsing '{Name}'",
                name
            );
            return null;
        }
    }

    private static void Dump(string fileName, JObject? token)
    {
        try
        {
            if (token == null)
            {
                ApplicationContext.Context.Value?.Logger.LogDebug(
                    "Skipping dumping '{FileName}' because the token is null",
                    fileName
                );
                return;
            }

            if (File.Exists(FileNameAppsettingsJson))
            {
                ApplicationContext.Context.Value?.Logger.LogDebug(
                    "Skipping dumping '{FileName}' because the file already exists",
                    fileName
                );
                return;
            }

            var rawJson = token.ToString(Formatting.Indented);
            File.WriteAllText(fileName, rawJson, Encoding.UTF8);
        }
        catch (Exception exception)
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                exception,
                "Exception thrown while dumping '{FileName}'",
                fileName
            );
        }
    }

    private static void UnpackAppSettings()
    {
        ApplicationContext.Context.Value?.Logger.LogInformation("Unpacking appsettings...");
        Dump(FileNameAppsettingsJson, AppsettingsToken);
        Dump(FileNameAppsettingsEnvironmentJson, AppsettingsEnvironmentToken);
    }

    private static JObject? LoadOrFallbackTo(FileInfo fileInfo, JObject? fallback)
    {
        if (!fileInfo.Exists)
        {
            return fallback;
        }

        try
        {
            using var reader = fileInfo.OpenText();
            var rawJson = reader.ReadToEnd();
            JObject parsedObject = JObject.Parse(rawJson);
            return parsedObject;
        }
        catch (Exception exception)
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                exception,
                "Exception thrown while loading and parsing '{Name}' and will return the fallback instead",
                Path.GetRelativePath(Environment.CurrentDirectory, fileInfo.FullName)
            );
            return fallback;
        }
    }

    private static void ValidateConfiguration()
    {
        var builder = Host.CreateApplicationBuilder();

        FileInfo genericConfigurationFileInfo = new(FileNameAppsettingsJson);
        JObject? genericConfigurationToken = LoadOrFallbackTo(genericConfigurationFileInfo, AppsettingsToken);

        FileInfo environmentConfigurationFileInfo = new(FileNameAppsettingsEnvironmentJson);
        JObject? environmentConfigurationToken = LoadOrFallbackTo(environmentConfigurationFileInfo, AppsettingsToken);

        JsonMergeSettings jsonMergeSettings = new()
        {
            MergeArrayHandling = MergeArrayHandling.Replace,
            MergeNullValueHandling = MergeNullValueHandling.Merge,
            PropertyNameComparison = StringComparison.Ordinal,
        };

        JObject mergedConfiguration = new();

        if (genericConfigurationToken is not null)
        {
            mergedConfiguration.Merge(genericConfigurationToken, jsonMergeSettings);
        }

        if (environmentConfigurationToken is not null)
        {
            mergedConfiguration.Merge(environmentConfigurationToken, jsonMergeSettings);
        }

        if (!mergedConfiguration.TryGetValue("Api", out var apiSectionJToken))
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

        if (apiConfiguration.TokenGenerationOptions.Secret.Length < 128)
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

        if (apiConfiguration.StaticFilePaths == default)
        {
            apiConfiguration.StaticFilePaths = [new StaticFilePathOptions("wwwroot")];
        }

        var updatedApiConfigurationJObject = JObject.FromObject(apiConfiguration);
        mergedConfiguration["Api"] = updatedApiConfigurationJObject;

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

        var updatedAppSettingsJson = mergedConfiguration.ToString();
        File.WriteAllText(FileNameAppsettingsEnvironmentJson, updatedAppSettingsJson);

        // Below this point should be only self-signed certificate generation

        if (!mergedConfiguration.TryGetValue("Kestrel", out var kestrelToken) || kestrelToken is not JObject kestrel)
        {
            return;
        }

        if (!kestrel.TryGetValue("Endpoints", out var endpointsToken) || endpointsToken is not JObject endpoints)
        {
            return;
        }

        foreach (var endpointToken in endpoints.PropertyValues())
        {
            if (endpointToken is not JObject endpointValue)
            {
                continue;
            }
            var endpoint = endpointValue.ToObject<PartialKestrelEndpoint>();

            if (endpoint.Certificate is not { } certificate)
            {
                continue;
            }

            try
            {
                var certificatePath = certificate.Path;
                var keyPath = certificate.KeyPath;

                if (!string.Equals(certificatePath, SelfSignedCertificateName) ||
                    !string.Equals(keyPath, SelfSignedKeyName))
                {
                    continue;
                }

                if (File.Exists(certificatePath) && File.Exists(keyPath))
                {
                    var existingSelfSignedCertificate = X509Certificate2.CreateFromPemFile(certificatePath, keyPath);
                    if (existingSelfSignedCertificate.NotAfter.ToUniversalTime() > DateTime.UtcNow)
                    {
                        continue;
                    }

                    ApplicationContext.CurrentContext.Logger.LogInformation(
                        "Self-signed certificate is expired and will be regenerated"
                    );
                }

                AsymmetricAlgorithm algorithm;
                CertificateRequest certificateRequest;
                var osVersion = Environment.OSVersion;

                PartialKestrelEndpointCertificateType certificateType = certificate.SelfSignedCertificateType;
                if (certificateType == PartialKestrelEndpointCertificateType.None)
                {
                    certificateType = osVersion.Platform switch
                    {
                        PlatformID.Win32S or PlatformID.Win32Windows or PlatformID.Win32NT or PlatformID.WinCE
                            or PlatformID.Xbox => osVersion.Version.Major < 11
                                ? PartialKestrelEndpointCertificateType.RSA
                                : PartialKestrelEndpointCertificateType.ECDSA,
                        PlatformID.Unix or PlatformID.MacOSX or PlatformID.Other =>
                            PartialKestrelEndpointCertificateType.ECDSA,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }

                switch (certificateType)
                {
                    case PartialKestrelEndpointCertificateType.None:
                        throw new InvalidOperationException();
                    case PartialKestrelEndpointCertificateType.RSA:
                    {
                        var rsa = RSA.Create(4096);
                        algorithm = rsa;
                        certificateRequest = new CertificateRequest(
                            "cn=self-signed",
                            rsa,
                            HashAlgorithmName.SHA256,
                            RSASignaturePadding.Pkcs1
                        );
                        break;
                    }
                    case PartialKestrelEndpointCertificateType.ECDSA:
                    {
                        var ecdsa = ECDsa.Create();
                        algorithm = ecdsa;
                        certificateRequest = new CertificateRequest("cn=self-signed", ecdsa, HashAlgorithmName.SHA384);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var selfSignedCertificate = certificateRequest.CreateSelfSigned(
                    DateTimeOffset.Now,
                    DateTimeOffset.Now.AddDays(30)
                );

                var certificatePem = selfSignedCertificate.ExportCertificatePem();

                string keyPem;
                if (selfSignedCertificate.GetRSAPrivateKey() is { } rsaPrivateKey)
                {
                    keyPem = rsaPrivateKey.ExportRSAPrivateKeyPem();
                }
                else if (selfSignedCertificate.GetECDsaPrivateKey() is { } ecDsaPrivateKey)
                {
                    keyPem = ecDsaPrivateKey.ExportECPrivateKeyPem();
                }
                else
                {
                    throw new InvalidOperationException(
                        "Private key should be RSA or ECDSA, why was this code changed without verifying it?"
                    );
                }

                File.WriteAllText(SelfSignedCertificateName, certificatePem);
                File.WriteAllText(SelfSignedKeyName, keyPem);

                algorithm.Dispose();
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogWarning(
                    exception,
                    "Error creating self-signed certificate"
                );
            }
        }
    }
}