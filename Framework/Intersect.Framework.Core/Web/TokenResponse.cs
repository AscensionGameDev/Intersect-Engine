using Newtonsoft.Json;

namespace Intersect.Web;

public sealed class TokenResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty("token_type")]
    public string TokenType { get; set; } = TokenTypes.Bearer;

    [JsonProperty("expires_in")]
    public int ExpiresIn => (int)(Expires - DateTime.UtcNow).TotalMinutes;

    [JsonProperty(".issued")]
    public DateTime Issued { get; set; } = DateTime.UtcNow;

    [JsonProperty(".expires")]
    public DateTime Expires { get; set; }
}
