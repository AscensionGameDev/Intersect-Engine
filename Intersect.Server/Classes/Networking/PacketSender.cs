using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intersect.Collections;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Logging;
using Intersect.Models;
using Intersect.Network.Packets;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Database.PlayerData.Security;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Maps;

using EventInstance = Intersect.Server.Entities.EventInstance;

namespace Intersect.Server.Networking
{
    using LegacyDatabase = LegacyDatabase;

    public static class PacketSender
    {
        //Cached GameDataPacket that gets sent to clients
        public static GameDataPacket CachedGameDataPacket = null;

        //PingPacket
        public static void SendPing(Client client, bool request = true)
        {
            if (client != null)
            {
                client.SendPacket(new PingPacket(request));
            }
        }

        //ConfigPacket
        public static void SendServerConfig(Client client)
        {
            client.SendPacket(new ConfigPacket(Options.GetOptionsData()));
        }

        //JoinGamePacket
        public static void SendJoinGame(Client client)
        {
            if (!client.IsEditor) SendEntityDataTo(client, client.Entity);

            client.SendPacket(new JoinGamePacket());
            PacketSender.SendGameData(client);

            if (!client.IsEditor)
            {
                var player = client.Entity;
                var sw = new Stopwatch();
                sw.Start();
                player.RecalculateStatsAndPoints();
                System.Console.WriteLine("Took " + sw.ElapsedMilliseconds + "ms to recalculate player stats!");
                ((Player) client.Entity).InGame = true;
                PacketSender.SendTimeTo(client);
                
                if (client.Power.Editor)
                {
                    PacketSender.SendChatMsg(client, Strings.Player.adminjoined, CustomColors.AdminJoined);
                }
                else if (client.Power.IsModerator)
                {
                    PacketSender.SendChatMsg(client, Strings.Player.modjoined, CustomColors.ModJoined);
                }

                if (player.MapId == Guid.Empty)
                    player.WarpToSpawn();
                else
                    player.Warp(player.MapId, player.X, player.Y, player.Dir, false, player.Z);

                PacketSender.SendEntityDataTo(client, player);

                //Search for login activated events and run them
                foreach (EventBase evt in EventBase.Lookup.Values)
                {
                    if (evt != null)
                    {
                        player.StartCommonEvent(evt, CommonEventTrigger.Login);
                    }
                }
            }
        }

        //MapAreaPacket
        public static void SendAreaPacket(Client client, Guid mapId)
        {
            var surroundingMaps = MapInstance.Get(mapId).GetSurroundingMaps(true);
            var packets = new List<MapPacket>();
            foreach (var map in surroundingMaps)
            {
                packets.Add(GenerateMapPacket(client, map.Id));
            }

            client.SendPacket(new MapAreaPacket(packets.ToArray()));
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
                var mapPacket = new MapPacket(mapId, false, map.JsonData, map.TileData, map.AttributeData, map.Revision, map.MapGridX, map.MapGridY, new bool[4]);
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
                            mapPacket.CameraHolds = new bool[4] { true, true, true, true };
                            break;

                        case 0:
                            mapPacket.CameraHolds = new bool[4] { 0 == map.MapGridY, LegacyDatabase.MapGrids[map.MapGrid].YMax - 1 == map.MapGridY, 0 == map.MapGridX, LegacyDatabase.MapGrids[map.MapGrid].XMax - 1 == map.MapGridX };
                            break;
                    }
                }

