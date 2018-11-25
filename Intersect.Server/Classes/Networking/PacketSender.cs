using System;
using System.Collections.Generic;
using System.Linq;
using Intersect.Collections;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Logging;
using Intersect.Models;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Characters;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Maps;

namespace Intersect.Server.Networking
{
    using LegacyDatabase = LegacyDatabase;

    public static class PacketSender
    {
        public static void SendDataToMap(Guid mapId, byte[] data, Client except = null)
        {
            if (!MapInstance.Lookup.Keys.Contains(mapId))
            {
                return;
            }
            List<Player> players = MapInstance.Get(mapId).GetPlayersOnMap();
            foreach (var player in players)
            {
                if (player != null && player.MyClient != except) player.MyClient.SendPacket(data);
            }
        }

        public static void SendDataToProximity(Guid mapId, byte[] data, Client except = null)
        {
            if (!MapInstance.Lookup.Keys.Contains(mapId))
            {
                return;
            }
            SendDataToMap(mapId, data, except);
            for (int i = 0; i < MapInstance.Get(mapId).SurroundingMaps.Count; i++)
            {
                SendDataToMap(MapInstance.Get(mapId).SurroundingMaps[i], data, except);
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
                bf.WriteLong((int)ServerPackets.Ping);
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
            bf.WriteLong((int)ServerPackets.JoinGame);
            SendEntityDataTo(client, client.Entity);
            client.SendPacket(bf.ToArray());
            bf.Dispose();

            if (!client.IsEditor)
            {
                SendGlobalMsg(Strings.Player.joined.ToString( client.Entity.Name, Options.GameName));
            }
        }

        public static void SendMap(Client client, Guid mapId, bool allEditors = false)
        {
            if (client == null)
            {
                Log.Error("Attempted to send packet to null client.");
                return;
            }

            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.MapData);
            bf.WriteGuid(mapId);
            var map = MapInstance.Get(mapId);
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
                byte[] mapData;
                if (client.IsEditor)
                {
                    foreach (var id in map.EventIds)
                    {
                        var evt = EventBase.Get(id);
                        if (evt != null) SendGameObject(client, evt);
                    }
                    bf.WriteString(map.JsonData);
                    var tileData = map.TileData;
                    bf.WriteInteger(tileData.Length);
                    bf.WriteBytes(tileData);
                    var attributeData = map.AttributeData;
                    bf.WriteInteger(attributeData.Length);
                    bf.WriteBytes(attributeData);
                    bf.WriteInteger(map.MapGridX);
                    bf.WriteInteger(map.MapGridY);
                }
                else
                {
                    if (client.SentMaps.ContainsKey(mapId))
                    {
                        if (client.SentMaps[mapId].Item1 > Globals.System.GetTimeMs() &&
                            client.SentMaps[mapId].Item2 == map.Revision) return;
                        client.SentMaps.Remove(mapId);
                    }

                    try
                    {
                        client.SentMaps.Add(mapId,
                            new Tuple<long, int>(Globals.System.GetTimeMs() + 5000, map.Revision));
                    }
                    catch (Exception exception)
                    {
                        Log.Error($"Current Map #: {mapId}");
                        Log.Error($"# Sent maps: {client.SentMaps.Count}");
                        Log.Error($"# Maps: {MapInstance.Lookup.Count}");
                        Log.Error(exception);
                        throw;
                    }

                    bf.WriteString(map.JsonData);
                    var tileData = map.TileData;
                    bf.WriteInteger(tileData.Length);
                    bf.WriteBytes(tileData);
                    var attributeData = map.AttributeData;
                    bf.WriteInteger(attributeData.Length);
                    bf.WriteBytes(attributeData);
                    bf.WriteInteger(map.Revision);
                    bf.WriteInteger(map.MapGridX);
                    bf.WriteInteger(map.MapGridY);
                    switch (Options.GameBorderStyle)
                    {
                        case 1:
                            bf.WriteInteger(1);
                            bf.WriteInteger(1);
                            bf.WriteInteger(1);
                            bf.WriteInteger(1);
                            break;

                        case 0:
                            bf.WriteInteger(0 == map.MapGridX ? 1 : 0);
                            bf.WriteInteger(LegacyDatabase.MapGrids[map.MapGrid].XMax - 1 == map.MapGridX ? 1 : 0);
                            bf.WriteInteger(0 == map.MapGridY ? 1 : 0);
                            bf.WriteInteger(LegacyDatabase.MapGrids[map.MapGrid].YMax - 1 == map.MapGridY ? 1 : 0);
                            break;

                        default:
                            bf.WriteInteger(0);
                            bf.WriteInteger(0);
                            bf.WriteInteger(0);
                            bf.WriteInteger(0);
                            break;
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
                    SendMapItems(client, mapId);
                }
            }
            bf.Dispose();
        }

