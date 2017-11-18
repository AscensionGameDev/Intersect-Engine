using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Collections;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Localization;
using Intersect.Logging;
using Intersect.Models;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Items;
using Intersect.Server.Classes.Maps;

namespace Intersect.Server.Classes.Networking
{
    public static class PacketSender
    {
        public static void SendDataToMap(int mapNum, byte[] data, Client except = null)
        {
            if (!MapInstance.Lookup.IndexKeys.Contains(mapNum))
            {
                return;
            }
            List<Player> Players = MapInstance.Lookup.Get<MapInstance>(mapNum).GetPlayersOnMap();
            foreach (var player in Players)
            {
                if (player != null && player.MyClient != except) player.MyClient.SendPacket(data);
            }
        }

        public static void SendDataToProximity(int mapNum, byte[] data, Client except = null)
        {
            if (!MapInstance.Lookup.IndexKeys.Contains(mapNum))
            {
                return;
            }
            SendDataToMap(mapNum, data, except);
            for (int i = 0; i < MapInstance.Lookup.Get<MapInstance>(mapNum).SurroundingMaps.Count; i++)
            {
                SendDataToMap(MapInstance.Lookup.Get<MapInstance>(mapNum).SurroundingMaps[i], data, except);
            }
        }

        public static void SendDataToEditors(byte[] data)
        {
            lock (Globals.ClientLock)
            {
                foreach (var client in Globals.Clients)
                {
                    if (client.IsEditor) client.SendPacket(data);
                }
            }
        }

        public static void SendPing(Client client, bool request = true)
        {
            if (client != null)
            {
                var bf = new ByteBuffer();
                bf.WriteLong((int) ServerPackets.Ping);
                bf.WriteInteger(Convert.ToInt32(request));
                client.SendPacket(bf.ToArray());
                bf.Dispose();
            }
        }

        public static void SendServerConfig(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.ServerConfig);
            bf.WriteBytes(Options.GetOptionsData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendJoinGame(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.JoinGame);
            SendEntityDataTo(client, client.Entity);
            client.SendPacket(bf.ToArray());
            bf.Dispose();

            if (!client.IsEditor)
            {
                SendGlobalMsg(Strings.Get("player", "joined", client.Entity.MyName, Options.GameName));
            }
        }

        public static void SendMap(Client client, int mapNum, bool allEditors = false)
        {
            if (client == null)
            {
                Log.Error("Attempted to send packet to null client.");
                return;
            }

            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.MapData);
            bf.WriteLong(mapNum);
            var map = MapInstance.Lookup.Get<MapInstance>(mapNum);
            if (map == null)
            {
                bf.WriteInteger(1);
                if (client.IsEditor)
                {
                    if (allEditors) SendDataToEditors(bf.ToArray());
                }
                else
                {
                    client.SendPacket(bf.ToArray());
                }
            }
            else
            {
                bf.WriteInteger(0);
                byte[] MapData = null;
                if (client.IsEditor)
                {
                    MapData = map.GetMapPacket(false);
                    bf.WriteInteger(MapData.Length);
                    bf.WriteBytes(MapData);
                    var tileData = map.GetTileData(false);
                    bf.WriteInteger(tileData.Length);
                    bf.WriteBytes(tileData);
                    bf.WriteInteger(map.MapGridX);
                    bf.WriteInteger(map.MapGridY);
                }
                else
                {
                    if (client.SentMaps.ContainsKey(mapNum))
                    {
                        if (client.SentMaps[mapNum].Item1 > Globals.System.GetTimeMs() &&
                            client.SentMaps[mapNum].Item2 == map.Revision) return;
                        client.SentMaps.Remove(mapNum);
                    }
                    try
                    {
                        client.SentMaps.Add(mapNum,
                            new Tuple<long, int>(Globals.System.GetTimeMs() + 5000, map.Revision));
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"Current Map #: {mapNum}");
                        Log.Error($"# Sent maps: {client.SentMaps.Count}");
                        Log.Error($"# Maps: {MapInstance.Lookup.Count}");
                        throw exception;
                    }
                    MapData = map.GetMapPacket(true);
                    bf.WriteInteger(MapData.Length);
                    bf.WriteBytes(MapData);
                    var tileData = map.GetTileData();
                    bf.WriteInteger(tileData.Length);
                    bf.WriteBytes(tileData);
                    bf.WriteInteger(map.Revision);
                    bf.WriteInteger(map.MapGridX);
                    bf.WriteInteger(map.MapGridY);
                    if (Options.GameBorderStyle == 1)
                    {
                        bf.WriteInteger(1);
                        bf.WriteInteger(1);
                        bf.WriteInteger(1);
                        bf.WriteInteger(1);
                    }
                    else if (Options.GameBorderStyle == 0)
                    {
                        if (0 == map.MapGridX)
                        {
                            bf.WriteInteger(1);
                        }
                        else
                        {
                            bf.WriteInteger(0);
                        }
                        if (Database.MapGrids[map.MapGrid].XMax - 1 == map.MapGridX)
                        {
                            bf.WriteInteger(1);
                        }
                        else
                        {
                            bf.WriteInteger(0);
                        }
                        if (0 == map.MapGridY)
                        {
                            bf.WriteInteger(1);
                        }
                        else
                        {
                            bf.WriteInteger(0);
                        }
                        if (Database.MapGrids[map.MapGrid].YMax - 1 == map.MapGridY)
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
                client.SendPacket(bf.ToArray());
                if (client.IsEditor)
                {
                    if (allEditors) SendDataToEditors(bf.ToArray());
                }
                else
                {
                    map.SendMapEntitiesTo(client.Entity);
                    SendMapItems(client, mapNum);
                }
            }
            bf.Dispose();
        }

