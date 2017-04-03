using Intersect.Memory;

namespace Intersect.Network.Packets.Ping
{
    public class PingPacketGroup : IPacketGroup
    {
        public PacketGroups Group => PacketGroups.Ping;

        public IPacket Create(IConnection connection, IBuffer buffer)
            => new PingPacket(connection);
    }
}