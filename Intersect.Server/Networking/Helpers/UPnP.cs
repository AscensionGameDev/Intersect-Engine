using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Intersect.Logging;
using Intersect.Server.Localization;

using Open.Nat;

namespace Intersect.Server.Networking.Helpers
{

    public static class UpnP
    {

        private static NatDevice sDevice;

        private static string sExternalIp;

        private static StringBuilder sLog = new StringBuilder();

        private static bool sPortForwarded;

        public static async Task<NatDevice> ConnectNatDevice()
        {
            try
            {
                var nat = new NatDiscoverer();
                var cts = new CancellationTokenSource(5000);
                sDevice = await nat.DiscoverDeviceAsync(PortMapper.Upnp, cts);
                sExternalIp = (await sDevice.GetExternalIPAsync()).ToString();
                Log.Pretty.Info(Strings.Upnp.initialized);
                sLog.AppendLine("Connected to UPnP device: " + sDevice.ToString());
            }
            catch (Exception exception)
            {
                Log.Pretty.Error(exception, Strings.Upnp.initializationfailed);
                sLog.AppendLine(Strings.Upnp.initializationfailed);
                sLog.AppendLine("UPnP Initialization Error: " + exception.ToString());
            }

            return null;
        }

        public static async Task<NatDevice> OpenServerPort(int port, Protocol protocol)
        {
            if (sDevice == null)
            {
                return null;
            }

            try
            {
                var map = await sDevice.GetSpecificMappingAsync(protocol, port);
                if (map != null)
                {
                    await sDevice.DeletePortMapAsync(map);
                }

                await sDevice.CreatePortMapAsync(new Mapping(protocol, port, port, "Intersect Engine"));
                switch (protocol)
                {
                    case Protocol.Tcp:
                        Log.Pretty.Info(Strings.Upnp.forwardedtcp.ToString(port));
                        sLog.AppendLine(Strings.Upnp.forwardedtcp.ToString(port));

                        break;

                    case Protocol.Udp:
                        Log.Pretty.Info(Strings.Upnp.forwardedudp.ToString(port));
                        sLog.AppendLine(Strings.Upnp.forwardedudp.ToString(port));
                        sPortForwarded = true;

                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null);
                }
            }
            catch (Exception ex)
            {
                switch (protocol)
                {
                    case Protocol.Tcp:
                        Log.Pretty.Warn(Strings.Upnp.failedforwardingtcp.ToString(port));
                        Log.Warn("UPnP Could Not Open TCP Port " + port + Environment.NewLine + ex.ToString());
                        sLog.AppendLine("UPnP Could Not Open TCP Port " + port + Environment.NewLine + ex.ToString());

                        break;

                    case Protocol.Udp:
                        Log.Pretty.Warn(Strings.Upnp.failedforwardingudp.ToString(port));
                        Log.Warn("UPnP Could Not Open UDP Port " + port + Environment.NewLine + ex.ToString());
                        sLog.AppendLine("UPnP Could Not Open UDP Port " + port + Environment.NewLine + ex.ToString());

                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null);
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
