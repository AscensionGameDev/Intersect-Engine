using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Intersect.Client.Network;
using Intersect.Logging;
using Lidgren.Network;

namespace Intersect.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            Log.Global.AddOutput(new ConsoleOutput());

            var config = new NetPeerConfiguration("intersect-beta-4.0");
            config.AcceptIncomingConnections = false;
            /*config.LocalAddress =
                Dns.GetHostEntry("localhost")?
                    .AddressList?.First(
                        ip =>
                            (ip.AddressFamily == AddressFamily.InterNetwork));
            config.Port = 14232;*/
            var network = new ClientNetwork(config);
            Thread.Sleep(1000);
            network.Start();
        }
    }
}