using MessagePack;

namespace Intersect.Network.Packets.Server;

[MessagePackObject]
public partial class FurnitureInstancesPacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public FurnitureInstancesPacket()
    {
    }

    [Key(0)]
    public Guid MapId;

    public FurnitureInstancesPacket(Guid mapId, FurnitureInstanceUpdatePacket[] furniture)
    {
        MapId = mapId;
        Furniture = furniture;
    }

    [Key(1)]
    public FurnitureInstanceUpdatePacket[] Furniture { get; set; }

}
