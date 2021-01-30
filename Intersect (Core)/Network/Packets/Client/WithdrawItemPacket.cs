using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class WithdrawItemPacket : SlotQuantityPacket
    {
        //Parameterless Constructor for MessagePack
        public WithdrawItemPacket() : base(0, 0)
        {
        }

        public WithdrawItemPacket(int slot, int quantity) : base(slot, quantity)
        {
        }

    }

}
