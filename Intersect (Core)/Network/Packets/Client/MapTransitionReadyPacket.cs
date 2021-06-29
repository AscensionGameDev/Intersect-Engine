using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class MapTransitionReadyPacket : IntersectPacket
    {
        public MapTransitionReadyPacket(System.Guid mapId, float x, float y)
        {
            NewMapId = mapId;
            X = x;
            Y = y;
        }

        [Key(0)]
        public System.Guid NewMapId { get; set; }

        [Key(1)]
        public float X { get; set; }

        [Key(2)]
        public float Y { get; set; }

    }
}
