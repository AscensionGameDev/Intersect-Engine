using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

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
        set => SecretData = string.IsNullOrWhiteSpace(value) ? default : Convert.FromHexString(value);
    }

    [Newtonsoft.Json.JsonIgnore]
    public byte[] SecretData { get; set; }
}