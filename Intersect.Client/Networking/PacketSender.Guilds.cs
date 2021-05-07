using Intersect.Network.Packets.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Client.Networking
{
    public static partial class PacketSender
    {
        public static void SendRequestGuild()
        {
            Network.SendPacket(new RequestGuildPacket());
        }

        public static void SendGuildInviteAccept(Object sender, EventArgs e)
        {
            Network.SendPacket(new GuildInviteAcceptPacket());
        }

        public static void SendGuildInviteDecline(Object sender, EventArgs e)
        {
            Network.SendPacket(new GuildInviteDeclinePacket());
        }

        public static void SendInviteGuild(string name)
        {
            Network.SendPacket(new UpdateGuildMemberPacket(Guid.Empty, name, Enums.GuildMemberUpdateActions.Invite));
        }

        public static void SendLeaveGuild()
        {
            Network.SendPacket(new GuildLeavePacket());
        }

        public static void SendKickGuildMember(Guid id)
        {
            Network.SendPacket(new UpdateGuildMemberPacket(id, null, Enums.GuildMemberUpdateActions.Remove));
        }
        public static void SendPromoteGuildMember(Guid id, int rank)
        {
            Network.SendPacket(new UpdateGuildMemberPacket(id, null, Enums.GuildMemberUpdateActions.Promote, rank));
        }

        public static void SendDemoteGuildMember(Guid id, int rank)
        {
            Network.SendPacket(new UpdateGuildMemberPacket(id, null, Enums.GuildMemberUpdateActions.Demote, rank));
        }

        public static void SendTransferGuild(Guid id)
        {
            Network.SendPacket(new UpdateGuildMemberPacket(id, null, Enums.GuildMemberUpdateActions.Transfer));
        }
    }
}
