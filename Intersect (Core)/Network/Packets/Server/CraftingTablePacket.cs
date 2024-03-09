using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public partial class CraftingTablePacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public CraftingTablePacket()
        {
        }

        public CraftingTablePacket(string tableData, bool close, bool journalMode)
        {
            TableData = tableData;
            Close = close;
            JournalMode = journalMode;
        }

        [Key(0)]
        public string TableData { get; set; }

        [Key(1)]
        public bool Close { get; set; }

        [Key(2)]
        public bool JournalMode { get; set; }
    }

}
