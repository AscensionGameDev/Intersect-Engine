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
        public StoreBagItemPacket(int invSlot, int quantity) : base(invSlot, quantity)
        {

        }


    }

}
