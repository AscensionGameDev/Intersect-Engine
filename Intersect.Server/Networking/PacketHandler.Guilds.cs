using Intersect.Enums;
using Intersect.Network.Packets.Client;
using Intersect.Network.Packets.Server;
using Intersect.Server.Entities;
using Intersect.Server.Localization;
using System;
using System.Linq;

namespace Intersect.Server.Networking
{
    internal sealed partial class PacketHandler
    {
        //RequestGuildPacket
        public void HandlePacket(Client client, RequestGuildPacket packet)
        {
            var player = client.Entity;
            if (player == null)
            {
                return;
            }

            PacketSender.SendGuild(player);
        }


        //UpdateGuildMemberPacket
        public void HandlePacket(Client client, UpdateGuildMemberPacket packet)
        {
            var player = client.Entity;
            if (player == null)
            {
                return;
            }

            var guild = player.Guild;

            // Are we in a guild?
            if (guild == null)
            {
                PacketSender.SendChatMsg(player, Strings.Guilds.NotInGuild, ChatMessageType.Guild, CustomColors.Alerts.Error);
                return;
            }

            var isOwner = player.GuildRank == 0;
            var rank = Options.Instance.Guild.Ranks[Math.Max(0, Math.Min(player.GuildRank, Options.Instance.Guild.Ranks.Length - 1))];
            GuildMember member = null;

            // Handle our desired action, assuming we're allowed to of course.
            switch (packet.Action)
            {
                case GuildMemberUpdateActions.Invite:
                    // Are we allowed to invite players?
                    var inviteRankIndex = Options.Instance.Guild.Ranks.Length - 1;
                    var inviteRank = Options.Instance.Guild.Ranks[inviteRankIndex];
                    if (!rank.Permissions.Invite)
                    {
                        PacketSender.SendChatMsg(player, Strings.Guilds.NotAllowed, ChatMessageType.Guild, CustomColors.Alerts.Error);
                        return;
                    }

                    if (inviteRank.Limit > -1 && guild.Members.Where(m => m.Value.Rank == inviteRankIndex).Count() >= inviteRank.Limit)
                    {
                        PacketSender.SendChatMsg(player, Strings.Guilds.RankLimitResponse.ToString(inviteRank.Title, player.Name), ChatMessageType.Guild, CustomColors.Alerts.Error);
                        return;
                    }

                    // Does our target player exist online?
                    var target = Player.FindOnline(packet.Name);
                    if (target != null)
                    {
                        target.SendGuildInvite(player);
                    }
                    else
                    {
                        PacketSender.SendChatMsg(player, Strings.Guilds.InviteNotOnline, ChatMessageType.Guild, CustomColors.Alerts.Error);
                    }
                    break;
                case GuildMemberUpdateActions.Remove:
                    if (guild.Members.TryGetValue(packet.Id, out member))
                    {
                        if ((!rank.Permissions.Kick && !isOwner) || member.Rank <= player.GuildRank)
                        {
                            PacketSender.SendChatMsg(player, Strings.Guilds.NotAllowed, ChatMessageType.Guild, CustomColors.Alerts.Error);
                            return;
                        }

                        // Start common events for all online guild members that this one left
                        foreach (var mem in guild.FindOnlineMembers())
                        {
                            mem.StartCommonEventsWithTrigger(CommonEventTrigger.GuildMemberKicked, guild.Name, member.Name);
                        }

                        guild.RemoveMember(Player.Find(packet.Id));
                    }
                    else
                    {
                        PacketSender.SendChatMsg(player, Strings.Guilds.NoSuchPlayer, ChatMessageType.Guild, CustomColors.Alerts.Error);
                    }
                    break;
                case GuildMemberUpdateActions.Promote:
                    if (guild.Members.TryGetValue(packet.Id, out member))
                    {
                        var promotionRankIndex = Math.Max(0, Math.Min(packet.Rank, Options.Instance.Guild.Ranks.Length - 1));
                        var promotionRank = Options.Instance.Guild.Ranks[promotionRankIndex];
                        if ((!rank.Permissions.Promote && !isOwner) || member.Rank <= player.GuildRank || packet.Rank <= player.GuildRank || packet.Rank > member.Rank)
                        {
                            PacketSender.SendChatMsg(player, Strings.Guilds.NotAllowed, ChatMessageType.Guild, CustomColors.Alerts.Error);
                            return;
                        }

                        if (promotionRank.Limit > -1 && guild.Members.Where(m => m.Value.Rank == promotionRankIndex).Count() >= promotionRank.Limit)
                        {
                            PacketSender.SendChatMsg(player, Strings.Guilds.RankLimitResponse.ToString(promotionRank.Title, player.Name), ChatMessageType.Guild, CustomColors.Alerts.Error);
                            return;
                        }

                        guild.SetPlayerRank(packet.Id, packet.Rank);

                        PacketSender.SendGuildMsg(player, Strings.Guilds.Promoted.ToString(member.Name, promotionRank.Title), CustomColors.Alerts.Success);
                    }
                    else
                    {
                        PacketSender.SendChatMsg(player, Strings.Guilds.NoSuchPlayer, ChatMessageType.Guild, CustomColors.Alerts.Error);
                    }
                    break;
                case GuildMemberUpdateActions.Demote:
                    if (guild.Members.TryGetValue(packet.Id, out member))
                    {
                        var demotionRankIndex = Math.Max(0, Math.Min(packet.Rank, Options.Instance.Guild.Ranks.Length - 1));
                        var demotionRank = Options.Instance.Guild.Ranks[demotionRankIndex];
                        if ((!rank.Permissions.Demote && !isOwner) || member.Rank <= player.GuildRank || packet.Rank <= player.GuildRank || packet.Rank < member.Rank)
                        {
                            PacketSender.SendChatMsg(player, Strings.Guilds.NotAllowed, ChatMessageType.Guild, CustomColors.Alerts.Error);
                            return;
                        }

                        if (demotionRank.Limit > -1 && guild.Members.Where(m => m.Value.Rank == demotionRankIndex).Count() >= demotionRank.Limit)
                        {
                            PacketSender.SendChatMsg(player, Strings.Guilds.RankLimitResponse.ToString(demotionRank.Title, player.Name), ChatMessageType.Guild, CustomColors.Alerts.Error);
                            return;
                        }

                        guild.SetPlayerRank(packet.Id, packet.Rank);

                        PacketSender.SendGuildMsg(player, Strings.Guilds.Demoted.ToString(member.Name, demotionRank.Title), CustomColors.Alerts.Error);
                    }
                    else
                    {
                        PacketSender.SendChatMsg(player, Strings.Guilds.NoSuchPlayer, ChatMessageType.Guild, CustomColors.Alerts.Error);
                    }
                    break;
                case GuildMemberUpdateActions.Transfer:
                    if (guild.Members.TryGetValue(packet.Id, out member))
                    {
                        if (!isOwner)
                        {
                            PacketSender.SendChatMsg(player, Strings.Guilds.NotAllowed, ChatMessageType.Guild, CustomColors.Alerts.Error);
                            return;
                        }

                        guild.TransferOwnership(player, Player.Find(packet.Id));

                        PacketSender.SendGuildMsg(player, Strings.Guilds.Transferred.ToString(guild.Name, player.Name, member.Name), CustomColors.Alerts.Success);
                    }
                    else
                    {
                        PacketSender.SendChatMsg(player, Strings.Guilds.NoSuchPlayer, ChatMessageType.Guild, CustomColors.Alerts.Error);
                    }
                    break;
                default:
                    /// ???
                    break;
            }
        }

