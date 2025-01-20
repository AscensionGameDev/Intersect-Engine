using Intersect.Examples.Plugin.Packets.Server;
using Intersect.Network;
using Intersect.Network.Packets.Client;

namespace Intersect.Examples.Plugin.Server.Networking.Hooks
{
    public class ExamplePluginLoginPostHook : IPacketHandler<LoginPacket>
    {
        public bool Handle(IPacketSender packetSender, LoginPacket packet)
        {
            var packetSent = packetSender.Send(new ExamplePluginServerPacket($"Login packet received: {packet.Username}"));
            if (packetSent)
            {
                ApplicationContext.Context.Value?.Logger.LogInformation("Sent a message to the client!");
            }
            else
            {
                ApplicationContext.Context.Value?.Logger.LogError("Failed to send a message to the client!");
            }

            return packetSent;
        }

        public bool Handle(IPacketSender packetSender, IPacket packet) => Handle(packetSender, packet as LoginPacket);
    }
}
