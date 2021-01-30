using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class SwapBagItemsPacket : SlotSwapPacket
    {
        //Parameterless Constructor for MessagePack
        public SwapBagItemsPacket() : base(0, 0)
        {
        }

        public SwapBagItemsPacket(int slot1, int slot2) : base(slot1, slot2) { }

    }

}
