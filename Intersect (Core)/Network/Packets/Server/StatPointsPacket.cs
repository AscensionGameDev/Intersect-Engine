using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class StatPointsPacket : IntersectPacket
    {
        //Parameterless Constructor for MessagePack
        public StatPointsPacket()
        {
        }

        public StatPointsPacket(int points)
        {
            Points = points;
        }

        [Key(0)]
        public int Points { get; set; }

    }

}
