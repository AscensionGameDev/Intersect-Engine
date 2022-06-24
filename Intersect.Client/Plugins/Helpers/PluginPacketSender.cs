using System;
using System.Linq;

using Intersect.Core;
using Intersect.Network;
using Intersect.Plugins.Interfaces;

namespace Intersect.Client.Plugins.Helpers
{
    public sealed partial class PluginPacketSender : IPacketSender
    {
        private static IPacketSender VirtualSender => Networking.Network.PacketHandler?.VirtualSender;
        
        private readonly IPacketHelper mPluginPluginPacketHelper;

        public PluginPacketSender(IPacketHelper pluginPacketHelper)
        {
            mPluginPluginPacketHelper = pluginPacketHelper;
        }

        /// <inheritdoc />
        public IApplicationContext ApplicationContext => VirtualSender?.ApplicationContext;

        /// <inheritdoc />
        public bool Send(IPacket packet)
        {
            if (packet == default)
            {
                throw new ArgumentNullException(nameof(packet));
            }
            
            var packetType = packet.GetType();
            var packetTypeRegisteredByPlugin = mPluginPluginPacketHelper.AllPluginPacketTypes.Contains(packetType);

            if (packetTypeRegisteredByPlugin)
            {
                return VirtualSender?.Send(packet) ?? false;
            }

#if DEBUG
            ApplicationContext?.Logger.Error(
                $"Tried to send packet of type {packetType.FullName} but it was not registered by this plugin.\n" +
                "Available packet types:\n" +
                $"{string.Join("\n", mPluginPluginPacketHelper.AllPluginPacketTypes.Select(type => type.FullName))}"
            );
#endif
            return false;

        }
    }
}
