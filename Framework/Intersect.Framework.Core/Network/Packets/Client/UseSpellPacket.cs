using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class UseSpellPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public UseSpellPacket()
    {
    }

    public UseSpellPacket(int slot, Guid targetId, bool softRetargetOnSelfCast)
    {
        Slot = slot;
        TargetId = targetId;
        SoftRetargetOnSelfCast = softRetargetOnSelfCast;
    }

    [Key(0)]
    public int Slot { get; set; }

    [Key(1)]
    public Guid TargetId { get; set; }

    [Key(2)]
    public bool SoftRetargetOnSelfCast { get; init; }
}
