using System;

namespace Intersect.Network.Packets.Server
{

    public class MapItemUpdatePacket : CerasPacket
    {

        public MapItemUpdatePacket(Guid mapId, int itemIndex, string itemData)
        {
            MapId = mapId;
            ItemIndex = itemIndex;
            ItemData = itemData;
        }

        public Guid MapId { get; set; }

        public int ItemIndex { get; set; }

        public string ItemData { get; set; }

    }

}
