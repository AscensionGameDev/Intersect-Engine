﻿using System;

using Intersect.Admin.Actions;
using Intersect.Server.Database;
using Intersect.Server.Database.Logging.Entities;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Admin.Actions
{

    public static class ActionProcessing
    {

        //BanAction
        public static void ProcessAction(Client client, Player player, BanAction action)
        {
            var target = DbInterface.GetPlayer(action.Name);
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

                    using (var logging = DbInterface.LoggingContext)
                    {
                        logging.UserActivityHistory.Add(
                            new UserActivityHistory
                            {
                                UserId = target.UserId,
                                PlayerId = target.Id,
                                Ip = target.Client.GetIp(),
                                Peer = UserActivityHistory.PeerType.Client,
                                Action = UserActivityHistory.UserAction.DisconnectBan,
                                Meta = $"{target.Client.Name},{target.Name}"
                            }
                        );
                    }

                    target.Client?.Disconnect();
                    PacketSender.SendChatMsg(player, Strings.Account.banned.ToString(target.Name), Color.Red);
                }
                else
                {
                    PacketSender.SendChatMsg(player, Strings.Account.alreadybanned.ToString(target.Name), Color.Red);
                }
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline);
            }
        }

        //KickAction
        public static void ProcessAction(Client client, Player player, KickAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                using (var logging = DbInterface.LoggingContext)
                {
                    logging.UserActivityHistory.Add(
                        new UserActivityHistory
                        {
                            UserId = target.UserId,
                            PlayerId = target.Id,
                            Ip = target.Client.GetIp(),
                            Peer = UserActivityHistory.PeerType.Client,
                            Action = UserActivityHistory.UserAction.DisconnectKick,
                            Meta = $"{target.Client.Name},{target.Name}"
                        }
                    );
                }

                PacketSender.SendGlobalMsg(Strings.Player.kicked.ToString(target.Name, player.Name));
                target.Client?.Disconnect(); //Kick em'
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline);
            }
        }

        //KillAction
        public static void ProcessAction(Client client, Player player, KillAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                target.Die(); //Kill em'
                PacketSender.SendGlobalMsg(Strings.Player.killed.ToString(target.Name, player.Name));
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline);
            }
        }

        //MuteAction
        public static void ProcessAction(Client client, Player player, MuteAction action)
        {
            var target = DbInterface.GetPlayer(action.Name);
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

                    PacketSender.SendChatMsg(player, Strings.Account.muted.ToString(target.Name), Color.Red);
                }
                else
                {
                    PacketSender.SendChatMsg(player, Strings.Account.alreadymuted.ToString(target.Name), Color.Red);
                }
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline);
            }
        }

        //SetAccessAction
        public static void ProcessAction(Client client, Player player, SetAccessAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (client == null || target == null || target.Client == null)
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline);

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
                    PacketSender.SendChatMsg(player, Strings.Player.adminsetpower);
                }
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.changeownpower);
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
                PacketSender.SendChatMsg(player, Strings.Player.offline);
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
                PacketSender.SendChatMsg(player, Strings.Player.offline);
            }
        }

        //UnbanAction
        public static void ProcessAction(Client client, Player player, UnbanAction action)
        {
            var unbannedUser = DbInterface.GetUser(action.Name);
            if (unbannedUser != null)
            {
                Ban.Remove(unbannedUser);
                PacketSender.SendChatMsg(player, Strings.Account.unbanned.ToString(unbannedUser.Name));
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Account.notfound.ToString(action.Name));
            }
        }

        //UnmuteAction
        public static void ProcessAction(Client client, Player player, UnmuteAction action)
        {
            var unmutedUser = DbInterface.GetUser(action.Name);
            if (unmutedUser != null)
            {
                Mute.Remove(unmutedUser);
                PacketSender.SendChatMsg(player, Strings.Account.unmuted.ToString(unmutedUser.Name));
            }
            else
            {
                var target = Player.FindOnline(action.Name);
                if (target != null)
                {
                    Mute.Remove(target.User);
                    PacketSender.SendChatMsg(player, Strings.Account.unmuted.ToString(target.Name));
                }
                else
                {
                    PacketSender.SendChatMsg(player, Strings.Account.notfound.ToString(action.Name));
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
                PacketSender.SendChatMsg(player, Strings.Player.warpedto.ToString(target.Name));
                PacketSender.SendChatMsg(target, Strings.Player.warpedtoyou.ToString(player.Name));
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline);
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
                PacketSender.SendChatMsg(player, Strings.Player.haswarpedto.ToString(target.Name), player.Name);
                PacketSender.SendChatMsg(target, Strings.Player.beenwarpedto.ToString(player.Name), player.Name);
            }
            else
            {
                PacketSender.SendChatMsg(player, Strings.Player.offline);
            }
        }

    }

}
