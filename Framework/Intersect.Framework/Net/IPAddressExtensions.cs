
using System.Net;

namespace Intersect.Framework.Net;

public static class IPAddressExtensions
{
    private static readonly Dictionary<NetworkTypes, IPRange[]> IPRangesForNetworkType = new()
    {
        {
            NetworkTypes.Loopback, [
                new IPRange(IPAddress.Parse("::1")),
                new IPRange(IPAddress.Parse("0.0.0.0"), IPAddress.Parse("0.255.255.255")),
                new IPRange(IPAddress.Parse("127.0.0.0"), IPAddress.Parse("127.255.255.255")),
            ]
        },
        {
            NetworkTypes.Subnet, [
                new IPRange(IPAddress.Parse("fe80::"), IPAddress.Parse("fe80::ffff:ffff:ffff:ffff")),
                new IPRange(IPAddress.Parse("169.254.0.0"), IPAddress.Parse("169.254.255.255")),
                new IPRange(IPAddress.Parse("255.255.255.255")),
            ]
        },
        {
            NetworkTypes.PrivateNetwork, [
                new IPRange(IPAddress.Parse("fc00::"), IPAddress.Parse("fdff:ffff:ffff:ffff:ffff:ffff:ffff:ffff")),
                new IPRange(IPAddress.Parse("::ffff:10.0.0.0"), IPAddress.Parse("::ffff:10.255.255.255")),
                // new IpRange(IPAddress.Parse("100.64.0.0"), IPAddress.Parse("100.127.255.255")),
                // new IpRange(IPAddress.Parse("172.16.0.0"), IPAddress.Parse("172.31.255.255")),
                // new IpRange(IPAddress.Parse("192.0.0.0"), IPAddress.Parse("192.0.0.255")),
                // new IpRange(IPAddress.Parse("192.168.0.0"), IPAddress.Parse("192.168.255.255")),
                // new IpRange(IPAddress.Parse("192.18.0.0"), IPAddress.Parse("192.19.255.255")),
                new IPRange(IPAddress.Parse("10.0.0.0"), IPAddress.Parse("10.255.255.255")),
                new IPRange(IPAddress.Parse("100.64.0.0"), IPAddress.Parse("100.127.255.255")),
                new IPRange(IPAddress.Parse("172.16.0.0"), IPAddress.Parse("172.31.255.255")),
                new IPRange(IPAddress.Parse("192.0.0.0"), IPAddress.Parse("192.0.0.255")),
                new IPRange(IPAddress.Parse("192.168.0.0"), IPAddress.Parse("192.168.255.255")),
                new IPRange(IPAddress.Parse("192.18.0.0"), IPAddress.Parse("192.19.255.255")),
            ]
        },
    };

    public static NetworkTypes GetNetworkType(this IPAddress address)
    {
        if (address.IsLoopback())
        {
            return NetworkTypes.Loopback;
        }

        if (address.IsPrivate())
        {
            return NetworkTypes.PrivateNetwork;
        }

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (address.IsSubnet())
        {
            return NetworkTypes.Subnet;
        }

        return NetworkTypes.Public;
    }

    public static bool IsLoopback(this IPAddress address) => MatchesNetworkTypes(address, NetworkTypes.Loopback);

    public static bool IsPrivate(this IPAddress address) => MatchesNetworkTypes(address, NetworkTypes.PrivateNetwork);

    public static bool IsPublic(this IPAddress address) =>
        IPRangesForNetworkType.Values.All(ipRanges => !ipRanges.Any(range => range.Contains(address)));

    public static bool IsSubnet(this IPAddress address) => MatchesNetworkTypes(address, NetworkTypes.Subnet);

    public static bool MatchesNetworkTypes(this IPAddress address, NetworkTypes networkTypes)
    {
        if (address.IsIPv4MappedToIPv6)
        {
            return MatchesNetworkTypes(address.MapToIPv4(), networkTypes);
        }

        if (networkTypes == NetworkTypes.Public)
        {
            return IsPublic(address);
        }

        var individualFlags = networkTypes.GetFlags();
        return individualFlags.Any(
            flag => IPRangesForNetworkType.TryGetValue(flag, out var ranges) &&
                    ranges.Any(range => range.Contains(address))
        );
    }
}