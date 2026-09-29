using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Events;
using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class PlayerEntityPacket : EntityPacket
{
    //Parameterless Constructor for MessagePack
    public PlayerEntityPacket()
    {
    }


    [Key(24)]
    public Access AccessLevel { get; set; }


    [Key(25)]
    public Gender Gender { get; set; }


    [Key(26)]
    public Guid ClassId { get; set; }


    [Key(27)]
    public EquipmentPacket Equipment { get; set; }


    [Key(28)]
    public long CombatTimeRemaining { get; set; }


    [Key(29)]
    public string Guild { get; set; }


    [Key(30)]
    public int GuildRank { get; set; }

    /// <summary>Tints of the sprite colour layers ("_c1", "_c2", ...), empty when the sprite is not customised.</summary>
    [Key(31)]
    public Color[] SpriteColors { get; set; } = [];

    [Key(32)]
    public SpritePart[] SpriteParts { get; set; } = [];

    /// <summary>The mount the player would ride if IsMounted is true (may be Guid.Empty even while mounted if the mount was deleted since).</summary>
    [Key(33)]
    public Guid MountId { get; set; }

    [Key(34)]
    public bool IsMounted { get; set; }

    [Key(35)]
    public Guid FactionId { get; set; }
}
