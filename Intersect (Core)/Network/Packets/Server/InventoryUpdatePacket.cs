using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network.Packets.Server
{
    public class InventoryUpdatePacket : CerasPacket
    {
        public int Slot { get; set; }
        public string ItemData { get; set; }

        public InventoryUpdatePacket(int slot, string itemData)
        {
            Slot = slot;
            ItemData = itemData;
        }
    }
}
