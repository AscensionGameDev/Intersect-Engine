using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class MapItemUpdatePacket : CerasPacket
    {
        public Guid MapId { get; set; }
        public int ItemIndex { get; set; }
        public string ItemData { get; set; }

        public MapItemUpdatePacket(Guid mapId, int itemIndex, string itemData)
        {
            MapId = mapId;
            ItemIndex = itemIndex;
            ItemData = itemData;
        }
    }
}
