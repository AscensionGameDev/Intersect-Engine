using System.Net;
using Intersect.Framework.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Intersect.Server.Web.Middleware;

internal static class NetworkFilterMiddleware
{
    private const string SchemeHttps = "https";

    public static void UseNetworkFilterMiddleware(this WebApplication app, NetworkTypes allowedNetworkTypes)
    {
        app.Use(
            async (context, next) =>
            {
                var request = context.Request;

                if (SchemeHttps.Equals(request.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    await next.Invoke(context);
                    return;
                }

                if (!IPAddress.TryParse(
                        context.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                        out var remoteIpAddress
                    ))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return;
                }

                if (remoteIpAddress.MatchesNetworkTypes(allowedNetworkTypes))
                {
                    await next.Invoke(context);
                    return;
                }

                context.Response.StatusCode = (int)HttpStatusCode.UpgradeRequired;
                context.Response.Headers.Upgrade = "TLS/1.2";
                context.Response.Headers.UpgradeInsecureRequests = "1";
                await context.Response.WriteAsync("HTTPS Required");
            }
        );
    }
}