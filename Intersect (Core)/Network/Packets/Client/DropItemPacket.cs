namespace Intersect.Network.Packets.Client
{

    public class DropItemPacket : SlotQuantityPacket
    {

        public DropItemPacket(int slot, int quantity) : base(slot, quantity)
        {
        }

    }

}
