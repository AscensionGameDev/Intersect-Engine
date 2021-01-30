using MessagePack;
using System;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class PickupItemPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public PickupItemPacket()
        {
        }

        public PickupItemPacket(Guid mapId, int tileIndex, Guid uniqueId)
        {
            MapId = mapId;
            UniqueId = uniqueId;
            TileIndex = tileIndex;
        }

        [Key(0)]
        public Guid UniqueId { get; set; }

        [Key(1)]
        public Guid MapId { get; set; }

        [Key(2)]
        public int TileIndex { get; set; }

    }

}
