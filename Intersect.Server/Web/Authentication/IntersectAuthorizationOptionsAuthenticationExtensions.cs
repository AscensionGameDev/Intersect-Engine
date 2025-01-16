using Intersect.Server.Database.PlayerData.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Intersect.Server.Web.Authentication;

public static class IntersectAuthorizationOptionsAuthenticationExtensions
{
    private static readonly string[] AllSchemes =
    [
        JwtBearerDefaults.AuthenticationScheme,
        CookieAuthenticationDefaults.AuthenticationScheme,
    ];

    public static AuthorizationOptions AddIntersectPolicies(this AuthorizationOptions authorizationOptions)
    {
        authorizationOptions.DefaultPolicy = new AuthorizationPolicyBuilder(authorizationOptions.DefaultPolicy)
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireRole(nameof(UserRights.Api))
            .Build();

        authorizationOptions.AddPolicy(
            JwtBearerDefaults.AuthenticationScheme,
            builder => builder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireRole(nameof(UserRights.Api), nameof(UserRights.Editor))
        );

        authorizationOptions.AddPolicy(
            "Developer",
            policy => policy.AddAuthenticationSchemes(AllSchemes).RequireRole(nameof(UserRights.Editor))
        );

        authorizationOptions.AddPolicy(
            nameof(UserRights.ApiRoles.UserManage),
            policy => policy.AddAuthenticationSchemes(AllSchemes)
                .RequireRole(nameof(UserRights.Api), nameof(UserRights.ApiRoles.UserManage))
        );

        authorizationOptions.AddPolicy(
            nameof(UserRights.ApiRoles.UserQuery),
            policy => policy.AddAuthenticationSchemes(AllSchemes)
                .RequireRole(nameof(UserRights.Api), nameof(UserRights.ApiRoles.UserQuery))
        );

        return authorizationOptions;
    }
}