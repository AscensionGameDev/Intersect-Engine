using System;
using System.Threading;
using System.Threading.Tasks;
using Intersect.Logging;
using Intersect.Server.Classes.Localization;
using Open.Nat;

namespace Intersect.Server.Classes.Networking
{
    public static class UpnP
    {
        private static NatDevice sDevice;
        private static bool sPortForwarded;

        public static async Task<NatDevice> ConnectNatDevice()
        {
            try
            {
                var nat = new NatDiscoverer();
                var cts = new CancellationTokenSource(5000);
                sDevice = await nat.DiscoverDeviceAsync(PortMapper.Upnp, cts);
                Console.WriteLine(Strings.Upnp.initialized);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Upnp.initializationfailed);
            }
            return null;
        }

        public static async Task<NatDevice> OpenServerPort(int port, Protocol protocol)
        {
            if (sDevice == null) return null;
            try
            {
                Mapping map = await sDevice.GetSpecificMappingAsync(protocol, port);
                if (map != null) await sDevice.DeletePortMapAsync(map);
                await sDevice.CreatePortMapAsync(new Mapping(protocol, port, port));
                switch (protocol)
                {
                    case Protocol.Tcp:
                        Console.WriteLine(Strings.Upnp.forwardedtcp.ToString( port));
                        break;

                    case Protocol.Udp:
                        Console.WriteLine(Strings.Upnp.forwardedudp.ToString( port));
                        sPortForwarded = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                switch (protocol)
                {
                    case Protocol.Tcp:
                        Console.WriteLine(Strings.Upnp.failedforwardingtcp.ToString(port));
                        Log.Error("UPnP Error Opening TCP Port " + port + Environment.NewLine + ex.ToString());
                        break;

                    case Protocol.Udp:
                        Console.WriteLine(Strings.Upnp.failedforwardingudp.ToString(port));
                        Log.Error("UPnP Error Opening UDP Port " + port + Environment.NewLine + ex.ToString());
                        break;
                }
            }
            return null;
        }

        public static bool ForwardingSucceeded()
        {
            return sPortForwarded;
        }
    }
}