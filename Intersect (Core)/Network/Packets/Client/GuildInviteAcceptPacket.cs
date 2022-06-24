using MessagePack;

namespace Intersect.Network.Packets.Client
{
    [MessagePackObject]
    public partial class GuildInviteAcceptPacket : IntersectPacket
    {
        /// <summary>
        /// Parameterless Constructor for MessagePack
        /// </summary>
        public GuildInviteAcceptPacket()
        {

        }
    }
}
