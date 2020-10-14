using System;
using System.Collections.Generic;
using System.Linq;

using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Entities.Events;
using Intersect.Client.Entities.Projectiles;
using Intersect.Client.General;
using Intersect.Client.Interface.Game;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Interface.Menu;
using Intersect.Client.Items;
using Intersect.Client.Localization;
using Intersect.Client.Maps;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Logging;
using Intersect.Network;
using Intersect.Network.Packets;
using Intersect.Network.Packets.Server;
using Intersect.Utilities;

namespace Intersect.Client.Networking
{

    public static class PacketHandler
    {

        public static long Ping = 0;

        public static long PingTime;

        public static bool HandlePacket(IPacket packet)
        {
            if (packet is AbstractTimedPacket timedPacket)
            {
                Timing.Global.Synchronize(timedPacket.UTC, timedPacket.Offset);
            }

            if (packet is CerasPacket)
            {
                HandlePacket((dynamic)packet);
            }

            return true;
        }

        //PingPacket
        private static void HandlePacket(PingPacket packet)
        {
            if (packet.RequestingReply)
            {
                PacketSender.SendPing();
                PingTime = Globals.System.GetTimeMs();
            }
            else
            {
                Network.Ping = (int) (Globals.System.GetTimeMs() - PingTime) / 2;
            }
        }

        //ConfigPacket
        private static void HandlePacket(ConfigPacket packet)
        {
            Options.LoadFromServer(packet.Config);
            Globals.Bank = new Item[Options.MaxBankSlots];
            Graphics.InitInGame();
        }

        //JoinGamePacket
        private static void HandlePacket(JoinGamePacket packet)
        {
            Main.JoinGame();
            Globals.JoiningGame = true;
        }

        //MapAreaPacket
        private static void HandlePacket(MapAreaPacket packet)
        {
            foreach (var map in packet.Maps)
            {
                HandleMap(map);
            }
        }

        //MapPacket
        private static void HandleMap(MapPacket packet)
        {
            var mapId = packet.MapId;
            var map = MapInstance.Get(mapId);
            if (map != null)
            {
                if (packet.Revision == map.Revision)
                {
                    return;
                }
                else
                {
                    map.Dispose(false, false);
                }
            }

            map = new MapInstance(mapId);
            MapInstance.Lookup.Set(mapId, map);
            lock (map.MapLock)
            {
                map.Load(packet.Data);
                map.LoadTileData(packet.TileData);
                map.AttributeData = packet.AttributeData;
                map.CreateMapSounds();
                if (mapId == Globals.Me.CurrentMap)
                {
                    Audio.PlayMusic(map.Music, 3, 3, true);
                }

                map.MapGridX = packet.GridX;
                map.MapGridY = packet.GridY;
                map.CameraHolds = packet.CameraHolds;
                map.Autotiles.InitAutotiles(map.GenerateAutotileGrid());

                //Process Entities and Items if provided in this packet
                if (packet.MapEntities != null)
                {
                    HandlePacket((dynamic) packet.MapEntities);
                }

                if (packet.MapItems != null)
                {
                    HandlePacket((dynamic) packet.MapItems);
                }

                if (Globals.PendingEvents.ContainsKey(mapId))
                {
                    foreach (var evt in Globals.PendingEvents[mapId])
                    {
                        map.AddEvent(evt.Key, evt.Value);
                    }

                    Globals.PendingEvents[mapId].Clear();
                }
            }

            if (MapInstance.OnMapLoaded != null)
            {
                MapInstance.OnMapLoaded(map);
            }
        }

        //MapPacket
        private static void HandlePacket(MapPacket packet)
        {
            HandleMap(packet);
            Globals.Me.FetchNewMaps();
        }

        //PlayerEntityPacket
        private static void HandlePacket(PlayerEntityPacket packet)
        {
            var en = Globals.GetEntity(packet.EntityId, EntityTypes.Player);
            if (en != null)
            {
                en.Load(packet);
                if (packet.IsSelf)
                {
                    Globals.Me = (Player) Globals.Entities[packet.EntityId];
                }
            }
            else
            {
                Globals.Entities.Add(packet.EntityId, new Player(packet.EntityId, packet));
                if (packet.IsSelf)
                {
                    Globals.Me = (Player) Globals.Entities[packet.EntityId];
                }
            }
        }

        //NpcEntityPacket
        private static void HandlePacket(NpcEntityPacket packet)
        {
            var en = Globals.GetEntity(packet.EntityId, EntityTypes.GlobalEntity);
            if (en != null)
            {
                en.Load(packet);
                en.Type = packet.Aggression;
            }
            else
            {
                Globals.Entities.Add(packet.EntityId, new Entity(packet.EntityId, packet));
                Globals.Entities[packet.EntityId].Type = packet.Aggression;
            }
        }

        //ResourceEntityPacket
        private static void HandlePacket(ResourceEntityPacket packet)
        {
            var en = Globals.GetEntity(packet.EntityId, EntityTypes.Resource);
            if (en != null)
            {
                en.Load(packet);
            }
            else
            {
                Globals.Entities.Add(packet.EntityId, new Resource(packet.EntityId, packet));
            }
        }

        //ProjectileEntityPacket
        private static void HandlePacket(ProjectileEntityPacket packet)
        {
            var en = Globals.GetEntity(packet.EntityId, EntityTypes.Projectile);
            if (en != null)
            {
                en.Load(packet);
            }
            else
            {
                Globals.Entities.Add(packet.EntityId, new Projectile(packet.EntityId, packet));
            }
        }

