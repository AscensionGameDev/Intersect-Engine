namespace Intersect.Network.Packets.Client
{

    public class DepositItemPacket : SlotQuantityPacket
    {

        public DepositItemPacket(int slot, int quantity) : base(slot, quantity)
        {
        }

    }

}
