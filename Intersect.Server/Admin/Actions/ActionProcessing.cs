
using Intersect.Admin.Actions;
using Intersect.Server.Database;
using Intersect.Server.Database.Logging.Entities;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.Localization;
using Intersect.Server.Networking;
using System;

namespace Intersect.Server.Admin.Actions
{

    public static class ActionProcessing
    {

        //BanAction
        public static void ProcessAction(Client client, Player player, BanAction action)
        {
            var target = Player.Find(action.Name);
            if (target != null)
            {
                if (string.IsNullOrEmpty(Ban.CheckBan(target.User, "")))
                {
                    if (action.BanIp == true)
                    {
                        Ban.Add(
                            target.User, action.DurationDays, action.Reason, player.Name, target.Client?.GetIp() ?? ""
                        );
                    }
                    else
                    {
                        Ban.Add(target.User, action.DurationDays, action.Reason, player.Name, "");
                    }

                    UserActivityHistory.LogActivity(target?.UserId ?? Guid.Empty, target?.Id ?? Guid.Empty, target?.Client?.GetIp(), UserActivityHistory.PeerType.Client, UserActivityHistory.UserAction.DisconnectBan, $"{target.User?.Name},{target.Name}");

                    target.Client?.Disconnect();
                    PacketSender.SendChatMsg(player, Strings.Account.banned.ToString(target.Name), Enums.ChatMessageType.Admin, Color.Red);
                }
                else
                {
                    PacketSender.SendChatMsg(player, Strings.Account.alreadybanned.ToString(target.Name), Enums.ChatMessageType.Admin, Color.Red);
                }
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline, Enums.ChatMessageType.Admin);
            }
        }

