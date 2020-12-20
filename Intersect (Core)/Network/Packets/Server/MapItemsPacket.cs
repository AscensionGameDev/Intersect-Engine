using System;
using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{

    public class MapItemsPacket : CerasPacket
    {

        public Guid MapId;

        public MapItemsPacket(Guid mapId, Dictionary<Point, List<string>> items)
        {
            MapId = mapId;
            Items = items;
        }

        public Dictionary<Point, List<string>> Items { get; set; }

    }

}
