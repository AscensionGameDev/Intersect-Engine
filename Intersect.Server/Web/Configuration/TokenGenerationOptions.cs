using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Intersect.Core;


namespace Intersect.Server.Web.Configuration;

public class TokenGenerationOptions
{
    public const int DefaultAccessTokenLifetime = 5;
    public const string DefaultAudience = "https://localhost/api";
    public const string DefaultIssuer = "https://localhost";
    public const int DefaultRefreshTokenLifetime = 10080;

    /// <summary>
    /// Lifetime (in minutes) of access tokens (default is 5 minutes).
    /// </summary>
    public int AccessTokenLifetime { get; set; } = DefaultAccessTokenLifetime;

    public string Audience { get; set; } = DefaultAudience;

    public string Issuer { get; set; } = DefaultIssuer;

    /// <summary>
    /// Lifetime (in minutes) of refresh tokens (default is 10080 minutes or 7 days).
    /// </summary>
    public int RefreshTokenLifetime { get; set; } = DefaultRefreshTokenLifetime;

    [Required]
    public string Secret
    {
        get => Convert.ToHexString(SecretData ??= RandomNumberGenerator.GetBytes(64));
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                SecretData = default;
                return;
            }

            try
            {
                value = value.Trim();
                SecretData = Convert.FromHexString(value.Trim());
            }
            catch (Exception exception)
            {
                ApplicationContext.Context.Value?.Logger.LogError(exception, $"Failed to parse secret (should be hex), value was {value.Length} characters long");
                SecretData = default;
            }
        }
    }

    [Newtonsoft.Json.JsonIgnore]
    public byte[] SecretData { get; set; }
}