using System.Numerics;
using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
[Union(0, typeof(PlayerEntityPacket))]
[Union(1, typeof(EventEntityPacket))]
[Union(2, typeof(NpcEntityPacket))]
[Union(3, typeof(ProjectileEntityPacket))]
[Union(4, typeof(ResourceEntityPacket))]
[Union(5, typeof(PetEntityPacket))]
public abstract partial class EntityPacket : IntersectPacket
{

    [Key(1)]
    public Guid EntityId { get; set; }

    [Key(2)]
    public Guid MapId { get; set; }

    [Key(3)]
    public string Name { get; set; }

    [Key(4)]
    public string Sprite { get; set; }

    [Key(5)]
    public string Face { get; set; }

    [Key(6)]
    public int Level { get; set; }

    [Key(7)]
    public Vector3 Position { get; set; }
    
    [Key(8)]
    public byte Dummy8 { get; set; }
    
    [Key(9)]
    public byte Dummy9 { get; set; }

    [Key(10)]
    public byte Dir { get; set; }

    [Key(11)]
    public bool Passable { get; set; }

    [Key(12)]
    public bool HideName { get; set; }

    [Key(13)]
    public bool HideEntity { get; set; }

    [Key(14)]
    public Guid[] Animations { get; set; }

    [Key(15)]
    public long[] Vital { get; set; }

    [Key(16)]
    public long[] MaxVital { get; set; }

    [Key(17)]
    public int[] Stats { get; set; }

    [Key(18)]
    public StatusPacket[] StatusEffects { get; set; }

    [Key(19)]
    public bool IsSelf { get; set; }

    [Key(20)]
    public Color NameColor { get; set; }

    [Key(21)]
    public LabelPacket HeaderLabel { get; set; }

    [Key(22)]
    public LabelPacket FooterLabel { get; set; }

    [Key(23)]
    public Color Color { get; set; }

}
