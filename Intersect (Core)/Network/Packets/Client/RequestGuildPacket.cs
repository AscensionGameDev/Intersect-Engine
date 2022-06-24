using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public partial class RequestGuildPacket : IntersectPacket
    {
        /// <summary>
        /// Parameterless Constructor for MessagePack
        /// </summary>
        public RequestGuildPacket()
        {

        }
    }
}
