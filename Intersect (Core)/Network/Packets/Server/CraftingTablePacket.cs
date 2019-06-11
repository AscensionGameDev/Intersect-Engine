using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class CraftingTablePacket : CerasPacket
    {
        public string TableData { get; set; }
        public bool Close { get; set; }

        public CraftingTablePacket(string tableData, bool close)
        {
            TableData = tableData;
            Close = close;
        }
    }
}
