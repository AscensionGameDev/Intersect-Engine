using Intersect.Logging;
using Intersect.Network;
using Intersect.Server.Network;
using Lidgren.Network;

namespace Intersect.Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            Log.Global.AddOutput(new ConsoleOutput());

            var config = new NetworkConfiguration(4500);
            var network = new ServerNetwork(config);
            network.Start();
        }
    }
}