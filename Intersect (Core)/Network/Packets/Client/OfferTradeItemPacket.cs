using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class OfferTradeItemPacket : SlotQuantityPacket
    {
        //Parameterless Constructor for MessagePack
        public OfferTradeItemPacket() : base(0, 0)
        {
        }

        public OfferTradeItemPacket(int slot, int quantity) : base(slot, quantity)
        {
        }

    }

}
