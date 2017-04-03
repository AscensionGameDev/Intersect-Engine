using Intersect.Memory;

namespace Intersect.Network
{
    public interface IPacketGroup
    {
        PacketGroups Group { get; }

        IPacket Create(IConnection connection, IBuffer message);
    }
}
