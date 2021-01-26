using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class MapListPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public MapListPacket()
        {
        }

        public MapListPacket(string mapListData)
        {
            MapListData = mapListData;
        }

        [Key(0)]
        public string MapListData { get; set; }

    }

}
