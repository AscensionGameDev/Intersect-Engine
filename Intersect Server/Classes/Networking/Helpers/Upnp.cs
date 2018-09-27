using System;
using System.Text;
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
        private static string sExternalIp;
        private static bool sPortForwarded;
        private static StringBuilder sLog = new StringBuilder();

        public static async Task<NatDevice> ConnectNatDevice()
        {
            try
            {
                var nat = new NatDiscoverer();
                var cts = new CancellationTokenSource(5000);
                sDevice = await nat.DiscoverDeviceAsync(PortMapper.Upnp, cts);
                sExternalIp = (await sDevice.GetExternalIPAsync()).ToString();
                Console.WriteLine(Strings.Upnp.initialized);
                sLog.AppendLine("Connected to UPnP device: " + sDevice.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(Strings.Upnp.initializationfailed);
                sLog.AppendLine(Strings.Upnp.initializationfailed);
                sLog.AppendLine("UPnP Initialization Error: " + ex.ToString());
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
                await sDevice.CreatePortMapAsync(new Mapping(protocol, port, port,"Intersect Engine"));
                switch (protocol)
                {
                    case Protocol.Tcp:
                        Console.WriteLine(Strings.Upnp.forwardedtcp.ToString(port));
                        sLog.AppendLine(Strings.Upnp.forwardedtcp.ToString(port));
                        break;

                    case Protocol.Udp:
                        Console.WriteLine(Strings.Upnp.forwardedudp.ToString(port));
                        sLog.AppendLine(Strings.Upnp.forwardedudp.ToString(port));
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
                        Log.Warn("UPnP Could Not Open TCP Port " + port + Environment.NewLine + ex.ToString());
                        sLog.AppendLine("UPnP Could Not Open TCP Port " + port + Environment.NewLine + ex.ToString());
                        break;

                    case Protocol.Udp:
                        Console.WriteLine(Strings.Upnp.failedforwardingudp.ToString(port));
                        Log.Warn("UPnP Could Not Open UDP Port " + port + Environment.NewLine + ex.ToString());
                        sLog.AppendLine("UPnP Could Not Open UDP Port " + port + Environment.NewLine + ex.ToString());
                        break;
                }
            }
            return null;
        }

        public static bool ForwardingSucceeded()
        {
            return sPortForwarded;
        }

        public static string GetExternalIp()
        {
            return sExternalIp;
        }

        public static string GetLog()
        {
            return sLog.ToString();
        }
    }
}