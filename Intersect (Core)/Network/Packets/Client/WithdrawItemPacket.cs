using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public partial class WithdrawItemPacket : SlotQuantityPacket
    {
        //Parameterless Constructor for MessagePack
        public WithdrawItemPacket() : base(0, 0)
        {
        }

        public WithdrawItemPacket(int slot, int quantity, int invSlot = -1) : base(slot, quantity)
        {
            InvSlot = invSlot;
        }

        [Key(0)]
        public int InvSlot { get; set; }

    }

}
