namespace Intersect.Network.Packets.Client
{
    public class UnequipItemPacket : CerasPacket
    {
        public int Slot { get; set; }

        public UnequipItemPacket(int slot)
        {
            Slot = slot;
        }
    }
}
