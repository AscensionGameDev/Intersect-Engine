using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intersect.Enums;
using Intersect.ErrorHandling;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Logging;
using Intersect.Models;
using Intersect.Network;
using Intersect.Network.Packets.Client;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Maps;
using Intersect.Server.Notifications;
using Intersect.Utilities;

namespace Intersect.Server.Networking
{
    using DbInterface = DbInterface;
    using Packets = Intersect.Network.Packets;

    public class PacketHandler
    {
        public bool PreProcessPacket(IConnection connection, long pSize)
        {
            var client = Client.FindBeta4Client(connection);
            if (client == null)
            {
                throw new Exception("Client is null!");
            }

            if (client.Banned || client.FloodKicked) return false;

            var threshholds = Options.Instance.SecurityOpts.PacketOpts.Threshholds;
            if (client.IsEditor != null)
            {
                //Is Editor
                threshholds = Options.Instance.SecurityOpts.PacketOpts.EditorThreshholds;
            }
            else if (client.User != null)
            {
                //Logged In
                threshholds = Options.Instance.SecurityOpts.PacketOpts.PlayerThreshholds;
            }

            if (pSize > threshholds.MaxPacketSize)
            {
                Log.Error(Strings.Errors.floodsize.ToString(pSize, client?.User?.Name ?? "", client?.Entity?.Name ?? "", client.GetIp()));
                client.FloodKicked = true;
                client.Disconnect("Flooding detected.");
                return false;
            }

            if (client.PacketTimer > Globals.Timing.TimeMs)
            {
                client.PacketCount++;
                if (client.PacketCount > threshholds.MaxPacketPerSec)
                {
                    Log.Error(Strings.Errors.floodburst.ToString(client.PacketCount, client?.User?.Name ?? "", client?.Entity?.Name ?? "", client.GetIp()));
                    client.FloodKicked = true;
                    client.Disconnect("Flooding detected.");
                    return false;
                }
                else if (client.PacketCount > threshholds.KickAvgPacketPerSec && !client.PacketFloodDetect)
                {
                    client.FloodDetects++;
                    client.TotalFloodDetects++;
                    client.PacketFloodDetect = true;

                    if (client.FloodDetects > 3)
                    {
                        Log.Error(Strings.Errors.floodaverage.ToString(client.TotalFloodDetects, client?.User?.Name ?? "", client?.Entity?.Name ?? "", client.GetIp()));
                        client.FloodKicked = true;
                        client.Disconnect("Flooding detected.");
                        return false;
                    }

                    //TODO: Make this check a rolling average somehow to prevent constant flooding right below the threshholds.
                    if (client.TotalFloodDetects > 10)
                    {
                        //Log.Error(string.Format("[Flood]: Total Detections: {00} [User: {01} | Player: {02} | IP {03}]", client.TotalFloodDetects, client?.User?.Name ?? "", client?.Entity?.Name ?? "", client.GetIp()));
                        //client.Disconnect("Flooding detected.");
                        //return false;
                    }

                }
                else if (client.PacketCount < threshholds.KickAvgPacketPerSec / 2)
                {
                    if (client.FloodDetects > 1)
                    {
                        client.FloodDetects--;
                    }
                }
            }
            else
            {
                if (client.PacketFloodDetect)
                {
                    //Log.Error(string.Format("Possible Flood Detected: Packets in last second {00} [User: {01} | Player: {02} | IP {03}]", client.PacketCount, client?.User?.Name ?? "", client?.Entity?.Name ?? "", client.GetIp()));
                }
                client.PacketCount = 0;
                client.PacketTimer = Globals.Timing.TimeMs + 1000;
                client.PacketFloodDetect = false;
            }

            return true;
        }

        public bool HandlePacket(IConnection connection, IPacket packet)
        {
            var client = Client.FindBeta4Client(connection);
            if (client == null)
            {
                throw new Exception("Client is null!");
            }

            if (client.Banned)
            {
                return false;
            }

            if (packet is Packets.EditorPacket && !client.IsEditor)
            {
                return false;
            }

            try
            {
                HandlePacket(client, client.Entity, (dynamic)packet);
            }
            catch (Exception exception)
            {
                var packetType = packet?.GetType().Name ?? "NULL_PACKET";
                var packetMessage =
                    $"Client Packet Error! [Packet: {packetType} | User: {client.User?.Name ?? ""} | Player: {client.Entity?.Name ?? ""} | IP {client.GetIp()}]";

                // TODO: Re-combine these once we figure out how to prevent the OutOfMemoryException that happens occasionally
                Log.Error(packetMessage);
                Log.Error(new ExceptionInfo(exception));
                if (exception.InnerException != null)
                {
                    Log.Error(new ExceptionInfo(exception.InnerException));
                }

                // Make the call that triggered the OOME in the first place so that we know when it stops happening
                Log.Error(exception, packetMessage);

                client.Disconnect($"Error processing packet type '{packetType}'.");
            }
            return true;
        }
        
        #region "Client Packets"
        //PingPacket
        public void HandlePacket(Client client, Player player, PingPacket packet)
        {
            client.Pinged();
            PacketSender.SendPing(client, false);
        }

        //LoginPacket
        public void HandlePacket(Client client, Player player, LoginPacket packet)
        {
            if (client.AccountAttempts > 3 && client.TimeoutMs > Globals.Timing.TimeMs)
            {
                PacketSender.SendError(client, Strings.Errors.errortimeout);
                client.ResetTimeout();
                return;
            }

            client.ResetTimeout();

            if (!DbInterface.CheckPassword(packet.Username, packet.Password))
            {
                client.FailedAttempt();
                PacketSender.SendError(client, Strings.Account.badlogin);
                return;
            }

            lock (Globals.ClientLock)
            {
                Globals.Clients.ForEach(user =>
                {
                    if (user == client) return;
                    if (user?.IsEditor ?? false) return;

                    if (!string.Equals(user?.Name, packet.Username, StringComparison.InvariantCultureIgnoreCase)) return;
                    user?.Disconnect();
                });
            }

            if (!DbInterface.LoadUser(client, packet.Username))
            {
                PacketSender.SendError(client, Strings.Account.loadfail);
                return;
            }

            //Check for ban
            var isBanned = Ban.CheckBan(client.User, client.GetIp());
            if (isBanned != null)
            {
                client.SetUser(null);
                client.Banned = true;
                PacketSender.SendError(client, isBanned);
                return;
            }

            //Check that server is in admin only mode
            if (Options.AdminOnly)
            {
                if (client.Power == UserRights.None)
                {
                    PacketSender.SendError(client, Strings.Account.adminonly);
                    return;
                }
            }

            //Check Mute Status and Load into user property
            Mute.FindMuteReason(client.User, client.GetIp());

            PacketSender.SendServerConfig(client);
            //Character selection if more than one.
            if (Options.MaxCharacters > 1)
            {
                PacketSender.SendPlayerCharacters(client);
            }
            else if (client.Characters?.Count > 0)
            {
                client.LoadCharacter(client.Characters.First());
                client.Entity.SetOnline();
                PacketSender.SendJoinGame(client);
            }
            else
            {
                PacketSender.SendGameObjects(client, GameObjectType.Class);
                PacketSender.SendCreateCharacter(client);
            }
        }

        //LogoutPacket
        public void HandlePacket(Client client, Player player, LogoutPacket packet)
        {
            client?.Logout();
            if (Options.MaxCharacters > 1 && packet.ReturningToCharSelect)
            {
                PacketSender.SendPlayerCharacters(client);
            }
        }

        //NeedMapPacket
        public void HandlePacket(Client client, Player player, NeedMapPacket packet)
        {
            var map = MapInstance.Get(packet.MapId);
            if (map != null)
            {
                PacketSender.SendMap(client, packet.MapId);
                if (player != null && packet.MapId == player.MapId)
                {
                    PacketSender.SendMapGrid(client, map.MapGrid);
                }
            }
        }

        //MovePacket
        public void HandlePacket(Client client, Player player, MovePacket packet)
        {
            if (player == null) return;
            //check if player is stunned or snared, if so don't let them move.
            var statuses = client.Entity.Statuses.Values.ToArray();
            foreach (var status in statuses)
            {
                if (status.Type == StatusTypes.Stun || status.Type == StatusTypes.Snare || status.Type == StatusTypes.Sleep)
                {
                    return;
                }
            }

            if (!TileHelper.IsTileValid(packet.MapId, packet.X, packet.Y))
            {
                //POSSIBLE HACKING ATTEMPT!
                PacketSender.SendEntityPositionTo(client, client.Entity);
                return;
            }
            var canMove = player.CanMove(packet.Dir);
            if ((canMove == -1 || canMove == -4) && client.Entity.MoveRoute == null)
            {
                player.Move(packet.Dir, client, false);
                if (player.MoveTimer > Globals.Timing.TimeMs)
                {
                    //TODO: Make this based moreso on the players current ping instead of a flat value that can be abused
                    player.MoveTimer = Globals.Timing.TimeMs + (long)(player.GetMovementTime() * .75f);
                }
            }
            else
            {
                PacketSender.SendEntityPositionTo(client, client.Entity);
                return;
            }
            if (packet.MapId != client.Entity.MapId || packet.X != client.Entity.X || packet.Y != client.Entity.Y)
            {
                PacketSender.SendEntityPositionTo(client, client.Entity);
            }
        }

