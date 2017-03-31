using Lidgren.Network;

namespace Intersect.Network
{
    public interface IPacket
    {
        NetConnection Connection { get; }

        PacketGroups Group { get; }

        bool Read(ref NetIncomingMessage message);
        bool Write(ref NetOutgoingMessage message);
    }
}