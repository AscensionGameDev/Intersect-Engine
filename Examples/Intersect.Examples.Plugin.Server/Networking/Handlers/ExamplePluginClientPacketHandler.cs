using Intersect.Examples.Plugin.Packets.Client;
using Intersect.Network;

namespace Intersect.Examples.Plugin.Server.Networking.Handlers
{
    public class ExamplePluginClientPacketHandler : IPacketHandler<ExamplePluginClientPacket>
    {
        public bool Handle(IPacketSender packetSender, ExamplePluginClientPacket packet)
        {
            ApplicationContext.Context.Value?.Logger.LogInformation($"Received example plugin response from the client: {packet.ExamplePluginMessage}");
            return true;
        }

        public bool Handle(IPacketSender packetSender, IPacket packet) => Handle(packetSender, packet as ExamplePluginClientPacket);
    }
}
