using System;

namespace Intersect.Network.Packets.Server
{

    public class InventoryUpdatePacket : CerasPacket
    {

        public InventoryUpdatePacket(int slot, Guid id, int quantity, Guid? bagId, int[] statBuffs)
        {
            Slot = slot;
            ItemId = id;
            BagId = bagId;
            Quantity = quantity;
            StatBuffs = statBuffs;
        }

        public int Slot { get; set; }

        public Guid ItemId { get; set; }

        public Guid? BagId { get; set; }

        public int Quantity { get; set; }

        public int[] StatBuffs { get; set; }

    }

}
