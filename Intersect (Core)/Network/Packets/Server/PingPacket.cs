using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class PingPacket : AbstractTimedPacket
    {
        //Parameterless Constructor for MessagePack
        public PingPacket()
        {

        }

        public PingPacket(bool requestingReply)
        {
            RequestingReply = requestingReply;
        }

        [Key(3)]
        public bool RequestingReply { get; set; }

    }

}
