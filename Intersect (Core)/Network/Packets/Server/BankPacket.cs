using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class BankPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public BankPacket()
        {
        }
        public BankPacket(bool close)
        {
            Close = close;
        }

        [Key(0)]
        public bool Close { get; set; }

    }

}
