namespace Intersect.Network.Packets.Client
{

    public class SellItemPacket : SlotQuantityPacket
    {

        public SellItemPacket(int slot, int quantity) : base(slot, quantity)
        {
        }

    }

}
