namespace Intersect.Network.Packets.Client
{

    public class BuyItemPacket : SlotQuantityPacket
    {

        public BuyItemPacket(int slot, int quantity) : base(slot, quantity)
        {
        }

    }

}
