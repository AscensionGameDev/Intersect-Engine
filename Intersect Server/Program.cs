using Intersect.Logging;
using Intersect.Server.Network;
using Lidgren.Network;

namespace Intersect.Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            Log.Global.AddOutput(new ConsoleOutput());

            var config = new NetPeerConfiguration("intersect-beta-4.0");
            config.AcceptIncomingConnections = true;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.MaximumConnections = 100;
            config.Port = 14232;
            var network = new ServerNetwork(config);
            network.Start();
        }
    }
}