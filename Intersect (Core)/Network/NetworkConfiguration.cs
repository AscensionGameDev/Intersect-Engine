using System;

namespace Intersect.Network
{

    public class NetworkConfiguration
    {

        public NetworkConfiguration() : this("localhost", 5400)
        {
        }

        public NetworkConfiguration(string host, ushort port) : this(host, port, 0, false)
        {
        }

        public NetworkConfiguration(ushort port, ushort maximumConnections = 512) : this(
            null, port, maximumConnections, true
        )
        {
        }

        private NetworkConfiguration(string host, ushort port, ushort maximumConnections, bool isServer)
        {
            Host = host;
            Port = port;
            IsServer = isServer;
            MaximumConnections = IsServer ? Math.Max(maximumConnections, (ushort) 1) : 0;

            if (port < 26 || port == 53)
            {
                Port = 5400;
            }
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public bool IsServer { get; }

        public int MaximumConnections { get; set; }

    }

}
