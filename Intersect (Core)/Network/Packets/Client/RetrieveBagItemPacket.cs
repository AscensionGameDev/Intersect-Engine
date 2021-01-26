using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class RetrieveBagItemPacket : SlotQuantityPacket
    {
        //Parameterless Constructor for MessagePack
        public RetrieveBagItemPacket() : base(0, 0)
        {
        }

        public RetrieveBagItemPacket(int bagSlot, int quantity, int invSlot) : base(bagSlot, quantity)
        {
            InventorySlot = invSlot;
        }

        [Key(4)]
        public int InventorySlot { get; set; }

    }

}
