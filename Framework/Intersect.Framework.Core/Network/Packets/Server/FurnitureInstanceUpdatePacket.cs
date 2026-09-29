using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class FurnitureInstanceUpdatePacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public FurnitureInstanceUpdatePacket()
    {
    }

    //No descriptor implies removal...
    public FurnitureInstanceUpdatePacket(Guid mapId, Guid id)
    {
        MapId = mapId;
        Id = id;
    }

    //Descriptor implies furniture added or updated
    public FurnitureInstanceUpdatePacket(Guid mapId, Guid id, Guid descriptorId, byte x, byte y, Direction direction)
    {
        MapId = mapId;
        Id = id;
        DescriptorId = descriptorId;
        X = x;
        Y = y;
        Direction = direction;
    }

    [Key(0)]
    public Guid MapId { get; set; }

    [Key(1)]
    public Guid Id { get; set; }

    [Key(2)]
    public Guid DescriptorId { get; set; }

    [Key(3)]
    public byte X { get; set; }

    [Key(4)]
    public byte Y { get; set; }

    [Key(5)]
    public Direction Direction { get; set; }

}
