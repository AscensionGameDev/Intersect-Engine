namespace Intersect.Network.Packets.Server
{

    public class BankPacket : CerasPacket
    {

        public BankPacket(bool close)
        {
            Close = close;
        }

        public bool Close { get; set; }

    }

}
