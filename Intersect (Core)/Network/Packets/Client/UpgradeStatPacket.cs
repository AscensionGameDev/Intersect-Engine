using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class UpgradeStatPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public UpgradeStatPacket()
        {
        }

        public UpgradeStatPacket(byte stat)
        {
            Stat = stat;
        }

        [Key(0)]
        public byte Stat { get; set; }

    }

}
