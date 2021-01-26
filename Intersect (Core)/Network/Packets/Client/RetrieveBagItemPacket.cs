using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class RetrieveBagItemPacket : SlotQuantityPacket
    {
        //Parameterless Constructor for MessagePack
        public RetrieveBagItemPacket() : base(0, 0)
        {
        }

        public RetrieveBagItemPacket(int bagSlot, int quantity) : base(bagSlot, quantity)
        {

        }

    }

}
