using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class CraftingTablePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public CraftingTablePacket()
        {
        }

        public CraftingTablePacket(string tableData, bool close)
        {
            TableData = tableData;
            Close = close;
        }

        [Key(0)]
        public string TableData { get; set; }

        [Key(1)]
        public bool Close { get; set; }

    }

}