        //EventEntityPacket
        private static void HandlePacket(EventEntityPacket packet)
        {
            var map = MapInstance.Get(packet.MapId);
            if (map != null)
            {
                map?.AddEvent(packet.EntityId, packet);
            }
            else
            {
                var dict = Globals.PendingEvents.ContainsKey(packet.MapId)
                    ? Globals.PendingEvents[packet.MapId]
                    : new Dictionary<Guid, EventEntityPacket>();

                if (dict.ContainsKey(packet.EntityId))
                {
                    dict[packet.EntityId] = packet;
                }
                else
                {
                    dict.Add(packet.EntityId, packet);
                }

                if (!Globals.PendingEvents.ContainsKey(packet.MapId))
                {
                    Globals.PendingEvents.Add(packet.MapId, dict);
                }
            }
        }

        //MapEntitiesPacket
        private static void HandlePacket(MapEntitiesPacket packet)
        {
            var mapEntities = new Dictionary<Guid, List<Guid>>();
            foreach (var pkt in packet.MapEntities)
            {
                HandlePacket((dynamic) pkt);

                if (!mapEntities.ContainsKey(pkt.MapId))
                {
                    mapEntities.Add(pkt.MapId, new List<Guid>());
                }

                mapEntities[pkt.MapId].Add(pkt.EntityId);
            }

            //Remove any entities on the map that shouldn't be there anymore!
            foreach (var entities in mapEntities)
            {
                foreach (var entity in Globals.Entities)
                {
                    if (entity.Value.CurrentMap == entities.Key && !entities.Value.Contains(entity.Key))
                    {
                        if (!Globals.EntitiesToDispose.Contains(entity.Key) && entity.Value != Globals.Me)
                        {
                            Globals.EntitiesToDispose.Add(entity.Key);
                        }
                    }
                }
            }
        }

        //EntityPositionPacket
        private static void HandlePacket(EntityPositionPacket packet)
        {
            var id = packet.Id;
            var type = packet.Type;
            var mapId = packet.MapId;
            Entity en;
            if (type != EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }

                en = Globals.Entities[id];
            }
            else
            {
                if (MapInstance.Get(mapId) == null)
                {
                    return;
                }

                if (!MapInstance.Get(mapId).LocalEntities.ContainsKey(id))
                {
                    return;
                }

                en = MapInstance.Get(mapId).LocalEntities[id];
            }

            if (en == Globals.Me)
            {
                Log.Debug($"received epp: {Timing.Global.Milliseconds}");
            }

            if (en == Globals.Me &&
                (Globals.Me.DashQueue.Count > 0 || Globals.Me.DashTimer > Globals.System.GetTimeMs()))
            {
                return;
            }

            if (en == Globals.Me && Globals.Me.CurrentMap != mapId)
            {
                Globals.Me.CurrentMap = mapId;
                Globals.NeedsMaps = true;
                Globals.Me.FetchNewMaps();
            }
            else
            {
                en.CurrentMap = mapId;
            }

            en.X = packet.X;
            en.Y = packet.Y;
            en.Dir = packet.Direction;
            en.Passable = packet.Passable;
            en.HideName = packet.HideName;
        }

        //EntityLeftPacket
        private static void HandlePacket(EntityLeftPacket packet)
        {
            var id = packet.Id;
            var type = packet.Type;
            var mapId = packet.MapId;
            if (id == Globals.Me?.Id && type < EntityTypes.Event)
            {
                return;
            }

            if (type != EntityTypes.Event)
            {
                if (Globals.Entities?.ContainsKey(id) ?? false)
                {
                    Globals.Entities[id]?.Dispose();
                    Globals.EntitiesToDispose?.Add(id);
                }
            }
            else
            {
                var map = MapInstance.Get(mapId);
                if (map?.LocalEntities?.ContainsKey(id) ?? false)
                {
                    map.LocalEntities[id]?.Dispose();
                    map.LocalEntities[id] = null;
                    map.LocalEntities.Remove(id);
                }
            }
        }

        //ChatMsgPacket
        private static void HandlePacket(ChatMsgPacket packet)
        {
            ChatboxMsg.AddMessage(
                new ChatboxMsg(
                    packet.Message ?? "", new Color(packet.Color.A, packet.Color.R, packet.Color.G, packet.Color.B),
                    packet.Target
                )
            );
        }

        //ActionMsgPacket
        private static void HandlePacket(ActionMsgPacket packet)
        {
            var map = MapInstance.Get(packet.MapId);
            if (map != null)
            {
                map.ActionMsgs.Add(
                    new ActionMessage(
                        map, packet.X, packet.Y, packet.Message,
                        new Color(packet.Color.A, packet.Color.R, packet.Color.G, packet.Color.B)
                    )
                );
            }
        }

        //GameDataPacket
        private static void HandlePacket(GameDataPacket packet)
        {
            foreach (var pkt in packet.GameObjects)
            {
                HandlePacket((dynamic) pkt);
            }

            CustomColors.Load(packet.ColorsJson);
            Globals.HasGameData = true;
        }

        //MapListPacket
        private static void HandlePacket(MapListPacket packet)
        {
            MapList.List.JsonData = packet.MapListData;
            MapList.List.PostLoad(MapBase.Lookup, false, true);

            //TODO ? If admin window is open update it
        }

