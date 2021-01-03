using Intersect.Examples.Plugin.Packets.Client;
using Intersect.Examples.Plugin.Packets.Server;
using Intersect.Logging;
using Intersect.Network;

using System;

namespace Intersect.Examples.Plugin.Client.PacketHandlers
{
    public class ExamplePluginServerPacketHandler : IPacketHandler<ExamplePluginServerPacket>
    {
        public bool Handle(IPacketSender packetSender, ExamplePluginServerPacket packet)
        {
            if (default == packetSender)
            {
                throw new ArgumentNullException(nameof(packetSender));
            }

            if (default == packet)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            Log.Info($"Received server packet! The server said '{packet.ExamplePluginMessage}'.");
            
            var packetSent = packetSender.Send(new ExamplePluginClientPacket("A message from the client!"));
            if (packetSent)
            {
                Log.Info("Sent response back to the server!");
            } else
            {
                Log.Error("Failed to send response back to the server!");
            }

            return packetSent;
        }

        public bool Handle(IPacketSender packetSender, IPacket packet) => Handle(packetSender, packet as ExamplePluginServerPacket);
    }
}
