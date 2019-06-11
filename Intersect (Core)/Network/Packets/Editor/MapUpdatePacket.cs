using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Editor
{
    public class MapUpdatePacket : EditorPacket
    {
        public Guid MapId { get; set; }
        public string JsonData { get; set; }
        public byte[] TileData { get; set; }
        public byte[] AttributeData { get; set; }

        public MapUpdatePacket(Guid mapId, string jsonData, byte[] tileData, byte[] attributeData)
        {
            MapId = mapId;
            JsonData = jsonData;
            TileData = tileData;
            AttributeData = attributeData;
        }
    }
}