        //EntityMovePacket
        private static void HandlePacket(EntityMovePacket packet)
        {
            var id = packet.Id;
            var type = packet.Type;
            var mapId = packet.MapId;
            Entity en;
            if (type < EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }

                en = Globals.Entities[id];
            }
            else
            {
                var gameMap = MapInstance.Get(mapId);
                if (gameMap == null)
                {
                    return;
                }

                if (!gameMap.LocalEntities.ContainsKey(id))
                {
                    return;
                }

                en = gameMap.LocalEntities[id];
            }

            if (en == null)
            {
                return;
            }

            var entityMap = MapInstance.Get(en.CurrentMap);
            if (entityMap == null)
            {
                return;
            }

            if (en.Dashing != null || en.DashQueue.Count > 0)
            {
                return;
            }

            var map = mapId;
            var x = packet.X;
            var y = packet.Y;
            var dir = packet.Direction;
            var correction = packet.Correction;
            if ((en.CurrentMap != map || en.X != x || en.Y != y) &&
                (en != Globals.Me || en == Globals.Me && correction) &&
                en.Dashing == null)
            {
                en.CurrentMap = map;
                en.X = x;
                en.Y = y;
                en.Dir = dir;
                en.IsMoving = true;

                switch (en.Dir)
                {
                    case 0:
                        en.OffsetY = Options.TileWidth;
                        en.OffsetX = 0;

                        break;
                    case 1:
                        en.OffsetY = -Options.TileWidth;
                        en.OffsetX = 0;

                        break;
                    case 2:
                        en.OffsetY = 0;
                        en.OffsetX = Options.TileWidth;

                        break;
                    case 3:
                        en.OffsetY = 0;
                        en.OffsetX = -Options.TileWidth;

                        break;
                }
            }

