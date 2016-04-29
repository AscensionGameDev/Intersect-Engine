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
using System.Collections.Generic;
using System.Linq;
using Intersect_Library;
using Intersect_Library.GameObjects.Maps;
using Intersect_Library.GameObjects.Maps.MapList;
using Intersect_Server.Classes.Core;
using Intersect_Server.Classes.Entities;
using Intersect_Server.Classes.General;


namespace Intersect_Server.Classes.Networking
{
    public static class PacketSender
    {

        public static void SendDataToMap(int mapNum, byte[] data)
        {
            if (!Globals.GameMaps.ContainsKey(mapNum)) { return; }
            List<int> Players = Globals.GameMaps[mapNum].GetPlayersOnMap();
            for (int i = 0; i < Players.Count; i++)
            {
                Globals.Clients[Players[i]].SendPacket(data);
            }
        }
        public static void SendDataToProximity(int mapNum, byte[] data)
        {
            if (!Globals.GameMaps.ContainsKey(mapNum)) { return; }
            SendDataToMap(mapNum, data);
            for (int i = 0; i < Globals.GameMaps[mapNum].SurroundingMaps.Count; i++)
            {
                SendDataToMap(Globals.GameMaps[mapNum].SurroundingMaps[i], data);
            }
        }
        public static void SendDataToEditors(byte[] data)
        {
            for (var i = 0; i < Globals.Clients.Count; i++)
            {
                if (Globals.Clients[i] == null) continue;
                if (!Globals.Clients[i].IsConnected()) continue;
                if (Globals.Clients[i].IsEditor)
                {
                    Globals.Clients[i].SendPacket(data);
                }
            }
        }

        public static void SendPing(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.RequestPing);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
            client.ConnectionTimeout = Environment.TickCount + (client.TimeoutLength * 1000);
        }

