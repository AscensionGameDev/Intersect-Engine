using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class SwapBankItemsPacket : SlotSwapPacket
    {
        //Parameterless Constructor for MessagePack
        public SwapBankItemsPacket() : base(0, 0)
        {
        }

        public SwapBankItemsPacket(int slot1, int slot2) : base(slot1, slot2) { }

    }

}
