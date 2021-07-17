using System;

using Intersect.Client.Framework.Plugins.Interfaces;
using Intersect.Client.MonoGame.Network;
using Intersect.Network;

namespace Intersect.Client.Plugins.Helpers
{
    /// <summary>
    /// Implementation if <see cref="IClientNetworkHelper"/>.
    /// </summary>
    public sealed class ClientNetworkHelper : IClientNetworkHelper
    {
        private IClient Client => MonoSocket.ClientLidgrenNetwork;
        
        public ClientNetworkHelper(IPacketSender packetSender)
        {
            PacketSender = packetSender;
        }

        /// <inheritdoc />
        public bool IsConnected => Client?.IsConnected ?? false;

        /// <inheritdoc />
        public bool IsServerOnline => Client?.IsServerOnline ?? false;

        /// <inheritdoc />
        public IPacketSender PacketSender { get; }

        /// <inheritdoc />
        public ConnectionStatistics Statistics => Client?.Connection?.Statistics;
    }
}
