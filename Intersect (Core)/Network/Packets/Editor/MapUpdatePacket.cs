using MessagePack;
using System;

namespace Intersect.Network.Packets.Editor
{

    [MessagePackObject]
    public class MapUpdatePacket : EditorPacket
    {
        //Parameterless Constructor for MessagePack
        public MapUpdatePacket()
        {
        }

        public MapUpdatePacket(Guid mapId, string jsonData, byte[] tileData, byte[] attributeData)
        {
            MapId = mapId;
            JsonData = jsonData;
            TileData = tileData;
            AttributeData = attributeData;
        }

        [Key(0)]
        public Guid MapId { get; set; }

        [Key(1)]
        public string JsonData { get; set; }

        [Key(2)]
        public byte[] TileData { get; set; }

        [Key(3)]
        public byte[] AttributeData { get; set; }

    }

}
