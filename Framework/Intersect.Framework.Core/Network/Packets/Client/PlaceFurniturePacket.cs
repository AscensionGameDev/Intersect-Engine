using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class PlaceFurniturePacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public PlaceFurniturePacket()
    {
    }

    public PlaceFurniturePacket(Guid furnitureDescriptorId, byte x, byte y, Direction direction)
    {
        FurnitureDescriptorId = furnitureDescriptorId;
        X = x;
        Y = y;
        Direction = direction;
    }

    [Key(0)]
    public Guid FurnitureDescriptorId { get; set; }

    [Key(1)]
    public byte X { get; set; }

    [Key(2)]
    public byte Y { get; set; }

    [Key(3)]
    public Direction Direction { get; set; }

}
