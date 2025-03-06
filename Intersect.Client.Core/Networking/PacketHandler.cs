using Intersect.Client.Core;
using Intersect.Client.Entities;
using Intersect.Client.Entities.Events;
using Intersect.Client.Entities.Projectiles;
using Intersect.Client.Framework.Entities;
using Intersect.Client.Framework.Items;
using Intersect.Client.General;
using Intersect.Client.Interface.Game.Chat;
using Intersect.Client.Interface.Menu;
using Intersect.Client.Items;
using Intersect.Client.Localization;
using Intersect.Client.Maps;
using Intersect.Configuration;
using Intersect.Core;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Network;
using Intersect.Network.Packets;
using Intersect.Network.Packets.Server;
using Intersect.Utilities;
using Intersect.Framework;
using Intersect.Models;
using Intersect.Client.Interface.Shared;
using Intersect.Framework.Core;
using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.Framework.Core.GameObjects.Crafting;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Mapping.Tilesets;
using Intersect.Framework.Core.GameObjects.Maps;
using Intersect.Framework.Core.GameObjects.Maps.Attributes;
using Intersect.Framework.Core.GameObjects.Maps.MapList;
using Microsoft.Extensions.Logging;

namespace Intersect.Client.Networking;


internal sealed partial class PacketHandler
{
    private sealed partial class VirtualPacketSender : IPacketSender
    {
        public IApplicationContext ApplicationContext { get; }

        public INetwork Network => Networking.Network.Socket.Network;

