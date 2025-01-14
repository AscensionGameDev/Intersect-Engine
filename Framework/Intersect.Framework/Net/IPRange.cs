using System.Net;
using System.Net.Sockets;

namespace Intersect.Framework.Net;

public readonly record struct IPRange
{
    public AddressFamily AddressFamily { get; }
    public byte[] Start { get; }
    public byte[] End { get; }

    public IPRange(IPAddress address)
    {
        ArgumentNullException.ThrowIfNull(address, nameof(address));

        AddressFamily = address.AddressFamily;
        Start = address.GetAddressBytes();
        End = address.GetAddressBytes();
    }

    public IPRange(IPAddress start, IPAddress end)
    {
        ArgumentNullException.ThrowIfNull(start, nameof(start));
        ArgumentNullException.ThrowIfNull(end, nameof(end));

        if (start.AddressFamily != end.AddressFamily)
        {
            throw new ArgumentException("AddressFamily mismatch");
        }

        AddressFamily = start.AddressFamily;
        Start = start.GetAddressBytes();
        End = end.GetAddressBytes();
    }

    public bool Contains(IPAddress address)
    {
        ArgumentNullException.ThrowIfNull(address, nameof(address));

        if (address.AddressFamily != AddressFamily)
        {
            return false;
        }

        var octets = address.GetAddressBytes();
        if (Start.Length != End.Length || End.Length != octets.Length)
        {
            return false;
        }

        for (var index = 0; index < octets.Length; index++)
        {
            var start = Start[index];
            var end = End[index];
            var octet = octets[index];

            if (octet < start || end < octet)
            {
                return false;
            }
        }

        return true;
    }
}