using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public class FadeCompletePacket : IntersectPacket
{
    public FadeCompletePacket()
    {
    }
}
