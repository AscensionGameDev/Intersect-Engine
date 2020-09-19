using System;

namespace Intersect.Network.Packets.Server
{

    public class GuildInvitePacket : CerasPacket
    {

        public GuildInvitePacket(string inviter, string guildName)
        {
            Inviter = inviter;
            GuildName = guildName;
        }

        public string Inviter { get; set; }

        public string GuildName { get; set; }

    }

}