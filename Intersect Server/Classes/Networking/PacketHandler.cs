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

namespace Intersect_Server.Classes.Networking
{
    public class PacketHandler
    {
        public void HandlePacket(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
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
                case ClientPackets.EnterMap:
                    HandleEnterMap(client, packet);
                    break;
                case ClientPackets.TryAttack:
                    HandleTryAttack(client, packet);
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
                default:
                    Globals.GeneralLogs.Add(@"Non implemented packet received: " + packetHeader);
                    break;
            }

        }

        private static void HandlePing(Client client)
        {
            client.ConnectionTimeout = -1;
            client.PingTime = Environment.TickCount + 10000;
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

                    if (Database.LoadUser(client))
                    {
                        Globals.Entities[index] = new Player(index, client);
                        client.Entity = (Player) Globals.Entities[index];
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
            if (MapInstance.GetObjects().ContainsKey(mapNum))
            {
                PacketSender.SendMap(client, mapNum);
                if (!client.IsEditor && mapNum == client.Entity.CurrentMap)
                {
                    PacketSender.SendEnterMap(client, mapNum);
                }
            }
        }

        private static void HandlePlayerMove(Client client, byte[] packet)
        {
            var index = client.EntityIndex;
            var oldMap = Globals.Entities[index].CurrentMap;
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int map = bf.ReadInteger();
            int x = bf.ReadInteger();
            int y = bf.ReadInteger();
            int dir = bf.ReadInteger();

            //check if player is stunned or snared, if so don't let them move.
            for (var n = 0; n < client.Entity.Status.Count; n++)
            {
                if (client.Entity.Status[n].Type == (int)StatusTypes.Stun || client.Entity.Status[n].Type == (int)StatusTypes.Snare)
                {
                    bf.Dispose();
                    return;
                }
            }

            if (TileHelper.IsTileValid(map, x, y))
            {
                Globals.Entities[index].CurrentMap = map;
                Globals.Entities[index].CurrentX = x;
                Globals.Entities[index].CurrentY = y;
                Globals.Entities[index].TryToChangeDimension();
                Globals.Entities[index].Dir = dir;
            }
            else
            {
                //POSSIBLE HACKING ATTEMPT!
                return;
            }



            bf.Dispose();


            //TODO: Add Check if valid before sending the move to everyone.
            PacketSender.SendEntityMove(index, (int)EntityTypes.Player, Globals.Entities[index]);

            // Check for a warp, if so warp the player.
            Attribute attribute =
                MapInstance.GetMap(Globals.Entities[index].CurrentMap).Attributes[
                    Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY];
            if (attribute != null && attribute.value == (int)MapAttributes.Warp)
            {
                Globals.Entities[index].Warp(attribute.data1, attribute.data2, attribute.data3, Globals.Entities[index].Dir);
            }

            if (oldMap != Globals.Entities[index].CurrentMap)
            {
                MapInstance.GetMap(Globals.Entities[index].CurrentMap).PlayerEnteredMap(client);
            }

            for (int i = 0; i < client.Entity.MyEvents.Count; i++)
            {
                if (client.Entity.MyEvents[i] != null)
                {
                    if (client.Entity.MyEvents[i].MapNum == client.Entity.CurrentMap)
                    {
                        if (client.Entity.MyEvents[i].PageInstance != null)
                        {
                            if (client.Entity.MyEvents[i].PageInstance.CurrentMap == client.Entity.CurrentMap &&
                                client.Entity.MyEvents[i].PageInstance.CurrentX == client.Entity.CurrentX &&
                                client.Entity.MyEvents[i].PageInstance.CurrentY == client.Entity.CurrentY &&
                                client.Entity.MyEvents[i].PageInstance.CurrentZ == client.Entity.CurrentZ)
                            {
                                if (client.Entity.MyEvents[i].PageInstance.Trigger != 1) return;
                                if (client.Entity.MyEvents[i].CallStack.Count != 0) return;
                                var newStack = new CommandInstance(client.Entity.MyEvents[i].PageInstance.MyPage)
                                {
                                    CommandIndex = 0,
                                    ListIndex = 0
                                };
                                client.Entity.MyEvents[i].CallStack.Push(newStack);
                            }
                        }
                    }
                }
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
            PacketSender.SendGlobalMsg(((Player)Globals.Entities[index]).MyName + ": " + msg);
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
                        PacketSender.SendServerConfig(client);
                        PacketSender.SendJoinGame(client);
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
            MapInstance.GetMap(mapNum).Load(bf.ReadBytes((int)mapLength), MapInstance.GetMap(mapNum).Revision + 1);
            Database.SaveGameObject(MapInstance.GetMap(mapNum));
            foreach (var t in Globals.Clients)
            {
                if (t == null) continue;
                if (t.IsEditor)
                {
                    PacketSender.SendMapList(t);
                }
            }
            PacketSender.SendMap(client, (int)mapNum); //Sends map to everyone/everything in proximity
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
                newMap = Database.AddGameObject(GameObject.Map).GetId();
                tmpMap = MapInstance.GetMap(newMap);
                Database.SaveGameObject(tmpMap);
                Database.GenerateMapGrids();
                PacketSender.SendMap(client, newMap);
                PacketSender.SendEnterMap(client, newMap);
                //FolderDirectory parent = null;
                destType = -1;
                if (destType == -1)
                {
                    MapList.GetList().AddMap(newMap, MapBase.GetObjects());
                }
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
                        if (MapInstance.GetMap(relativeMap).Up == -1)
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
                        if (MapInstance.GetMap(relativeMap).Down == -1)
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
                        if (MapInstance.GetMap(relativeMap).Left == -1)
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
                        if (MapInstance.GetMap(relativeMap).Right == -1)
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
                    PacketSender.SendMap(client, newMap);
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

        private static void HandleEnterMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int chunkNum = (int)bf.ReadLong();
            //TODO See if the player is close enough to be switching chunks.
            PacketSender.SendEnterMap(client, chunkNum);
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
                    PacketSender.SendPlayerMsg(client, "You are blinded and can't attack");
                    bf.Dispose();
                    return;
                }
            }

            //Fire projectile instead if weapon has it
            if (Options.WeaponIndex > -1)
            {
                if (client.Entity.Equipment[Options.WeaponIndex] >= 0 && ItemBase.GetItem(client.Entity.Inventory[client.Entity.Equipment[Options.WeaponIndex]].ItemNum) != null)
                {
                    var projectileBase = ProjectileBase.GetProjectile( ItemBase.GetItem(client.Entity.Inventory[client.Entity.Equipment[Options.WeaponIndex]].ItemNum).Projectile);
                    if (projectileBase != null)
                    {
                        MapInstance.GetMap(client.Entity.CurrentMap).SpawnMapProjectile(client.Entity,projectileBase,client.Entity.CurrentMap, client.Entity.CurrentX, client.Entity.CurrentY,client.Entity.CurrentZ, client.Entity.Dir);
                        bf.Dispose();
                        return;
                    }
                }
            }

            //Attack normally.
            if (target > -1) client.Entity.TryAttack((int)target);
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
            PacketSender.SendEntityDataTo(client, index, (int)EntityTypes.Player, client.Entity.Data(), client.Entity);
            Globals.Entities[index].Warp(Globals.Entities[index].CurrentMap, Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY, Globals.Entities[index].Dir);

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
                    PacketSender.SendGameObjects(client,GameObject.Class);
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
            if (classBase == null)
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
                    player.Gender = classBase.Sprites[Sprite].Gender;
                }
                player.WarpToSpawn(true);
                player.Vital[(int)Vitals.Health] = classBase.MaxVital[(int)Vitals.Health];
                player.Vital[(int)Vitals.Mana] = classBase.MaxVital[(int)Vitals.Mana];
                player.MaxVital[(int)Vitals.Health] = classBase.MaxVital[(int)Vitals.Health];
                player.MaxVital[(int)Vitals.Mana] = classBase.MaxVital[(int)Vitals.Mana];

