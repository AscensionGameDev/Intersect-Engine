using MessagePack;
using System;

using System.Collections.Generic;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class MapItemsPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public MapItemsPacket()
        {
        }

        [Key(0)]
        public Guid MapId;

        public MapItemsPacket(Guid mapId, Dictionary<Point, List<string>> items)
        {
            MapId = mapId;
            Items = items;
        }

        [Key(1)]
        public Dictionary<Point, List<string>> Items { get; set; }

    }

}
