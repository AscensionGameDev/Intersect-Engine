using System.Security.Claims;
using Intersect.Server.Database.PlayerData.Security;

namespace Intersect.Server.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static IEnumerable<string> GetRoles(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.Claims.Where(claim => claim.Type == ClaimTypes.Role).Select(claim => claim.Value);

    public static bool HasRole(this ClaimsPrincipal claimsPrincipal, string role) =>
        claimsPrincipal.Claims.Any(claim => claim.Type == ClaimTypes.Role && claim.Value == role);

    public static bool IsEditor(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.HasRole(nameof(UserRights.Editor));

    public static bool HasSameRoles(this ClaimsPrincipal claimsPrincipal, IEnumerable<string> roles) =>
        roles.SequenceEqual(claimsPrincipal.GetRoles());
}