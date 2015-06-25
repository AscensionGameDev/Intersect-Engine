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
using System.IO;

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
                default:
                    Console.WriteLine(@"Non implemented packet received: " + packetHeader);
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
                    Globals.Entities[index] = new Player(index, client) { MyName = username };
                    client.Entity = (Player)Globals.Entities[index];
                    Console.WriteLine(Globals.Entities[index].MyName + " logged in.");
                    client.Id = Database.GetUserId(username);
                    Database.LoadPlayer(client);
                    PacketSender.SendJoinGame(client);
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
            PacketSender.SendMap(client, (int)bf.ReadLong());
        }

        private static void HandlePlayerMove(Client client, byte[] packet)
        {
            var index = client.EntityIndex;
            var oldMap = Globals.Entities[index].CurrentMap;
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            Globals.Entities[index].CurrentMap = bf.ReadInteger();
            if (oldMap != Globals.Entities[index].CurrentMap)
            {
                Globals.GameMaps[Globals.Entities[index].CurrentMap].PlayerEnteredMap();
            }
            Globals.Entities[index].CurrentX = bf.ReadInteger();
            Globals.Entities[index].CurrentY = bf.ReadInteger();
            Globals.Entities[index].Dir = bf.ReadInteger();
            bf.Dispose();

            // Check for a warp, if so warp the player.
            if (Globals.GameMaps[Globals.Entities[index].CurrentMap].Attributes[Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY].value == (int)Enums.MapAttributes.Warp)
            {
                Globals.Entities[index].Warp(Globals.GameMaps[Globals.Entities[index].CurrentMap].Attributes[Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY].data1,
                    Globals.GameMaps[Globals.Entities[index].CurrentMap].Attributes[Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY].data2,
                    Globals.GameMaps[Globals.Entities[index].CurrentMap].Attributes[Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY].data3,
                    Globals.Entities[index].Dir);
            }

            //TODO: Add Check if valid before sending the move to everyone.
            PacketSender.SendEntityMove(index, 0, Globals.Entities[index]);
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
            if (usr != "jcsnider" && (usr != "kibbelz" || pass != "test")) return;
            client.IsEditor = true;
            PacketSender.SendJoinGame(client);
            PacketSender.SendGameData(client);
            PacketSender.SendTilesets(client);
            PacketSender.SendMapList(client);
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
                if (Globals.Clients[i].isConnected)
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
            Globals.GameMaps[mapNum].Load(bf.ReadBytes((int)mapLength));
            Globals.GameMaps[mapNum].Save();
            foreach (var t in Globals.Clients)
            {
                if (t == null) continue;
                if (t.IsEditor)
                {
                    PacketSender.SendMapList(t);
                }
            }
            bf.Dispose();
        }

        private static void HandleCreateMap(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            var newMap = -1;
            var tmpMap = new MapStruct(-1);
            bf.WriteBytes(packet);
            var location = (int)bf.ReadLong();
            if (location == -1)
            {
                newMap = Database.AddMap();
                tmpMap = Globals.GameMaps[newMap];
                tmpMap.Save();
                Database.GenerateMapGrids();
                PacketSender.SendMap(client, newMap);
                PacketSender.SendEnterMap(client, newMap);
            }
            else
            {
                var relativeMap = (int)bf.ReadLong();
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
                    if (tmpMap.MapGridY - 1 >= 0)
                    {
                        tmpMap.Up = Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX, tmpMap.MapGridY - 1];
                    }
                    if (tmpMap.MapGridY + 1 <= Database.MapGrids[tmpMap.MapGrid].Height)
                    {
                        tmpMap.Down = Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX, tmpMap.MapGridY + 1];
                    }

                    if (tmpMap.MapGridX - 1 >= 0) { tmpMap.Left = Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX - 1, tmpMap.MapGridY]; }

                    if (tmpMap.MapGridX + 1 <= Database.MapGrids[tmpMap.MapGrid].Width)
                    {
                        tmpMap.Right = Database.MapGrids[tmpMap.MapGrid].MyGrid[tmpMap.MapGridX + 1, tmpMap.MapGridY];
                    }

                    tmpMap.Save();
                    Database.GenerateMapGrids();
                    PacketSender.SendMap(client, newMap);
                    PacketSender.SendEnterMap(client, newMap);
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
            client.Entity.TryAttack((int)bf.ReadLong());
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
            PacketSender.SendGameTime(client);
            PacketSender.SendPlayerMsg(client, "Welcome to the Intersect game server.");
            PacketSender.SendGlobalMsg(Globals.Entities[index].MyName + " has joined the Intersect engine");
            PacketSender.SendTilesets(client);
            for (var i = 0; i < Globals.Entities.Count; i++)
            {
                if (Globals.Entities[i] != null)
                {
                    PacketSender.SendEntityData(client, i, 0, Globals.Entities[i]);
                }
            }
            for (var i = 0; i < Globals.Clients.Count; i++)
            {
                if (i == client.ClientIndex) continue;
                if (Globals.Clients[i] == null) continue;
                if (!Globals.Clients[i].isConnected) continue;
                if (!Globals.Clients[i].IsEditor)
                {
                    PacketSender.SendEntityData(Globals.Clients[i], client.EntityIndex, 0, client.Entity);
                }
            }
            Globals.Entities[index].Warp(Globals.Entities[index].CurrentMap, Globals.Entities[index].CurrentX, Globals.Entities[index].CurrentY, Globals.Entities[index].Dir);

        }

        private static void HandleActivateEvent(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            ((Player)(client.Entity)).TryActivateEvent(bf.ReadInteger());
            bf.Dispose();
        }

        private static void HandleEventResponse(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            ((Player)(client.Entity)).RespondToEvent(bf.ReadInteger(), bf.ReadInteger());
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
                    Database.CreateAccount(username, password, email);
                    client.Id = Database.GetUserId(username);
                    Globals.Entities[index].MyName = username;
                    Console.WriteLine(Globals.Entities[index].MyName + " logged in.");
                    Globals.Entities[index].MySprite = "5";
                    Globals.Entities[index].CurrentMap = Constants.SpawnMap;
                    Globals.Entities[index].CurrentX = Constants.SpawnX;
                    Globals.Entities[index].CurrentY = Constants.SpawnY;
                    Database.SavePlayer(client);
                    PacketSender.SendJoinGame(client);
                }
            }
            bf.Dispose();
        }

        private static void HandleItemData(Client client, byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            var itemNum = bf.ReadLong();
            Globals.GameItems[itemNum].Load(bf.ReadBytes(bf.Length()));
            Globals.GameItems[itemNum].Save((int)itemNum);
            bf.Dispose();
        }

        private static void HandleItemEditor(Client client)
        {
            for (var i = 0; i < Constants.MaxItems; i++)
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
            Globals.GameNpcs[npcNum].Load(bf.ReadBytes(bf.Length()));
            Globals.GameNpcs[npcNum].Save(npcNum);
            bf.Dispose();
        }

        private static void HandleNpcEditor(Client client)
        {
            for (var i = 0; i < Constants.MaxNpcs; i++)
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
            Globals.GameSpells[index].Load(bf.ReadBytes(bf.Length()));
            Globals.GameSpells[index].Save(index);
            bf.Dispose();
        }

        private static void HandleSpellEditor(Client client)
        {
            for (var i = 0; i < Constants.MaxSpells; i++)
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
            Globals.GameAnimations[index].Load(bf.ReadBytes(bf.Length()));
            Globals.GameAnimations[index].Save(index);
            bf.Dispose();
        }

        private static void HandleAnimationEditor(Client client)
        {
            for (var i = 0; i < Constants.MaxAnimations; i++)
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
            client.Entity.UseSpell(slot);
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
    }
}

