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
using System.Drawing;
using System.IO;
using Intersect_Server.Classes.Entities;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Maps;

namespace Intersect_Server.Classes
{
    public class PacketHandler
    {
        public void HandlePacket(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var packetHeader = (Enums.ClientPackets)bf.ReadLong();
            packet = bf.ReadBytes(bf.Length());
            bf.Dispose();

            switch (packetHeader)
            {
                case Enums.ClientPackets.Ping:
                    HandlePing(client);
                    break;
                case Enums.ClientPackets.Login:
                    HandleLogin(client, packet);
                    break;
                case Enums.ClientPackets.NeedMap:
                    HandleNeedMap(client, packet);
                    break;
                case Enums.ClientPackets.SendMove:
                    HandlePlayerMove(client, packet);
                    break;
                case Enums.ClientPackets.LocalMessage:
                    HandleLocalMsg(client, packet);
                    break;
                case Enums.ClientPackets.EditorLogin:
                    HandleEditorLogin(client, packet);
                    break;
                case Enums.ClientPackets.SaveTilesetArray:
                    HandleTilesets(client, packet);
                    break;
                case Enums.ClientPackets.SaveMap:
                    HandleMap(client, packet);
                    break;
                case Enums.ClientPackets.CreateMap:
                    HandleCreateMap(client, packet);
                    break;
                case Enums.ClientPackets.EnterMap:
                    HandleEnterMap(client, packet);
                    break;
                case Enums.ClientPackets.TryAttack:
                    HandleTryAttack(client, packet);
                    break;
                case Enums.ClientPackets.SendDir:
                    HandleDir(client, packet);
                    break;
                case Enums.ClientPackets.EnterGame:
                    HandleEnterGame(client, packet);
                    break;
                case Enums.ClientPackets.ActivateEvent:
                    HandleActivateEvent(client, packet);
                    break;
                case Enums.ClientPackets.EventResponse:
                    HandleEventResponse(client, packet);
                    break;
                case Enums.ClientPackets.CreateAccount:
                    HandleCreateAccount(client, packet);
                    break;
                case Enums.ClientPackets.OpenItemEditor:
                    HandleItemEditor(client);
                    break;
                case Enums.ClientPackets.SaveItem:
                    HandleItemData(client, packet);
                    break;
                case Enums.ClientPackets.OpenNpcEditor:
                    HandleNpcEditor(client);
                    break;
                case Enums.ClientPackets.SaveNpc:
                    HandleNpcData(client, packet);
                    break;
                case Enums.ClientPackets.OpenSpellEditor:
                    HandleSpellEditor(client);
                    break;
                case Enums.ClientPackets.SaveSpell:
                    HandleSpellData(client, packet);
                    break;
                case Enums.ClientPackets.OpenAnimationEditor:
                    HandleAnimationEditor(client);
                    break;
                case Enums.ClientPackets.SaveAnimation:
                    HandleAnimationData(client, packet);
                    break;
                case Enums.ClientPackets.PickupItem:
                    HandlePickupItem(client, packet);
                    break;
                case Enums.ClientPackets.SwapItems:
                    HandleSwapItems(client, packet);
                    break;
                case Enums.ClientPackets.DropItems:
                    HandleDropItems(client, packet);
                    break;
                case Enums.ClientPackets.UseItem:
                    HandleUseItem(client, packet);
                    break;
                case Enums.ClientPackets.SwapSpells:
                    HandleSwapSpells(client, packet);
                    break;
                case Enums.ClientPackets.ForgetSpell:
                    HandleForgetSpell(client, packet);
                    break;
                case Enums.ClientPackets.UseSpell:
                    HandleUseSpell(client, packet);
                    break;
                case Enums.ClientPackets.UnequipItem:
                    HandleUnequipItem(client, packet);
                    break;
                case Enums.ClientPackets.UpgradeStat:
                    HandleUpgradeStat(client, packet);
                    break;
                case Enums.ClientPackets.HotbarChange:
                    HandleHotbarChange(client, packet);
                    break;
                case Enums.ClientPackets.OpenResourceEditor:
                    HandleResourceEditor(client);
                    break;
                case Enums.ClientPackets.SaveResource:
                    HandleResourceData(client, packet);
                    break;
                case Enums.ClientPackets.OpenClassEditor:
                    HandleClassEditor(client);
                    break;
                case Enums.ClientPackets.SaveClass:
                    HandleClassData(client, packet);
                    break;
                case Enums.ClientPackets.MapListUpdate:
                    HandleMapListUpdate(client, packet);
                    break;
                case Enums.ClientPackets.CreateCharacter:
                    HandleCreateCharacter(client, packet);
                    break;
                case Enums.ClientPackets.OpenQuestEditor:
                    HandleQuestEditor(client);
                    break;
                case Enums.ClientPackets.SaveQuest:
                    HandleQuestData(client, packet);
                    break;
                case Enums.ClientPackets.OpenAdminWindow:
                    HandleOpenAdminWindow(client);
                    break;
                case Enums.ClientPackets.AdminAction:
                    HandleAdminAction(client, packet);
                    break;
                case Enums.ClientPackets.NeedGrid:
                    HandleNeedGrid(client, packet);
                    break;
                case Enums.ClientPackets.OpenProjectileEditor:
                    HandleProjectileEditor(client);
                    break;
                case Enums.ClientPackets.SaveProjectile:
                    HandleProjectileData(client, packet);
                    break;
                case Enums.ClientPackets.UnlinkMap:
                    HandleUnlinkMap(client, packet);
                    break;
                case Enums.ClientPackets.LinkMap:
                    HandleLinkMap(client, packet);
                    break;
                case Enums.ClientPackets.OpenCommonEventEditor:
                    HandleOpenCommonEventEditor(client);
                    break;
                case Enums.ClientPackets.SaveCommonEvent:
                    HandleCommonEvent(client, packet);
                    break;
                case Enums.ClientPackets.OpenSwitchVariableEditor:
                    HandleOpenSwitchVariableEditor(client);
                    break;
                case Enums.ClientPackets.SaveSwitchVariable:
                    HandleSaveSwitchVariables(client, packet);
                    break;
                case Enums.ClientPackets.OpenShopEditor:
                    HandleOpenShopEditor(client);
                    break;
                case Enums.ClientPackets.SaveShop:
                    HandleSaveShop(client, packet);
                    break;
                case Enums.ClientPackets.BuyItem:
                    HandleBuyItem(client, packet);
                    break;
                case Enums.ClientPackets.SellItem:
                    HandleSellItem(client, packet);
                    break;
                case Enums.ClientPackets.CloseShop:
                    HandleCloseShop(client, packet);
                    break;
                case Enums.ClientPackets.CloseBank:
                    HandleCloseBank(client, packet);
                    break;
                case Enums.ClientPackets.DepositItem:
                    HandleDepositItem(client, packet);
                    break;
                case Enums.ClientPackets.WithdrawItem:
                    HandleWithdrawItem(client, packet);
                    break;
                case Enums.ClientPackets.MoveBankItem:
                    HandleMoveBankItem(client, packet);
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
                    Globals.Entities[index] = new Player(index, client);
                    client.Entity = (Player)Globals.Entities[index];
                    Globals.GeneralLogs.Add(client.MyAccount + " logged in.");
                    PacketSender.SendServerConfig(client);
                    if (Database.LoadPlayer(client))
                    {
                        PacketSender.SendJoinGame(client);
                    }
                    else
                    {
                        for (int i = 0; i < Options.MaxClasses; i++)
                        {
                            PacketSender.SendClass(client, i);
                        }
                        PacketSender.SendCreateCharacter(client);
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
            if (MapHelper.IsMapValid(mapNum))
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
            PacketSender.SendEntityMove(index, (int)Enums.EntityTypes.Player, Globals.Entities[index]);

            // Check for a warp, if so warp the player.
            Attribute attribute =
                Globals.GameMaps[Globals.Entities[index].CurrentMap].Attributes[
                    Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY];
            if (attribute != null && attribute.value == (int)Enums.MapAttributes.Warp)
            {
                Globals.Entities[index].Warp(attribute.data1, attribute.data2, attribute.data3, Globals.Entities[index].Dir);
            }

            if (oldMap != Globals.Entities[index].CurrentMap)
            {
                Globals.GameMaps[Globals.Entities[index].CurrentMap].PlayerEnteredMap(client);
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
                        PacketSender.SendTilesets(client);
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

        private static void HandleTilesets(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var tilesetCount = bf.ReadLong();
            if (tilesetCount <= 0) return;
            Globals.Tilesets = new string[tilesetCount];
            for (var i = 0; i < tilesetCount; i++)
            {
                Globals.Tilesets[i] = bf.ReadString();
            }
            File.WriteAllLines("Resources/Tilesets.dat", Globals.Tilesets);

            //Send the updated tilesets to all clients.
            for (var i = 0; i < Globals.Clients.Count; i++)
            {
                if (i == client.ClientIndex) continue;
                if (Globals.Clients[i] == null) continue;
                if (Globals.Clients[i].IsConnected())
                {
                    PacketSender.SendTilesets(Globals.Clients[i]);
                }
            }
        }

        private static void HandleMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var mapNum = bf.ReadLong();
            var mapLength = bf.ReadLong();
            Globals.GameMaps[mapNum].Load(bf.ReadBytes((int)mapLength), Globals.GameMaps[mapNum].Revision + 1);
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
            var tmpMap = new MapStruct(-1);
            bf.WriteBytes(packet);
            var location = (int)bf.ReadInteger();
            if (location == -1)
            {
                var destType = bf.ReadInteger();
                newMap = Database.AddMap();
                tmpMap = Globals.GameMaps[newMap];
                tmpMap.Save(true);
                Database.GenerateMapGrids();
                PacketSender.SendMap(client, newMap);
                PacketSender.SendEnterMap(client, newMap);
                FolderDirectory parent = null;
                destType = -1;
                if (destType == -1)
                {
                    Database.MapStructure.AddMap(newMap);
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
                Globals.GameMaps[relativeMap].Load(File.ReadAllBytes("Resources/Maps/" + relativeMap + ".map"), -1);
                switch (location)
                {
                    case 0:
                        if (Globals.GameMaps[relativeMap].Up == -1)
                        {
                            newMap = Database.AddMap();
                            tmpMap = Globals.GameMaps[newMap];
                            tmpMap.MapGrid = Globals.GameMaps[relativeMap].MapGrid;
                            tmpMap.MapGridX = Globals.GameMaps[relativeMap].MapGridX;
                            tmpMap.MapGridY = Globals.GameMaps[relativeMap].MapGridY - 1;
                            Globals.GameMaps[relativeMap].Up = newMap;

                        }
                        break;

                    case 1:
                        if (Globals.GameMaps[relativeMap].Down == -1)
                        {
                            newMap = Database.AddMap();
                            tmpMap = Globals.GameMaps[newMap];
                            tmpMap.MapGrid = Globals.GameMaps[relativeMap].MapGrid;
                            tmpMap.MapGridX = Globals.GameMaps[relativeMap].MapGridX;
                            tmpMap.MapGridY = Globals.GameMaps[relativeMap].MapGridY + 1;
                            Globals.GameMaps[relativeMap].Down = newMap;
                        }
                        break;

                    case 2:
                        if (Globals.GameMaps[relativeMap].Left == -1)
                        {
                            newMap = Database.AddMap();
                            tmpMap = Globals.GameMaps[newMap];
                            tmpMap.MapGrid = Globals.GameMaps[relativeMap].MapGrid;
                            tmpMap.MapGridX = Globals.GameMaps[relativeMap].MapGridX - 1;
                            tmpMap.MapGridY = Globals.GameMaps[relativeMap].MapGridY;
                            Globals.GameMaps[relativeMap].Left = newMap;
                        }
                        break;

                    case 3:
                        if (Globals.GameMaps[relativeMap].Right == -1)
                        {
                            newMap = Database.AddMap();
                            tmpMap = Globals.GameMaps[newMap];
                            tmpMap.MapGrid = Globals.GameMaps[relativeMap].MapGrid;
                            tmpMap.MapGridX = Globals.GameMaps[relativeMap].MapGridX + 1;
                            tmpMap.MapGridY = Globals.GameMaps[relativeMap].MapGridY;
                            Globals.GameMaps[relativeMap].Right = newMap;
                        }
                        break;

                }

                if (newMap > -1)
                {
                    Globals.GameMaps[relativeMap].Save();

                    if (tmpMap.MapGridX >= 0 && tmpMap.MapGridX < Database.MapGrids[tmpMap.MapGrid].Width)
                    {
                        if (tmpMap.MapGridY + 1 < Database.MapGrids[tmpMap.MapGrid].Height)
                        {
                            tmpMap.Down = Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX, tmpMap.MapGridY + 1];
                            if (tmpMap.Down > -1)
                            {
                                Globals.GameMaps[tmpMap.Down].Up = newMap;
                                Globals.GameMaps[tmpMap.Down].Save();
                            }
                        }
                        if (tmpMap.MapGridY - 1 >= 0)
                        {
                            tmpMap.Up = Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX, tmpMap.MapGridY - 1];
                            if (tmpMap.Up > -1)
                            {
                                Globals.GameMaps[tmpMap.Up].Down = newMap;
                                Globals.GameMaps[tmpMap.Up].Save();
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
                                Globals.GameMaps[tmpMap.Left].Right = newMap;
                                Globals.GameMaps[tmpMap.Left].Save();
                            }
                        }

                        if (tmpMap.MapGridX + 1 < Database.MapGrids[tmpMap.MapGrid].Width)
                        {
                            tmpMap.Right =
                                Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX + 1, tmpMap.MapGridY];
                            if (tmpMap.Right > -1)
                            {
                                Globals.GameMaps[tmpMap.Right].Left = newMap;
                                Globals.GameMaps[tmpMap.Right].Save();
                            }
                        }
                    }

                    tmpMap.Save(true);
                    Database.GenerateMapGrids();
                    PacketSender.SendMap(client, newMap);
                    PacketSender.SendEnterMap(client, newMap);
                    var folderDir = Database.MapStructure.FindMapParent(relativeMap, null);
                    if (folderDir != null)
                    {
                        folderDir.Children.AddMap(newMap);
                    }
                    else
                    {
                        Database.MapStructure.AddMap(newMap);
                    }
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

            //Fire projectile instead if weapon has it
            if (Options.WeaponIndex > -1)
            {
                if (client.Entity.Equipment[Options.WeaponIndex] >= 0 &&
                    client.Entity.Inventory[client.Entity.Equipment[Options.WeaponIndex]].ItemNum >= 0)
                {
                    if (
                        Globals.GameItems[client.Entity.Inventory[client.Entity.Equipment[Options.WeaponIndex]].ItemNum]
                            .Projectile >= 0)
                    {
                        Globals.GameMaps[client.Entity.CurrentMap].SpawnMapProjectile(client.Entity,
                            Globals.GameItems[
                                client.Entity.Inventory[client.Entity.Equipment[Options.WeaponIndex]].ItemNum].Projectile,
                            client.Entity.CurrentMap, client.Entity.CurrentX, client.Entity.CurrentY,
                            client.Entity.CurrentZ, client.Entity.Dir);
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
            PacketSender.SendTilesets(client);
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
            PacketSender.SendEntityDataTo(client, index, (int)Enums.EntityTypes.Player, client.Entity.Data(), client.Entity);
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
                    for (int i = 0; i < Options.MaxClasses; i++)
                    {
                        PacketSender.SendClass(client, i);
                    }
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
            if (Database.CharacterNameInUse(Name))
            {
                PacketSender.SendLoginError(client, "An account with this character name exists. Please choose another.");
            }
            else
            {
                Globals.Entities[index].MyName = Name;
                Database.Characters.Add(Name);
                ((Player)Globals.Entities[index]).Class = Class;
                if (Globals.GameClasses[Class].Sprites.Count > 0)
                {
                    Globals.Entities[index].MySprite = Globals.GameClasses[Class].Sprites[Sprite].Sprite;
                    ((Player)Globals.Entities[index]).Gender = Globals.GameClasses[Class].Sprites[Sprite].Gender;
                }
                ((Player)Globals.Entities[index]).WarpToSpawn();
                Globals.Entities[index].Vital[(int)Enums.Vitals.Health] = Globals.GameClasses[Class].MaxVital[(int)Enums.Vitals.Health];
                Globals.Entities[index].Vital[(int)Enums.Vitals.Mana] = Globals.GameClasses[Class].MaxVital[(int)Enums.Vitals.Mana];
                Globals.Entities[index].MaxVital[(int)Enums.Vitals.Health] = Globals.GameClasses[Class].MaxVital[(int)Enums.Vitals.Health];
                Globals.Entities[index].MaxVital[(int)Enums.Vitals.Mana] = Globals.GameClasses[Class].MaxVital[(int)Enums.Vitals.Mana];

                for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
                {
                    Globals.Entities[index].Stat[i].Stat = Globals.GameClasses[Class].Stat[i];
                }
                ((Player)Globals.Entities[index]).StatPoints = Globals.GameClasses[Class].Points;

                client.Entity = (Player)Globals.Entities[index];

                for (int i = 0; i < Globals.GameClasses[Class].Spells.Count; i++)
                {
                    if (Globals.GameClasses[Class].Spells[i].Level <= 1)
                    {
                        SpellInstance TempSpell = new SpellInstance();
                        TempSpell.SpellNum = Globals.GameClasses[Class].Spells[i].SpellNum;
                        ((Player)Globals.Entities[index]).TryTeachSpell(TempSpell, false);
                    }
                }

                for (int i = 0; i < Options.MaxNpcDrops; i++)
                {
                    ItemInstance TempItem = new ItemInstance(Globals.GameClasses[Class].Items[i].ItemNum, Globals.GameClasses[Class].Items[i].Amount);
                    ((Player)Globals.Entities[index]).TryGiveItem(TempItem, false);
                }

                Database.SavePlayer(client);
                Globals.GeneralLogs.Add(client.MyAccount + " has created a character.");
                PacketSender.SendJoinGame(client);
            }
            bf.Dispose();
        }

        private static void HandleItemData(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var itemNum = bf.ReadInteger();
            Globals.GameItems[itemNum].Load(bf.ReadBytes(bf.Length()), itemNum);
            Globals.GameItems[itemNum].Save((int)itemNum);
            bf.Dispose();
        }

        private static void HandleItemEditor(Client client)
        {
            for (var i = 0; i < Options.MaxItems; i++)
            {
                PacketSender.SendItem(client, i);
            }
            PacketSender.SendItemEditor(client);
        }

        private static void HandleNpcData(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var npcNum = bf.ReadInteger();
            Globals.GameNpcs[npcNum].Load(bf.ReadBytes(bf.Length()), npcNum);
            Globals.GameNpcs[npcNum].Save(npcNum);
            bf.Dispose();
        }

        private static void HandleNpcEditor(Client client)
        {
            for (var i = 0; i < Options.MaxNpcs; i++)
            {
                PacketSender.SendNpc(client, i);
            }
            PacketSender.SendNpcEditor(client);
        }

        private static void HandleSpellData(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.GameSpells[index].Load(bf.ReadBytes(bf.Length()), index);
            Globals.GameSpells[index].Save(index);
            bf.Dispose();
        }

        private static void HandleSpellEditor(Client client)
        {
            for (var i = 0; i < Options.MaxSpells; i++)
            {
                PacketSender.SendSpell(client, i);
            }
            PacketSender.SendSpellEditor(client);
        }

        private static void HandleAnimationData(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.GameAnimations[index].Load(bf.ReadBytes(bf.Length()), index);
            Globals.GameAnimations[index].Save(index);
            bf.Dispose();
        }

        private static void HandleAnimationEditor(Client client)
        {
            for (var i = 0; i < Options.MaxAnimations; i++)
            {
                PacketSender.SendAnimation(client, i);
            }
            PacketSender.SendAnimationEditor(client);
        }

        private static void HandlePickupItem(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            if (index < Globals.GameMaps[client.Entity.CurrentMap].MapItems.Count)
            {
                if (Globals.GameMaps[client.Entity.CurrentMap].MapItems[index].X == client.Entity.CurrentX && Globals.GameMaps[client.Entity.CurrentMap].MapItems[index].Y == client.Entity.CurrentY)
                {
                    if (client.Entity.TryGiveItem(((ItemInstance)Globals.GameMaps[client.Entity.CurrentMap].MapItems[index])))
                    {
                        //Remove Item From Map
                        Globals.GameMaps[client.Entity.CurrentMap].RemoveItem(index);
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

        private static void HandleResourceData(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var resourceNum = bf.ReadInteger();
            Globals.GameResources[resourceNum].Load(bf.ReadBytes(bf.Length()), resourceNum);
            Globals.GameResources[resourceNum].Save(resourceNum);
            bf.Dispose();
        }

        private static void HandleResourceEditor(Client client)
        {
            for (var i = 0; i < Options.MaxResources; i++)
            {
                PacketSender.SendResource(client, i);
            }
            PacketSender.SendResourceEditor(client);
        }

        private static void HandleClassData(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var classNum = bf.ReadInteger();
            Globals.GameClasses[classNum].Load(bf.ReadBytes(bf.Length()), classNum);
            Globals.GameClasses[classNum].Save(classNum);
            bf.Dispose();
        }

        private static void HandleClassEditor(Client client)
        {
            for (var i = 0; i < Options.MaxClasses; i++)
            {
                PacketSender.SendClass(client, i);
            }
            PacketSender.SendClassEditor(client);
        }

        private static void HandleQuestData(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var questNum = bf.ReadInteger();
            Globals.GameQuests[questNum].Load(bf.ReadBytes(bf.Length()), questNum);
            Globals.GameQuests[questNum].Save(questNum);
            bf.Dispose();
        }

        private static void HandleQuestEditor(Client client)
        {
            for (var i = 0; i < Options.MaxQuests; i++)
            {
                PacketSender.SendQuest(client, i);
            }
            PacketSender.SendQuestEditor(client);
        }

        private static void HandleProjectileData(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var projectileNum = bf.ReadInteger();
            Globals.GameProjectiles[projectileNum].Load(bf.ReadBytes(bf.Length()), projectileNum);
            Globals.GameProjectiles[projectileNum].Save(projectileNum);
            bf.Dispose();
        }

        private static void HandleProjectileEditor(Client client)
        {
            for (var i = 0; i < Options.MaxProjectiles; i++)
            {
                PacketSender.SendProjectile(client, i);
            }
            PacketSender.SendProjectileEditor(client);
        }

        private static void HandleMapListUpdate(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            int destType = -1;
            FolderDirectory parent = null;
            int mapNum = -1;
            bf.WriteBytes(packet);
            var type = bf.ReadInteger();
            switch (type)
            {
                case (int)Enums.MapListUpdates.MoveItem:
                    Database.MapStructure.HandleMove(bf.ReadInteger(), bf.ReadInteger(), bf.ReadInteger(), bf.ReadInteger());
                    break;
                case (int)Enums.MapListUpdates.AddFolder:
                    destType = bf.ReadInteger();
                    parent = null;
                    if (destType == -1)
                    {
                        Database.MapStructure.AddFolder("New Folder");
                    }
                    else if (destType == 0)
                    {
                        parent = Database.MapStructure.FindDir(bf.ReadInteger());
                        if (parent == null)
                        {
                            Database.MapStructure.AddFolder("New Folder");
                        }
                        else
                        {
                            parent.Children.AddFolder("New Folder");
                        }
                    }
                    else if (destType == 1)
                    {
                        mapNum = bf.ReadInteger();
                        parent = Database.MapStructure.FindMapParent(mapNum, null);
                        if (parent == null)
                        {
                            Database.MapStructure.AddFolder("New Folder");
                        }
                        else
                        {
                            parent.Children.AddFolder("New Folder");
                        }
                    }
                    break;
                case (int)Enums.MapListUpdates.Rename:
                    destType = bf.ReadInteger();
                    parent = null;
                    if (destType == 0)
                    {
                        parent = Database.MapStructure.FindDir(bf.ReadInteger());
                        parent.Name = bf.ReadString();
                        PacketSender.SendMapListToEditors();
                    }
                    else if (destType == 1)
                    {
                        mapNum = bf.ReadInteger();
                        Globals.GameMaps[mapNum].MyName = bf.ReadString();
                        Globals.GameMaps[mapNum].Save();
                        PacketSender.SendMapListToEditors();

                    }
                    break;
                case (int)Enums.MapListUpdates.Delete:
                    destType = bf.ReadInteger();
                    parent = null;
                    if (destType == 0)
                    {
                        Database.MapStructure.DeleteFolder(bf.ReadInteger());
                        PacketSender.SendMapListToEditors();
                    }
                    else if (destType == 1)
                    {
                        mapNum = bf.ReadInteger();
                        Globals.GameMaps[mapNum].Deleted = 1;
                        Database.CheckAllMapConnections();
                        Database.MapStructure.DeleteMap(mapNum);
                        Database.GenerateMapGrids();
                        Globals.GameMaps[mapNum].Save(false);
                        PacketSender.SendMapToEditors(mapNum);
                        PacketSender.SendMapListToEditors();
                    }
                    break;
            }
            File.WriteAllBytes("Resources/Maps/MapStructure.dat", Database.MapStructure.Data());
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
                case (int)Enums.AdminActions.WarpTo:
                    client.Entity.Warp(bf.ReadInteger(), client.Entity.CurrentX, client.Entity.CurrentY);
                    break;
            }
        }

        private static void HandleNeedGrid(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            int mapNum = (int)bf.ReadLong();
            if (MapHelper.IsMapValid(mapNum))
            {
                if (client.IsEditor)
                {
                    PacketSender.SendMapGrid(client, Globals.GameMaps[mapNum].MapGrid);
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
            if (MapHelper.IsMapValid(mapNum))
            {
                if (client.IsEditor)
                {
                    if (Globals.GameMaps[mapNum] != null && Globals.GameMaps[mapNum].Deleted == 0)
                    {
                        Globals.GameMaps[mapNum].ClearConnections();

                        int gridX = Globals.GameMaps[mapNum].MapGridX;
                        int gridY = Globals.GameMaps[mapNum].MapGridY;

                        //Up
                        if (gridY - 1 >= 0 && Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[gridX, gridY - 1] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[gridX, gridY - 1]] != null)
                                Globals.GameMaps[Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[gridX, gridY - 1]].ClearConnections((int)Enums.Directions.Down);
                        }

                        //Down
                        if (gridY + 1 < Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].Height && Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[gridX, gridY + 1] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[gridX, gridY + 1]] != null)
                                Globals.GameMaps[Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[gridX, gridY + 1]].ClearConnections((int)Enums.Directions.Up);
                        }

                        //Left
                        if (gridX - 1 >= 0 && Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[gridX - 1, gridY] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[gridX - 1, gridY]] != null)
                                Globals.GameMaps[Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[gridX - 1, gridY]].ClearConnections((int)Enums.Directions.Right);
                        }

                        //Right
                        if (gridX + 1 < Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].Width && Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[gridX + 1, gridY] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[gridX + 1, gridY]] != null)
                                Globals.GameMaps[Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[gridX + 1, gridY]].ClearConnections((int)Enums.Directions.Left);
                        }

                        Database.GenerateMapGrids();
                        if (MapHelper.IsMapValid(curMap))
                        {
                            mapGrid = Globals.GameMaps[curMap].MapGrid;
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
            long adjacentMap = bf.ReadLong();
            long linkMap = bf.ReadLong();
            long gridX = bf.ReadLong();
            long gridY = bf.ReadLong();
            bool canLink = true;
            if (MapHelper.IsMapValid((int)linkMap) && MapHelper.IsMapValid((int)adjacentMap))
            {
                //Clear to test if we can link.
                int linkGrid = Globals.GameMaps[linkMap].MapGrid;
                int adjacentGrid = Globals.GameMaps[adjacentMap].MapGrid;
                if (linkGrid != adjacentGrid)
                {
                    long xOffset = Globals.GameMaps[linkMap].MapGridX - gridX;
                    long yOffset = Globals.GameMaps[linkMap].MapGridY - gridY;
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

                                        if (inXBounds && y > 0 && Database.MapGrids[adjacentGrid].MyGrid[x, y - 1] > -1)
                                        {
                                            Globals.GameMaps[Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]].Up = Database.MapGrids[adjacentGrid].MyGrid[x, y - 1];
                                            Globals.GameMaps[Database.MapGrids[adjacentGrid].MyGrid[x, y - 1]].Down = Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            Globals.GameMaps[Database.MapGrids[adjacentGrid].MyGrid[x, y - 1]].Save();
                                        }

                                        if (inXBounds && y + 1 < Database.MapGrids[adjacentGrid].Height && Database.MapGrids[adjacentGrid].MyGrid[x, y + 1] > -1)
                                        {
                                            Globals.GameMaps[Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]].Down = Database.MapGrids[adjacentGrid].MyGrid[x, y + 1];
                                            Globals.GameMaps[Database.MapGrids[adjacentGrid].MyGrid[x, y + 1]].Up = Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            Globals.GameMaps[Database.MapGrids[adjacentGrid].MyGrid[x, y + 1]].Save();
                                        }

                                        if (inYBounds && x - 1 > 0 && Database.MapGrids[adjacentGrid].MyGrid[x - 1, y] > -1)
                                        {
                                            Globals.GameMaps[Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]].Left = Database.MapGrids[adjacentGrid].MyGrid[x - 1, y];
                                            Globals.GameMaps[Database.MapGrids[adjacentGrid].MyGrid[x - 1, y]].Right = Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            Globals.GameMaps[Database.MapGrids[adjacentGrid].MyGrid[x - 1, y]].Save();
                                        }

                                        if (inYBounds && x + 1 < Database.MapGrids[adjacentGrid].Width && Database.MapGrids[adjacentGrid].MyGrid[x + 1, y] > -1)
                                        {
                                            Globals.GameMaps[Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]].Right = Database.MapGrids[adjacentGrid].MyGrid[x + 1, y];
                                            Globals.GameMaps[Database.MapGrids[adjacentGrid].MyGrid[x + 1, y]].Left = Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset];
                                            Globals.GameMaps[Database.MapGrids[adjacentGrid].MyGrid[x + 1, y]].Save();
                                        }

                                        Globals.GameMaps[Database.MapGrids[linkGrid].MyGrid[x + xOffset, y + yOffset]].Save();
                                    }
                                }
                            }
                        }
                        Database.GenerateMapGrids();
                        PacketSender.SendMapGrid(client, Globals.GameMaps[adjacentMap].MapGrid);
                    }
                }
            }
        }

        private static void HandleOpenCommonEventEditor(Client client)
        {
            for (var i = 0; i < Options.MaxCommonEvents; i++)
            {
                PacketSender.SendCommonEvent(client, i);
            }
            PacketSender.SendOpenCommonEventEditor(client);
        }

        private static void HandleCommonEvent(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.CommonEvents[index] = new EventStruct(index, bf, true);
            File.WriteAllBytes("Resources/Common Events/" + index + ".evt", Globals.CommonEvents[index].EventData());
            bf.Dispose();
        }

        private static void HandleOpenSwitchVariableEditor(Client client)
        {
            for (var i = 0; i < Options.MaxCommonEvents; i++)
            {
                PacketSender.SendCommonEvent(client, i);
            }
            PacketSender.SendOpenSwitchVariableEditor(client);
        }

        private static void HandleSaveSwitchVariables(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var changeCount = bf.ReadInteger();
            for (int i = 0; i < changeCount; i++)
            {
                var type = bf.ReadInteger();
                var index = bf.ReadInteger();
                switch (type)
                {
                    case (int)Enums.SwitchVariableTypes.PlayerSwitch:
                        Globals.PlayerSwitches[index] = bf.ReadString();
                        break;
                    case (int)Enums.SwitchVariableTypes.PlayerVariable:
                        Globals.PlayerVariables[index] = bf.ReadString();
                        break;
                    case (int)Enums.SwitchVariableTypes.ServerSwitch:
                        Globals.ServerSwitches[index] = bf.ReadString();
                        Globals.ServerSwitchValues[index] = Convert.ToBoolean(bf.ReadInteger());
                        break;
                    case (int)Enums.SwitchVariableTypes.ServerVariable:
                        Globals.ServerVariables[index] = bf.ReadString();
                        Globals.ServerVariableValues[index] = bf.ReadInteger();
                        break;
                }
            }
            Database.SaveSwitchesAndVariables();
            bf.Dispose();
        }

        private static void HandleOpenShopEditor(Client client)
        {
            for (var i = 0; i < Options.MaxShops; i++)
            {
                PacketSender.SendShop(client, i);
            }
            PacketSender.SendOpenShopEditor(client);
        }

        private static void HandleSaveShop(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var index = bf.ReadInteger();
            Globals.GameShops[index].Load(bf.ReadBytes(bf.Length()), index);
            Globals.GameShops[index].Save(index);
            bf.Dispose();
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
    }
}