        //KickAction
        public static void ProcessAction(Client client, Player player, KickAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                UserActivityHistory.LogActivity(target?.UserId ?? Guid.Empty, target?.Id ?? Guid.Empty, target?.Client?.GetIp(), UserActivityHistory.PeerType.Client, UserActivityHistory.UserAction.DisconnectKick, $"{target.User?.Name},{target.Name}");

                PacketSender.SendGlobalMsg(Strings.Player.kicked.ToString(target.Name, player.Name));
                target.Client?.Disconnect(); //Kick em'
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline, Enums.ChatMessageType.Admin);
            }
        }

        //KillAction
        public static void ProcessAction(Client client, Player player, KillAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                lock (target.EntityLock)
                {
                    target.Die(); //Kill em'
                }
                
                PacketSender.SendGlobalMsg(Strings.Player.killed.ToString(target.Name, player.Name));
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline, Enums.ChatMessageType.Admin);
            }
        }

        //MuteAction
        public static void ProcessAction(Client client, Player player, MuteAction action)
        {
            var target = Player.Find(action.Name);
            if (target != null)
            {
                if (string.IsNullOrEmpty(Mute.FindMuteReason(target.UserId, "")))
                {
                    if (action.BanIp == true)
                    {
                        Mute.Add(
                            target.User, action.DurationDays, action.Reason, player.Name, target.Client?.GetIp() ?? ""
                        );
                    }
                    else
                    {
                        Mute.Add(target.User, action.DurationDays, action.Reason, player.Name, "");
                    }

                    PacketSender.SendChatMsg(player, Strings.Account.muted.ToString(target.Name), Enums.ChatMessageType.Admin, Color.Red);
                }
                else
                {
                    PacketSender.SendChatMsg(player, Strings.Account.alreadymuted.ToString(target.Name), Enums.ChatMessageType.Admin, Color.Red);
                }
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline, Enums.ChatMessageType.Admin);
            }
        }

        //SetAccessAction
        public static void ProcessAction(Client client, Player player, SetAccessAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (client == null || target == null || target.Client == null)
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline, Enums.ChatMessageType.Admin);

                return;
            }

            if (action.Name.Trim().ToLower() != player.Name.Trim().ToLower())
            {
                if (client.Power.IsAdmin)
                {
                    var power = UserRights.None;
                    if (action.Power == "Admin")
                    {
                        power = UserRights.Admin;
                    }
                    else if (action.Power == "Moderator")
                    {
                        power = UserRights.Moderation;
                    }

                    var targetClient = target.Client;
                    targetClient.Power = power;
                    if (targetClient.Power.IsAdmin)
                    {
                        PacketSender.SendGlobalMsg(Strings.Player.admin.ToString(target.Name));
                    }
                    else if (targetClient.Power.IsModerator)
                    {
                        PacketSender.SendGlobalMsg(Strings.Player.mod.ToString(target.Name));
                    }
                    else
                    {
                        PacketSender.SendGlobalMsg(Strings.Player.deadmin.ToString(target.Name));
                    }
                }
                else
                {
                    PacketSender.SendChatMsg(player, Strings.Player.adminsetpower, Enums.ChatMessageType.Admin);
                }
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.changeownpower, Enums.ChatMessageType.Admin);
            }
        }

        //SetFaceAction
        public static void ProcessAction(Client client, Player player, SetFaceAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                target.Face = action.Face;
                PacketSender.SendEntityDataToProximity(target);
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline, Enums.ChatMessageType.Admin);
            }
        }

        //SetSpriteAction
        public static void ProcessAction(Client client, Player player, SetSpriteAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                target.Sprite = action.Sprite;
                PacketSender.SendEntityDataToProximity(target);
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline, Enums.ChatMessageType.Admin);
            }
        }

        //UnbanAction
        public static void ProcessAction(Client client, Player player, UnbanAction action)
        {
            var unbannedUser = User.Find(action.Name);
            if (unbannedUser != null)
            {
                Ban.Remove(unbannedUser);
                PacketSender.SendChatMsg(player, Strings.Account.unbanned.ToString(unbannedUser.Name), Enums.ChatMessageType.Admin);
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Account.notfound.ToString(action.Name), Enums.ChatMessageType.Admin);
            }
        }

        //UnmuteAction
        public static void ProcessAction(Client client, Player player, UnmuteAction action)
        {
            var unmutedUser = User.Find(action.Name);
            if (unmutedUser != null)
            {
                Mute.Remove(unmutedUser);
                PacketSender.SendChatMsg(player, Strings.Account.unmuted.ToString(unmutedUser.Name), Enums.ChatMessageType.Admin);
            }
            else
            {
                var target = Player.FindOnline(action.Name);
                if (target != null)
                {
                    Mute.Remove(target.User);
                    PacketSender.SendChatMsg(player, Strings.Account.unmuted.ToString(target.Name), Enums.ChatMessageType.Admin);
                }
                else
                {
                    PacketSender.SendChatMsg(player, Strings.Account.notfound.ToString(action.Name), Enums.ChatMessageType.Admin);
                }
            }
        }

        //WarpMeToAction
        public static void ProcessAction(Client client, Player player, WarpMeToAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                player.Warp(target.MapId, (byte) target.X, (byte) target.Y);
                PacketSender.SendChatMsg(player, Strings.Player.warpedto.ToString(target.Name), Enums.ChatMessageType.Admin);
                PacketSender.SendChatMsg(target, Strings.Player.warpedtoyou.ToString(player.Name), Enums.ChatMessageType.Notice);
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline, Enums.ChatMessageType.Admin);
            }
        }

        //WarpToLocationAction
        public static void ProcessAction(Client client, Player player, WarpToLocationAction action)
        {
            player.Warp(action.MapId, action.X, action.Y, 0, true);
        }

        //WarpToMapAction
        public static void ProcessAction(Client client, Player player, WarpToMapAction action)
        {
            player.Warp(action.MapId, (byte) player.X, (byte) player.Y);
        }

        //WarpToMeAction
        public static void ProcessAction(Client client, Player player, WarpToMeAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                target.Warp(player.MapId, (byte) player.X, (byte) player.Y);
                PacketSender.SendChatMsg(player, Strings.Player.haswarpedto.ToString(target.Name), Enums.ChatMessageType.Admin, player.Name);
                PacketSender.SendChatMsg(target, Strings.Player.beenwarpedto.ToString(player.Name), Enums.ChatMessageType.Notice, player.Name);
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline, Enums.ChatMessageType.Admin);
            }
        }

    }

}
