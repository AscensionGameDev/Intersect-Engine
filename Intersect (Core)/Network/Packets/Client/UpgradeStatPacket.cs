namespace Intersect.Network.Packets.Client
{

    public class UpgradeStatPacket : CerasPacket
    {

        public UpgradeStatPacket(byte stat)
        {
            Stat = stat;
        }

        public byte Stat { get; set; }

    }

}