        //GuildInviteAcceptPacket
        public void HandlePacket(Client client, GuildInviteAcceptPacket packet)
        {
            var player = client.Entity;
            if (player == null)
            {
                return;
            }

            var guild = player?.GuildInvite?.Item2;

            // Have we received an invite at all?
            if (guild == null || player.GuildInvite == null)
            {
                PacketSender.SendChatMsg(player, Strings.Guilds.NotReceivedInvite, ChatMessageType.Guild, CustomColors.Alerts.Error);
                return;
            }

            if (Options.Instance.Guild.Ranks[Options.Instance.Guild.Ranks.Length - 1].Limit > -1 && guild.Members.Where(m => m.Value.Rank == Options.Instance.Guild.Ranks.Length - 1).Count() >= Options.Instance.Guild.Ranks[Options.Instance.Guild.Ranks.Length - 1].Limit)
            {
                // Inform the inviter that the guild is full
                if (player.GuildInvite.Item1 != null)
                {
                    var onlinePlayer = Player.FindOnline(player.GuildInvite.Item1.Id);
                    if (onlinePlayer != null)
                    {
                        PacketSender.SendChatMsg(onlinePlayer, Strings.Guilds.RankLimitResponse.ToString(Options.Instance.Guild.Ranks[Options.Instance.Guild.Ranks.Length - 1].Title, player.Name), ChatMessageType.Guild, CustomColors.Alerts.Error);
                    }
                }

                //Inform the acceptor that they are actually not in the guild
                PacketSender.SendChatMsg(player, Strings.Guilds.RankLimit.ToString(player.GuildInvite.Item2.Name), ChatMessageType.Guild, CustomColors.Alerts.Error);

                player.GuildInvite = null;

                return;
            }

            // Accept our invite!
            guild.AddMember(player, Options.Instance.Guild.Ranks.Length - 1);
            player.GuildInvite = null;

            // Start common events for all online guild members that this one left
            foreach (var member in guild.FindOnlineMembers())
            {
                member.StartCommonEventsWithTrigger(CommonEventTrigger.GuildMemberJoined, guild.Name, player.Name);
            }

            // Send the updated data around.
            PacketSender.SendEntityDataToProximity(player);
        }

