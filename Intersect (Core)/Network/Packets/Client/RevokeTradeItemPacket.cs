using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class RevokeTradeItemPacket : SlotQuantityPacket
    {
        //Parameterless Constructor for MessagePack
        public RevokeTradeItemPacket() : base(0,0)
        {
        }

        public RevokeTradeItemPacket(int slot, int quantity) : base(slot, quantity)
        {
        }

    }

}