        public static void SendMapToEditors(Guid mapId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.MapData);
            bf.WriteGuid(mapId);
            if (MapInstance.Get(mapId) == null)
            {
                bf.WriteInteger(1);
            }
            else
            {
                var map = MapInstance.Get(mapId);
                bf.WriteInteger(0);
                bf.WriteString(map.JsonData);
                var tileData = map.TileData;
                bf.WriteInteger(tileData.Length);
                bf.WriteBytes(tileData);
                var attributeData = map.AttributeData;
                bf.WriteInteger(attributeData.Length);
                bf.WriteBytes(attributeData);
            }
            SendDataToEditors(bf.ToArray());
            bf.Dispose();
        }

        private static byte[] GetEntityPacket(EntityInstance en, Client forClient)
        {
            var bf = new ByteBuffer();
            bf.WriteGuid(en.Id);
            bf.WriteInteger((int)en.GetEntityType());
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

        public static void SendEntityDataTo(Client client, EntityInstance en)
        {
            if (en == null)
            {
                return;
            }
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityData);
            bf.WriteGuid(en.Id);
            bf.WriteInteger((int)en.GetEntityType());
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
                SendPlayerEquipmentTo(client, (Player)en);
            }

            if (en.GetType() == typeof(Npc))
            {
                SendNpcAggressionTo(client.Entity,(Npc)en);
            }
        }

        public static void SendMapEntitiesTo(Client client, List<EntityInstance> entities)
        {
            var buff = new ByteBuffer();
            buff.WriteLong((long)ServerPackets.MapEntities);
            var sendEntities = new List<EntityInstance>();
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

            for (int i = 0; i < sendEntities.Count; i++)
            {
                if (sendEntities[i].GetType() == typeof(Npc))
                {
                    SendNpcAggressionTo(client.Entity, (Npc)sendEntities[i]);
                }
            }
        }

        public static void SendMapEntityEquipmentTo(Client client, List<EntityInstance> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] != null && entities[i] != client.Entity)
                {
                    //If a player, send equipment to all (for paperdolls)
                    if (entities[i].GetType() == typeof(Player) && client.Entity != entities[i])
                    {
                        SendPlayerEquipmentTo(client, (Player)entities[i]);
                    }
                }
            }
        }

        public static void SendEntityDataToProximity(EntityInstance en, Client except = null)
        {
            if (en == null)
            {
                return;
            }
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityData);
            bf.WriteGuid(en.Id);
            bf.WriteInteger((int)en.GetEntityType());
            bf.WriteBytes(en.Data());
            SendDataToProximity(en.MapId, bf.ToArray(), except);
            bf.Dispose();
            SendEntityVitals(en);
            SendEntityStats(en);

            //If a player, send equipment to all (for paperdolls)
            if (en.GetType() == typeof(Player))
            {
                SendPlayerEquipmentToProximity((Player)en);
            }

            if (en.GetType() == typeof(Npc))
            {
                SendNpcAggressionToProximity((Npc)en);
            }
        }

        public static void SendEntityPositionTo(Client client, EntityInstance en)
        {
            if (en == null)
            {
                return;
            }
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityPosition);
            bf.WriteGuid(en.Id);
            bf.WriteInteger((int)en.GetEntityType());
            bf.WriteGuid(en.MapId);
            bf.WriteInteger(en.X);
            bf.WriteInteger(en.Y);
            bf.WriteInteger(en.Dir);
            bf.WriteBoolean(en.Passable);
            bf.WriteBoolean(en.HideName);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityPositionToAll(EntityInstance en)
        {
            if (en == null)
            {
                return;
            }
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityPosition);
            bf.WriteGuid(en.Id);
            bf.WriteInteger((int)en.GetEntityType());
            bf.WriteGuid(en.MapId);
            bf.WriteInteger(en.X);
            bf.WriteInteger(en.Y);
            bf.WriteInteger(en.Dir);
            bf.WriteBoolean(en.Passable);
            bf.WriteBoolean(en.HideName);
            SendDataToProximity(en.MapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendNpcAggressionToProximity(Npc en)
        {
            if (en == null)
            {
                return;
            }
            var map = en.Map;
            foreach (var mp in en.Map.GetSurroundingMaps(true))
            {
                var players = mp.GetPlayersOnMap();
                foreach (var pl in players)
                {
                    SendNpcAggressionTo(pl, en);
                }
            }
        }

        public static void SendNpcAggressionTo(Player en, Npc npc)
        {
            if (en == null || npc == null) return;
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.NpcAggression);
            bf.WriteGuid(npc.Id);

            //Declare Aggression state
            if (npc.MyTarget != null)
            {
                bf.WriteInteger(-1);
            }
            else
            {
                //TODO (0 is attack when attacked, 1 is attack on sight, 2 is friendly, 3 is guard)
                var aggression = 0;
                if (npc.IsFriend(en) || !en.CanAttack(npc, null))
                {
                    aggression = 2;
                }
                else if (npc.ShouldAttackPlayerOnSight(en))
                {
                    aggression = 1;
                }
                bf.WriteInteger(aggression);
            }

            SendDataTo(en.MyClient, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityLeave(Guid id, int type, Guid mapId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityLeave);
            bf.WriteGuid(id);
            bf.WriteInteger(type);
            bf.WriteGuid(mapId);
            SendDataToProximity(mapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityLeaveTo(Client client, Guid entityId, int type, Guid mapId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityLeave);
            bf.WriteGuid(entityId);
            bf.WriteInteger(type);
            bf.WriteGuid(mapId);
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
            bf.WriteLong((int)ServerPackets.ChatMessage);
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
                if ((GameObjectType)val == GameObjectType.Map) continue;
                if (((GameObjectType)val == GameObjectType.Shop ||
                     (GameObjectType)val == GameObjectType.Event ||
                     (GameObjectType)val == GameObjectType.PlayerSwitch ||
                     (GameObjectType)val == GameObjectType.PlayerVariable ||
                     (GameObjectType)val == GameObjectType.ServerSwitch ||
                     (GameObjectType)val == GameObjectType.ServerVariable) && !client.IsEditor) continue;
                SendGameObjects(client, (GameObjectType)val);
            }

            if (!client.IsEditor)
            {
                SendGameObject(client, ClassBase.Get(client.Entity.ClassId));
            }

            //Let the client/editor know they have everything now
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.GameData);
            bf.WriteBytes(CustomColors.GetData());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendGlobalMsg(string message, string target = "")
        {
            SendGlobalMsg(message, CustomColors.AnnouncementChat, target);
        }

        public static void SendGlobalMsg(string message, Color clr, string target = "")
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ChatMessage);
            bf.WriteString(message);
            bf.WriteByte(clr.A);
            bf.WriteByte(clr.R);
            bf.WriteByte(clr.G);
            bf.WriteByte(clr.B);
            bf.WriteString(target);
            SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendProximityMsg(string message, Guid mapId, string target = "")
        {
            SendProximityMsg(message, mapId, CustomColors.ProximityMsg);
        }

        public static void SendProximityMsg(string message, Guid mapId, Color clr, string target = "")
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ChatMessage);
            bf.WriteString(message);
            bf.WriteByte(clr.A);
            bf.WriteByte(clr.R);
            bf.WriteByte(clr.G);
            bf.WriteByte(clr.B);
            bf.WriteString(target);
            SendDataToProximity(mapId, bf.ToArray());
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
                        if (client.Power != UserRights.None)
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

        public static void SendDataToAllBut(EntityInstance en, byte[] packet)
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

        public static void SendRemoveProjectileSpawn(Guid mapId, Guid baseEntityId, int spawnIndex)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ProjectileSpawnDead);
            bf.WriteGuid(baseEntityId);
            bf.WriteLong(spawnIndex);
            SendDataToProximity(mapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityMove(EntityInstance en, bool correction = false)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityMove);
            bf.WriteGuid(en.Id);
            bf.WriteInteger((int)en.GetEntityType());
            bf.WriteGuid(en.MapId);
            bf.WriteInteger(en.X);
            bf.WriteInteger(en.Y);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(Convert.ToInt32(correction));
            SendDataToProximity(en.MapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityMoveTo(Client client, EntityInstance en, bool correction = false)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityMove);
            bf.WriteGuid(en.Id);
            bf.WriteInteger((int)en.GetEntityType());
            bf.WriteGuid(en.MapId);
            bf.WriteInteger(en.X);
            bf.WriteInteger(en.Y);
            bf.WriteInteger(en.Dir);
            bf.WriteInteger(Convert.ToInt32(correction));
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityVitals(EntityInstance en)
        {
            if (en == null) return;
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityVitals);
            bf.WriteGuid(en.Id);
            bf.WriteInteger((int)en.GetEntityType());
            bf.WriteGuid(en.MapId);
            for (var i = 0; i < (int)Vitals.VitalCount; i++)
            {
                bf.WriteInteger(en.GetMaxVital(i));
                bf.WriteInteger(en.GetVital(i));
            }
            var statuses = en.Statuses.Values.ToArray();
            bf.WriteInteger(statuses.Length);
            foreach (var status in statuses)
            {
                bf.WriteGuid(status.Spell.Id);
                bf.WriteInteger((int)status.Type);
                bf.WriteString(status.Data);
                bf.WriteInteger((int)(status.Duration - Globals.System.GetTimeMs()));
                bf.WriteInteger((int)(status.Duration - status.StartTime));
            }
            //If player and in party send vitals to party just in case party members are not in the proximity
            if (en.GetType() == typeof(Player))
            {
                for (var i = 0; i < ((Player)en).Party.Count; i++)
                {
                    SendPartyUpdateTo(((Player)en).Party[i].MyClient, (Player)en);
                }
            }
            SendDataToProximity(en.MapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityStats(EntityInstance en)
        {
            if (en == null) return;
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityStats);
            bf.WriteGuid(en.Id);
            bf.WriteInteger((int)en.GetEntityType());
            bf.WriteGuid(en.MapId);
            for (var i = 0; i < (int)Stats.StatCount; i++)
            {
                bf.WriteInteger(en.Stat[i].Value());
            }
            SendDataToProximity(en.MapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityVitalsTo(Client client, EntityInstance en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityVitals);
            bf.WriteGuid(en.Id);
            bf.WriteInteger((int)en.GetEntityType());
            bf.WriteGuid(en.MapId);
            for (var i = 0; i < (int)Vitals.VitalCount; i++)
            {
                bf.WriteInteger(en.GetMaxVital(i));
                bf.WriteInteger(en.GetVital(i));
            }
            var statuses = en.Statuses.Values.ToArray();
            bf.WriteInteger(statuses.Length);
            foreach (var status in statuses)
            {
                bf.WriteGuid(status.Spell.Id);
                bf.WriteInteger((int)status.Type);
                bf.WriteString(status.Data);
                bf.WriteInteger((int)(status.Duration - Globals.System.GetTimeMs()));
                bf.WriteInteger((int)(status.Duration - status.StartTime));
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityStatsTo(Client client, EntityInstance en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityStats);
            bf.WriteGuid(en.Id);
            bf.WriteInteger((int)en.GetEntityType());
            bf.WriteGuid(en.MapId);
            for (var i = 0; i < (int)Stats.StatCount; i++)
            {
                bf.WriteInteger(en.Stat[i].Value());
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityDir(Guid entityId, int type, int dir, Guid mapId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityDir);
            bf.WriteGuid(entityId);
            bf.WriteInteger(type);
            bf.WriteGuid(mapId);
            bf.WriteInteger(dir);
            SendDataToProximity(mapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityAttack(EntityInstance en, int type, Guid mapId, int attackTime)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityAttack);
            bf.WriteGuid(en.Id);
            bf.WriteInteger(type);
            bf.WriteGuid(mapId);
            bf.WriteInteger(attackTime);
            SendDataToProximity(mapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityDirTo(Client client, Guid entityId, int type, int dir, Guid mapId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityDir);
            bf.WriteGuid(entityId);
            bf.WriteInteger(type);
            bf.WriteGuid(mapId);
            bf.WriteInteger(dir);
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEventDialog(Player player, string prompt, string face, Guid eventId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EventDialog);
            bf.WriteString(prompt);
            bf.WriteString(face);
            bf.WriteInteger(0);
            bf.WriteGuid(eventId);
            player.MyClient.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEventDialog(Player player, string prompt, string opt1, string opt2, string opt3,
            string opt4, string face, Guid eventId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EventDialog);
            bf.WriteString(prompt);
            bf.WriteString(face);
            bf.WriteInteger(1);
            bf.WriteString(opt1);
            bf.WriteString(opt2);
            bf.WriteString(opt3);
            bf.WriteString(opt4);;
            bf.WriteGuid(eventId);
            player.MyClient.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapList(Client client)
        {
            var bf = new ByteBuffer();
            DatabaseObjectLookup gameMaps = new DatabaseObjectLookup();
            foreach (var pair in MapInstance.Lookup.Pairs) gameMaps.Set(pair.Key, pair.Value);
            bf.WriteLong((int)ServerPackets.MapList);
            bf.WriteString(MapList.GetList().JsonData);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapListToAll()
        {
            var bf = new ByteBuffer();
            DatabaseObjectLookup gameMaps = new DatabaseObjectLookup();
            foreach (var pair in MapInstance.Lookup.Pairs) gameMaps.Set(pair.Key, pair.Value);
            bf.WriteLong((int)ServerPackets.MapList);
            MapList.GetList().PostLoad(MapInstance.Lookup,true,true);
            bf.WriteString(MapList.GetList().JsonData);
            SendDataToAll(bf.ToArray());
            bf.Dispose();
        }

        public static void SendLoginError(Client client, string error, string header = "")
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.LoginError);
            bf.WriteString(error);
            bf.WriteString(header);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapItems(Client client, Guid mapId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.MapItems);
            bf.WriteGuid(mapId);
            bf.WriteInteger(MapInstance.Get(mapId).MapItems.Count);
            for (int i = 0; i < MapInstance.Get(mapId).MapItems.Count; i++)
            {
                if (MapInstance.Get(mapId).MapItems[i] != null)
                {
                    bf.WriteInteger(i);
                    bf.WriteBytes(MapInstance.Get(mapId).MapItems[i].Data());
                }
                else
                {
                    bf.WriteInteger(-1);
                }
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapItemsToProximity(Guid mapId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.MapItems);
            bf.WriteGuid(mapId);
            bf.WriteInteger(MapInstance.Get(mapId).MapItems.Count);
            for (int i = 0; i < MapInstance.Get(mapId).MapItems.Count; i++)
            {
                bf.WriteBytes(MapInstance.Get(mapId).MapItems[i].Data());
            }
            SendDataToProximity(mapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendMapItemUpdate(Guid mapId, int index)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.MapItemUpdate);
            bf.WriteGuid(mapId);
            bf.WriteInteger(index);
            if (MapInstance.Get(mapId).MapItems[index] == null ||
                MapInstance.Get(mapId).MapItems[index].ItemId == Guid.Empty)
            {
                bf.WriteInteger(-1);
            }
            else
            {
                bf.WriteInteger(1);
                bf.WriteBytes(MapInstance.Get(mapId).MapItems[index].Data());
            }
            SendDataToProximity(mapId, bf.ToArray());
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
            bf.WriteBytes(client.Entity.Items[slot].Data());
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
            bf.WriteGuid(en.Id);
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (client.Entity == en)
                {
                    bf.WriteInteger(en.Equipment[i]);
                }
                else
                {
                    if (en.Equipment[i] == -1 || en.Items[en.Equipment[i]].ItemId == Guid.Empty)
                    {
                        bf.WriteGuid(Guid.Empty);
                    }
                    else
                    {
                        bf.WriteGuid(en.Items[en.Equipment[i]].ItemId);
                    }
                }
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendPlayerEquipmentToProximity(Player en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.PlayerEquipment);
            bf.WriteGuid(en.Id);
            for (int i = 0; i < Options.EquipmentSlots.Count; i++)
            {
                if (en.Equipment[i] == -1 || en.Items[en.Equipment[i]].ItemId == Guid.Empty)
                {
                    bf.WriteGuid(Guid.Empty);
                }
                else
                {
                    bf.WriteGuid(en.Items[en.Equipment[i]].ItemId);
                }
            }
            SendDataToProximity(en.MapId, bf.ToArray(), en.MyClient);
            SendPlayerEquipmentTo(en.MyClient, en);
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
                bf.WriteGuid(client.Entity.Hotbar[i].ItemOrSpellId);
                bf.WriteGuid(client.Entity.Hotbar[i].BagId);
                if (client.Entity.Hotbar[i].PreferredStatBuffs != null)
                {
                    bf.WriteBoolean(true);
                    for (var s = 0; s < (int)Stats.StatCount; s++)
                        bf.WriteInteger(client.Entity.Hotbar[i].PreferredStatBuffs[s]);
                }
                else
                {
                    bf.WriteBoolean(false);
                }
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendCreateCharacter(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.CreateCharacter);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendPlayerCharacters(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.PlayerCharacters);
            bf.WriteInteger(client.Characters.Count);
            bf.WriteBoolean(client.Characters.Count < Options.MaxCharacters);
            foreach (var character in client.Characters.OrderByDescending(p => p.LastOnline))
            {
                bf.WriteGuid(character.Id);
                bf.WriteString(character.Name);
                bf.WriteString(character.Sprite);
                bf.WriteString(character.Face);
                bf.WriteInteger(character.Level);
                bf.WriteString(ClassBase.GetName(character.ClassId));


                var equipmentArray = character.Equipment;
                var equipment = new string[Options.EquipmentSlots.Count + 1];
                //Draw the equipment/paperdolls
                for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
                {
                    if (Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z]) > -1)
                    {
                        if (equipmentArray[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] > -1 && equipmentArray[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] < Options.MaxInvItems)
                        {
                            Guid itemId = character.Items[equipmentArray[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])]].ItemId;

                            if (ItemBase.Get(itemId) != null)
                            {
                                var itemdata = ItemBase.Get(itemId);
                                if (character.Gender == 0)
                                {
                                    equipment[z] = itemdata.MalePaperdoll;
                                }
                                else
                                {
                                    equipment[z] = itemdata.FemalePaperdoll;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Options.PaperdollOrder[1][z] == "Player")
                        {
                            equipment[z] = "Player";
                        }
                    }
                }

                for (int i = 0; i < Options.EquipmentSlots.Count + 1; i++)
                {
                    bf.WriteString(equipment[i]);
                }

            }
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

        public static void SendMapGridToAll(int gridIndex)
        {
            for (int i = 0; i < Globals.Clients.Count; i++)
            {
                if (Globals.Clients[i] != null)
                {
                    if (Globals.Clients[i].IsEditor)
                    {
                        if (LegacyDatabase.MapGrids[gridIndex].HasMap(Globals.Clients[i].EditorMap))
                        {
                            SendMapGrid(Globals.Clients[i], gridIndex);
                        }
                    }
                    else
                    {
                        if (Globals.Clients[i].Entity != null)
                        {
                            if (LegacyDatabase.MapGrids[gridIndex].HasMap(Globals.Clients[i].Entity.MapId))
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
            bf.WriteLong((int)ServerPackets.MapGrid);
            bf.WriteLong(LegacyDatabase.MapGrids[gridIndex].Width);
            bf.WriteLong(LegacyDatabase.MapGrids[gridIndex].Height);
            bf.WriteBoolean(clearKnownMaps);
            if (clearKnownMaps) client.SentMaps.Clear();
            for (int x = 0; x < LegacyDatabase.MapGrids[gridIndex].Width; x++)
            {
                for (int y = 0; y < LegacyDatabase.MapGrids[gridIndex].Height; y++)
                {
                    if (MapInstance.Get(LegacyDatabase.MapGrids[gridIndex].MyGrid[x, y]) != null)
                    {
                        bf.WriteGuid(LegacyDatabase.MapGrids[gridIndex].MyGrid[x, y]);
                        if (client.IsEditor)
                        {
                            bf.WriteString(MapInstance.Lookup
                                .Get<MapInstance>(LegacyDatabase.MapGrids[gridIndex].MyGrid[x, y]).Name);
                            bf.WriteInteger(MapInstance.Lookup
                                .Get<MapInstance>(LegacyDatabase.MapGrids[gridIndex].MyGrid[x, y]).Revision);
                        }
                    }
                    else
                    {
                        bf.WriteGuid(Guid.Empty);
                    }
                }
            }
            client.SendPacket(bf.ToArray());
            if (!client.IsEditor && clearKnownMaps) SendMap(client, client.Entity.MapId);
            bf.Dispose();
        }

        public static void SendEntityCastTime(EntityInstance en, Guid spellId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.CastTime);
            bf.WriteGuid(en.Id);
            bf.WriteGuid(spellId);
            SendDataToProximity(en.MapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendSpellCooldown(Client client, int spellSlot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.SpellCooldown);
            bf.WriteLong(spellSlot);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendItemCooldown(Client client, Guid itemId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ItemCooldown);
            bf.WriteGuid(itemId);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendExperience(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int) ServerPackets.Experience);
            bf.WriteLong(client.Entity.Exp);
            bf.WriteLong(client.Entity.ExperienceToNextLevel);
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

        public static void SendAnimationToProximity(Guid animId, int targetType, Guid entityId, Guid mapId, int x, int y,
            int direction)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.SendPlayAnimation);
            bf.WriteGuid(animId);
            bf.WriteInteger(targetType);
            bf.WriteGuid(entityId);
            bf.WriteGuid(mapId);
            bf.WriteInteger(x);
            bf.WriteInteger(y);
            bf.WriteInteger(direction);
            SendDataToProximity(mapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendHoldPlayer(Client client, Guid eventId, Guid mapId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.HoldPlayer);
            bf.WriteGuid(eventId);
            bf.WriteGuid(mapId);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendReleasePlayer(Client client, Guid eventId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ReleasePlayer);
            bf.WriteGuid(eventId);
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

        public static void SendShowPicture(Client client, string picture, int size, bool clickable)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ShowPicture);
            bf.WriteString(picture);
            bf.WriteInteger(size);
            bf.WriteBoolean(clickable);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendHidePicture(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.HidePicture);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenShop(Client client, ShopBase shop)
        {
            if (shop == null) return;
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenShop);
            bf.WriteString(shop.JsonData);
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

        public static void SendOpenCraftingTable(Client client, CraftingTableBase table)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenCraftingTable);
            if (table != null)
            {
                bf.WriteString(table.JsonData);
                client.SendPacket(bf.ToArray());
            }
            bf.Dispose();
        }

        public static void SendCloseCraftingTable(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.CloseCraftingTable);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendBankUpdate(Client client, int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.BankUpdate);
            bf.WriteInteger(slot);
            if (client.Entity.Bank[slot] == null || client.Entity.Bank[slot].ItemId == Guid.Empty ||
                client.Entity.Bank[slot].Quantity <= 0)
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
                case GameObjectType.CraftTables:
                    foreach (var obj in CraftingTableBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Crafts:
                    foreach (var obj in CraftBase.Lookup)
                        SendGameObject(client, obj.Value);
                    break;
                case GameObjectType.Map:
                    throw new Exception("Maps are not sent as batches, use the proper send map functions");
                case GameObjectType.Event:
                    foreach (var obj in EventBase.Lookup)
                        if (((EventBase)obj.Value).CommonEvent)
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
            if (client == null || obj == null) return;

            if (client.IsEditor)
            {
                //If editor send quest events and map events
                if (obj.Type == GameObjectType.Quest)
                {
                    SendQuestEventsTo(client, (QuestBase) obj);
                }
            }
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.GameObject);
            bf.WriteInteger((int)obj.Type);
            bf.WriteGuid(obj.Id);
            bf.WriteInteger(Convert.ToInt32(another));
            bf.WriteInteger(Convert.ToInt32(deleted));
            if (!deleted) bf.WriteString(obj.JsonData);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendQuestEventsTo(Client client, QuestBase qst)
        {
            SendEventIfExists(client, qst.StartEvent);
            SendEventIfExists(client, qst.EndEvent);
            foreach (var tsk in qst.Tasks)
            {
                SendEventIfExists(client, tsk.CompletionEvent);
            }
        }

        public static void SendEventIfExists(Client client, EventBase evt)
        {
            if (evt != null && evt.Id != Guid.Empty)
            {
                SendGameObject(client, evt, false, false);
            }
        }

        public static void SendGameObjectToAll(IDatabaseObject obj, bool deleted = false, bool another = false)
        {
            foreach (var client in Globals.Clients)
                SendGameObject(client, obj, deleted, another);
        }

        public static void SendOpenEditor(Client client, GameObjectType type)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.GameObjectEditor);
            bf.WriteInteger((int)type);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendEntityDash(EntityInstance en, Guid endMapId, int endX, int endY, int dashTime, int direction)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EntityDash);
            bf.WriteGuid(en.Id);
            bf.WriteGuid(endMapId);
            bf.WriteInteger(endX);
            bf.WriteInteger(endY);
            bf.WriteInteger(dashTime);
            bf.WriteInteger(direction);
            SendDataToProximity(en.MapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendActionMsg(EntityInstance en, string message, Color color)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ActionMsg);
            bf.WriteGuid(en.MapId);
            bf.WriteInteger(en.X);
            bf.WriteInteger(en.Y);
            bf.WriteString(message);
            bf.WriteByte(color.A);
            bf.WriteByte(color.R);
            bf.WriteByte(color.G);
            bf.WriteByte(color.B);
            SendDataToProximity(en.MapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendEnterMap(Client client, Guid mapId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.EnterMap);
            bf.WriteGuid(mapId);
            var map = MapInstance.Get(mapId);
            if (!(map.MapGridX == -1 || map.MapGridY == -1))
            {
                for (var y = map.MapGridY - 1; y < map.MapGridY + 2; y++)
                {
                    for (var x = map.MapGridX - 1;
                        x < map.MapGridX + 2;
                        x++)
                    {
                        if (x >= LegacyDatabase.MapGrids[map.MapGrid].XMin &&
                            x < LegacyDatabase.MapGrids[map.MapGrid].XMax &&
                            y >= LegacyDatabase.MapGrids[map.MapGrid].YMin &&
                            y < LegacyDatabase.MapGrids[map.MapGrid].YMax)
                        {
                            bf.WriteGuid(LegacyDatabase.MapGrids[map.MapGrid].MyGrid[x, y]);
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
            bf.WriteLong((long)ServerPackets.TimeBase);
            bf.WriteBytes(TimeBase.GetTimeBase().SaveTimeBase());
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendTimeBaseToAllEditors()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long)ServerPackets.TimeBase);
            bf.WriteBytes(TimeBase.GetTimeBase().SaveTimeBase());
            SendDataToEditors(bf.ToArray());
            bf.Dispose();
        }

        public static void SendTimeToAll()
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long)ServerPackets.Time);
            bf.WriteLong(ServerTime.GetTime().ToBinary());
            if (TimeBase.GetTimeBase().SyncTime)
            {
                bf.WriteDouble(1);
            }
            else
            {
                bf.WriteDouble((double)TimeBase.GetTimeBase().Rate);
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
            bf.WriteLong((long)ServerPackets.Time);
            bf.WriteLong(ServerTime.GetTime().ToBinary());
            if (TimeBase.GetTimeBase().SyncTime)
            {
                bf.WriteDouble(1);
            }
            else
            {
                bf.WriteDouble((double)TimeBase.GetTimeBase().Rate);
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
            bf.WriteLong((long)ServerPackets.PartyData);
            bf.WriteInteger(client.Entity.Party.Count);
            for (int i = 0; i < client.Entity.Party.Count; i++)
            {
                bf.WriteBytes(client.Entity.Party[i].PartyData());
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendPartyUpdateTo(Client client, Player entity)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long)ServerPackets.PartyUpdate);
            var partyIndex = -1;
            for (int i = 0; i < client.Entity.Party.Count; i++)
            {
                if (client.Entity.Party[i] == entity)
                {
                    partyIndex = i;
                }
            }
            if (partyIndex > -1)
            {
                bf.WriteInteger(partyIndex);
                bf.WriteBytes(entity.PartyData());
                client.SendPacket(bf.ToArray());
            }
            bf.Dispose();
        }

        public static void SendPartyInvite(Client client, Player leader)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long)ServerPackets.PartyInvite);
            bf.WriteString(leader.Name);
            bf.WriteGuid(leader.Id);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendChatBubble(Guid entityId, int type, string text, Guid mapId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.ChatBubble);
            bf.WriteGuid(entityId);
            bf.WriteInteger(type);
            bf.WriteGuid(mapId);
            bf.WriteString(text);
            SendDataToProximity(mapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendQuestOffer(Player player, Guid questId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.QuestOffer);
            bf.WriteGuid(questId);
            SendDataTo(player.MyClient, bf.ToArray());
            bf.Dispose();
        }

        public static void SendQuestsProgress(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.QuestProgress);
            bf.WriteInteger(client.Entity.Quests.Count);
            foreach (var quest in client.Entity.Quests)
            {
                bf.WriteGuid(quest.QuestId);
                bf.WriteByte(1);
                bf.WriteBoolean(quest.Completed);
                bf.WriteGuid(quest.TaskId);
                bf.WriteInteger(quest.TaskProgress);
            }
            SendDataTo(client, bf.ToArray());
            bf.Dispose();
        }

        public static void SendQuestProgress(Player player, Guid questId)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.QuestProgress);
            bf.WriteInteger(1);
            bf.WriteGuid(questId);
            var questProgress = player.FindQuest(questId);
            if (questProgress != null)
            {
                bf.WriteByte(1);
                bf.WriteBoolean(questProgress.Completed);
                bf.WriteGuid(questProgress.TaskId);
                bf.WriteInteger(questProgress.TaskProgress);
            }
            else
            {
                bf.WriteByte(0);
            }
            SendDataTo(player.MyClient, bf.ToArray());
            bf.Dispose();
        }

        public static void StartTrade(Client client, Player target)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long)ServerPackets.TradeStart);
            bf.WriteGuid(target.Id);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendTradeUpdate(Client client, Player trader, int slot)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long)ServerPackets.TradeUpdate);
            bf.WriteGuid(trader.Id);
            bf.WriteInteger(slot);
            if (trader.Trading.Offer[slot] == null ||
                trader.Trading.Offer[slot].ItemId == Guid.Empty ||
                trader.Trading.Offer[slot].Quantity <= 0)
            {
                bf.WriteInteger(0);
            }
            else
            {
                bf.WriteInteger(1);
                bf.WriteBytes(trader.Trading.Offer[slot].Data());
            }
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendTradeClose(Client client)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long)ServerPackets.TradeClose);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendTradeRequest(Client client, Player partner)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long)ServerPackets.TradeRequest);
            bf.WriteString(partner.Name);
            bf.WriteGuid(partner.Id);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendPlayerDeath(Player en)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long)ServerPackets.PlayerDeath);
            bf.WriteGuid(en.Id);
            SendDataToProximity(en.MapId, bf.ToArray());
            bf.Dispose();
        }

        public static void UpdateEntityZDimension(EntityInstance en, int z)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((long)ServerPackets.EntityZDimension);
            bf.WriteGuid(en.Id);
            bf.WriteInteger(z);
            SendDataToProximity(en.MapId, bf.ToArray());
            bf.Dispose();
        }

        public static void SendOpenBag(Client client, int slots, Bag bag)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.OpenBag);
            bf.WriteInteger(slots);
            client.SendPacket(bf.ToArray());
            for (int i = 0; i < slots; i++)
            {
                SendBagUpdate(client, i, bag.Slots[i]);
            }
            bf.Dispose();
        }

        public static void SendBagUpdate(Client client, int slot, Item item)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.BagUpdate);
            bf.WriteInteger(slot);
            if (item == null || item.ItemId == Guid.Empty || item.Quantity <= 0)
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
            bf.WriteLong((int)ServerPackets.CloseBag);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }

        public static void SendMoveRouteToggle(Client client, bool routeOn)
        {
            var bf = new ByteBuffer();
            bf.WriteLong((int)ServerPackets.MoveRouteToggle);
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
                        if (friend.Target.Name.ToLower() == c.Entity.Name.ToLower())
                        {
                            online.Add(friend.Target.Name);
                            map.Add(MapList.GetList().FindMap(friend.Target.MapId).Name);
                            found = true;
                            break;
                        }
                    }
                }
                if (found == false)
                {
                    offline.Add(friend.Target.Name);
                }
            }

            bf.WriteLong((int)ServerPackets.SendFriends);

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
            bf.WriteLong((long)ServerPackets.FriendRequest);
            bf.WriteString(partner.Name);
            bf.WriteGuid(partner.Id);
            client.SendPacket(bf.ToArray());
            bf.Dispose();
        }
    }
}