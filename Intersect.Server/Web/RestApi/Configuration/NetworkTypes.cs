using System;

namespace Intersect.Server.Web.RestApi.Configuration
{
    [Flags]
    public enum NetworkTypes
    {
        Loopback = 1,
        Subnet = 2,
        PrivateNetwork = 4
    }
}
