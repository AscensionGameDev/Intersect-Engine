using Intersect.Enums;
using Intersect.Network.Packets.Server;
using Intersect.Server.Entities;
using Intersect.Server.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Server.Networking
{
    public static partial class PacketSender
    {
        //ChatMsgPacket
        public static void SendGuildMsg(Player player, string message, Color clr, string target = "")
        {
            foreach (var p in player.Guild.FindOnlineMembers())
            {
                if (p != null)
                {
                    SendChatMsg(p, message, ChatMessageType.Guild, clr, target);
                }
            }
        }

        /// <summary>
        /// Send a player their guild member list.
        /// </summary>
        /// <param name="player"></param>
        public static void SendGuild(Player player)
        {
            if (player == null || player.Guild == null)
            {
                return;
            }

            var members = player.Guild.Members.Values.ToArray();
            var onlineMembers = player.Guild.FindOnlineMembers();

            foreach (var member in members)
            {
                member.Online = onlineMembers.Any(m => m.Id == member.Id);
            }

            player.SendPacket(new GuildPacket(members));
        }



        //GuildRequestPacket
        public static void SendGuildInvite(Player player, Player from)
        {
            player.SendPacket(new GuildInvitePacket(from.Name, from.Guild.Name));
        }

    }
}
