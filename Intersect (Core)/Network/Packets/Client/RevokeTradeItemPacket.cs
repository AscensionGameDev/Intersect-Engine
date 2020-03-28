namespace Intersect.Network.Packets.Client
{

    public class RevokeTradeItemPacket : SlotQuantityPacket
    {

        public RevokeTradeItemPacket(int slot, int quantity) : base(slot, quantity)
        {
        }

    }

}
