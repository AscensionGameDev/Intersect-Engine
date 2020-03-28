using System;

namespace Intersect.Network.Packets.Server
{

    public class MapItemsPacket : CerasPacket
    {

        public Guid MapId;

        public MapItemsPacket(Guid mapId, string[] items)
        {
            MapId = mapId;
            Items = items;
        }

        public string[] Items { get; set; }

    }

}
