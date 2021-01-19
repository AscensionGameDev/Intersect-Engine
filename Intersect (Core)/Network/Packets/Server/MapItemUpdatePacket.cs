using System;

namespace Intersect.Network.Packets.Server
{

    public class MapItemUpdatePacket : CerasPacket
    {

        public MapItemUpdatePacket(Guid mapId, Point location, string itemData, bool remove = false)
        {
            MapId = mapId;
            Location = location;
            ItemData = itemData;
            Remove = remove;
        }

        public Guid MapId { get; set; }

        public bool Remove { get; set; }

        public Point Location { get; set; }

        public string ItemData { get; set; }

    }

}
