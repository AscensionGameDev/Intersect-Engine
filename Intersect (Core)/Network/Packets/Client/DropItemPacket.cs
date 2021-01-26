using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class DropItemPacket : SlotQuantityPacket
    {
        //Parameterless Constructor for MessagePack
        public DropItemPacket() : base(0, 0)
        {
        }

        public DropItemPacket(int slot, int quantity) : base(slot, quantity)
        {
        }

    }

}
