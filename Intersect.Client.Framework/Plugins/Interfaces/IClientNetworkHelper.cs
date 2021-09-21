using Intersect.Network;
using Intersect.Plugins.Interfaces;

namespace Intersect.Client.Framework.Plugins.Interfaces
{
    /// <summary>
    /// Defines the API for interfacing with the client's networking layer.
    /// </summary>
    public interface IClientNetworkHelper
    {
        /// <summary>
        /// If the client is connected to the server.
        /// </summary>
        bool IsConnected { get; }
        
        /// <summary>
        /// If the server is online.
        /// </summary>
        bool IsServerOnline { get; }

        /// <summary>
        /// The active packet sender for the network.
        /// </summary>
        IPacketSender PacketSender { get; }
        
        /// <summary>
        /// The statistics for the active connection.
        /// </summary>
        ConnectionStatistics Statistics { get; }
    }
}
