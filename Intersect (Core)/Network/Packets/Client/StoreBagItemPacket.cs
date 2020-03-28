namespace Intersect.Network.Packets.Client
{

    public class StoreBagItemPacket : SlotQuantityPacket
    {

        public StoreBagItemPacket(int slot, int quantity) : base(slot, quantity)
        {
        }

    }

}