        public VirtualPacketSender(IApplicationContext applicationContext) =>
            ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));

        #region Implementation of IPacketSender

        /// <inheritdoc />
        public bool Send(IPacket packet)
        {
            if (packet is IntersectPacket intersectPacket)
            {
                Networking.Network.SendPacket(intersectPacket);
                return true;
            }

            return false;
        }

        #endregion
    }

    public long Ping { get; set; } = 0;

    public long PingTime { get; set; }

    public IClientContext Context { get; }

    public ILogger Logger => Context.Logger;

    public PacketHandlerRegistry Registry { get; }

    public IPacketSender VirtualSender { get; }

    public PacketHandler(IClientContext context, PacketHandlerRegistry packetHandlerRegistry)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Registry = packetHandlerRegistry ?? throw new ArgumentNullException(nameof(packetHandlerRegistry));

        if (!Registry.TryRegisterAvailableMethodHandlers(GetType(), this, false) || Registry.IsEmpty)
        {
            throw new InvalidOperationException("Failed to register method handlers, see logs for more details.");
        }

        if (!Registry.TryRegisterAvailableTypeHandlers(GetType().Assembly))
        {
            throw new InvalidOperationException("Failed to register type handlers, see logs for more details.");
        }

        VirtualSender = new VirtualPacketSender(context);
    }

    public bool HandlePacket(IPacket packet)
    {
        if (packet is AbstractTimedPacket timedPacket)
        {
            Timing.Global.Synchronize(timedPacket.UTC, timedPacket.Offset);
        }

        if (!(packet is IntersectPacket))
        {
            return false;
        }

        if (!packet.IsValid)
        {
            return false;
        }

        if (!Registry.TryGetHandler(packet, out HandlePacketGeneric handler))
        {
            Logger.LogError($"No registered handler for {packet.GetType().FullName}!");

            return false;
        }

        if (Registry.TryGetPreprocessors(packet, out var preprocessors))
        {
            if (!preprocessors.All(preprocessor => preprocessor.Handle(VirtualSender, packet)))
            {
                // Preprocessors are intended to be silent filter functions
                return false;
            }
        }

        if (Registry.TryGetPreHooks(packet, out var preHooks))
        {
            if (!preHooks.All(hook => hook.Handle(VirtualSender, packet)))
            {
                // Hooks should not fail, if they do that's an error
                Logger.LogError($"PreHook handler failed for {packet.GetType().FullName}.");
                return false;
            }
        }

        if (!handler(VirtualSender, packet))
        {
            return false;
        }

        if (Registry.TryGetPostHooks(packet, out var postHooks))
        {
            if (!postHooks.All(hook => hook.Handle(VirtualSender, packet)))
            {
                // Hooks should not fail, if they do that's an error
                Logger.LogError($"PostHook handler failed for {packet.GetType().FullName}.");
                return false;
            }
        }

        return true;
    }

    //PingPacket
    public void HandlePacket(IPacketSender packetSender, Intersect.Network.Packets.Server.PingPacket packet)
    {
        if (!packet.RequestingReply)
        {
            return;
        }

        PacketSender.SendPing();
        PingTime = Timing.Global.Milliseconds;
    }

    //ConfigPacket
    public void HandlePacket(IPacketSender packetSender, ConfigPacket packet)
    {
        ApplicationContext.Context.Value?.Logger.LogDebug("Received configuration from server.");
        Options.LoadFromServer(packet.Config);
        Globals.WaitingOnServer = false;
        MainMenu.HandleReceivedConfiguration();
        try
        {
            Strings.Load();
        }
        catch (Exception exception)
        {
            ApplicationContext.Context.Value?.Logger.LogError(exception, "Error loading strings");
            throw;
        }
        Graphics.InitInGame();
    }

    //JoinGamePacket
    public void HandlePacket(IPacketSender packetSender, JoinGamePacket packet)
    {
        Main.JoinGame();
        Globals.JoiningGame = true;
    }

    public void HandlePacket(IPacketSender packetSender, MapAreaPacket packet)
    {
        foreach (var map in packet.Maps)
        {
            HandleMap(packetSender, map);
        }
    }

    public void HandlePacket(IPacketSender packetSender, MapAreaIdsPacket packet)
    {
        // TODO: Background all of this?
        List<ObjectCacheKey<MapDescriptor>> cacheKeys = new(packet.MapIds.Length);
        List<MapPacket> loadedCachedMaps = new(packet.MapIds.Length);
        foreach (var mapId in packet.MapIds)
        {
            if (ObjectDataDiskCache<MapDescriptor>.TryLoad(mapId, out var cacheData))
            {
                ObjectCacheKey<MapDescriptor> cacheKey = new(cacheData.Id);
                var deserializedCachedPacket = MessagePacker.Instance.Deserialize<MapPacket>(cacheData.Data, silent: true);
                if (deserializedCachedPacket != default)
                {
                    cacheKey = new ObjectCacheKey<MapDescriptor>(
                        cacheData.Id,
                        cacheData.Checksum,
                        cacheData.Version
                    );
                    cacheKeys.Add(cacheKey);
                    loadedCachedMaps.Add(deserializedCachedPacket);
                    continue;
                }

                ApplicationContext.Context.Value?.Logger.LogWarning($"Failed to deserialized cached data for {cacheKey}, will fetch again");
            }

            cacheKeys.Add(new ObjectCacheKey<MapDescriptor>(new Id<MapDescriptor>(mapId)));
        }

        PacketSender.SendNeedMap(cacheKeys.ToArray());

        foreach (var cachedMap in loadedCachedMaps)
        {
            HandleMap(packetSender, cachedMap, skipSave: true);
        }
    }

    private void HandleMap(IPacketSender packetSender, MapPacket packet, bool skipSave = false)
    {
        var mapId = packet.MapId;

        if (!skipSave)
        {
            ApplicationContext.CurrentContext.Logger.LogDebug(
                "Saving map {Id} @ ({GridX}, {GridY}) revision {Revision} version {Version} holds {CameraHolds}",
                packet.MapId,
                packet.GridX,
                packet.GridY,
                packet.Revision,
                packet.CacheVersion,
                $"[{string.Join(", ", packet.CameraHolds ?? [])}]"
            );

            ObjectCacheData<MapDescriptor> cacheData = new()
            {
                Id = new Id<MapDescriptor>(mapId),
                Data = (packet as IntersectPacket).Data,
                Version = packet.CacheVersion,
            };
            ObjectCacheKey<MapDescriptor> cacheKey = new(new Id<MapDescriptor>(mapId), cacheData.Checksum, cacheData.Version);

            if (!ObjectDataDiskCache<MapDescriptor>.TrySave(cacheData))
            {
                ApplicationContext.CurrentContext.Logger.LogWarning("Failed to save cache for {CacheKey}", cacheKey);
            }
        }

        MapInstance.UpdateMapRequestTime(packet.MapId);

        if (MapInstance.TryGet(mapId, out var mapInstance))
        {
            if (skipSave && packet.Revision == mapInstance.Revision)
            {
                return;
            }

            mapInstance.Dispose(false, false);
        }

        mapInstance = new MapInstance(mapId);
        MapInstance.Lookup.Set(mapId, mapInstance);
        lock (mapInstance.MapLock)
        {
            mapInstance.Load(packet.Data);
            mapInstance.LoadTileData(packet.TileData);
            mapInstance.AttributeData = packet.AttributeData;
            mapInstance.CreateMapSounds();

            if (mapId == Globals.Me?.MapId)
            {
                Audio.PlayMusic(
                    mapInstance.Music,
                    ClientConfiguration.Instance.MusicFadeTimer,
                    ClientConfiguration.Instance.MusicFadeTimer,
                    true
                );
            }

            if (!Globals.GridMaps.TryGetValue(packet.MapId, out var gridPosition))
            {
                ApplicationContext.CurrentContext.Logger.LogDebug(
                    "Falling back to packet position for map '{MapName}' ({MapId})",
                    mapInstance.Name,
                    mapInstance.Id
                );
                gridPosition = new Point(packet.GridX, packet.GridY);
            }

            mapInstance.GridX = gridPosition.X;
            mapInstance.GridY = gridPosition.Y;
            mapInstance.CameraHolds = packet.CameraHolds ?? [false, false, false, false];

            ApplicationContext.CurrentContext.Logger.LogDebug(
                "Loading map {Id} ({Name}) @ ({GridX}, {GridY}) revision {Revision} version {Version} holds {CameraHolds}",
                mapInstance.Id,
                mapInstance.Name,
                gridPosition.X,
                gridPosition.Y,
                mapInstance.Revision,
                packet.CacheVersion,
                $"[{string.Join(", ", mapInstance.CameraHolds)}]"
            );

            mapInstance.Autotiles.InitAutotiles(mapInstance.GenerateAutotileGrid());

            if (Globals.PendingEvents.TryGetValue(mapId, out var pendingEventsForMap))
            {
                foreach (var (eventId, eventEntityPacket) in pendingEventsForMap)
                {
                    mapInstance.AddEvent(eventId, eventEntityPacket);
                }

                pendingEventsForMap.Clear();
            }
        }

        mapInstance.MarkLoadFinished();
    }

    //MapPacket
    public void HandlePacket(IPacketSender packetSender, MapPacket packet)
    {
        HandleMap(packetSender, packet);
        Player.FetchNewMaps();
    }

    //PlayerEntityPacket
    public void HandlePacket(IPacketSender packetSender, PlayerEntityPacket packet)
    {
        var en = Globals.GetEntity(packet.EntityId, EntityType.Player);
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
    public void HandlePacket(IPacketSender packetSender, NpcEntityPacket packet)
    {
        var en = Globals.GetEntity(packet.EntityId, EntityType.GlobalEntity);
        if (en != null)
        {
            en.Load(packet);
            en.Aggression = packet.Aggression;
        }
        else
        {
            var entity = new Entity(packet.EntityId, packet, EntityType.GlobalEntity)
            {
                Aggression = packet.Aggression,
            };
            Globals.Entities.Add(entity.Id, entity);
        }
    }

    //ResourceEntityPacket
    public void HandlePacket(IPacketSender packetSender, ResourceEntityPacket packet)
    {
        if (Globals.TryGetEntity(EntityType.Resource, packet.EntityId, out var entity))
        {
            entity.Load(packet);
        }
        else
        {
            entity = new Resource(packet.EntityId, packet);
            if (!Globals.Entities.TryAdd(entity.Id, entity))
            {
                ApplicationContext.CurrentContext.Logger.LogError(
                    "Failed to register new {EntityType} {EntityId} ({EntityName})",
                    EntityType.Resource,
                    packet.EntityId,
                    packet.Name
                );
            }
        }
    }

    //ProjectileEntityPacket
    public void HandlePacket(IPacketSender packetSender, ProjectileEntityPacket packet)
    {
        var en = Globals.GetEntity(packet.EntityId, EntityType.Projectile);
        if (en != null)
        {
            en.Load(packet);
        }
        else
        {
            var entity = new Projectile(packet.EntityId, packet);
            Globals.Entities.Add(entity.Id, entity);
        }
    }

    //EventEntityPacket
    public void HandlePacket(IPacketSender packetSender, EventEntityPacket packet)
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
    public void HandlePacket(IPacketSender packetSender, MapEntitiesPacket entitiesPacket)
    {
        Dictionary<Guid, HashSet<Guid>> entitiesByMapId = [];
        foreach (var entityPacket in entitiesPacket.MapEntities)
        {
            HandlePacket(entityPacket);

            if (!entitiesByMapId.TryGetValue(entityPacket.MapId, out var value))
            {
                value = [];
                entitiesByMapId.Add(entityPacket.MapId, value);
            }

            value.Add(entityPacket.EntityId);
        }

        // Remove any entities on the map that shouldn't be there anymore!
        foreach (var (mapId, entitiesOnMap) in entitiesByMapId)
        {
            foreach (var (entityId, entity) in Globals.Entities)
            {
                if (entity.MapId != mapId || entitiesOnMap.Contains(entityId))
                {
                    continue;
                }

                if (!Globals.EntitiesToDispose.Contains(entityId) && entity != Globals.Me && entity is not Projectile)
                {
                    Globals.EntitiesToDispose.Add(entityId);
                }
            }
        }
    }

    // MapInstanceChanged Packet
    public void HandlePacket(IPacketSender packetSender, MapInstanceChangedPacket packet)
    {
        var disposingEntities = new List<Guid>();
        foreach (var pkt in packet.EntitiesToDispose)
        {
            HandlePacket(pkt);
            disposingEntities.Add(pkt.EntityId);
        }

        foreach (var entity in disposingEntities)
        {
            Globals.EntitiesToDispose.Add(entity);
        }

        foreach (var mapId in packet.MapIdsToRefresh)
        {
            var map = MapInstance.Get(mapId);
            if (map != null)
            {
                Globals.EntitiesToDispose.AddRange(map.LocalEntities.Values
                    .ToList()
                    .Select(en => en.Id));
            }
        }
    }

    //EntityPositionPacket
    public void HandlePacket(IPacketSender packetSender, EntityPositionPacket packet)
    {
        var id = packet.Id;
        var type = packet.Type;
        var mapId = packet.MapId;
        Entity en;
        if (type != EntityType.Event)
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
            ApplicationContext.Context.Value?.Logger.LogDebug($"received epp: {Timing.Global.Milliseconds}");
        }

        if (en == Globals.Me &&
            (Globals.Me.DashQueue.Count > 0 || Globals.Me.DashTimer > Timing.Global.Milliseconds))
        {
            return;
        }

        if (en == Globals.Me && Globals.Me.MapId != mapId)
        {
            Globals.Me.MapId = mapId;
            Globals.NeedsMaps = true;
            Player.FetchNewMaps();
        }
        else
        {
            en.MapId = mapId;
        }

        en.X = packet.X;
        en.Y = packet.Y;
        en.DirectionFacing = (Direction)packet.Direction;
        en.Passable = packet.Passable;
        en.HideName = packet.HideName;
    }

    //EntityLeftPacket
    public void HandlePacket(IPacketSender packetSender, EntityLeftPacket packet)
    {
        var id = packet.Id;
        var type = packet.Type;
        var mapId = packet.MapId;
        if (id == Globals.Me?.Id && type < EntityType.Event)
        {
            return;
        }

        if (type != EntityType.Event)
        {
            if (Globals.Entities?.ContainsKey(id) ?? false)
            {
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
    public void HandlePacket(IPacketSender packetSender, Intersect.Network.Packets.Server.ChatMsgPacket packet)
    {
        ChatboxMsg.AddMessage(
            new ChatboxMsg(
                packet.Message ?? "", new Color(packet.Color.A, packet.Color.R, packet.Color.G, packet.Color.B), packet.Type,
                packet.Target
            )
        );
    }

    //AnnouncementPacket
    public void HandlePacket(IPacketSender packetSender, AnnouncementPacket packet)
    {
        Interface.Interface.EnqueueInGame(
            gameInterface => gameInterface.AnnouncementWindow.ShowAnnouncement(packet.Message, packet.Duration)
        );
    }

    //ActionMsgPackets
    public void HandlePacket(IPacketSender packetSender, ActionMsgPackets packet)
    {
        foreach (var pkt in packet.Packets)
        {
            HandlePacket(pkt);
        }
    }

    //ActionMsgPacket
    public void HandlePacket(IPacketSender packetSender, ActionMsgPacket packet)
    {
        var map = MapInstance.Get(packet.MapId);
        if (map != null)
        {
            map.ActionMessages.Add(
                new ActionMessage(
                    map, packet.X, packet.Y, packet.Message,
                    new Color(packet.Color.A, packet.Color.R, packet.Color.G, packet.Color.B)
                )
            );
        }
    }

    //GameDataPacket
    public void HandlePacket(IPacketSender packetSender, GameDataPacket packet)
    {
        foreach (var pkt in packet.GameObjects)
        {
            HandlePacket(pkt);
        }

        CustomColors.Load(packet.ColorsJson);
        Globals.HasGameData = true;
    }

    //MapListPacket
    public void HandlePacket(IPacketSender packetSender, MapListPacket packet)
    {
        MapList.List.JsonData = packet.MapListData;
        MapList.List.PostLoad(MapDescriptor.Lookup, false, true);

        //TODO ? If admin window is open update it
    }

    //EntityMovementPackets
    public void HandlePacket(IPacketSender packetSender, EntityMovementPackets packet)
    {
        if (packet.GlobalMovements != null)
        {
            foreach (var pkt in packet.GlobalMovements)
            {
                HandlePacket(pkt);
            }
        }

        if (packet.LocalMovements != null)
        {
            foreach (var pkt in packet.LocalMovements)
            {
                HandlePacket(pkt);
            }
        }
    }

    //EntityMovePacket
    public void HandlePacket(IPacketSender packetSender, EntityMovePacket packet)
    {
        var id = packet.Id;
        var type = packet.Type;
        var mapId = packet.MapId;
        Entity en;
        if (type < EntityType.Event)
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

        var entityMap = MapInstance.Get(en.MapId);
        if (entityMap == null)
        {
            return;
        }

        if (en is Player && Options.Instance.Combat.MovementCancelsCast)
        {
            en.CastTime = 0;
        }

        if (en.Dashing != null || en.DashQueue.Count > 0)
        {
            return;
        }

        var map = mapId;
        var x = packet.X;
        var y = packet.Y;
        Direction dir = (Direction)packet.Direction;
        var correction = packet.Correction;
        if ((en.MapId != map || en.X != x || en.Y != y) &&
            (en != Globals.Me || en == Globals.Me && correction) &&
            en.Dashing == null)
        {
            en.MapId = map;
            en.X = x;
            en.Y = y;
            en.DirectionFacing = dir;
            if (en is Player p)
            {
                p.DirectionMoving = dir;
            }
            en.IsMoving = true;

            switch (en.DirectionFacing)
            {
                case Direction.Up:
                    en.OffsetY = Options.Instance.Map.TileWidth;
                    en.OffsetX = 0;

                    break;
                case Direction.Down:
                    en.OffsetY = -Options.Instance.Map.TileWidth;
                    en.OffsetX = 0;

                    break;
                case Direction.Left:
                    en.OffsetY = 0;
                    en.OffsetX = Options.Instance.Map.TileWidth;

                    break;
                case Direction.Right:
                    en.OffsetY = 0;
                    en.OffsetX = -Options.Instance.Map.TileWidth;

                    break;
                case Direction.UpLeft:
                    en.OffsetY = Options.Instance.Map.TileHeight;
                    en.OffsetX = Options.Instance.Map.TileWidth;

                    break;
                case Direction.UpRight:
                    en.OffsetY = Options.Instance.Map.TileHeight;
                    en.OffsetX = -Options.Instance.Map.TileWidth;

                    break;
                case Direction.DownLeft:
                    en.OffsetY = -Options.Instance.Map.TileHeight;
                    en.OffsetX = Options.Instance.Map.TileWidth;

                    break;
                case Direction.DownRight:
                    en.OffsetY = -Options.Instance.Map.TileHeight;
                    en.OffsetX = -Options.Instance.Map.TileWidth;

                    break;
            }
        }

        // Set the Z-Dimension if the player has moved up or down a dimension.
        if (entityMap.Attributes[en.X, en.Y] != null &&
            entityMap.Attributes[en.X, en.Y].Type == MapAttributeType.ZDimension)
        {
            if (((MapZDimensionAttribute) entityMap.Attributes[en.X, en.Y]).GatewayTo > 0)
            {
                en.Z = (byte) (((MapZDimensionAttribute) entityMap.Attributes[en.X, en.Y]).GatewayTo - 1);
            }
        }
    }

    public void HandlePacket(IPacketSender packetSender, MapEntityVitalsPacket packet)
    {
        // Get our map, cancel out if it doesn't exist.
        var map = MapInstance.Get(packet.MapId);
        if (map == null)
        {
            return;
        }

        foreach (var en in packet.EntityUpdates)
        {
            Entity entity = null;

            if (en.Type < EntityType.Event)
            {
                if (!Globals.Entities.ContainsKey(en.Id))
                {
                    return;
                }

                entity = Globals.Entities[en.Id];
            }
            else
            {
                if (!map.LocalEntities.ContainsKey(en.Id))
                {
                    return;
                }

                entity = map.LocalEntities[en.Id];
            }

            if (entity == null)
            {
                return;
            }

            entity.Vital = en.Vitals;
            entity.MaxVital = en.MaxVitals;

            if (entity == Globals.Me)
            {
                if (en.CombatTimeRemaining > 0)
                {
                    Globals.Me.CombatTimer = Timing.Global.Milliseconds + en.CombatTimeRemaining;
                }
            }
        }
    }

    public void HandlePacket(IPacketSender packetSender, MapEntityStatusPacket packet)
    {
        // Get our map, cancel out if it doesn't exist.
        var map = MapInstance.Get(packet.MapId);
        if (map == null)
        {
            return;
        }

        foreach (var en in packet.EntityUpdates)
        {
            Entity entity = null;

            if (en.Type < EntityType.Event)
            {
                if (!Globals.Entities.ContainsKey(en.Id))
                {
                    return;
                }

                entity = Globals.Entities[en.Id];
            }
            else
            {
                if (!map.LocalEntities.ContainsKey(en.Id))
                {
                    return;
                }

                entity = map.LocalEntities[en.Id];
            }

            if (entity == null)
            {
                return;
            }

            //Update status effects
            entity.Status.Clear();
            foreach (var status in en.Statuses)
            {
                var instance = new Status(
                    status.SpellId, status.Type, status.TransformSprite, status.TimeRemaining, status.TotalDuration
                );

                entity.Status.Add(instance);

                if (instance.Type == SpellEffect.Stun || instance.Type == SpellEffect.Silence)
                {
                    entity.CastTime = 0;
                }
                else if (instance.Type == SpellEffect.Shield)
                {
                    instance.Shield = status.VitalShields;
                }
            }

            entity.SortStatuses();

            if (!Interface.Interface.HasInGameUI)
            {
                continue;
            }

            //If its you or your target, update the entity box.
            if (en.Id == Globals.Me.Id && Interface.Interface.GameUi.PlayerStatusWindow != null)
            {
                Interface.Interface.GameUi.PlayerStatusWindow.ShouldUpdateStatuses = true;
            }
            else if (en.Id == Globals.Me.TargetId && Globals.Me.TargetBox != null)
            {
                Globals.Me.TargetBox.ShouldUpdateStatuses = true;
            }
        }
    }

    //EntityVitalsPacket
    public void HandlePacket(IPacketSender packetSender, EntityVitalsPacket packet)
    {
        var id = packet.Id;
        var type = packet.Type;
        var mapId = packet.MapId;
        Entity en = null;
        if (type < EntityType.Event)
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
                Globals.Me.CombatTimer = Timing.Global.Milliseconds + packet.CombatTimeRemaining;
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

            if (instance.Type == SpellEffect.Stun || instance.Type == SpellEffect.Silence)
            {
                en.CastTime = 0;
            }
            else if (instance.Type == SpellEffect.Shield)
            {
                instance.Shield = status.VitalShields;
            }
        }

        en.SortStatuses();

        if (!Interface.Interface.HasInGameUI)
        {
            return;
        }

        //If its you or your target, update the entity box.
        if (id == Globals.Me.Id && Interface.Interface.GameUi.PlayerStatusWindow != null)
        {
            Interface.Interface.GameUi.PlayerStatusWindow.ShouldUpdateStatuses = true;
        }
        else if (id == Globals.Me.TargetId && Globals.Me.TargetBox != null)
        {
            Globals.Me.TargetBox.ShouldUpdateStatuses = true;
        }
    }

    //EntityStatsPacket
    public void HandlePacket(IPacketSender packetSender, EntityStatsPacket packet)
    {
        var id = packet.Id;
        var type = packet.Type;
        var mapId = packet.MapId;
        Entity en = null;
        if (type < EntityType.Event)
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
    public void HandlePacket(IPacketSender packetSender, EntityDirectionPacket packet)
    {
        var id = packet.Id;
        var type = packet.Type;
        var mapId = packet.MapId;
        Entity en = null;
        if (type < EntityType.Event)
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

        en.DirectionFacing = (Direction)packet.Direction;
    }

    //EntityAttackPacket
    public void HandlePacket(IPacketSender packetSender, EntityAttackPacket packet)
    {
        var id = packet.Id;
        var type = packet.Type;
        var mapId = packet.MapId;
        var attackTimer = packet.AttackTimer;

        Entity en = null;
        if (type < EntityType.Event)
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

        var isSelf = en == Globals.Me;
        en.IsBlocking = packet.IsBlocking;

        if (attackTimer > -1)
        {
            en.AttackTimer = Timing.Global.Milliseconds + attackTimer;
            if (!isSelf)
            {
                en.AttackTime = attackTimer;
            }
        }
    }

    //EntityDiePacket
    public void HandlePacket(IPacketSender packetSender, EntityDiePacket packet)
    {
        var id = packet.Id;
        var type = packet.Type;
        var mapId = packet.MapId;

        Entity? en = null;
        if (type < EntityType.Event)
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

        en.ClearAnimations();
    }

    //EventDialogPacket
    public void HandlePacket(IPacketSender packetSender, EventDialogPacket packet)
    {
        var ed = new Dialog();
        ed.Prompt = packet.Prompt;
        ed.Face = packet.Face;
        if (packet.Type != 0)
        {
            ed.Options = packet.Responses;
        }

        ed.EventId = packet.EventId;
        Globals.EventDialogs.Add(ed);
    }

    //InputVariablePacket
    public void HandlePacket(IPacketSender packetSender, InputVariablePacket packet)
    {
        var type = packet.Type switch
        {
            VariableDataType.String => InputType.TextInput,
            VariableDataType.Boolean => InputType.YesNoCancel,
            _ => InputType.NumericInput,
        };

        _ = new InputBox(
            title: packet.Title,
            prompt: packet.Prompt,
            inputType: type,
            userData: packet.EventId,
            onSubmit: PacketSender.SendEventInputVariable,
            onCancel: PacketSender.SendEventInputVariableCancel
        );
    }

    //ErrorMessagePacket
    public void HandlePacket(IPacketSender packetSender, ErrorMessagePacket packet)
    {
        Fade.FadeIn(ClientConfiguration.Instance.FadeDurationMs);
        Globals.WaitingOnServer = false;
        Interface.Interface.ShowAlert(packet.Error, packet.Header, alertType: AlertType.Error);
        Interface.Interface.MenuUi?.Reset();
    }

    //MapItemsPacket
    public void HandlePacket(IPacketSender packetSender, MapItemsPacket packet)
    {
        var map = MapInstance.Get(packet.MapId);
        if (map == null)
        {
            return;
        }

        map.MapItems.Clear();
        foreach(var item in packet.Items)
        {
            var mapItem = new MapItemInstance(item.TileIndex,item.Id, item.ItemId, item.BagId, item.Quantity, item.Properties);

            if (!map.MapItems.ContainsKey(mapItem.TileIndex))
            {
                map.MapItems.Add(mapItem.TileIndex, new List<IMapItemInstance>());
            }

            map.MapItems[mapItem.TileIndex].Add(mapItem);
        }
    }

    //MapItemUpdatePacket
    public void HandlePacket(IPacketSender packetSender, MapItemUpdatePacket packet)
    {
        var map = MapInstance.Get(packet.MapId);
        if (map == null)
        {
            return;
        }

        // Are we deleting this item?
        if (packet.ItemId == Guid.Empty)
        {
            // Find our item based on our unique Id and remove it.
            foreach(var location in map.MapItems.Keys)
            {
                var tempItem = map.MapItems[location].Where(item => item.Id == packet.Id).SingleOrDefault();
                if (tempItem != null)
                {
                    map.MapItems[location].Remove(tempItem);
                }
            }
        }
        else
        {
            if (!map.MapItems.ContainsKey(packet.TileIndex))
            {
                map.MapItems.Add(packet.TileIndex, new List<IMapItemInstance>());
            }

            // Check if the item already exists, if it does replace it. Otherwise just add it.
            var mapItem = new MapItemInstance(packet.TileIndex, packet.Id, packet.ItemId, packet.BagId, packet.Quantity, packet.Properties);
            if (map.MapItems[packet.TileIndex].Any(item => item.Id == mapItem.Id))
            {
                for (var index = 0; index < map.MapItems[packet.TileIndex].Count; index++)
                {
                    if (map.MapItems[packet.TileIndex][index].Id == mapItem.Id)
                    {
                        map.MapItems[packet.TileIndex][index] = mapItem;
                    }
                }
            }
            else
            {
                // Reverse the array again to match server, add item.. then  reverse again to get the right render order.
                map.MapItems[packet.TileIndex].Add(mapItem);
            }
        }
    }

    //InventoryPacket
    public void HandlePacket(IPacketSender packetSender, InventoryPacket packet)
    {
        foreach (var inv in packet.Slots)
        {
            HandlePacket(inv);
        }
    }

    //InventoryUpdatePacket
    public void HandlePacket(IPacketSender packetSender, InventoryUpdatePacket packet)
    {
        Globals.Me?.UpdateInventory(packet.Slot, packet.ItemId, packet.Quantity, packet.BagId, packet.Properties);
    }

    //SpellsPacket
    public void HandlePacket(IPacketSender packetSender, SpellsPacket packet)
    {
        foreach (var spl in packet.Slots)
        {
            HandlePacket(spl);
        }
    }

    //SpellUpdatePacket
    public void HandlePacket(IPacketSender packetSender, SpellUpdatePacket packet)
    {
        if (Globals.Me != null)
        {
            Globals.Me.Spells[packet.Slot].Load(packet.SpellId);
        }
    }

    //EquipmentPacket
    public void HandlePacket(IPacketSender packetSender, EquipmentPacket packet)
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
    public void HandlePacket(IPacketSender packetSender, StatPointsPacket packet)
    {
        if (Globals.Me != null)
        {
            Globals.Me.StatPoints = packet.Points;
        }
    }

    //HotbarPacket
    public void HandlePacket(IPacketSender packetSender, HotbarPacket packet)
    {
        for (var i = 0; i < Options.Instance.Player.HotbarSlotCount; i++)
        {
            if (Globals.Me == null)
            {
                ApplicationContext.Context.Value?.Logger.LogDebug("Can't set hotbar, Globals.Me is null!");

                break;
            }

            if (Globals.Me.Hotbar == null)
            {
                ApplicationContext.Context.Value?.Logger.LogDebug("Can't set hotbar, hotbar is null!");

                break;
            }

            var hotbarEntry = Globals.Me.Hotbar[i];
            hotbarEntry.Load(packet.SlotData[i]);
        }
    }

    //CharacterCreationPacket
    public void HandlePacket(IPacketSender packetSender, CharacterCreationPacket packet)
    {
        Globals.WaitingOnServer = false;
        Interface.Interface.MenuUi.MainMenu.NotifyOpenCharacterCreation(packet.Force);
    }

    //AdminPanelPacket
    public void HandlePacket(IPacketSender packetSender, AdminPanelPacket packet)
    {
        Interface.Interface.EnqueueInGame(gameInterface => gameInterface.NotifyOpenAdminWindow());
    }

    //SpellCastPacket
    public void HandlePacket(IPacketSender packetSender, SpellCastPacket packet)
    {
        var entityId = packet.EntityId;
        var spellId = packet.SpellId;
        if (SpellDescriptor.Get(spellId) != null && Globals.Entities.ContainsKey(entityId))
        {
            Globals.Entities[entityId].CastTime = Timing.Global.Milliseconds + SpellDescriptor.Get(spellId).CastDuration;
            Globals.Entities[entityId].SpellCast = spellId;
        }
    }

    //SpellCooldownPacket
    public void HandlePacket(IPacketSender packetSender, SpellCooldownPacket packet)
    {
        foreach (var cd in packet.SpellCds)
        {
            var time = Timing.Global.Milliseconds + cd.Value;
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
    public void HandlePacket(IPacketSender packetSender, ItemCooldownPacket packet)
    {
        foreach (var cd in packet.ItemCds)
        {
            var time = Timing.Global.Milliseconds + cd.Value;
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

    //GlobalCooldownPacket
    public void HandlePacket(IPacketSender packetSender, GlobalCooldownPacket packet)
    {
        Globals.Me.GlobalCooldown = Timing.Global.Milliseconds + packet.GlobalCooldown;
    }

    //ExperiencePacket
    public void HandlePacket(IPacketSender packetSender, ExperiencePacket packet)
    {
        if (Globals.Me != null)
        {
            Globals.Me.Experience = packet.Experience;
            Globals.Me.ExperienceToNextLevel = packet.ExperienceToNextLevel;
        }
    }

    //ProjectileDeadPacket
    public void HandlePacket(IPacketSender packetSender, ProjectileDeadPacket packet)
    {
        if (packet.ProjectileDeaths != null)
        {
            foreach (var projDeath in packet.ProjectileDeaths)
            {
                if (Globals.Entities.ContainsKey(projDeath) && Globals.Entities[projDeath] is Projectile)
                {
                    Globals.EntitiesToDispose?.Add(projDeath);
                }
            }
        }
        if (packet.SpawnDeaths != null)
        {
            foreach (var spawnDeath in packet.SpawnDeaths)
            {
                if (Globals.Entities.ContainsKey(spawnDeath.Key) && Globals.Entities[spawnDeath.Key] is Projectile projectile)
                {
                    projectile.SpawnDead(spawnDeath.Value);
                }
            }
        }
    }

    //PlayAnimationPackets
    public void HandlePacket(IPacketSender sender, PlayAnimationPackets packet)
    {
        foreach (var pkt in packet.Packets)
        {
            HandlePacket(pkt);
        }
    }

    //PlayAnimationPacket
    public void HandlePacket(IPacketSender packetSender, PlayAnimationPacket packet)
    {
        var mapId = packet.MapId;
        var animationDescriptorId = packet.AnimationId;
        var targetType = packet.TargetType;
        var entityId = packet.EntityId;

        AnimationSource animationSource = new(packet.SourceType, packet.SourceId);

        switch (targetType)
        {
            case -1:
            {
                if (!MapInstance.TryGet(mapId, out var map))
                {
                    return;
                }

                map.AddTileAnimation(
                    animationDescriptorId,
                    packet.X,
                    packet.Y,
                    packet.Direction,
                    source: animationSource
                );

                break;
            }

            case 1:
            {
                if (Globals.EntitiesToDispose.Contains(entityId))
                {
                    return;
                }

                if (!Globals.Entities.TryGetValue(entityId, out var entity))
                {
                    return;
                }

                if (!AnimationDescriptor.TryGet(animationDescriptorId, out var animationDescriptor))
                {
                    return;
                }

                if (animationSource == default || entity.RemoveAnimationIfExists(animationSource, dispose: true))
                {
                    var animationInstance = new Animation(
                        animationDescriptor,
                        false,
                        packet.Direction != Direction.None,
                        -1,
                        entity,
                        source: animationSource
                    );

                    if (packet.Direction > Direction.None)
                    {
                        animationInstance.SetDir(packet.Direction);
                    }

                    entity.TryAddAnimation(animation: animationInstance, animationSource: animationSource);
                }
                else
                {
                    ApplicationContext.CurrentContext.Logger.LogDebug(
                        "Unable to add new instance of animation {AnimationId} to entity {EntityId} ({EntityName})  because one already exists for the animation source {AnimationSource} and it could not be removed",
                        animationDescriptorId,
                        entity.Id,
                        entity.Name,
                        animationSource
                    );
                }

                break;
            }

            case 2:
            {
                if (!MapInstance.TryGet(mapId, out var map))
                {
                    return;
                }

                if (!map.LocalEntities.TryGetValue(entityId, out var entity))
                {
                    return;
                }

                if (!AnimationDescriptor.TryGet(animationDescriptorId, out var animationDescriptor))
                {
                    return;
                }

                if (animationSource == default || entity.RemoveAnimationIfExists(animationSource, dispose: true))
                {
                    var animationInstance = new Animation(
                        animationDescriptor,
                        false,
                        packet.Direction == Direction.None,
                        -1,
                        entity,
                        source: animationSource
                    );

                    if (packet.Direction > Direction.None)
                    {
                        animationInstance.SetDir(packet.Direction);
                    }

                    entity.TryAddAnimation(animation: animationInstance, animationSource: animationSource);
                }
                else
                {
                    ApplicationContext.CurrentContext.Logger.LogDebug(
                        "Unable to add new instance of animation {AnimationId} to entity {EntityId} ({EntityName}) because one already exists for the animation source {AnimationSource} and it could not be removed",
                        animationDescriptorId,
                        entity.Id,
                        entity.Name,
                        animationSource
                    );
                }

                break;
            }
        }
    }

    //HoldPlayerPacket
    public void HandlePacket(IPacketSender packetSender, HoldPlayerPacket packet)
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
    public void HandlePacket(IPacketSender packetSender, PlayMusicPacket packet)
    {
        Audio.PlayMusic(packet.BGM, ClientConfiguration.Instance.MusicFadeTimer, ClientConfiguration.Instance.MusicFadeTimer, true);
    }

    //StopMusicPacket
    public void HandlePacket(IPacketSender packetSender, StopMusicPacket packet)
    {
        Audio.StopMusic(ClientConfiguration.Instance.MusicFadeTimer);
    }

    //PlaySoundPacket
    public void HandlePacket(IPacketSender packetSender, PlaySoundPacket packet)
    {
        Audio.AddGameSound(packet.Sound, false);
    }

    //StopSoundsPacket
    public void HandlePacket(IPacketSender packetSender, StopSoundsPacket packet)
    {
        Audio.StopAllSounds();
    }

    //ShowPicturePacket
    public void HandlePacket(IPacketSender packetSender, ShowPicturePacket packet)
    {
        PacketSender.SendClosePicture(Globals.Picture?.EventId ?? Guid.Empty);
        packet.ReceiveTime = Timing.Global.Milliseconds;
        Globals.Picture = packet;
    }

    //HidePicturePacket
    public void HandlePacket(IPacketSender packetSender, HidePicturePacket packet)
    {
        PacketSender.SendClosePicture(Globals.Picture?.EventId ?? Guid.Empty);
        Globals.Picture = null;
    }

    //ShopPacket
    public void HandlePacket(IPacketSender packetSender, ShopPacket packet)
    {
        if (!Interface.Interface.HasInGameUI)
        {
            throw new ArgumentNullException(nameof(Interface.Interface.GameUi));
        }

        if (packet == null)
        {
            throw new ArgumentNullException(nameof(packet));
        }

        if (packet.ShopData != null)
        {
            Globals.GameShop = new ShopDescriptor();
            Globals.GameShop.Load(packet.ShopData);
            Interface.Interface.EnqueueInGame(gameInterface => gameInterface.NotifyOpenShop());
        }
        else
        {
            Globals.GameShop = null;
            Interface.Interface.EnqueueInGame(gameInterface => gameInterface.NotifyCloseShop());
        }
    }

    //CraftingTablePacket
    public void HandlePacket(IPacketSender packetSender, CraftingTablePacket packet)
    {
        if (!packet.Close)
        {
            Globals.ActiveCraftingTable = new CraftingTableDescriptor();
            Globals.ActiveCraftingTable.Load(packet.TableData);
            Interface.Interface.EnqueueInGame(gameInterface => gameInterface.NotifyOpenCraftingTable(packet.JournalMode));
        }
        else
        {
            Interface.Interface.EnqueueInGame(gameInterface => gameInterface.NotifyCloseCraftingTable());
        }
    }

    //BankPacket
    public void HandlePacket(IPacketSender packetSender, BankPacket packet)
    {
        if (!packet.Close)
        {
            Globals.IsGuildBank = packet.Guild;
            Globals.BankSlots = new Item[packet.Slots];
            foreach (var itm in packet.Items)
            {
                HandlePacket(itm);
            }
            Globals.BankSlotCount = packet.Slots;
            Interface.Interface.EnqueueInGame(gameInterface => gameInterface.NotifyOpenBank());
        }
        else
        {
            Interface.Interface.EnqueueInGame(gameInterface => gameInterface.NotifyCloseBank());
        }
    }

    //BankUpdatePacket
    public void HandlePacket(IPacketSender packetSender, BankUpdatePacket packet)
    {
        var slot = packet.Slot;
        if (packet.ItemId != Guid.Empty)
        {
            Globals.BankSlots[slot] = new Item();
            Globals.BankSlots[slot].Load(packet.ItemId, packet.Quantity, packet.BagId, packet.Properties);
        }
        else
        {
            Globals.BankSlots[slot] = null;
        }
    }

    //GameObjectPacket
    public void HandlePacket(IPacketSender packetSender, GameObjectPacket packet)
    {
        var type = packet.Type;
        var objectId = packet.Id;
        var another = packet.AnotherFollowing;
        var deleted = packet.Deleted;
        var json = string.Empty;
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
                var obj = new TilesetDescriptor(objectId);
                obj.Load(json);
                TilesetDescriptor.Lookup.Set(objectId, obj);
                if (Globals.HasGameData && !another)
                {
                    Globals.ContentManager.LoadTilesets(TilesetDescriptor.GetNameList());
                }

                break;
            case GameObjectType.Event:
                //Clients don't store event data, im an idiot.
                break;
            default:
                var lookup = type.GetLookup();

                _ = lookup.DeleteAt(objectId);
                if (!deleted)
                {
                    var objectType = type.GetObjectType();
                    var databaseObject = lookup.AddNew(objectType, objectId);
                    databaseObject.Load(json);
                }

                if (type == GameObjectType.Resource)
                {

                }

                break;
        }
    }

    //EntityDashPacket
    public void HandlePacket(IPacketSender packetSender, EntityDashPacket packet)
    {
        if (!Globals.Entities.TryGetValue(packet.EntityId, out var value))
        {
            return;
        }

        value.DashQueue.Enqueue(
            new Dash(
                packet.EndMapId,
                packet.EndX,
                packet.EndY,
                packet.DashEndMilliseconds,
                packet.DashLengthMilliseconds,
                packet.Direction
            )
        );
    }

    //MapGridPacket
    public void HandlePacket(IPacketSender packetSender, MapGridPacket packet)
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
                    Globals.GridMaps[Globals.MapGrid[x, y]] = new Point(x, y);
                    // MapInstance.UpdateMapRequestTime(Globals.MapGrid[x, y]);
                }
            }
        }

        if (Globals.Me != null)
        {
            Player.FetchNewMaps();
        }

        Graphics.GridSwitched = true;
    }

    //TimePacket
    public void HandlePacket(IPacketSender packetSender, TimePacket packet)
    {
        Time.LoadTime(
            packet.Time,
            Color.FromArgb(packet.Color.A, packet.Color.R, packet.Color.G, packet.Color.B),
            packet.Rate
        );
    }

    //PartyPacket
    public void HandlePacket(IPacketSender packetSender, PartyPacket packet)
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
    public void HandlePacket(IPacketSender packetSender, PartyUpdatePacket packet)
    {
        var index = packet.MemberIndex;
        if (index < Globals.Me.Party.Count)
        {
            var mem = packet.MemberData;
            Globals.Me.Party[index] = new PartyMember(mem.Id, mem.Name, mem.Vital, mem.MaxVital, mem.Level);
        }
    }

    //PartyInvitePacket
    public void HandlePacket(IPacketSender packetSender, Intersect.Network.Packets.Server.PartyInvitePacket packet)
    {
        _ = new InputBox(
            title: Strings.Parties.PartyInvite,
            prompt: Strings.Parties.InvitePrompt.ToString(packet.LeaderName),
            inputType: InputType.YesNo,
            userData: packet.LeaderId,
            onSubmit: PacketSender.SendPartyAccept,
            onCancel: PacketSender.SendPartyDecline
        );
    }

    //ChatBubblePacket
    public void HandlePacket(IPacketSender packetSender, ChatBubblePacket packet)
    {
        var id = packet.EntityId;
        var type = packet.Type;
        var mapId = packet.MapId;
        IEntity en = null;
        if (type < EntityType.Event)
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
                return;
            {
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
    public void HandlePacket(IPacketSender packetSender, QuestOfferPacket packet)
    {
        if (!Globals.QuestOffers.Contains(packet.QuestId))
        {
            Globals.QuestOffers.Add(packet.QuestId);
        }
    }

    //QuestProgressPacket
    public void HandlePacket(IPacketSender packetSender, QuestProgressPacket packet)
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

            Globals.Me.HiddenQuests = packet.HiddenQuests;

            Interface.Interface.EnqueueInGame(uiInGame => uiInGame.NotifyQuestsUpdated());
        }
    }

    //TradePacket
    public void HandlePacket(IPacketSender packetSender, TradePacket packet)
    {
        if (!string.IsNullOrEmpty(packet.TradePartner))
        {
            Globals.Trade = new Item[2, Options.Instance.Player.MaxInventory];

            //Gotta initialize the trade values
            for (var x = 0; x < 2; x++)
            {
                for (var y = 0; y < Options.Instance.Player.MaxInventory; y++)
                {
                    Globals.Trade[x, y] = new Item();
                }
            }

            Interface.Interface.EnqueueInGame(gameInterface => gameInterface.NotifyOpenTrading(packet.TradePartner));
        }
        else
        {
            Interface.Interface.EnqueueInGame(gameInterface => gameInterface.NotifyCloseTrading());
        }
    }

    //TradeUpdatePacket
    public void HandlePacket(IPacketSender packetSender, TradeUpdatePacket packet)
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
            Globals.Trade[side, slot].Load(packet.ItemId, packet.Quantity, packet.BagId, packet.Properties);
        }
    }

    //TradeRequestPacket
    public void HandlePacket(IPacketSender packetSender, Intersect.Network.Packets.Server.TradeRequestPacket packet)
    {
        _ = new InputBox(
            title: Strings.Trading.TradeRequest,
            prompt: Strings.Trading.RequestPrompt.ToString(packet.PartnerName),
            inputType: InputType.YesNo,
            userData: packet.PartnerId,
            onSubmit: PacketSender.SendTradeRequestAccept,
            onCancel: PacketSender.SendTradeRequestDecline
        );
    }

    //NpcAggressionPacket
    public void HandlePacket(IPacketSender packetSender, NpcAggressionPacket packet)
    {
        if (Globals.Entities.ContainsKey(packet.EntityId))
        {
            Globals.Entities[packet.EntityId].Aggression = packet.Aggression;
        }
    }

    //PlayerDeathPacket
    public void HandlePacket(IPacketSender packetSender, PlayerDeathPacket packet)
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
    public void HandlePacket(IPacketSender packetSender, EntityZDimensionPacket packet)
    {
        if (Globals.Entities.ContainsKey(packet.EntityId))
        {
            Globals.Entities[packet.EntityId].Z = packet.Level;
        }
    }

    //BagPacket
    public void HandlePacket(IPacketSender packetSender, BagPacket packet)
    {
        if (!packet.Close)
        {
            Globals.BagSlots = new Item[packet.Slots];
            Interface.Interface.EnqueueInGame(gameInterface => gameInterface.NotifyOpenBag());
        }
        else
        {
            Interface.Interface.EnqueueInGame(gameInterface => gameInterface.NotifyCloseBag());
        }
    }

    //BagUpdatePacket
    public void HandlePacket(IPacketSender packetSender, BagUpdatePacket packet)
    {
        if (packet.ItemId == Guid.Empty)
        {
            Globals.BagSlots[packet.Slot] = null;
        }
        else
        {
            Globals.BagSlots[packet.Slot] = new Item();
            Globals.BagSlots[packet.Slot].Load(packet.ItemId, packet.Quantity, packet.BagId, packet.Properties);
        }
    }

    //MoveRoutePacket
    public void HandlePacket(IPacketSender packetSender, MoveRoutePacket packet)
    {
        Globals.MoveRouteActive = packet.Active;
    }

    //FriendsPacket
    public void HandlePacket(IPacketSender packetSender, FriendsPacket packet)
    {
        Globals.Me?.Friends.Clear();

        foreach (var friend in packet.OnlineFriends)
        {
            var f = new FriendInstance()
            {
                Name = friend.Key,
                Map = friend.Value,
                Online = true
            };

            Globals.Me?.Friends.Add(f);
        }

        foreach (var friend in packet.OfflineFriends)
        {
            var f = new FriendInstance()
            {
                Name = friend,
                Online = false
            };

            Globals.Me?.Friends.Add(f);
        }

        if (!Interface.Interface.HasInGameUI)
        {
            return;
        }

        Interface.Interface.EnqueueInGame(gameInterface => gameInterface.NotifyUpdateFriendsList());
    }

    //FriendRequestPacket
    public void HandlePacket(IPacketSender packetSender, FriendRequestPacket packet)
    {
        _ = new InputBox(
            title: Strings.Friends.Request,
            prompt: Strings.Friends.RequestPrompt.ToString(packet.FriendName),
            inputType: InputType.YesNo,
            userData: packet.FriendId,
            onSubmit: PacketSender.SendFriendRequestAccept,
            onCancel: PacketSender.SendFriendRequestDecline
        );
    }

    //CharactersPacket
    public void HandlePacket(IPacketSender packetSender, CharactersPacket packet)
    {
        List<CharacterSelectionPreviewMetadata> characterSelectionPreviews =
        [
            ..packet.Characters.Select(
                characterPacket => new CharacterSelectionPreviewMetadata(
                    characterPacket.Id,
                    characterPacket.Name,
                    characterPacket.Sprite,
                    characterPacket.Face,
                    characterPacket.Level,
                    characterPacket.ClassName,
                    characterPacket.Equipment
                )
            )
        ];

        if (packet.FreeSlot)
        {
            characterSelectionPreviews.Add(default);
        }

        Globals.WaitingOnServer = false;
        Interface.Interface.MenuUi.MainMenu.NotifyOpenCharacterSelection(characterSelectionPreviews);
    }

    //PasswordResetResultPacket
    public void HandlePacket(IPacketSender packetSender, PasswordResetResultPacket packet)
    {
        if (packet.Succeeded)
        {
            // Show Success Message and Open Login Screen
            Interface.Interface.ShowAlert(
                Strings.ResetPass.Success,
                Strings.ResetPass.SuccessMessage,
                AlertType.Information
            );
            Interface.Interface.MenuUi.MainMenu.NotifyOpenLogin();
        }
        else
        {
            Interface.Interface.ShowAlert(
                Strings.ResetPass.Error,
                Strings.ResetPass.ErrorMessage,
                alertType: AlertType.Error
            );
        }

        Globals.WaitingOnServer = false;
    }

    //TargetOverridePacket
    public void HandlePacket(IPacketSender packetSender, TargetOverridePacket packet)
    {
        if (Globals.Entities.ContainsKey(packet.TargetId))
        {
            Globals.Me.TryTarget(Globals.Entities[packet.TargetId], true);
        }
    }

    //EnteringGamePacket
    public void HandlePacket(IPacketSender packetSender, EnteringGamePacket packet)
    {
        //Fade out, we're finally loading the game world!
        Fade.FadeOut(ClientConfiguration.Instance.FadeDurationMs);
    }

    //CancelCastPacket
    public void HandlePacket(IPacketSender packetSender, CancelCastPacket packet)
    {
        if (!Globals.Entities.TryGetValue(packet.EntityId, out var entity))
        {
            return;
        }

        AnimationSource animationSource = new(AnimationSourceType.SpellCast, entity.SpellCast);

        if (entity.TryRemoveAnimation(
                animationSource: animationSource,
                dispose: true,
                animation: out var removedAnimation
            ))
        {
            ApplicationContext.CurrentContext.Logger.LogDebug(
                "Removing cancelled spell cast animation {AnimationId} ({AnimationName})",
                removedAnimation.Descriptor?.Id,
                removedAnimation.Descriptor?.Name
            );
        }

        entity.CastTime = 0;
        entity.SpellCast = default;
    }

    //GuildPacket
    public void HandlePacket(IPacketSender packetSender, GuildPacket packet)
    {
        if (Globals.Me == null || Globals.Me.Guild == null)
        {
            return;
        }

        var updatedGuildMembers = packet.Members.OrderByDescending(m => m.Online).ThenBy(m => m.Rank)
            .ThenBy(m => m.Name).ToArray();

        var currentGuildMembers = Globals.Me.GuildMembers;
        var hasUpdates = currentGuildMembers?.Length != updatedGuildMembers.Length;
        if (!hasUpdates)
        {
            for (var index = 0; index < currentGuildMembers.Length; ++index)
            {
                var currentGuildMember = currentGuildMembers[index];
                var updatedGuildMember = updatedGuildMembers[index];

                if (currentGuildMember.Id != updatedGuildMember.Id)
                {
                    hasUpdates = true;
                    break;
                }

                if (currentGuildMember.Online != updatedGuildMember.Online)
                {
                    hasUpdates = true;
                    break;
                }

                if (currentGuildMember.Rank != updatedGuildMember.Rank)
                {
                    hasUpdates = true;
                    break;
                }

                if (!string.Equals(currentGuildMember.Name, updatedGuildMember.Name))
                {
                    hasUpdates = true;
                    break;
                }
            }
        }

        if (hasUpdates)
        {
            Globals.Me.GuildMembers = updatedGuildMembers;
            Interface.Interface.EnqueueInGame(gameInterface => gameInterface.NotifyUpdateGuildList());
        }
    }


    //GuildInvitePacket
    public void HandlePacket(IPacketSender packetSender, GuildInvitePacket packet)
    {
        Interface.Interface.EnqueueInGame(
            () =>
            {
                _ = new InputBox(
                    title: Strings.Guilds.InviteRequestTitle,
                    prompt: (string.IsNullOrWhiteSpace(packet.GuildName)
                        ? Strings.Guilds.InviteRequestPromptMissingGuild
                        : Strings.Guilds.InviteRequestPrompt).ToString(packet.Inviter, packet.GuildName),
                    inputType: InputType.YesNo,
                    onSubmit: PacketSender.SendGuildInviteAccept,
                    onCancel: PacketSender.SendGuildInviteDecline
                );
            }
        );
    }

    public void HandlePacket(IPacketSender packetSender, FadePacket packet)
    {
        switch (packet.FadeType)
        {
            case FadeType.None:
                Fade.Cancel();
                break;
            case FadeType.FadeIn:
                Fade.FadeIn(packet.DurationMs, packet.WaitForCompletion);
                break;
            case FadeType.FadeOut:
                Fade.FadeOut(packet.DurationMs, packet.WaitForCompletion);
                break;
        }
    }

}
