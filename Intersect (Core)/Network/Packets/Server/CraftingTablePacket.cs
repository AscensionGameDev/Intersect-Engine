namespace Intersect.Network.Packets.Server
{

    public class CraftingTablePacket : CerasPacket
    {

        public CraftingTablePacket(string tableData, bool close)
        {
            TableData = tableData;
            Close = close;
        }

        public string TableData { get; set; }

        public bool Close { get; set; }

    }

}
