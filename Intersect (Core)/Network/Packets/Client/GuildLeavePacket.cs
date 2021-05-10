using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public class GuildLeavePacket : IntersectPacket
    {
        /// <summary>
        /// Parameterless Constructor for MessagePack
        /// </summary>
        public GuildLeavePacket()
        {

        }
    }
}
