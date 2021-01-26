using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class MapAreaPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public MapAreaPacket()
        {
        }

        public MapAreaPacket(MapPacket[] maps)
        {
            Maps = maps;
        }

        [Key(0)]
        public MapPacket[] Maps { get; set; }

    }

}
