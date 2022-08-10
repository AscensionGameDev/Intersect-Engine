
using MessagePack;

namespace Intersect.Network.Packets.Common;

[MessagePackObject]
public class BatchPacket : IntersectPacket
{
    public BatchPacket()
    {
        Packets = Array.Empty<IntersectPacket>();
    }

    public BatchPacket(params IntersectPacket[] packets)
    {
        Packets = packets;
    }

    [Key(0)]
    public IntersectPacket[] Packets { get; set; }
}
