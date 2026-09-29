using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class UseSpellPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public UseSpellPacket()
    {
    }

    public UseSpellPacket(int slot, Guid targetId, bool softRetargetOnSelfCast, Guid aimMapId = default, byte aimX = 0, byte aimY = 0)
    {
        Slot = slot;
        TargetId = targetId;
        SoftRetargetOnSelfCast = softRetargetOnSelfCast;
        AimMapId = aimMapId;
        AimX = aimX;
        AimY = aimY;
    }

    [Key(0)]
    public int Slot { get; set; }

    [Key(1)]
    public Guid TargetId { get; set; }

    [Key(2)]
    public bool SoftRetargetOnSelfCast { get; init; }

    // Where the caster's mouse was pointing at cast time, in map tile coordinates - only meaningful
    // for spells whose projectile has MouseAimedBehavior. AimMapId == Guid.Empty means "no aim point".
    [Key(3)]
    public Guid AimMapId { get; init; }

    [Key(4)]
    public byte AimX { get; init; }

    [Key(5)]
    public byte AimY { get; init; }
}
