namespace Intersect.Network.Packets.Server
{

    public class MapListPacket : CerasPacket
    {

        public MapListPacket(string mapListData)
        {
            MapListData = mapListData;
        }

        public string MapListData { get; set; }

    }

}
