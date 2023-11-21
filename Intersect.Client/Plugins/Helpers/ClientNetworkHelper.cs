using System;

using Intersect.Client.Framework.Plugins.Interfaces;
using Intersect.Client.MonoGame.Network;
using Intersect.Network;
using Intersect.Plugins.Helpers;
using Intersect.Plugins.Interfaces;

namespace Intersect.Client.Plugins.Helpers
{
    /// <summary>
    /// Implementation if <see cref="IClientNetworkHelper"/>.
    /// </summary>
    public sealed partial class ClientNetworkHelper : IClientNetworkHelper
    {
        private static IClient Client => MonoSocket.Instance.Network; // TODO: Single player wrapper

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