        public static void SendMapToEditors(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.MapData);
            bf.WriteLong(mapNum);
            if (MapInstance.Lookup.Get<MapInstance>(mapNum) == null)
            {
                bf.WriteInteger(1);
            }
            else
            {
                bf.WriteInteger(0);
                byte[] MapData = MapInstance.Lookup.Get<MapInstance>(mapNum).GetMapPacket(false);
                bf.WriteInteger(MapData.Length);
                bf.WriteBytes(MapData);
                var tileData = MapInstance.Lookup.Get<MapInstance>(mapNum).GetTileData(false);
                bf.WriteInteger(tileData.Length);
                bf.WriteBytes(tileData);
            }
            SendDataToEditors(bf.ToArray());
            bf.Dispose();
        }

        private static byte[] GetEntityPacket(Entity en, Client forClient)
        {
            var bf = new ByteBuffer();
            bf.WriteLong(en.SpawnTime);
            bf.WriteLong(en.MyIndex);
            bf.WriteInteger((int) en.GetEntityType());
            bf.WriteBytes(en.Data());

            if (en.GetType() == typeof(Player))
            {
                if (forClient != null && forClient.Entity == en)
                {
                    bf.WriteInteger(1);
                }
                else
                {
                    bf.WriteInteger(0);
                }
            }

            return bf.ToArray();
        }

        public static void SendEntityDataTo(Client client, Entity en)
        {
            if (en == null)
            {
                return;
            }
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityData);
            bf.WriteLong(en.SpawnTime);
            bf.WriteLong(en.MyIndex);
            bf.WriteInteger((int) en.GetEntityType());
            bf.WriteBytes(en.Data());

            if (en == client.Entity)
            {
                bf.WriteInteger(1);
            }

            client.SendPacket(bf.ToArray());
            bf.Dispose();

            if (en == client.Entity)
            {
                SendExperience(client);
                SendInventory(client);
                SendPlayerSpells(client);
                SendPointsTo(client);
                SendHotbarSlots(client);
                SendQuestsProgress(client);
            }

