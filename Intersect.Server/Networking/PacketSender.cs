using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Logging;
using Intersect.Models;
using Intersect.Network;
using Intersect.Network.Packets.Server;
using Intersect.Server.Core;
using Intersect.Server.Database;
using Intersect.Server.Database.Logging.Entities;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.Entities.Events;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Maps;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.Server.Networking
{

    public static partial class PacketSender
    {

        //Cached GameDataPacket that gets sent to clients
        public static GameDataPacket CachedGameDataPacket = null;

        public static ConcurrentDictionary<string, long> SentPacketTypes = new ConcurrentDictionary<string, long>();

        public static long SentPackets { get; set; }

        public static long SentBytes { get; set; }

        public static void ResetMetrics()
        {
            SentPackets = 0;
            SentBytes = 0;
        }

        //PingPacket
        public static void SendPing(Client client, bool request = true)
        {
            if (client != null && client.LastPing + 250 < Globals.Timing.Milliseconds)
            {
                client.Send(new PingPacket(request), TransmissionMode.Any);
                client.LastPing = Globals.Timing.Milliseconds;
            }
        }

        //ConfigPacket
        public static void SendServerConfig(Client client)
        {
            client.Send(new ConfigPacket(Options.OptionsData));
        }

        //EnteringGamePacket
        public static void SendEnteringGamePacket(Player player)
        {
            player.SendPacket(new EnteringGamePacket());
        }

        //JoinGamePacket
        public static void SendJoinGame(Client client)
        {
            UserActivityHistory.LogActivity(client?.User?.Id ?? Guid.Empty, client?.Entity?.Id ?? Guid.Empty, client?.GetIp(), UserActivityHistory.PeerType.Client, UserActivityHistory.UserAction.SelectPlayer, $"{client?.Name},{client?.Entity?.Name}");

            if (!client.IsEditor)
            {
                SendEnteringGamePacket(client.Entity);
                SendEntityDataTo(client.Entity, client.Entity);
            }

            client.TimedBufferPacketsRemaining = 5;
            client.Send(new JoinGamePacket());
            PacketSender.SendGameData(client);

            if (!client.IsEditor)
            {
                var player = client.Entity;
                player.RecalculateStatsAndPoints();
                player.UnequipInvalidItems();
                player.InGame = true;

                SendTimeTo(client);

                if (client.Power.Editor)
                {
                    SendChatMsg(player, Strings.Player.adminjoined, ChatMessageType.Notice, CustomColors.Alerts.AdminJoined);
                }
                else if (client.Power.IsModerator)
                {
                    SendChatMsg(player, Strings.Player.modjoined, ChatMessageType.Notice, CustomColors.Alerts.ModJoined);
                }

                if (player.MapId == Guid.Empty)
                {
                    player.WarpToSpawn();
                }
                else
                {
                    player.Warp(
                        player.MapId, (byte) player.X, (byte) player.Y, (byte) player.Dir, false, (byte) player.Z
                    );
                }

                SendEntityDataTo(client.Entity, player);

                //Search for login activated events and run them
                player.StartCommonEventsWithTrigger(CommonEventTrigger.Login);
            }
        }

        //MapAreaPacket
        public static void SendAreaPacket(Player player)
        {
            var client = player.Client;
            if (client == null)
            {
                return;
            }

            var surroundingMaps = MapInstance.Get(player.MapId).GetSurroundingMaps(true);
            var packets = new List<MapPacket>();
            foreach (var map in surroundingMaps)
            {
                packets.Add(GenerateMapPacket(client, map.Id));
            }

            player.SendPacket(new MapAreaPacket(packets.ToArray()));
        }

        //MapPacket
        public static MapPacket GenerateMapPacket(Client client, Guid mapId)
        {
            if (client == null)
            {
                Log.Error("Attempted to send packet to null client.");

                return null;
            }

            var map = MapInstance.Get(mapId);
            if (map == null)
            {
                return new MapPacket(mapId, true);
            }
            else
            {
                var mapPacket = new MapPacket(
                    mapId, false, map.JsonData, map.TileData, map.AttributeData, map.Revision, map.MapGridX,
                    map.MapGridY, new bool[4]
                );

                if (client.IsEditor)
                {
                    foreach (var id in map.EventIds)
                    {
                        var evt = EventBase.Get(id);
                        if (evt != null)
                        {
                            SendGameObject(client, evt);
                        }
                    }
                }
                else
                {
                    switch (Options.GameBorderStyle)
                    {
                        case 1:
                            mapPacket.CameraHolds = new bool[4] {true, true, true, true};

                            break;

                        case 0:
                            var grid = DbInterface.GetGrid(map.MapGrid);
                            if (grid != null)
                            {
                                mapPacket.CameraHolds = new bool[4]
                                {
                                    0 == map.MapGridY, grid.YMax - 1 == map.MapGridY,
                                    0 == map.MapGridX, grid.XMax - 1 == map.MapGridX
                                };
                            }

                            break;
                    }
                }

                if (client.IsEditor)
                {
                    return mapPacket;
                }
                else
                {
                    mapPacket.MapItems = GenerateMapItemsPacket(client.Entity, mapId);
                    mapPacket.MapEntities = GenerateMapEntitiesPacket(mapId, client.Entity);

                    return mapPacket;
                }
            }
        }

        //MapPacket
        public static void SendMap(Client client, Guid mapId, bool allEditors = false)
        {
            var sentMaps = client?.SentMaps;
            if (sentMaps == null)
            {
                return;
            }

            var map = MapInstance.Get(mapId);

            if (map == null)
                return;

            if (!client.IsEditor)
            {
                if (sentMaps.TryGetValue(mapId, out var sentMap))
                {
                    if (sentMap.Item1 > Globals.Timing.Milliseconds && sentMap.Item2 == map.Revision)
                    {
                        return;
                    }

                    client.SentMaps.Remove(mapId);
                }

                try
                {
                    sentMaps.Add(mapId, new Tuple<long, int>(Globals.Timing.Milliseconds + 5000, map.Revision));
                }
                catch (Exception exception)
                {
                    Log.Error($"Current Map #: {mapId}");
                    Log.Error($"# Sent maps: {sentMaps.Count}");
                    Log.Error($"# Maps: {MapInstance.Lookup.Count}");
                    Log.Error(exception);

                    throw;
                }
            }

            if (client.IsEditor)
            {
                if (allEditors)
                {
                    SendDataToEditors(GenerateMapPacket(client, mapId));
                }
                else
                {
                    client.Send(GenerateMapPacket(client, mapId));
                }
            }
            else
            {
                client.Send(GenerateMapPacket(client, mapId));
                var entity = client.Entity;
                if (entity != null)
                {
                    //TODO: INCLUDE EVENTS IN MAP PACKET
                    if (mapId == entity.MapId)
                    {
                        entity.SendEvents();
                    }
                }

                //TODO - Include Aggression and Equipment in ENTITY DATA PACKETS!
                //SendMapEntityEquipmentTo(client, sendEntities); //Send the equipment of each player

                //for (var i = 0; i < sendEntities.Count; i++)
                //{
                //    if (sendEntities[i].GetType() == typeof(Npc))
                //    {
                //        SendNpcAggressionTo(client.Entity, (Npc)sendEntities[i]);
                //    }
                //}
            }
        }

        //MapPacket
        public static void SendMapToEditors(Guid mapId)
        {
            MapPacket packet = null;
            var map = MapInstance.Get(mapId);
            if (map == null)
            {
                packet = new MapPacket(mapId, true);
            }
            else
            {
                packet = new MapPacket(
                    mapId, false, map.JsonData, map.TileData, map.AttributeData, map.Revision, map.MapGridX,
                    map.MapGridY
                );
            }

            SendDataToEditors(packet);
        }

        //MapEntitiesPacket
        public static MapEntitiesPacket GenerateMapEntitiesPacket(Guid mapId, Player forPlayer = null)
        {
            var map = MapInstance.Get(mapId);
            if (map != null)
            {
                var entities = map.GetEntities(false);
                var sendEntities = new List<Entity>();
                for (var i = 0; i < entities.Count; i++)
                {
                    if (entities[i] != null)
                    {
                        sendEntities.Add(entities[i]);
                    }
                }

                var enPackets = new List<EntityPacket>();
                for (var i = 0; i < sendEntities.Count; i++)
                {
                    enPackets.Add(sendEntities[i].EntityPacket(null, forPlayer));
                }

                return new MapEntitiesPacket(enPackets.ToArray());
            }

            return null;
        }

        //EntityPacket
        public static void SendEntityDataTo(Player player, Entity en)
        {
            if (en == null)
            {
                return;
            }

            var packet = en.EntityPacket(null, player);
            packet.IsSelf = en == player;

            player.SendPacket(packet);

            if (en == player)
            {
                SendExperience(player);
                SendInventory(player);
                SendPlayerSpells(player);
                SendPointsTo(player);
                SendHotbarSlots(player);
                SendQuestsProgress(player);
                SendItemCooldowns(player);
                SendSpellCooldowns(player);
            }

            //If a player, send equipment to all (for paperdolls)
            if (en.GetType() == typeof(Player))
            {
                SendPlayerEquipmentTo(player, (Player) en);
            }

            if (en.GetType() == typeof(Npc))
            {
                SendNpcAggressionTo(player, (Npc) en);
            }
        }

        //MapEntitiesPacket
        public static void SendMapEntitiesTo(Player player, ConcurrentDictionary<Guid, Entity> entities)
        {
            var sendEntities = new List<Entity>();

            foreach (var en in entities)
            {
                if (en.Value != null && en.Value != player)
                {
                    sendEntities.Add(en.Value);
                }
            }

            var enPackets = new List<EntityPacket>();
            for (var i = 0; i < sendEntities.Count; i++)
            {
                enPackets.Add(sendEntities[i].EntityPacket(null, player));
            }

            player.SendPacket(new MapEntitiesPacket(enPackets.ToArray()));

            SendMapEntityEquipmentTo(player, sendEntities); //Send the equipment of each player

            for (var i = 0; i < sendEntities.Count; i++)
            {
                if (sendEntities[i].GetType() == typeof(Npc))
                {
                    SendNpcAggressionTo(player, (Npc) sendEntities[i]);
                }
            }
        }

        public static void SendMapEntityEquipmentTo(Player player, List<Entity> entities)
        {
            for (var i = 0; i < entities.Count; i++)
            {
                if (entities[i] != null && entities[i] != player)
                {
                    //If a player, send equipment to all (for paperdolls)
                    if (entities[i].GetType() == typeof(Player) && player != entities[i])
                    {
                        SendPlayerEquipmentTo(player, (Player) entities[i]);
                    }
                }
            }
        }

        //EntityDataPacket
        public static void SendEntityDataToProximity(Entity en, Player except = null)
        {
            if (en == null)
            {
                return;
            }

            if (en is Projectile)
            {
                SendDataToProximity(en.MapId, en.EntityPacket(null, null), null, TransmissionMode.All);
            }
            else
            {
                foreach (var map in en.Map.GetSurroundingMaps(true))
                {
                    foreach (var player in map.GetPlayersOnMap())
                    {
                        if (player != except)
                        {
                            SendEntityDataTo(player, en);
                        }
                    }
                }
            }

            SendEntityStats(en);

            //If a player, send equipment to all (for paperdolls)
            if (en.GetType() == typeof(Player))
            {
                SendPlayerEquipmentToProximity((Player) en);
            }

            if (en.GetType() == typeof(Npc))
            {
                SendNpcAggressionToProximity((Npc) en);
            }
        }

        //EntityDataPacket
        public static void SendEntityDataToMap(Entity en, MapInstance map, Player except = null)
        {
            if (en == null)
            {
                return;
            }

            if (en is Projectile)
            {
                SendDataToMap(map.Id, en.EntityPacket(null, null), null, TransmissionMode.All);
            }
            else
            {
                foreach (var player in map.GetPlayersOnMap())
                {
                    if (player != except)
                    {
                        SendEntityDataTo(player, en);
                    }
                }
            }

            SendEntityStats(en);

            //If a player, send equipment to all (for paperdolls)
            if (en.GetType() == typeof(Player))
            {
                SendPlayerEquipmentToProximity((Player) en);
            }

            if (en.GetType() == typeof(Npc))
            {
                SendNpcAggressionToProximity((Npc) en);
            }
        }

        //EntityPositionPacket
        public static void SendEntityPositionTo(Client client, Entity en)
        {
            if (en == null)
            {
                return;
            }

            client.Send(
                new EntityPositionPacket(
                    en.Id, en.GetEntityType(), en.MapId, (byte) en.X, (byte) en.Y, (byte) en.Dir, en.Passable,
                    en.HideName
                )
            );
        }

        //EntityPositionPacket
        public static void SendEntityPositionToAll(Entity en)
        {
            if (en == null)
            {
                return;
            }

            SendDataToProximity(
                en.MapId,
                new EntityPositionPacket(
                    en.Id, en.GetEntityType(), en.MapId, (byte) en.X, (byte) en.Y, (byte) en.Dir, en.Passable,
                    en.HideName
                )
            );
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

        //NpcAggressionPacket
        public static void SendNpcAggressionTo(Player player, Npc npc)
        {
            if (player == null || npc == null)
            {
                return;
            }

            player.SendPacket(new NpcAggressionPacket(npc.Id, npc.GetAggression(player)), TransmissionMode.Any);
        }

        //EntityLeftArea
        public static void SendEntityLeaveMap(Entity en, Guid leftMap)
        {
            SendDataToMap(leftMap, new EntityLeftPacket(en.Id, en.GetEntityType(), en.MapId));
        }

        //EntityLeftPacket
        public static void SendEntityLeave(Entity en)
        {
            SendDataToProximity(en.MapId, new EntityLeftPacket(en.Id, en.GetEntityType(), en.MapId));
        }

        //EntityLeavePacket
        public static void SendEntityLeaveTo(Player player, Entity en)
        {
            player.SendPacket(new EntityLeftPacket(en.Id, en.GetEntityType(), en.MapId));
        }

        //EventLeavePacket
        public static void SendEntityLeaveTo(Player player, Event evt)
        {
            player.SendPacket(new EntityLeftPacket(evt.Id, EntityTypes.Event, evt.MapId));
        }

        /// <summary>
        /// Sends a chat message to the specified player.
        /// </summary>
        /// <param name="player">The player to which to send a message.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="type">The type of message we are sending.</param>
        /// <param name="target">The sender of this message, should we decide to respond from the client.</param>
        public static void SendChatMsg(Player player, string message, ChatMessageType type, string target = "")
        {
            SendChatMsg(player, message, type, CustomColors.Chat.PlayerMsg, target);
        }

        /// <summary>
        /// Sends a chat message to the specified player.
        /// </summary>
        /// <param name="player">The player to which to send a message.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="type">The type of message we are sending.</param>
        /// <param name="color">The color assigned to this message.</param>
        /// <param name="target">The sender of this message, should we decide to respond from the client.</param>
        public static void SendChatMsg(Player player, string message, ChatMessageType type, Color color, string target = "")
        {
            if (player == null)
            {
                return;
            }

            player.SendPacket(new ChatMsgPacket(message, type, color, target), TransmissionMode.All);
        }

        //GameDataPacket
        public static void SendGameData(Client client)
        {
            if (client == null)
            {
                return;
            }

            //If this is an editor, send the shops, events, variables, etc
            if (client.IsEditor)
            {
                foreach (var val in Enum.GetValues(typeof(GameObjectType)))
                {
                    if ((GameObjectType)val == GameObjectType.Map)
                    {
                        continue;
                    }

                    if (((GameObjectType)val == GameObjectType.Shop ||
                         (GameObjectType)val == GameObjectType.Event ||
                         (GameObjectType)val == GameObjectType.PlayerVariable ||
                         (GameObjectType)val == GameObjectType.ServerVariable ||
                         (GameObjectType)val == GameObjectType.GuildVariable))
                    {
                        SendGameObjects(client, (GameObjectType)val, null);
                    }
                }
            }

            //Now send the cached game data that we send to all clients
            client.Send(CachedGameDataPacket);
        }

        //GameDataPacket
        public static void CacheGameDataPacket()
        {
            var gameObjects = new List<GameObjectPacket>();

            //Send massive amounts of game data
            foreach (var val in Enum.GetValues(typeof(GameObjectType)))
            {
                if ((GameObjectType) val == GameObjectType.Map)
                {
                    continue;
                }

                if ((GameObjectType) val == GameObjectType.Shop ||
                    (GameObjectType) val == GameObjectType.Event ||
                    (GameObjectType) val == GameObjectType.PlayerVariable ||
                    (GameObjectType) val == GameObjectType.ServerVariable ||
                    (GameObjectType)val == GameObjectType.GuildVariable)
                {
                    continue;
                }

                SendGameObjects(null, (GameObjectType) val, gameObjects);
            }

            CachedGameDataPacket = new GameDataPacket(gameObjects.ToArray(), CustomColors.Json());
        }

        /// <summary>
        /// Sends a global chat message to every user online.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="target">The sender of this message, should we decide to respond from the client.</param>
        public static void SendGlobalMsg(string message, string target = "")
        {
            SendGlobalMsg(message, CustomColors.Chat.AnnouncementChat, target);
        }

        /// <summary>
        /// Sends a global chat message to every user online.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The color assigned to this message.</param>
        /// <param name="target">The sender of this message, should we decide to respond from the client.</param>
        /// <param name="type">The type of message we are sending.</param>
        public static void SendGlobalMsg(string message, Color color, string target = "", ChatMessageType type = ChatMessageType.Global)
        {
            SendDataToAllPlayers(new ChatMsgPacket(message, type, color, target));
        }

        /// <summary>
        /// Sends a chat message to the proximity of a specified map.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="type">The type of message we are sending.</param>
        /// <param name="mapId">The Map we are sending this message to (and its surroundings).</param>
        /// <param name="target">The sender of this message, should we decide to respond from the client.</param>
        /// <returns>Returns whether or not the message was sent successfully.</returns>
        public static bool SendProximityMsg(string message, ChatMessageType type, Guid mapId, string target = "")
        {
            return SendProximityMsg(message, type, mapId, CustomColors.Chat.ProximityMsg);
        }

        /// <summary>
        /// Sends a chat message to the proximity of a specified map.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="type">The type of message we are sending.</param>
        /// <param name="mapId">The Map we are sending this message to (and its surroundings).</param>
        /// <param name="color">The color assigned to this message.</param>
        /// <param name="target">The sender of this message, should we decide to respond from the client.</param>
        /// <returns></returns>
        public static bool SendProximityMsg(string message, ChatMessageType type, Guid mapId, Color color, string target = "")
        {
            return SendDataToProximity(mapId, new ChatMsgPacket(message, type, color, target));
        }

        /// <summary>
        /// Sends a message to all online staff members.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The color assigned to this message.</param>
        /// <param name="target">The sender of this message, should we decide to respond from the client.</param>
        public static void SendAdminMsg(string message, Color color, string target = "")
        {
            foreach (var player in Globals.OnlineList)
            {
                if (player != null)
                {
                    if (player.Power != UserRights.None)
                    {
                        SendChatMsg(player, message, ChatMessageType.Admin , color, target);
                    }
                }
            }
        }

        /// <summary>
        /// Sends a message to our player's party.
        /// </summary>
        /// <param name="player">The player of which to send a message to their party.</param>
        /// <param name="message">The message to send.</param>
        /// <param name="color">The color assigned to this message.</param>
        /// <param name="target">The sender of this message, should we decide to respond from the client.</param>
        public static void SendPartyMsg(Player player, string message, Color color, string target = "")
        {
            foreach (var p in player.Party)
            {
                if (p != null)
                {
                    SendChatMsg(p, message, ChatMessageType.Party , color, target);
                }
            }
        }

        //ProjectileDeadPacket
        public static void SendRemoveProjectileSpawns(Guid mapId, Guid[] projDeaths, KeyValuePair<Guid, int>[] spawnDeaths)
        {
            SendDataToProximity(mapId, new ProjectileDeadPacket(projDeaths, spawnDeaths));
        }

        //EntityMovePacket
        public static void SendEntityMove(Entity en, bool correction = false)
        {
            var map = en?.Map;
            if (map != null)
            {
                if (en is Player && !Options.Instance.Packets.BatchPlayerMovementPackets)
                {
                    SendDataToProximity(
                        en.MapId,
                        new EntityMovePacket(
                            en.Id, en.GetEntityType(), en.MapId, (byte)en.X, (byte)en.Y, (byte)en.Dir, correction
                        ), null, TransmissionMode.Any
                    );
                    return;
                }
                map.AddBatchedMovement(en, correction, null);
            }
        }

        //EntityMovePacket
        public static void SendEntityMoveTo(Player player, Entity en, bool correction = false)
        {
            var map = en?.Map;
            if (map != null)
            {
                if (en is Player && !Options.Instance.Packets.BatchPlayerMovementPackets)
                {
                    player.SendPacket(
                        new EntityMovePacket(
                            en.Id, en.GetEntityType(), en.MapId, (byte)en.X, (byte)en.Y, (byte)en.Dir, correction
                        )
                    );
                    return;
                }
                map.AddBatchedMovement(en, correction, player);
                return;
            }
        }

        //EntityVitalsPacket
        public static EntityVitalsPacket GenerateEntityVitalsPacket(Entity en)
        {
            return new EntityVitalsPacket(
                en.Id, en.GetEntityType(), en.MapId, en.GetVitals(), en.GetMaxVitals(), en.StatusPackets(),
                en.CombatTimer - Globals.Timing.Milliseconds
            );
        }

        //EntityVitalsPacket
        public static void SendMapEntityVitalUpdate(MapInstance map, Entity[] entities)
        {
            // Generate a list of vitals to send to our users!
            var data = new List<EntityVitalData>();
            foreach (var entity in entities)
            {
                data.Add(new EntityVitalData()
                {
                    Id = entity.Id,
                    Type = entity.GetEntityType(),
                    Vitals = entity.GetVitals(),
                    MaxVitals = entity.GetMaxVitals(),
                    CombatTimeRemaining = entity.CombatTimer - Timing.Global.Milliseconds
                });
            }

            // Send the data to the surroundings!
            SendDataToProximity(map.Id, new MapEntityVitalsPacket(map.Id, data.ToArray()));
        }

        public static void SendMapEntityStatusUpdate(MapInstance map, Entity[] entities)
        {
            // Generate a list of statuses to send to our users!
            var data = new List<EntityStatusData>();
            foreach (var entity in entities)
            {
                data.Add(new EntityStatusData()
                {
                    Id = entity.Id,
                    Type = entity.GetEntityType(),
                    Statuses = entity.StatusPackets()
                });
            }

            // Send the data to the surroundings!
            SendDataToProximity(map.Id, new MapEntityStatusPacket(map.Id, data.ToArray()));
        }

        //EntityStatsPacket
        public static void SendEntityStats(Entity en)
        {
            if (en == null || en is EventPageInstance || en is Projectile || en is Resource)
            {
                return;
            }

            SendDataToProximity(en.MapId, GenerateEntityStatsPacket(en), null, TransmissionMode.Any);
        }

        //EntityVitalsPacket
        public static void SendEntityVitalsTo(Client client, Entity en)
        {
            if (en == null || en is EventPageInstance || en is Projectile)
            {
                return;
            }

            client.Send(GenerateEntityVitalsPacket(en), TransmissionMode.Any);
        }

        //EntityStatsPacket
        public static EntityStatsPacket GenerateEntityStatsPacket(Entity en)
        {
            var stats = new int[(int) Stats.StatCount];
            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                stats[i] = en.Stat[i].Value();
            }

            return new EntityStatsPacket(en.Id, en.GetEntityType(), en.MapId, stats);
        }

        //EntityStatsPacket
        public static void SendEntityStatsTo(Client client, Entity en)
        {
            client.Send(GenerateEntityStatsPacket(en), TransmissionMode.Any);
        }

        //EntityDirectionPacket
        public static void SendEntityDir(Entity en)
        {
            SendDataToProximity(
                en.MapId, new EntityDirectionPacket(en.Id, en.GetEntityType(), en.MapId, (byte) en.Dir), null, TransmissionMode.Any
            );
        }

        //EntityAttackPacket
        public static void SendEntityAttack(Entity en, int attackTime)
        {
            SendDataToProximity(en.MapId, new EntityAttackPacket(en.Id, en.GetEntityType(), en.MapId, attackTime), null, TransmissionMode.Any);
        }

        //EntityDiePacket
        public static void SendEntityDie(Entity en)
        {
            SendDataToProximity(en.MapId, new EntityDiePacket(en.Id, en.GetEntityType(), en.MapId));
        }

        //EntityDirectionPacket
        public static void SendEntityDirTo(Player player, Entity en)
        {
            player.SendPacket(new EntityDirectionPacket(en.Id, en.GetEntityType(), en.MapId, (byte) en.Dir), TransmissionMode.Any);
        }

        //EventDialogPacket
        public static void SendEventDialog(Player player, string prompt, string face, Guid eventId)
        {
            player.SendPacket(new EventDialogPacket(eventId, prompt, face, 0, null));
        }

        //EventDialogPacket
        public static void SendEventDialog(
            Player player,
            string prompt,
            string opt1,
            string opt2,
            string opt3,
            string opt4,
            string face,
            Guid eventId
        )
        {
            player.SendPacket(new EventDialogPacket(eventId, prompt, face, 1, new string[4] {opt1, opt2, opt3, opt4}));
        }

        public static void SendInputVariableDialog(
            Player player,
            string title,
            string prompt,
            VariableDataTypes type,
            Guid eventId
        )
        {
            player.SendPacket(new InputVariablePacket(eventId, title, prompt, type));
        }

        //MapListPacket
        public static void SendMapList(Client client)
        {
            client.Send(new MapListPacket(MapList.List.JsonData));
        }

        //MapListPacket
        public static void SendMapListToAll()
        {
            SendDataToAll(new MapListPacket(MapList.List.JsonData));
        }

        //ErrorPacket
        public static void SendError(Client client, string error, string header = "")
        {
            client.Send(new ErrorMessagePacket(header, error));
        }

        //MapItemsPacket
        public static MapItemsPacket GenerateMapItemsPacket(Player player, Guid mapId)
        {
            var map = MapInstance.Get(mapId);

            var items = new List<MapItemUpdatePacket>();

            // Generate our data to be send to the client.
            foreach (var item in map.AllMapItems.Values)
            {
                if (item.VisibleToAll || item.Owner == player?.Id)
                {
                    items.Add(new MapItemUpdatePacket(mapId, item.TileIndex, item.UniqueId, item.ItemId, item.BagId, item.Quantity, item.StatBuffs));
                }
            }

            return new MapItemsPacket(mapId, items.ToArray());
        }

        //MapItemsPacket
        public static void SendMapItems(Player player, Guid mapId)
        {
            player.SendPacket(GenerateMapItemsPacket(player, mapId));
        }

        //MapItemsPacket
        public static void SendMapItemsToProximity(Guid mapId)
        {
            var map = MapInstance.Get(mapId);
            if (map == null)
            {
                return;
            }

            // Collect a list of all players in the surrounding.
            var playerList = new List<Player>();
            playerList.AddRange(map.GetPlayersOnMap());

            foreach (var surrMap in map.SurroundingMaps)
            {
                playerList.AddRange(surrMap.GetPlayersOnMap());
            }

            // Send them all a map item update.
            foreach(var player in playerList)
            {
                player.SendPacket(GenerateMapItemsPacket(player, mapId));
            }
        }

        /// <summary>
        /// Send a map item update to the relevant players.
        /// </summary>
        /// <param name="mapId">The Id of the <see cref="MapInstance"/> we are sending the item update for.</param>
        /// <param name="uniqueId">The Id for the <see cref="MapItem"/> we are sending the item update for.</param>
        /// <param name="itemRef">The map item that we are sending (or null if removing), passing this saves us a lookup for it.</param>
        /// <param name="sendToAll">If we are removing the item from the map, do we send this data to everyone?</param>
        /// <param name="owner">The previous owner of an item being removed when the data is not send to everyone.</param>
        public static void SendMapItemUpdate(Guid mapId, MapItem itemRef, bool removing, bool sendToAll = true, Guid owner = new Guid())
        {
            var map = MapInstance.Get(mapId);

            // Does the item exist? If not, send a delete notification. If it does, send an update.
            if (removing)
            {
                // Are we to send the removal to all players?
                if (sendToAll)
                {
                    SendDataToProximity(mapId, new MapItemUpdatePacket(mapId, itemRef.TileIndex, itemRef.UniqueId));
                }
                else
                {
                    // Nope, just to its owner!
                    var player = Player.FindOnline(owner);
                    if (player != null)
                    {
                        player.SendPacket(new MapItemUpdatePacket(mapId, itemRef.TileIndex, itemRef.UniqueId));
                    }
                    else
                    {
                        // Uh, our player doesn't exist.. send it to everyone anyway.
                        SendDataToProximity(mapId, new MapItemUpdatePacket(mapId, itemRef.TileIndex, itemRef.UniqueId));
                    }
                }
                
            }
            else
            {
                // Is the item owned? If so, only send to that particular player.
                if (!itemRef.VisibleToAll)
                {
                    var player = Player.FindOnline(itemRef.Owner);
                    if (player != null)
                    {
                        player.SendPacket(new MapItemUpdatePacket(mapId, itemRef.TileIndex, itemRef.UniqueId, itemRef.ItemId, itemRef.BagId, itemRef.Quantity, itemRef.StatBuffs));
                    }
                }
                else
                {
                    SendDataToProximity(mapId, new MapItemUpdatePacket(mapId, itemRef.TileIndex, itemRef.UniqueId, itemRef.ItemId, itemRef.BagId, itemRef.Quantity, itemRef.StatBuffs));
                }
            }
        }


        //InventoryPacket
        public static void SendInventory(Player player)
        {
            if (player == null)
            {
                return;
            }

            var invItems = new InventoryUpdatePacket[Options.MaxInvItems];
            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                invItems[i] = new InventoryUpdatePacket(
                    i, player.Items[i].ItemId, player.Items[i].Quantity, player.Items[i].BagId,
                    player.Items[i].StatBuffs
                );
            }

            player.SendPacket(new InventoryPacket(invItems));
        }

        //InventoryUpdatePacket
        public static void SendInventoryItemUpdate(Player player, int slot)
        {
            if (player == null)
            {
                return;
            }

            player.SendPacket(
                new InventoryUpdatePacket(
                    slot, player.Items[slot].ItemId, player.Items[slot].Quantity, player.Items[slot].BagId,
                    player.Items[slot].StatBuffs
                )
            );
        }

        //SpellsPacket
        public static void SendPlayerSpells(Player player)
        {
            if (player == null)
            {
                return;
            }

            var spells = new SpellUpdatePacket[Options.MaxPlayerSkills];
            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                spells[i] = new SpellUpdatePacket(i, player.Spells[i].SpellId);
            }

            player.SendPacket(new SpellsPacket(spells));
        }

        //SpellUpdatePacket
        public static void SendPlayerSpellUpdate(Player player, int slot)
        {
            if (player == null)
            {
                return;
            }

            player.SendPacket(new SpellUpdatePacket(slot, player.Spells[slot].SpellId));
        }

        //EquipmentPacket
        public static EquipmentPacket GenerateEquipmentPacket(Player forPlayer, Player en)
        {
            if (forPlayer != null && forPlayer == en)
            {
                return new EquipmentPacket(en.Id, en.Equipment, null);
            }
            else
            {
                var equipment = new Guid[Options.EquipmentSlots.Count];

                for (var i = 0; i < Options.EquipmentSlots.Count; i++)
                {
                    if (en.Equipment[i] == -1 || en.Items[en.Equipment[i]].ItemId == Guid.Empty)
                    {
                        equipment[i] = Guid.Empty;
                    }
                    else
                    {
                        equipment[i] = en.Items[en.Equipment[i]].ItemId;
                    }

                }

                return new EquipmentPacket(en.Id, null, equipment);
            }
        }

        //EquipmentPacket
        public static void SendPlayerEquipmentTo(Player forPlayer, Player en)
        {
            forPlayer.SendPacket(GenerateEquipmentPacket(forPlayer, en), TransmissionMode.Any);
        }

        //EquipmentPacket
        public static void SendPlayerEquipmentToProximity(Player en)
        {
            SendDataToProximity(en.MapId, GenerateEquipmentPacket(null, en), null, TransmissionMode.Any);
            SendPlayerEquipmentTo(en, en);
        }

        //StatPointsPacket
        public static void SendPointsTo(Player player)
        {
            player.SendPacket(new StatPointsPacket(player.StatPoints), TransmissionMode.Any);
        }

        //HotbarPacket
        public static void SendHotbarSlots(Player player)
        {
            var hotbarData = new string[Options.MaxHotbar];
            for (var i = 0; i < Options.MaxHotbar; i++)
            {
                hotbarData[i] = player.Hotbar[i].Data();
            }

            player.SendPacket(new HotbarPacket(hotbarData));
        }

        //CreateCharacterPacket
        public static void SendCreateCharacter(Client client)
        {
            client.Send(new CharacterCreationPacket());
        }

        //CharactersPacket
        public static void SendPlayerCharacters(Client client)
        {
            var characters = new List<CharacterPacket>();
            if (client.User == null)
            {
                return;
            }

            if (client.Characters.Count > 0)
            {
                foreach (var character in client.Characters.OrderByDescending(p => p.LastOnline))
                {
                    var equipmentArray = character.Equipment;
                    var equipment = new string[Options.EquipmentSlots.Count + 1];

                    //Draw the equipment/paperdolls
                    for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
                    {
                        if (Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z]) > -1)
                        {
                            if (equipmentArray[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] > -1 &&
                                equipmentArray[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] <
                                Options.MaxInvItems)
                            {
                                var itemId = character
                                    .Items[equipmentArray[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])]]
                                    .ItemId;

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

                    characters.Add(
                        new CharacterPacket(
                            character.Id, character.Name, character.Sprite, character.Face, character.Level,
                            ClassBase.GetName(character.ClassId), equipment
                        )
                    );
                }
            }

            client.Send(
                new CharactersPacket(characters.ToArray(), client.Characters.Count < Options.MaxCharacters)
            );
        }

        //AdminPanelPacket
        public static void SendOpenAdminWindow(Client client)
        {
            client.Send(new AdminPanelPacket(), TransmissionMode.Any);
        }

        //MapGridPacket
        public static void SendMapGridToAll(int gridId)
        {
            SendMapGridToAll(DbInterface.GetGrid(gridId));
        }
        public static void SendMapGridToAll(MapGrid grid)
        {
            if (grid == null) return;
            for (var i = 0; i < Globals.Clients.Count; i++)
            {
                if (Globals.Clients[i] != null)
                {
                    if (Globals.Clients[i].IsEditor)
                    {
                        if (grid.HasMap(Globals.Clients[i].EditorMap))
                        {
                            SendMapGrid(Globals.Clients[i], grid);
                        }
                    }
                    else
                    {
                        if (Globals.Clients[i].Entity != null)
                        {
                            if (grid.HasMap(Globals.Clients[i].Entity.MapId))
                            {
                                SendMapGrid(Globals.Clients[i], grid, true);
                            }
                        }
                    }
                }
            }
        }

        //MapGridPacket
        public static void SendMapGrid(Client client, int gridId, bool clearKnownMaps = false)
        {
            var grid = DbInterface.GetGrid(gridId);
            SendMapGrid(client,grid,clearKnownMaps);
        }
        public static void SendMapGrid(Client client, MapGrid grid, bool clearKnownMaps = false)
        {
            if (client == null || grid == null)
            {
                return;
            }

            if (clearKnownMaps)
            {
                client.SentMaps.Clear();
            }

            if (client.IsEditor)
            {
                client.Send(new MapGridPacket(null, grid.GetEditorData(), clearKnownMaps));
            }
            else
            {
                client.Send(new MapGridPacket(grid.GetClientData(), null, clearKnownMaps));
                if (clearKnownMaps)
                {
                    SendAreaPacket(client.Entity);
                }
            }
        }

        //SpellCastPacket
        public static void SendEntityCastTime(Entity en, Guid spellId)
        {
            SendDataToProximity(en.MapId, new SpellCastPacket(en.Id, spellId), null, TransmissionMode.Any);
        }

        //CancelCastPacket
        public static void SendEntityCancelCast(Entity en)
        {
            if (en == null || en is EventPageInstance || en is Projectile)
            {
                return;
            }

            SendDataToProximity(en.MapId, new CancelCastPacket(en.Id), null, TransmissionMode.Any);
        }

        //SpellCooldownPacket
        public static void SendSpellCooldown(Player player, Guid spellId)
        {
            if (player.SpellCooldowns.ContainsKey(spellId))
            {
                var cds = new Dictionary<Guid, long>();
                cds.Add(spellId, player.SpellCooldowns[spellId] - Globals.Timing.MillisecondsUTC);
                player.SendPacket(new SpellCooldownPacket(cds), TransmissionMode.All);
            }
        }

        public static void SendSpellCooldowns(Player player)
        {
            if (player.SpellCooldowns.Count > 0)
            {
                var cds = new Dictionary<Guid, long>();
                foreach (var cd in player.SpellCooldowns)
                {
                    cds.Add(cd.Key, cd.Value - Globals.Timing.MillisecondsUTC);
                }

                player.SendPacket(new SpellCooldownPacket(cds), TransmissionMode.All);
            }
        }

        //ItemCooldownPacket
        public static void SendItemCooldown(Player player, Guid itemId)
        {
            if (player.ItemCooldowns.ContainsKey(itemId))
            {
                var cds = new Dictionary<Guid, long>();
                cds.Add(itemId, player.ItemCooldowns[itemId] - Globals.Timing.MillisecondsUTC);
                player.SendPacket(new ItemCooldownPacket(cds), TransmissionMode.All);
            }
        }

        public static void SendItemCooldowns(Player player)
        {
            if (player.ItemCooldowns.Count > 0)
            {
                var cds = new Dictionary<Guid, long>();
                foreach (var cd in player.ItemCooldowns)
                {
                    cds.Add(cd.Key, cd.Value - Globals.Timing.MillisecondsUTC);
                }

                player.SendPacket(new ItemCooldownPacket(cds), TransmissionMode.All);
            }
        }

        //ExperiencePacket
        public static void SendExperience(Player player)
        {
            player.SendPacket(new ExperiencePacket(player.Exp, player.ExperienceToNextLevel), TransmissionMode.Any);
        }

        //PlayAnimationPacket
        public static void SendAnimationToProximity(
            Guid animId,
            int targetType,
            Guid entityId,
            Guid mapId,
            byte x,
            byte y,
            sbyte direction
        )
        {
            var map = MapInstance.Get(mapId);
            if (map != null)
            {
                if (Options.Instance.Packets.BatchAnimationPackets)
                {
                    map.AddBatchedAnimation(new PlayAnimationPacket(animId, targetType, entityId, mapId, x, y, direction));
                }
                else
                {
                    SendDataToProximity(mapId, new PlayAnimationPacket(animId, targetType, entityId, mapId, x, y, direction), null, TransmissionMode.Any);
                }
            }
        }

        //HoldPlayerPacket
        public static void SendHoldPlayer(Player player, Guid eventId, Guid mapId)
        {
            player.SendPacket(new HoldPlayerPacket(eventId, mapId, false));
        }

        //HoldPlayerPacket
        public static void SendReleasePlayer(Player player, Guid eventId)
        {
            player.SendPacket(new HoldPlayerPacket(eventId, Guid.Empty, true));
        }

        //PlayMusicPacket
        public static void SendPlayMusic(Player player, string bgm)
        {
            player.SendPacket(new PlayMusicPacket(bgm));
        }

        //StopMusicPacket
        public static void SendFadeMusic(Player player)
        {
            player.SendPacket(new StopMusicPacket());
        }

        //PlaySoundPacket
        public static void SendPlaySound(Player player, string sound)
        {
            player.SendPacket(new PlaySoundPacket(sound));
        }

        //StopSoundPacket
        public static void SendStopSounds(Player player)
        {
            player.SendPacket(new StopSoundsPacket());
        }

        //ShowPicturePacket
        public static void SendShowPicture(Player player, string picture, int size, bool clickable, int hideTime, Guid eventId)
        {
            player.SendPacket(new ShowPicturePacket(picture, size, clickable, hideTime, eventId));
        }

        //HidePicturePacket
        public static void SendHidePicture(Player player)
        {
            player.SendPacket(new HidePicturePacket());
        }

        //ShopPacket
        public static void SendOpenShop(Player player, ShopBase shop)
        {
            if (shop == null)
            {
                return;
            }

            player.SendPacket(new ShopPacket(shop.JsonData, false));
        }

        //ShopPacket
        public static void SendCloseShop(Player player)
        {
            player.SendPacket(new ShopPacket(null, true));
        }

        //CraftingTablePacket
        public static void SendOpenCraftingTable(Player player, CraftingTableBase table)
        {
            if (table != null)
            {
                player.SendPacket(new CraftingTablePacket(table.JsonData, false));
            }
        }

        //CraftingTablePacket
        public static void SendCloseCraftingTable(Player player)
        {
            player.SendPacket(new CraftingTablePacket(null, true));
        }

        //GameObjectPacket
        public static void SendGameObjects(Client client, GameObjectType type, List<GameObjectPacket> packetList = null)
        {
            switch (type)
            {
                case GameObjectType.Animation:
                    foreach (var obj in AnimationBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.Class:
                    foreach (var obj in ClassBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.Item:
                    foreach (var obj in ItemBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.Npc:
                    foreach (var obj in NpcBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.Projectile:
                    foreach (var obj in ProjectileBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.Quest:
                    foreach (var obj in QuestBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.Resource:
                    foreach (var obj in ResourceBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.Shop:
                    foreach (var obj in ShopBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.Spell:
                    foreach (var obj in SpellBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.CraftTables:
                    foreach (var obj in CraftingTableBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.Crafts:
                    foreach (var obj in CraftBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.Map:
                    throw new Exception("Maps are not sent as batches, use the proper send map functions");
                case GameObjectType.Event:
                    foreach (var obj in EventBase.Lookup)
                    {
                        if (((EventBase) obj.Value).CommonEvent)
                        {
                            SendGameObject(client, obj.Value, false, false, packetList);
                        }
                    }

                    break;
                case GameObjectType.PlayerVariable:
                    foreach (var obj in PlayerVariableBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.ServerVariable:
                    foreach (var obj in ServerVariableBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.Tileset:
                    foreach (var obj in TilesetBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.Time:
                    break;
                case GameObjectType.GuildVariable:
                    foreach (var obj in GuildVariableBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        //GameObjectPacket
        public static void SendGameObject(
            Client client,
            IDatabaseObject obj,
            bool deleted = false,
            bool another = false,
            List<GameObjectPacket> packetList = null
        )
        {
            if (client == null && packetList == null || obj == null)
            {
                return;
            }

            if (client != null && client.IsEditor)
            {
                //If editor send quest events and map events
                if (obj.Type == GameObjectType.Quest)
                {
                    SendQuestEventsTo(client, (QuestBase) obj);
                }
            }

            if (packetList == null)
            {
                client.Send(
                    new GameObjectPacket(obj.Id, obj.Type, deleted ? null : obj.JsonData, deleted, another)
                );
            }
            else
            {
                packetList.Add(new GameObjectPacket(obj.Id, obj.Type, deleted ? null : obj.JsonData, deleted, another));
            }
        }

        //GameObjectPacket
        public static void SendQuestEventsTo(Client client, QuestBase qst)
        {
            SendEventIfExists(client, qst.StartEvent);
            SendEventIfExists(client, qst.EndEvent);
            foreach (var tsk in qst.Tasks)
            {
                SendEventIfExists(client, tsk.CompletionEvent);
            }
        }

        //GameObjectPacket
        public static void SendEventIfExists(Client client, EventBase evt)
        {
            if (evt != null && evt.Id != Guid.Empty)
            {
                SendGameObject(client, evt, false, false);
            }
        }

        //GameObjectPacket
        public static void SendGameObjectToAll(IDatabaseObject obj, bool deleted = false, bool another = false)
        {
            foreach (var client in Globals.Clients)
            {
                SendGameObject(client, obj, deleted, another);
            }
        }

        //OpenEditorPacket
        public static void SendOpenEditor(Client client, GameObjectType type)
        {
            client.Send(new OpenEditorPacket(type));
        }

        //EntityDashPacket
        public static void SendEntityDash(Entity en, Guid endMapId, byte endX, byte endY, int dashTime, sbyte direction)
        {
            SendDataToProximity(en.MapId, new EntityDashPacket(en.Id, endMapId, endX, endY, dashTime, direction));
        }

        /// <summary>
        /// Send a game announcement to all players.
        /// </summary>
        /// <param name="message">The message to send as an announcement.</param>
        /// <param name="duration">The duration (in milliseconds) for the message to display.</param>
        public static void SendGameAnnouncement(string message, long duration)
        {
             SendDataToAllPlayers(new AnnouncementPacket(message, duration));
        }

        //ActionMsgPacket
        public static void SendActionMsg(Entity en, string message, Color color)
        {
            var map = en?.Map;
            if (map == null)
            {
                return;
            }

            if (Options.Instance.Packets.BatchActionMessagePackets)
            {
                map.AddBatchedActionMessage(new ActionMsgPacket(en.MapId, en.X, en.Y, message, color));
            }
            else
            {
                SendDataToProximity(en.MapId, new ActionMsgPacket(en.MapId, en.X, en.Y, message, color));
            }
        }

        //EnterMapPacket
        public static void SendEnterMap(Client client, Guid mapId)
        {
            client.Send(new EnterMapPacket(mapId));
        }

        //TimeDataPacket
        public static void SendTimeBaseTo(Client client)
        {
            client.Send(new TimeDataPacket(TimeBase.GetTimeBase().GetInstanceJson()));
        }

        //TimeDataPacket
        public static void SendTimeBaseToAllEditors()
        {
            SendDataToEditors(new TimeDataPacket(TimeBase.GetTimeBase().GetInstanceJson()));
        }

        //TimePacket
        public static void SendTimeToAll()
        {
            SendDataToAllPlayers(
                new TimePacket(
                    Time.GetTime(), TimeBase.GetTimeBase().SyncTime ? 1 : TimeBase.GetTimeBase().Rate,
                    Time.GetTimeColor()
                )
            );
        }

        //TimePacket
        public static void SendTimeTo(Client client)
        {
            client?.Send(
                new TimePacket(
                    Time.GetTime(), TimeBase.GetTimeBase().SyncTime ? 1 : TimeBase.GetTimeBase().Rate,
                    Time.GetTimeColor()
                )
            );
        }

        //PartyPacket
        public static void SendParty(Player player)
        {
            var memberPackets = new PartyMemberPacket[player.Party.Count];
            for (var i = 0; i < player.Party.Count; i++)
            {
                var mem = player.Party[i];
                memberPackets[i] = new PartyMemberPacket(
                    mem.Id, mem.Name, mem.GetVitals(), mem.GetMaxVitals(), mem.Level
                );
            }

            player.SendPacket(new PartyPacket(memberPackets));
        }

        //PartyUpdatePacket
        public static void SendPartyUpdateTo(Player player, Player member)
        {
            var partyIndex = -1;
            for (var i = 0; i < player.Party.Count; i++)
            {
                if (player.Party[i] == member)
                {
                    partyIndex = i;
                }
            }

            if (partyIndex > -1)
            {
                player.SendPacket(
                    new PartyUpdatePacket(
                        partyIndex,
                        new PartyMemberPacket(
                            member.Id, member.Name, member.GetVitals(), member.GetMaxVitals(), member.Level
                        )
                    ), TransmissionMode.Any
                );
            }
        }

        //PartyInvitePacket
        public static void SendPartyInvite(Player player, Player leader)
        {
            player.SendPacket(new PartyInvitePacket(leader.Name, leader.Id));
        }

        //ChatBubblePacket
        public static void SendChatBubble(Guid entityId, EntityTypes type, string text, Guid mapId)
        {
            SendDataToProximity(mapId, new ChatBubblePacket(entityId, type, mapId, text), null, TransmissionMode.Any);
        }

        //QuestOfferPacket
        public static void SendQuestOffer(Player player, Guid questId)
        {
            player.SendPacket(new QuestOfferPacket(questId));
        }

        //QuestProgressPacket
        public static void SendQuestsProgress(Player player)
        {
            var dict = new Dictionary<Guid, string>();
            foreach (var quest in player.Quests)
            {
                dict.Add(quest.QuestId, quest.Data());
            }

            var hiddenQuests = new List<Guid>();
            foreach (var pair in QuestBase.Lookup)
            {
                var quest = (QuestBase)pair.Value;
                if (!player.Quests.Any(q => q.QuestId == quest.Id) && quest.DoNotShowUnlessRequirementsMet && !player.CanStartQuest(quest))
                {
                    hiddenQuests.Add(quest.Id);
                }
            }

            player.SendPacket(new QuestProgressPacket(dict, hiddenQuests.ToArray()));
        }

        //TradePacket
        public static void StartTrade(Player player, Player target)
        {
            player.SendPacket(new TradePacket(target.Name));
        }

        //TradeUpdatePacket
        public static void SendTradeUpdate(Player player, Player trader, int slot)
        {
            if (trader.Trading.Offer[slot] != null &&
                trader.Trading.Offer[slot].ItemId != Guid.Empty &&
                trader.Trading.Offer[slot].Quantity > 0)
            {
                player.SendPacket(
                    new TradeUpdatePacket(
                        trader.Id, slot, trader.Trading.Offer[slot].ItemId, trader.Trading.Offer[slot].Quantity,
                        trader.Trading.Offer[slot].BagId, trader.Trading.Offer[slot].StatBuffs
                    )
                );
            }
            else
            {
                player.SendPacket(new TradeUpdatePacket(trader.Id, slot, Guid.Empty, 0, null, null));
            }
        }

        //TradePacket
        public static void SendTradeClose(Player player)
        {
            player.SendPacket(new TradePacket(null));
        }

        //TradeRequestPacket
        public static void SendTradeRequest(Player player, Player partner)
        {
            player.SendPacket(new TradeRequestPacket(partner.Id, partner.Name));
        }

        //PlayerDeathPacket
        public static void SendPlayerDeath(Player en)
        {
            SendDataToProximity(en.MapId, new PlayerDeathPacket(en.Id));
        }

        //EntityZDimensionPacket
        public static void UpdateEntityZDimension(Entity en, byte z)
        {
            SendDataToProximity(en.MapId, new EntityZDimensionPacket(en.Id, z));
        }

        //BagPacket
        public static void SendOpenBag(Player player, int slots, Bag bag)
        {
            player.SendPacket(new BagPacket(slots, false));
            for (var i = 0; i < slots; i++)
            {
                SendBagUpdate(player, i, bag.Slots[i]);
            }
        }

        //BagUpdatePacket
        public static void SendBagUpdate(Player player, int slot, Item item)
        {
            if (item != null && item.ItemId != Guid.Empty && item.Quantity > 0)
            {
                player.SendPacket(new BagUpdatePacket(slot, item.ItemId, item.Quantity, item.BagId, item.StatBuffs));
            }
            else
            {
                player.SendPacket(new BagUpdatePacket(slot, Guid.Empty, 0, null, null));
            }
        }

        //BagPacket
        public static void SendCloseBag(Player player)
        {
            player.SendPacket(new BagPacket(0, true));
        }

        //MoveRoutePacket
        public static void SendMoveRouteToggle(Player player, bool routeOn)
        {
            player.SendPacket(new MoveRoutePacket(routeOn));
        }

        //FriendsPacket
        public static void SendFriends(Player player)
        {
            if (player == null)
            {
                return;
            }

            var online = new Dictionary<string, string>();
            var offline = new List<string>();

            foreach (var friend in player.CachedFriends)
            {
                var plyr = Player.FindOnline(friend.Key);
                if (plyr != null)
                {
                    online.Add(plyr.Name, MapList.List.FindMap(plyr.MapId)?.Name ?? "");
                }
                else
                {
                    offline.Add(friend.Value);
                }
            }

            player.SendPacket(new FriendsPacket(online, offline.ToArray()));
        }

        //FriendRequestPacket
        public static void SendFriendRequest(Player player, Player partner)
        {
            player.SendPacket(new FriendRequestPacket(partner.Id, partner.Name));
        }

        //PasswordResetResultPacket
        public static void SendPasswordResetResult(Client client, bool result)
        {
            client.Send(new PasswordResetResultPacket(result));
        }

        //TargetOverridePacket
        public static void SetPlayerTarget(Player player, Guid targetId)
        {
            player.SendPacket(new TargetOverridePacket(targetId));
        }

        //GuildMsgPacket
        public static void SendGuildMsg(Player player, string message, Color clr, string target = "")
        {
            foreach (var p in player.Guild.FindOnlineMembers())
            {
                if (p != null)
                {
                    SendChatMsg(p, message, ChatMessageType.Guild, clr, target);
                }
            }
        }

        /// <summary>
        /// Send a player their guild member list.
        /// </summary>
        /// <param name="player"></param>
        public static void SendGuild(Player player)
        {
            if (player == null || player.Guild == null)
            {
                return;
            }

            var members = player.Guild.Members.Values.ToArray();
            var onlineMembers = player.Guild.FindOnlineMembers();

            foreach (var member in members)
            {
                member.Online = onlineMembers.Any(m => m.Id == member.Id);
            }

            player.SendPacket(new GuildPacket(members));
        }

        //GuildRequestPacket
        public static void SendGuildInvite(Player player, Player from)
        {
            player.SendPacket(new GuildInvitePacket(from.Name, from.Guild.Name));
        }

        public static void SendDataToMap(Guid mapId, IPacket packet, Player except = null, TransmissionMode mode = TransmissionMode.All)
        {
            if (!MapInstance.Lookup.Keys.Contains(mapId))
            {
                return;
            }

            var players = MapInstance.Get(mapId).GetPlayersOnMap();
            foreach (var player in players)
            {
                if (player != null && player != except)
                {
                    player.SendPacket(packet, mode);
                }
            }
        }

        public static bool SendDataToProximity(Guid mapId, IPacket packet, Player except = null, TransmissionMode mode = TransmissionMode.All)
        {
            var map = MapInstance.Get(mapId);
            if (map == null)
            {
                return false;
            }
            
            SendDataToMap(mapId, packet, except, mode);

            foreach (var surrMap in map.SurroundingMapIds)
            {
                SendDataToMap(surrMap, packet, except, mode);
            }

            return true;
        }

        public static void SendDataToPlayers(IPacket packet, IEnumerable<Player> players, TransmissionMode mode = TransmissionMode.All)
        {
            foreach (var player in players)
            {
                player.SendPacket(packet, mode);
            }
        }

        public static void SendDataToEditors(IPacket packet)
        {
            lock (Globals.ClientLock)
            {
                foreach (var client in Globals.Clients)
                {
                    if (client.IsEditor)
                    {
                        client.Send(packet);
                    }
                }
            }
        }

        public static void SendDataToAllPlayers(IPacket packet, TransmissionMode mode = TransmissionMode.All)
        {
            lock (Globals.ClientLock)
            {
                foreach (var client in Globals.Clients)
                {
                    if (client?.Entity != null)
                    {
                        client.Send(packet, mode);
                    }
                }
            }
        }

        public static void SendDataToAll(IPacket packet, TransmissionMode mode = TransmissionMode.All)
        {
            lock (Globals.ClientLock)
            {
                foreach (var client in Globals.Clients)
                {
                    if ((client?.IsEditor ?? false) || client?.Entity != null)
                    {
                        client.Send(packet, mode);
                    }
                }
            }
        }

    }

}
