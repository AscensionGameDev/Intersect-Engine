using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class SwapSpellsPacket : SlotSwapPacket
    {
        //Parameterless Constructor for MessagePack
        public SwapSpellsPacket() : base(0, 0)
        {
        }

        public SwapSpellsPacket(int slot1, int slot2) : base(slot1, slot2) { }

    }

}
