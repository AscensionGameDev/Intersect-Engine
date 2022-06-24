using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public partial class DepositItemPacket : SlotQuantityPacket
    {

        //Parameterless Constructor for MessagePack
        public DepositItemPacket() : base(0, 0)
        {
        }

        public DepositItemPacket(int slot, int quantity, int bankSlot = -1) : base(slot, quantity)
        {
            BankSlot = bankSlot;
        }

        [Key(0)]
        public int BankSlot { get; set; }
    }

}
