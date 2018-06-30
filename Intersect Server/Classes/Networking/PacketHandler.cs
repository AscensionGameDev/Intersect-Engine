using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Server.Classes.Localization;
using Intersect.Logging;
using Intersect.Models;
using Intersect.Network;
using Intersect.Network.Packets.Reflectable;
using Intersect.Server.Classes.Database;
using Intersect.Server.Classes.Database.PlayerData;
using Intersect.Server.Classes.Database.PlayerData.Characters;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Spells;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.Server.Classes.Networking
{
    using LegacyDatabase = Intersect.Server.Classes.Core.LegacyDatabase;

    public class PacketHandler
    {
        public bool HandlePacket(IPacket packet)
        {
            var binaryPacket = packet as BinaryPacket;

            var bf = binaryPacket?.Buffer;

            if (packet == null || bf == null) return false;

            //Compressed?
            if (bf.ReadByte() == 1)
            {
                var data = Compression.DecompressPacket(bf.ReadBytes(bf.Length()));
                bf = new ByteBuffer();
                bf.WriteBytes(data);
            }

            HandlePacket(Client.FindBeta4Client(packet.Connection), bf);
            return true;
        }

        public void HandlePacket(Client client, ByteBuffer bf)
        {
            //The raw packet is here, no more processing (decompression, length calculations, etc should have to happen)
            if (client == null)
            {
                Log.Debug("Client missing... >.>");
                return;
            }

            if (bf == null || bf.Length() == 0) return;
            var packetHeader = (ClientPackets)bf.ReadLong();
            var packet = bf.ReadBytes(bf.Length());
            bf.Dispose();
            switch (packetHeader)
            {
                case ClientPackets.Ping:
                    HandlePing(client);
                    break;
                case ClientPackets.Login:
                    HandleLogin(client, packet);
                    break;
                case ClientPackets.Logout:
                    HandleLogout(client, packet);
                    break;
                case ClientPackets.NeedMap:
                    HandleNeedMap(client, packet);
                    break;
                case ClientPackets.SendMove:
                    HandlePlayerMove(client, packet);
                    break;
                case ClientPackets.LocalMessage:
                    HandleLocalMsg(client, packet);
                    break;
                case ClientPackets.EditorLogin:
                    HandleEditorLogin(client, packet);
                    break;
                case ClientPackets.SaveMap:
                    HandleMap(client, packet);
                    break;
                case ClientPackets.CreateMap:
                    HandleCreateMap(client, packet);
                    break;
                case ClientPackets.TryAttack:
                    HandleTryAttack(client, packet);
                    break;
                case ClientPackets.TryBlock:
                    HandleTryBlock(client, packet);
                    break;
                case ClientPackets.SendDir:
                    HandleDir(client, packet);
                    break;
                case ClientPackets.EnterGame:
                    HandleEnterGame(client, packet);
                    break;
                case ClientPackets.ActivateEvent:
                    HandleActivateEvent(client, packet);
                    break;
                case ClientPackets.EventResponse:
                    HandleEventResponse(client, packet);
                    break;
                case ClientPackets.CreateAccount:
                    HandleCreateAccount(client, packet);
                    break;
                case ClientPackets.PickupItem:
                    HandlePickupItem(client, packet);
                    break;
                case ClientPackets.SwapItems:
                    HandleSwapItems(client, packet);
                    break;
                case ClientPackets.DropItems:
                    HandleDropItems(client, packet);
                    break;
                case ClientPackets.UseItem:
                    HandleUseItem(client, packet);
                    break;
                case ClientPackets.SwapSpells:
                    HandleSwapSpells(client, packet);
                    break;
                case ClientPackets.ForgetSpell:
                    HandleForgetSpell(client, packet);
                    break;
                case ClientPackets.UseSpell:
                    HandleUseSpell(client, packet);
                    break;
                case ClientPackets.UnequipItem:
                    HandleUnequipItem(client, packet);
                    break;
                case ClientPackets.UpgradeStat:
                    HandleUpgradeStat(client, packet);
                    break;
                case ClientPackets.HotbarChange:
                    HandleHotbarChange(client, packet);
                    break;
                case ClientPackets.MapListUpdate:
                    HandleMapListUpdate(client, packet);
                    break;
                case ClientPackets.CreateCharacter:
                    HandleCreateCharacter(client, packet);
                    break;
                case ClientPackets.OpenAdminWindow:
                    HandleOpenAdminWindow(client);
                    break;
                case ClientPackets.AdminAction:
                    HandleAdminAction(client, packet);
                    break;
                case ClientPackets.NeedGrid:
                    HandleNeedGrid(client, packet);
                    break;
                case ClientPackets.UnlinkMap:
                    HandleUnlinkMap(client, packet);
                    break;
                case ClientPackets.LinkMap:
                    HandleLinkMap(client, packet);
                    break;
                case ClientPackets.BuyItem:
                    HandleBuyItem(client, packet);
                    break;
                case ClientPackets.SellItem:
                    HandleSellItem(client, packet);
                    break;
                case ClientPackets.CloseShop:
                    HandleCloseShop(client, packet);
                    break;
                case ClientPackets.CloseCraftingTable:
                    HandleCloseCrafting(client, packet);
                    break;
                case ClientPackets.CraftItem:
                    HandleCraftItem(client, packet);
                    break;
                case ClientPackets.CloseBank:
                    HandleCloseBank(client, packet);
                    break;
                case ClientPackets.DepositItem:
                    HandleDepositItem(client, packet);
                    break;
                case ClientPackets.WithdrawItem:
                    HandleWithdrawItem(client, packet);
                    break;
                case ClientPackets.MoveBankItem:
                    HandleMoveBankItem(client, packet);
                    break;
                case ClientPackets.NewGameObject:
                    HandleCreateGameObject(client, packet);
                    break;
                case ClientPackets.OpenObjectEditor:
                    HandleRequestOpenEditor(client, packet);
                    break;
                case ClientPackets.DeleteGameObject:
                    HandleDeleteGameObject(client, packet);
                    break;
                case ClientPackets.SaveGameObject:
                    HandleSaveGameObject(client, packet);
                    break;
                case ClientPackets.SaveTime:
                    HandleSaveTime(client, packet);
                    break;
                case ClientPackets.PartyInvite:
                    HandlePartyInvite(client, packet);
                    break;
                case ClientPackets.PartyAcceptInvite:
                    HandleAcceptPartyInvite(client, packet);
                    break;
                case ClientPackets.PartyDeclineInvite:
                    HandleDeclinePartyInvite(client, packet);
                    break;
                case ClientPackets.PartyKick:
                    HandlePartyKick(client, packet);
                    break;
                case ClientPackets.PartyLeave:
                    HandlePartyLeave(client, packet);
                    break;
                case ClientPackets.AcceptQuest:
                    HandleAcceptQuest(client, packet);
                    break;
                case ClientPackets.DeclineQuest:
                    HandleDeclineQuest(client, packet);
                    break;
                case ClientPackets.CancelQuest:
                    HandleCancelQuest(client, packet);
                    break;
                case ClientPackets.TradeRequest:
                    HandleTradeRequest(client, packet);
                    break;
                case ClientPackets.TradeAccept:
                    HandleTradeAccept(client, packet);
                    break;
                case ClientPackets.TradeDecline:
                    HandleTradeDecline(client, packet);
                    break;
                case ClientPackets.TradeOffer:
                    HandleTradeOffer(client, packet);
                    break;
                case ClientPackets.TradeRevoke:
                    HandleTradeRevoke(client, packet);
                    break;
                case ClientPackets.TradeRequestAccept:
                    HandleTradeRequestAccept(client, packet);
                    break;
                case ClientPackets.TradeRequestDecline:
                    HandleTradeRequestDecline(client, packet);
                    break;
                case ClientPackets.AddTilesets:
                    HandleAddTilesets(client, packet);
                    break;
                case ClientPackets.EnterMap:
                    HandleEnterMap(client, packet);
                    break;
                case ClientPackets.CloseBag:
                    HandleCloseBag(client, packet);
                    break;
                case ClientPackets.StoreBagItem:
                    HandleStoreBagItem(client, packet);
                    break;
                case ClientPackets.RetreiveBagItem:
                    HandleRetreiveBagItem(client, packet);
                    break;
                case ClientPackets.MoveBagItem:
                    HandleMoveBagItem(client, packet);
                    break;
                case ClientPackets.RequestFriends:
                    HandleRequestFriends(client, packet);
                    break;
                case ClientPackets.AddFriend:
                    HandleAddFriend(client, packet);
                    break;
                case ClientPackets.RemoveFriend:
                    HandleRemoveFriend(client, packet);
                    break;
                case ClientPackets.FriendRequestAccept:
                    HandleFriendRequest(client, packet);
                    break;
                case ClientPackets.FriendRequestDecline:
                    HandleFriendRequestDecline(client, packet);
                    break;
                case ClientPackets.PlayGame:
                    HandlePlayGame(client, packet);
                    break;
                case ClientPackets.DeleteChar:
                    HandleDeleteChar(client, packet);
                    break;
                case ClientPackets.CreateNewChar:
                    HandleCreateNewChar(client, packet);
                    break;
                default:
                    break;
            }
        }

        private static void HandlePing(Client client)
        {
            client.Pinged();
            PacketSender.SendPing(client, false);
        }

        private static void HandleLogin(Client client, byte[] packet)
        {
            using (var bf = new ByteBuffer())
            {
                bf.WriteBytes(packet);
                var index = client.EntityIndex;
                var username = bf.ReadString();
                var password = bf.ReadString();

                if (!LegacyDatabase.AccountExists(username))
                {
                    PacketSender.SendLoginError(client, Strings.Account.badlogin);
                    return;
                }

                if (!LegacyDatabase.CheckPassword(username, password))
                {
                    PacketSender.SendLoginError(client, Strings.Account.badlogin);
                    return;
                }

                lock (Globals.ClientLock)
                {
                    Globals.Clients?.ForEach(user =>
                    {
                        if (user == client) return;
                        if (user?.IsEditor ?? false) return;

                        if (!string.Equals(user?.Name, username, StringComparison.InvariantCultureIgnoreCase)) return;
                        user?.Disconnect();
                    });
                }

                if (!LegacyDatabase.LoadUser(client, username))
                {
                    PacketSender.SendLoginError(client, Strings.Account.loadfail);
                    return;
                }

                //Check for ban
                var isBanned = Ban.CheckBan(client.User, client.GetIp());
                if (isBanned != null)
                {
                    PacketSender.SendLoginError(client, isBanned);
                    return;
                }

                //Check Mute Status and Load into user property
                Mute.CheckMute(client.User, client.GetIp());

                PacketSender.SendServerConfig(client);
                //Character selection if more than one.
                if (Options.MaxCharacters > 1)
                {
                    PacketSender.SendPlayerCharacters(client);
                }
                else if (client.Characters?.Count > 0)
                {
                    client.LoadCharacter(client.Characters.First());
                    PacketSender.SendJoinGame(client);
                }
                else
                {
                    PacketSender.SendGameObjects(client, GameObjectType.Class);
                    PacketSender.SendCreateCharacter(client);
                }
            }
        }

        private static void HandleLogout(Client client, byte[] packet)
        {
            client?.Logout();
        }

        private static void HandleNeedMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = (int)bf.ReadLong();
            var map = MapInstance.Lookup.Get<MapInstance>(mapNum);
            if (map != null)
            {
                PacketSender.SendMap(client, mapNum);
                if (!client.IsEditor && client.Entity != null && mapNum == client.Entity.MapIndex)
                {
                    PacketSender.SendMapGrid(client, MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid);
                }
            }
        }

        private static void HandlePlayerMove(Client client, byte[] packet)
        {
            var index = client.EntityIndex;
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int map = bf.ReadInteger();
            int x = bf.ReadInteger();
            int y = bf.ReadInteger();
            int dir = bf.ReadInteger();

            //check if player is stunned or snared, if so don't let them move.
            var statuses = client.Entity.Statuses.Values.ToArray();
            foreach (var status in statuses)
            {
                if (status.Type == (int)StatusTypes.Stun || status.Type == (int)StatusTypes.Snare)
                {
                    bf.Dispose();
                    return;
                }
            }

            if (!TileHelper.IsTileValid(map, x, y))
            {
                //POSSIBLE HACKING ATTEMPT!
                PacketSender.SendEntityPositionTo(client, client.Entity);
                return;
            }
            bf.Dispose();
            if ((Globals.Entities[index].CanMove(dir) == -1 || Globals.Entities[index].CanMove(dir) == -4) &&
                client.Entity.MoveRoute == null)
            {
                Globals.Entities[index].Move(dir, client, false);
                if (Globals.Entities[index].MoveTimer > Globals.System.GetTimeMs())
                {
                    //TODO: Make this based moreso on the players current ping instead of a flat value that can be abused
                    Globals.Entities[index].MoveTimer = Globals.System.GetTimeMs() +
                                                        (long)(Globals.Entities[index].GetMovementTime() * .75f);
                }
            }
            else
            {
                PacketSender.SendEntityPositionTo(client, client.Entity);
                return;
            }
            if (map != client.Entity.MapIndex || x != client.Entity.X || y != client.Entity.Y)
            {
                PacketSender.SendEntityPositionTo(client, client.Entity);
            }
        }

        private static void HandleLocalMsg(Client client, byte[] packet)
        {
            var index = client.EntityIndex;
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var msg = bf.ReadString();
            string channel = bf.ReadInteger().ToString();
            if (client.User.IsMuted()) //Don't let the toungless toxic kids speak.
            {
                PacketSender.SendPlayerMsg(client, client.User.GetMuteReason());
                return;
            }

            //If no /command, then use the designated channel.
            if (msg[0] != '/')
            {
                msg = "/" + channel + " " + msg;
            }

            //Check for /commands
            if (msg[0] == '/')
            {
                string[] splitString = msg.Split();
                msg = msg.Remove(0, splitString[0].Length); //Chop off the /command at the start of the sentance
                var cmd = splitString[0].ToLower();

                if (cmd == Strings.Chat.localcmd || cmd == "/0")
                {
                    if (client.Access == 2)
                    {
                        PacketSender.SendProximityMsg(Strings.Chat.local.ToString(client.Entity.Name, msg),
                            client.Entity.MapIndex, CustomColors.AdminLocalChat, client.Entity.Name);
                    }
                    else if (client.Access == 1)
                    {
                        PacketSender.SendProximityMsg(Strings.Chat.local.ToString(client.Entity.Name, msg),
                            client.Entity.MapIndex, CustomColors.ModLocalChat, client.Entity.Name);
                    }
                    else
                    {
                        PacketSender.SendProximityMsg(Strings.Chat.local.ToString(client.Entity.Name, msg),
                            client.Entity.MapIndex, CustomColors.LocalChat, client.Entity.Name);
                    }
                    PacketSender.SendChatBubble(client.Entity.MyIndex, (int)EntityTypes.GlobalEntity, msg,
                        client.Entity.MapIndex);
                }
                else if (cmd == Strings.Chat.allcmd || cmd == "/1" || cmd == Strings.Chat.globalcmd)
                {
                    if (client.Access == 2)
                    {
                        PacketSender.SendGlobalMsg(Strings.Chat.Global.ToString(client.Entity.Name, msg),
                            CustomColors.AdminGlobalChat, client.Entity.Name);
                    }
                    else if (client.Access == 1)
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
                    if (client.Entity.InParty(client.Entity))
                    {
                        PacketSender.SendPartyMsg(client,
                            Strings.Chat.party.ToString(client.Entity.Name, msg), CustomColors.PartyChat,
                            client.Entity.Name);
                    }
                    else
                    {
                        PacketSender.SendPlayerMsg(client, Strings.Parties.notinparty, CustomColors.Error);
                    }
                }
                else if (cmd == Strings.Chat.admincmd || cmd == "/3")
                {
                    if (client.Access > 0)
                    {
                        PacketSender.SendAdminMsg(Strings.Chat.admin.ToString(client.Entity.Name, msg),
                            CustomColors.AdminChat, client.Entity.Name);
                    }
                }
                else if (cmd == Strings.Chat.announcementcmd)
                {
                    if (client.Access > 0)
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
                                PacketSender.SendPlayerMsg(client,
                                    Strings.Chat.Private.ToString(client.Entity.Name, msg), CustomColors.PrivateChat,
                                    client.Entity.Name);
                                PacketSender.SendPlayerMsg(Globals.Clients[i],
                                    Strings.Chat.Private.ToString(client.Entity.Name, msg), CustomColors.PrivateChat,
                                    client.Entity.Name);
                                Globals.Clients[i].Entity.ChatTarget = client.Entity;
                                client.Entity.ChatTarget = Globals.Clients[i].Entity;
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, Strings.Player.offline, CustomColors.Error);
                }
                else if (cmd == Strings.Chat.replycmd || cmd == Strings.Chat.rcmd)
                {
                    if (msg.Trim().Length == 0) return;
                    if (client.Entity.ChatTarget != null)
                    {
                        PacketSender.SendPlayerMsg(client, Strings.Chat.Private.ToString(client.Entity.Name, msg),
                            CustomColors.PrivateChat, client.Entity.Name);
                        PacketSender.SendPlayerMsg(client.Entity.ChatTarget.MyClient,
                            Strings.Chat.Private.ToString(client.Entity.Name, msg), CustomColors.PrivateChat,
                            client.Entity.Name);
                        client.Entity.ChatTarget.ChatTarget = client.Entity;
                    }
                    else
                    {
                        PacketSender.SendPlayerMsg(client, Strings.Player.offline, CustomColors.Error);
                    }
                }
                else
                {
                    //Search for command activated events and run them
                    foreach (var evt in EventBase.Lookup)
                    {
                        if ((EventBase)evt.Value != null)
                        {
                            if (client.Entity.StartCommonEvent((EventBase)evt.Value,
                                    (int)EventPage.CommonEventTriggers.Command, splitString[0].TrimStart('/'), msg) ==
                                true)
                            {
                                return; //Found our /command, exit now :)
                            }
                        }
                    }

                    //No common event /command, invalid command.
                    PacketSender.SendPlayerMsg(client, Strings.Commands.invalid, CustomColors.Error);
                }
            }

            bf.Dispose();
        }

        private static void HandleEditorLogin(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var usr = bf.ReadString();
            var pass = bf.ReadString();
            if (!LegacyDatabase.AccountExists(usr))
            {
                PacketSender.SendLoginError(client, Strings.Account.badlogin);
                return;
            }

            if (!LegacyDatabase.CheckPassword(usr, pass))
            {
                PacketSender.SendLoginError(client, Strings.Account.badlogin);
                return;
            }

            if (LegacyDatabase.CheckPower(usr) != 2)
            {
                PacketSender.SendLoginError(client, Strings.Account.badaccess);
                return;
            }

            client.IsEditor = true;
            LegacyDatabase.LoadUser(client, usr);
            lock (Globals.ClientLock)
            {
                var clients = Globals.Clients.ToArray();
                foreach (var user in clients)
                {
                    if (user.Name != null && user.Name.ToLower() == usr.ToLower() && user != client && user.IsEditor)
                    {
                        user.Disconnect();
                    }
                }
            }
            PacketSender.SendServerConfig(client);
            PacketSender.SendJoinGame(client);
            PacketSender.SendTimeBaseTo(client);
            PacketSender.SendGameData(client);
            PacketSender.SendMapList(client);
        }

        private static void HandleMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapNum = (int)bf.ReadInteger();
            var map = MapInstance.Lookup.Get<MapInstance>(mapNum);
            if (map == null) return;
            MapInstance.Lookup.Get<MapInstance>(mapNum).Load(bf.ReadString(), MapInstance.Lookup.Get<MapInstance>(mapNum).Revision + 1);
            LegacyDatabase.SaveGameObject(MapInstance.Lookup.Get<MapInstance>(mapNum));
            var tileDataLength = bf.ReadInteger();
            var tileData = bf.ReadBytes(tileDataLength);
            if (map.TileData != null) map.TileData = tileData;
            var attributeDataLength = bf.ReadInteger();
            var attributeData = bf.ReadBytes(attributeDataLength);
            map.LoadAttributes(attributeData);
            LegacyDatabase.SaveMapAttributes(map.Index, map.AttributesData());
            LegacyDatabase.SaveMapTiles(map.Index, tileData);
            Globals.Clients?.ForEach(t =>
            {
                if (t?.IsEditor ?? false) PacketSender.SendMapList(t);
            });
            var players = new List<Player>();
            foreach (var surrMap in map.GetSurroundingMaps(true))
            {
                players.AddRange(surrMap.GetPlayersOnMap().ToArray());
            }
            foreach (var player in players)
            {
                player.Warp(player.MapIndex, player.X, player.Y, player.Dir, false, player.Z, true);
                PacketSender.SendMap(player.MyClient, (int)mapNum);
            }
            PacketSender.SendMap(client, (int)mapNum, true); //Sends map to everyone/everything in proximity
            bf.Dispose();
        }

        private static void HandleCreateMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            var newMap = -1;
            var tmpMap = new MapInstance(-1);
            bf.WriteBytes(packet);
            var location = (int)bf.ReadInteger();
            if (location == -1)
            {
                var destType = bf.ReadInteger();
                newMap = LegacyDatabase.AddGameObject(GameObjectType.Map).Index;
                tmpMap = MapInstance.Lookup.Get<MapInstance>(newMap);
                LegacyDatabase.SaveGameObject(tmpMap);
                LegacyDatabase.GenerateMapGrids();
                PacketSender.SendMap(client, newMap, true);
                PacketSender.SendMapGridToAll(tmpMap.MapGrid);
                //FolderDirectory parent = null;
                destType = -1;
                if (destType == -1)
                {
                    MapList.GetList().AddMap(newMap, MapBase.Lookup);
                }
                LegacyDatabase.SaveGameDatabase();
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
                var relativeMap = (int)bf.ReadLong();
                switch (location)
                {
                    case 0:
                        if (MapInstance.Lookup.Get<MapInstance>(MapInstance.Lookup.Get<MapInstance>(relativeMap).Up) ==
                            null)
                        {
                            newMap = LegacyDatabase.AddGameObject(GameObjectType.Map).Index;
                            tmpMap = MapInstance.Lookup.Get<MapInstance>(newMap);
                            tmpMap.MapGrid = MapInstance.Lookup.Get<MapInstance>(relativeMap).MapGrid;
                            tmpMap.MapGridX = MapInstance.Lookup.Get<MapInstance>(relativeMap).MapGridX;
                            tmpMap.MapGridY = MapInstance.Lookup.Get<MapInstance>(relativeMap).MapGridY - 1;
                            MapInstance.Lookup.Get<MapInstance>(relativeMap).Up = newMap;
                        }
                        break;

                    case 1:
                        if (MapInstance.Lookup.Get<MapInstance>(MapInstance.Lookup.Get<MapInstance>(relativeMap)
                                .Down) == null)
                        {
                            newMap = LegacyDatabase.AddGameObject(GameObjectType.Map).Index;
                            tmpMap = MapInstance.Lookup.Get<MapInstance>(newMap);
                            tmpMap.MapGrid = MapInstance.Lookup.Get<MapInstance>(relativeMap).MapGrid;
                            tmpMap.MapGridX = MapInstance.Lookup.Get<MapInstance>(relativeMap).MapGridX;
                            tmpMap.MapGridY = MapInstance.Lookup.Get<MapInstance>(relativeMap).MapGridY + 1;
                            MapInstance.Lookup.Get<MapInstance>(relativeMap).Down = newMap;
                        }
                        break;

                    case 2:
                        if (MapInstance.Lookup.Get<MapInstance>(MapInstance.Lookup.Get<MapInstance>(relativeMap)
                                .Left) == null)
                        {
                            newMap = LegacyDatabase.AddGameObject(GameObjectType.Map).Index;
                            tmpMap = MapInstance.Lookup.Get<MapInstance>(newMap);
                            tmpMap.MapGrid = MapInstance.Lookup.Get<MapInstance>(relativeMap).MapGrid;
                            tmpMap.MapGridX = MapInstance.Lookup.Get<MapInstance>(relativeMap).MapGridX - 1;
                            tmpMap.MapGridY = MapInstance.Lookup.Get<MapInstance>(relativeMap).MapGridY;
                            MapInstance.Lookup.Get<MapInstance>(relativeMap).Left = newMap;
                        }
                        break;

                    case 3:
                        if (MapInstance.Lookup.Get<MapInstance>(MapInstance.Lookup.Get<MapInstance>(relativeMap)
                                .Right) == null)
                        {
                            newMap = LegacyDatabase.AddGameObject(GameObjectType.Map).Index;
                            tmpMap = MapInstance.Lookup.Get<MapInstance>(newMap);
                            tmpMap.MapGrid = MapInstance.Lookup.Get<MapInstance>(relativeMap).MapGrid;
                            tmpMap.MapGridX = MapInstance.Lookup.Get<MapInstance>(relativeMap).MapGridX + 1;
                            tmpMap.MapGridY = MapInstance.Lookup.Get<MapInstance>(relativeMap).MapGridY;
                            MapInstance.Lookup.Get<MapInstance>(relativeMap).Right = newMap;
                        }
                        break;
                }

                if (newMap > -1)
                {
                    LegacyDatabase.SaveGameObject(MapInstance.Lookup.Get<MapInstance>(relativeMap));

                    if (tmpMap.MapGridX >= 0 && tmpMap.MapGridX < LegacyDatabase.MapGrids[tmpMap.MapGrid].Width)
                    {
                        if (tmpMap.MapGridY + 1 < LegacyDatabase.MapGrids[tmpMap.MapGrid].Height)
                        {
                            tmpMap.Down = LegacyDatabase.MapGrids[tmpMap.MapGrid]
                                .MyGrid[tmpMap.MapGridX, tmpMap.MapGridY + 1];
                            if (tmpMap.Down > -1)
                            {
                                MapInstance.Lookup.Get<MapInstance>(tmpMap.Down).Up = newMap;
                                LegacyDatabase.SaveGameObject(MapInstance.Lookup.Get<MapInstance>(tmpMap.Down));
                            }
                        }
                        if (tmpMap.MapGridY - 1 >= 0)
                        {
                            tmpMap.Up = LegacyDatabase.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX, tmpMap.MapGridY - 1];
                            if (tmpMap.Up > -1)
                            {
                                MapInstance.Lookup.Get<MapInstance>(tmpMap.Up).Down = newMap;
                                LegacyDatabase.SaveGameObject(MapInstance.Lookup.Get<MapInstance>(tmpMap.Up));
                            }
                        }
                    }

                    if (tmpMap.MapGridY >= 0 && tmpMap.MapGridY < LegacyDatabase.MapGrids[tmpMap.MapGrid].Height)
                    {
                        if (tmpMap.MapGridX - 1 >= 0)
                        {
                            tmpMap.Left = LegacyDatabase.MapGrids[tmpMap.MapGrid]
                                .MyGrid[tmpMap.MapGridX - 1, tmpMap.MapGridY];
                            if (tmpMap.Left > -1)
                            {
                                MapInstance.Lookup.Get<MapInstance>(tmpMap.Left).Right = newMap;
                                LegacyDatabase.SaveGameObject(MapInstance.Lookup.Get<MapInstance>(tmpMap.Left));
                            }
                        }

                        if (tmpMap.MapGridX + 1 < LegacyDatabase.MapGrids[tmpMap.MapGrid].Width)
                        {
                            tmpMap.Right =
                                LegacyDatabase.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX + 1, tmpMap.MapGridY];
                            if (tmpMap.Right > -1)
                            {
                                MapInstance.Lookup.Get<MapInstance>(tmpMap.Right).Left = newMap;
                                LegacyDatabase.SaveGameObject(MapInstance.Lookup.Get<MapInstance>(tmpMap.Right));
                            }
                        }
                    }

                    LegacyDatabase.SaveGameObject(tmpMap);
                    LegacyDatabase.GenerateMapGrids();
                    PacketSender.SendMap(client, newMap, true);
                    PacketSender.SendMapGridToAll(MapInstance.Lookup.Get<MapInstance>(newMap).MapGrid);
                    PacketSender.SendEnterMap(client, newMap);
                    var folderDir = MapList.GetList().FindMapParent(relativeMap, null);
                    if (folderDir != null)
                    {
                        folderDir.Children.AddMap(newMap, MapBase.Lookup);
                    }
                    else
                    {
                        MapList.GetList().AddMap(newMap, MapBase.Lookup);
                    }
                    LegacyDatabase.SaveGameDatabase();
                    PacketSender.SendMapListToAll();
                }
            }
            bf.Dispose();
        }

        private static void HandleTryBlock(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);

            //check if player is blinded or stunned
            var statuses = client.Entity.Statuses.Values.ToArray();
            foreach (var status in statuses)
            {
                if (status.Type == (int)StatusTypes.Stun)
                {
                    PacketSender.SendPlayerMsg(client, Strings.Combat.stunblocking);
                    bf.Dispose();
                    return;
                }
            }

            client.Entity.TryBlock(bf.ReadInteger());

            bf.Dispose();
        }

        private static void HandleTryAttack(Client client, byte[] packet)
        {
            bool unequippedAttack = false;

            using (var buffer = new ByteBuffer())
            {
                buffer.WriteBytes(packet);
                long target = buffer.ReadLong();

                if (client.Entity.CastTime >= Globals.System.GetTimeMs())
                {
                    PacketSender.SendPlayerMsg(client, Strings.Combat.channelingnoattack);
                    return;
                }

                //check if player is blinded or stunned
                var statuses = client.Entity.Statuses.Values.ToArray();
                foreach (var status in statuses)
                {
                    if (status.Type == (int)StatusTypes.Stun)
                    {
                        PacketSender.SendPlayerMsg(client, Strings.Combat.stunattacking);
                        return;
                    }
                    if (status.Type == (int)StatusTypes.Blind)
                    {
                        PacketSender.SendActionMsg(client.Entity, Strings.Combat.miss,
                            CustomColors.Missed);
                        return;
                    }
                }

                var attackingTile = new TileHelper(client.Entity.MapIndex, client.Entity.X, client.Entity.Y);
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
                    if (client.Entity.Equipment[Options.WeaponIndex] >= 0 &&
                        ItemBase.Lookup.Get<ItemBase>(client.Entity
                            .Items[client.Entity.Equipment[Options.WeaponIndex]].ItemNum) !=
                        null)
                    {
                        ItemBase weaponItem = ItemBase.Lookup.Get<ItemBase>(client.Entity
                            .Items[client.Entity.Equipment[Options.WeaponIndex]].ItemNum);

                        //Check for animation
                        var attackAnim = ItemBase.Lookup.Get<ItemBase>(client.Entity.Items[client.Entity.Equipment[Options.WeaponIndex]].ItemNum).AttackAnimation;
                        if (attackAnim != null && attackingTile.TryFix())
                        {
                            PacketSender.SendAnimationToProximity(attackAnim.Index, -1, -1, attackingTile.GetMap(), attackingTile.GetX(), attackingTile.GetY(), client.Entity.Dir);
                        }

                        var weaponInvSlot = client.Entity.Equipment[Options.WeaponIndex];
                        var invItem = client.Entity.Items[weaponInvSlot];
                        var weapon = ItemBase.Lookup.Get<ItemBase>(invItem?.ItemNum ?? -1);
                        var projectileBase = ProjectileBase.Lookup.Get<ProjectileBase>(weapon?.Projectile ?? -1);

                        if (projectileBase != null)
                        {
                            if (projectileBase.AmmoItemId > -1)
                            {
                                var itemSlot = client.Entity.FindItem(projectileBase.AmmoItemId, projectileBase.AmmoRequired);
                                if (itemSlot == -1)
                                {
                                    PacketSender.SendPlayerMsg(client,
                                        Strings.Items.notenough.ToString(ItemBase.GetName(projectileBase.AmmoItemId)),
                                        CustomColors.NoAmmo);
                                    return;
                                }
#if INTERSECT_DIAGNOSTIC
                                PacketSender.SendPlayerMsg(client,
                                    Strings.Get("items", "notenough", $"REGISTERED_AMMO ({projectileBase.Ammo}:'{ItemBase.GetName(projectileBase.Ammo)}':{projectileBase.AmmoRequired})"),
                                    CustomColors.NoAmmo);
#endif
                                if (!client.Entity.TakeItemsByNum(projectileBase.AmmoItemId, projectileBase.AmmoRequired))
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
                            MapInstance.Lookup.Get<MapInstance>(client.Entity.MapIndex)
                                .SpawnMapProjectile(client.Entity, projectileBase, null, weaponItem,
                                    client.Entity.MapIndex,
                                    client.Entity.X, client.Entity.Y, client.Entity.Z,
                                    client.Entity.Dir, null);
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

                if (unequippedAttack)
                {
                    var classBase = ClassBase.Lookup.Get<ClassBase>(client.Entity.ClassIndex);
                    if (classBase != null)
                    {
                        //Check for animation
                        if (classBase.AttackAnimation != null)
                        {
                            PacketSender.SendAnimationToProximity(classBase.AttackAnimation, -1, -1, attackingTile.GetMap(), attackingTile.GetX(), attackingTile.GetY(), client.Entity.Dir);
                        }
                    }
                }

                //Attack normally.
                if (target > -1 && target < Globals.Entities.Count && Globals.Entities[(int)target] != null)
                    client.Entity.TryAttack(Globals.Entities[(int)target]);
            }
        }

        private static void HandleDir(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            client.Entity.ChangeDir((int)bf.ReadLong());
            bf.Dispose();
        }

        private static void HandleEnterGame(Client client, byte[] packet)
        {
            var index = client.EntityIndex;
            ((Player)client.Entity).InGame = true;
            PacketSender.SendTimeTo(client);
            PacketSender.SendGameData(client);
            if (client.Access == 1)
            {
                PacketSender.SendPlayerMsg(client, Strings.Player.modjoined,
                    CustomColors.ModJoined);
            }
            else if (client.Access == 2)
            {
                PacketSender.SendPlayerMsg(client, Strings.Player.adminjoined,
                    CustomColors.AdminJoined);
            }
            Globals.Entities[index].Warp(Globals.Entities[index].MapIndex, Globals.Entities[index].X,
                Globals.Entities[index].Y, Globals.Entities[index].Dir, false, Globals.Entities[index].Z);
            PacketSender.SendEntityDataTo(client, client.Entity);

            //Search for login activated events and run them
            foreach (EventBase evt in EventBase.Lookup.IndexValues)
            {
                if (evt != null)
                {
                    ((Player)client.Entity).StartCommonEvent(evt, (int)EventPage.CommonEventTriggers.JoinGame);
                }
            }
        }

        private static void HandleActivateEvent(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            ((Player)(client.Entity)).TryActivateEvent(bf.ReadInteger(), bf.ReadInteger());
            bf.Dispose();
        }

        private static void HandleEventResponse(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            ((Player)(client.Entity)).RespondToEvent(bf.ReadInteger(), bf.ReadInteger(), bf.ReadInteger());
            bf.Dispose();
        }

        private static void HandleCreateAccount(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var username = bf.ReadString();
            var password = bf.ReadString();
            var email = bf.ReadString();
            if (!FieldChecking.IsValidUsername(username, Strings.Regex.username))
            {
                PacketSender.SendLoginError(client, Strings.Account.invalidname);
                return;
            }
            if (!FieldChecking.IsWellformedEmailAddress(email, Strings.Regex.email))
            {
                PacketSender.SendLoginError(client, Strings.Account.invalidemail);
                return;
            }
            if (LegacyDatabase.AccountExists(username))
            {
                PacketSender.SendLoginError(client, Strings.Account.exists);
            }
            else
            {
                if (LegacyDatabase.EmailInUse(email))
                {
                    PacketSender.SendLoginError(client, Strings.Account.emailexists);
                }
                else
                {
                    LegacyDatabase.CreateAccount(client, username, password, email);
                    PacketSender.SendServerConfig(client);

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
            bf.Dispose();
        }

        private static void HandleCreateCharacter(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var name = bf.ReadString();
            if (!FieldChecking.IsValidUsername(name, Strings.Regex.username))
            {
                PacketSender.SendLoginError(client, Strings.Account.invalidname);
                return;
            }

            var Class = bf.ReadInteger();
            var sprite = bf.ReadInteger();
            var index = client.EntityIndex;
            var classBase = ClassBase.Lookup.Get<ClassBase>(Class);
            if (classBase == null || classBase.Locked)
            {
                PacketSender.SendLoginError(client, Strings.Account.invalidclass);
                return;
            }
            if (LegacyDatabase.CharacterNameInUse(name))
            {
                PacketSender.SendLoginError(client, Strings.Account.characterexists);
            }
            else
            {
                var newChar = new Player();
                client.Characters.Add(newChar);
                newChar.FixLists();
                client.LoadCharacter(newChar);
                var player = (Player)Globals.Entities[index];

                client.Entity = player;
                player.Name = name;
                player.ClassIndex = Class;
                if (classBase.Sprites.Count > 0)
                {
                    player.Sprite = classBase.Sprites[sprite].Sprite;
                    player.Face = classBase.Sprites[sprite].Face;
                    player.Gender = classBase.Sprites[sprite].Gender;
                }
                PacketSender.SendJoinGame(client);
                player.WarpToSpawn();

                player.SetMaxVital(Vitals.Health, classBase.BaseVital[(int)Vitals.Health]);
                player.SetMaxVital(Vitals.Mana, classBase.BaseVital[(int)Vitals.Mana]);
                player.SetVital(Vitals.Health, classBase.BaseVital[(int)Vitals.Health]);
                player.SetVital(Vitals.Mana, classBase.BaseVital[(int)Vitals.Mana]);

                for (int i = 0; i < (int)Stats.StatCount; i++)
                {
                    player.Stat[i].Stat = classBase.BaseStat[i];
                }
                player.StatPoints = classBase.BasePoints;

                for (int i = 0; i < classBase.Spells.Count; i++)
                {
                    if (classBase.Spells[i].Level <= 1)
                    {
                        Spell tempSpell = new Spell(classBase.Spells[i].Spell);
                        player.TryTeachSpell(tempSpell, false);
                    }
                }

                foreach (var item in classBase.Items)
                {
                    if (ItemBase.Lookup.Get<ItemBase>(item.Item) != null)
                    {
                        var tempItem = new Item(item.Item, item.Amount);
                        player.TryGiveItem(tempItem, false);
                    }
                }

                LegacyDatabase.SavePlayers();
            }
            bf.Dispose();
        }

        private static void HandlePickupItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            if (index < MapInstance.Lookup.Get<MapInstance>(client.Entity.MapIndex).MapItems.Count &&
                MapInstance.Lookup.Get<MapInstance>(client.Entity.MapIndex).MapItems[index] != null)
            {
                if (MapInstance.Lookup.Get<MapInstance>(client.Entity.MapIndex).MapItems[index].X ==
                    client.Entity.X &&
                    MapInstance.Lookup.Get<MapInstance>(client.Entity.MapIndex).MapItems[index].Y ==
                    client.Entity.Y)
                {
                    if (
                        client.Entity.TryGiveItem(MapInstance.Lookup.Get<MapInstance>(client.Entity.MapIndex) .MapItems[index]))
                    {
                        //Remove Item From Map
                        MapInstance.Lookup.Get<MapInstance>(client.Entity.MapIndex).RemoveItem(index);
                    }
                }
            }
            bf.Dispose();
        }

        private static void HandleSwapItems(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var item1 = bf.ReadInteger();
            var item2 = bf.ReadInteger();
            client.Entity.SwapItems(item1, item2);
            bf.Dispose();
        }

        private static void HandleDropItems(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slot = bf.ReadInteger();
            var amount = bf.ReadInteger();
            client.Entity.DropItems(slot, amount);
            bf.Dispose();
        }

        private static void HandleUseItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slot = bf.ReadInteger();
            client.Entity.UseItem(slot);
            bf.Dispose();
        }

        private static void HandleSwapSpells(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var spell1 = bf.ReadInteger();
            var spell2 = bf.ReadInteger();
            client.Entity.SwapSpells(spell1, spell2);
            bf.Dispose();
        }

        private static void HandleForgetSpell(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slot = bf.ReadInteger();
            client.Entity.ForgetSpell(slot);
            bf.Dispose();
        }

        private static void HandleUseSpell(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slot = bf.ReadInteger();
            var target = bf.ReadInteger();
            if (target > -1 && target <= Globals.Entities.Count)
            {
                client.Entity.UseSpell(slot, Globals.Entities[target]);
            }
            else
            {
                client.Entity.UseSpell(slot, null);
            }
            bf.Dispose();
        }

        private static void HandleUnequipItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slot = bf.ReadInteger();
            client.Entity.UnequipItem(slot);
            bf.Dispose();
        }

        private static void HandleUpgradeStat(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var stat = bf.ReadInteger();
            client.Entity.UpgradeStat(stat);
            bf.Dispose();
        }

        private static void HandleHotbarChange(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            var type = bf.ReadInteger();
            var slot = bf.ReadInteger();
            client.Entity.HotbarChange(index, type, slot);
            bf.Dispose();
        }

        private static void HandleMapListUpdate(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            int destType = -1;
            MapListFolder parent = null;
            int mapNum = -1;
            bf.WriteBytes(packet);
            var type = bf.ReadInteger();
            switch (type)
            {
                case (int)MapListUpdates.MoveItem:
                    MapList.GetList().HandleMove(bf.ReadInteger(), bf.ReadInteger(), bf.ReadInteger(),
                        bf.ReadInteger());
                    break;
                case (int)MapListUpdates.AddFolder:
                    destType = bf.ReadInteger();
                    parent = null;
                    if (destType == -1)
                    {
                        MapList.GetList().AddFolder(Strings.Mapping.newfolder);
                    }
                    else if (destType == 0)
                    {
                        parent = MapList.GetList().FindDir(bf.ReadInteger());
                        if (parent == null)
                        {
                            MapList.GetList().AddFolder(Strings.Mapping.newfolder);
                        }
                        else
                        {
                            parent.Children.AddFolder(Strings.Mapping.newfolder);
                        }
                    }
                    else if (destType == 1)
                    {
                        mapNum = bf.ReadInteger();
                        parent = MapList.GetList().FindMapParent(mapNum, null);
                        if (parent == null)
                        {
                            MapList.GetList().AddFolder(Strings.Mapping.newfolder);
                        }
                        else
                        {
                            parent.Children.AddFolder(Strings.Mapping.newfolder);
                        }
                    }
                    break;
                case (int)MapListUpdates.Rename:
                    destType = bf.ReadInteger();
                    parent = null;
                    if (destType == 0)
                    {
                        parent = MapList.GetList().FindDir(bf.ReadInteger());
                        parent.Name = bf.ReadString();
                        PacketSender.SendMapListToAll();
                    }
                    else if (destType == 1)
                    {
                        mapNum = bf.ReadInteger();
                        MapInstance.Lookup.Get<MapInstance>(mapNum).Name = bf.ReadString();
                        LegacyDatabase.SaveGameObject(MapInstance.Lookup.Get<MapInstance>(mapNum));
                        PacketSender.SendMapListToAll();
                    }
                    break;
                case (int)MapListUpdates.Delete:
                    destType = bf.ReadInteger();
                    parent = null;
                    if (destType == 0)
                    {
                        MapList.GetList().DeleteFolder(bf.ReadInteger());
                        PacketSender.SendMapListToAll();
                    }
                    else if (destType == 1)
                    {
                        if (MapInstance.Lookup.Count == 1)
                        {
                            PacketSender.SendAlert(client, Strings.Mapping.lastmap,
                                Strings.Mapping.lastmaperror);
                            return;
                        }
                        mapNum = bf.ReadInteger();
                        var players = MapInstance.Lookup.Get<MapInstance>(mapNum).GetPlayersOnMap();
                        MapList.GetList().DeleteMap(mapNum);
                        LegacyDatabase.SaveGameObject(MapInstance.Lookup.Get<MapInstance>(mapNum));
                        LegacyDatabase.DeleteGameObject(MapInstance.Lookup.Get<MapInstance>(mapNum));
                        LegacyDatabase.GenerateMapGrids();
                        PacketSender.SendMapListToAll();
                        foreach (var player in players)
                        {
                            player.WarpToSpawn();
                        }
                        PacketSender.SendMapToEditors(mapNum);
                    }
                    break;
            }
            PacketSender.SendMapListToAll();
            LegacyDatabase.SaveGameDatabase();
            bf.Dispose();
        }

        private static void HandleOpenAdminWindow(Client client)
        {
            if (client.Access > 0)
            {
                PacketSender.SendMapList(client);
                PacketSender.SendOpenAdminWindow(client);
            }
        }

        private static void HandleAdminAction(Client client, byte[] packet)
        {
            if (client.Access == 0)
            {
                return;
            }
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = bf.ReadInteger();
            string val1 = bf.ReadString();
            string val2 = bf.ReadString();
            string val3 = bf.ReadString();
            string val4 = bf.ReadString();

            switch (type)
            {
                case (int)AdminActions.WarpTo:
                    client.Entity.Warp(Convert.ToInt32(val1), client.Entity.X, client.Entity.Y);
                    break;
                case (int)AdminActions.WarpMeTo:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.Name.ToLower())
                            {
                                client.Entity.Warp(Globals.Clients[i].Entity.MapIndex,
                                    Globals.Clients[i].Entity.X, Globals.Clients[i].Entity.Y);
                                PacketSender.SendPlayerMsg(client, Strings.Player.warpedto.ToString(val1));
                                PacketSender.SendPlayerMsg(Globals.Clients[i],
                                    Strings.Player.warpedtoyou.ToString(client.Entity.Name));
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, Strings.Player.offline);
                    break;
                case (int)AdminActions.WarpToMe:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.Name.ToLower())
                            {
                                Globals.Clients[i].Entity.Warp(client.Entity.MapIndex, client.Entity.X,
                                    client.Entity.Y);
                                PacketSender.SendPlayerMsg(client, Strings.Player.haswarpedto.ToString(val1),
                                    client.Entity.Name);
                                PacketSender.SendPlayerMsg(Globals.Clients[i],
                                    Strings.Player.beenwarpedto.ToString(client.Entity.Name), client.Entity.Name);
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, Strings.Player.offline);
                    break;
                case (int)AdminActions.WarpToLoc:
                    if (client.Access > 0)
                    {
                        client.Entity.Warp(Convert.ToInt32(val1), Convert.ToInt32(val2), Convert.ToInt32(val3), 0, true);
                    }
                    break;
                case (int)AdminActions.Kick:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.Name.ToLower())
                            {
                                PacketSender.SendGlobalMsg(Strings.Player.kicked.ToString(
                                    Globals.Clients[i].Entity.Name, client.Entity.Name));
                                Globals.Clients[i].Disconnect(); //Kick em'
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, Strings.Player.offline);
                    break;
                case (int)AdminActions.Kill:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.Name.ToLower())
                            {
                                Globals.Clients[i].Entity.Die(); //Kill em'
                                PacketSender.SendGlobalMsg(Strings.Player.killed.ToString(
                                    Globals.Clients[i].Entity.Name, client.Entity.Name));
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, Strings.Player.offline);
                    break;
                case (int)AdminActions.SetSprite:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.Name.ToLower())
                            {
                                Globals.Clients[i].Entity.Sprite = val2;
                                PacketSender.SendEntityDataToProximity(Globals.Clients[i].Entity);
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, Strings.Player.offline);
                    break;
                case (int)AdminActions.SetFace:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.Name.ToLower())
                            {
                                Globals.Clients[i].Entity.Face = val2;
                                PacketSender.SendEntityDataToProximity(Globals.Clients[i].Entity);
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, Strings.Player.offline);
                    break;
                case (int)AdminActions.SetAccess:
                    int p;
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.Name.ToLower())
                            {
                                if (val1.ToLower() != client.Entity.Name.ToLower()) //Can't increase your own power!
                                {
                                    if (client.Access == 2)
                                    {
                                        if (val2 == "Admin")
                                        {
                                            p = 2;
                                        }
                                        else if (val2 == "Moderator")
                                        {
                                            p = 1;
                                        }
                                        else
                                        {
                                            p = 0;
                                        }

                                        var targetClient = Globals.Clients[i];
                                        targetClient.Access = p;
                                        if (targetClient.Access == 2)
                                        {
                                            PacketSender.SendGlobalMsg(Strings.Player.admin.ToString(val1));
                                        }
                                        else if (targetClient.Access == 1)
                                        {
                                            PacketSender.SendGlobalMsg(Strings.Player.mod.ToString(val1));
                                        }
                                        else
                                        {
                                            PacketSender.SendGlobalMsg(Strings.Player.deadmin.ToString(val1));
                                        }
                                        return;
                                    }
                                    else
                                    {
                                        PacketSender.SendPlayerMsg(client, Strings.Player.adminsetpower);
                                        return;
                                    }
                                }
                                else
                                {
                                    PacketSender.SendPlayerMsg(client, Strings.Player.changeownpower);
                                    return;
                                }
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, Strings.Player.offline);
                    break;
                case (int)AdminActions.UnMute:
                    var unmutedUser = LegacyDatabase.GetUser(val1);
                    if (unmutedUser != null)
                    {
                        Mute.DeleteMute(unmutedUser);
                        PacketSender.SendPlayerMsg(client, Strings.Account.unmuted.ToString(val1));
                    }
                    else
                    {
                        PacketSender.SendPlayerMsg(client, Strings.Account.notfound.ToString(val1));
                    }
                    break;
                case (int)AdminActions.UnBan:
                    var unbannedUser = LegacyDatabase.GetUser(val1);
                    if (unbannedUser != null)
                    {
                        Ban.DeleteBan(unbannedUser);
                        PacketSender.SendPlayerMsg(client, Strings.Account.unbanned.ToString(val1));
                    }
                    else
                    {
                        PacketSender.SendPlayerMsg(client, Strings.Account.notfound.ToString(val1));
                    }
                    break;
                case (int)AdminActions.Mute:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.Name.ToLower())
                            {
                                if (Convert.ToBoolean(val4) == true)
                                {
                                    Mute.AddMute(Globals.Clients[i], Convert.ToInt32(val2), val3,
                                        client.Entity.Name, Globals.Clients[i].GetIp());
                                }
                                else
                                {
                                    Mute.AddMute(Globals.Clients[i], Convert.ToInt32(val2), val3,
                                        client.Entity.Name, "");
                                }
                                PacketSender.SendGlobalMsg(Strings.Account.muted.ToString(
                                    Globals.Clients[i].Entity.Name));
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, Strings.Player.offline);
                    break;
                case (int)AdminActions.Ban:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.Name.ToLower())
                            {
                                if (Convert.ToBoolean(val4) == true)
                                {
                                    Ban.AddBan(Globals.Clients[i], Convert.ToInt32(val2), val3,
                                        client.Entity.Name, Globals.Clients[i].GetIp());
                                }
                                else
                                {
                                    Ban.AddBan(Globals.Clients[i], Convert.ToInt32(val2), val3,
                                        client.Entity.Name, "");
                                }

                                PacketSender.SendGlobalMsg(Strings.Account.banned.ToString(
                                    Globals.Clients[i].Entity.Name));
                                Globals.Clients[i].Disconnect(); //Kick em'
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, Strings.Player.offline);
                    break;
            }
        }

        private static void HandleNeedGrid(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = (int)bf.ReadLong();
            if (MapInstance.Lookup.IndexKeys.Contains(mapNum))
            {
                if (client.IsEditor)
                {
                    PacketSender.SendMapGrid(client, MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid);
                }
            }
        }

        private static void HandleUnlinkMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = (int)bf.ReadLong();
            int curMap = (int)bf.ReadLong();
            int mapGrid = 0;
            if (MapInstance.Lookup.IndexKeys.Contains(mapNum))
            {
                if (client.IsEditor)
                {
                    if (MapInstance.Lookup.Get<MapInstance>(mapNum) != null)
                    {
                        MapInstance.Lookup.Get<MapInstance>(mapNum).ClearConnections();

                        int gridX = MapInstance.Lookup.Get<MapInstance>(mapNum).MapGridX;
                        int gridY = MapInstance.Lookup.Get<MapInstance>(mapNum).MapGridY;

                        //Up
                        if (gridY - 1 >= 0 &&
                            LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid]
                                .MyGrid[gridX, gridY - 1] > -1)
                        {
                            if (
                                MapInstance.Lookup.Get<MapInstance>(
                                    LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid]
                                        .MyGrid[gridX, gridY - 1]) !=
                                null)
                                MapInstance.Lookup.Get<MapInstance>(
                                        LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid]
                                            .MyGrid[gridX, gridY - 1])
                                    .ClearConnections((int)Directions.Down);
                        }

                        //Down
                        if (gridY + 1 < LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid].Height &&
                            LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid]
                                .MyGrid[gridX, gridY + 1] > -1)
                        {
                            if (
                                MapInstance.Lookup.Get<MapInstance>(
                                    LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid]
                                        .MyGrid[gridX, gridY + 1]) !=
                                null)
                                MapInstance.Lookup.Get<MapInstance>(
                                        LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid]
                                            .MyGrid[gridX, gridY + 1])
                                    .ClearConnections((int)Directions.Up);
                        }

                        //Left
                        if (gridX - 1 >= 0 &&
                            LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid]
                                .MyGrid[gridX - 1, gridY] > -1)
                        {
                            if (
                                MapInstance.Lookup.Get<MapInstance>(
                                    LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid]
                                        .MyGrid[gridX - 1, gridY]) !=
                                null)
                                MapInstance.Lookup.Get<MapInstance>(
                                        LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid]
                                            .MyGrid[gridX - 1, gridY])
                                    .ClearConnections((int)Directions.Right);
                        }

                        //Right
                        if (gridX + 1 < LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid].Width &&
                            LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid]
                                .MyGrid[gridX + 1, gridY] > -1)
                        {
                            if (
                                MapInstance.Lookup.Get<MapInstance>(
                                    LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid]
                                        .MyGrid[gridX + 1, gridY]) !=
                                null)
                                MapInstance.Lookup.Get<MapInstance>(
                                        LegacyDatabase.MapGrids[MapInstance.Lookup.Get<MapInstance>(mapNum).MapGrid]
                                            .MyGrid[gridX + 1, gridY])
                                    .ClearConnections((int)Directions.Left);
                        }

                        LegacyDatabase.GenerateMapGrids();
                        if (MapInstance.Lookup.IndexKeys.Contains(curMap))
                        {
                            mapGrid = MapInstance.Lookup.Get<MapInstance>(curMap).MapGrid;
                        }
                    }
                    PacketSender.SendMapGridToAll(mapGrid);
                }
            }
        }

        private static void HandleLinkMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int adjacentMap = (int)bf.ReadLong();
            int linkMap = (int)bf.ReadLong();
            long gridX = bf.ReadLong();
            long gridY = bf.ReadLong();
            bool canLink = true;
            if (MapInstance.Lookup.IndexKeys.Contains(linkMap) &&
                MapInstance.Lookup.IndexKeys.Contains(adjacentMap))
            {
                //Clear to test if we can link.
                int linkGrid = MapInstance.Lookup.Get<MapInstance>(linkMap).MapGrid;
                int adjacentGrid = MapInstance.Lookup.Get<MapInstance>(adjacentMap).MapGrid;
                if (linkGrid != adjacentGrid)
                {
                    long xOffset = MapInstance.Lookup.Get<MapInstance>(linkMap).MapGridX - gridX;
                    long yOffset = MapInstance.Lookup.Get<MapInstance>(linkMap).MapGridY - gridY;
                    for (int x = 0; x < LegacyDatabase.MapGrids[adjacentGrid].Width; x++)
                    {
                        for (int y = 0; y < LegacyDatabase.MapGrids[adjacentGrid].Height; y++)
                        {
                            if (x + xOffset >= 0 && x + xOffset < LegacyDatabase.MapGrids[linkGrid].Width &&
                                y + yOffset >= 0 &&
                                y + yOffset < LegacyDatabase.MapGrids[linkGrid].Height)
                            {
                                if (LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x, y] != -1 &&
                                    LegacyDatabase.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset] != -1)
                                {
                                    //Incompatible Link!
                                    PacketSender.SendAlert(client, Strings.Mapping.linkfail,
                                        Strings.Mapping.linkfailerror.ToString(MapBase.GetName(linkMap),
                                            MapBase.GetName(adjacentMap),
                                            MapBase.GetName(LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x, y]),
                                            MapBase.GetName(
                                                LegacyDatabase.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset])));
                                    return;
                                }
                            }
                        }
                    }
                    if (canLink)
                    {
                        for (int x = -1; x < LegacyDatabase.MapGrids[adjacentGrid].Width + 1; x++)
                        {
                            for (int y = -1; y < LegacyDatabase.MapGrids[adjacentGrid].Height + 1; y++)
                            {
                                if (x + xOffset >= 0 && x + xOffset < LegacyDatabase.MapGrids[linkGrid].Width &&
                                    y + yOffset >= 0 && y + yOffset < LegacyDatabase.MapGrids[linkGrid].Height)
                                {
                                    if (LegacyDatabase.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset] != -1)
                                    {
                                        bool inXBounds = x > -1 &&
                                                         x < LegacyDatabase.MapGrids[adjacentGrid].Width;
                                        bool inYBounds = y > -1 &&
                                                         y < LegacyDatabase.MapGrids[adjacentGrid].Height;
                                        if (inXBounds && inYBounds)
                                            LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x, y] =
                                                LegacyDatabase.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];

                                        if (inXBounds && y - 1 >= 0 &&
                                            LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x, y - 1] > -1)
                                        {
                                            MapInstance.Lookup.Get<MapInstance>(
                                                    LegacyDatabase.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Up =
                                                LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x, y - 1];
                                            MapInstance.Lookup
                                                    .Get<MapInstance>(LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x, y - 1])
                                                    .Down =
                                                LegacyDatabase.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            LegacyDatabase.SaveGameObject(
                                                MapInstance.Lookup.Get<MapInstance>(LegacyDatabase.MapGrids[adjacentGrid]
                                                    .MyGrid[x, y - 1]));
                                        }

                                        if (inXBounds && y + 1 < LegacyDatabase.MapGrids[adjacentGrid].Height &&
                                            LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x, y + 1] > -1)
                                        {
                                            MapInstance.Lookup.Get<MapInstance>(
                                                    LegacyDatabase.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Down =
                                                LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x, y + 1];
                                            MapInstance.Lookup
                                                    .Get<MapInstance>(LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x, y + 1])
                                                    .Up =
                                                LegacyDatabase.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            LegacyDatabase.SaveGameObject(
                                                MapInstance.Lookup.Get<MapInstance>(LegacyDatabase.MapGrids[adjacentGrid]
                                                    .MyGrid[x, y + 1]));
                                        }

                                        if (inYBounds && x - 1 >= 0 &&
                                            LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x - 1, y] > -1)
                                        {
                                            MapInstance.Lookup.Get<MapInstance>(
                                                    LegacyDatabase.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Left =
                                                LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x - 1, y];
                                            MapInstance.Lookup
                                                    .Get<MapInstance>(LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x - 1, y])
                                                    .Right =
                                                LegacyDatabase.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            LegacyDatabase.SaveGameObject(
                                                MapInstance.Lookup.Get<MapInstance>(LegacyDatabase.MapGrids[adjacentGrid]
                                                    .MyGrid[x - 1, y]));
                                        }

                                        if (inYBounds && x + 1 < LegacyDatabase.MapGrids[adjacentGrid].Width &&
                                            LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x + 1, y] > -1)
                                        {
                                            MapInstance.Lookup.Get<MapInstance>(
                                                    LegacyDatabase.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Right
                                                =
                                                LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x + 1, y];
                                            MapInstance.Lookup
                                                    .Get<MapInstance>(LegacyDatabase.MapGrids[adjacentGrid].MyGrid[x + 1, y])
                                                    .Left =
                                                LegacyDatabase.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            LegacyDatabase.SaveGameObject(
                                                MapInstance.Lookup.Get<MapInstance>(LegacyDatabase.MapGrids[adjacentGrid]
                                                    .MyGrid[x + 1, y]));
                                        }

                                        LegacyDatabase.SaveGameObject(
                                            MapInstance.Lookup.Get<MapInstance>(
                                                LegacyDatabase.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]));
                                    }
                                }
                            }
                        }
                        LegacyDatabase.SaveGameObject(MapInstance.Lookup.Get<MapInstance>(linkMap));
                        LegacyDatabase.GenerateMapGrids();
                        PacketSender.SendMapGridToAll(MapInstance.Lookup.Get<MapInstance>(adjacentMap).MapGrid);
                    }
                }
            }
        }

        private static void HandleBuyItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slot = bf.ReadInteger();
            var amount = bf.ReadInteger();
            client.Entity.BuyItem(slot, amount);
            bf.Dispose();
        }

        private static void HandleSellItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slot = bf.ReadInteger();
            var amount = bf.ReadInteger();
            client.Entity.SellItem(slot, amount);
            bf.Dispose();
        }

        private static void HandleCloseShop(Client client, byte[] packet)
        {
            client.Entity.CloseShop();
        }

        private static void HandleCloseCrafting(Client client, byte[] packet)
        {
            client.Entity.CloseCraftingTable();
        }

        private static void HandleCraftItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            client.Entity.CraftIndex = bf.ReadInteger();
            client.Entity.CraftTimer = Globals.System.GetTimeMs();
            bf.Dispose();
        }

        private static void HandleCloseBank(Client client, byte[] packet)
        {
            client.Entity.CloseBank();
        }

        private static void HandleDepositItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slot = bf.ReadInteger();
            var amount = bf.ReadInteger();
            client.Entity.DepositItem(slot, amount);
            bf.Dispose();
        }

        private static void HandleWithdrawItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slot = bf.ReadInteger();
            var amount = bf.ReadInteger();
            client.Entity.WithdrawItem(slot, amount);
            bf.Dispose();
        }

        private static void HandleMoveBankItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var item1 = bf.ReadInteger();
            var item2 = bf.ReadInteger();
            client.Entity.SwapBankItems(item1, item2);
            bf.Dispose();
        }

        private static void HandleCreateGameObject(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = (GameObjectType)bf.ReadInteger();
            var obj = LegacyDatabase.AddGameObject(type);
            PacketSender.SendGameObjectToAll(obj);
            bf.Dispose();
        }

        private static void HandleRequestOpenEditor(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = (GameObjectType)bf.ReadInteger();
            PacketSender.SendGameObjects(client, type);
            PacketSender.SendOpenEditor(client, type);
            bf.Dispose();
        }

        private void HandleDeleteGameObject(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = (GameObjectType)bf.ReadInteger();
            var id = bf.ReadInteger();
            // TODO: YO COME DO THIS
            IDatabaseObject obj = null;
            switch (type)
            {
                case GameObjectType.Animation:
                    obj = AnimationBase.Lookup.Get<AnimationBase>(id);
                    break;
                case GameObjectType.Class:
                    if (ClassBase.Lookup.Count == 1)
                    {
                        PacketSender.SendAlert(client, Strings.Classes.lastclass,
                            Strings.Classes.lastclasserror);
                        return;
                    }
                    obj = DatabaseObject<ClassBase>.Lookup.Get(id);
                    break;
                case GameObjectType.Item:
                    obj = ItemBase.Lookup.Get<ItemBase>(id);
                    break;
                case GameObjectType.Npc:
                    obj = NpcBase.Lookup.Get<NpcBase>(id);
                    break;
                case GameObjectType.Projectile:
                    obj = ProjectileBase.Lookup.Get<ProjectileBase>(id);
                    break;
                case GameObjectType.Quest:
                    obj = QuestBase.Lookup.Get<QuestBase>(id);
                    break;
                case GameObjectType.Resource:
                    obj = ResourceBase.Lookup.Get<ResourceBase>(id);
                    break;
                case GameObjectType.Shop:
                    obj = ShopBase.Lookup.Get<ShopBase>(id);
                    break;
                case GameObjectType.Spell:
                    obj = SpellBase.Lookup.Get<SpellBase>(id);
                    break;
                case GameObjectType.CraftTables:
                    obj = DatabaseObject<CraftingTableBase>.Lookup.Get(id);
                    break;
                case GameObjectType.Crafts:
                    obj = DatabaseObject<CraftBase>.Lookup.Get(id);
                    break;
                case GameObjectType.Map:
                    break;
                case GameObjectType.CommonEvent:
                    obj = EventBase.Lookup.Get<EventBase>(id);
                    break;
                case GameObjectType.PlayerSwitch:
                    obj = PlayerSwitchBase.Lookup.Get<PlayerSwitchBase>(id);
                    break;
                case GameObjectType.PlayerVariable:
                    obj = PlayerVariableBase.Lookup.Get<PlayerVariableBase>(id);
                    break;
                case GameObjectType.ServerSwitch:
                    obj = ServerSwitchBase.Lookup.Get<ServerSwitchBase>(id);
                    break;
                case GameObjectType.ServerVariable:
                    obj = ServerVariableBase.Lookup.Get<ServerVariableBase>(id);
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
                LegacyDatabase.DeleteGameObject(obj);
                LegacyDatabase.SaveGameDatabase();
                PacketSender.SendGameObjectToAll(obj, true);
            }
            bf.Dispose();
        }

        private void HandleSaveGameObject(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = (GameObjectType)bf.ReadInteger();
            var id = bf.ReadInteger();
            IDatabaseObject obj = null;
            switch (type)
            {
                case GameObjectType.Animation:
                    obj = AnimationBase.Lookup.Get<AnimationBase>(id);
                    break;
                case GameObjectType.Class:
                    obj = DatabaseObject<ClassBase>.Lookup.Get(id);
                    break;
                case GameObjectType.Item:
                    obj = ItemBase.Lookup.Get<ItemBase>(id);
                    break;
                case GameObjectType.Npc:
                    obj = NpcBase.Lookup.Get<NpcBase>(id);
                    break;
                case GameObjectType.Projectile:
                    obj = ProjectileBase.Lookup.Get<ProjectileBase>(id);
                    break;
                case GameObjectType.Quest:
                    obj = QuestBase.Lookup.Get<QuestBase>(id);
                    break;
                case GameObjectType.Resource:
                    obj = ResourceBase.Lookup.Get<ResourceBase>(id);
                    break;
                case GameObjectType.Shop:
                    obj = ShopBase.Lookup.Get<ShopBase>(id);
                    break;
                case GameObjectType.Spell:
                    obj = SpellBase.Lookup.Get<SpellBase>(id);
                    break;
                case GameObjectType.CraftTables:
                    obj = DatabaseObject<CraftingTableBase>.Lookup.Get(id);
                    break;
                case GameObjectType.Crafts:
                    obj = DatabaseObject<CraftBase>.Lookup.Get(id);
                    break;
                case GameObjectType.Map:
                    break;
                case GameObjectType.CommonEvent:
                    obj = EventBase.Lookup.Get<EventBase>(id);
                    break;
                case GameObjectType.PlayerSwitch:
                    obj = PlayerSwitchBase.Lookup.Get<PlayerSwitchBase>(id);
                    break;
                case GameObjectType.PlayerVariable:
                    obj = PlayerVariableBase.Lookup.Get<PlayerVariableBase>(id);
                    break;
                case GameObjectType.ServerSwitch:
                    obj = ServerSwitchBase.Lookup.Get<ServerSwitchBase>(id);
                    break;
                case GameObjectType.ServerVariable:
                    obj = ServerVariableBase.Lookup.Get<ServerVariableBase>(id);
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
                    
                    //Must detach owned entities and reattach after json populates -_-
                    LegacyDatabase.DeteachOwnedType(((ResourceBase)obj).Initial);
                    LegacyDatabase.DeteachOwnedType(((ResourceBase)obj).Exhausted);
                }
                else if (type == GameObjectType.Npc)
                {
                    Globals.KillNpcsOf((NpcBase)obj);
                }
                else if (type == GameObjectType.Projectile)
                {
                    Globals.KillProjectilesOf((ProjectileBase)obj);
                }
                else if (type == GameObjectType.Animation)
                {
                    //Must detach owned entities and reattach after json populates -_-
                    LegacyDatabase.DeteachOwnedType(((AnimationBase)obj).Upper);
                    LegacyDatabase.DeteachOwnedType(((AnimationBase)obj).Lower);
                }

                JsonConvert.PopulateObject(bf.ReadString(), obj, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
                
                if (type == GameObjectType.Resource)
                {
                    //Must detach owned entities and reattach after json populates -_-
                    LegacyDatabase.AttachOwnedType(((ResourceBase)obj).Initial);
                    LegacyDatabase.AttachOwnedType(((ResourceBase)obj).Exhausted);
                }
                else if (type == GameObjectType.Animation)
                {
                    //Must detach owned entities and reattach after json populates -_-
                    LegacyDatabase.AttachOwnedType(((AnimationBase)obj).Upper);
                    LegacyDatabase.AttachOwnedType(((AnimationBase)obj).Lower);
                }
                else if (type == GameObjectType.Quest)
                {
                    var qst = (QuestBase) obj;
                    foreach (var evt in qst.RemoveEvents)
                    {
                        var evtb = EventBase.Get(evt);
                        if (evtb != null) LegacyDatabase.DeleteGameObject(evtb);
                    }
                    foreach (var evt in qst.AddEvents)
                    {
                        var evtb = (EventBase)LegacyDatabase.AddGameObject(GameObjectType.CommonEvent);
                        qst.Tasks[evt.Key].CompletionEvent = evtb;
                        JsonConvert.PopulateObject(JsonConvert.SerializeObject(evt.Value),evtb, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
                    }
                    qst.AddEvents.Clear();
                    qst.RemoveEvents.Clear();
                }

                PacketSender.SendGameObjectToAll(obj, false);
                LegacyDatabase.SaveGameObject(obj);
            }
            bf.Dispose();
        }

        private void HandleSaveTime(Client client, byte[] packet)
        {
            if (client.IsEditor)
            {
                TimeBase.GetTimeBase().LoadTimeBase(packet);
                LegacyDatabase.SaveGameDatabase();
                ServerTime.Init();
                PacketSender.SendTimeBaseToAllEditors();
            }
        }

        private static void HandlePartyInvite(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int target = bf.ReadInteger();
            //Check if valid target
            if (target < 0 || target > Globals.Entities.Count)
            {
                return;
            }
            if (Globals.Entities[target] != null && Globals.Entities[target].GetEntityType() == EntityTypes.Player &&
                target != client.Entity.MyIndex)
            {
                ((Player)Globals.Entities[target]).InviteToParty(client.Entity);
            }
            else
            {
                PacketSender.SendPlayerMsg(client, Strings.Player.notarget, CustomColors.NoTarget);
            }
            bf.Dispose();
        }

        private static void HandleAcceptPartyInvite(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int leader = bf.ReadInteger();
            if (client.Entity.PartyRequester != null && client.Entity.PartyRequester.MyIndex == leader)
            {
                if (client.Entity.PartyRequester.IsValidPlayer)
                {
                    client.Entity.PartyRequester.AddParty(client.Entity);
                }

                client.Entity.PartyRequester = null;
            }
            bf.Dispose();
        }

        private static void HandleDeclinePartyInvite(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int leader = bf.ReadInteger();
            if (client.Entity.PartyRequester != null && client.Entity.PartyRequester.MyIndex == leader)
            {
                if (client.Entity.PartyRequester.IsValidPlayer)
                {
                    PacketSender.SendPlayerMsg(client.Entity.PartyRequester.MyClient,
                        Strings.Parties.declined.ToString(client.Entity.Name), CustomColors.Declined);

                    if (client.Entity.PartyRequests.ContainsKey(client.Entity.PartyRequester))
                    {
                        client.Entity.PartyRequests[client.Entity.PartyRequester] = Globals.System.GetTimeMs() +
                                                                                    Player.REQUEST_DECLINE_TIMEOUT;
                    }
                    else
                    {
                        client.Entity.PartyRequests.Add(client.Entity.PartyRequester,
                            Globals.System.GetTimeMs() + Player.REQUEST_DECLINE_TIMEOUT);
                    }
                }
                client.Entity.PartyRequester = null;
            }
        }

        private static void HandlePartyKick(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int target = bf.ReadInteger();
            client.Entity.KickParty(target);
            bf.Dispose();
        }

        private static void HandlePartyLeave(Client client, byte[] packet)
        {
            client.Entity.LeaveParty();
        }

        private static void HandleAcceptQuest(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int questId = bf.ReadInteger();
            client.Entity.AcceptQuest(questId);
            bf.Dispose();
        }

        private static void HandleDeclineQuest(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int questId = bf.ReadInteger();
            client.Entity.DeclineQuest(questId);
            bf.Dispose();
        }

        private static void HandleCancelQuest(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int questId = bf.ReadInteger();
            client.Entity.CancelQuest(questId);
            bf.Dispose();
        }

        private static void HandleTradeRequest(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int target = bf.ReadInteger();
            //Check if valid target
            if (target < 0 || target > Globals.Entities.Count)
            {
                return;
            }
            if (Globals.Entities[target] != null && Globals.Entities[target].GetEntityType() == EntityTypes.Player &&
                target != client.Entity.MyIndex)
            {
                ((Player)Globals.Entities[target]).InviteToTrade(client.Entity);
            }
            bf.Dispose();
        }

        private static void HandleTradeRequestAccept(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int target = bf.ReadInteger();
            if (client.Entity.Trading.Requester != null && client.Entity.Trading.Requester.MyIndex == target)
            {
                if (client.Entity.Trading.Requester.IsValidPlayer)
                {
                    if (client.Entity.Trading.Requester.Trading.Counterparty == null) //They could have accepted another trade since.
                    {
                        client.Entity.Trading.Requester.StartTrade(client.Entity);
                    }
                    else
                    {
                        PacketSender.SendPlayerMsg(client, Strings.Trading.busy.ToString(
                            client.Entity.Trading.Requester.Name), Color.Red);
                    }
                }

                client.Entity.Trading.Requester = null;
            }
            bf.Dispose();
        }

        private static void HandleTradeRequestDecline(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int target = bf.ReadInteger();
            if (client.Entity.Trading.Requester != null && client.Entity.Trading.Requester.MyIndex == target)
            {
                if (client.Entity.Trading.Requester.IsValidPlayer)
                {
                    PacketSender.SendPlayerMsg(client.Entity.Trading.Requester.MyClient,
                        Strings.Trading.declined.ToString(client.Entity.Name), CustomColors.Declined);
                    if (client.Entity.Trading.Requests.ContainsKey(client.Entity.Trading.Requester))
                    {
                        client.Entity.Trading.Requests[client.Entity.Trading.Requester] = Globals.System.GetTimeMs() +
                                                                                    Player.REQUEST_DECLINE_TIMEOUT;
                    }
                    else
                    {
                        client.Entity.Trading.Requests.Add(client.Entity.Trading.Requester,
                            Globals.System.GetTimeMs() + Player.REQUEST_DECLINE_TIMEOUT);
                    }
                }
                client.Entity.Trading.Requester = null;
            }
        }

        private static void HandleTradeOffer(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slot = bf.ReadInteger();
            var amount = bf.ReadInteger();
            client.Entity.OfferItem(slot, amount);
            bf.Dispose();
        }

        private static void HandleTradeRevoke(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slot = bf.ReadInteger();
            var amount = bf.ReadInteger();
            client.Entity.RevokeItem(slot, amount);
            bf.Dispose();
        }

        private static void HandleTradeAccept(Client client, byte[] packet)
        {
            client.Entity.Trading.Accepted = true;
            if (client.Entity.Trading.Counterparty.Trading.Accepted)
            {
                Item[] t = new Item[Options.MaxInvItems];

                //Swap the trade boxes over, then return the trade boxes to their new owners!
                t = client.Entity.Trading.Offer;
                client.Entity.Trading.Offer = client.Entity.Trading.Counterparty.Trading.Offer;
                client.Entity.Trading.Counterparty.Trading.Offer = t;
                client.Entity.Trading.Counterparty.ReturnTradeItems();
                client.Entity.ReturnTradeItems();

                PacketSender.SendPlayerMsg(client, Strings.Trading.accepted, CustomColors.Accepted);
                PacketSender.SendPlayerMsg(client.Entity.Trading.Counterparty.MyClient, Strings.Trading.accepted, CustomColors.Accepted);
                PacketSender.SendTradeClose(client.Entity.Trading.Counterparty.MyClient);
                PacketSender.SendTradeClose(client);
                client.Entity.Trading.Counterparty.Trading.Counterparty = null;
                client.Entity.Trading.Counterparty = null;
            }
        }

        private static void HandleTradeDecline(Client client, byte[] packet)
        {
            client.Entity.CancelTrade();
        }

        private static void HandleAddTilesets(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            var type = GameObjectType.Tileset;
            if (client.IsEditor)
            {
                bf.WriteBytes(packet);
                var count = bf.ReadInteger();
                for (int i = 0; i < count; i++)
                {
                    var value = bf.ReadString().Trim().ToLower();
                    if (type == GameObjectType.Tileset)
                    {
                        foreach (var tileset in TilesetBase.Lookup)
                            if (tileset.Value.Name.Trim().ToLower() == value) return;
                    }
                    var obj = LegacyDatabase.AddGameObject(type);
                    if (type == GameObjectType.Tileset)
                    {
                        ((TilesetBase)obj).Name = value;
                        LegacyDatabase.SaveGameObject(obj);
                    }
                    PacketSender.SendGameObjectToAll(obj, false, i != count - 1);
                }
                bf.Dispose();
            }
        }

        private static void HandleEnterMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            if (client.IsEditor)
            {
                var mapNum = bf.ReadInteger();
                client.EditorMap = mapNum;
            }

            bf.Dispose();
        }

        private static void HandleCloseBag(Client client, byte[] packet)
        {
            client.Entity.CloseBag();
        }

        private static void HandleStoreBagItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slot = bf.ReadInteger();
            var amount = bf.ReadInteger();
            client.Entity.StoreBagItem(slot, amount);
            bf.Dispose();
        }

        private static void HandleRetreiveBagItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var slot = bf.ReadInteger();
            var amount = bf.ReadInteger();
            client.Entity.RetreiveBagItem(slot, amount);
            bf.Dispose();
        }

        private static void HandleMoveBagItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var item1 = bf.ReadInteger();
            var item2 = bf.ReadInteger();
            client.Entity.SwapBagItems(item1, item2);
            bf.Dispose();
        }

        private static void HandleRequestFriends(Client client, byte[] packet)
        {
            PacketSender.SendFriends(client);
        }

        private static void HandleFriendRequest(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int target = bf.ReadInteger();
            //Check if valid target
            if (target < 0 || target > Globals.Entities.Count)
            {
                return;
            }
            if (Globals.Entities[target] != null && Globals.Entities[target].GetEntityType() == EntityTypes.Player &&
                target != client.Entity.MyIndex)
            {
                Player targetPlayer = ((Player) Globals.Entities[target]);
                if (!client.Entity.HasFriend(targetPlayer)) // Incase one user deleted friend then re-requested
                {
                    client.Entity.AddFriend(targetPlayer);
                    PacketSender.SendPlayerMsg(client, Strings.Friends.notification.ToString(targetPlayer.Name), CustomColors.Accepted);
                    PacketSender.SendFriends(client);
                }

                if (!targetPlayer.HasFriend(client.Entity)) // Incase one user deleted friend then re-requested
                {
                    targetPlayer.AddFriend(client.Entity);
                    PacketSender.SendPlayerMsg(targetPlayer.MyClient, Strings.Friends.accept.ToString(client.Entity.Name), CustomColors.Accepted);
                    PacketSender.SendFriends(targetPlayer.MyClient);
                }

                return;
            }
            bf.Dispose();
        }

        private static void HandleFriendRequestDecline(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int target = bf.ReadInteger();
            if (client.Entity.FriendRequester != null && client.Entity.FriendRequester.MyIndex == target)
            {
                if (client.Entity.FriendRequester.IsValidPlayer)
                {
                    if (client.Entity.FriendRequests.ContainsKey(client.Entity.FriendRequester))
                    {
                        client.Entity.FriendRequests[client.Entity.FriendRequester] =
                            Globals.System.GetTimeMs() + Player.REQUEST_DECLINE_TIMEOUT;
                    }
                    else
                    {
                        client.Entity.FriendRequests.Add(client.Entity.FriendRequester,
                            Globals.System.GetTimeMs() + Player.REQUEST_DECLINE_TIMEOUT);
                    }
                }
                client.Entity.FriendRequester = null;
            }
        }

        private static void HandleAddFriend(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            string name = bf.ReadString();

            //Don't add yourself!
            if (name.ToLower() == client.Entity.Name.ToLower())
            {
                return;
            }

            var character = LegacyDatabase.GetCharacter(name);
            if (character != null)
            {
                if (!client.Entity.HasFriend(character))
                {
                    //Add the friend
                    foreach (var c in Globals.Clients) //Check the player is online
                    {
                        if (c != null && c.Entity != null)
                        {
                            if (name.ToLower() == c.Entity.Name.ToLower())
                            {
                                c.Entity.FriendRequest(client.Entity);
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, Strings.Player.offline, CustomColors.Error);
                }
                else
                {
                    PacketSender.SendPlayerMsg(client, Strings.Friends.alreadyfriends.ToString(name), CustomColors.Info);
                }
            }
            bf.Dispose();
        }

        private static void HandleRemoveFriend(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            string name = bf.ReadString();
            var charId = LegacyDatabase.GetCharacterId(name);

            if (charId != null)
            {
                var character = LegacyDatabase.GetCharacter((Guid)charId);
                if (character != null && client.Entity.HasFriend(character))
                {
                    LegacyDatabase.DeleteCharacterFriend(client.Entity, character);
                    PacketSender.SendPlayerMsg(client, Strings.Friends.remove, CustomColors.Declined);
                    PacketSender.SendFriends(client);
                }
            }
            bf.Dispose();
        }

        private static void HandlePlayGame(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var charId = bf.ReadGuid();
            var character = LegacyDatabase.GetUserCharacter(client.User, charId);
            if (character != null)
            {
                client.LoadCharacter(character);
                PacketSender.SendJoinGame(client);
            }
            bf.Dispose();
        }

        private static void HandleDeleteChar(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var charId = bf.ReadGuid();
            var character = LegacyDatabase.GetUserCharacter(client.User, charId);
            if (character != null)
            {
                LegacyDatabase.DeleteCharacter(charId);
            }
            PacketSender.SendLoginError(client, Strings.Account.deletechar, Strings.Account.deleted);
            PacketSender.SendPlayerCharacters(client);
            bf.Dispose();
        }

        private static void HandleCreateNewChar(Client client, byte[] packet)
        {
            if (client.Characters.Count < Options.MaxCharacters)
            {
                PacketSender.SendGameObjects(client, GameObjectType.Class);
                PacketSender.SendCreateCharacter(client);
            }
            else
            {
                PacketSender.SendLoginError(client, Strings.Account.maxchars);
            }
        }
    }
}