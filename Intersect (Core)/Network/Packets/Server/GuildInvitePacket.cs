
using MessagePack;

namespace Intersect.Network.Packets.Server
{
    [MessagePackObject]
    public class GuildInvitePacket : IntersectPacket
    {
        /// <summary>
        /// Parameterless Constructor for MessagePack
        /// </summary>
        public GuildInvitePacket()
        {

        }

        public GuildInvitePacket(string inviter, string guildName)
        {
            Inviter = inviter;
            GuildName = guildName;
        }

        [Key(0)]
        public string Inviter { get; set; }

        [Key(1)]
        public string GuildName { get; set; }
    }
}