            //If a player, send equipment to all (for paperdolls)
            if (en.GetType() == typeof(Player))
            {
                SendPlayerEquipmentTo(client, (Player) en);
            }
        }

        public static void SendMapEntitiesTo(Client client, List<Entity> entities)
        {
            var buff = new ByteBuffer();
            buff.WriteLong((long) ServerPackets.MapEntities);
            var sendEntities = new List<Entity>();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] != null && entities[i] != client.Entity)
                {
                    sendEntities.Add(entities[i]);
                }
            }
            buff.WriteInteger(sendEntities.Count);
            for (int i = 0; i < sendEntities.Count; i++)
            {
                buff.WriteBytes(GetEntityPacket(sendEntities[i], client));
            }
            client.SendPacket(buff.ToArray());

            SendMapEntityEquipmentTo(client, sendEntities); //Send the equipment of each player
        }

        public static void SendMapEntityEquipmentTo(Client client, List<Entity> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] != null && entities[i] != client.Entity)
                {
                    //If a player, send equipment to all (for paperdolls)
                    if (entities[i].GetType() == typeof(Player) && client.Entity != entities[i])
                    {
                        SendPlayerEquipmentTo(client, (Player) entities[i]);
                    }
                }
            }
        }

        public static void SendEntityDataToProximity(Entity en, Client except = null)
        {
            if (en == null)
            {
                return;
            }
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityData);
            bf.WriteLong(en.SpawnTime);
            bf.WriteLong(en.MyIndex);
            bf.WriteInteger((int) en.GetEntityType());
            bf.WriteBytes(en.Data());
            SendDataToProximity(en.CurrentMap, bf.ToArray(), except);
            bf.Dispose();
            SendEntityVitals(en);
            SendEntityStats(en);

            //If a player, send equipment to all (for paperdolls)
            if (en.GetType() == typeof(Player))
            {
                SendPlayerEquipmentToProximity((Player) en);
            }
        }

        public static void SendEntityPositionTo(Client client, Entity en)
        {
            if (en == null)
            {
                return;
            }
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityPosition);
            bf.WriteLong(en.MyIndex);
            bf.WriteInteger((int) en.GetEntityType());
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(en.Passable);
            bf.WriteInteger(en.HideName);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityPositionToAll(Entity en)
        {
            if (en == null)
            {
                return;
            }
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityPosition);
            bf.WriteLong(en.MyIndex);
            bf.WriteInteger((int) en.GetEntityType());
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(en.Passable);
            bf.WriteInteger(en.HideName);
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendNpcAggressionToProximity(Npc en)
        {
            if (en == null)
            {
                return;
            }
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.NPCAggression);
            bf.WriteInteger(en.MyIndex);

            //Declare Aggression state
            if (en.MyTarget != null)
            {
                bf.WriteInteger(-1);
            }
            else
            {
                bf.WriteInteger(en.MyBase.Behavior);
            }

            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityLeave(int entityIndex, int type, int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityLeave);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(mapNum);
            SendDataToProximity(mapNum, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityLeaveTo(Client client, int entityIndex, int type, int map)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityLeave);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(map);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendDataToAll(byte[] packet)
        {
            lock (Globals.ClientLock)
            {
                foreach (var client in Globals.Clients)
                {
                    if (client != null)
                    {
                        if (client.IsEditor || client.Entity != null)
                        {
                            client.SendPacket(packet);
                        }
                    }
                }
            }
        }

        public static void SendDataTo(Client client, byte[] packet)
        {
            client.SendPacket(packet);
        }

        public static void SendPlayerMsg(Client client, string message, string target = "")
        {
            SendPlayerMsg(client, message, CustomColors.PlayerMsg, target);
        }

        public static void SendPlayerMsg(Client client, string message, Color clr, string target = "")
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.ChatMessage);
            bf.WriteString(message);
            bf.WriteByte(clr.A);
            bf.WriteByte(clr.R);
            bf.WriteByte(clr.G);
            bf.WriteByte(clr.B);
            bf.WriteString(target);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendGameData(Client client)
        {
            //Send massive amounts of game data
            foreach (var val in Enum.GetValues(typeof(GameObjectType)))
            {
                if ((GameObjectType) val == GameObjectType.Map) continue;
                if (((GameObjectType) val == GameObjectType.Shop ||
                     (GameObjectType) val == GameObjectType.CommonEvent ||
                     (GameObjectType) val == GameObjectType.PlayerSwitch ||
                     (GameObjectType) val == GameObjectType.PlayerVariable ||
                     (GameObjectType) val == GameObjectType.ServerSwitch ||
                     (GameObjectType) val == GameObjectType.ServerVariable) && !client.IsEditor) continue;
                SendGameObjects(client, (GameObjectType) val);
            }
            //Let the client/editor know they have everything now
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.GameData);
            bf.WriteBytes(CustomColors.GetData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendGlobalMsg(string message, string target = "")
        {
            SendGlobalMsg(message, CustomColors.GlobalMsg, target);
        }

        public static void SendGlobalMsg(string message, Color clr, string target = "")
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.ChatMessage);
            bf.WriteString(message);
            bf.WriteByte(clr.A);
            bf.WriteByte(clr.R);
            bf.WriteByte(clr.G);
            bf.WriteByte(clr.B);
            bf.WriteString(target);
            SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendProximityMsg(string message, int centerMap, string target = "")
        {
            SendProximityMsg(message, centerMap, CustomColors.ProximityMsg);
        }

        public static void SendProximityMsg(string message, int centerMap, Color clr, string target = "")
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.ChatMessage);
            bf.WriteString(message);
            bf.WriteByte(clr.A);
            bf.WriteByte(clr.R);
            bf.WriteByte(clr.G);
            bf.WriteByte(clr.B);
            bf.WriteString(target);
            SendDataToProximity(centerMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendAdminMsg(string message, Color clr, string target = "")
        {
            foreach (var client in Globals.Clients)
            {
                if (client != null)
                {
                    if (client.IsEditor || client.Entity != null)
                    {
                        if (client.Power > 0)
                        {
                            SendPlayerMsg(client, message, clr, target);
                        }
                    }
                }
            }
        }

        public static void SendPartyMsg(Client client, string message, Color clr, string target = "")
        {
            foreach (var c in Globals.Clients)
            {
                if (c != null)
                {
                    if (c.IsEditor || c.Entity != null)
                    {
                        if (client.Entity.InParty(c.Entity))
                        {
                            SendPlayerMsg(c, message, clr, target);
                        }
                    }
                }
            }
        }

        public static void SendDataToAllBut(Entity en, byte[] packet)
        {
            lock (Globals.ClientLock)
            {
                foreach (var client in Globals.Clients)
                {
                    if (client.Entity != null && client.Entity != en)
                    {
                        client.SendPacket(packet);
                    }
                }
            }
        }

        public static void SendDataToAllBut(Client user, byte[] packet)
        {
            lock (Globals.ClientLock)
            {
                foreach (var client in Globals.Clients)
                {
                    if (client.Entity != null && client != user)
                    {
                        client.SendPacket(packet);
                    }
                }
            }
        }

        internal static void SendRemoveProjectileSpawn(int map, int baseEntityIndex, int spawnIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.ProjectileSpawnDead);
            bf.WriteLong(baseEntityIndex);
            bf.WriteLong(spawnIndex);
            SendDataToProximity(map, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityMove(Entity en, bool correction = false)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityMove);
            bf.WriteLong(en.MyIndex);
            bf.WriteInteger((int) en.GetEntityType());
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(Convert.ToInt32(correction));
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityMoveTo(Client client, Entity en, bool correction = false)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityMove);
            bf.WriteLong(en.MyIndex);
            bf.WriteInteger((int) en.GetEntityType());
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(Convert.ToInt32(correction));
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityVitals(Entity en)
        {
            if (en == null) return;
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityVitals);
            bf.WriteLong(en.MyIndex);
            bf.WriteInteger((int) en.GetEntityType());
            bf.WriteInteger(en.CurrentMap);
            for (var i = 0; i < (int) Vitals.VitalCount; i++)
            {
                bf.WriteInteger(en.MaxVital[i]);
                bf.WriteInteger(en.Vital[i]);
            }
            var statuses = en.Statuses.Values.ToArray();
            bf.WriteInteger(statuses.Length);
            foreach (var status in statuses)
            {
                bf.WriteInteger(status._spell.Index);
                bf.WriteInteger(status.Type);
                bf.WriteString(status.Data);
                bf.WriteInteger((int) (status.Duration - Globals.System.GetTimeMs()));
                bf.WriteInteger((int) (status.Duration - status.StartTime));
            }
            //If player and in party send vitals to party just in case party members are not in the proximity
            if (en.GetType() == typeof(Player))
            {
                for (var i = 0; i < ((Player) en).Party.Count; i++)
                {
                    SendEntityVitalsTo(((Player) en).Party[i].MyClient, en);
                }
            }
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityStats(Entity en)
        {
            if (en == null) return;
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityStats);
            bf.WriteLong(en.MyIndex);
            bf.WriteInteger((int) en.GetEntityType());
            bf.WriteInteger(en.CurrentMap);
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                bf.WriteInteger(en.Stat[i].Value());
            }
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityVitalsTo(Client client, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityVitals);
            bf.WriteLong(en.MyIndex);
            bf.WriteInteger((int) en.GetEntityType());
            bf.WriteInteger(en.CurrentMap);
            for (var i = 0; i < (int) Vitals.VitalCount; i++)
            {
                bf.WriteInteger(en.MaxVital[i]);
                bf.WriteInteger(en.Vital[i]);
            }
            var statuses = en.Statuses.Values.ToArray();
            bf.WriteInteger(statuses.Length);
            foreach (var status in statuses)
            {
                bf.WriteInteger(status._spell.Index);
                bf.WriteInteger(status.Type);
                bf.WriteString(status.Data);
                bf.WriteInteger((int)(status.Duration - Globals.System.GetTimeMs()));
                bf.WriteInteger((int)(status.Duration - status.StartTime));
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityStatsTo(Client client, Entity en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityStats);
            bf.WriteLong(en.MyIndex);
            bf.WriteInteger((int) en.GetEntityType());
            bf.WriteInteger(en.CurrentMap);
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                bf.WriteInteger(en.Stat[i].Value());
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityDir(int entityIndex, int type, int dir, int map)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityDir);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(map);
            bf.WriteInteger(dir);
            SendDataToProximity(map, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityAttack(Entity en, int type, int map, int attackTime)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityAttack);
            bf.WriteLong(en.MyIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(map);
            bf.WriteInteger(attackTime);
            SendDataToProximity(map, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityDirTo(Client client, int entityIndex, int type, int dir, int map)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityDir);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(map);
            bf.WriteInteger(dir);
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEventDialog(Client client, string prompt, string face, int mapNum, int eventIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EventDialog);
            bf.WriteString(prompt);
            bf.WriteString(face);
            bf.WriteInteger(0);
            bf.WriteInteger(mapNum);
            bf.WriteInteger(eventIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEventDialog(Client client, string prompt, string opt1, string opt2, string opt3,
            string opt4, string face, int mapNum, int eventIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EventDialog);
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
            DatabaseObjectLookup gameMaps = new DatabaseObjectLookup();
            foreach (var pair in MapInstance.Lookup.IndexClone) gameMaps.Set(pair.Key, pair.Value);
            bf.WriteLong((int) ServerPackets.MapList);
            bf.WriteBytes(MapList.GetList().Data(gameMaps));
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapListToAll()
        {
            var bf = new ByteBuffer();
            DatabaseObjectLookup gameMaps = new DatabaseObjectLookup();
            foreach (var pair in MapInstance.Lookup.IndexClone) gameMaps.Set(pair.Key, pair.Value);
            bf.WriteLong((int) ServerPackets.MapList);
            bf.WriteBytes(MapList.GetList().Data(gameMaps));
            SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendLoginError(Client client, string error, string header = "")
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.LoginError);
            bf.WriteString(error);
            bf.WriteString(header);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapItems(Client client, int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.MapItems);
            bf.WriteInteger(mapNum);
            bf.WriteInteger(MapInstance.Lookup.Get<MapInstance>(mapNum).MapItems.Count);
            for (int i = 0; i < MapInstance.Lookup.Get<MapInstance>(mapNum).MapItems.Count; i++)
            {
                if (MapInstance.Lookup.Get<MapInstance>(mapNum).MapItems[i] != null)
                {
                    bf.WriteInteger(i);
                    bf.WriteBytes(MapInstance.Lookup.Get<MapInstance>(mapNum).MapItems[i].Data());
                }
                else
                {
                    bf.WriteInteger(-1);
                }
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapItemsToProximity(int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.MapItems);
            bf.WriteInteger(mapNum);
            bf.WriteInteger(MapInstance.Lookup.Get<MapInstance>(mapNum).MapItems.Count);
            for (int i = 0; i < MapInstance.Lookup.Get<MapInstance>(mapNum).MapItems.Count; i++)
            {
                bf.WriteBytes(MapInstance.Lookup.Get<MapInstance>(mapNum).MapItems[i].Data());
            }
            SendDataToProximity(mapNum, bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapItemUpdate(int mapNum, int index)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.MapItemUpdate);
            bf.WriteInteger(mapNum);
            bf.WriteInteger(index);
            if (MapInstance.Lookup.Get<MapInstance>(mapNum).MapItems[index] == null ||
                MapInstance.Lookup.Get<MapInstance>(mapNum).MapItems[index].ItemNum == -1)
            {
                bf.WriteInteger(-1);
            }
            else
            {
                bf.WriteInteger(1);
                bf.WriteBytes(MapInstance.Lookup.Get<MapInstance>(mapNum).MapItems[index].Data());
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
            bf.WriteLong((int) ServerPackets.InventoryUpdate);
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
            bf.WriteLong((int) ServerPackets.SpellUpdate);
            bf.WriteInteger(slot);
            bf.WriteBytes(client.Entity.Spells[slot].Data());
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendPlayerEquipmentTo(Client client, Player en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.PlayerEquipment);
            bf.WriteInteger(en.MyClient.EntityIndex);
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (client.Entity == en)
                {
                    bf.WriteInteger(en.Equipment[i]);
                }
                else
                {
                    if (en.Equipment[i] == -1 || en.Inventory[en.Equipment[i]].ItemNum == -1)
                    {
                        bf.WriteInteger(-1);
                    }
                    else
                    {
                        bf.WriteInteger(en.Inventory[en.Equipment[i]].ItemNum);
                    }
                }
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendPlayerEquipmentToProximity(Player en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.PlayerEquipment);
            bf.WriteInteger(en.MyClient.EntityIndex);
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (en.Equipment[i] == -1 || en.Inventory[en.Equipment[i]].ItemNum == -1)
                {
                    bf.WriteInteger(-1);
                }
                else
                {
                    bf.WriteInteger(en.Inventory[en.Equipment[i]].ItemNum);
                }
            }
            SendDataToProximity(en.CurrentMap, bf.ToArray(), en.MyClient);
            SendPlayerEquipmentTo(en.MyClient, en);
            bf.Dispose();
        }

        public static void SendPointsTo(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.StatPoints);
            bf.WriteInteger(client.Entity.StatPoints);
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendHotbarSlots(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.HotbarSlots);
            for (int i = 0; i < Options.MaxHotbar; i++)
            {
                bf.WriteInteger(client.Entity.Hotbar[i].Type);
                bf.WriteInteger(client.Entity.Hotbar[i].Slot);
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendCreateCharacter(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.CreateCharacter);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendPlayerCharacters(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.PlayerCharacters);
            if (client.Characters.Count < Options.MaxCharacters)
            {
                bf.WriteInteger(client.Characters.Count + 1);
            }
            else
            {
                bf.WriteInteger(client.Characters.Count);
            }
            foreach (var character in client.Characters)
            {
                bf.WriteInteger(character.Slot);
                bf.WriteString(character.Name);
                bf.WriteString(character.Sprite);
                bf.WriteString(character.Face);
                bf.WriteInteger(character.Level);
                bf.WriteString(ClassBase.GetName(character.Class));

                for (int n = 0; n < Options.EquipmentSlots.Count; n++)
                {
                    bf.WriteString(character.Equipment[n]);
                }
            }
            if (client.Characters.Count < Options.MaxCharacters)
            {
                bf.WriteInteger(-1);
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenAdminWindow(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.OpenAdminWindow);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapGridToAll(int gridIndex)
        {
            for (int i = 0; i < Globals.Clients.Count; i++)
            {
                if (Globals.Clients[i] != null)
                {
                    if (Globals.Clients[i].IsEditor)
                    {
                        if (Database.MapGrids[gridIndex].HasMap(Globals.Clients[i].EditorMap))
                        {
                            SendMapGrid(Globals.Clients[i], gridIndex);
                        }
                    }
                    else
                    {
                        if (Globals.Clients[i].Entity != null)
                        {
                            if (Database.MapGrids[gridIndex].HasMap(Globals.Clients[i].Entity.CurrentMap))
                            {
                                SendMapGrid(Globals.Clients[i], gridIndex, true);
                            }
                        }
                    }
                }
            }
        }

        public static void SendMapGrid(Client client, int gridIndex, bool clearKnownMaps = false)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.MapGrid);
            bf.WriteLong(Database.MapGrids[gridIndex].Width);
            bf.WriteLong(Database.MapGrids[gridIndex].Height);
            bf.WriteBoolean(clearKnownMaps);
            if (clearKnownMaps) client.SentMaps.Clear();
            for (int x = 0; x < Database.MapGrids[gridIndex].Width; x++)
            {
                for (int y = 0; y < Database.MapGrids[gridIndex].Height; y++)
                {
                    if (MapInstance.Lookup.Get<MapInstance>(Database.MapGrids[gridIndex].MyGrid[x, y]) != null)
                    {
                        bf.WriteInteger(Database.MapGrids[gridIndex].MyGrid[x, y]);
                        if (client.IsEditor)
                        {
                            bf.WriteString(MapInstance.Lookup
                                .Get<MapInstance>(Database.MapGrids[gridIndex].MyGrid[x, y]).Name);
                            bf.WriteInteger(MapInstance.Lookup
                                .Get<MapInstance>(Database.MapGrids[gridIndex].MyGrid[x, y]).Revision);
                        }
                    }
                    else
                    {
                        bf.WriteInteger(-1);
                    }
                }
            }
            client.SendPacket(bf.ToArray());
            if (!client.IsEditor && clearKnownMaps) SendMap(client, client.Entity.CurrentMap);
            bf.Dispose();
        }

        public static void SendEntityCastTime(Entity en, int SpellNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.CastTime);
            bf.WriteInteger(en.MyIndex);
            bf.WriteInteger(SpellNum);
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendSpellCooldown(Client client, int SpellSlot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.SendSpellCooldown);
            bf.WriteLong(SpellSlot);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendExperience(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.Experience);
            bf.WriteLong(client.Entity.Experience);
            bf.WriteLong(client.Entity.GetExperienceToNextLevel());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendAlert(Client client, string title, string message)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.SendAlert);
            bf.WriteString(title);
            bf.WriteString(message);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendAnimationToProximity(int animNum, int targetType, int entityIndex, int map, int x, int y,
            int direction)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.SendPlayAnimation);
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
            bf.WriteLong((int) ServerPackets.HoldPlayer);
            bf.WriteInteger(eventMap);
            bf.WriteInteger(eventIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendReleasePlayer(Client client, int eventMap, int eventIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.ReleasePlayer);
            bf.WriteInteger(eventMap);
            bf.WriteInteger(eventIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendPlayMusic(Client client, string bgm)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.PlayMusic);
            bf.WriteString(bgm);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendFadeMusic(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.FadeMusic);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendPlaySound(Client client, string sound)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.PlaySound);
            bf.WriteString(sound);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendStopSounds(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.StopSounds);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenShop(Client client, int shopNum)
        {
            if (ShopBase.Lookup.Get<ShopBase>(shopNum) == null) return;
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.OpenShop);
            bf.WriteBytes(ShopBase.Lookup.Get<ShopBase>(shopNum).ShopData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCloseShop(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.CloseShop);
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
            bf.WriteLong((int) ServerPackets.OpenBank);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCloseBank(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.CloseBank);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenCraftingBench(Client client, int benchNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.OpenCraftingBench);
            bf.WriteBytes(BenchBase.Lookup.Get<BenchBase>(benchNum).CraftData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCloseCraftingBench(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.CloseCraftingBench);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendBankUpdate(Client client, int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.BankUpdate);
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

        public static void SendGameObjects(Client client, GameObjectType type)
        {
            switch (type)
            {
                case GameObjectType.Animation:
                    foreach (var obj in AnimationBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Class:
                    foreach (var obj in ClassBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Item:
                    foreach (var obj in ItemBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Npc:
                    foreach (var obj in NpcBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Projectile:
                    foreach (var obj in ProjectileBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Quest:
                    foreach (var obj in QuestBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Resource:
                    foreach (var obj in ResourceBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Shop:
                    foreach (var obj in ShopBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Spell:
                    foreach (var obj in SpellBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Bench:
                    foreach (var obj in BenchBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Map:
                    throw new Exception("Maps are not sent as batches, use the proper send map functions");
                case GameObjectType.CommonEvent:
                    foreach (var obj in EventBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.PlayerSwitch:
                    foreach (var obj in PlayerSwitchBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.PlayerVariable:
                    foreach (var obj in PlayerVariableBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.ServerSwitch:
                    foreach (var obj in ServerSwitchBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.ServerVariable:
                    foreach (var obj in ServerVariableBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Tileset:
                    foreach (var obj in TilesetBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Time:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static void SendGameObject(Client client, IDatabaseObject obj, bool deleted = false,
            bool another = false)
        {
            if (client == null) return;
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.GameObject);
            bf.WriteInteger((int) obj.Type);
            bf.WriteInteger(obj.Index);
            bf.WriteInteger(Convert.ToInt32(another));
            bf.WriteInteger(Convert.ToInt32(deleted));
            if (!deleted) bf.WriteBytes(obj.BinaryData);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendGameObjectToAll(IDatabaseObject obj, bool deleted = false, bool another = false)
        {
            foreach (var client in Globals.Clients)
                SendGameObject(client, obj, deleted, another);
        }

        public static void SendOpenEditor(Client client, GameObjectType type)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.GameObjectEditor);
            bf.WriteInteger((int) type);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityDash(Entity en, int endMap, int endX, int endY, int dashTime, int direction)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EntityDash);
            bf.WriteLong(en.MyIndex);
            bf.WriteInteger(endMap);
            bf.WriteInteger(endX);
            bf.WriteInteger(endY);
            bf.WriteInteger(dashTime);
            bf.WriteInteger(direction);
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendActionMsg(Entity en, string message, Color color)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.ActionMsg);
            bf.WriteInteger(en.CurrentMap);
            bf.WriteInteger(en.CurrentX);
            bf.WriteInteger(en.CurrentY);
            bf.WriteString(message);
            bf.WriteByte(color.A);
            bf.WriteByte(color.R);
            bf.WriteByte(color.G);
            bf.WriteByte(color.B);
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEnterMap(Client client, int mapNum)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.EnterMap);
            bf.WriteLong(mapNum);
            var map = MapInstance.Lookup.Get<MapInstance>(mapNum);
            if (!(map.MapGridX == -1 || map.MapGridY == -1))
            {
                for (var y = map.MapGridY - 1; y < map.MapGridY + 2; y++)
                {
                    for (var x = map.MapGridX - 1;
                        x < map.MapGridX + 2;
                        x++)
                    {
                        if (x >= Database.MapGrids[map.MapGrid].XMin &&
                            x < Database.MapGrids[map.MapGrid].XMax &&
                            y >= Database.MapGrids[map.MapGrid].YMin &&
                            y < Database.MapGrids[map.MapGrid].YMax)
                        {
                            bf.WriteLong(Database.MapGrids[map.MapGrid].MyGrid[x, y]);
                        }
                        else
                        {
                            bf.WriteLong(-1);
                        }
                    }
                }
                client.SendPacket(bf.ToArray());
            }
            bf.Dispose();
        }

        public static void SendTimeBaseTo(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long) ServerPackets.TimeBase);
            bf.WriteBytes(TimeBase.GetTimeBase().SaveTimeBase());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendTimeBaseToAllEditors()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long) ServerPackets.TimeBase);
            bf.WriteBytes(TimeBase.GetTimeBase().SaveTimeBase());
            SendDataToEditors(bf.ToArray());
            bf.Dispose();
        }

        public static void SendTimeToAll()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long) ServerPackets.Time);
            bf.WriteLong(ServerTime.GetTime().ToBinary());
            if (TimeBase.GetTimeBase().SyncTime)
            {
                bf.WriteDouble(1);
            }
            else
            {
                bf.WriteDouble((double) TimeBase.GetTimeBase().Rate);
            }
            //Write the color tint the clients should be using when outdoors
            var clr = ServerTime.GetTimeColor();
            bf.WriteByte(clr.A);
            bf.WriteByte(clr.R);
            bf.WriteByte(clr.G);
            bf.WriteByte(clr.B);
            SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendTimeTo(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long) ServerPackets.Time);
            bf.WriteLong(ServerTime.GetTime().ToBinary());
            if (TimeBase.GetTimeBase().SyncTime)
            {
                bf.WriteDouble(1);
            }
            else
            {
                bf.WriteDouble((double) TimeBase.GetTimeBase().Rate);
            }
            //Write the color tint the clients should be using when outdoors
            var clr = ServerTime.GetTimeColor();
            bf.WriteByte(clr.A);
            bf.WriteByte(clr.R);
            bf.WriteByte(clr.G);
            bf.WriteByte(clr.B);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendParty(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long) ServerPackets.PartyData);
            bf.WriteInteger(client.Entity.Party.Count);
            for (int i = 0; i < client.Entity.Party.Count; i++)
            {
                bf.WriteInteger(client.Entity.Party[i].MyIndex);
            }
            client.SendPacket(bf.ToArray());
            for (int i = 0; i < client.Entity.Party.Count; i++)
            {
                SendEntityDataTo(client, client.Entity.Party[i]);
            }
            bf.Dispose();
        }

        public static void SendPartyInvite(Client client, Player leader)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long) ServerPackets.PartyInvite);
            bf.WriteString(leader.MyName);
            bf.WriteInteger(leader.MyIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendChatBubble(int entityIndex, int type, string text, int map)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.ChatBubble);
            bf.WriteLong(entityIndex);
            bf.WriteInteger(type);
            bf.WriteInteger(map);
            bf.WriteString(text);
            SendDataToProximity(map, bf.ToArray());
            bf.Dispose();
        }

        public static void SendQuestOffer(Player player, int questId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.QuestOffer);
            bf.WriteInteger(questId);
            SendDataTo(player.MyClient, bf.ToArray());
            bf.Dispose();
        }

        public static void SendQuestsProgress(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.QuestProgress);
            bf.WriteInteger(client.Entity.Quests.Count);
            foreach (var quest in client.Entity.Quests)
            {
                bf.WriteInteger(quest.Key);
                bf.WriteByte(1);
                bf.WriteInteger(quest.Value.completed);
                bf.WriteInteger(quest.Value.task);
                bf.WriteInteger(quest.Value.taskProgress);
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendQuestProgress(Player player, int questId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.QuestProgress);
            bf.WriteInteger(1);
            bf.WriteInteger(questId);
            if (player.Quests.ContainsKey(questId))
            {
                bf.WriteByte(1);
                bf.WriteInteger(player.Quests[questId].completed);
                bf.WriteInteger(player.Quests[questId].task);
                bf.WriteInteger(player.Quests[questId].taskProgress);
            }
            else
            {
                bf.WriteByte(0);
            }
            SendDataTo(player.MyClient, bf.ToArray());
            bf.Dispose();
        }

        public static void StartTrade(Client client, int target)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long) ServerPackets.TradeStart);
            bf.WriteInteger(target);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendTradeUpdate(Client client, int index, int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long) ServerPackets.TradeUpdate);
            bf.WriteInteger(index);
            bf.WriteInteger(slot);
            if (((Player) Globals.Entities[index]).Trade[slot] == null ||
                ((Player) Globals.Entities[index]).Trade[slot].ItemNum < 0 ||
                ((Player) Globals.Entities[index]).Trade[slot].ItemVal <= 0)
            {
                bf.WriteInteger(0);
            }
            else
            {
                bf.WriteInteger(1);
                bf.WriteBytes(((Player) Globals.Entities[index]).Trade[slot].Data());
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendTradeClose(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long) ServerPackets.TradeClose);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendTradeRequest(Client client, Player partner)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long) ServerPackets.TradeRequest);
            bf.WriteString(partner.MyName);
            bf.WriteInteger(partner.MyIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendPlayerDeath(Player en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long) ServerPackets.PlayerDeath);
            bf.WriteLong(en.MyIndex);
            SendDataToProximity(en.CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void UpdateEntityZDimension(int index, int z)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long) ServerPackets.EntityZDimension);
            bf.WriteLong(index);
            bf.WriteInteger(z);
            SendDataToProximity(Globals.Entities[index].CurrentMap, bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenBag(Client client, int slots, BagInstance bag)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.OpenBag);
            bf.WriteInteger(slots);
            client.SendPacket(bf.ToArray());
            for (int i = 0; i < slots; i++)
            {
                SendBagUpdate(client, i, bag.Items[i]);
            }
            bf.Dispose();
        }

        public static void SendBagUpdate(Client client, int slot, ItemInstance item)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.BagUpdate);
            bf.WriteInteger(slot);
            if (item == null || item.ItemNum < 0 || item.ItemVal <= 0)
            {
                bf.WriteInteger(0);
            }
            else
            {
                bf.WriteInteger(1);
                bf.WriteBytes(item.Data());
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendCloseBag(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.CloseBag);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMoveRouteToggle(Client client, bool routeOn)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.MoveRouteToggle);
            bf.WriteBoolean(routeOn);
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendFriends(Client client)
        {
            var bf = new ByteBuffer();
            List<string> online = new List<string>();
            List<string> offline = new List<string>();
            List<string> map = new List<string>();
            bool found = false;

            foreach (var friend in client.Entity.Friends)
            {
                found = false;
                foreach (var c in Globals.Clients)
                {
                    if (c != null && c.Entity != null)
                    {
                        if (friend.Value.ToLower() == c.Entity.MyName.ToLower())
                        {
                            online.Add(friend.Value);
                            map.Add(MapList.GetList().FindMap(client.Entity.CurrentMap).Name);
                            found = true;
                            break;
                        }
                    }
                }
                if (found == false)
                {
                    offline.Add(friend.Value);
                }
            }

            bf.WriteLong((int) ServerPackets.SendFriends);

            bf.WriteInteger(online.Count);
            for (int i = 0; i < online.Count; i++)
            {
                bf.WriteString(online[i]);
                bf.WriteString(map[i]);
            }

            bf.WriteInteger(offline.Count);
            for (int i = 0; i < offline.Count; i++)
            {
                bf.WriteString(offline[i]);
            }

            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendFriendRequest(Client client, Player partner)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long) ServerPackets.FriendRequest);
            bf.WriteString(partner.MyName);
            bf.WriteInteger(partner.MyIndex);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }
    }
}