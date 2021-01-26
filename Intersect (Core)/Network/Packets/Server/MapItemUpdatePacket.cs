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

        //Item data implies item added or updated
        public MapItemUpdatePacket(Guid mapId, Point location, string itemData, bool remove = false)
        {
            MapId = mapId;
            Location = location;
            ItemData = itemData;
            Remove = remove;
        }

        [Key(0)]
        public Guid MapId { get; set; }

        [Key(1)]
        public bool Remove { get; set; }

        [Key(2)]
        public Point Location { get; set; }

        [Key(3)]
        public string ItemData { get; set; }

    }

}
