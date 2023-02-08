using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Intersect.Server.Web.RestApi.Configuration;
using Intersect.Server.Web.RestApi.Types;

using Microsoft.Owin;

namespace Intersect.Server.Web.RestApi.Middleware
{
    internal class NetworkFilterMiddleware : OwinMiddleware
    {
        private const string SCHEME_HTTPS = "https";

        private readonly NetworkTypes _allowedNetworkTypes;

        public NetworkFilterMiddleware(OwinMiddleware next, NetworkTypes allowedNetworkTypes) : base(next)
        {
            _allowedNetworkTypes = allowedNetworkTypes;
        }

        public override Task Invoke(IOwinContext context)
        {
            if (context == default)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var request = context.Request;

            if (SCHEME_HTTPS.Equals(request.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return Next?.Invoke(context);
            }

            if (!IPAddress.TryParse(request.RemoteIpAddress, out var remoteIpAddress))
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Task.CompletedTask;
            }

            foreach (var networkTypeFlag in NetworkTypeFlags)
            {
                if (_allowedNetworkTypes.HasFlag(networkTypeFlag))
                {
                    var ipRanges = IpRangesForNetworkType[networkTypeFlag];
                    if (ipRanges.Any(range => range.IsInRange(remoteIpAddress)))
                    {
                        return Next?.Invoke(context);
                    }
                }
            }

            context.Response.StatusCode = (int)HttpStatusCode.UpgradeRequired;
            context.Response.ReasonPhrase = "HTTPS Required";
            context.Response.Headers["Upgrade"] = "TLS/1.2";
            context.Response.Headers["Upgrade-Insecure-Requests"] = "1";
            return Task.CompletedTask;
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
}
