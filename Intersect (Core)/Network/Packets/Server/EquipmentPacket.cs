using System;

namespace Intersect.Network.Packets.Server
{

    public class EquipmentPacket : CerasPacket
    {

        //We send item ids for the equipment of others, but we send the correlating inventory slots for wearers because we want the equipped icons to show in the inventory.
        public EquipmentPacket(Guid entityId, int[] invSlots, Guid[] itemIds)
        {
            EntityId = entityId;
            InventorySlots = invSlots;
            ItemIds = itemIds;
        }

        public Guid EntityId { get; set; }

        public int[] InventorySlots { get; set; }

        public Guid[] ItemIds { get; set; }

    }

}
