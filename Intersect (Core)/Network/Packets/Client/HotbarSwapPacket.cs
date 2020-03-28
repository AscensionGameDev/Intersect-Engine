namespace Intersect.Network.Packets.Client
{

    public class HotbarSwapPacket : SlotSwapPacket
    {

        public HotbarSwapPacket(int slot1, int slot2) : base(slot1, slot2) { }

    }

}
