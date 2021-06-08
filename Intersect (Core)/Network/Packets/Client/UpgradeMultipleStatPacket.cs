using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class UpgradeMultipleStatPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public UpgradeMultipleStatPacket()
        {
        }

        public UpgradeMultipleStatPacket(byte stat)
        {
            Stat = stat;
        }

        [Key(0)]
        public byte Stat { get; set; }

    }

}