        //ChatMsgPacket
        public void HandlePacket(Client client, Player player, ChatMsgPacket packet)
        {
            if (player == null) return;

            var msg = packet.Message;
            var channel = packet.Channel;
            if (client.User.IsMuted) //Don't let the toungless toxic kids speak.
            {
                PacketSender.SendChatMsg(client, client.User.Mute?.Reason);
                return;
            }

            if (player.LastChatTime > Globals.Timing.RealTimeMs)
            {
                PacketSender.SendChatMsg(client, Strings.Chat.toofast);
                player.LastChatTime = Globals.Timing.RealTimeMs + Options.MinChatInterval;
                return;
            }

            if (packet.Message.Length > Options.MaxChatLength)
                return;
            
            //If no /command, then use the designated channel.
            if (msg[0] != '/')
            {
                msg = "/" + channel + " " + msg;
            }

            player.LastChatTime = Globals.Timing.RealTimeMs + Options.MinChatInterval;

            //Check for /commands
            if (msg[0] == '/')
            {
                string[] splitString = msg.Split();
                msg = msg.Remove(0, splitString[0].Length); //Chop off the /command at the start of the sentance
                var cmd = splitString[0].ToLower();

                if (cmd == Strings.Chat.localcmd || cmd == "/0")
                {
                    if (msg.Trim().Length == 0) return;
                    if (client.Power.IsAdmin)
                    {
                        PacketSender.SendProximityMsg(Strings.Chat.local.ToString(client.Entity.Name, msg), player.MapId, CustomColors.AdminLocalChat, client.Entity.Name);
                    }
                    else if (client.Power.IsModerator)
                    {
                        PacketSender.SendProximityMsg(Strings.Chat.local.ToString(client.Entity.Name, msg), player.MapId, CustomColors.ModLocalChat, client.Entity.Name);
                    }
                    else
                    {
                        PacketSender.SendProximityMsg(Strings.Chat.local.ToString(client.Entity.Name, msg), player.MapId, CustomColors.LocalChat, client.Entity.Name);
                    }
                    PacketSender.SendChatBubble(player.Id, (int)EntityTypes.GlobalEntity, msg, player.MapId);
                }
                else if (cmd == Strings.Chat.allcmd || cmd == "/1" || cmd == Strings.Chat.globalcmd)
                {
                    if (msg.Trim().Length == 0) return;
                    if (client.Power.IsAdmin)
                    {
                        PacketSender.SendGlobalMsg(Strings.Chat.Global.ToString(client.Entity.Name, msg),
                            CustomColors.AdminGlobalChat, client.Entity.Name);
                    }
                    else if (client.Power.IsModerator)
                    {
                        PacketSender.SendGlobalMsg(Strings.Chat.Global.ToString(client.Entity.Name, msg),
                            CustomColors.ModGlobalChat, client.Entity.Name);
                    }
                    else
                    {
                        PacketSender.SendGlobalMsg(Strings.Chat.Global.ToString(client.Entity.Name, msg),
                            CustomColors.GlobalChat, client.Entity.Name);
                    }
                }
                else if (cmd == Strings.Chat.partycmd || cmd == "/2")
                {
                    if (msg.Trim().Length == 0) return;
                    if (client.Entity.InParty(client.Entity))
                    {
                        PacketSender.SendPartyMsg(client,
                            Strings.Chat.party.ToString(client.Entity.Name, msg), CustomColors.PartyChat,
                            client.Entity.Name);
                    }
                    else
                    {
                        PacketSender.SendChatMsg(client, Strings.Parties.notinparty, CustomColors.Error);
                    }
                }
                else if (cmd == Strings.Chat.admincmd || cmd == "/3")
                {
                    if (msg.Trim().Length == 0) return;
                    if (client.Power.IsModerator)
                    {
                        PacketSender.SendAdminMsg(Strings.Chat.admin.ToString(client.Entity.Name, msg),
                            CustomColors.AdminChat, client.Entity.Name);
                    }
                }
                else if (cmd == Strings.Chat.announcementcmd)
                {
                    if (msg.Trim().Length == 0) return;
                    if (client.Power.IsModerator)
                    {
                        PacketSender.SendGlobalMsg(Strings.Chat.announcement.ToString(client.Entity.Name, msg),
                            CustomColors.AnnouncementChat, client.Entity.Name);
                    }
                }
                else if (cmd == Strings.Chat.pmcmd || cmd == Strings.Chat.messagecmd)
                {
                    if (splitString.Length < 3)
                    {
                        return;
                    }
                    msg = msg.Remove(0, splitString[1].Length + 1); //Chop off the player name parameter
                    if (msg.Trim().Length == 0) return;

                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (splitString[1].ToLower() == Globals.Clients[i].Entity.Name.ToLower())
                            {
                                PacketSender.SendChatMsg(client,
                                    Strings.Chat.Private.ToString(client.Entity.Name, msg), CustomColors.PrivateChat,
                                    client.Entity.Name);
                                PacketSender.SendChatMsg(Globals.Clients[i],
                                    Strings.Chat.Private.ToString(client.Entity.Name, msg), CustomColors.PrivateChat,
                                    client.Entity.Name);
                                Globals.Clients[i].Entity.ChatTarget = client.Entity;
                                client.Entity.ChatTarget = Globals.Clients[i].Entity;
                                return;
                            }
                        }
                    }
                    PacketSender.SendChatMsg(client, Strings.Player.offline, CustomColors.Error);
                }
                else if (cmd == Strings.Chat.replycmd || cmd == Strings.Chat.rcmd)
                {
                    if (msg.Trim().Length == 0) return;
                    if (client.Entity.ChatTarget != null)
                    {
                        PacketSender.SendChatMsg(client, Strings.Chat.Private.ToString(client.Entity.Name, msg),
                            CustomColors.PrivateChat, client.Entity.Name);
                        PacketSender.SendChatMsg(client.Entity.ChatTarget.Client,
                            Strings.Chat.Private.ToString(client.Entity.Name, msg), CustomColors.PrivateChat,
                            client.Entity.Name);
                        client.Entity.ChatTarget.ChatTarget = client.Entity;
                    }
                    else
                    {
                        PacketSender.SendChatMsg(client, Strings.Player.offline, CustomColors.Error);
                    }
                }
                else
                {
                    //Search for command activated events and run them
                    foreach (var evt in EventBase.Lookup)
                    {
                        if ((EventBase)evt.Value != null)
                        {
                            if (client.Entity.StartCommonEvent((EventBase)evt.Value, CommonEventTrigger.SlashCommand, splitString[0].TrimStart('/'), msg) ==
                                true)
                            {
                                return; //Found our /command, exit now :)
                            }
                        }
                    }

                    //No common event /command, invalid command.
                    PacketSender.SendChatMsg(client, Strings.Commands.invalid, CustomColors.Error);
                }
            }
        }

        //BlockPacket
        public void HandlePacket(Client client, Player player, BlockPacket packet)
        {
            if (player == null) return;
            //check if player is blinded or stunned
            var statuses = client.Entity.Statuses.Values.ToArray();
            foreach (var status in statuses)
            {
                if (status.Type == StatusTypes.Stun)
                {
                    PacketSender.SendChatMsg(client, Strings.Combat.stunblocking);
                    return;
                }
                if (status.Type == StatusTypes.Sleep)
                {
                    PacketSender.SendChatMsg(client, Strings.Combat.sleepblocking);
                    return;
                }
            }

            client.Entity.TryBlock(packet.Blocking);
        }

        //BumpPacket
        public void HandlePacket(Client client, Player player, BumpPacket packet)
        {
            if (player == null) return;
            player.TryBumpEvent(packet.MapId, packet.EventId);
        }

        //AttackPacket
        public void HandlePacket(Client client, Player player, AttackPacket packet)
        {
            if (player == null) return;
            bool unequippedAttack = false;
            var target = packet.Target;

            if (client.Entity.CastTime >= Globals.Timing.TimeMs)
            {
                PacketSender.SendChatMsg(client, Strings.Combat.channelingnoattack);
                return;
            }

            //check if player is blinded or stunned
            var statuses = client.Entity.Statuses.Values.ToArray();
            foreach (var status in statuses)
            {
                if (status.Type == StatusTypes.Stun)
                {
                    PacketSender.SendChatMsg(client, Strings.Combat.stunattacking);
                    return;
                }
                if (status.Type == StatusTypes.Sleep)
                {
                    PacketSender.SendChatMsg(client, Strings.Combat.sleepattacking);
                    return;
                }
                if (status.Type == StatusTypes.Blind)
                {
                    PacketSender.SendActionMsg(client.Entity, Strings.Combat.miss,
                        CustomColors.Missed);
                    return;
                }
            }

            var attackingTile = new TileHelper(client.Entity.MapId, client.Entity.X, client.Entity.Y);
            switch (client.Entity.Dir)
            {
                case 0:
                    attackingTile.Translate(0, -1);
                    break;
                case 1:
                    attackingTile.Translate(0, 1);
                    break;
                case 2:
                    attackingTile.Translate(-1, 0);
                    break;
                case 3:
                    attackingTile.Translate(1, 0);
                    break;
            }

            //Fire projectile instead if weapon has it
            if (Options.WeaponIndex > -1)
            {
                if (client.Entity.Equipment[Options.WeaponIndex] >= 0 && ItemBase.Get(client.Entity.Items[client.Entity.Equipment[Options.WeaponIndex]].ItemId) != null)
                {
                    ItemBase weaponItem = ItemBase.Get(client.Entity.Items[client.Entity.Equipment[Options.WeaponIndex]].ItemId);

                    //Check for animation
                    var attackAnim = ItemBase.Get(client.Entity.Items[client.Entity.Equipment[Options.WeaponIndex]].ItemId).AttackAnimation;
                    if (attackAnim != null && attackingTile.TryFix())
                    {
                        PacketSender.SendAnimationToProximity(attackAnim.Id, -1, Guid.Empty, attackingTile.GetMapId(), attackingTile.GetX(), attackingTile.GetY(), (sbyte)client.Entity.Dir);
                    }

                    var weaponInvSlot = client.Entity.Equipment[Options.WeaponIndex];
                    var invItem = client.Entity.Items[weaponInvSlot];
                    var weapon = ItemBase.Get(invItem?.ItemId ?? Guid.Empty);
                    var projectileBase = ProjectileBase.Get(weapon?.ProjectileId ?? Guid.Empty);

                    if (projectileBase != null)
                    {
                        if (projectileBase.AmmoItemId != Guid.Empty)
                        {
                            var itemSlot = client.Entity.FindItem(projectileBase.AmmoItemId, projectileBase.AmmoRequired);
                            if (itemSlot == -1)
                            {
                                PacketSender.SendChatMsg(client,
                                    Strings.Items.notenough.ToString(ItemBase.GetName(projectileBase.AmmoItemId)),
                                    CustomColors.NoAmmo);
                                return;
                            }
#if INTERSECT_DIAGNOSTIC
                                PacketSender.SendPlayerMsg(client,
                                    Strings.Get("items", "notenough", $"REGISTERED_AMMO ({projectileBase.Ammo}:'{ItemBase.GetName(projectileBase.Ammo)}':{projectileBase.AmmoRequired})"),
                                    CustomColors.NoAmmo);
#endif
                            if (!client.Entity.TakeItemsById(projectileBase.AmmoItemId, projectileBase.AmmoRequired))
                            {
#if INTERSECT_DIAGNOSTIC
                                    PacketSender.SendPlayerMsg(client,
                                        Strings.Get("items", "notenough", "FAILED_TO_DEDUCT_AMMO"),
                                        CustomColors.NoAmmo);
                                    PacketSender.SendPlayerMsg(client,
                                        Strings.Get("items", "notenough", $"FAILED_TO_DEDUCT_AMMO {client.Entity.CountItems(projectileBase.Ammo)}"),
                                        CustomColors.NoAmmo);
#endif
                            }
                        }
#if INTERSECT_DIAGNOSTIC
                            else
                            {
                                PacketSender.SendPlayerMsg(client,
                                    Strings.Get("items", "notenough", "NO_REGISTERED_AMMO"),
                                    CustomColors.NoAmmo);
                            }
#endif
                        MapInstance.Get(client.Entity.MapId)
                            .SpawnMapProjectile(client.Entity, projectileBase, null, weaponItem,
                                client.Entity.MapId,
                                (byte)client.Entity.X, (byte)client.Entity.Y, (byte)client.Entity.Z,
                                (byte)client.Entity.Dir, null);
                        return;
                    }
#if INTERSECT_DIAGNOSTIC
                        else
                        {
                            PacketSender.SendPlayerMsg(client,
                                Strings.Get("items", "notenough", "NONPROJECTILE"),
                                CustomColors.NoAmmo);
                            return;
                        }
#endif
                }
                else
                {
                    unequippedAttack = true;
#if INTERSECT_DIAGNOSTIC
                        PacketSender.SendPlayerMsg(client,
                            Strings.Get("items", "notenough", "NO_WEAPON"),
                            CustomColors.NoAmmo);
#endif
                }
            }
            else
            {
                unequippedAttack = true;
            }

            if (unequippedAttack)
            {
                var classBase = ClassBase.Get(client.Entity.ClassId);
                if (classBase != null)
                {
                    //Check for animation
                    if (classBase.AttackAnimation != null)
                    {
                        PacketSender.SendAnimationToProximity(classBase.AttackAnimationId, -1, Guid.Empty, attackingTile.GetMapId(), attackingTile.GetX(), attackingTile.GetY(), (sbyte)client.Entity.Dir);
                    }
                }
            }

            foreach (var map in client.Entity.Map.GetSurroundingMaps(true))
            {
                foreach (var entity in map.GetEntities())
                {
                    if (entity.Id == target)
                    {
                        client.Entity.TryAttack(entity);
                        break;
                    }
                }
            }
        }

        //DirectionPacket
        public void HandlePacket(Client client, Player player, DirectionPacket packet)
        {
            if (player == null) return;
            player.ChangeDir(packet.Direction);
        }

        //EnterGamePacket
        public void HandlePacket(Client client, Player player, EnterGamePacket packet)
        {
            
        }

        //ActivateEventPacket
        public void HandlePacket(Client client, Player player, ActivateEventPacket packet)
        {
            if (player == null) return;
            player.TryActivateEvent(packet.EventId);
        }

        //EventResponsePacket
        public void HandlePacket(Client client, Player player, EventResponsePacket packet)
        {
            if (player == null) return;
            player.RespondToEvent(packet.EventId, packet.Response);
        }

        //CreateAccountPacket
        public void HandlePacket(Client client, Player player, CreateAccountPacket packet)
        {
            if (client.TimeoutMs > Globals.Timing.TimeMs)
            {
                PacketSender.SendError(client, Strings.Errors.errortimeout);
                client.ResetTimeout();
                return;
            }

            client.ResetTimeout();

            if (Options.BlockClientRegistrations)
            {
                PacketSender.SendError(client, Strings.Account.registrationsblocked);
                return;
            }
            //Check for ban
            var isBanned = Ban.CheckBan(client.GetIp());
            if (isBanned != null)
            {
                PacketSender.SendError(client, isBanned);
                return;
            }
            if (!FieldChecking.IsValidUsername(packet.Username, Strings.Regex.username))
            {
                PacketSender.SendError(client, Strings.Account.invalidname);
                return;
            }
            if (!FieldChecking.IsWellformedEmailAddress(packet.Email, Strings.Regex.email))
            {
                PacketSender.SendError(client, Strings.Account.invalidemail);
                return;
            }
            if (DbInterface.AccountExists(packet.Username))
            {
                PacketSender.SendError(client, Strings.Account.exists);
            }
            else
            {
                if (DbInterface.EmailInUse(packet.Email))
                {
                    PacketSender.SendError(client, Strings.Account.emailexists);
                }
                else
                {
                    DbInterface.CreateAccount(client, packet.Username, packet.Password, packet.Email);
                    PacketSender.SendServerConfig(client);

                    //Check that server is in admin only mode
                    if (Options.AdminOnly)
                    {
                        if (client.Power == UserRights.None)
                        {
                            PacketSender.SendError(client, Strings.Account.adminonly);
                            return;
                        }
                    }

                    //Character selection if more than one.
                    if (Options.MaxCharacters > 1)
                    {
                        PacketSender.SendPlayerCharacters(client);
                    }
                    else
                    {
                        PacketSender.SendGameObjects(client, GameObjectType.Class);
                        PacketSender.SendCreateCharacter(client);
                    }
                }
            }
        }

        //CreateCharacterPacket
        public void HandlePacket(Client client, Player player, CreateCharacterPacket packet)
        {
            if (client.User == null) return;
            if (!FieldChecking.IsValidUsername(packet.Name, Strings.Regex.username))
            {
                PacketSender.SendError(client, Strings.Account.invalidname);
                return;
            }
            var index = client.Id;
            var classBase = ClassBase.Get(packet.ClassId);
            if (classBase == null || classBase.Locked)
            {
                PacketSender.SendError(client, Strings.Account.invalidclass);
                return;
            }
            if (DbInterface.CharacterNameInUse(packet.Name))
            {
                PacketSender.SendError(client, Strings.Account.characterexists);
            }
            else
            {
                var newChar = new Player();
                newChar.Id = Guid.NewGuid();
                client.Characters.Add(newChar);
                newChar.ValidateLists();
                for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    newChar.Equipment[i] = -1;
                }

                newChar.Name = packet.Name;
                newChar.ClassId = packet.ClassId;
                newChar.Level = 1;

                if (classBase.Sprites.Count > 0)
                {
                    newChar.Sprite = classBase.Sprites[packet.Sprite].Sprite;
                    newChar.Face = classBase.Sprites[packet.Sprite].Face;
                    newChar.Gender = classBase.Sprites[packet.Sprite].Gender;
                }

                client.LoadCharacter(newChar);

                newChar.SetVital(Vitals.Health, classBase.BaseVital[(int)Vitals.Health]);
                newChar.SetVital(Vitals.Mana, classBase.BaseVital[(int)Vitals.Mana]);

                for (int i = 0; i < (int)Stats.StatCount; i++)
                {
                    newChar.Stat[i].Stat = 0;
                }
                newChar.StatPoints = classBase.BasePoints;

                for (int i = 0; i < classBase.Spells.Count; i++)
                {
                    if (classBase.Spells[i].Level <= 1)
                    {
                        Spell tempSpell = new Spell(classBase.Spells[i].Id);
                        newChar.TryTeachSpell(tempSpell, false);
                    }
                }

                foreach (var item in classBase.Items)
                {
                    if (ItemBase.Get(item.Id) != null)
                    {
                        var tempItem = new Item(item.Id, item.Quantity);
                        newChar.TryGiveItem(tempItem, false);
                    }
                }

                PacketSender.SendJoinGame(client);
                newChar.SetOnline();

                DbInterface.SavePlayerDatabaseAsync();
            }
        }

        //PickupItemPacket
        public void HandlePacket(Client client, Player player, PickupItemPacket packet)
        {
            if (player == null) return;
            if (packet.MapItemIndex < MapInstance.Get(player.MapId).MapItems.Count && MapInstance.Get(player.MapId).MapItems[packet.MapItemIndex] != null)
            {
                if (MapInstance.Get(player.MapId).MapItems[packet.MapItemIndex].X == player.X && MapInstance.Get(player.MapId).MapItems[packet.MapItemIndex].Y == player.Y)
                {
                    if (player.TryGiveItem(MapInstance.Get(player.MapId).MapItems[packet.MapItemIndex]))
                    {
                        //Remove Item From Map
                        MapInstance.Get(player.MapId).RemoveItem(packet.MapItemIndex);
                    }
                }
            }
        }

        //SwapInvItemsPacket
        public void HandlePacket(Client client, Player player, SwapInvItemsPacket packet)
        {
            if (player == null) return;
            player.SwapItems(packet.Slot1, packet.Slot2);
        }

        //DropItemPacket
        public void HandlePacket(Client client, Player player, DropItemPacket packet)
        {
            if (packet == null)
            {
                return;
            }

            player?.DropItems(packet.Slot, packet.Quantity);
        }

        //UseItemPacket
        public void HandlePacket(Client client, Player player, UseItemPacket packet)
        {
            if (player == null) return;
            EntityInstance target = null;
            if (packet.TargetId != Guid.Empty)
            {
                foreach (var map in player.Map.GetSurroundingMaps(true))
                {
                    foreach (var en in map.GetEntities())
                    {
                        if (en.Id == packet.TargetId)
                        {
                            target = en;
                            break;
                        }
                    }
                }
            }
            player.UseItem(packet.Slot, target);
        }

        //SwapSpellsPacket
        public void HandlePacket(Client client, Player player, SwapSpellsPacket packet)
        {
            if (player == null) return;
            player.SwapSpells(packet.Slot1, packet.Slot2);
        }

        //ForgetSpellPacket
        public void HandlePacket(Client client, Player player, ForgetSpellPacket packet)
        {
            if (player == null) return;
            player.ForgetSpell(packet.Slot);
        }

        //UseSpellPacket
        public void HandlePacket(Client client, Player player, UseSpellPacket packet)
        {
            if (player == null) return;
            var casted = false;

            if (packet.TargetId != Guid.Empty)
            {
                foreach (var map in player.Map.GetSurroundingMaps(true))
                {
                    foreach (var en in map.GetEntities())
                    {
                        if (en.Id == packet.TargetId)
                        {
                            player.UseSpell(packet.Slot, en);
                            casted = true;
                            break;
                        }
                    }
                }
            }

            if (!casted) player.UseSpell(packet.Slot, null);
        }

        //UnequipItemPacket
        public void HandlePacket(Client client, Player player, UnequipItemPacket packet)
        {
            if (player == null) return;
            player.UnequipItem(packet.Slot);
        }

        //UpgradeStatPacket
        public void HandlePacket(Client client, Player player, UpgradeStatPacket packet)
        {
            if (player == null) return;
            player.UpgradeStat(packet.Stat);
        }

        //HotbarUpdatePacket
        public void HandlePacket(Client client, Player player, HotbarUpdatePacket packet)
        {
            if (player == null) return;
            player.HotbarChange(packet.HotbarSlot, packet.Type, packet.Index);
        }

        //HotbarSwapPacket
        public void HandlePacket(Client client, Player player, HotbarSwapPacket packet)
        {
            if (player == null) return;
            player.HotbarSwap(packet.Slot1, packet.Slot2);
        }

        //OpenAdminWindowPacket
        public void HandlePacket(Client client, Player player, OpenAdminWindowPacket packet)
        {
            if (client.Power.IsModerator)
            {
                PacketSender.SendMapList(client);
                PacketSender.SendOpenAdminWindow(client);
            }
        }

        //AdminActionPacket
        public void HandlePacket(Client client, Player player, AdminActionPacket packet)
        {
            if (!client.Power.Editor && !client.Power.IsModerator)
            {
                return;
            }
            Classes.Admin.Actions.ActionProcessing.ProcessAction(client, player, (dynamic) packet.Action);
        }

        //BuyItemPacket
        public void HandlePacket(Client client, Player player, BuyItemPacket packet)
        {
            if (player == null) return;
            player.BuyItem(packet.Slot, packet.Quantity);
        }

        //SellItemPacket
        public void HandlePacket(Client client, Player player, SellItemPacket packet)
        {
            if (player == null) return;
            player.SellItem(packet.Slot, packet.Quanity);
        }

        //CloseShopPacket
        public void HandlePacket(Client client, Player player, CloseShopPacket packet)
        {
            if (player == null) return;
            player.CloseShop();
        }

        //CloseCraftingPacket
        public void HandlePacket(Client client, Player player, CloseCraftingPacket packet)
        {
            if (player == null) return;
            player.CloseCraftingTable();
        }

        //CraftItemPacket
        public void HandlePacket(Client client, Player player, CraftItemPacket packet)
        {
            if (player == null) return;
            player.CraftId = packet.CraftId;
            player.CraftTimer = Globals.Timing.TimeMs;
        }

        //CloseBankPacket
        public void HandlePacket(Client client, Player player, CloseBankPacket packet)
        {
            if (player == null) return;
            player.CloseBank();
        }

        //DepositItemPacket
        public void HandlePacket(Client client, Player player, DepositItemPacket packet)
        {
            if (player == null) return;
            player.TryDepositItem(packet.Slot, packet.Quantity);
        }

        //WithdrawItemPacket
        public void HandlePacket(Client client, Player player, WithdrawItemPacket packet)
        {
            if (player == null) return;
            player.WithdrawItem(packet.Slot, packet.Quantity);
        }

        //MoveBankItemPacket
        public void HandlePacket(Client client, Player player, SwapBankItemsPacket packet)
        {
            if (player == null) return;
            player.SwapBankItems(packet.Slot1, packet.Slot2);
        }

        //PartyInvitePacket
        public void HandlePacket(Client client, Player player, PartyInvitePacket packet)
        {
            if (player == null) return;
            var target = Player.FindOnline(packet.TargetId);
            if (target == null) return;
            if (target.Id != player.Id)
            {
                target.InviteToParty(player);
            }
            else
            {
                PacketSender.SendChatMsg(client, Strings.Player.notarget, CustomColors.NoTarget);
            }
        }

        //PartyInviteResponsePacket
        public void HandlePacket(Client client, Player player, PartyInviteResponsePacket packet)
        {
            if (player == null) return;
            var leader = packet.PartyId;
            if (player.PartyRequester != null && player.PartyRequester.Id == leader)
            {
                if (packet.AcceptingInvite)
                {
                    if (player.PartyRequester.IsValidPlayer)
                    {
                        player.PartyRequester.AddParty(player);
                    }
                }
                else
                {
                    PacketSender.SendChatMsg(client.Entity.PartyRequester.Client, Strings.Parties.declined.ToString(client.Entity.Name), CustomColors.Declined);
                    if (player.PartyRequests.ContainsKey(player.PartyRequester))
                    {
                        player.PartyRequests[player.PartyRequester] = Globals.Timing.TimeMs + Options.RequestTimeout;
                    }
                    else
                    {
                        player.PartyRequests.Add(player.PartyRequester, Globals.Timing.TimeMs + Options.RequestTimeout);
                    }
                }
                player.PartyRequester = null;
            }
        }

        //PartyKickPacket
        public void HandlePacket(Client client, Player player, PartyKickPacket packet)
        {
            if (player == null) return;
            player.KickParty(packet.TargetId);
        }

        //PartyLeavePacket
        public void HandlePacket(Client client, Player player, PartyLeavePacket packet)
        {
            if (player == null) return;
            player.LeaveParty();
        }

        //QuestResponsePacket
        public void HandlePacket(Client client, Player player, QuestResponsePacket packet)
        {
            if (player == null) return;
            if (packet.AcceptingQuest)
            {
                player.AcceptQuest(packet.QuestId);
            }
            else
            {
                player.DeclineQuest(packet.QuestId);
            }
        }

        //AbandonQuestPacket
        public void HandlePacket(Client client, Player player, AbandonQuestPacket packet)
        {
            if (player == null) return;
            player.CancelQuest(packet.QuestId);
        }

        //TradeRequestPacket
        public void HandlePacket(Client client, Player player, TradeRequestPacket packet)
        {
            if (player == null) return;
            var target = Player.FindOnline(packet.TargetId);
            if (target == null) return;
            if (target.Id != player.Id)
            {
                target.InviteToTrade(player);
            }
        }

        //TradeRequestResponsePacket
        public void HandlePacket(Client client, Player player, TradeRequestResponsePacket packet)
        {
            if (player == null) return;
            var target = packet.TradeId;
            if (player.Trading.Requester != null && player.Trading.Requester.Id == target)
            {
                if (player.Trading.Requester.IsValidPlayer)
                {
                    if (packet.AcceptingInvite)
                    {
                        if (player.Trading.Requester.Trading.Counterparty == null) //They could have accepted another trade since.
                        {
                            player.Trading.Requester.StartTrade(player);
                        }
                        else
                        {
                            PacketSender.SendChatMsg(client, Strings.Trading.busy.ToString(player.Trading.Requester.Name), Color.Red);
                        }
                    }
                    else
                    {
                        PacketSender.SendChatMsg(player.Trading.Requester.Client, Strings.Trading.declined.ToString(player.Name), CustomColors.Declined);
                        if (player.Trading.Requests.ContainsKey(player.Trading.Requester))
                        {
                            player.Trading.Requests[player.Trading.Requester] = Globals.Timing.TimeMs + Options.RequestTimeout;
                        }
                        else
                        {
                            player.Trading.Requests.Add(player.Trading.Requester, Globals.Timing.TimeMs + Options.RequestTimeout);
                        }
                    }
                }
            }
            player.Trading.Requester = null;
        }

        //OfferTradeItemPacket
        public void HandlePacket(Client client, Player player, OfferTradeItemPacket packet)
        {
            if (player == null) return;
            player?.OfferItem(packet.Slot, packet.Quanity);
        }

        //RevokeTradeItemPacket
        public void HandlePacket(Client client, Player player, RevokeTradeItemPacket packet)
        {
            if (player == null) return;
            player?.RevokeItem(packet.Slot, packet.Quanity);
        }

        //AcceptTradePacket
        public void HandlePacket(Client client, Player player, AcceptTradePacket packet)
        {
            if (player == null) return;
            player.Trading.Accepted = true;
            if (player.Trading.Counterparty.Trading.Accepted)
            {
                Item[] t = new Item[Options.MaxInvItems];

                //Swap the trade boxes over, then return the trade boxes to their new owners!
                t = player.Trading.Offer;
                player.Trading.Offer = player.Trading.Counterparty.Trading.Offer;
                player.Trading.Counterparty.Trading.Offer = t;
                player.Trading.Counterparty.ReturnTradeItems();
                player.ReturnTradeItems();

                PacketSender.SendChatMsg(client, Strings.Trading.accepted, CustomColors.Accepted);
                PacketSender.SendChatMsg(player.Trading.Counterparty.Client, Strings.Trading.accepted, CustomColors.Accepted);
                PacketSender.SendTradeClose(player.Trading.Counterparty.Client);
                PacketSender.SendTradeClose(client);
                player.Trading.Counterparty.Trading.Counterparty = null;
                player.Trading.Counterparty = null;
            }
        }

        //DeclineTradePacket
        public void HandlePacket(Client client, Player player, DeclineTradePacket packet)
        {
            if (player == null) return;
            player?.CancelTrade();
        }

        //CloseBagPacket
        public void HandlePacket(Client client, Player player,  CloseBagPacket packet)
        {
            if (player == null) return;
            player?.CloseBag();
        }

        //StoreBagItemPacket
        public void HandlePacket(Client client, Player player, StoreBagItemPacket packet)
        {
            if (player == null) return;
            player?.StoreBagItem(packet.Slot, packet.Quanity);
        }

        //RetrieveBagItemPacket
        public void HandlePacket(Client client, Player player, RetrieveBagItemPacket packet)
        {
            if (player == null) return;
            player?.RetrieveBagItem(packet.Slot, packet.Quanity);
        }

        //SwapBagItemPacket
        public void HandlePacket(Client client, Player player, SwapBagItemsPacket packet)
        {
            if (player == null) return;
            player?.SwapBagItems(packet.Slot1,packet.Slot2);
        }

        //RequestFriendsPacket
        public void HandlePacket(Client client, Player player, RequestFriendsPacket packet)
        {
            if (player == null) return;
            PacketSender.SendFriends(client);
        }

        //UpdateFriendsPacket
        public void HandlePacket(Client client, Player player, UpdateFriendsPacket packet)
        {
            if (player == null) return;
            if (packet.Adding)
            {
                //Don't add yourself!
                if (packet.Name.ToLower() == client.Entity.Name.ToLower())
                {
                    return;
                }

                var character = DbInterface.GetPlayer(packet.Name);
                if (character != null)
                {
                    if (!client.Entity.HasFriend(character))
                    {
                        var target = Player.FindOnline(packet.Name);
                        if (target != null)
                        {
                            target.FriendRequest(client.Entity);
                        }
                        else
                        {
                            PacketSender.SendChatMsg(client, Strings.Player.offline, CustomColors.Error);
                        }
                    }
                    else
                    {
                        PacketSender.SendChatMsg(client, Strings.Friends.alreadyfriends.ToString(packet.Name), CustomColors.Info);
                    }
                }
            }
            else
            {
                var charId = DbInterface.GetCharacterId(packet.Name);

                if (charId != null)
                {
                    var character = DbInterface.GetPlayer((Guid)charId);
                    if (character != null && client.Entity.HasFriend(character))
                    {
                        player.RemoveFriend(character);
                        character.RemoveFriend(player);
                        PacketSender.SendChatMsg(client, Strings.Friends.remove, CustomColors.Declined);
                        PacketSender.SendFriends(client);
                        if (character.Client != null) PacketSender.SendFriends(character.Client);
                    }
                }
            }
        }

        //FriendRequestResponsePacket
        public void HandlePacket(Client client, Player player, FriendRequestResponsePacket packet)
        {
            if (player == null) return;
            var target = Player.FindOnline(packet.FriendId);
            if (target == null || target.Id == player.Id) return;
            if (packet.AcceptingRequest)
            {
                if (!player.HasFriend(target)) // Incase one user deleted friend then re-requested
                {
                    player.AddFriend(target);
                    PacketSender.SendChatMsg(client, Strings.Friends.notification.ToString(target.Name), CustomColors.Accepted);
                    PacketSender.SendFriends(client);
                }

                if (!target.HasFriend(player)) // Incase one user deleted friend then re-requested
                {
                    target.AddFriend(player);
                    PacketSender.SendChatMsg(target.Client, Strings.Friends.accept.ToString(player.Name), CustomColors.Accepted);
                    PacketSender.SendFriends(target.Client);
                }
            }
            else
            {
                if (player.FriendRequester == target)
                {
                    if (player.FriendRequester.IsValidPlayer)
                    {
                        if (player.FriendRequests.ContainsKey(player.FriendRequester))
                        {
                            player.FriendRequests[player.FriendRequester] = Globals.Timing.TimeMs + Options.RequestTimeout;
                        }
                        else
                        {
                            player.FriendRequests.Add(client.Entity.FriendRequester, Globals.Timing.TimeMs + Options.RequestTimeout);
                        }
                    }
                    player.FriendRequester = null;
                }
            }
        }

        //SelectCharacterPacket
        public void HandlePacket(Client client, Player player, SelectCharacterPacket packet)
        {
            var sw = new Stopwatch();
            sw.Start();
            var character = DbInterface.GetUserCharacter(client.User, packet.CharacterId);
            if (character != null)
            {
                client.LoadCharacter(character);
                sw.Stop();
                Log.Debug("Took " + sw.ElapsedMilliseconds + "ms to load character from db!");
                try
                {
                    client.Entity?.SetOnline();
                    PacketSender.SendJoinGame(client);
                }
                catch (Exception exception)
                {
                    Log.Warn(exception);
                    PacketSender.SendError(client, Strings.Account.loadfail);
                    client.Logout();
                }
            }
        }

        //DeleteCharacterPacket
        public void HandlePacket(Client client, Player player, DeleteCharacterPacket packet)
        {
            var character = DbInterface.GetUserCharacter(client.User, packet.CharacterId);
            if (character != null)
            {
                foreach (var chr in client.Characters.ToArray())
                {
                    if (chr.Id == packet.CharacterId)
                    {
                        client.Characters.Remove(chr);
                        DbInterface.DeleteCharacter(chr);
                    }
                }
            }
            PacketSender.SendError(client, Strings.Account.deletechar, Strings.Account.deleted);
            PacketSender.SendPlayerCharacters(client);
        }

        //NewCharacterPacket
        public void HandlePacket(Client client, Player player, NewCharacterPacket packet)
        {
            if (client?.Characters?.Count < Options.MaxCharacters)
            {
                PacketSender.SendGameObjects(client, GameObjectType.Class);
                PacketSender.SendCreateCharacter(client);
            }
            else
            {
                PacketSender.SendError(client, Strings.Account.maxchars);
            }
        }

        //RequestPasswordResetPacket
        public void HandlePacket(Client client, Player player, RequestPasswordResetPacket packet)
        {
            if (client.TimeoutMs > Globals.Timing.TimeMs)
            {
                PacketSender.SendError(client, Strings.Errors.errortimeout);
                client.ResetTimeout();
                return;
            }

            if (Options.Instance.SmtpValid)
            {
                //Find account with that name or email
                var userName = DbInterface.UsernameFromEmail(packet.NameOrEmail);
                if (string.IsNullOrEmpty(userName)) userName = packet.NameOrEmail;
                if (DbInterface.AccountExists(userName))
                {
                    //Send reset email
                    var user = DbInterface.GetUser(userName);
                    var email = new PasswordResetEmail(user);
                    email.Send();
                }
                else
                {
                    client.FailedAttempt();
                }
            }
            else
            {
                client.FailedAttempt();
            }
        }

        //ResetPasswordPacket
        public void HandlePacket(Client client, Player player, ResetPasswordPacket packet)
        {
            //Find account with that name or email
            var success = false;
            var userName = DbInterface.UsernameFromEmail(packet.NameOrEmail);
            if (string.IsNullOrEmpty(userName)) userName = packet.NameOrEmail;
            if (DbInterface.AccountExists(userName))
            {
                //Reset Password
                var user = DbInterface.GetUser(userName);
                if (user.PasswordResetCode.ToLower().Trim() == packet.ResetCode.ToLower().Trim() && user.PasswordResetTime > DateTime.UtcNow)
                {
                    user.PasswordResetCode = "";
                    user.PasswordResetTime = DateTime.MinValue;
                    DbInterface.ResetPass(user, packet.NewPassword);
                    success = true;
                }
            }

            PacketSender.SendPasswordResetResult(client, success);
        }
        #endregion

        #region "Editor Packets"

        //PingPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.PingPacket packet)
        {

        }

        //LoginPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.LoginPacket packet)
        {
            if (client.AccountAttempts > 3 && client.TimeoutMs > Globals.Timing.TimeMs)
            {
                PacketSender.SendError(client, Strings.Errors.errortimeout);
                client.ResetTimeout();
                return;
            }

            client.ResetTimeout();

            if (!DbInterface.AccountExists(packet.Username))
            {
                client.FailedAttempt();
                PacketSender.SendError(client, Strings.Account.badlogin);
                return;
            }

            if (!DbInterface.CheckPassword(packet.Username, packet.Password))
            {
                client.FailedAttempt();
                PacketSender.SendError(client, Strings.Account.badlogin);
                return;
            }

            if (!DbInterface.CheckAccess(packet.Username).Editor)
            {
                client.FailedAttempt();
                PacketSender.SendError(client, Strings.Account.badaccess);
                return;
            }

            client.IsEditor = true;
            var sw = new Stopwatch();
            sw.Start();
            DbInterface.LoadUser(client, packet.Username);
            sw.Stop();
            Log.Debug("Took " + sw.ElapsedMilliseconds + "ms to load player from db!");
            lock (Globals.ClientLock)
            {
                var clients = Globals.Clients.ToArray();
                foreach (var user in clients)
                {
                    if (user.Name != null && user.Name.ToLower() == packet.Username.ToLower() && user != client && user.IsEditor)
                    {
                        user.Disconnect();
                    }
                }
            }
            PacketSender.SendServerConfig(client);
            PacketSender.SendJoinGame(client);
            PacketSender.SendTimeBaseTo(client);
            PacketSender.SendMapList(client);
        }

        //MapPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.MapUpdatePacket packet)
        {
            if (!client.IsEditor) return;
            var map = MapInstance.Get(packet.MapId);
            if (map == null) return;
            map.Load(packet.JsonData, MapInstance.Get(packet.MapId).Revision + 1);
            MapList.List.UpdateMap(packet.MapId);

            //Event Fixing
            var removedEvents = new List<Guid>();
            foreach (var id in map.EventIds)
            {
                if (!map.LocalEvents.ContainsKey(id))
                {
                    var evt = EventBase.Get(id);
                    if (evt != null) DbInterface.DeleteGameObject(evt);
                    removedEvents.Add(id);
                }
            }
            foreach (var id in removedEvents)
                map.EventIds.Remove(id);
            foreach (var evt in map.LocalEvents)
            {
                var dbObj = EventBase.Get(evt.Key);
                if (dbObj == null)
                {
                    dbObj = (EventBase)DbInterface.AddGameObject(GameObjectType.Event, evt.Key);
                }
                dbObj.Load(evt.Value.JsonData);
                if (!map.EventIds.Contains(evt.Key)) map.EventIds.Add(evt.Key);
            }
            map.LocalEvents.Clear();
            
            if (packet.TileData != null && map.TileData != null) map.TileData = packet.TileData;
            map.AttributeData = packet.AttributeData;

            DbInterface.SaveGameDatabase();
            map.Initialize();
            var players = new List<Player>();
            foreach (var surrMap in map.GetSurroundingMaps(true))
            {
                players.AddRange(surrMap.GetPlayersOnMap().ToArray());
            }
            foreach (var plyr in players)
            {
                plyr.Warp(plyr.MapId, (byte)plyr.X, (byte)plyr.Y, (byte)plyr.Dir, false, (byte)plyr.Z, true);
                PacketSender.SendMap(plyr.Client, packet.MapId);
            }
            PacketSender.SendMap(client, packet.MapId, true); //Sends map to everyone/everything in proximity
            PacketSender.SendMapListToAll();
        }

        //CreateMapPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.CreateMapPacket packet)
        {
            if (!client.IsEditor) return;
            var newMap = Guid.Empty;
            var tmpMap = new MapInstance(true);
            if (!packet.AttachedToMap)
            {
                var destType = (int)packet.MapListParentType;
                newMap = DbInterface.AddGameObject(GameObjectType.Map).Id;
                tmpMap = MapInstance.Get(newMap);
                DbInterface.GenerateMapGrids();
                PacketSender.SendMap(client, newMap, true);
                PacketSender.SendMapGridToAll(tmpMap.MapGrid);
                //FolderDirectory parent = null;
                destType = -1;
                if (destType == -1)
                {
                    MapList.List.AddMap(newMap, tmpMap.TimeCreated, MapBase.Lookup);
                }
                DbInterface.SaveGameDatabase();
                PacketSender.SendMapListToAll();
                /*else if (destType == 0)
                {
                    parent = Database.MapStructure.FindDir(bf.ReadInteger());
                    if (parent == null)
                    {
                        Database.MapStructure.AddMap(newMap);
                    }
                    else
                    {
                        parent.Children.AddMap(newMap);
                    }
                }
                else if (destType == 1)
                {
                    var mapNum = bf.ReadInteger();
                    parent = Database.MapStructure.FindMapParent(mapNum, null);
                    if (parent == null)
                    {
                        Database.MapStructure.AddMap(newMap);
                    }
                    else
                    {
                        parent.Children.AddMap(newMap);
                    }
                }*/
            }
            else
            {
                var relativeMap = packet.MapId;
                switch (packet.AttachDir)
                {
                    case 0:
                        if (MapInstance.Get(MapInstance.Get(relativeMap).Up) == null)
                        {
                            newMap = DbInterface.AddGameObject(GameObjectType.Map).Id;
                            tmpMap = MapInstance.Get(newMap);
                            tmpMap.MapGrid = MapInstance.Get(relativeMap).MapGrid;
                            tmpMap.MapGridX = MapInstance.Get(relativeMap).MapGridX;
                            tmpMap.MapGridY = MapInstance.Get(relativeMap).MapGridY - 1;
                            MapInstance.Get(relativeMap).Up = newMap;
                        }
                        break;

                    case 1:
                        if (MapInstance.Get(MapInstance.Get(relativeMap).Down) == null)
                        {
                            newMap = DbInterface.AddGameObject(GameObjectType.Map).Id;
                            tmpMap = MapInstance.Get(newMap);
                            tmpMap.MapGrid = MapInstance.Get(relativeMap).MapGrid;
                            tmpMap.MapGridX = MapInstance.Get(relativeMap).MapGridX;
                            tmpMap.MapGridY = MapInstance.Get(relativeMap).MapGridY + 1;
                            MapInstance.Get(relativeMap).Down = newMap;
                        }
                        break;

                    case 2:
                        if (MapInstance.Get(MapInstance.Get(relativeMap).Left) == null)
                        {
                            newMap = DbInterface.AddGameObject(GameObjectType.Map).Id;
                            tmpMap = MapInstance.Get(newMap);
                            tmpMap.MapGrid = MapInstance.Get(relativeMap).MapGrid;
                            tmpMap.MapGridX = MapInstance.Get(relativeMap).MapGridX - 1;
                            tmpMap.MapGridY = MapInstance.Get(relativeMap).MapGridY;
                            MapInstance.Get(relativeMap).Left = newMap;
                        }
                        break;

                    case 3:
                        if (MapInstance.Get(MapInstance.Get(relativeMap).Right) == null)
                        {
                            newMap = DbInterface.AddGameObject(GameObjectType.Map).Id;
                            tmpMap = MapInstance.Get(newMap);
                            tmpMap.MapGrid = MapInstance.Get(relativeMap).MapGrid;
                            tmpMap.MapGridX = MapInstance.Get(relativeMap).MapGridX + 1;
                            tmpMap.MapGridY = MapInstance.Get(relativeMap).MapGridY;
                            MapInstance.Get(relativeMap).Right = newMap;
                        }
                        break;
                }

                if (newMap != Guid.Empty)
                {
                    if (tmpMap.MapGridX >= 0 && tmpMap.MapGridX < DbInterface.MapGrids[tmpMap.MapGrid].Width)
                    {
                        if (tmpMap.MapGridY + 1 < DbInterface.MapGrids[tmpMap.MapGrid].Height)
                        {
                            tmpMap.Down = DbInterface.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX, tmpMap.MapGridY + 1];
                            if (tmpMap.Down != Guid.Empty)
                            {
                                MapInstance.Get(tmpMap.Down).Up = newMap;
                            }
                        }
                        if (tmpMap.MapGridY - 1 >= 0)
                        {
                            tmpMap.Up = DbInterface.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX, tmpMap.MapGridY - 1];
                            if (tmpMap.Up != Guid.Empty)
                            {
                                MapInstance.Get(tmpMap.Up).Down = newMap;
                            }
                        }
                    }

                    if (tmpMap.MapGridY >= 0 && tmpMap.MapGridY < DbInterface.MapGrids[tmpMap.MapGrid].Height)
                    {
                        if (tmpMap.MapGridX - 1 >= 0)
                        {
                            tmpMap.Left = DbInterface.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX - 1, tmpMap.MapGridY];
                            if (tmpMap.Left != Guid.Empty)
                            {
                                MapInstance.Get(tmpMap.Left).Right = newMap;
                            }
                        }

                        if (tmpMap.MapGridX + 1 < DbInterface.MapGrids[tmpMap.MapGrid].Width)
                        {
                            tmpMap.Right = DbInterface.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX + 1, tmpMap.MapGridY];
                            if (tmpMap.Right != Guid.Empty)
                            {
                                MapInstance.Get(tmpMap.Right).Left = newMap;
                            }
                        }
                    }

                    DbInterface.SaveGameDatabase();
                    DbInterface.GenerateMapGrids();
                    PacketSender.SendMap(client, newMap, true);
                    PacketSender.SendMapGridToAll(MapInstance.Get(newMap).MapGrid);
                    PacketSender.SendEnterMap(client, newMap);
                    var folderDir = MapList.List.FindMapParent(relativeMap, null);
                    if (folderDir != null)
                    {
                        folderDir.Children.AddMap(newMap, MapInstance.Get(newMap).TimeCreated, MapBase.Lookup);
                    }
                    else
                    {
                        MapList.List.AddMap(newMap, MapInstance.Get(newMap).TimeCreated, MapBase.Lookup);
                    }
                    DbInterface.SaveGameDatabase();
                    PacketSender.SendMapListToAll();
                }
            }
        }

        //MapListUpdatePacket
        public void HandlePacket(Client client, Player player, Packets.Editor.MapListUpdatePacket packet)
        {
            if (!client.IsEditor) return;
            MapListFolder parent = null;
            var mapId = Guid.Empty;
            switch (packet.UpdateType)
            {
                case MapListUpdates.MoveItem:
                    MapList.List.HandleMove(packet.TargetType,packet.TargetId,packet.ParentType,packet.ParentId);
                    break;
                case MapListUpdates.AddFolder:
                    if (packet.ParentId == Guid.Empty)
                    {
                        MapList.List.AddFolder(Strings.Mapping.newfolder);
                    }
                    else if (packet.ParentType == 0)
                    {
                        parent = MapList.List.FindDir(packet.ParentId);
                        if (parent == null)
                        {
                            MapList.List.AddFolder(Strings.Mapping.newfolder);
                        }
                        else
                        {
                            parent.Children.AddFolder(Strings.Mapping.newfolder);
                        }
                    }
                    else if (packet.ParentType == 1)
                    {
                        mapId = packet.ParentId;
                        parent = MapList.List.FindMapParent(mapId, null);
                        if (parent == null)
                        {
                            MapList.List.AddFolder(Strings.Mapping.newfolder);
                        }
                        else
                        {
                            parent.Children.AddFolder(Strings.Mapping.newfolder);
                        }
                    }
                    break;
                case MapListUpdates.Rename:
                    if (packet.TargetType == 0)
                    {
                        parent = MapList.List.FindDir(packet.TargetId);
                        parent.Name = packet.Name;
                        PacketSender.SendMapListToAll();
                    }
                    else if (packet.TargetType == 1)
                    {
                        var mapListMap = MapList.List.FindMap(packet.TargetId);
                        mapListMap.Name = packet.Name;
                        MapInstance.Get(packet.TargetId).Name = packet.Name;
                        DbInterface.SaveGameDatabase();
                        PacketSender.SendMapListToAll();
                    }
                    break;
                case MapListUpdates.Delete:
                    if (packet.TargetType == 0)
                    {
                        MapList.List.DeleteFolder(packet.TargetId);
                        PacketSender.SendMapListToAll();
                    }
                    else if (packet.TargetType == 1)
                    {
                        if (MapInstance.Lookup.Count == 1)
                        {
                            PacketSender.SendError(client, Strings.Mapping.lastmaperror, Strings.Mapping.lastmap);
                            return;
                        }
                        mapId = packet.TargetId;
                        var players = MapInstance.Get(mapId).GetPlayersOnMap();
                        MapList.List.DeleteMap(mapId);
                        DbInterface.DeleteGameObject(MapInstance.Get(mapId));
                        DbInterface.SaveGameDatabase();
                        DbInterface.GenerateMapGrids();
                        PacketSender.SendMapListToAll();
                        foreach (var plyr in players)
                        {
                            plyr.WarpToSpawn();
                        }
                        PacketSender.SendMapToEditors(mapId);
                    }
                    break;
            }
            PacketSender.SendMapListToAll();
            DbInterface.SaveGameDatabase();
        }

        //UnlinkMapPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.UnlinkMapPacket packet)
        {
            if (!client.IsEditor) return;
            var mapId = packet.MapId;
            var curMapId = packet.CurrentMapId;
            int mapGrid = 0;
            if (MapInstance.Lookup.Keys.Contains(mapId))
            {
                if (client.IsEditor)
                {
                    if (MapInstance.Get(mapId) != null)
                    {
                        MapInstance.Get(mapId).ClearConnections();

                        int gridX = MapInstance.Get(mapId).MapGridX;
                        int gridY = MapInstance.Get(mapId).MapGridY;

                        //Up
                        if (gridY - 1 >= 0 && DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].MyGrid[gridX, gridY - 1] != Guid.Empty)
                        {
                            if (MapInstance.Get(DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].MyGrid[gridX, gridY - 1]) != null)
                                MapInstance.Get(DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].MyGrid[gridX, gridY - 1]).ClearConnections((int)Directions.Down);
                        }

                        //Down
                        if (gridY + 1 < DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].Height && DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].MyGrid[gridX, gridY + 1] != Guid.Empty)
                        {
                            if (MapInstance.Get(DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].MyGrid[gridX, gridY + 1]) != null)
                                MapInstance.Get(DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].MyGrid[gridX, gridY + 1]).ClearConnections((int)Directions.Up);
                        }

                        //Left
                        if (gridX - 1 >= 0 && DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].MyGrid[gridX - 1, gridY] != Guid.Empty)
                        {
                            if (MapInstance.Get(DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].MyGrid[gridX - 1, gridY]) != null)
                                MapInstance.Get(DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].MyGrid[gridX - 1, gridY]).ClearConnections((int)Directions.Right);
                        }

                        //Right
                        if (gridX + 1 < DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].Width && DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].MyGrid[gridX + 1, gridY] != Guid.Empty)
                        {
                            if (MapInstance.Get(DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].MyGrid[gridX + 1, gridY]) != null)
                                MapInstance.Get(DbInterface.MapGrids[MapInstance.Get(mapId).MapGrid].MyGrid[gridX + 1, gridY]).ClearConnections((int)Directions.Left);
                        }

                        DbInterface.GenerateMapGrids();
                        if (MapInstance.Lookup.Keys.Contains(curMapId))
                        {
                            mapGrid = MapInstance.Get(curMapId).MapGrid;
                        }
                    }
                    PacketSender.SendMapGridToAll(mapGrid);
                }
            }
        }

        //LinkMapPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.LinkMapPacket packet)
        {
            if (!client.IsEditor) return;
            var adjacentMap = packet.AdjacentMapId;
            var linkMap = packet.LinkMapId;
            long gridX = packet.GridX;
            long gridY = packet.GridY;
            bool canLink = true;
            if (MapInstance.Lookup.Keys.Contains(linkMap) && MapInstance.Lookup.Keys.Contains(adjacentMap))
            {
                //Clear to test if we can link.
                int linkGrid = MapInstance.Get(linkMap).MapGrid;
                int adjacentGrid = MapInstance.Get(adjacentMap).MapGrid;
                if (linkGrid != adjacentGrid)
                {
                    long xOffset = MapInstance.Get(linkMap).MapGridX - gridX;
                    long yOffset = MapInstance.Get(linkMap).MapGridY - gridY;
                    for (int x = 0; x < DbInterface.MapGrids[adjacentGrid].Width; x++)
                    {
                        for (int y = 0; y < DbInterface.MapGrids[adjacentGrid].Height; y++)
                        {
                            if (x + xOffset >= 0 && x + xOffset < DbInterface.MapGrids[linkGrid].Width && y + yOffset >= 0 && y + yOffset < DbInterface.MapGrids[linkGrid].Height)
                            {
                                if (DbInterface.MapGrids[adjacentGrid].MyGrid[x, y] != Guid.Empty && DbInterface.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset] != Guid.Empty)
                                {
                                    //Incompatible Link!
                                    PacketSender.SendError(client, Strings.Mapping.linkfailerror.ToString(MapBase.GetName(linkMap), MapBase.GetName(adjacentMap), MapBase.GetName(DbInterface.MapGrids[adjacentGrid].MyGrid[x, y]), MapBase.GetName(DbInterface.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset])), Strings.Mapping.linkfail);
                                    return;
                                }
                            }
                        }
                    }
                    if (canLink)
                    {
                        for (int x = -1; x < DbInterface.MapGrids[adjacentGrid].Width + 1; x++)
                        {
                            for (int y = -1; y < DbInterface.MapGrids[adjacentGrid].Height + 1; y++)
                            {
                                if (x + xOffset >= 0 && x + xOffset < DbInterface.MapGrids[linkGrid].Width && y + yOffset >= 0 && y + yOffset < DbInterface.MapGrids[linkGrid].Height)
                                {
                                    if (DbInterface.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset] != Guid.Empty)
                                    {
                                        bool inXBounds = x > -1 && x < DbInterface.MapGrids[adjacentGrid].Width;
                                        bool inYBounds = y > -1 && y < DbInterface.MapGrids[adjacentGrid].Height;
                                        if (inXBounds && inYBounds)
                                            DbInterface.MapGrids[adjacentGrid].MyGrid[x, y] = DbInterface.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];

                                        if (inXBounds && y - 1 >= 0 && DbInterface.MapGrids[adjacentGrid].MyGrid[x, y - 1] != Guid.Empty)
                                        {
                                            MapInstance.Get(DbInterface.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Up = DbInterface.MapGrids[adjacentGrid].MyGrid[x, y - 1];
                                            MapInstance.Lookup.Get<MapInstance>(DbInterface.MapGrids[adjacentGrid].MyGrid[x, y - 1]).Down = DbInterface.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                        }

                                        if (inXBounds && y + 1 < DbInterface.MapGrids[adjacentGrid].Height && DbInterface.MapGrids[adjacentGrid].MyGrid[x, y + 1] != Guid.Empty)
                                        {
                                            MapInstance.Get(DbInterface.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Down = DbInterface.MapGrids[adjacentGrid].MyGrid[x, y + 1];
                                            MapInstance.Lookup.Get<MapInstance>(DbInterface.MapGrids[adjacentGrid].MyGrid[x, y + 1]).Up = DbInterface.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                        }

                                        if (inYBounds && x - 1 >= 0 && DbInterface.MapGrids[adjacentGrid].MyGrid[x - 1, y] != Guid.Empty)
                                        {
                                            MapInstance.Get(DbInterface.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Left = DbInterface.MapGrids[adjacentGrid].MyGrid[x - 1, y];
                                            MapInstance.Lookup.Get<MapInstance>(DbInterface.MapGrids[adjacentGrid].MyGrid[x - 1, y]).Right = DbInterface.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                        }

                                        if (inYBounds && x + 1 < DbInterface.MapGrids[adjacentGrid].Width &&DbInterface.MapGrids[adjacentGrid].MyGrid[x + 1, y] != Guid.Empty)
                                        {
                                            MapInstance.Get(DbInterface.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Right = DbInterface.MapGrids[adjacentGrid].MyGrid[x + 1, y];
                                            MapInstance.Lookup.Get<MapInstance>(DbInterface.MapGrids[adjacentGrid].MyGrid[x + 1, y]).Left = DbInterface.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                        }
                                    }
                                }
                            }
                        }
                        DbInterface.SaveGameDatabase();
                        DbInterface.GenerateMapGrids();
                        PacketSender.SendMapGridToAll(MapInstance.Get(adjacentMap).MapGrid);
                    }
                }
            }
        }

        //CreateGameObjectPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.CreateGameObjectPacket packet)
        {
            if (!client.IsEditor) return;
            var type = packet.Type;
            var obj = DbInterface.AddGameObject(type);
            if (type == GameObjectType.Event)
            {
                ((EventBase)obj).CommonEvent = true;
                DbInterface.SaveGameDatabase();
            }
            PacketSender.CacheGameDataPacket();
            PacketSender.SendGameObjectToAll(obj);
        }

        //RequestOpenEditorPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.RequestOpenEditorPacket packet)
        {
            if (!client.IsEditor) return;
            var type = packet.Type;
            PacketSender.SendGameObjects(client, type);
            PacketSender.SendOpenEditor(client, type);
        }

        //DeleteGameObjectPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.DeleteGameObjectPacket packet)
        {
            if (!client.IsEditor) return;
            var type = packet.Type;
            var id = packet.Id;
            // TODO: YO COME DO THIS
            IDatabaseObject obj = null;
            switch (type)
            {
                case GameObjectType.Animation:
                    obj = AnimationBase.Get(id);
                    break;
                case GameObjectType.Class:
                    if (ClassBase.Lookup.Count == 1)
                    {
                        PacketSender.SendError(client, Strings.Classes.lastclasserror, Strings.Classes.lastclass);
                        return;
                    }
                    obj = DatabaseObject<ClassBase>.Lookup.Get(id);
                    break;
                case GameObjectType.Item:
                    obj = ItemBase.Get(id);
                    break;
                case GameObjectType.Npc:
                    obj = NpcBase.Get(id);
                    break;
                case GameObjectType.Projectile:
                    obj = ProjectileBase.Get(id);
                    break;
                case GameObjectType.Quest:
                    obj = QuestBase.Get(id);
                    break;
                case GameObjectType.Resource:
                    obj = ResourceBase.Get(id);
                    break;
                case GameObjectType.Shop:
                    obj = ShopBase.Get(id);
                    break;
                case GameObjectType.Spell:
                    obj = SpellBase.Get(id);
                    break;
                case GameObjectType.CraftTables:
                    obj = DatabaseObject<CraftingTableBase>.Lookup.Get(id);
                    break;
                case GameObjectType.Crafts:
                    obj = DatabaseObject<CraftBase>.Lookup.Get(id);
                    break;
                case GameObjectType.Map:
                    break;
                case GameObjectType.Event:
                    obj = EventBase.Get(id);
                    break;
                case GameObjectType.PlayerVariable:
                    obj = PlayerVariableBase.Get(id);
                    break;
                case GameObjectType.ServerVariable:
                    obj = ServerVariableBase.Get(id);
                    break;
                case GameObjectType.Tileset:
                    break;
                case GameObjectType.Time:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (obj != null)
            {
                //if Item or Resource, kill all global entities of that kind
                if (type == GameObjectType.Item)
                {
                    Globals.KillItemsOf((ItemBase)obj);
                }
                else if (type == GameObjectType.Resource)
                {
                    Globals.KillResourcesOf((ResourceBase)obj);
                }
                else if (type == GameObjectType.Npc)
                {
                    Globals.KillNpcsOf((NpcBase)obj);
                }
                DbInterface.DeleteGameObject(obj);
                DbInterface.SaveGameDatabase();
                PacketSender.CacheGameDataPacket();
                PacketSender.SendGameObjectToAll(obj, true);
            }
        }

        //SaveGameObjectPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.SaveGameObjectPacket packet)
        {
            if (!client.IsEditor) return;
            var type = packet.Type;
            var id = packet.Id;
            IDatabaseObject obj = null;
            switch (type)
            {
                case GameObjectType.Animation:
                    obj = AnimationBase.Get(id);
                    break;
                case GameObjectType.Class:
                    obj = DatabaseObject<ClassBase>.Lookup.Get(id);
                    break;
                case GameObjectType.Item:
                    obj = ItemBase.Get(id);
                    break;
                case GameObjectType.Npc:
                    obj = NpcBase.Get(id);
                    break;
                case GameObjectType.Projectile:
                    obj = ProjectileBase.Get(id);
                    break;
                case GameObjectType.Quest:
                    obj = QuestBase.Get(id);
                    break;
                case GameObjectType.Resource:
                    obj = ResourceBase.Get(id);
                    break;
                case GameObjectType.Shop:
                    obj = ShopBase.Get(id);
                    break;
                case GameObjectType.Spell:
                    obj = SpellBase.Get(id);
                    break;
                case GameObjectType.CraftTables:
                    obj = DatabaseObject<CraftingTableBase>.Lookup.Get(id);
                    break;
                case GameObjectType.Crafts:
                    obj = DatabaseObject<CraftBase>.Lookup.Get(id);
                    break;
                case GameObjectType.Map:
                    break;
                case GameObjectType.Event:
                    obj = EventBase.Get(id);
                    break;
                case GameObjectType.PlayerVariable:
                    obj = PlayerVariableBase.Get(id);
                    break;
                case GameObjectType.ServerVariable:
                    obj = ServerVariableBase.Get(id);
                    break;
                case GameObjectType.Tileset:
                    break;
                case GameObjectType.Time:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (obj != null)
            {
                //if Item or Resource, kill all global entities of that kind
                if (type == GameObjectType.Item)
                {
                    Globals.KillItemsOf((ItemBase)obj);
                }
                else if (type == GameObjectType.Npc)
                {
                    Globals.KillNpcsOf((NpcBase)obj);
                }
                else if (type == GameObjectType.Projectile)
                {
                    Globals.KillProjectilesOf((ProjectileBase)obj);
                }
                
                obj.Load(packet.Data);

                if (type == GameObjectType.Quest)
                {
                    var qst = (QuestBase)obj;
                    foreach (var evt in qst.RemoveEvents)
                    {
                        var evtb = EventBase.Get(evt);
                        if (evtb != null) DbInterface.DeleteGameObject(evtb);
                    }
                    foreach (var evt in qst.AddEvents)
                    {
                        var evtb = (EventBase)DbInterface.AddGameObject(GameObjectType.Event, evt.Key);
                        evtb.CommonEvent = false;
                        foreach (var tsk in qst.Tasks)
                        {
                            if (tsk.Id == evt.Key) tsk.CompletionEvent = evtb;
                        }
                        evtb.Load(evt.Value.JsonData);
                    }
                    qst.AddEvents.Clear();
                    qst.RemoveEvents.Clear();
                }
                PacketSender.CacheGameDataPacket();
                PacketSender.SendGameObjectToAll(obj, false);
                DbInterface.SaveGameDatabase();
            }
        }

        //SaveTimeDataPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.SaveTimeDataPacket packet)
        {
            if (!client.IsEditor) return;
            TimeBase.GetTimeBase().LoadFromJson(packet.TimeJson);
            DbInterface.SaveGameDatabase();
            ServerTime.Init();
            PacketSender.SendTimeBaseToAllEditors();
        }

        //AddTilesetsPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.AddTilesetsPacket packet)
        {
            if (!client.IsEditor) return;
            foreach (var tileset in packet.Tilesets)
            {
                var value = tileset.Trim().ToLower();
                var found = false;
                foreach (var tset in TilesetBase.Lookup)
                    if (tset.Value.Name.Trim().ToLower() == value)
                        found = true;

                if (!found)
                {
                    var obj = DbInterface.AddGameObject(GameObjectType.Tileset);
                    ((TilesetBase) obj).Name = value;
                    DbInterface.SaveGameDatabase();
                    PacketSender.CacheGameDataPacket();
                    PacketSender.SendGameObjectToAll(obj);
                }
            }
        }

        //RequestGridPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.RequestGridPacket packet)
        {
            if (!client.IsEditor) return;
            if (MapInstance.Lookup.Keys.Contains(packet.MapId))
            {
                if (client.IsEditor)
                {
                    PacketSender.SendMapGrid(client, MapInstance.Get(packet.MapId).MapGrid);
                }
            }
        }

        //OpenMapPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.EnterMapPacket packet)
        {
            if (!client.IsEditor) return;
            client.EditorMap = packet.MapId;
        }

        //NeedMapPacket
        public void HandlePacket(Client client, Player player, Packets.Editor.NeedMapPacket packet)
        {
            if (!client.IsEditor) return;
            var map = MapInstance.Get(packet.MapId);
            if (map != null)
            {
                PacketSender.SendMap(client, packet.MapId);
            }
        }

        #endregion
    }
}