using System;
using Lidgren.Network;

namespace Intersect.Network.Packets.Ping
{
    public class PingPacketGroup : IPacketGroup
    {
        public PacketGroups Group => PacketGroups.Ping;

        public IPacket Create(NetIncomingMessage message)
            => new PingPacket(message?.SenderConnection);
    }
}