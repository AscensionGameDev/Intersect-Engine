using Intersect.Memory;

namespace Intersect.Network
{
    public interface IPacket
    {
        IConnection Connection { get; }

        PacketGroups Group { get; }

        bool Read(ref IBuffer buffer);
        bool Write(ref IBuffer buffer);
    }
}