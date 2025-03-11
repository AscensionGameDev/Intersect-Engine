using Microsoft.AspNetCore.Authentication;

namespace Intersect.Server.Web.Extensions;

public static class BaseContextExtensions
{
    public static ILogger GetAPILogger<TOptions>(this BaseContext<TOptions> context) where TOptions : AuthenticationSchemeOptions
    {
        return context.HttpContext.GetAPILogger();
    }
}