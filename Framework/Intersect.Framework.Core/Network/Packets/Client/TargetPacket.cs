using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class TargetPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public TargetPacket()
    {
    }

    public TargetPacket(Guid targetId)
    {
        TargetId = targetId;
    }

    [Key(0)]
    public Guid TargetId { get; set; }
}
