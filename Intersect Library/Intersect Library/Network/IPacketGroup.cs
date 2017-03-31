using Lidgren.Network;

namespace Intersect.Network
{
    public interface IPacketGroup
    {
        PacketGroups Group { get; }

        IPacket Create(NetIncomingMessage message);
    }
}
