using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class MapItemsPacket : CerasPacket
    {
        public Guid MapId;
        public string[] Items { get; set; }

        public MapItemsPacket(Guid mapId, string[] items)
        {
            MapId = mapId;
            Items = items;
        }
    }
}
