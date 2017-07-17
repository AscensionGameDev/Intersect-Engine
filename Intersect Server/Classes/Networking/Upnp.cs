using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Intersect.Localization;
using Intersect.Logging;
using Open.Nat;

namespace Intersect.Server.Classes.Networking
{
    public static class UPnP
    {
        private static NatDevice _device;
        private static bool _portForwarded;
        
        public static async Task<NatDevice> ConnectNatDevice()
        {
            try
            {
                var nat = new NatDiscoverer();
                var cts = new CancellationTokenSource(5000);
                _device = await nat.DiscoverDeviceAsync(PortMapper.Upnp, cts);
                Console.WriteLine(Strings.Get("upnp","initialized"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Get("upnp", "initializationfailed"));
            }
            return null;
        }
        public static async Task<NatDevice> OpenServerPort(int port, Protocol protocol)
        {
            if (_device == null) return null;
            try
            {
                await _device.CreatePortMapAsync(new Mapping(protocol,port,port, "Intersect"));
                switch (protocol)
                {
                    case Protocol.Tcp:
                        Console.WriteLine(Strings.Get("upnp", "forwardedtcp",port));
                        break;

                    case Protocol.Udp:
                        Console.WriteLine(Strings.Get("upnp", "forwardedudp",port));
                        _portForwarded = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                switch (protocol)
                {
                    case Protocol.Tcp:
                        Console.WriteLine(Strings.Get("upnp", "failedforwardingtcp",port));
                        break;

                    case Protocol.Udp:
                        Console.WriteLine(Strings.Get("upnp", "failedforwardingudp", port));
                        break;
                }
            }
            return null;
        }

        public static bool ForwardingSucceeded()
        {
            return _portForwarded;
        }
    }
}
