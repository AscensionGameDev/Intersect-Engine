namespace Intersect.Network.Packets.Server
{

    public class BagPacket : CerasPacket
    {

        public BagPacket(int slots, bool close)
        {
            Slots = slots;
            Close = close;
        }

        public int Slots { get; set; }

        public bool Close { get; set; }

    }

}