                if (client.IsEditor)
                {
                    return mapPacket;
                }
                else
                {
                    mapPacket.MapItems = GenerateMapItemsPacket(mapId);
                    mapPacket.MapEntities = GenerateMapEntitiesPacket(mapId);

                    return mapPacket;
                }
            }
        }

        //MapPacket
        public static void SendMap(Client client, Guid mapId, bool allEditors = false)
        {
            if (client == null)
            {
                Log.Error("Attempted to send packet to null client.");
                return;
            }

            var map = MapInstance.Get(mapId);

            if (!client.IsEditor)
            {
                if (client.SentMaps.ContainsKey(mapId))
                {
                    if (client.SentMaps[mapId].Item1 > Globals.Timing.TimeMs && client.SentMaps[mapId].Item2 == map.Revision)
                    {
                        return;
                    }

                    client.SentMaps.Remove(mapId);
                }

                try
                {
                    client.SentMaps.Add(mapId, new Tuple<long, int>(Globals.Timing.TimeMs + 5000, map.Revision));
                }
                catch (Exception exception)
                {
                    Log.Error($"Current Map #: {mapId}");
                    Log.Error($"# Sent maps: {client.SentMaps.Count}");
                    Log.Error($"# Maps: {MapInstance.Lookup.Count}");
                    Log.Error(exception);

                    throw;
                }
            }

            if (client.IsEditor)
            {
                if (allEditors)
                {
                    SendDataToEditors(GenerateMapPacket(client,mapId));
                }
                else
                {
                    client.SendPacket(GenerateMapPacket(client, mapId));
                }
            }
            else
            {
                client.SendPacket(GenerateMapPacket(client,mapId));

                //TODO: INCLUDE EVENTS IN MAP PACKET
                if (mapId == client.Entity.MapId)
                    client.Entity.SendEvents();

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
                packet = new MapPacket(mapId,true);
            }
            else
            {
                packet = new MapPacket(mapId, false, map.JsonData, map.TileData, map.AttributeData, map.Revision, map.MapGridX, map.MapGridY);
            }
            SendDataToEditors(packet);
        }

        //MapEntitiesPacket
        public static MapEntitiesPacket GenerateMapEntitiesPacket(Guid mapId)
        {
            var map = MapInstance.Get(mapId);
            if (map != null)
            {
                var entities = map.GetEntities(false);
                var sendEntities = new List<EntityInstance>();
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
                    enPackets.Add(sendEntities[i].EntityPacket());
                }

                return new MapEntitiesPacket(enPackets.ToArray());
            }

            return null;
        }

        //EntityPacket
        public static void SendEntityDataTo(Client client, EntityInstance en)
        {
            if (en == null)
            {
                return;
            }

            var packet = en.EntityPacket();
            packet.IsSelf = en == client.Entity;
            client.SendPacket(packet);

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

        //MapEntitiesPacket
        public static void SendMapEntitiesTo(Client client, List<EntityInstance> entities)
        {
            var sendEntities = new List<EntityInstance>();
            for (var i = 0; i < entities.Count; i++)
            {
                if (entities[i] != null && entities[i] != client.Entity)
                {
                    sendEntities.Add(entities[i]);
                }
            }

            var enPackets = new List<EntityPacket>();
            for (var i = 0; i < sendEntities.Count; i++)
            {
                enPackets.Add(sendEntities[i].EntityPacket());
            }

            client.SendPacket(new MapEntitiesPacket(enPackets.ToArray()));

            SendMapEntityEquipmentTo(client, sendEntities); //Send the equipment of each player

            for (var i = 0; i < sendEntities.Count; i++)
            {
                if (sendEntities[i].GetType() == typeof(Npc))
                {
                    SendNpcAggressionTo(client.Entity, (Npc)sendEntities[i]);
                }
            }
        }

        public static void SendMapEntityEquipmentTo(Client client, List<EntityInstance> entities)
        {
            for (var i = 0; i < entities.Count; i++)
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

        //EntityDataPacket
        public static void SendEntityDataToProximity(EntityInstance en, Client except = null)
        {
            if (en == null)
            {
                return;
            }

            SendDataToProximity(en.MapId, en.EntityPacket());
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

        //EntityPositionPacket
        public static void SendEntityPositionTo(Client client, EntityInstance en)
        {
            if (en == null)
            {
                return;
            }
            
            client.SendPacket(new EntityPositionPacket(en.Id,en.GetEntityType(),en.MapId,en.X,en.Y,en.Dir,en.Passable,en.HideName));
        }

        //EntityPositionPacket
        public static void SendEntityPositionToAll(EntityInstance en)
        {
            if (en == null)
            {
                return;
            }

            SendDataToProximity(en.MapId, new EntityPositionPacket(en.Id, en.GetEntityType(), en.MapId, en.X, en.Y, en.Dir, en.Passable, en.HideName));
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
        public static void SendNpcAggressionTo(Player en, Npc npc)
        {
            if (en == null || npc == null)
            {
                return;
            }

            var aggression = -1;

            //Declare Aggression state
            if (npc.Target == null)
            {
                //TODO (0 is attack when attacked, 1 is attack on sight, 2 is friendly, 3 is guard)
                if (npc.IsFriend(en) || !en.CanAttack(npc, null))
                {
                    aggression = 2;
                }
                else if (npc.ShouldAttackPlayerOnSight(en))
                {
                    aggression = 1;
                }
            }

            en.Client.SendPacket(new NpcAggressionPacket(npc.Id,aggression));
        }

        //EntityLeftPacket
        public static void SendEntityLeave(EntityInstance en)
        {
            SendDataToProximity(en.MapId, new EntityLeftPacket(en.Id,en.GetEntityType(),en.MapId));
        }

        //EntityLeavePacket
        public static void SendEntityLeaveTo(Client client, EntityInstance en)
        {
            client.SendPacket(new EntityLeftPacket(en.Id,en.GetEntityType(),en.MapId));
        }

        //EventLeavePacket
        public static void SendEntityLeaveTo(Client client, EventInstance evt)
        {
            client.SendPacket(new EntityLeftPacket(evt.Id, EntityTypes.Event, evt.MapId));
        }

        //ChatMsgPacket
        public static void SendChatMsg(Client client, string message, string target = "")
        {
            SendChatMsg(client, message, CustomColors.PlayerMsg, target);
        }

        //ChatMsgPacket
        public static void SendChatMsg(Client client, string message, Color clr, string target = "")
        {
            client.SendPacket(new ChatMsgPacket(message,clr,target));
        }

        //GameDataPacket
        public static void SendGameData(Client client)
        {
            if (!client.IsEditor)
            {
                var sw = new Stopwatch();
                sw.Start();
                client.SendPacket(CachedGameDataPacket);
                SendGameObject(client, ClassBase.Get(client.Entity.ClassId));
                Console.WriteLine("Took " + sw.ElapsedMilliseconds + "ms to send game data to client!");
                return;
            }

            var gameObjects = new List<GameObjectPacket>();

            //Send massive amounts of game data
            foreach (var val in Enum.GetValues(typeof(GameObjectType)))
            {
                if ((GameObjectType)val == GameObjectType.Map)
                {
                    continue;
                }

                if (((GameObjectType)val == GameObjectType.Shop ||
                     (GameObjectType)val == GameObjectType.Event ||
                     (GameObjectType)val == GameObjectType.PlayerSwitch ||
                     (GameObjectType)val == GameObjectType.PlayerVariable ||
                     (GameObjectType)val == GameObjectType.ServerSwitch ||
                     (GameObjectType)val == GameObjectType.ServerVariable) && !client.IsEditor)
                {
                    continue;
                }

                SendGameObjects(client, (GameObjectType)val, gameObjects);

            }

            //Let the client/editor know they have everything now
            client.SendPacket(new GameDataPacket(gameObjects.ToArray(), CustomColors.Json()));
        }

        //GameDataPacket
        public static void CacheGameDataPacket()
        {
            var gameObjects = new List<GameObjectPacket>();

            //Send massive amounts of game data
            foreach (var val in Enum.GetValues(typeof(GameObjectType)))
            {
                if ((GameObjectType)val == GameObjectType.Map)
                {
                    continue;
                }

                if (((GameObjectType)val == GameObjectType.Shop ||
                     (GameObjectType)val == GameObjectType.Event ||
                     (GameObjectType)val == GameObjectType.PlayerSwitch ||
                     (GameObjectType)val == GameObjectType.PlayerVariable ||
                     (GameObjectType)val == GameObjectType.ServerSwitch ||
                     (GameObjectType)val == GameObjectType.ServerVariable))
                {
                    continue;
                }

                SendGameObjects(null, (GameObjectType)val, gameObjects);

            }

            CachedGameDataPacket = new GameDataPacket(gameObjects.ToArray(), CustomColors.Json());
        }

        //ChatMsgPacket
        public static void SendGlobalMsg(string message, string target = "")
        {
            SendGlobalMsg(message, CustomColors.AnnouncementChat, target);
        }

        //ChatMsgPacket
        public static void SendGlobalMsg(string message, Color clr, string target = "")
        {
            SendDataToAllPlayers(new ChatMsgPacket(message,clr,target));
        }

        //ChatMsgPacket
        public static void SendProximityMsg(string message, Guid mapId, string target = "")
        {
            SendProximityMsg(message, mapId, CustomColors.ProximityMsg);
        }

        //ChatMsgPacket
        public static void SendProximityMsg(string message, Guid mapId, Color clr, string target = "")
        {
            SendDataToProximity(mapId, new ChatMsgPacket(message,clr,target));
        }

        //ChatMsgPacket
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
                            SendChatMsg(client, message, clr, target);
                        }
                    }
                }
            }
        }

        //ChatMsgPacket
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
                            SendChatMsg(c, message, clr, target);
                        }
                    }
                }
            }
        }

        //ProjectileDeadPacket
        public static void SendRemoveProjectileSpawn(Guid mapId, Guid baseEntityId, int spawnIndex)
        {
            SendDataToProximity(mapId, new ProjectileDeadPacket(baseEntityId,spawnIndex));
        }

        //EntityMovePacket
        public static void SendEntityMove(EntityInstance en, bool correction = false)
        {
            SendDataToProximity(en.MapId, new EntityMovePacket(en.Id,en.GetEntityType(),en.MapId,en.X,en.Y,en.Dir,correction));
        }

        //EntityMovePacket
        public static void SendEntityMoveTo(Client client, EntityInstance en, bool correction = false)
        {
            client.SendPacket(new EntityMovePacket(en.Id, en.GetEntityType(), en.MapId, en.X, en.Y, en.Dir, correction));
        }

        //EntityVitalsPacket
        public static EntityVitalsPacket GenerateEntityVitalsPacket(EntityInstance en)
        {
            var statuses = en.Statuses.Values.ToArray();

            return new EntityVitalsPacket(en.Id, en.GetEntityType(), en.MapId, en.GetVitals(), en.GetMaxVitals(), en.StatusPackets());
        }

        //EntityVitalsPacket
        public static void SendEntityVitals(EntityInstance en)
        {
            if (en == null)
            {
                return;
            }
            
            //If player and in party send vitals to party just in case party members are not in the proximity
            if (en.GetType() == typeof(Player))
            {
                for (var i = 0; i < ((Player)en).Party.Count; i++)
                {
                    SendPartyUpdateTo(((Player)en).Party[i].Client, (Player)en);
                }
            }

            SendDataToProximity(en.MapId, GenerateEntityVitalsPacket(en));
        }

        //EntityStatsPacket
        public static void SendEntityStats(EntityInstance en)
        {
            if (en == null)
            {
                return;
            }

            SendDataToProximity(en.MapId, GenerateEntityStatsPacket(en));
        }

        //EntityVitalsPacket
        public static void SendEntityVitalsTo(Client client, EntityInstance en)
        {
            if (en == null) return;
            client.SendPacket(GenerateEntityVitalsPacket(en));
        }

        //EntityStatsPacket
        public static EntityStatsPacket GenerateEntityStatsPacket(EntityInstance en)
        {
            var stats = new int[(int)Stats.StatCount];
            for (var i = 0; i < (int)Stats.StatCount; i++)
            {
                stats[i] = en.Stat[i].Value();
            }

            return new EntityStatsPacket(en.Id, en.GetEntityType(), en.MapId, stats);
        }

        //EntityStatsPacket
        public static void SendEntityStatsTo(Client client, EntityInstance en)
        {
            client.SendPacket(GenerateEntityStatsPacket(en));
        }

        //EntityDirectionPacket
        public static void SendEntityDir(EntityInstance en)
        {
            SendDataToProximity(en.MapId, new EntityDirectionPacket(en.Id, en.GetEntityType(), en.MapId, en.Dir));
        }

        //EntityAttackPacket
        public static void SendEntityAttack(EntityInstance en, int attackTime)
        {
            SendDataToProximity(en.MapId, new EntityAttackPacket(en.Id,en.GetEntityType(),en.MapId,attackTime));
        }

        //EntityDirectionPacket
        public static void SendEntityDirTo(Client client, EntityInstance en)
        {
            client.SendPacket(new EntityDirectionPacket(en.Id,en.GetEntityType(),en.MapId,en.Dir));
        }

        //EventDialogPacket
        public static void SendEventDialog(Player player, string prompt, string face, Guid eventId)
        {
            player.Client.SendPacket(new EventDialogPacket(eventId,prompt,face,0,null));
        }

        //EventDialogPacket
        public static void SendEventDialog(Player player, string prompt, string opt1, string opt2, string opt3,string opt4, string face, Guid eventId)
        {
            player.Client.SendPacket(new EventDialogPacket(eventId,prompt,face,1,new string[4] {opt1,opt2,opt3,opt4}));
        }

        //MapListPacket
        public static void SendMapList(Client client)
        {
            client.SendPacket(new MapListPacket(MapList.List.JsonData));
        }

        //MapListPacket
        public static void SendMapListToAll()
        {
            SendDataToAll(new MapListPacket(MapList.List.JsonData));
        }

        //ErrorPacket
        public static void SendError(Client client, string error, string header = "")
        {
            client.SendPacket(new ErrorMessagePacket(header,error));
        }

        //MapItemsPacket
        public static MapItemsPacket GenerateMapItemsPacket(Guid mapId)
        {
            var map = MapInstance.Get(mapId);
            var items = new string[map.MapItems.Count];
            for (var i = 0; i < map.MapItems.Count; i++)
            {
                if (map.MapItems[i] != null)
                {
                    items[i] = map.MapItems[i].Data();
                }
            }
            return new MapItemsPacket(mapId, items);
        }

        //MapItemsPacket
        public static void SendMapItems(Client client, Guid mapId)
        {
            
            client.SendPacket(GenerateMapItemsPacket(mapId));
        }

        //MapItemsPacket
        public static void SendMapItemsToProximity(Guid mapId)
        {
            var map = MapInstance.Get(mapId);
            var items = new string[map.MapItems.Count];
            for (var i = 0; i < map.MapItems.Count; i++)
            {
                if (map.MapItems[i] != null)
                {
                    items[i] = map.MapItems[i].Data();
                }
            }
            SendDataToProximity(mapId, new MapItemsPacket(mapId, items));
        }

        //MapItemUpdatePacket
        public static void SendMapItemUpdate(Guid mapId, int index)
        {
            var map = MapInstance.Get(mapId);
            string itemData = null;
            if (map != null && map.MapItems[index].ItemId != Guid.Empty)
            {
                itemData = map.MapItems[index].Data();
            }
            SendDataToProximity(mapId, new MapItemUpdatePacket(mapId,index,itemData));
        }

        //InventoryPacket
        public static void SendInventory(Client client)
        {
            var invItems = new InventoryUpdatePacket[Options.MaxInvItems];
            for (var i = 0; i < Options.MaxInvItems; i++)
            {
                invItems[i] = new InventoryUpdatePacket(i, client.Entity.Items[i].ItemId, client.Entity.Items[i].Quantity, client.Entity.Items[i].BagId, client.Entity.Items[i].StatBuffs);
            }
            client.SendPacket(new InventoryPacket(invItems));
        }

        //InventoryUpdatePacket
        public static void SendInventoryItemUpdate(Client client, int slot)
        {
            client.SendPacket(new InventoryUpdatePacket(slot, client.Entity.Items[slot].ItemId, client.Entity.Items[slot].Quantity, client.Entity.Items[slot].BagId, client.Entity.Items[slot].StatBuffs));
        }

        //SpellsPacket
        public static void SendPlayerSpells(Client client)
        {
            var spells = new SpellUpdatePacket[Options.MaxPlayerSkills];
            for (var i = 0; i < Options.MaxPlayerSkills; i++)
            {
                spells[i] = new SpellUpdatePacket(i, client.Entity.Spells[i].SpellId);
            }
            client.SendPacket(new SpellsPacket(spells));
        }

        //SpellUpdatePacket
        public static void SendPlayerSpellUpdate(Client client, int slot)
        {
            client.SendPacket(new SpellUpdatePacket(slot, client.Entity.Spells[slot].SpellId));
        }

        //EquipmentPacket
        public static EquipmentPacket GenerateEquipmentPacket(Client forClient, Player en)
        {
            if (forClient != null && forClient.Entity == en)
            {
                return new EquipmentPacket(en.Id, en.Equipment, null);
            }
            else
            {
                Guid[] equipment = new Guid[Options.EquipmentSlots.Count];
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
        public static void SendPlayerEquipmentTo(Client client, Player en)
        {
            client.SendPacket(GenerateEquipmentPacket(client,en));
        }

        //EquipmentPacket
        public static void SendPlayerEquipmentToProximity(Player en)
        {
            SendDataToProximity(en.MapId, GenerateEquipmentPacket(null, en));
            SendPlayerEquipmentTo(en.Client, en);
        }

        //StatPointsPacket
        public static void SendPointsTo(Client client)
        {
            client.SendPacket(new StatPointsPacket(client.Entity.StatPoints));
        }

        //HotbarPacket
        public static void SendHotbarSlots(Client client)
        {
            var hotbarData = new string[Options.MaxHotbar];
            for (var i = 0; i < Options.MaxHotbar; i++)
            {
                hotbarData[i] = client.Entity.Hotbar[i].Data();
            }
            client.SendPacket(new HotbarPacket(hotbarData));
        }

        //CreateCharacterPacket
        public static void SendCreateCharacter(Client client)
        {
            client.SendPacket(new CharacterCreationPacket());
        }

        //CharactersPacket
        public static void SendPlayerCharacters(Client client)
        {
            var characters = new List<CharacterPacket>();
            foreach (var character in client.Characters.OrderByDescending(p => p.LastOnline))
            {
                var equipmentArray = character.Equipment;
                var equipment = new string[Options.EquipmentSlots.Count + 1];
                //Draw the equipment/paperdolls
                for (var z = 0; z < Options.PaperdollOrder[1].Count; z++)
                {
                    if (Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z]) > -1)
                    {
                        if (equipmentArray[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] > -1 && equipmentArray[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])] < Options.MaxInvItems)
                        {
                            var itemId = character.Items[equipmentArray[Options.EquipmentSlots.IndexOf(Options.PaperdollOrder[1][z])]].ItemId;

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

                characters.Add(new CharacterPacket(character.Id, character.Name, character.Sprite, character.Face, character.Level, ClassBase.GetName(character.ClassId),equipment));
            }
            client.SendPacket(new CharactersPacket(characters.ToArray(), client.Characters.Count < Options.MaxCharacters));
        }

        //AdminPanelPacket
        public static void SendOpenAdminWindow(Client client)
        {
            client.SendPacket(new AdminPanelPacket());
        }

        //MapGridPacket
        public static void SendMapGridToAll(int gridIndex)
        {
            for (var i = 0; i < Globals.Clients.Count; i++)
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

        //MapGridPacket
        public static void SendMapGrid(Client client, int gridIndex, bool clearKnownMaps = false)
        {
            var grid = LegacyDatabase.MapGrids[gridIndex];
            if (clearKnownMaps)
            {
                client.SentMaps.Clear();
            }

            if (client.IsEditor)
            {
                client.SendPacket(new MapGridPacket(null, grid.GetEditorData(), clearKnownMaps));
            }
            else
            {
                client.SendPacket(new MapGridPacket(grid.GetClientData(), null, clearKnownMaps));
                if (clearKnownMaps)
                    SendAreaPacket(client, client.Entity.MapId);
            }
        }

        //SpellCastPacket
        public static void SendEntityCastTime(EntityInstance en, Guid spellId)
        {
            SendDataToProximity(en.MapId, new SpellCastPacket(en.Id,spellId));
        }

        //SpellCooldownPacket
        public static void SendSpellCooldown(Client client, int spellSlot)
        {
            client.SendPacket(new SpellCooldownPacket(spellSlot));
        }

        //ItemCooldownPacket
        public static void SendItemCooldown(Client client, Guid itemId)
        {
            client.SendPacket(new ItemCooldownPacket(itemId));
        }

        //ExperiencePacket
        public static void SendExperience(Client client)
        {
            client.SendPacket(new ExperiencePacket(client.Entity.Exp,client.Entity.ExperienceToNextLevel));
        }

        //PlayAnimationPacket
        public static void SendAnimationToProximity(Guid animId, int targetType, Guid entityId, Guid mapId, byte x, byte y, sbyte direction)
        {
            SendDataToProximity(mapId, new PlayAnimationPacket(animId,targetType,entityId,mapId,x,y,direction));
        }

        //HoldPlayerPacket
        public static void SendHoldPlayer(Client client, Guid eventId, Guid mapId)
        {
            client.SendPacket(new HoldPlayerPacket(eventId,mapId,false));
        }

        //HoldPlayerPacket
        public static void SendReleasePlayer(Client client, Guid eventId)
        {
            client.SendPacket(new HoldPlayerPacket(eventId, Guid.Empty, true));
        }

        //PlayMusicPacket
        public static void SendPlayMusic(Client client, string bgm)
        {
            client.SendPacket(new PlayMusicPacket(bgm));
        }

        //StopMusicPacket
        public static void SendFadeMusic(Client client)
        {
            client.SendPacket(new StopMusicPacket());
        }

        //PlaySoundPacket
        public static void SendPlaySound(Client client, string sound)
        {
            client.SendPacket(new PlaySoundPacket(sound));
        }

        //StopSoundPacket
        public static void SendStopSounds(Client client)
        {
            client.SendPacket(new StopSoundsPacket());
        }

        //ShowPicturePacket
        public static void SendShowPicture(Client client, string picture, int size, bool clickable)
        {
            client.SendPacket(new ShowPicturePacket(picture,size,clickable));
        }

        //HidePicturePacket
        public static void SendHidePicture(Client client)
        {
            client.SendPacket(new HidePicturePacket());
        }

        //ShopPacket
        public static void SendOpenShop(Client client, ShopBase shop)
        {
            if (shop == null)
            {
                return;
            }
            client.SendPacket(new ShopPacket(shop.JsonData,false));
        }

        //ShopPacket
        public static void SendCloseShop(Client client)
        {
            client.SendPacket(new ShopPacket(null,true));
        }

        //BankPacket
        public static void SendOpenBank(Client client)
        {
            for (var i = 0; i < Options.MaxBankSlots; i++)
            {
                SendBankUpdate(client, i);
            }
            client.SendPacket(new BankPacket(false));
        }

        //BankPacket
        public static void SendCloseBank(Client client)
        {
            client.SendPacket(new BankPacket(true));
        }

        //CraftingTablePacket
        public static void SendOpenCraftingTable(Client client, CraftingTableBase table)
        {
            if (table != null)
                client.SendPacket(new CraftingTablePacket(table.JsonData, false));
        }

        //CraftingTablePacket
        public static void SendCloseCraftingTable(Client client)
        {
            client.SendPacket(new CraftingTablePacket(null,true));
        }

        //BankUpdatePacket
        public static void SendBankUpdate(Client client, int slot)
        {
            if (client.Entity.Bank[slot] != null && client.Entity.Bank[slot].ItemId != Guid.Empty && client.Entity.Bank[slot].Quantity > 0)
            {
                client.SendPacket(new BankUpdatePacket(slot, client.Entity.Bank[slot].ItemId, client.Entity.Bank[slot].Quantity, client.Entity.Bank[slot].BagId, client.Entity.Bank[slot].StatBuffs));
            }
            else
            {
                client.SendPacket(new BankUpdatePacket(slot,Guid.Empty,0,null,null));
            }
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
                        if (((EventBase)obj.Value).CommonEvent)
                        {
                            SendGameObject(client, obj.Value, false, false, packetList);
                        }
                    }

                    break;
                case GameObjectType.PlayerSwitch:
                    foreach (var obj in PlayerSwitchBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.PlayerVariable:
                    foreach (var obj in PlayerVariableBase.Lookup)
                    {
                        SendGameObject(client, obj.Value, false, false, packetList);
                    }

                    break;
                case GameObjectType.ServerSwitch:
                    foreach (var obj in ServerSwitchBase.Lookup)
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        //GameObjectPacket
        public static void SendGameObject(Client client, IDatabaseObject obj, bool deleted = false, bool another = false, List<GameObjectPacket> packetList = null)
        {
            if ((client == null && packetList == null) || obj == null)
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
                client.SendPacket(new GameObjectPacket(obj.Id,obj.Type,deleted ? null : obj.JsonData,deleted,another));
            else
                packetList.Add(new GameObjectPacket(obj.Id, obj.Type, deleted ? null : obj.JsonData, deleted, another));
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
            client.SendPacket(new OpenEditorPacket(type));
        }
        
        //EntityDashPacket
        public static void SendEntityDash(EntityInstance en, Guid endMapId, byte endX, byte endY, int dashTime, sbyte direction)
        {
            SendDataToProximity(en.MapId, new EntityDashPacket(en.Id,endMapId,endX,endY,dashTime,direction));
        }

        //ActionMsgPacket
        public static void SendActionMsg(EntityInstance en, string message, Color color)
        {
            SendDataToProximity(en.MapId, new ActionMsgPacket(en.MapId,en.X,en.Y,message,color));
        }

        //EnterMapPacket
        public static void SendEnterMap(Client client, Guid mapId)
        {
            client.SendPacket(new EnterMapPacket(mapId));
        }

        //TimeDataPacket
        public static void SendTimeBaseTo(Client client)
        {
            client.SendPacket(new TimeDataPacket(TimeBase.GetTimeBase().SaveTimeBase()));
        }

        //TimeDataPacket
        public static void SendTimeBaseToAllEditors()
        {
            SendDataToEditors(new TimeDataPacket(TimeBase.GetTimeBase().SaveTimeBase()));
        }

        //TimePacket
        public static void SendTimeToAll()
        {
            SendDataToAllPlayers(new TimePacket(ServerTime.GetTime(),TimeBase.GetTimeBase().SyncTime ? 1 : TimeBase.GetTimeBase().Rate, ServerTime.GetTimeColor()));
        }

        //TimePacket
        public static void SendTimeTo(Client client)
        {
            client.SendPacket(new TimePacket(ServerTime.GetTime(), TimeBase.GetTimeBase().SyncTime ? 1 : TimeBase.GetTimeBase().Rate, ServerTime.GetTimeColor()));
        }

        //PartyPacket
        public static void SendParty(Client client)
        {
            var memberPackets = new PartyMemberPacket[client.Entity.Party.Count];
            for (var i = 0; i < client.Entity.Party.Count; i++)
            {
                var mem = client.Entity.Party[i];
                memberPackets[i] = new PartyMemberPacket(mem.Id, mem.Name, mem.GetVitals(), mem.GetMaxVitals(), mem.Level);
            }
            client.SendPacket(new PartyPacket(memberPackets));
        }

        //PartyUpdatePacket
        public static void SendPartyUpdateTo(Client client, Player entity)
        {
            var partyIndex = -1;
            for (var i = 0; i < client.Entity.Party.Count; i++)
            {
                if (client.Entity.Party[i] == entity)
                {
                    partyIndex = i;
                }
            }
            if (partyIndex > -1)
            {
                client.SendPacket(new PartyUpdatePacket(partyIndex,new PartyMemberPacket(entity.Id,entity.Name,entity.GetVitals(),entity.GetMaxVitals(),entity.Level)));
            }
        }
        
        //PartyInvitePacket
        public static void SendPartyInvite(Client client, Player leader)
        {
            client.SendPacket(new PartyInvitePacket(leader.Name,leader.Id));
        }

        //ChatBubblePacket
        public static void SendChatBubble(Guid entityId, EntityTypes type, string text, Guid mapId)
        {
            SendDataToProximity(mapId, new ChatBubblePacket(entityId,type,mapId,text));
        }

        //QuestOfferPacket
        public static void SendQuestOffer(Player player, Guid questId)
        {
            player.Client.SendPacket(new QuestOfferPacket(questId));
        }

        //QuestProgressPacket
        public static void SendQuestsProgress(Client client)
        {
            var dict = new Dictionary<Guid, string>();
            foreach (var quest in client.Entity.Quests)
            {
                dict.Add(quest.QuestId,quest.Data());
            }
            client.SendPacket(new QuestProgressPacket(dict));
        }

        //QuestProgressPacket
        public static void SendQuestProgress(Player player, Guid questId)
        {
            var dict = new Dictionary<Guid, string>();
            var questProgress = player.FindQuest(questId);
            if (questProgress != null)
            {
                dict.Add(questId,questProgress.Data());
            }
            else
            {
                dict.Add(questId,null);
            }
            player.Client.SendPacket(new QuestProgressPacket(dict));
        }

        //TradePacket
        public static void StartTrade(Client client, Player target)
        {
            client.SendPacket(new TradePacket(target.Id));
        }

        //TradeUpdatePacket
        public static void SendTradeUpdate(Client client, Player trader, int slot)
        {
            if (trader.Trading.Offer[slot] != null &&  trader.Trading.Offer[slot].ItemId != Guid.Empty && trader.Trading.Offer[slot].Quantity > 0)
            {
                client.SendPacket(new TradeUpdatePacket(trader.Id,slot, trader.Trading.Offer[slot].ItemId, trader.Trading.Offer[slot].Quantity, trader.Trading.Offer[slot].BagId, trader.Trading.Offer[slot].StatBuffs));
            }
            else
            {
                client.SendPacket(new TradeUpdatePacket(trader.Id, slot, Guid.Empty,0,null,null));
            }
        }

        //TradePacket
        public static void SendTradeClose(Client client)
        {
            client.SendPacket(new TradePacket(Guid.Empty));
        }

        //TradeRequestPacket
        public static void SendTradeRequest(Client client, Player partner)
        {
            client.SendPacket(new TradeRequestPacket(partner.Id,partner.Name));
        }

        //PlayerDeathPacket
        public static void SendPlayerDeath(Player en)
        {
            SendDataToProximity(en.MapId, new PlayerDeathPacket(en.Id));
        }

        //EntityZDimensionPacket
        public static void UpdateEntityZDimension(EntityInstance en, byte z)
        {
            SendDataToProximity(en.MapId, new EntityZDimensionPacket(en.Id,z));
        }

        //BagPacket
        public static void SendOpenBag(Client client, int slots, Bag bag)
        {
            client.SendPacket(new BagPacket(slots,false));
            for (var i = 0; i < slots; i++)
            {
                SendBagUpdate(client, i, bag.Slots[i]);
            }
        }

        //BagUpdatePacket
        public static void SendBagUpdate(Client client, int slot, Item item)
        {
            if (item != null && item.ItemId != Guid.Empty && item.Quantity > 0)
            {
                client.SendPacket(new BagUpdatePacket(slot, item.ItemId,item.Quantity,item.BagId,item.StatBuffs));
            }
            else
            {
                client.SendPacket(new BagUpdatePacket(slot, Guid.Empty,0,null,null));
            }
        }

        //BagPacket
        public static void SendCloseBag(Client client)
        {
            client.SendPacket(new BagPacket(0,true));
        }

        //MoveRoutePacket
        public static void SendMoveRouteToggle(Client client, bool routeOn)
        {
            client.SendPacket(new MoveRoutePacket(routeOn));
        }

        //FriendsPacket
        public static void SendFriends(Client client)
        {
            var online = new Dictionary<string,string>();
            var offline = new List<string>();
            var found = false;

            foreach (var friend in client.Entity.Friends)
            {
                found = false;
                foreach (var c in Globals.Clients)
                {
                    if (c != null && c.Entity != null)
                    {
                        if (friend.Target.Name.ToLower() == c.Entity.Name.ToLower())
                        {
                            online.Add(friend.Target.Name, MapList.List.FindMap(friend.Target.MapId).Name);
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

            client.SendPacket(new FriendsPacket(online, offline.ToArray()));
        }

        //FriendRequestPacket
        public static void SendFriendRequest(Client client, Player partner)
        {
            client.SendPacket(new FriendRequestPacket(partner.Id,partner.Name));
        }

        //PasswordResetResultPacket
        public static void SendPasswordResetResult(Client client, bool result)
        {
            client.SendPacket(new PasswordResetResultPacket(result));
        }

        //TargetOverridePacket
        public static void SetPlayerTarget(Client client, Guid targetId)
        {
            client.SendPacket(new TargetOverridePacket(targetId));
        }


        public static void SendDataToMap(Guid mapId, CerasPacket packet, Client except = null)
        {
            if (!MapInstance.Lookup.Keys.Contains(mapId))
            {
                return;
            }
            var players = MapInstance.Get(mapId).GetPlayersOnMap();
            foreach (var player in players)
            {
                if (player != null && player.Client != except)
                {
                    player.Client.SendPacket(packet);
                }
            }
        }

        public static void SendDataToProximity(Guid mapId, CerasPacket packet, Client except = null)
        {
            if (!MapInstance.Lookup.Keys.Contains(mapId))
            {
                return;
            }
            SendDataToMap(mapId, packet, except);
            for (var i = 0; i < MapInstance.Get(mapId).SurroundingMaps.Count; i++)
            {
                SendDataToMap(MapInstance.Get(mapId).SurroundingMaps[i], packet, except);
            }
        }

        public static void SendDataToEditors(CerasPacket packet)
        {
            lock (Globals.ClientLock)
            {
                foreach (var client in Globals.Clients)
                {
                    if (client.IsEditor)
                    {
                        client.SendPacket(packet);
                    }
                }
            }
        }

        public static void SendDataToAllPlayers(CerasPacket packet)
        {
            lock (Globals.ClientLock)
            {
                foreach (var client in Globals.Clients)
                {
                    if (client != null)
                    {
                        if (client.Entity != null)
                        {
                            client.SendPacket(packet);
                        }
                    }
                }
            }
        }

        public static void SendDataToAll(CerasPacket packet)
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
    }
}