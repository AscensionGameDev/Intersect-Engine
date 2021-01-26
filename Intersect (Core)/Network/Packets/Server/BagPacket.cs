using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class BagPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public BagPacket()
        {
        }

        public BagPacket(int slots, bool close)
        {
            Slots = slots;
            Close = close;
        }

        [Key(0)]
        public int Slots { get; set; }

        [Key(1)]
        public bool Close { get; set; }

    }

}
