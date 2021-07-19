using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class UpdateFutureWarpPacket : IntersectPacket
    {
        public UpdateFutureWarpPacket(System.Guid mapId, float x, float y, byte dir)
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
