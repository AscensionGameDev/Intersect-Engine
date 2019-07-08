using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Intersect.Admin.Actions;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Networking;

namespace Intersect.Server.Classes.Admin.Actions
{
    public static class ActionProcessing
    {
        //BanAction
        public static void ProcessAction(Client client, Player player, BanAction action) 
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                if (action.BanIp == true)
                {
                    Ban.Add(target.Client, action.DurationDays, action.Reason, player.Name, target.Client.GetIp());
                }
                else
                {
                    Ban.Add(target.Client, action.DurationDays, action.Reason, player.Name, "");
                }
                target.Client.Disconnect();
                PacketSender.SendChatMsg(client,Strings.Account.banned.ToString(target.Name),Color.Red);
            }
            else
            {
                PacketSender.SendChatMsg(client, Strings.Player.offline);
            }
        }

        //KickAction
        public static void ProcessAction(Client client, Player player, KickAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                PacketSender.SendGlobalMsg(Strings.Player.kicked.ToString(target.Name, player.Name));
                target.Client.Disconnect(); //Kick em'
            }
            else
            {
                PacketSender.SendChatMsg(client, Strings.Player.offline);
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
                PacketSender.SendChatMsg(client, Strings.Player.offline);
            }
        }

        //MuteAction
        public static void ProcessAction(Client client, Player player, MuteAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                if (action.BanIp == true)
                {
                    Mute.Add(target.Client, action.DurationDays, action.Reason, player.Name, target.Client.GetIp());
                }
                else
                {
                    Mute.Add(target.Client, action.DurationDays, action.Reason, player.Name, "");
                }

                PacketSender.SendChatMsg(client, Strings.Account.muted.ToString(target.Name), Color.Red);
            }
            else
            {
                PacketSender.SendChatMsg(client, Strings.Player.offline);
            }
        }

        //SetAccessAction
        public static void ProcessAction(Client client, Player player, SetAccessAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                if (action.Name.Trim().ToLower() != player.Name.Trim().ToLower())
                {
                    if (client.Power == UserRights.Admin)
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
                        if (targetClient.Power == UserRights.Admin)
                        {
                            PacketSender.SendGlobalMsg(Strings.Player.admin.ToString(target.Name));
                        }
                        else if (targetClient.Power == UserRights.Moderation)
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
                        PacketSender.SendChatMsg(client, Strings.Player.adminsetpower);
                    }
                }
                else
                {
                    PacketSender.SendChatMsg(client, Strings.Player.changeownpower);
                }
            }
            else
            {
                PacketSender.SendChatMsg(client, Strings.Player.offline);
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
                PacketSender.SendChatMsg(client, Strings.Player.offline);
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
                PacketSender.SendChatMsg(client, Strings.Player.offline);
            }
        }

        //UnbanAction
        public static void ProcessAction(Client client, Player player, UnbanAction action)
        {
            var unbannedUser = LegacyDatabase.GetUser(action.Name);
            if (unbannedUser != null)
            {
                Ban.Remove(unbannedUser);
                PacketSender.SendChatMsg(client, Strings.Account.unbanned.ToString(unbannedUser.Name));
            }
            else
            {
                PacketSender.SendChatMsg(client, Strings.Account.notfound.ToString(unbannedUser.Name));
            }
        }

        //UnmuteAction
        public static void ProcessAction(Client client, Player player, UnmuteAction action)
        {
            var unmutedUser = LegacyDatabase.GetUser(action.Name);
            if (unmutedUser != null)
            {
                Mute.Remove(unmutedUser);
                PacketSender.SendChatMsg(client, Strings.Account.unmuted.ToString(unmutedUser.Name));
            }
            else
            {
                PacketSender.SendChatMsg(client, Strings.Account.notfound.ToString(unmutedUser.Name));
            }
        }

        //WarpMeToAction
        public static void ProcessAction(Client client, Player player, WarpMeToAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                player.Warp(target.MapId, (byte)target.X, (byte)target.Y);
                PacketSender.SendChatMsg(client, Strings.Player.warpedto.ToString(target.Name));
                PacketSender.SendChatMsg(target.Client, Strings.Player.warpedtoyou.ToString(player.Name));
            }
            else
            {
                PacketSender.SendChatMsg(client, Strings.Player.offline);
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
            player.Warp(action.MapId, (byte)player.X, (byte)player.Y);
        }

        //WarpToMeAction
        public static void ProcessAction(Client client, Player player, WarpToMeAction action)
        {
            var target = Player.FindOnline(action.Name);
            if (target != null)
            {
                target.Warp(player.MapId, (byte)player.X, (byte)player.Y);
                PacketSender.SendChatMsg(client, Strings.Player.haswarpedto.ToString(target.Name), player.Name);
                PacketSender.SendChatMsg(target.Client, Strings.Player.beenwarpedto.ToString(player.Name), player.Name);
            }
            else
            {
                PacketSender.SendChatMsg(client, Strings.Player.offline);
            }
        }
    }
}
