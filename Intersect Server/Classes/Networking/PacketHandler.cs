/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
using System;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps;
using Intersect_Library.GameObjects.Maps.MapList;
using Intersect_Server.Classes.Core;
using Intersect_Server.Classes.Entities;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Items;
using Intersect_Server.Classes.Maps;
using Intersect_Server.Classes.Spells;

using Attribute = Intersect_Library.GameObjects.Maps.Attribute;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Intersect_Server.Classes.Networking
{
    public class PacketHandler
    {
        public void HandlePacket(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);

            //Compressed?
            if (bf.ReadByte() == 1)
            {
                packet = bf.ReadBytes(bf.Length());
                var data = Compression.DecompressPacket(packet);
                bf = new ByteBuffer();
                bf.WriteBytes(data);
            }

            var packetHeader = (ClientPackets)bf.ReadLong();
            packet = bf.ReadBytes(bf.Length());
            bf.Dispose();
            switch (packetHeader)
            {
                case ClientPackets.Ping:
                    HandlePing(client);
                    break;
                case ClientPackets.Login:
                    HandleLogin(client, packet);
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
                case ClientPackets.CloseCraftingBench:
                    HandleCloseCraftingBench(client, packet);
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
                case ClientPackets.PartyKick:
                    HandlePartyKick(client, packet);
                    break;
                case ClientPackets.PartyLeave:
                    HandlePartyLeave(client,packet);
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
                case ClientPackets.RequestDecline:
                    HandleRequestDecline(client, packet);
                    break;
                case ClientPackets.AddTilesets:
                    HandleAddTilesets(client,packet);
                    break;
                default:
                    Globals.GeneralLogs.Add(@"Non implemented packet received: " + packetHeader);
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
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = client.EntityIndex;
            var username = bf.ReadString();
            var password = bf.ReadString();
            if (Database.AccountExists(username))
            {
                if (Database.CheckPassword(username, password))
                {
                    client.MyAccount = username;

                    //Check for ban
                    string isBanned = Database.CheckBan(username, client.GetIP());
                    if (isBanned == null)
                    {
                        lock (Globals.ClientLock)
                        {
                            var clients = Globals.Clients.ToArray();
                            foreach (var user in clients)
                            {
                                if (user.MyAccount.ToLower() == client.MyAccount.ToLower() && user != client &&
                                    user.IsEditor == false)
                                {
                                    user.Disconnect();
                                }
                            }
                        }

                        if (Database.LoadUser(client))
                        {
                            //Check for mute
                            string isMuted = Database.CheckMute(username, client.GetIP());
                            if (isMuted != null)
                            {
                                client.Muted = true;
                                client.MuteReason = isMuted;
                            }

                            Globals.Entities[index] = new Player(index, client);
                            client.Entity = (Player)Globals.Entities[index];
                            Globals.GeneralLogs.Add(client.MyAccount + " logged in.");
                            PacketSender.SendServerConfig(client);
                            if (Database.LoadCharacter(client))
                            {
                                PacketSender.SendJoinGame(client);
                            }
                            else
                            {
                                PacketSender.SendGameObjects(client, GameObject.Class);
                                PacketSender.SendCreateCharacter(client);
                            }
                        }
                        else
                        {
                            PacketSender.SendLoginError(client, "Failed to load account. Please try logging in again.");
                        }
                    }
                    else
                    {
                        PacketSender.SendLoginError(client, isBanned);
                    }
                }
                else
                {
                    PacketSender.SendLoginError(client, "Username or password incorrect.");
                }
            }
            else
            {
                PacketSender.SendLoginError(client, "Username or password incorrect.");
            }
            bf.Dispose();
        }

        private static void HandleNeedMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = (int)bf.ReadLong();
            var map = MapInstance.GetMap(mapNum);
            if (map != null)
            {
                PacketSender.SendMap(client, mapNum);
                map.SendMapEntitiesTo(client.Entity);
                if (!client.IsEditor && client != null && client.Entity != null && mapNum == client.Entity.CurrentMap)
                {
                    PacketSender.SendMapGrid(client, MapInstance.GetMap(mapNum).MapGrid);
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
            for (var n = 0; n < client.Entity.Status.Count; n++)
            {
                if (client.Entity.Status[n].Type == (int)StatusTypes.Stun ||
                    client.Entity.Status[n].Type == (int)StatusTypes.Snare)
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
            if (Globals.Entities[index].CanMove(dir) == -1 || Globals.Entities[index].CanMove(dir) == -4)
            {
                Globals.Entities[index].Move(dir, client, false);
                if (Globals.Entities[index].MoveTimer > Globals.System.GetTimeMs())
                {
                    Globals.Entities[index].MoveTimer = Globals.System.GetTimeMs() +
                                                        (long)(Globals.Entities[index].GetMovementTime() / 2f);
                }
            }
            else
            {
                PacketSender.SendEntityPositionTo(client, client.Entity);
                return;
            }
            if (map != client.Entity.CurrentMap || x != client.Entity.CurrentX || y != client.Entity.CurrentY)
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
            if (msg == "killme")
            {
                client.Entity.Die();
            }
            if (client.Muted == true) //Don't let the toungless toxic kids speak.
            {
                PacketSender.SendPlayerMsg(client, client.MuteReason);
                return;
            }

            if (client.Power == 2)
            {
                PacketSender.SendGlobalMsg(client.Entity.MyName + ": " + msg, Color.Red, client.Entity.MyName);
            }
            else if (client.Power == 1)
            {
                PacketSender.SendGlobalMsg(client.Entity.MyName + ": " + msg, new Color(0, 70, 255), client.Entity.MyName);
            }
            else
            {
                PacketSender.SendGlobalMsg(client.Entity.MyName + ": " + msg, client.Entity.MyName);
            }
            PacketSender.SendChatBubble(client.Entity.MyIndex, (int)EntityTypes.GlobalEntity, msg, client.Entity.CurrentMap);
            bf.Dispose();
        }

        private static void HandleEditorLogin(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var usr = bf.ReadString();
            var pass = bf.ReadString();
            if (Database.AccountExists(usr))
            {
                if (Database.CheckPassword(usr, pass))
                {
                    if (Database.CheckPower(usr) == 2)
                    {
                        client.IsEditor = true;
                        client.MyAccount = usr;
                        lock (Globals.ClientLock)
                        {
                            var clients = Globals.Clients.ToArray();
                            foreach (var user in clients)
                            {
                                if (user.MyAccount.ToLower() == client.MyAccount.ToLower() && user != client &&
                                    user.IsEditor)
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
                    else
                    {
                        PacketSender.SendLoginError(client, "Access denied! Invalid power level!");
                    }
                }
                else
                {
                    PacketSender.SendLoginError(client, "Username or password invalid.");
                }
            }
            else
            {
                PacketSender.SendLoginError(client, "Username or password invalid.");
            }
        }

        private static void HandleMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapNum = (int)bf.ReadLong();
            var mapLength = bf.ReadLong();
            var map = MapInstance.GetMap(mapNum);
            if (map != null)
            {
                MapInstance.GetMap(mapNum).Load(bf.ReadBytes((int)mapLength), MapInstance.GetMap(mapNum).Revision + 1);
                Database.SaveGameObject(MapInstance.GetMap(mapNum));
                MapInstance.GetMap(mapNum).InitAutotiles();
                foreach (var t in Globals.Clients)
                {
                    if (t == null) continue;
                    if (t.IsEditor)
                    {
                        PacketSender.SendMapList(t);
                    }
                }
                var players = new List<Player>();
                foreach (var surrMap in map.GetSurroundingMaps(true))
                {
                    players.AddRange(surrMap.GetPlayersOnMap().ToArray());
                }
                foreach (var player in players)
                {
                    player.Warp(player.CurrentMap, player.CurrentX, player.CurrentY);
                    PacketSender.SendMap(player.MyClient,(int)mapNum);
                }
                PacketSender.SendMap(client, (int)mapNum); //Sends map to everyone/everything in proximity
                bf.Dispose();
            }
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
                newMap = Database.AddGameObject(GameObject.Map).GetId();
                tmpMap = MapInstance.GetMap(newMap);
                Database.SaveGameObject(tmpMap);
                Database.GenerateMapGrids();
                tmpMap.UpdateSurroundingTiles();
                PacketSender.SendMap(client, newMap);
                PacketSender.SendMapGrid(client, tmpMap.MapGrid);
                //FolderDirectory parent = null;
                destType = -1;
                if (destType == -1)
                {
                    MapList.GetList().AddMap(newMap, MapBase.GetObjects());
                }
                Database.SaveMapFolders();
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
                        if (MapInstance.GetMap(MapInstance.GetMap(relativeMap).Up) == null)
                        {
                            newMap = Database.AddGameObject(GameObject.Map).GetId();
                            tmpMap = MapInstance.GetMap(newMap);
                            tmpMap.MapGrid = MapInstance.GetMap(relativeMap).MapGrid;
                            tmpMap.MapGridX = MapInstance.GetMap(relativeMap).MapGridX;
                            tmpMap.MapGridY = MapInstance.GetMap(relativeMap).MapGridY - 1;
                            MapInstance.GetMap(relativeMap).Up = newMap;

                        }
                        break;

                    case 1:
                        if (MapInstance.GetMap(MapInstance.GetMap(relativeMap).Down) == null)
                        {
                            newMap = Database.AddGameObject(GameObject.Map).GetId();
                            tmpMap = MapInstance.GetMap(newMap);
                            tmpMap.MapGrid = MapInstance.GetMap(relativeMap).MapGrid;
                            tmpMap.MapGridX = MapInstance.GetMap(relativeMap).MapGridX;
                            tmpMap.MapGridY = MapInstance.GetMap(relativeMap).MapGridY + 1;
                            MapInstance.GetMap(relativeMap).Down = newMap;
                        }
                        break;

                    case 2:
                        if (MapInstance.GetMap(MapInstance.GetMap(relativeMap).Left) == null)
                        {
                            newMap = Database.AddGameObject(GameObject.Map).GetId();
                            tmpMap = MapInstance.GetMap(newMap);
                            tmpMap.MapGrid = MapInstance.GetMap(relativeMap).MapGrid;
                            tmpMap.MapGridX = MapInstance.GetMap(relativeMap).MapGridX - 1;
                            tmpMap.MapGridY = MapInstance.GetMap(relativeMap).MapGridY;
                            MapInstance.GetMap(relativeMap).Left = newMap;
                        }
                        break;

                    case 3:
                        if (MapInstance.GetMap(MapInstance.GetMap(relativeMap).Right) == null)
                        {
                            newMap = Database.AddGameObject(GameObject.Map).GetId();
                            tmpMap = MapInstance.GetMap(newMap);
                            tmpMap.MapGrid = MapInstance.GetMap(relativeMap).MapGrid;
                            tmpMap.MapGridX = MapInstance.GetMap(relativeMap).MapGridX + 1;
                            tmpMap.MapGridY = MapInstance.GetMap(relativeMap).MapGridY;
                            MapInstance.GetMap(relativeMap).Right = newMap;
                        }
                        break;

                }

                if (newMap > -1)
                {
                    Database.SaveGameObject(MapInstance.GetMap(relativeMap));

                    if (tmpMap.MapGridX >= 0 && tmpMap.MapGridX < Database.MapGrids[tmpMap.MapGrid].Width)
                    {
                        if (tmpMap.MapGridY + 1 < Database.MapGrids[tmpMap.MapGrid].Height)
                        {
                            tmpMap.Down = Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX, tmpMap.MapGridY + 1];
                            if (tmpMap.Down > -1)
                            {
                                MapInstance.GetMap(tmpMap.Down).Up = newMap;
                                Database.SaveGameObject(MapInstance.GetMap(tmpMap.Down));
                            }
                        }
                        if (tmpMap.MapGridY - 1 >= 0)
                        {
                            tmpMap.Up = Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX, tmpMap.MapGridY - 1];
                            if (tmpMap.Up > -1)
                            {
                                MapInstance.GetMap(tmpMap.Up).Down = newMap;
                                Database.SaveGameObject(MapInstance.GetMap(tmpMap.Up));
                            }
                        }
                    }

                    if (tmpMap.MapGridY >= 0 && tmpMap.MapGridY < Database.MapGrids[tmpMap.MapGrid].Height)
                    {
                        if (tmpMap.MapGridX - 1 >= 0)
                        {
                            tmpMap.Left = Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX - 1, tmpMap.MapGridY];
                            if (tmpMap.Left > -1)
                            {
                                MapInstance.GetMap(tmpMap.Left).Right = newMap;
                                Database.SaveGameObject(MapInstance.GetMap(tmpMap.Left));
                            }
                        }

                        if (tmpMap.MapGridX + 1 < Database.MapGrids[tmpMap.MapGrid].Width)
                        {
                            tmpMap.Right =
                                Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX + 1, tmpMap.MapGridY];
                            if (tmpMap.Right > -1)
                            {
                                MapInstance.GetMap(tmpMap.Right).Left = newMap;
                                Database.SaveGameObject(MapInstance.GetMap(tmpMap.Right));
                            }
                        }
                    }

                    Database.SaveGameObject(tmpMap);
                    Database.GenerateMapGrids();
                    MapInstance.GetMap(newMap).UpdateSurroundingTiles();
                    PacketSender.SendMap(client, newMap);
                    PacketSender.SendMapGrid(client, MapInstance.GetMap(newMap).MapGrid);
                    PacketSender.SendEnterMap(client, newMap);
                    var folderDir = MapList.GetList().FindMapParent(relativeMap, null);
                    if (folderDir != null)
                    {
                        folderDir.Children.AddMap(newMap, MapBase.GetObjects());
                    }
                    else
                    {
                        MapList.GetList().AddMap(newMap, MapBase.GetObjects());
                    }
                    Database.SaveMapFolders();
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
            for (var n = 0; n < client.Entity.Status.Count; n++)
            {
                if (client.Entity.Status[n].Type == (int)StatusTypes.Stun)
                {
                    PacketSender.SendPlayerMsg(client, "You are stunned and can't block");
                    bf.Dispose();
                    return;
                }
            }

            client.Entity.TryBlock(bf.ReadInteger());

            bf.Dispose();
        }

        private static void HandleTryAttack(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            long target = bf.ReadLong();

            //check if player is blinded or stunned
            for (var n = 0; n < client.Entity.Status.Count; n++)
            {
                if (client.Entity.Status[n].Type == (int)StatusTypes.Stun)
                {
                    PacketSender.SendPlayerMsg(client, "You are stunned and can't attack");
                    bf.Dispose();
                    return;
                }
                if (client.Entity.Status[n].Type == (int)StatusTypes.Blind)
                {
                    PacketSender.SendActionMsg(client.EntityIndex, "MISS!", new Color(255, 255, 255, 255));
                    bf.Dispose();
                    return;
                }
            }

            //Fire projectile instead if weapon has it
            if (Options.WeaponIndex > -1)
            {
                if (client.Entity.Equipment[Options.WeaponIndex] >= 0 &&
                    ItemBase.GetItem(client.Entity.Inventory[client.Entity.Equipment[Options.WeaponIndex]].ItemNum) !=
                    null)
                {
                    var projectileBase =
                        ProjectileBase.GetProjectile(
                            ItemBase.GetItem(
                                client.Entity.Inventory[client.Entity.Equipment[Options.WeaponIndex]].ItemNum)
                                .Projectile);
                    if (projectileBase != null)
                    {
                        MapInstance.GetMap(client.Entity.CurrentMap)
                            .SpawnMapProjectile(client.Entity, projectileBase, client.Entity.CurrentMap,
                                client.Entity.CurrentX, client.Entity.CurrentY, client.Entity.CurrentZ,
                                client.Entity.Dir);
                        bf.Dispose();
                        return;
                    }
                }
            }

            //Attack normally.
            if (target > -1 && target < Globals.Entities.Count && Globals.Entities[(int)target] != null)
                client.Entity.TryAttack(Globals.Entities[(int)target]);
            bf.Dispose();
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
            PacketSender.SendPlayerMsg(client, "Welcome to the Intersect game server.");
            PacketSender.SendGlobalMsg(Globals.Entities[index].MyName + " has joined the Intersect engine");
            if (client.Power == 1)
            {
                PacketSender.SendPlayerMsg(client,
                    "You are a moderator! Press Insert at any time to access the administration menu or F2 for debug information.",
                    Color.OrangeRed);
            }
            else if (client.Power == 2)
            {
                PacketSender.SendPlayerMsg(client,
                    "You are an administrator! Press Insert at any time to access the administration menu or F2 for debug information.",
                    Color.OrangeRed);
            }
            Globals.Entities[index].Warp(Globals.Entities[index].CurrentMap, Globals.Entities[index].CurrentX,
                Globals.Entities[index].CurrentY, Globals.Entities[index].Dir);
            PacketSender.SendEntityDataTo(client, client.Entity);

            //Search for login activated events and run them
            foreach (var evt in EventBase.GetObjects())
            {
                ((Player)client.Entity).StartCommonEvent(evt.Value, (int)EventPage.CommonEventTriggers.JoinGame);
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
            var index = client.EntityIndex;
            if (Database.AccountExists(username))
            {
                PacketSender.SendLoginError(client, "Account already exists!");
            }
            else
            {
                if (Database.EmailInUse(email))
                {
                    PacketSender.SendLoginError(client, "An account with this email address already exists.");
                }
                else
                {
                    Database.CreateAccount(client, username, password, email);
                    Globals.GeneralLogs.Add(Globals.Entities[index].MyName + " logged in.");
                    PacketSender.SendServerConfig(client);
                    PacketSender.SendGameObjects(client, GameObject.Class);
                    PacketSender.SendCreateCharacter(client);
                }
            }
            bf.Dispose();
        }

        private static void HandleCreateCharacter(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var Name = bf.ReadString();
            var Class = bf.ReadInteger();
            var Sprite = bf.ReadInteger();
            var index = client.EntityIndex;
            var classBase = ClassBase.GetClass(Class);
            if (classBase == null || classBase.Locked == 1)
            {
                PacketSender.SendLoginError(client, "Invalid class selected. Try again.");
                return;
            }
            if (Database.CharacterNameInUse(Name))
            {
                PacketSender.SendLoginError(client, "An account with this character name exists. Please choose another.");
            }
            else
            {
                var player = (Player)Globals.Entities[index];
                client.Entity = player;
                player.MyName = Name;
                player.Class = Class;
                if (classBase.Sprites.Count > 0)
                {
                    player.MySprite = classBase.Sprites[Sprite].Sprite;
                    player.Face = classBase.Sprites[Sprite].Face;
                    player.Gender = classBase.Sprites[Sprite].Gender;
                }
                PacketSender.SendJoinGame(client);
                player.WarpToSpawn(true);
                player.Vital[(int)Vitals.Health] = classBase.BaseVital[(int)Vitals.Health];
                player.Vital[(int)Vitals.Mana] = classBase.BaseVital[(int)Vitals.Mana];
                player.MaxVital[(int)Vitals.Health] = classBase.BaseVital[(int)Vitals.Health];
                player.MaxVital[(int)Vitals.Mana] = classBase.BaseVital[(int)Vitals.Mana];

                for (int i = 0; i < (int)Stats.StatCount; i++)
                {
                    player.Stat[i].Stat = classBase.BaseStat[i];
                }
                player.StatPoints = classBase.BasePoints;

                for (int i = 0; i < classBase.Spells.Count; i++)
                {
                    if (classBase.Spells[i].Level <= 1)
                    {
                        SpellInstance TempSpell = new SpellInstance();
                        TempSpell.SpellNum = classBase.Spells[i].SpellNum;
                        player.TryTeachSpell(TempSpell, false);
                    }
                }

                for (int i = 0; i < Options.MaxNpcDrops; i++)
                {
                    ItemInstance TempItem = new ItemInstance(classBase.Items[i].ItemNum, classBase.Items[i].Amount);
                    player.TryGiveItem(TempItem, false);
                }
                Task.Run(() => Database.SaveCharacter(client.Entity, true));
                Globals.GeneralLogs.Add(client.MyAccount + " has created a character.");
            }
            bf.Dispose();
        }

        private static void HandlePickupItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            if (index < MapInstance.GetMap(client.Entity.CurrentMap).MapItems.Count &&
                MapInstance.GetMap(client.Entity.CurrentMap).MapItems[index] != null)
            {
                if (MapInstance.GetMap(client.Entity.CurrentMap).MapItems[index].X == client.Entity.CurrentX &&
                    MapInstance.GetMap(client.Entity.CurrentMap).MapItems[index].Y == client.Entity.CurrentY)
                {
                    if (
                        client.Entity.TryGiveItem(
                            ((ItemInstance)MapInstance.GetMap(client.Entity.CurrentMap).MapItems[index])))
                    {
                        //Remove Item From Map
                        MapInstance.GetMap(client.Entity.CurrentMap).RemoveItem(index);
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
            client.Entity.UseSpell(slot, target);
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
                    MapList.GetList().HandleMove(bf.ReadInteger(), bf.ReadInteger(), bf.ReadInteger(), bf.ReadInteger());
                    break;
                case (int)MapListUpdates.AddFolder:
                    destType = bf.ReadInteger();
                    parent = null;
                    if (destType == -1)
                    {
                        MapList.GetList().AddFolder("New Folder");
                    }
                    else if (destType == 0)
                    {
                        parent = MapList.GetList().FindDir(bf.ReadInteger());
                        if (parent == null)
                        {
                            MapList.GetList().AddFolder("New Folder");
                        }
                        else
                        {
                            parent.Children.AddFolder("New Folder");
                        }
                    }
                    else if (destType == 1)
                    {
                        mapNum = bf.ReadInteger();
                        parent = MapList.GetList().FindMapParent(mapNum, null);
                        if (parent == null)
                        {
                            MapList.GetList().AddFolder("New Folder");
                        }
                        else
                        {
                            parent.Children.AddFolder("New Folder");
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
                        MapInstance.GetMap(mapNum).MyName = bf.ReadString();
                        Database.SaveGameObject(MapInstance.GetMap(mapNum));
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
                        if (MapInstance.ObjectCount() == 1)
                        {
                            PacketSender.SendAlert(client, "Last Map",
                                "Failed to delete map, you must have at least one map at all times!");
                            return;
                        }
                        mapNum = bf.ReadInteger();
                        Database.CheckAllMapConnections();
                        MapList.GetList().DeleteMap(mapNum);
                        Database.GenerateMapGrids();
                        Database.SaveGameObject(MapInstance.GetMap(mapNum));
                        Database.DeleteGameObject(MapInstance.GetMap(mapNum));
                        PacketSender.SendMapListToAll();
                    }
                    break;
            }
            PacketSender.SendMapListToAll();
            Database.SaveMapFolders();
            bf.Dispose();
        }

        private static void HandleOpenAdminWindow(Client client)
        {
            if (client.Power > 0)
            {
                PacketSender.SendMapList(client);
                PacketSender.SendOpenAdminWindow(client);
            }
        }

        private static void HandleAdminAction(Client client, byte[] packet)
        {
            if (client.Power == 0)
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
                    client.Entity.Warp(Convert.ToInt32(val1), client.Entity.CurrentX, client.Entity.CurrentY);
                    break;
                case (int)AdminActions.WarpMeTo:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.MyName.ToLower())
                            {
                                client.Entity.Warp(Globals.Clients[i].Entity.CurrentMap,
                                    Globals.Clients[i].Entity.CurrentX, Globals.Clients[i].Entity.CurrentY);
                                PacketSender.SendPlayerMsg(client, "Warped to " + val1);
                                PacketSender.SendPlayerMsg(Globals.Clients[i], client.Entity.MyName + " warped to you");
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, val1 + " is not online.");
                    break;
                case (int)AdminActions.WarpToMe:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.MyName.ToLower())
                            {
                                Globals.Clients[i].Entity.Warp(client.Entity.CurrentMap, client.Entity.CurrentX,
                                    client.Entity.CurrentY);
                                PacketSender.SendPlayerMsg(client, val1 + " has been warped to you", client.Entity.MyName);
                                PacketSender.SendPlayerMsg(Globals.Clients[i],
                                    "You have been warped to " + client.Entity.MyName, client.Entity.MyName);
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, val1 + " is not online.");
                    break;
                case (int)AdminActions.WarpToLoc:
                    if (client.Power > 0)
                    {
                        client.Entity.Warp(Convert.ToInt32(val1), Convert.ToInt32(val2), Convert.ToInt32(val3));
                    }
                    break;
                case (int)AdminActions.Kick:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.MyName.ToLower())
                            {
                                PacketSender.SendGlobalMsg(Globals.Clients[i].Entity.MyName + " has been kicked by " +
                                                           client.Entity.MyName + "!");
                                Globals.Clients[i].Disconnect(); //Kick em'
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, val1 + " is not online.");
                    break;
                case (int)AdminActions.Kill:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.MyName.ToLower())
                            {
                                Globals.Clients[i].Entity.Die(); //Kill em'
                                PacketSender.SendGlobalMsg(Globals.Clients[i].Entity.MyName + " has been killed by " +
                                                           client.Entity.MyName + "!");
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, val1 + " is not online.");
                    break;
                case (int)AdminActions.SetSprite:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.MyName.ToLower())
                            {
                                Globals.Clients[i].Entity.MySprite = val2;
                                PacketSender.SendEntityDataToProximity(Globals.Clients[i].Entity);
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, val1 + " is not online.");
                    break;
                case (int)AdminActions.SetFace:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.MyName.ToLower())
                            {
                                Globals.Clients[i].Entity.Face = val2;
                                PacketSender.SendEntityDataToProximity(Globals.Clients[i].Entity);
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, val1 + " is not online.");
                    break;
                case (int)AdminActions.SetAccess:
                    int p;
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.MyName.ToLower())
                            {
                                if (val1.ToLower() != client.Entity.MyName.ToLower()) //Can't increase your own power!
                                {
                                    if (client.Power == 2)
                                    {
                                        if (val2 == "Admin")
                                        { p = 2; }
                                        else if (val2 == "Moderator")
                                        { p = 1; }
                                        else { p = 0; }

                                        Globals.Clients[i].Power = p;
                                        if (Globals.Clients[i].Power == 2)
                                        {
                                            PacketSender.SendGlobalMsg(val1 +
                                                                       " has been given administrative powers!");
                                        }
                                        else if (Globals.Clients[i].Power == 1)
                                        {
                                            PacketSender.SendGlobalMsg(val1 +
                                                                       " has been given moderation powers!");
                                        }
                                        else
                                        {
                                            PacketSender.SendGlobalMsg(val1 +
                                                                       " has had their power revoked!");
                                        }
                                        return;
                                    }
                                    else
                                    {
                                        PacketSender.SendPlayerMsg(client, "Only admins can set power.");
                                        return;
                                    }
                                }
                                else
                                {
                                    PacketSender.SendPlayerMsg(client, "You cannot alter your own power!");
                                    return;
                                }
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, val1 + " is not online.");
                    break;
                case (int)AdminActions.UnMute:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.MyName.ToLower())
                            {
                                Database.DeleteMute(Globals.Clients[i].MyAccount);
                                Globals.Clients[i].Muted = false;
                                Globals.Clients[i].MuteReason = "";
                                PacketSender.SendGlobalMsg(Globals.Clients[i].Entity.MyName + " has been unmuted!");
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, val1 + " is not online.");
                    break;
                case (int)AdminActions.UnBan:
                    if (Database.AccountExists(val1))
                    {
                        Database.DeleteBan(val1);
                        PacketSender.SendPlayerMsg(client, "Account: " + val1 + " has been unbanned.");
                    }
                    else
                    {
                        PacketSender.SendPlayerMsg(client, val1 + " is not online.");
                    }
                    break;
                case (int)AdminActions.Mute:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.MyName.ToLower())
                            {
                                if (Convert.ToBoolean(val4) == true)
                                {
                                    Database.AddMute(Globals.Clients[i], Convert.ToInt32(val2), val3,
                                        client.Entity.MyName, Globals.Clients[i].GetIP());
                                }
                                else
                                {
                                    Database.AddMute(Globals.Clients[i], Convert.ToInt32(val2), val3,
                                        client.Entity.MyName, "");
                                }
                                Globals.Clients[i].Muted = true;
                                Globals.Clients[i].MuteReason = Database.CheckMute(Globals.Clients[i].MyAccount,
                                    Globals.Clients[i].GetIP());
                                PacketSender.SendGlobalMsg(Globals.Clients[i].Entity.MyName + " has been muted!");
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, val1 + " is not online.");
                    break;
                case (int)AdminActions.Ban:
                    for (int i = 0; i < Globals.Clients.Count; i++)
                    {
                        if (Globals.Clients[i] != null && Globals.Clients[i].Entity != null)
                        {
                            if (val1.ToLower() == Globals.Clients[i].Entity.MyName.ToLower())
                            {
                                if (Convert.ToBoolean(val4) == true)
                                {
                                    Database.AddBan(Globals.Clients[i], Convert.ToInt32(val2), val3,
                                        client.Entity.MyName, Globals.Clients[i].GetIP());
                                }
                                else
                                {
                                    Database.AddBan(Globals.Clients[i], Convert.ToInt32(val2), val3,
                                        client.Entity.MyName, "");
                                }

                                PacketSender.SendGlobalMsg(Globals.Clients[i].Entity.MyName + " has been banned!");
                                Globals.Clients[i].Disconnect(); //Kick em'
                                return;
                            }
                        }
                    }
                    PacketSender.SendPlayerMsg(client, val1 + " is not online.");
                    break;
            }
        }

        private static void HandleNeedGrid(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = (int)bf.ReadLong();
            if (MapInstance.GetObjects().ContainsKey(mapNum))
            {
                if (client.IsEditor)
                {
                    PacketSender.SendMapGrid(client, MapInstance.GetMap(mapNum).MapGrid);
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
            if (MapInstance.GetObjects().ContainsKey(mapNum))
            {
                if (client.IsEditor)
                {
                    if (MapInstance.GetMap(mapNum) != null)
                    {
                        MapInstance.GetMap(mapNum).ClearConnections();

                        int gridX = MapInstance.GetMap(mapNum).MapGridX;
                        int gridY = MapInstance.GetMap(mapNum).MapGridY;

                        //Up
                        if (gridY - 1 >= 0 &&
                            Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX, gridY - 1] > -1)
                        {
                            if (
                                MapInstance.GetMap(
                                    Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX, gridY - 1]) !=
                                null)
                                MapInstance.GetMap(
                                    Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX, gridY - 1])
                                    .ClearConnections((int)Directions.Down);
                        }

                        //Down
                        if (gridY + 1 < Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].Height &&
                            Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX, gridY + 1] > -1)
                        {
                            if (
                                MapInstance.GetMap(
                                    Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX, gridY + 1]) !=
                                null)
                                MapInstance.GetMap(
                                    Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX, gridY + 1])
                                    .ClearConnections((int)Directions.Up);
                        }

                        //Left
                        if (gridX - 1 >= 0 &&
                            Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX - 1, gridY] > -1)
                        {
                            if (
                                MapInstance.GetMap(
                                    Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX - 1, gridY]) !=
                                null)
                                MapInstance.GetMap(
                                    Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX - 1, gridY])
                                    .ClearConnections((int)Directions.Right);
                        }

                        //Right
                        if (gridX + 1 < Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].Width &&
                            Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX + 1, gridY] > -1)
                        {
                            if (
                                MapInstance.GetMap(
                                    Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX + 1, gridY]) !=
                                null)
                                MapInstance.GetMap(
                                    Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX + 1, gridY])
                                    .ClearConnections((int)Directions.Left);
                        }

                        Database.GenerateMapGrids();
                        if (MapInstance.GetObjects().ContainsKey(curMap))
                        {
                            mapGrid = MapInstance.GetMap(curMap).MapGrid;
                            MapInstance.GetMap(curMap).UpdateSurroundingTiles();
                        }
                    }
                    PacketSender.SendMapGrid(client, mapGrid);
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
            if (MapInstance.GetObjects().ContainsKey((int)linkMap) &&
                MapInstance.GetObjects().ContainsKey((int)adjacentMap))
            {
                //Clear to test if we can link.
                int linkGrid = MapInstance.GetMap(linkMap).MapGrid;
                int adjacentGrid = MapInstance.GetMap(adjacentMap).MapGrid;
                if (linkGrid != adjacentGrid)
                {
                    long xOffset = MapInstance.GetMap(linkMap).MapGridX - gridX;
                    long yOffset = MapInstance.GetMap(linkMap).MapGridY - gridY;
                    for (int x = 0; x < Database.MapGrids[adjacentGrid].Width; x++)
                    {
                        for (int y = 0; y < Database.MapGrids[adjacentGrid].Height; y++)
                        {
                            if (x + xOffset >= 0 && x + xOffset < Database.MapGrids[linkGrid].Width && y + yOffset >= 0 &&
                                y + yOffset < Database.MapGrids[linkGrid].Height)
                            {
                                if (Database.MapGrids[adjacentGrid].MyGrid[x, y] != -1 &&
                                    Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset] != -1)
                                {
                                    //Incompatible Link!
                                    PacketSender.SendAlert(client, "Map Link Failure",
                                        "Failed to link map " + linkMap + " to map " + adjacentMap +
                                        ". If this merge were to happen, maps " +
                                        Database.MapGrids[adjacentGrid].MyGrid[x, y] + " and " +
                                        Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset] +
                                        " would occupy the same space in the world.");
                                    return;
                                }
                            }
                        }
                    }
                    if (canLink)
                    {
                        for (int x = -1; x < Database.MapGrids[adjacentGrid].Width + 1; x++)
                        {
                            for (int y = -1; y < Database.MapGrids[adjacentGrid].Height + 1; y++)
                            {
                                if (x + xOffset >= 0 && x + xOffset < Database.MapGrids[linkGrid].Width &&
                                    y + yOffset >= 0 && y + yOffset < Database.MapGrids[linkGrid].Height)
                                {
                                    if (Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset] != -1)
                                    {
                                        bool inXBounds = x > -1 &&
                                                         x < Database.MapGrids[adjacentGrid].Width;
                                        bool inYBounds = y > -1 &&
                                                         y < Database.MapGrids[adjacentGrid].Height;
                                        if (inXBounds && inYBounds)
                                            Database.MapGrids[adjacentGrid].MyGrid[x, y] =
                                                Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];

                                        if (inXBounds && y - 1 >= 0 &&
                                            Database.MapGrids[adjacentGrid].MyGrid[x, y - 1] > -1)
                                        {
                                            MapInstance.GetMap(
                                                Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Up =
                                                Database.MapGrids[adjacentGrid].MyGrid[x, y - 1];
                                            MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x, y - 1]).Down =
                                                Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            Database.SaveGameObject(
                                                MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x, y - 1]));
                                        }

                                        if (inXBounds && y + 1 < Database.MapGrids[adjacentGrid].Height &&
                                            Database.MapGrids[adjacentGrid].MyGrid[x, y + 1] > -1)
                                        {
                                            MapInstance.GetMap(
                                                Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Down =
                                                Database.MapGrids[adjacentGrid].MyGrid[x, y + 1];
                                            MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x, y + 1]).Up =
                                                Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            Database.SaveGameObject(
                                                MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x, y + 1]));
                                        }

                                        if (inYBounds && x - 1 >= 0 &&
                                            Database.MapGrids[adjacentGrid].MyGrid[x - 1, y] > -1)
                                        {
                                            MapInstance.GetMap(
                                                Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Left =
                                                Database.MapGrids[adjacentGrid].MyGrid[x - 1, y];
                                            MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x - 1, y]).Right =
                                                Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            Database.SaveGameObject(
                                                MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x - 1, y]));
                                        }

                                        if (inYBounds && x + 1 < Database.MapGrids[adjacentGrid].Width &&
                                            Database.MapGrids[adjacentGrid].MyGrid[x + 1, y] > -1)
                                        {
                                            MapInstance.GetMap(
                                                Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Right =
                                                Database.MapGrids[adjacentGrid].MyGrid[x + 1, y];
                                            MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x + 1, y]).Left =
                                                Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            Database.SaveGameObject(
                                                MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x + 1, y]));
                                        }

                                        Database.SaveGameObject(
                                            MapInstance.GetMap(
                                                Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]));
                                    }
                                }
                            }
                        }
                        Database.SaveGameObject(MapInstance.GetMap(linkMap));
                        Database.GenerateMapGrids();
                        MapInstance.GetMap(linkMap).UpdateSurroundingTiles();
                        PacketSender.SendMapGrid(client, MapInstance.GetMap(adjacentMap).MapGrid);
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

        private static void HandleCloseCraftingBench(Client client, byte[] packet)
        {
            client.Entity.CloseCraftingBench();
        }

        private static void HandleCraftItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            client.Entity.CraftIndex = bf.ReadInteger();
            client.Entity.CraftTimer = Environment.TickCount;
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
            var type = (GameObject)bf.ReadInteger();
            var obj = Database.AddGameObject(type);
            PacketSender.SendGameObjectToAll(obj);
            bf.Dispose();
        }

        private static void HandleRequestOpenEditor(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = (GameObject)bf.ReadInteger();
            PacketSender.SendGameObjects(client, type);
            PacketSender.SendOpenEditor(client, type);
            bf.Dispose();
        }

        private void HandleDeleteGameObject(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = (GameObject)bf.ReadInteger();
            var id = bf.ReadInteger();
            DatabaseObject obj = null;
            switch (type)
            {
                case GameObject.Animation:
                    obj = AnimationBase.Get(id);
                    break;
                case GameObject.Class:
                    if (ClassBase.ObjectCount() == 1)
                    {
                        PacketSender.SendAlert(client, "Last Class",
                            "Failed to delete class, you must have at least one class at all times!");
                        return;
                    }
                    obj = ClassBase.Get(id);
                    break;
                case GameObject.Item:
                    obj = ItemBase.Get(id);
                    break;
                case GameObject.Npc:
                    obj = NpcBase.Get(id);
                    break;
                case GameObject.Projectile:
                    obj = ProjectileBase.Get(id);
                    break;
                case GameObject.Quest:
                    obj = QuestBase.Get(id);
                    break;
                case GameObject.Resource:
                    obj = ResourceBase.Get(id);
                    break;
                case GameObject.Shop:
                    obj = ShopBase.Get(id);
                    break;
                case GameObject.Spell:
                    obj = SpellBase.Get(id);
                    break;
                case GameObject.Bench:
                    obj = BenchBase.Get(id);
                    break;
                case GameObject.Map:
                    break;
                case GameObject.CommonEvent:
                    obj = EventBase.Get(id);
                    break;
                case GameObject.PlayerSwitch:
                    obj = PlayerSwitchBase.Get(id);
                    break;
                case GameObject.PlayerVariable:
                    obj = PlayerVariableBase.Get(id);
                    break;
                case GameObject.ServerSwitch:
                    obj = ServerSwitchBase.Get(id);
                    break;
                case GameObject.ServerVariable:
                    obj = ServerVariableBase.Get(id);
                    break;
                case GameObject.Tileset:
                    break;
                case GameObject.Time:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (obj != null)
            {
                //if Item or Resource, kill all global entities of that kind
                if (type == GameObject.Item)
                {
                    Globals.KillItemsOf((ItemBase)obj);
                }
                else if (type == GameObject.Resource)
                {
                    Globals.KillResourcesOf((ResourceBase)obj);
                }
                else if (type == GameObject.Npc)
                {
                    Globals.KillNpcsOf((NpcBase)obj);
                }
                Database.DeleteGameObject(obj);
                PacketSender.SendGameObjectToAll(obj, true);
            }
            bf.Dispose();
        }

        private void HandleSaveGameObject(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = (GameObject)bf.ReadInteger();
            var id = bf.ReadInteger();
            DatabaseObject obj = null;
            switch (type)
            {
                case GameObject.Animation:
                    obj = AnimationBase.Get(id);
                    break;
                case GameObject.Class:
                    obj = ClassBase.Get(id);
                    break;
                case GameObject.Item:
                    obj = ItemBase.Get(id);
                    break;
                case GameObject.Npc:
                    obj = NpcBase.Get(id);
                    break;
                case GameObject.Projectile:
                    obj = ProjectileBase.Get(id);
                    break;
                case GameObject.Quest:
                    obj = QuestBase.Get(id);
                    break;
                case GameObject.Resource:
                    obj = ResourceBase.Get(id);
                    break;
                case GameObject.Shop:
                    obj = ShopBase.Get(id);
                    break;
                case GameObject.Spell:
                    obj = SpellBase.Get(id);
                    break;
                case GameObject.Bench:
                    obj = BenchBase.Get(id);
                    break;
                case GameObject.Map:
                    break;
                case GameObject.CommonEvent:
                    obj = EventBase.Get(id);
                    break;
                case GameObject.PlayerSwitch:
                    obj = PlayerSwitchBase.Get(id);
                    break;
                case GameObject.PlayerVariable:
                    obj = PlayerVariableBase.Get(id);
                    break;
                case GameObject.ServerSwitch:
                    obj = ServerSwitchBase.Get(id);
                    break;
                case GameObject.ServerVariable:
                    obj = ServerVariableBase.Get(id);
                    break;
                case GameObject.Tileset:
                    break;
                case GameObject.Time:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (obj != null)
            {
                //if Item or Resource, kill all global entities of that kind
                if (type == GameObject.Item)
                {
                    Globals.KillItemsOf((ItemBase)obj);
                }
                else if (type == GameObject.Resource)
                {
                    Globals.KillResourcesOf((ResourceBase)obj);
                }
                else if (type == GameObject.Npc)
                {
                    Globals.KillNpcsOf((NpcBase)obj);
                }
                obj.Load(bf.ReadBytes(bf.Length()));
                PacketSender.SendGameObjectToAll(obj, false);
                Database.SaveGameObject(obj);
            }
            bf.Dispose();
        }

        private void HandleSaveTime(Client client, byte[] packet)
        {
            if (client.IsEditor)
            {
                TimeBase.GetTimeBase().LoadTimeBase(packet);
                Database.SaveTime();
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
            if (target < 0) { return; }
            if (Globals.Entities[target].GetEntityType() == EntityTypes.Player && target != client.Entity.MyIndex)
            {
                if (((Player)Globals.Entities[target]).PendingRequest == false)
                {
                    ((Player)Globals.Entities[target]).PendingRequest = true;
                    PacketSender.SendPartyInvite(((Player)Globals.Entities[target]).MyClient, client.EntityIndex);
                }
            }
            else
            {
                PacketSender.SendPlayerMsg(client, "You need to select a valid target.", Color.Red);
            }
            bf.Dispose();
        }

        private static void HandleAcceptPartyInvite(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int leader = bf.ReadInteger();
            //Check if valid target
            if (leader < 0) { return; }
            if (Globals.Entities[leader].GetEntityType() == EntityTypes.Player && leader != client.Entity.MyIndex)
            {
                client.Entity.PendingRequest = false;
                ((Player)Globals.Entities[leader]).AddParty((Player)client.Entity);
            }
            bf.Dispose();
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
            if (target < 0) { return; }
            if (Globals.Entities[target].GetEntityType() == EntityTypes.Player && target != client.Entity.MyIndex)
            {
                if (((Player)Globals.Entities[target]).PendingRequest == false)
                {
                    ((Player)Globals.Entities[target]).PendingRequest = true;
                    PacketSender.SendTradeRequest(((Player)Globals.Entities[target]).MyClient, client.Entity.MyIndex);
                }
            }
            bf.Dispose();
        }

        private static void HandleTradeRequestAccept(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int target = bf.ReadInteger();
            //Check if valid target
            if (target < 0) { return; }
            if (Globals.Entities[target].GetEntityType() == EntityTypes.Player && target != client.Entity.MyIndex)
            {
                client.Entity.PendingRequest = false;
                ((Player)Globals.Entities[target]).StartTrade((Player)client.Entity);
            }
            bf.Dispose();
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
            client.Entity.TradeAccepted = true;
            if (((Player)Globals.Entities[client.Entity.Trading]).TradeAccepted == true)
            {
                ItemInstance[] t = new ItemInstance[Options.MaxInvItems];

                //Swap the trade boxes over, then return the trade boxes to their new owners!
                t = client.Entity.Trade;
                client.Entity.Trade = ((Player)Globals.Entities[client.Entity.Trading]).Trade;
                ((Player)Globals.Entities[client.Entity.Trading]).Trade = t;
                client.Entity.ReturnTradeItems();
                ((Player)Globals.Entities[client.Entity.Trading]).ReturnTradeItems();

                PacketSender.SendPlayerMsg(client, "The trade was successful!", Color.Green);
                PacketSender.SendPlayerMsg(((Player)Globals.Entities[client.Entity.Trading]).MyClient, "The trade was successful!", Color.Green);
                PacketSender.SendTradeClose(((Player)Globals.Entities[client.Entity.Trading]).MyClient);
                PacketSender.SendTradeClose(client);
                ((Player)Globals.Entities[client.Entity.Trading]).Trading = -1;
                client.Entity.Trading = -1;
            }
        }

        private static void HandleTradeDecline(Client client, byte[] packet)
        {
            client.Entity.CancelTrade();
        }

        private static void HandleRequestDecline(Client client, byte[] packet)
        {
            client.Entity.PendingRequest = false;
        }

        private static void HandleAddTilesets(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            var type = GameObject.Tileset;
            if (client.IsEditor)
            {
                bf.WriteBytes(packet);
                var count = bf.ReadInteger();
                for (int i = 0; i < count; i++)
                {
                    var value = bf.ReadString();
                    if (type == GameObject.Tileset)
                    {
                        foreach (var tileset in TilesetBase.GetObjects())
                            if (tileset.Value.GetValue() == value) return;
                    }
                    var obj = Database.AddGameObject(type);
                    if (type == GameObject.Tileset)
                    {
                        ((TilesetBase)obj).SetValue(value);
                        Database.SaveGameObject(obj);
                    }
                    PacketSender.SendGameObjectToAll(obj,false, i!=count-1);
                }
                bf.Dispose();
            }
        }
    }
}

