using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class AttackPacket : AbstractTimedPacket
{
    //Parameterless Constructor for MessagePack
    public AttackPacket()
    {

    }
    
    public AttackPacket(Guid target, Guid aimMapId = default, byte aimX = 0, byte aimY = 0)
    {
        Target = target;
        AimMapId = aimMapId;
        AimX = aimX;
        AimY = aimY;
    }

    [Key(3)]
    public Guid Target { get; set; }

    // Where the attacker's mouse was pointing at attack time, in map tile coordinates - only
    // meaningful for weapon projectiles whose descriptor has MouseAimedBehavior. AimMapId ==
    // Guid.Empty means "no aim point" (mirrors UseSpellPacket's identical fields).
    [Key(4)]
    public Guid AimMapId { get; init; }

    [Key(5)]
    public byte AimX { get; init; }

    [Key(6)]
    public byte AimY { get; init; }

}
