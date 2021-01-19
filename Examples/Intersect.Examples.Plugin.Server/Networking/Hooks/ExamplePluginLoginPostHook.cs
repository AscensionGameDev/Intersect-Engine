using Intersect.Examples.Plugin.Packets.Server;
using Intersect.Logging;
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
                Log.Info("Sent a message to the client!");
            }
            else
            {
                Log.Error("Failed to send a message to the client!");
            }

            return packetSent;
        }

        public bool Handle(IPacketSender packetSender, IPacket packet) => Handle(packetSender, packet as LoginPacket);
    }
}