        //GuildInviteDeclinePacket
        public void HandlePacket(Client client, GuildInviteDeclinePacket packet)
        {
            var player = client.Entity;
            if (player == null)
            {
                return;
            }

            // Have we received an invite at all?
            if (player.GuildInvite == null)
            {
                PacketSender.SendChatMsg(player, Strings.Guilds.NotReceivedInvite, ChatMessageType.Guild, CustomColors.Alerts.Error);
                return;
            }

            // Politely decline our invite.
            if (player.GuildInvite.Item1 != null)
            {
                var onlinePlayer = Player.FindOnline(player.GuildInvite.Item1.Id);
                if (onlinePlayer != null)
                {
                    PacketSender.SendChatMsg(onlinePlayer, Strings.Guilds.InviteDeclinedResponse.ToString(player.Name, player.GuildInvite.Item2.Name), ChatMessageType.Guild, CustomColors.Alerts.Info);
                    PacketSender.SendChatMsg(player, Strings.Guilds.InviteDeclined.ToString(onlinePlayer.Name, player.GuildInvite.Item2.Name), ChatMessageType.Guild, CustomColors.Alerts.Info);
                }
                else
                {
                    PacketSender.SendChatMsg(player, Strings.Guilds.InviteDeclined.ToString(player.GuildInvite.Item2.Name), ChatMessageType.Guild, CustomColors.Alerts.Info);
                }

                player.GuildInvite = null;
            }
        }

        //GuildLeavePacket
        public void HandlePacket(Client client, GuildLeavePacket packet)
        {
            var player = client.Entity;
            if (player == null)
            {
                return;
            }

            var guild = player.Guild;

            // Are we in a guild at all?
            if (guild == null)
            {
                return;
            }

            // Are we the guild master? If so, they're not allowed to leave.
            if (player.GuildRank == 0)
            {
                PacketSender.SendChatMsg(player, Strings.Guilds.GuildLeaderLeave, ChatMessageType.Guild, CustomColors.Alerts.Error);
                return;
            }

            // Start common events for all online guild members that this one left
            foreach (var member in guild.FindOnlineMembers())
            {
                member.StartCommonEventsWithTrigger(CommonEventTrigger.GuildMemberLeft, guild.Name, player.Name);
            }

            guild.RemoveMember(player);

            // Send the newly updated player information to their surroundings.
            PacketSender.SendEntityDataToProximity(player);

        }



    }
}