        public static void SendServerConfig(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ServerConfig);
            bf.WriteBytes(ServerOptions.GetServerConfig());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendJoinGame(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.JoinGame);
            bf.WriteLong(client.EntityIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMap(Client client, int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.MapData);
            bf.WriteLong(mapNum);
            bool isEditor = false;
            if (client != null && client.IsEditor) isEditor = true;
            byte[] MapData = null;
            if (isEditor)
            {
                MapData = Globals.GameMaps[mapNum].GetEditorMapData();
                bf.WriteLong(MapData.Length);
                bf.WriteBytes(MapData);
            }
            else
            {
                MapData = Globals.GameMaps[mapNum].GetClientMapData();
                bf.WriteLong(MapData.Length);
                bf.WriteBytes(MapData);
                bf.WriteInteger(Globals.GameMaps[mapNum].Revision);
                bf.WriteInteger(Globals.GameMaps[mapNum].MapGridX);
                bf.WriteInteger(Globals.GameMaps[mapNum].MapGridY);
                if (Options.GameBorderStyle == 1)
                {
                    bf.WriteInteger(1);
                    bf.WriteInteger(1);
                    bf.WriteInteger(1);
                    bf.WriteInteger(1);
                }
                else if (Options.GameBorderStyle == 0)
                {
                    if (0 == Globals.GameMaps[mapNum].MapGridX)
                    {
                        bf.WriteInteger(1);
                    }
                    else
                    {
                        bf.WriteInteger(0);
                    }
                    if (Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].XMax - 1 == Globals.GameMaps[mapNum].MapGridX)
                    {
                        bf.WriteInteger(1);
                    }
                    else
                    {
                        bf.WriteInteger(0);
                    }
                    if (0 == Globals.GameMaps[mapNum].MapGridY)
                    {
                        bf.WriteInteger(1);
                    }
                    else
                    {
                        bf.WriteInteger(0);
                    }
                    if (Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].YMax - 1 == Globals.GameMaps[mapNum].MapGridY)
                    {
                        bf.WriteInteger(1);
                    }
                    else
                    {
                        bf.WriteInteger(0);
                    }
                }
                else
                {
                    bf.WriteInteger(0);
                    bf.WriteInteger(0);
                    bf.WriteInteger(0);
                    bf.WriteInteger(0);
                }
            }
            if (client != null)
            {
                client.SendPacket(bf.ToArray());
                if (isEditor)
                {
                    SendDataToEditors(bf.ToArray());
                }
                else
                {
                    Globals.GameMaps[mapNum].SendMapEntitiesTo(client);
                }
            }
            else if (client == null)
            {
                SendDataToProximity(mapNum, bf.ToArray());
                SendMapItemsToProximity(mapNum);
                SendDataToEditors(bf.ToArray());
            }
            bf.Dispose();
        }

        public static void SendMapToEditors(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.MapData);
            bf.WriteLong(mapNum);
            byte[] MapData = Globals.GameMaps[mapNum].GetEditorMapData();
            bf.WriteLong(MapData.Length);
            bf.WriteBytes(MapData);
            SendDataToEditors(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityDataTo(Client client, int sendIndex, int type, byte[] data, Entity en)
        {
            if (sendIndex == -1) { return; }
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityData);
            bf.WriteLong(sendIndex);
            bf.WriteInteger(type);
            bf.WriteBytes(data);

            client.SendPacket(bf.ToArray());
            bf.Dispose();
            SendEntityVitalsTo(client, sendIndex, type, en);
            SendEntityStatsTo(client, sendIndex, type, en);
            SendEntityPositionTo(client, sendIndex, type, en);

            if (en == client.Entity)
            {
                SendExperience(client);
                SendInventory(client);
                SendPlayerSpells(client);
                SendPlayerEquipmentToProximity(client.Entity);
                SendPointsTo(client);
                SendHotbarSlots(client);
            }
        }

        public static void SendEntityDataToProximity(int entityIndex, int type, byte[] data, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityData);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteBytes(data);
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
            SendEntityVitals(entityIndex, type, en);
            SendEntityStats(entityIndex, type, en);
        }

        public static void SendEntityPositionTo(Client client, int entityIndex, int type, Entity en)
        {
            if (en == null) { return; }
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityPosition);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(en.Passable);
            bf.WriteInteger(en.HideName);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityPositionToAll(int entityIndex, int type, Entity en)
        {
            if (en == null) { return; }
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityPosition);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(en.Passable);
            bf.WriteInteger(en.HideName);
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityLeave(int entityIndex, int type, int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityLeave);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(mapNum);
            SendDataToProximity(mapNum, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityLeaveTo(Client client, int entityIndex, int type, int map)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityLeave);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(map);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendDataToAll(byte[] packet)
        {
            foreach (var t in Globals.Clients)
            {
                if (t == null) continue;
                if (t.IsConnected() && ((Player)Globals.Entities[t.EntityIndex]).InGame)
                {
                    t.SendPacket(packet);
                }
            }
        }

        public static void SendDataTo(Client client, byte[] packet)
        {
            client.SendPacket(packet);
        }

        public static void SendPlayerMsg(Client client, string message)
        {
            SendPlayerMsg(client, message, Color.Black);
        }

        public static void SendPlayerMsg(Client client, string message, Color clr)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ChatMessage);
            bf.WriteString(message);
            bf.WriteByte(clr.A);
            bf.WriteByte(clr.R);
            bf.WriteByte(clr.G);
            bf.WriteByte(clr.B);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendGameData(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.GameData);
            bf.WriteLong(Globals.GameMaps.Count); //Map Count
            bf.WriteInteger(Options.GameBorderStyle);
            client.SendPacket(bf.ToArray());
            bf.Dispose();

            //Send massive amounts of game data
            for (int i = 0; i < Options.MaxItems; i++)
            {
                SendItem(client, i);
            }
            for (int i = 0; i < Options.MaxAnimations; i++)
            {
                SendAnimation(client, i);
            }
            for (int i = 0; i < Options.MaxNpcs; i++)
            {
                SendNpc(client, i);
            }
            for (int i = 0; i < Options.MaxSpells; i++)
            {
                SendSpell(client, i);
            }
            for (int i = 0; i < Options.MaxResources; i++)
            {
                SendResource(client, i);
            }
            for (int i = 0; i < Options.MaxClasses; i++)
            {
                SendClass(client, i);
            }
            for (int i = 0; i < Options.MaxQuests; i++)
            {
                SendQuest(client, i);
            }
            for (int i = 0; i < Options.MaxProjectiles; i++)
            {
                SendProjectile(client, i);
            }
            if (client.IsEditor)
            {
                for (int i = 0; i < Options.MaxShops; i++)
                {
                    SendShop(client, i);
                }
                for (int i = 0; i < Options.MaxCommonEvents; i++)
                {
                    SendCommonEvent(client, i);
                }
                SendSwitchesAndVariablesTo(client);
            }
        }

        public static void SendGlobalMsg(string message)
        {
            SendGlobalMsg(message, Color.Black);
        }

        public static void SendGlobalMsg(string message, Color clr)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ChatMessage);
            bf.WriteString(message);
            bf.WriteByte(clr.A);
            bf.WriteByte(clr.R);
            bf.WriteByte(clr.G);
            bf.WriteByte(clr.B);
            SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendProximityMsg(string message, int centerMap)
        {
            SendProximityMsg(message, centerMap, Color.Black);
        }

        public static void SendProximityMsg(string message, int centerMap, Color clr)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ChatMessage);
            bf.WriteString(message);
            bf.WriteByte(clr.A);
            bf.WriteByte(clr.R);
            bf.WriteByte(clr.G);
            bf.WriteByte(clr.B);
            SendDataToProximity(centerMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendTilesets(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.TilesetArray);
            if (Globals.Tilesets != null)
            {
                bf.WriteLong(Globals.Tilesets.Length);
                foreach (var t in Globals.Tilesets)
                {
                    bf.WriteString(t);
                }
            }
            else
            {
                bf.WriteLong(0);
            }
            client.SendPacket(bf.ToArray());
        }

        public static void SendEnterMap(Client client, int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EnterMap);
            bf.WriteLong(mapNum);
            if (!(Globals.GameMaps[mapNum].MapGridX == -1 || Globals.GameMaps[mapNum].MapGridY == -1))
            {
                if (!client.IsEditor){  Globals.GameMaps[mapNum].PlayerEnteredMap(client);}
                for (var y = Globals.GameMaps[mapNum].MapGridY - 1; y < Globals.GameMaps[mapNum].MapGridY + 2; y++)
                {
                    for (var x = Globals.GameMaps[mapNum].MapGridX - 1; x < Globals.GameMaps[mapNum].MapGridX + 2; x++)
                    {
                        if (x >= Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].XMin && x < Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].XMax && y >= Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].YMin && y < Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].YMax)
                        {
                            bf.WriteLong(Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[x, y]);
                        }
                        else
                        {
                            bf.WriteLong(-1);
                        }
                        
                    }
                }
                client.SendPacket(bf.ToArray());
                
                //Send Map Info
                for (int i = 0; i < Globals.GameMaps[mapNum].SurroundingMaps.Count; i++)
                {
                    PacketSender.SendMapItems(client, Globals.GameMaps[mapNum].SurroundingMaps[i]);
                }
            }
            bf.Dispose();
        }

        public static void SendDataToAllBut(int index, byte[] packet, bool entityId)
        {
            for (var i = 0; i < Globals.Clients.Count; i++)
            {
                if (Globals.Clients[i] == null) continue;
                if ((!entityId || Globals.Clients[i].EntityIndex == index) && (entityId || i == index)) continue;
                if (!Globals.Clients[i].IsConnected() || Globals.Clients[i].EntityIndex <= -1) continue;
                if (Globals.Entities[Globals.Clients[i].EntityIndex] == null) continue;
                if (((Player)Globals.Entities[Globals.Clients[i].EntityIndex]).InGame)
                {
                    Globals.Clients[i].SendPacket(packet);
                }
            }
        }

        internal static void SendRemoveProjectileSpawn(int map, int baseEntityIndex, int spawnIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ProjectileSpawnDead);
            bf.WriteLong(baseEntityIndex);
            bf.WriteLong(spawnIndex);
            SendDataToProximity(map, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityMove(int entityIndex, int type, Entity en, int correction = 0)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityMove);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(correction);
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityMoveTo(Client client, int entityIndex, int type, Entity en, int correction = 0)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityMove);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(correction);
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityVitals(int entityIndex, int type, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityVitals);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(en.CurrentMap);
            for (var i = 0; i < (int)Vitals.VitalCount; i++)
            {
                bf.WriteInteger(en.MaxVital[i]);
                bf.WriteInteger(en.Vital[i]);
            }
            bf.WriteInteger(en.Status.Count);
            for (var i = 0; i < en.Status.Count; i++)
            {
                bf.WriteInteger(en.Status[i].Type);
                bf.WriteString(en.Status[i].Data);
            }
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityStats(int entityIndex, int type, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityStats);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(en.CurrentMap);
            for (var i = 0; i < (int)Stats.StatCount; i++)
            {
                bf.WriteInteger(en.Stat[i].Value());
            }
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityVitalsTo(Client client, int entityIndex, int type, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityVitals);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(en.CurrentMap);
            for (var i = 0; i < (int)Vitals.VitalCount; i++)
            {
                bf.WriteInteger(en.MaxVital[i]);
                bf.WriteInteger(en.Vital[i]);
            }
            bf.WriteInteger(en.Status.Count);
            for (var i = 0; i < en.Status.Count; i++)
            {
                bf.WriteInteger(en.Status[i].Type);
                bf.WriteString(en.Status[i].Data);
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityStatsTo(Client client, int entityIndex, int type, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityStats);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(en.CurrentMap);
            for (var i = 0; i < (int)Stats.StatCount; i++)
            {
                bf.WriteInteger(en.Stat[i].Value());
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityDir(int entityIndex, int type, int dir, int map)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityDir);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(map);
            bf.WriteInteger(dir);
            SendDataToProximity(map, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityDirTo(Client client, int entityIndex, int type, int dir, int map)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityDir);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(map);
            bf.WriteInteger(dir);
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEventDialog(Client client, string prompt,string face, int mapNum, int eventIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EventDialog);
            bf.WriteString(prompt);
            bf.WriteString(face);
            bf.WriteInteger(0);
            bf.WriteInteger(mapNum);
            bf.WriteInteger(eventIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }
        public static void SendEventDialog(Client client, string prompt, string opt1, string opt2, string opt3, string opt4,string face,int mapNum, int eventIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EventDialog);
            bf.WriteString(prompt);
            bf.WriteString(face);
            bf.WriteInteger(1);
            bf.WriteString(opt1);
            bf.WriteString(opt2);
            bf.WriteString(opt3);
            bf.WriteString(opt4);
            bf.WriteInteger(mapNum);
            bf.WriteInteger(eventIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapList(Client client)
        {
            var bf = new ByteBuffer();
            Dictionary<int, MapStruct> gameMaps = Globals.GameMaps.ToDictionary(k => k.Key, v => (MapStruct)v.Value);
            bf.WriteLong((int)ServerPackets.MapList);
            bf.WriteBytes(MapList.GetList().Data(gameMaps));
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapListToEditors()
        {
            var bf = new ByteBuffer();
            Dictionary<int, MapStruct> gameMaps = Globals.GameMaps.ToDictionary(k => k.Key, v => (MapStruct)v.Value);
            bf.WriteLong((int)ServerPackets.MapList);
            bf.WriteBytes(MapList.GetList().Data(gameMaps));
            SendDataToEditors(bf.ToArray());
            bf.Dispose();
        }

        public static void SendLoginError(Client client, string error)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.LoginError);
            bf.WriteString(error);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendItemList(Client client)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ItemList);
            for (int i = 0; i < Options.MaxItems; i++)
            {
                bf.WriteString(Globals.GameItems[i].Name);
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendItem(Client client, int itemNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ItemData);
            bf.WriteInteger(itemNum);
            bf.WriteBytes(Globals.GameItems[itemNum].ItemData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendItemEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenItemEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNpcList(Client client)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.NpcList);
            for (int i = 0; i < Options.MaxNpcs; i++)
            {
                bf.WriteString(Globals.GameNpcs[i].Name);
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNpc(Client client, int npcNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.NpcData);
            bf.WriteInteger(npcNum);
            bf.WriteBytes(Globals.GameNpcs[npcNum].NpcData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNpcEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenNpcEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendSpellList(Client client)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.SpellList);
            for (int i = 0; i < Options.MaxSpells; i++)
            {
                bf.WriteString(Globals.GameSpells[i].Name);
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendSpell(Client client, int spellNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.SpellData);
            bf.WriteInteger(spellNum);
            bf.WriteBytes(Globals.GameSpells[spellNum].SpellData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendSpellEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenSpellEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendAnimationsList(Client client)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.AnimationList);
            for (int i = 0; i < Options.MaxAnimations; i++)
            {
                bf.WriteString(Globals.GameAnimations[i].Name);
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendAnimation(Client client, int animNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.AnimationData);
            bf.WriteInteger(animNum);
            bf.WriteBytes(Globals.GameAnimations[animNum].AnimData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendAnimationEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenAnimationEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapItems(Client client, int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.MapItems);
            bf.WriteInteger(mapNum);
            bf.WriteInteger(Globals.GameMaps[mapNum].MapItems.Count);
            for (int i = 0; i < Globals.GameMaps[mapNum].MapItems.Count; i++)
            {
                bf.WriteBytes(Globals.GameMaps[mapNum].MapItems[i].Data());
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapItemsToProximity(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.MapItems);
            bf.WriteInteger(mapNum);
            bf.WriteInteger(Globals.GameMaps[mapNum].MapItems.Count);
            for (int i = 0; i < Globals.GameMaps[mapNum].MapItems.Count; i++)
            {
                bf.WriteBytes(Globals.GameMaps[mapNum].MapItems[i].Data());
            }
            SendDataToProximity(mapNum, bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapItemUpdate(int mapNum, int index)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.MapItemUpdate);
            bf.WriteInteger(mapNum);
            bf.WriteInteger(index);
            if (Globals.GameMaps[mapNum].MapItems[index].ItemNum == -1)
            {
                bf.WriteInteger(-1);
            }
            else
            {
                bf.WriteInteger(1);
                bf.WriteBytes(Globals.GameMaps[mapNum].MapItems[index].Data());
            }
            SendDataToProximity(mapNum, bf.ToArray());
            bf.Dispose();
        }

        public static void SendInventory(Client client)
        {
            for (int i = 0; i < Options.MaxInvItems; i++)
            {
                SendInventoryItemUpdate(client, i);
            }
        }
        public static void SendInventoryItemUpdate(Client client, int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.InventoryUpdate);
            bf.WriteInteger(slot);
            bf.WriteBytes(client.Entity.Inventory[slot].Data());
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }
        public static void SendPlayerSpells(Client client)
        {
            for (int i = 0; i < Options.MaxPlayerSkills; i++)
            {
                SendPlayerSpellUpdate(client, i);
            }
        }
        public static void SendPlayerSpellUpdate(Client client, int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.SpellUpdate);
            bf.WriteInteger(slot);
            bf.WriteBytes(client.Entity.Spells[slot].Data());
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }
        public static void SendPlayerEquipmentTo(Client client, Player en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.PlayerEquipment);
            bf.WriteInteger(en.MyClient.EntityIndex);
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                bf.WriteInteger(en.Equipment[i]);
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }
        public static void SendPlayerEquipmentToProximity(Player en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.PlayerEquipment);
            bf.WriteInteger(en.MyClient.EntityIndex);
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                bf.WriteInteger(en.Equipment[i]);
            }
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }
        public static void SendPointsTo(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.StatPoints);
            bf.WriteInteger(client.Entity.StatPoints);
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendHotbarSlots(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.HotbarSlots);
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                bf.WriteInteger(client.Entity.Hotbar[i].Type);
                bf.WriteInteger(client.Entity.Hotbar[i].Slot);
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendResource(Client client, int resourceNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ResourceData);
            bf.WriteInteger(resourceNum);
            bf.WriteBytes(Globals.GameResources[resourceNum].ResourceData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendResourceEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenResourceEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendClass(Client client, int classNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ClassData);
            bf.WriteInteger(classNum);
            bf.WriteBytes(Globals.GameClasses[classNum].ClassData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendClassEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenClassEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCreateCharacter(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.CreateCharacter);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendQuest(Client client, int questNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.QuestData);
            bf.WriteInteger(questNum);
            bf.WriteBytes(Globals.GameQuests[questNum].QuestData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendQuestEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenQuestEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenAdminWindow(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenAdminWindow);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapGrid(Client client, int gridIndex)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.MapGrid);
            bf.WriteLong(Database.MapGrids[gridIndex].Width);
            bf.WriteLong(Database.MapGrids[gridIndex].Height);
            for (int x = 0; x < Database.MapGrids[gridIndex].Width; x++)
            {
                for (int y = 0; y < Database.MapGrids[gridIndex].Height; y++)
                {
                    bf.WriteInteger(Database.MapGrids[gridIndex].MyGrid[x,y]);
                    if (Database.MapGrids[gridIndex].MyGrid[x, y] != -1)
                    {
                        bf.WriteString(Globals.GameMaps[Database.MapGrids[gridIndex].MyGrid[x, y]].MyName);
                        bf.WriteInteger(Globals.GameMaps[Database.MapGrids[gridIndex].MyGrid[x, y]].Revision);
                    }
                }
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendProjectile(Client client, int projectileNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ProjectileData);
            bf.WriteInteger(projectileNum);
            bf.WriteBytes(Globals.GameProjectiles[projectileNum].ProjectileData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendProjectileEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenProjectileEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityCastTime(int EntityIndex, int SpellNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.CastTime);
            bf.WriteLong(EntityIndex);
            bf.WriteInteger(SpellNum);
            SendDataToProximity(Globals.Entities[EntityIndex].CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendSpellCooldown(Client client, int SpellSlot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.SendSpellCooldown);
            bf.WriteLong(SpellSlot);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendExperience(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.Experience);
            bf.WriteInteger(client.Entity.Experience);
            bf.WriteInteger(client.Entity.GetExperienceToNextLevel());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendAlert(Client client, string title, string message)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.SendAlert);
            bf.WriteString(title);
            bf.WriteString(message);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendAnimationToProximity(int animNum, int targetType, int entityIndex, int map, int x, int y, int direction)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.SendPlayAnimation);
            bf.WriteInteger(animNum);
            bf.WriteInteger(targetType);
            bf.WriteInteger(entityIndex);
            bf.WriteInteger(map);
            bf.WriteInteger(x);
            bf.WriteInteger(y);
            bf.WriteInteger(direction);
            SendDataToProximity(map, bf.ToArray());
            bf.Dispose();
        }

        public static void SendHoldPlayer(Client client, int eventMap, int eventIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.HoldPlayer);
            bf.WriteInteger(eventMap);
            bf.WriteInteger(eventIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendReleasePlayer(Client client, int eventMap, int eventIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ReleasePlayer);
            bf.WriteInteger(eventMap);
            bf.WriteInteger(eventIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendPlayMusic(Client client, string bgm)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.PlayMusic);
            bf.WriteString(bgm);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendFadeMusic(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.FadeMusic);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendPlaySound(Client client, string sound)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.PlaySound);
            bf.WriteString(sound);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendStopSounds(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.StopSounds);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenCommonEventEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenCommonEventEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCommonEvent(Client client, int index)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.CommonEventData);
            bf.WriteInteger(index);
            bf.WriteBytes(Globals.CommonEvents[index].EventData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenSwitchVariableEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenSwitchVariableEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendSwitchesAndVariablesTo(Client client)
        {
            //Send Player Switches
            for (int i = 0; i < Options.MaxPlayerSwitches; i++)
            {
                SendSwitchVariable(client, (int) SwitchVariableTypes.PlayerSwitch, i);
            }
            //Send Player Variables
            for (int i = 0; i < Options.MaxPlayerVariables; i++)
            {
                SendSwitchVariable(client, (int)SwitchVariableTypes.PlayerVariable, i);
            }
            //Send Server Switches
            for (int i = 0; i < Options.MaxServerSwitches; i++)
            {
                SendSwitchVariable(client, (int)SwitchVariableTypes.ServerSwitch, i);
            }
            //Send Server Variables
            for (int i = 0; i < Options.MaxServerVariables; i++)
            {
                SendSwitchVariable(client, (int)SwitchVariableTypes.ServerVariable, i);
            }
        }

        public static void SendSwitchVariable(Client client, int type, int index)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.SwitchVariableData);
            bf.WriteInteger(type);
            bf.WriteInteger(index);
            switch (type)
            {
                case (int)SwitchVariableTypes.PlayerSwitch:
                    bf.WriteString(Globals.PlayerSwitches[index]);
                    break;
                case (int)SwitchVariableTypes.PlayerVariable:
                    bf.WriteString(Globals.PlayerVariables[index]);
                    break;
                case (int)SwitchVariableTypes.ServerSwitch:
                    bf.WriteString(Globals.ServerSwitches[index]);
                    bf.WriteByte(Convert.ToByte(Globals.ServerSwitchValues[index]));
                    break;
                case (int)SwitchVariableTypes.ServerVariable:
                    bf.WriteString(Globals.ServerVariables[index]);
                    bf.WriteInteger(Globals.ServerVariableValues[index]);
                    break;
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenShopEditor(Client client)
        {
            for (int i = 0; i < Options.MaxShops; i++)
            {
                SendShop(client, i);
            }
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenShopEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendShop(Client client, int index)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ShopData);
            bf.WriteInteger(index);
            bf.WriteBytes(Globals.GameShops[index].ShopData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenShop(Client client, int shopNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenShop);
            bf.WriteBytes(Globals.GameShops[shopNum].ShopData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCloseShop(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.CloseShop);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenBank(Client client)
        {
            for (int i = 0; i < Options.MaxBankSlots; i++)
            {
                SendBankUpdate(client, i);
            }
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenBank);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCloseBank(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.CloseBank);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendBankUpdate(Client client, int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.BankUpdate);
            bf.WriteInteger(slot);
            if (client.Entity.Bank[slot] == null || client.Entity.Bank[slot].ItemNum < 0 ||
                client.Entity.Bank[slot].ItemVal <= 0)
            {
                bf.WriteInteger(0);
            }
            else
            {
                bf.WriteInteger(1);
                bf.WriteBytes(client.Entity.Bank[slot].Data());
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }
    }
}

