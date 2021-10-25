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

        public CraftingTablePacket(string tableData, bool close, bool update)
        {
            TableData = tableData;
            Close = close;
            Update = update;
        }

        [Key(0)]
        public string TableData { get; set; }

        [Key(1)]
        public bool Close { get; set; }

        [Key(2)]
        public bool Update { get; set; }

    }

}
