using System.Net;
using Intersect.Server.Web.RestApi.Configuration;
using Intersect.Server.Web.RestApi.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Intersect.Server.Web.Middleware;

internal static class NetworkFilterMiddleware
{
    private const string SCHEME_HTTPS = "https";

    public static void UseNetworkFilterMiddleware(this WebApplication app, NetworkTypes allowedNetworkTypes)
    {
        app.Use(
            async (context, next) =>
            {
                var request = context.Request;

                if (SCHEME_HTTPS.Equals(request.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    await next.Invoke(context);
                    return;
                }

                if (!IPAddress.TryParse(context.Connection.RemoteIpAddress?.ToString() ?? string.Empty, out var remoteIpAddress))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return;
                }

                foreach (var networkTypeFlag in NetworkTypeFlags)
                {
                    if (!allowedNetworkTypes.HasFlag(networkTypeFlag))
                    {
                        continue;
                    }

                    var ipRanges = IpRangesForNetworkType[networkTypeFlag];
                    if (!ipRanges.Any(range => range.IsInRange(remoteIpAddress)))
                    {
                        continue;
                    }

                    await next.Invoke(context);
                    return;
                }

                context.Response.StatusCode = (int)HttpStatusCode.UpgradeRequired;
                context.Response.Headers["Upgrade"] = "TLS/1.2";
                context.Response.Headers["Upgrade-Insecure-Requests"] = "1";
                await context.Response.WriteAsync("HTTPS Required");
            }
        );
    }

    private static readonly IDictionary<NetworkTypes, IpRange[]> IpRangesForNetworkType = new Dictionary<NetworkTypes, IpRange[]>
    {
        {
            NetworkTypes.Loopback,
            new[]
            {
                new IpRange(IPAddress.Parse("::1")),
                new IpRange(IPAddress.Parse("0.0.0.0"), IPAddress.Parse("0.255.255.255")),
                new IpRange(IPAddress.Parse("127.0.0.0"), IPAddress.Parse("127.255.255.255"))
            }
        },
        {
            NetworkTypes.Subnet,
            new[]
            {
                new IpRange(IPAddress.Parse("fe80::"), IPAddress.Parse("fe80::ffff:ffff:ffff:ffff")),
                new IpRange(IPAddress.Parse("169.254.0.0"), IPAddress.Parse("169.254.255.255")),
                new IpRange(IPAddress.Parse("255.255.255.255")),
            }
        },
        {
            NetworkTypes.PrivateNetwork,
            new[]
            {
                new IpRange(IPAddress.Parse("fc00::"), IPAddress.Parse("fdff:ffff:ffff:ffff:ffff:ffff:ffff:ffff")),
                new IpRange(IPAddress.Parse("::ffff:10.0.0.0"), IPAddress.Parse("::ffff:10.255.255.255")),
                // new IpRange(IPAddress.Parse("100.64.0.0"), IPAddress.Parse("100.127.255.255")),
                // new IpRange(IPAddress.Parse("172.16.0.0"), IPAddress.Parse("172.31.255.255")),
                // new IpRange(IPAddress.Parse("192.0.0.0"), IPAddress.Parse("192.0.0.255")),
                // new IpRange(IPAddress.Parse("192.168.0.0"), IPAddress.Parse("192.168.255.255")),
                // new IpRange(IPAddress.Parse("192.18.0.0"), IPAddress.Parse("192.19.255.255")),
                new IpRange(IPAddress.Parse("10.0.0.0"), IPAddress.Parse("10.255.255.255")),
                new IpRange(IPAddress.Parse("100.64.0.0"), IPAddress.Parse("100.127.255.255")),
                new IpRange(IPAddress.Parse("172.16.0.0"), IPAddress.Parse("172.31.255.255")),
                new IpRange(IPAddress.Parse("192.0.0.0"), IPAddress.Parse("192.0.0.255")),
                new IpRange(IPAddress.Parse("192.168.0.0"), IPAddress.Parse("192.168.255.255")),
                new IpRange(IPAddress.Parse("192.18.0.0"), IPAddress.Parse("192.19.255.255")),
            }
        }
    };

    private static readonly NetworkTypes[] NetworkTypeFlags = Enum.GetValues(typeof(NetworkTypes)).OfType<NetworkTypes>().ToArray();
}