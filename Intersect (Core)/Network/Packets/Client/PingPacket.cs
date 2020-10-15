namespace Intersect.Network.Packets.Client
{

    public class PingPacket : AbstractTimedPacket
    {
        public bool Responding { get; set; }
    }

}
