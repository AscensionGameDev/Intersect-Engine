using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class HotbarSwapPacket : SlotSwapPacket
    {
        //Parameterless Constructor for MessagePack
        public HotbarSwapPacket() : base(0,0)
        {
        }

        public HotbarSwapPacket(int slot1, int slot2) : base(slot1, slot2) { }

    }

}
