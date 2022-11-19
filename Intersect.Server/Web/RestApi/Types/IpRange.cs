using System;
using System.Net;
using System.Net.Sockets;

namespace Intersect.Server.Web.RestApi.Types
{
    internal class IpRange
    {
        public IpRange(IPAddress address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            AddressFamily = address.AddressFamily;
            Start = address.GetAddressBytes();
            End = address.GetAddressBytes();
        }

        public IpRange(IPAddress start, IPAddress end)
        {
            if (start == default)
            {
                throw new ArgumentNullException(nameof(start));
            }

            if (end == default)
            {
                throw new ArgumentNullException(nameof(end));
            }

            if (start.AddressFamily != end.AddressFamily)
            {
                throw new ArgumentException("AddressFamily mismatch");
            }

            AddressFamily = start.AddressFamily;
            Start = start.GetAddressBytes();
            End = end.GetAddressBytes();
        }

        public AddressFamily AddressFamily { get; }

        public byte[] Start { get; }

        public byte[] End { get; }

        public bool IsInRange(IPAddress address)
        {
            if (Start == default || End == default || address == default) {
                return false;
            }

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
}
