using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class DungeonInviteResponsePacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public DungeonInviteResponsePacket()
    {
    }

    public DungeonInviteResponsePacket(Guid leaderId, bool accepted)
    {
        LeaderId = leaderId;
        Accepted = accepted;
    }

    [Key(0)]
    public Guid LeaderId { get; set; }

    [Key(1)]
    public bool Accepted { get; set; }
}
