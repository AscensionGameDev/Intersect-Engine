namespace Intersect.Network.Packets.Client
{
    public class UpgradeStatPacket : CerasPacket
    {
        public byte Stat { get; set; }

        public UpgradeStatPacket(byte stat)
        {
            Stat = stat;
        }
    }
}
