using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Intersect.Server.Database.PlayerData.Api;

namespace Intersect.Server.Web.Authentication;

public sealed record AuthenticationResult(
    AuthenticationResultType Type,
    ClaimsIdentity? Identity = default,
    ClaimsPrincipal? Principal = default
)
{
    public DateTime ExpiresAt { get; init; }

    public DateTime IssuedAt { get; init; }

    [NotNullIfNotNull(nameof(Identity))] public RefreshToken? RefreshToken { get; init; }
}