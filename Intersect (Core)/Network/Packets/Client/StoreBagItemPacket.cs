using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class StoreBagItemPacket : SlotQuantityPacket
    {
        //Parameterless Constructor for MessagePack
        public StoreBagItemPacket() : base(0, 0)
        {
        }
        public StoreBagItemPacket(int invSlot, int quantity, int bagSlot) : base(invSlot, quantity)
        {
            BagSlot = bagSlot;
        }

        [Key(0)]
        public int BagSlot { get; set; }


    }

}
