namespace Intersect.Network.Packets.Client
{

    public class WithdrawItemPacket : SlotQuantityPacket
    {

        public WithdrawItemPacket(int slot, int quantity) : base(slot, quantity)
        {
        }

    }

}
