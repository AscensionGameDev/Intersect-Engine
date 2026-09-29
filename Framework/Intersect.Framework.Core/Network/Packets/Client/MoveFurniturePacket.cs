using Intersect.Enums;
using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class MoveFurniturePacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public MoveFurniturePacket()
    {
    }

    public MoveFurniturePacket(Guid placementId, byte x, byte y, Direction direction)
    {
        PlacementId = placementId;
        X = x;
        Y = y;
        Direction = direction;
    }

    [Key(0)]
    public Guid PlacementId { get; set; }

    [Key(1)]
    public byte X { get; set; }

    [Key(2)]
    public byte Y { get; set; }

    [Key(3)]
    public Direction Direction { get; set; }

}
