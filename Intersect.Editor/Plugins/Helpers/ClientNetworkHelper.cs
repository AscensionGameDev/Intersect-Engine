using System;

using Intersect.Client.Framework.Plugins.Interfaces;
using Intersect.Editor.MonoGame.Network;
using Intersect.Network;
using Intersect.Plugins.Helpers;
using Intersect.Plugins.Interfaces;

namespace Intersect.Editor.Plugins.Helpers
{
    /// <summary>
    /// Implementation if <see cref="IClientNetworkHelper"/>.
    /// </summary>
    public sealed class ClientNetworkHelper : IClientNetworkHelper
    {
        private static IClient Client => MonoSocket.ClientLidgrenNetwork;

        public ClientNetworkHelper(IPacketHelper packetHelper)
        {
            PacketHelper = packetHelper ?? throw new ArgumentNullException(nameof(packetHelper));
            PacketSender = new PluginPacketSender(PacketHelper);
        }

        private IPacketHelper PacketHelper { get; }

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
