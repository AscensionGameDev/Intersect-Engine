namespace Intersect.Network.Packets.Client
{

    public class OfferTradeItemPacket : SlotQuantityPacket
    {

        public OfferTradeItemPacket(int slot, int quantity) : base(slot, quantity)
        {
        }

    }

}
