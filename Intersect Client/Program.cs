using Intersect.Client.Network;
using Intersect.Logging;
using Intersect.Network;
using System.Threading;

namespace Intersect.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            Log.Global.AddOutput(new ConsoleOutput());
            var config = new NetworkConfiguration("localhost", 4500);
            var network = new ClientNetwork(config);
            Thread.Sleep(1000);
            network.Start();
        }
    }
}