namespace Intersect.Framework.Net;

[Flags]
public enum NetworkTypes
{
    Public = 0,
    Loopback = 1,
    Subnet = 2,
    PrivateNetwork = 4,
}