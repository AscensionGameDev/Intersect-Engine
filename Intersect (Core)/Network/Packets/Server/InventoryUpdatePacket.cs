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
        public Guid ItemId { get; set; }
        public Guid? BagId { get; set; }
        public int Quantity { get; set; }
        public int[] StatBuffs { get; set; }

        public InventoryUpdatePacket(int slot, Guid id, int quantity, Guid? bagId, int[] statBuffs)
        {
            Slot = slot;
            ItemId = id;
            BagId = bagId;
            Quantity = quantity;
            StatBuffs = statBuffs;
        }
    }
}
