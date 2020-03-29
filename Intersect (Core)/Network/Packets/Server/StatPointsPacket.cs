namespace Intersect.Network.Packets.Server
{

    public class StatPointsPacket : CerasPacket
    {

        public StatPointsPacket(int points)
        {
            Points = points;
        }

        public int Points { get; set; }

    }

}
