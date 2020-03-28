namespace Intersect.Network.Packets.Server
{

    public class InventoryPacket : CerasPacket
    {

        public InventoryPacket(InventoryUpdatePacket[] slots)
        {
            Slots = slots;
        }

        public InventoryUpdatePacket[] Slots { get; set; }

    }

}
