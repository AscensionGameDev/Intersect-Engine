namespace Intersect.Network.Packets.Client
{

    public class UnequipItemPacket : CerasPacket
    {

        public UnequipItemPacket(int slot)
        {
            Slot = slot;
        }

        public int Slot { get; set; }

    }

}
