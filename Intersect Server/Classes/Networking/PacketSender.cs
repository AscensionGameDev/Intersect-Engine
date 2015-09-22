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

namespace Intersect_Server.Classes
{
    public static class PacketSender
    {

        private static void SendDataToMap(int mapNum, byte[] data)
        {
            if (mapNum < 0 || mapNum >= Globals.GameMaps.Length || Globals.GameMaps[mapNum] == null) { return; }
            List<int> Players = Globals.GameMaps[mapNum].GetPlayersOnMap();
            for (int i = 0; i < Players.Count; i++)
            {
                Globals.Clients[Players[i]].SendPacket(data);
            }
        }
        private static void SendDataToProximity(int mapNum, byte[] data)
        {
            if (mapNum < 0 || mapNum >= Globals.GameMaps.Length || Globals.GameMaps[mapNum] == null) { return; }
            SendDataToMap(mapNum, data);
            for (int i = 0; i < Globals.GameMaps[mapNum].SurroundingMaps.Count; i++)
            {
                SendDataToMap(Globals.GameMaps[mapNum].SurroundingMaps[i], data);
            }
        }
        private static void SendDataToEditors(byte[] data)
        {
            for (var i = 0; i < Globals.Clients.Count; i++)
            {
                if (Globals.Clients[i] == null) continue;
                if (!Globals.Clients[i].isConnected) continue;
                if (Globals.Clients[i].IsEditor)
                {
                    Globals.Clients[i].SendPacket(data);
                }
            }
        }

        public static void SendPing(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.RequestPing);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
            client.ConnectionTimeout = Environment.TickCount + (client.TimeoutLength * 1000);
        }

        public static void SendJoinGame(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.JoinGame);
            bf.WriteLong(client.EntityIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMap(Client client, int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.MapData);
            bf.WriteLong(mapNum);
            if (client.IsEditor)
            {
                bf.WriteLong(Globals.GameMaps[mapNum].MapData.Length);
                bf.WriteBytes(Globals.GameMaps[mapNum].MapData);
            }
            else
            {
                bf.WriteLong(Globals.GameMaps[mapNum].MapGameData.Length);
                bf.WriteBytes(Globals.GameMaps[mapNum].MapGameData);
            }
            client.SendPacket(bf.ToArray());
            if (!client.IsEditor)
            {
                //Send Extra Map Info
                PacketSender.SendMapItems(client, mapNum);
            }
            bf.Dispose();
        }

        public static void SendMapToEditors(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.MapData);
            bf.WriteLong(mapNum);
            bf.WriteLong(Globals.GameMaps[mapNum].MapData.Length);
            bf.WriteBytes(Globals.GameMaps[mapNum].MapData);
            SendDataToEditors(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityData(Client client, int sendIndex, int entityType, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityData);
            bf.WriteLong(sendIndex);
            bf.WriteInteger(entityType);
            if (entityType == 1)
            {
                bf.WriteBytes(((Event)en).Data());
            }
            else if (entityType == 3) //Resources
            {
                bf.WriteBytes(((Resource)en).Data());
            }
            else
            {
                bf.WriteBytes(en.Data());
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
            SendEntityVitalsTo(client, sendIndex,entityType,en);
            SendEntityStatsTo(client, sendIndex,entityType,en);
            SendEntityPositionTo(client, sendIndex, entityType, en);

            if (en == client.Entity)
            {
                PacketSender.SendInventory(client);
                PacketSender.SendPlayerSpells(client);
                PacketSender.SendPlayerEquipmentToProximity(client.Entity);
                PacketSender.SendPointsTo(client);
                PacketSender.SendHotbarSlots(client);
            }
        }

        public static void SendEntityDataToProximity(int sendIndex, int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityData);
            bf.WriteLong(sendIndex);
            bf.WriteInteger(isEvent);
            if (isEvent == 1)
            {
                bf.WriteBytes(((Event)en).Data());
            }
            else if (isEvent == 3) //Resources
            {
                bf.WriteBytes(((Resource)en).Data());
            }
            else
            {
                bf.WriteBytes(en.Data());
            }
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
            SendEntityVitals(sendIndex,isEvent,en);
            SendEntityStats(sendIndex,isEvent,en);
        }

