using Intersect.Memory;

namespace Intersect.Network
{
    public interface IPacket
    {
        IConnection Connection { get; }

        PacketCodes Code { get; }

        PacketType Type { get; }

        bool Read(ref IBuffer buffer);
        bool Write(ref IBuffer buffer);
    }
}