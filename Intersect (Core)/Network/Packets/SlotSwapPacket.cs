using Intersect.Network.Packets.Client;
using MessagePack;

namespace Intersect.Network.Packets
{
    [MessagePackObject]
    [Union(0, typeof(HotbarSwapPacket))]
    [Union(1, typeof(SwapBagItemsPacket))]
    [Union(2, typeof(SwapBankItemsPacket))]
    [Union(3, typeof(SwapInvItemsPacket))]
    [Union(4, typeof(SwapSpellsPacket))]
    public abstract class SlotSwapPacket : IntersectPacket
    {

        protected SlotSwapPacket(int slot1, int slot2)
        {
            Slot1 = slot1;
            Slot2 = slot2;
        }

        [Key(0)]
        public int Slot1 { get; set; }

        [Key(1)]
        public int Slot2 { get; set; }

        [Key(2)]
        public override bool IsValid => Slot1 != Slot2 && Slot1 >= 0 && Slot2 >= 0;

    }

}
