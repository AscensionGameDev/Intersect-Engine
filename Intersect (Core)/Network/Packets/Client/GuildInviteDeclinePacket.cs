using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class GuildInviteDeclinePacket : IntersectPacket
    {
        /// <summary>
        /// Parameterless Constructor for MessagePack
        /// </summary>
        public GuildInviteDeclinePacket()
        {

        }
    }
}
