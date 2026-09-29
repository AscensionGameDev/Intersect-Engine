using MessagePack;

namespace Intersect.Network.Packets.Client;

[MessagePackObject]
public partial class RemoveFurniturePacket : IntersectPacket
{
    //Parameterless Constructor for MessagePack
    public RemoveFurniturePacket()
    {
    }

    public RemoveFurniturePacket(Guid placementId)
    {
        PlacementId = placementId;
    }

    [Key(0)]
    public Guid PlacementId { get; set; }

}
