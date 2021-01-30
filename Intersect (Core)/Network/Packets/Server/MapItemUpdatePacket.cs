using MessagePack;
using System;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class MapItemUpdatePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public MapItemUpdatePacket()
        {
        }

        //No item data implies removal...
        public MapItemUpdatePacket(Guid mapId, int tileIndex, Guid uniqueId)
        {
            MapId = mapId;
            TileIndex = tileIndex;
            Id = uniqueId;
        }

        //Item data implies item added or updated
        public MapItemUpdatePacket(Guid mapId, int tileIndex, Guid uniqueId, Guid itemId, Guid? bagId, int quantity, int[] statbuffs)
        {
            MapId = mapId;
            TileIndex = tileIndex;
            Id = uniqueId;
            ItemId = itemId;
            BagId = bagId;
            Quantity = quantity;
            StatBuffs = statbuffs;
        }

        [Key(0)]
        public Guid MapId { get; set; }

        [Key(1)]
        public int TileIndex { get; set; }

        [Key(2)]
        public Guid Id { get; set; }

        [Key(3)]
        public Guid ItemId { get; set; }

        [Key(4)]
        public Guid? BagId { get; set; }

        [Key(5)]
        public int Quantity { get; set; }

        [Key(6)]
        public int[] StatBuffs { get; set; }

    }

}
