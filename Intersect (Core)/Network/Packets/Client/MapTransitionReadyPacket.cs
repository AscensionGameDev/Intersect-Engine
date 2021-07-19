using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class MapTransitionReadyPacket : IntersectPacket
    {
        public MapTransitionReadyPacket(System.Guid mapId, float x, float y, byte dir)
        {
            NewMapId = mapId;
            X = x;
            Y = y;
            Dir = dir;
        }

        [Key(0)]
        public System.Guid NewMapId { get; set; }

        [Key(1)]
        public float X { get; set; }

        [Key(2)]
        public float Y { get; set; }

        [Key(3)]
        public byte Dir { get; set; }
    }
}