        public static void SendEntityPositionTo(Client client, int sendIndex, int isEvent, Entity en)
        {
            if (en == null) { return; }
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityPosition);
            bf.WriteLong(sendIndex);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(en.Passable);
            bf.WriteInteger(en.HideName);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityPositionToAll(int sendIndex, int isEvent, Entity en)
        {
            if (en == null) { return; }
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityPosition);
            bf.WriteLong(sendIndex);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(en.Passable);
            bf.WriteInteger(en.HideName);
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityLeave(int index, int isEvent, int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityLeave);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            SendDataToProximity(mapNum, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityLeaveTo(Client client,int index, int isEvent)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityLeave);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendDataToAll(byte[] packet)
        {
            foreach (var t in Globals.Clients)
            {
                if (t == null) continue;
                if (t.isConnected && ((Player)Globals.Entities[t.EntityIndex]).InGame)
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
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.ChatMessage);
            bf.WriteString(message);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendGameData(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.GameData);
            bf.WriteLong(Globals.MapCount); //Map Count
            if (client.IsEditor)
            {
                for (var i = 0; i < Globals.MapCount; i++)
                {
                    bf.WriteString(Globals.GameMaps[i].MyName);
                }
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();

            //Send massive amounts of game data
            for (int i = 0; i < Constants.MaxItems; i++)
            {
                SendItem(client, i);
            }
            for (int i = 0; i < Constants.MaxAnimations; i++)
            {
                SendAnimation(client, i);
            }
            for (int i = 0; i < Constants.MaxNpcs; i++)
            {
                SendNpc(client, i);
            }
            for (int i = 0; i < Constants.MaxSpells; i++)
            {
                SendSpell(client, i);
            }
            for (int i = 0; i < Constants.MaxResources; i++)
            {
                SendResource(client, i);
            }
            for (int i = 0; i < Constants.MaxClasses; i++)
            {
                SendClass(client, i);
            }
        }

        public static void SendGlobalMsg(string message)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.ChatMessage);
            bf.WriteString(message);
            SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendProximityMsg(string message, int centerMap)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.ChatMessage);
            bf.WriteString(message);
            SendDataToProximity(centerMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendTilesets(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.TilesetArray);
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
            bf.WriteLong((int)Enums.ServerPackets.EnterMap);
            bf.WriteLong(mapNum);
            if (!(Globals.GameMaps[mapNum].MapGridX == -1 || Globals.GameMaps[mapNum].MapGridY == -1))
            {
                Globals.GameMaps[mapNum].PlayerEnteredMap();
                for (var y = Globals.GameMaps[mapNum].MapGridY - 1; y < Globals.GameMaps[mapNum].MapGridY + 2; y++)
                {
                    for (var x = Globals.GameMaps[mapNum].MapGridX - 1; x < Globals.GameMaps[mapNum].MapGridX + 2; x++)
                    {
                        bf.WriteLong(Database.MapGrids[Globals.GameMaps[mapNum].MapGrid].MyGrid[x, y]);
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
                if (!Globals.Clients[i].isConnected || Globals.Clients[i].EntityIndex <= -1) continue;
                if (Globals.Entities[Globals.Clients[i].EntityIndex] == null) continue;
                if (((Player)Globals.Entities[Globals.Clients[i].EntityIndex]).InGame)
                {
                    Globals.Clients[i].SendPacket(packet);
                }
            }
        }

        public static void SendEntityMove(int index, int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityMove);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityMoveTo(Client client,int index, int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityMove);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityVitals(int index,int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityVitals);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            for (var i = 0; i < (int) Enums.Vitals.VitalCount; i++)
            {
                bf.WriteInteger(en.MaxVital[i]);
                bf.WriteInteger(en.Vital[i]); 
            }
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityStats(int index,int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityStats);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            for (var i = 0; i < (int)Enums.Stats.StatCount; i++)
            {
                bf.WriteInteger(en.Stat[i]);
            }
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityVitalsTo(Client client, int index,int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityVitals);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            for (var i = 0; i < (int)Enums.Vitals.VitalCount; i++)
            {
                bf.WriteInteger(en.MaxVital[i]);
                bf.WriteInteger(en.Vital[i]);
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityStatsTo(Client client, int index,int isEvent, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityStats);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            for (var i = 0; i < (int) Enums.Stats.StatCount; i++)
            {
                bf.WriteInteger(en.Stat[i]);
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityDir(int index, int isEvent)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EntityDir);
            bf.WriteLong(index);
            bf.WriteInteger(isEvent);
            bf.WriteInteger(Globals.Entities[index].Dir);
            SendDataToProximity(Globals.Entities[index].CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEventDialog(Client client, string prompt, int eventIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EventDialog);
            bf.WriteString(prompt);
            bf.WriteInteger(0);
            bf.WriteInteger(eventIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }
        public static void SendEventDialog(Client client, string prompt,string opt1, string opt2, string opt3, string opt4, int eventIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.EventDialog);
            bf.WriteString(prompt);
            bf.WriteInteger(1);
            bf.WriteString(opt1);
            bf.WriteString(opt2);
            bf.WriteString(opt3);
            bf.WriteString(opt4);
            bf.WriteInteger(eventIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapList(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.MapList);
            bf.WriteBytes(Database.MapStructure.Data());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapListToEditors()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.MapList);
            bf.WriteBytes(Database.MapStructure.Data());
            SendDataToEditors(bf.ToArray());
            bf.Dispose();
        }

        public static void SendLoginError(Client client,string error)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.LoginError);
            bf.WriteString(error);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendGameTime(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.GameTime);
            bf.WriteInteger(Globals.GameTime);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendItemList(Client client)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.ItemList);
            for (int i = 0; i < Constants.MaxItems; i++)
            {
                bf.WriteString(Globals.GameItems[i].Name);
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendItem(Client client, int itemNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.ItemData);
            bf.WriteLong(itemNum);
            bf.WriteBytes(Globals.GameItems[itemNum].ItemData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendItemEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.OpenItemEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNpcList(Client client)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.NpcList);
            for (int i = 0; i < Constants.MaxNpcs; i++)
            {
                bf.WriteString(Globals.GameNpcs[i].Name);
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNpc(Client client, int npcNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.NpcData);
            bf.WriteInteger(npcNum);
            bf.WriteBytes(Globals.GameNpcs[npcNum].NpcData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendNpcEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.OpenNpcEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendSpellList(Client client)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.SpellList);
            for (int i = 0; i < Constants.MaxSpells; i++)
            {
                bf.WriteString(Globals.GameSpells[i].Name);
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendSpell(Client client, int spellNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.SpellData);
            bf.WriteInteger(spellNum);
            bf.WriteBytes(Globals.GameSpells[spellNum].SpellData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendSpellEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.OpenSpellEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendAnimationsList(Client client)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.AnimationList);
            for (int i = 0; i < Constants.MaxAnimations; i++)
            {
                bf.WriteString(Globals.GameAnimations[i].Name);
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendAnimation(Client client, int animNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.AnimationData);
            bf.WriteInteger(animNum);
            bf.WriteBytes(Globals.GameAnimations[animNum].AnimData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendAnimationEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.OpenAnimationEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapItems(Client client, int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.MapItems);
            bf.WriteInteger(mapNum);
            bf.WriteInteger(Globals.GameMaps[mapNum].MapItems.Count);
            for (int i = 0; i < Globals.GameMaps[mapNum].MapItems.Count; i++)
            {
                bf.WriteBytes(Globals.GameMaps[mapNum].MapItems[i].Data());
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapItemUpdate(int mapNum, int index)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.MapItemUpdate);
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
            for (int i = 0; i < Constants.MaxInvItems; i++) {
                SendInventoryItemUpdate(client,i);
            }
        }
        public static void SendInventoryItemUpdate(Client client, int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.InventoryUpdate);
            bf.WriteInteger(slot);
            bf.WriteBytes(client.Entity.Inventory[slot].Data());
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }
        public static void SendPlayerSpells(Client client)
        {
            for (int i = 0; i < Constants.MaxPlayerSkills; i++)
            {
                SendPlayerSpellUpdate(client, i);
            }
        }
        public static void SendPlayerSpellUpdate(Client client, int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.SpellUpdate);
            bf.WriteInteger(slot);
            bf.WriteBytes(client.Entity.Spells[slot].Data());
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }
        public static void SendPlayerEquipmentTo(Client client, Player en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.PlayerEquipment);
            bf.WriteInteger(en.MyClient.EntityIndex);
            for (int i = 0; i < Enums.EquipmentSlots.Count; i++ )
            {
                bf.WriteInteger(en.Equipment[i]);
            }
            SendDataTo(client,bf.ToArray());
            bf.Dispose();
        }
        public static void SendPlayerEquipmentToProximity(Player en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.PlayerEquipment);
            bf.WriteInteger(en.MyClient.EntityIndex);
            for (int i = 0; i < Enums.EquipmentSlots.Count; i++ )
            {
                bf.WriteInteger(en.Equipment[i]);
            }
            SendDataToProximity(en.CurrentMap,bf.ToArray());
            bf.Dispose();
        }
        public static void SendPointsTo(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.StatPoints);
            bf.WriteInteger(client.Entity.StatPoints);
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendHotbarSlots(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.HotbarSlots);
            for (int i = 0; i < Constants.MaxHotbar; i++)
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
            bf.WriteLong((int)Enums.ServerPackets.ResourceData);
            bf.WriteInteger(resourceNum);
            bf.WriteBytes(Globals.GameResources[resourceNum].ResourceData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendResourceEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.OpenResourceEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendClass(Client client, int classNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.ClassData);
            bf.WriteInteger(classNum);
            bf.WriteBytes(Globals.GameClasses[classNum].ClassData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendClassEditor(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.OpenClassEditor);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCreateCharacter(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)Enums.ServerPackets.CreateCharacter);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }
    }
}