                for (int i = 0; i < (int)Stats.StatCount; i++)
                {
                    player.Stat[i].Stat = classBase.Stat[i];
                }
                player.StatPoints = classBase.Points;

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
                Database.SaveCharacter(client.Entity, true);
                Globals.GeneralLogs.Add(client.MyAccount + " has created a character.");
                PacketSender.SendJoinGame(client);
            }
            bf.Dispose();
        }

        private static void HandlePickupItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            if (index < MapInstance.GetMap(client.Entity.CurrentMap).MapItems.Count)
            {
                if (MapInstance.GetMap(client.Entity.CurrentMap).MapItems[index].X == client.Entity.CurrentX && MapInstance.GetMap(client.Entity.CurrentMap).MapItems[index].Y == client.Entity.CurrentY)
                {
                    if (client.Entity.TryGiveItem(((ItemInstance)MapInstance.GetMap(client.Entity.CurrentMap).MapItems[index])))
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
                            PacketSender.SendAlert(client, "Last Map", "Failed to delete map, you must have at least one map at all times!");
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
            if (client.Power == 0) { return; }
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = bf.ReadInteger();
            switch (type)
            {
                case (int)AdminActions.WarpTo:
                    client.Entity.Warp(bf.ReadInteger(), client.Entity.CurrentX, client.Entity.CurrentY);
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
                        if (gridY - 1 >= 0 && Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX, gridY - 1] > -1)
                        {
                            if (MapInstance.GetMap(Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX, gridY - 1]) != null)
                                MapInstance.GetMap(Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX, gridY - 1]).ClearConnections((int)Directions.Down);
                        }

                        //Down
                        if (gridY + 1 < Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].Height && Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX, gridY + 1] > -1)
                        {
                            if (MapInstance.GetMap(Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX, gridY + 1]) != null)
                                MapInstance.GetMap(Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX, gridY + 1]).ClearConnections((int)Directions.Up);
                        }

                        //Left
                        if (gridX - 1 >= 0 && Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX - 1, gridY] > -1)
                        {
                            if (MapInstance.GetMap(Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX - 1, gridY]) != null)
                               MapInstance.GetMap(Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX - 1, gridY]).ClearConnections((int)Directions.Right);
                        }

                        //Right
                        if (gridX + 1 < Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].Width && Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX + 1, gridY] > -1)
                        {
                            if (MapInstance.GetMap(Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX + 1, gridY]) != null)
                                MapInstance.GetMap(Database.MapGrids[MapInstance.GetMap(mapNum).MapGrid].MyGrid[gridX + 1, gridY]).ClearConnections((int)Directions.Left);
                        }

                        Database.GenerateMapGrids();
                        if (MapInstance.GetObjects().ContainsKey(curMap))
                        {
                            mapGrid = MapInstance.GetMap(curMap).MapGrid;
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
            if (MapInstance.GetObjects().ContainsKey((int)linkMap) && MapInstance.GetObjects().ContainsKey((int)adjacentMap))
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
                            if (x + xOffset >= 0 && x + xOffset < Database.MapGrids[linkGrid].Width && y + yOffset >= 0 && y + yOffset < Database.MapGrids[linkGrid].Height)
                            {
                                if (Database.MapGrids[adjacentGrid].MyGrid[x, y] != -1 &&
                                    Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset] != -1)
                                {
                                    //Incompatible Link!
                                    PacketSender.SendAlert(client, "Map Link Failure", "Failed to link map " + linkMap + " to map " + adjacentMap + ". If this merge were to happen, maps " +
                                        Database.MapGrids[adjacentGrid].MyGrid[x, y] + " and " + Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset] + " would occupy the same space in the world.");
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
                                if (x + xOffset >= 0 && x + xOffset < Database.MapGrids[linkGrid].Width && y + yOffset >= 0 && y + yOffset < Database.MapGrids[linkGrid].Height)
                                {
                                    if (Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset] != -1)
                                    {
                                        bool inXBounds = x > -1 &&
                                                         x < Database.MapGrids[adjacentGrid].Width;
                                        bool inYBounds = y > -1 &&
                                                        y < Database.MapGrids[adjacentGrid].Height;
                                        if (inXBounds && inYBounds)
                                            Database.MapGrids[adjacentGrid].MyGrid[x, y] = Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];

                                        if (inXBounds && y >= 0 && Database.MapGrids[adjacentGrid].MyGrid[x, y - 1] > -1)
                                        {
                                            MapInstance.GetMap(Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Up = Database.MapGrids[adjacentGrid].MyGrid[x, y - 1];
                                            MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x, y - 1]).Down = Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            Database.SaveGameObject(MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x, y - 1]));
                                        }

                                        if (inXBounds && y + 1 < Database.MapGrids[adjacentGrid].Height && Database.MapGrids[adjacentGrid].MyGrid[x, y + 1] > -1)
                                        {
                                            MapInstance.GetMap(Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Down = Database.MapGrids[adjacentGrid].MyGrid[x, y + 1];
                                            MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x, y + 1]).Up = Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            Database.SaveGameObject(MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x, y + 1]));
                                        }

                                        if (inYBounds && x - 1 >= 0 && Database.MapGrids[adjacentGrid].MyGrid[x - 1, y] > -1)
                                        {
                                            MapInstance.GetMap(Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Left = Database.MapGrids[adjacentGrid].MyGrid[x - 1, y];
                                            MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x - 1, y]).Right = Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            Database.SaveGameObject(MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x - 1, y]));
                                        }

                                        if (inYBounds && x + 1 < Database.MapGrids[adjacentGrid].Width && Database.MapGrids[adjacentGrid].MyGrid[x + 1, y] > -1)
                                        {
                                            MapInstance.GetMap(Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]).Right = Database.MapGrids[adjacentGrid].MyGrid[x + 1, y];
                                            MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x + 1, y]).Left = Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            Database.SaveGameObject(MapInstance.GetMap(Database.MapGrids[adjacentGrid].MyGrid[x + 1, y]));
                                        }

                                        Database.SaveGameObject(MapInstance.GetMap(Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]));
                                    }
                                }
                            }
                        }
                        Database.GenerateMapGrids();
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
            var type = (GameObject) bf.ReadInteger();
            var value = bf.ReadString();
            if (type == GameObject.Tileset)
            {
                foreach (var tileset in TilesetBase.GetObjects())
                    if (tileset.Value.GetValue() == value) return;
            }
            var obj = Database.AddGameObject(type);
            if (type == GameObject.Tileset)
            {
                ((TilesetBase) obj).SetValue(value);
                Database.SaveGameObject(obj);
            }
            PacketSender.SendGameObjectToAll(obj);
            bf.Dispose();
        }

        private static void HandleRequestOpenEditor(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var type = (GameObject)bf.ReadInteger();
            PacketSender.SendGameObjects(client,type);
            PacketSender.SendOpenEditor(client,type);
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
                        PacketSender.SendAlert(client, "Last Class", "Failed to delete class, you must have at least one class at all times!");
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (obj != null)
            {
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (obj != null)
            {
                obj.Load(bf.ReadBytes(bf.Length()));
                PacketSender.SendGameObjectToAll(obj, false);
                Database.SaveGameObject(obj);
            }
            bf.Dispose();
        }
    }
}

