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
        private static IClient Client => MonoSocket.ClientLidgrenNetwork;

        /// <inheritdoc />
        public bool IsConnected => Client?.IsConnected ?? false;

        /// <inheritdoc />
        public bool IsServerOnline => Client?.IsServerOnline ?? false;

        /// <inheritdoc />
        public IPacketSender PacketSender => Networking.Network.PacketHandler?.VirtualSender;

        /// <inheritdoc />
        public ConnectionStatistics Statistics => Client?.Connection?.Statistics;
    }
}
