namespace Intersect.Network.Packets.Client
{

    public class RetrieveBagItemPacket : SlotQuantityPacket
    {

        public RetrieveBagItemPacket(int slot, int quantity) : base(slot, quantity)
        {
        }

    }

}