            // Set the Z-Dimension if the player has moved up or down a dimension.
            if (entityMap.Attributes[en.X, en.Y] != null &&
                entityMap.Attributes[en.X, en.Y].Type == MapAttributes.ZDimension)
            {
                if (((MapZDimensionAttribute) entityMap.Attributes[en.X, en.Y]).GatewayTo > 0)
                {
                    en.Z = (byte) (((MapZDimensionAttribute) entityMap.Attributes[en.X, en.Y]).GatewayTo - 1);
                }
            }
        }

        //EntityVitalsPacket
        private static void HandlePacket(EntityVitalsPacket packet)
        {
            var id = packet.Id;
            var type = packet.Type;
            var mapId = packet.MapId;
            Entity en = null;
            if (type < EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }

                en = Globals.Entities[id];
            }
            else
            {
                var entityMap = MapInstance.Get(mapId);
                if (entityMap == null)
                {
                    return;
                }

                if (!entityMap.LocalEntities.ContainsKey(id))
                {
                    return;
                }

                en = entityMap.LocalEntities[id];
            }

            if (en == null)
            {
                return;
            }

            en.Vital = packet.Vitals;
            en.MaxVital = packet.MaxVitals;

            if (en == Globals.Me)
            {
                if (packet.CombatTimeRemaining > 0)
                {
                    Globals.Me.CombatTimer = Globals.System.GetTimeMs() + packet.CombatTimeRemaining;
                }
            }

            //Update status effects
            en.Status.Clear();
            foreach (var status in packet.StatusEffects)
            {
                var instance = new Status(
                    status.SpellId, status.Type, status.TransformSprite, status.TimeRemaining, status.TotalDuration
                );

                en.Status.Add(instance);

                if (instance.Type == StatusTypes.Shield)
                {
                    instance.Shield = status.VitalShields;
                }
            }

            en.SortStatuses();

            if (Interface.Interface.GameUi != null)
            {
                //If its you or your target, update the entity box.
                if (id == Globals.Me.Id && Interface.Interface.GameUi.PlayerBox != null)
                {
                    Interface.Interface.GameUi.PlayerBox.UpdateStatuses = true;
                }
                else if (id == Globals.Me.TargetIndex && Globals.Me.TargetBox != null)
                {
                    Globals.Me.TargetBox.UpdateStatuses = true;
                }
            }
        }

        //EntityStatsPacket
        private static void HandlePacket(EntityStatsPacket packet)
        {
            var id = packet.Id;
            var type = packet.Type;
            var mapId = packet.MapId;
            Entity en = null;
            if (type < EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }

                en = Globals.Entities[id];
            }
            else
            {
                var entityMap = MapInstance.Get(mapId);
                if (entityMap == null)
                {
                    return;
                }

                if (!entityMap.LocalEntities.ContainsKey(id))
                {
                    return;
                }

                en = entityMap.LocalEntities[id];
            }

            if (en == null)
            {
                return;
            }

            en.Stat = packet.Stats;
        }

        //EntityDirectionPacket
        private static void HandlePacket(EntityDirectionPacket packet)
        {
            var id = packet.Id;
            var type = packet.Type;
            var mapId = packet.MapId;
            Entity en = null;
            if (type < EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }

                en = Globals.Entities[id];
            }
            else
            {
                var entityMap = MapInstance.Get(mapId);
                if (entityMap == null)
                {
                    return;
                }

                if (!entityMap.LocalEntities.ContainsKey(id))
                {
                    return;
                }

                en = entityMap.LocalEntities[id];
            }

            if (en == null)
            {
                return;
            }

            en.Dir = packet.Direction;
        }

        //EntityAttackPacket
        private static void HandlePacket(EntityAttackPacket packet)
        {
            var id = packet.Id;
            var type = packet.Type;
            var mapId = packet.MapId;
            var attackTimer = packet.AttackTimer;

            Entity en = null;
            if (type < EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }

                en = Globals.Entities[id];
            }
            else
            {
                var entityMap = MapInstance.Get(mapId);
                if (entityMap == null)
                {
                    return;
                }

                if (!entityMap.LocalEntities.ContainsKey(id))
                {
                    return;
                }

                en = entityMap.LocalEntities[id];
            }

            if (en == null)
            {
                return;
            }

            if (attackTimer > -1 && en != Globals.Me)
            {
                en.AttackTimer = Timing.Global.Ticks / TimeSpan.TicksPerMillisecond + attackTimer;
                en.AttackTime = attackTimer;
            }
        }

        //EntityDiePacket
        private static void HandlePacket(EntityDiePacket packet)
        {
            var id = packet.Id;
            var type = packet.Type;
            var mapId = packet.MapId;

            Entity en = null;
            if (type < EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }

                en = Globals.Entities[id];
            }
            else
            {
                var entityMap = MapInstance.Get(mapId);
                if (entityMap == null)
                {
                    return;
                }

                if (!entityMap.LocalEntities.ContainsKey(id))
                {
                    return;
                }

                en = entityMap.LocalEntities[id];
            }

            if (en == null)
            {
                return;
            }

            en.ClearAnimations(null);
        }

        //EventDialogPacket
        private static void HandlePacket(EventDialogPacket packet)
        {
            var ed = new Dialog();
            ed.Prompt = packet.Prompt;
            ed.Face = packet.Face;
            if (packet.Type != 0)
            {
                ed.Opt1 = packet.Responses[0];
                ed.Opt2 = packet.Responses[1];
                ed.Opt3 = packet.Responses[2];
                ed.Opt4 = packet.Responses[3];
            }

            ed.EventId = packet.EventId;
            Globals.EventDialogs.Add(ed);
        }

        //InputVariablePacket
        private static void HandlePacket(InputVariablePacket packet)
        {
            var type = InputBox.InputType.NumericInput;
            switch (packet.Type)
            {
                case VariableDataTypes.String:
                    type = InputBox.InputType.TextInput;

                    break;
                case VariableDataTypes.Integer:
                case VariableDataTypes.Number:
                    type = InputBox.InputType.NumericInput;

                    break;
                case VariableDataTypes.Boolean:
                    type = InputBox.InputType.YesNo;

                    break;
            }

            var iBox = new InputBox(
                packet.Title, packet.Prompt, true, type, PacketSender.SendEventInputVariable,
                PacketSender.SendEventInputVariableCancel, packet.EventId
            );
        }

        //ErrorMessagePacket
        private static void HandlePacket(ErrorMessagePacket packet)
        {
            Fade.FadeIn();
            Globals.WaitingOnServer = false;
            Interface.Interface.MsgboxErrors.Add(new KeyValuePair<string, string>(packet.Header, packet.Error));
            Interface.Interface.MenuUi.Reset();
        }

        //MapItemsPacket
        private static void HandlePacket(MapItemsPacket packet)
        {
            var map = MapInstance.Get(packet.MapId);
            if (map == null)
            {
                return;
            }

            map.MapItems.Clear();
            for (var i = 0; i < packet.Items.Length; i++)
            {
                if (packet.Items[i] != null)
                {
                    map.MapItems.Add(i, new MapItemInstance(packet.Items[i]));
                }
            }
        }

        //MapItemUpdatePacket
        private static void HandlePacket(MapItemUpdatePacket packet)
        {
            var map = MapInstance.Get(packet.MapId);
            if (map != null)
            {
                if (packet.ItemData == null)
                {
                    map.MapItems.Remove(packet.ItemIndex);
                }
                else
                {
                    if (!map.MapItems.ContainsKey(packet.ItemIndex))
                    {
                        map.MapItems.Add(packet.ItemIndex, new MapItemInstance(packet.ItemData));
                    }
                    else
                    {
                        map.MapItems[packet.ItemIndex] = new MapItemInstance(packet.ItemData);
                    }
                }
            }
        }

        //InventoryPacket
        private static void HandlePacket(InventoryPacket packet)
        {
            foreach (var inv in packet.Slots)
            {
                HandlePacket((dynamic) inv);
            }
        }

        //InventoryUpdatePacket
        private static void HandlePacket(InventoryUpdatePacket packet)
        {
            if (Globals.Me != null)
            {
                Globals.Me.Inventory[packet.Slot].Load(packet.ItemId, packet.Quantity, packet.BagId, packet.StatBuffs);
                if (Globals.Me.InventoryUpdatedDelegate != null)
                {
                    Globals.Me.InventoryUpdatedDelegate();
                }
            }
        }

        //SpellsPacket
        private static void HandlePacket(SpellsPacket packet)
        {
            foreach (var spl in packet.Slots)
            {
                HandlePacket((dynamic) spl);
            }
        }

        //SpellUpdatePacket
        private static void HandlePacket(SpellUpdatePacket packet)
        {
            if (Globals.Me != null)
            {
                Globals.Me.Spells[packet.Slot].Load(packet.SpellId);
            }
        }

        //EquipmentPacket
        private static void HandlePacket(EquipmentPacket packet)
        {
            var entityId = packet.EntityId;
            if (Globals.Entities.ContainsKey(entityId))
            {
                var entity = Globals.Entities[entityId];
                if (entity != null)
                {
                    if (entity == Globals.Me && packet.InventorySlots != null)
                    {
                        entity.MyEquipment = packet.InventorySlots;
                    }
                    else if (packet.ItemIds != null)
                    {
                        entity.Equipment = packet.ItemIds;
                    }
                }
            }
        }

        //StatPointsPacket
        private static void HandlePacket(StatPointsPacket packet)
        {
            if (Globals.Me != null)
            {
                Globals.Me.StatPoints = packet.Points;
            }
        }

        //HotbarPacket
        private static void HandlePacket(HotbarPacket packet)
        {
            for (var i = 0; i < Options.MaxHotbar; i++)
            {
                if (Globals.Me == null)
                {
                    Log.Debug("Can't set hotbar, Globals.Me is null!");

                    break;
                }

                if (Globals.Me.Hotbar == null)
                {
                    Log.Debug("Can't set hotbar, hotbar is null!");

                    break;
                }

                var hotbarEntry = Globals.Me.Hotbar[i];
                hotbarEntry.Load(packet.SlotData[i]);
            }
        }

        //CharacterCreationPacket
        private static void HandlePacket(CharacterCreationPacket packet)
        {
            Globals.WaitingOnServer = false;
            Interface.Interface.MenuUi.MainMenu.NotifyOpenCharacterCreation();
        }

        //AdminPanelPacket
        private static void HandlePacket(AdminPanelPacket packet)
        {
            Interface.Interface.GameUi.NotifyOpenAdminWindow();
        }

        //SpellCastPacket
        private static void HandlePacket(SpellCastPacket packet)
        {
            var entityId = packet.EntityId;
            var spellId = packet.SpellId;
            if (SpellBase.Get(spellId) != null && Globals.Entities.ContainsKey(entityId))
            {
                Globals.Entities[entityId].CastTime = Globals.System.GetTimeMs() + SpellBase.Get(spellId).CastDuration;
                Globals.Entities[entityId].SpellCast = spellId;
            }
        }

        //SpellCooldownPacket
        private static void HandlePacket(SpellCooldownPacket packet)
        {
            foreach (var cd in packet.SpellCds)
            {
                var time = Globals.System.GetTimeMs() + cd.Value;
                if (!Globals.Me.SpellCooldowns.ContainsKey(cd.Key))
                {
                    Globals.Me.SpellCooldowns.Add(cd.Key, time);
                }
                else
                {
                    Globals.Me.SpellCooldowns[cd.Key] = time;
                }
            }
        }

        //ItemCooldownPacket
        private static void HandlePacket(ItemCooldownPacket packet)
        {
            foreach (var cd in packet.ItemCds)
            {
                var time = Globals.System.GetTimeMs() + cd.Value;
                if (!Globals.Me.ItemCooldowns.ContainsKey(cd.Key))
                {
                    Globals.Me.ItemCooldowns.Add(cd.Key, time);
                }
                else
                {
                    Globals.Me.ItemCooldowns[cd.Key] = time;
                }
            }
        }

        //ExperiencePacket
        private static void HandlePacket(ExperiencePacket packet)
        {
            if (Globals.Me != null)
            {
                Globals.Me.Experience = packet.Experience;
                Globals.Me.ExperienceToNextLevel = packet.ExperienceToNextLevel;
            }
        }

        //ProjectileDeadPacket
        private static void HandlePacket(ProjectileDeadPacket packet)
        {
            var entityId = packet.ProjectileId;
            if (Globals.Entities.ContainsKey(entityId) && Globals.Entities[entityId].GetType() == typeof(Projectile))
            {
                ((Projectile) Globals.Entities[entityId]).SpawnDead(packet.SpawnId);
            }
        }

        //PlayAnimationPacket
        private static void HandlePacket(PlayAnimationPacket packet)
        {
            var mapId = packet.MapId;
            var animId = packet.AnimationId;
            var targetType = packet.TargetType;
            var entityId = packet.EntityId;
            if (targetType == -1)
            {
                var map = MapInstance.Get(mapId);
                if (map != null)
                {
                    map.AddTileAnimation(animId, packet.X, packet.Y, packet.Direction);
                }
            }
            else if (targetType == 1)
            {
                if (Globals.Entities.ContainsKey(entityId))
                {
                    if (Globals.Entities[entityId] != null && !Globals.EntitiesToDispose.Contains(entityId))
                    {
                        var animBase = AnimationBase.Get(animId);
                        if (animBase != null)
                        {
                            var animInstance = new Animation(
                                animBase, false, packet.Direction == -1 ? false : true, -1, Globals.Entities[entityId]
                            );

                            if (packet.Direction > -1)
                            {
                                animInstance.SetDir(packet.Direction);
                            }

                            Globals.Entities[entityId].Animations.Add(animInstance);
                        }
                    }
                }
            }
            else if (targetType == 2)
            {
                var map = MapInstance.Get(mapId);
                if (map != null)
                {
                    if (map.LocalEntities.ContainsKey(entityId))
                    {
                        if (map.LocalEntities[entityId] != null)
                        {
                            var animBase = AnimationBase.Get(animId);
                            if (animBase != null)
                            {
                                var animInstance = new Animation(
                                    animBase, false, packet.Direction == -1 ? true : false, -1,
                                    map.LocalEntities[entityId]
                                );

                                if (packet.Direction > -1)
                                {
                                    animInstance.SetDir(packet.Direction);
                                }

                                map.LocalEntities[entityId].Animations.Add(animInstance);
                            }
                        }
                    }
                }
            }
        }

        //HoldPlayerPacket
        private static void HandlePacket(HoldPlayerPacket packet)
        {
            var eventId = packet.EventId;
            var mapId = packet.MapId;
            if (!packet.Releasing)
            {
                if (!Globals.EventHolds.ContainsKey(eventId))
                {
                    Globals.EventHolds.Add(eventId, mapId);
                }
            }
            else
            {
                if (Globals.EventHolds.ContainsKey(eventId))
                {
                    Globals.EventHolds.Remove(eventId);
                }
            }
        }

        //PlayMusicPacket
        private static void HandlePacket(PlayMusicPacket packet)
        {
            Audio.PlayMusic(packet.BGM, 1f, 1f, true);
        }

        //StopMusicPacket
        private static void HandlePacket(StopMusicPacket packet)
        {
            Audio.StopMusic(3f);
        }

        //PlaySoundPacket
        private static void HandlePacket(PlaySoundPacket packet)
        {
            Audio.AddGameSound(packet.Sound, false);
        }

        //StopSoundsPacket
        private static void HandlePacket(StopSoundsPacket packet)
        {
            Audio.StopAllSounds();
        }

        //ShowPicturePacket
        private static void HandlePacket(ShowPicturePacket packet)
        {
            Globals.Picture = packet.Picture;
            Globals.PictureSize = packet.Size;
            Globals.PictureClickable = packet.Clickable;
        }

        //HidePicturePacket
        private static void HandlePacket(HidePicturePacket packet)
        {
            Globals.Picture = null;
        }

        //ShopPacket
        private static void HandlePacket(ShopPacket packet)
        {
            if (Interface.Interface.GameUi == null)
            {
                throw new ArgumentNullException(nameof(Interface.Interface.GameUi));
            }

            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (packet.ShopData != null)
            {
                Globals.GameShop = new ShopBase();
                Globals.GameShop.Load(packet.ShopData);
                Interface.Interface.GameUi.NotifyOpenShop();
            }
            else
            {
                Globals.GameShop = null;
                Interface.Interface.GameUi.NotifyCloseShop();
            }
        }

        //CraftingTablePacket
        private static void HandlePacket(CraftingTablePacket packet)
        {
            if (!packet.Close)
            {
                Globals.ActiveCraftingTable = new CraftingTableBase();
                Globals.ActiveCraftingTable.Load(packet.TableData);
                Interface.Interface.GameUi.NotifyOpenCraftingTable();
            }
            else
            {
                Interface.Interface.GameUi.NotifyCloseCraftingTable();
            }
        }

        //BankPacket
        private static void HandlePacket(BankPacket packet)
        {
            if (!packet.Close)
            {
                Interface.Interface.GameUi.NotifyOpenBank();
            }
            else
            {
                Interface.Interface.GameUi.NotifyCloseBank();
            }
        }

        //BankUpdatePacket
        private static void HandlePacket(BankUpdatePacket packet)
        {
            var slot = packet.Slot;
            if (packet.ItemId != Guid.Empty)
            {
                Globals.Bank[slot] = new Item();
                Globals.Bank[slot].Load(packet.ItemId, packet.Quantity, packet.BagId, packet.StatBuffs);
            }
            else
            {
                Globals.Bank[slot] = null;
            }
        }

        //GameObjectPacket
        private static void HandlePacket(GameObjectPacket packet)
        {
            var type = packet.Type;
            var id = packet.Id;
            var another = packet.AnotherFollowing;
            var deleted = packet.Deleted;
            var json = "";
            if (!deleted)
            {
                json = packet.Data;
            }

            switch (type)
            {
                case GameObjectType.Map:
                    //Handled in a different packet
                    break;
                case GameObjectType.Tileset:
                    var obj = new TilesetBase(id);
                    obj.Load(json);
                    TilesetBase.Lookup.Set(id, obj);
                    if (Globals.HasGameData && !another)
                    {
                        Globals.ContentManager.LoadTilesets(TilesetBase.GetNameList());
                    }

                    break;
                case GameObjectType.Event:
                    //Clients don't store event data, im an idiot.
                    break;
                default:
                    var lookup = type.GetLookup();
                    if (deleted)
                    {
                        lookup.Get(id).Delete();
                    }
                    else
                    {
                        lookup.DeleteAt(id);
                        var item = lookup.AddNew(type.GetObjectType(), id);
                        item.Load(json);
                    }

                    break;
            }
        }

        //EntityDashPacket
        private static void HandlePacket(EntityDashPacket packet)
        {
            if (Globals.Entities.ContainsKey(packet.EntityId))
            {
                Globals.Entities[packet.EntityId]
                    .DashQueue.Enqueue(
                        new Dash(
                            Globals.Entities[packet.EntityId], packet.EndMapId, packet.EndX, packet.EndY,
                            packet.DashTime, packet.Direction
                        )
                    );
            }
        }

        //MapGridPacket
        private static void HandlePacket(MapGridPacket packet)
        {
            Globals.MapGridWidth = packet.Grid.GetLength(0);
            Globals.MapGridHeight = packet.Grid.GetLength(1);
            var clearKnownMaps = packet.ClearKnownMaps;
            Globals.MapGrid = new Guid[Globals.MapGridWidth, Globals.MapGridHeight];
            if (clearKnownMaps)
            {
                foreach (var map in MapInstance.Lookup.Values.ToArray())
                {
                    ((MapInstance) map).Dispose();
                }
            }

            Globals.NeedsMaps = true;
            Globals.GridMaps.Clear();
            for (var x = 0; x < Globals.MapGridWidth; x++)
            {
                for (var y = 0; y < Globals.MapGridHeight; y++)
                {
                    Globals.MapGrid[x, y] = packet.Grid[x, y];
                    if (Globals.MapGrid[x, y] != Guid.Empty)
                    {
                        Globals.GridMaps.Add(Globals.MapGrid[x, y]);
                        if (MapInstance.MapRequests.ContainsKey(Globals.MapGrid[x, y]))
                        {
                            MapInstance.MapRequests[Globals.MapGrid[x, y]] = Globals.System.GetTimeMs() + 2000;
                        }
                        else
                        {
                            MapInstance.MapRequests.Add(Globals.MapGrid[x, y], Globals.System.GetTimeMs() + 2000);
                        }
                    }
                }
            }

            if (Globals.Me != null)
            {
                Globals.Me.FetchNewMaps();
            }

            Graphics.GridSwitched = true;
        }

        //TimePacket
        private static void HandlePacket(TimePacket packet)
        {
            Time.LoadTime(
                packet.Time, Color.FromArgb(packet.Color.A, packet.Color.R, packet.Color.G, packet.Color.B), packet.Rate
            );
        }

        //PartyPacket
        private static void HandlePacket(PartyPacket packet)
        {
            if (Globals.Me == null || Globals.Me.Party == null)
            {
                return;
            }

            Globals.Me.Party.Clear();
            for (var i = 0; i < packet.MemberData.Length; i++)
            {
                var mem = packet.MemberData[i];
                Globals.Me.Party.Add(new PartyMember(mem.Id, mem.Name, mem.Vital, mem.MaxVital, mem.Level));
            }
        }

        //PartyUpdatePacket
        private static void HandlePacket(PartyUpdatePacket packet)
        {
            var index = packet.MemberIndex;
            if (index < Globals.Me.Party.Count)
            {
                var mem = packet.MemberData;
                Globals.Me.Party[index] = new PartyMember(mem.Id, mem.Name, mem.Vital, mem.MaxVital, mem.Level);
            }
        }

        //PartyInvitePacket
        private static void HandlePacket(PartyInvitePacket packet)
        {
            var iBox = new InputBox(
                Strings.Parties.partyinvite, Strings.Parties.inviteprompt.ToString(packet.LeaderName), true,
                InputBox.InputType.YesNo, PacketSender.SendPartyAccept, PacketSender.SendPartyDecline, packet.LeaderId
            );
        }

        //ChatBubblePacket
        private static void HandlePacket(ChatBubblePacket packet)
        {
            var id = packet.EntityId;
            var type = packet.Type;
            var mapId = packet.MapId;
            Entity en = null;
            if (type < EntityTypes.Event)
            {
                if (!Globals.Entities.ContainsKey(id))
                {
                    return;
                }

                en = Globals.Entities[id];
            }
            else
            {
                var entityMap = MapInstance.Get(mapId);
                if (entityMap == null)
                {
                    return;
                }

                if (!entityMap.LocalEntities.ContainsKey(id))
                {
                    return;
                }

                en = entityMap.LocalEntities[id];
            }

            if (en == null)
            {
                return;
            }

            en.AddChatBubble(packet.Text);
        }

        //QuestOfferPacket
        private static void HandlePacket(QuestOfferPacket packet)
        {
            if (!Globals.QuestOffers.Contains(packet.QuestId))
            {
                Globals.QuestOffers.Add(packet.QuestId);
            }
        }

        //QuestProgressPacket
        private static void HandlePacket(QuestProgressPacket packet)
        {
            if (Globals.Me != null)
            {
                foreach (var quest in packet.Quests)
                {
                    if (quest.Value == null)
                    {
                        if (Globals.Me.QuestProgress.ContainsKey(quest.Key))
                        {
                            Globals.Me.QuestProgress.Remove(quest.Key);
                        }
                    }
                    else
                    {
                        if (Globals.Me.QuestProgress.ContainsKey(quest.Key))
                        {
                            Globals.Me.QuestProgress[quest.Key] = new QuestProgress(quest.Value);
                        }
                        else
                        {
                            Globals.Me.QuestProgress.Add(quest.Key, new QuestProgress(quest.Value));
                        }
                    }
                }

                if (Interface.Interface.GameUi != null)
                {
                    Interface.Interface.GameUi.NotifyQuestsUpdated();
                }
            }
        }

        //TradePacket
        private static void HandlePacket(TradePacket packet)
        {
            if (!string.IsNullOrEmpty(packet.TradePartner))
            {
                Globals.Trade = new Item[2, Options.MaxInvItems];

                //Gotta initialize the trade values
                for (var x = 0; x < 2; x++)
                {
                    for (var y = 0; y < Options.MaxInvItems; y++)
                    {
                        Globals.Trade[x, y] = new Item();
                    }
                }

                Interface.Interface.GameUi.NotifyOpenTrading(packet.TradePartner);
            }
            else
            {
                Interface.Interface.GameUi.NotifyCloseTrading();
            }
        }

        //TradeUpdatePacket
        private static void HandlePacket(TradeUpdatePacket packet)
        {
            var side = 0;

            if (packet.TraderId != Globals.Me.Id)
            {
                side = 1;
            }

            var slot = packet.Slot;
            if (packet.ItemId == Guid.Empty)
            {
                Globals.Trade[side, slot] = null;
            }
            else
            {
                Globals.Trade[side, slot] = new Item();
                Globals.Trade[side, slot].Load(packet.ItemId, packet.Quantity, packet.BagId, packet.StatBuffs);
            }
        }

        //TradeRequestPacket
        private static void HandlePacket(TradeRequestPacket packet)
        {
            var iBox = new InputBox(
                Strings.Trading.traderequest, Strings.Trading.requestprompt.ToString(packet.PartnerName), true,
                InputBox.InputType.YesNo, PacketSender.SendTradeRequestAccept, PacketSender.SendTradeRequestDecline,
                packet.PartnerId
            );
        }

        //NpcAggressionPacket
        private static void HandlePacket(NpcAggressionPacket packet)
        {
            if (Globals.Entities.ContainsKey(packet.EntityId))
            {
                Globals.Entities[packet.EntityId].Type = packet.Aggression;
            }
        }

        //PlayerDeathPacket
        private static void HandlePacket(PlayerDeathPacket packet)
        {
            if (Globals.Entities.ContainsKey(packet.PlayerId))
            {
                //Clear all dashes.
                Globals.Entities[packet.PlayerId].DashQueue.Clear();
                Globals.Entities[packet.PlayerId].Dashing = null;
                Globals.Entities[packet.PlayerId].DashTimer = 0;
            }
        }

        //EntityZDimensionPacket
        private static void HandlePacket(EntityZDimensionPacket packet)
        {
            if (Globals.Entities.ContainsKey(packet.EntityId))
            {
                Globals.Entities[packet.EntityId].Z = packet.Level;
            }
        }

        //BagPacket
        private static void HandlePacket(BagPacket packet)
        {
            if (!packet.Close)
            {
                Globals.Bag = new Item[packet.Slots];
                Interface.Interface.GameUi.NotifyOpenBag();
            }
            else
            {
                Interface.Interface.GameUi.NotifyCloseBag();
            }
        }

        //BagUpdatePacket
        private static void HandlePacket(BagUpdatePacket packet)
        {
            if (packet.ItemId == Guid.Empty)
            {
                Globals.Bag[packet.Slot] = null;
            }
            else
            {
                Globals.Bag[packet.Slot] = new Item();
                Globals.Bag[packet.Slot].Load(packet.ItemId, packet.Quantity, packet.BagId, packet.StatBuffs);
            }
        }

        //MoveRoutePacket
        private static void HandlePacket(MoveRoutePacket packet)
        {
            Globals.MoveRouteActive = packet.Active;
        }

        //FriendsPacket
        private static void HandlePacket(FriendsPacket packet)
        {
            Globals.Me.Friends.Clear();

            foreach (var friend in packet.OnlineFriends)
            {
                var f = new FriendInstance()
                {
                    Name = friend.Key,
                    Map = friend.Value,
                    Online = true
                };

                Globals.Me.Friends.Add(f);
            }

            foreach (var friend in packet.OfflineFriends)
            {
                var f = new FriendInstance()
                {
                    Name = friend,
                    Online = false
                };

                Globals.Me.Friends.Add(f);
            }

            Interface.Interface.GameUi.NotifyUpdateFriendsList();
        }

        //FriendRequestPacket
        private static void HandlePacket(FriendRequestPacket packet)
        {
            var iBox = new InputBox(
                Strings.Friends.request, Strings.Friends.requestprompt.ToString(packet.FriendName), true,
                InputBox.InputType.YesNo, PacketSender.SendFriendRequestAccept, PacketSender.SendFriendRequestDecline,
                packet.FriendId
            );
        }

        //CharactersPacket
        private static void HandlePacket(CharactersPacket packet)
        {
            var characters = new List<Character>();

            foreach (var chr in packet.Characters)
            {
                characters.Add(
                    new Character(chr.Id, chr.Name, chr.Sprite, chr.Face, chr.Level, chr.ClassName, chr.Equipment)
                );
            }

            if (packet.FreeSlot)
            {
                characters.Add(null);
            }

            Globals.WaitingOnServer = false;
            Interface.Interface.MenuUi.MainMenu.NotifyOpenCharacterSelection(characters);
        }

        //PasswordResetResultPacket
        private static void HandlePacket(PasswordResetResultPacket packet)
        {
            if (packet.Succeeded)
            {
                //Show Success Message and Open Login Screen
                Interface.Interface.MsgboxErrors.Add(
                    new KeyValuePair<string, string>(Strings.ResetPass.success, Strings.ResetPass.successmsg)
                );

                Interface.Interface.MenuUi.MainMenu.NotifyOpenLogin();
            }
            else
            {
                //Show Error Message
                Interface.Interface.MsgboxErrors.Add(
                    new KeyValuePair<string, string>(Strings.ResetPass.fail, Strings.ResetPass.failmsg)
                );
            }

            Globals.WaitingOnServer = false;
        }

        //TargetOverridePacket
        private static void HandlePacket(TargetOverridePacket packet)
        {
            if (Globals.Entities.ContainsKey(packet.TargetId))
            {
                Globals.Me.TargetIndex = packet.TargetId;
            }
        }

        //EnteringGamePacket
        private static void HandlePacket(EnteringGamePacket packet)
        {
            //Fade out, we're finally loading the game world!
            Fade.FadeOut();
        }

    }

}
