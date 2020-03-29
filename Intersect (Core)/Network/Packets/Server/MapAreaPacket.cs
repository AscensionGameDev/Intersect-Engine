namespace Intersect.Network.Packets.Server
{

    public class MapAreaPacket : CerasPacket
    {

        public MapAreaPacket(MapPacket[] maps)
        {
            Maps = maps;
        }

        public MapPacket[] Maps { get; set; }

    }

}
