using System;

namespace Intersect.Network.Packets.Server
{

    public class MapPacket : CerasPacket
    {

        public MapPacket(
            Guid mapId,
            bool deleted,
            string data = null,
            byte[] tileData = null,
            byte[] attributeData = null,
            int revision = -1,
            int gridX = -1,
            int gridY = -1,
            bool[] borders = null
        )
        {
            MapId = mapId;
            Deleted = deleted;
            Data = data;
            TileData = tileData;
            AttributeData = attributeData;
            Revision = revision;
            GridX = gridX;
            GridY = gridY;
            CameraHolds = borders;
        }

        public Guid MapId { get; set; }

        public bool Deleted { get; set; }

        public string Data { get; set; }

        public byte[] TileData { get; set; }

        public byte[] AttributeData { get; set; }

        public int Revision { get; set; }

        public int GridX { get; set; }

        public int GridY { get; set; }

        public bool[] CameraHolds { get; set; }

        public MapEntitiesPacket MapEntities { get; set; }

        public MapItemsPacket MapItems { get; set; }

    }

}
