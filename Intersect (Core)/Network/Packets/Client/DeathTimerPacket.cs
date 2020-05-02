namespace Intersect.Network.Packets.Client
{

    public class DeathTimerPacket : CerasPacket
    {

        public DeathTimerPacket(int sec)
        {
            Sec = sec;
        }

        public int Sec { get; set; }
    }

}
