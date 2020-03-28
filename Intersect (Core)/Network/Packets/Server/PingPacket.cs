namespace Intersect.Network.Packets.Server
{

    public class PingPacket : CerasPacket
    {

        public PingPacket(bool requestingReply)
        {
            RequestingReply = requestingReply;
        }

        public bool RequestingReply { get; set; }

    }

}
