using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.UI;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.Logging;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities.Combat;
using Intersect.Server.Entities.Events;
using Intersect.Server.General;
using Intersect.Server.Localization;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;
using Intersect.Server.Entities;
using Intersect.Server.Classes.Maps;

using Newtonsoft.Json;

namespace Intersect.Server.Maps
{
    public class MapProcessingLayer : IDisposable
    {
        public Guid Id;

        //SyncLock
        protected object mMapProcessLock = new object();

        public Guid InstanceLayer;

        private ConcurrentDictionary<Guid, Player> mPlayers = new ConcurrentDictionary<Guid, Player>();

        private readonly ConcurrentDictionary<Guid, Entity> mEntities = new ConcurrentDictionary<Guid, Entity>();
        private Entity[] mCachedEntities = new Entity[0];

        private MapInstance mMap;

        private BytePoint[] mMapBlocks = Array.Empty<BytePoint>();
        private BytePoint[] mNpcMapBlocks = Array.Empty<BytePoint>();

        private long LastUpdateTime;

        public ConcurrentDictionary<NpcSpawn, MapNpcSpawn> NpcSpawnInstances = new ConcurrentDictionary<NpcSpawn, MapNpcSpawn>();
        public ConcurrentDictionary<ResourceSpawn, MapResourceSpawn> ResourceSpawnInstances = new ConcurrentDictionary<ResourceSpawn, MapResourceSpawn>();
        public ConcurrentDictionary<Guid, ResourceSpawn> ResourceSpawns { get; set; } = new ConcurrentDictionary<Guid, ResourceSpawn>();

        public ConcurrentDictionary<Guid, MapItemSpawn> ItemRespawns = new ConcurrentDictionary<Guid, MapItemSpawn>();
        public ConcurrentDictionary<Guid, MapItem>[] TileItems { get; } = new ConcurrentDictionary<Guid, MapItem>[Options.Instance.MapOpts.Width * Options.Instance.MapOpts.Height];
        public ConcurrentDictionary<Guid, MapItem> AllMapItems { get; } = new ConcurrentDictionary<Guid, MapItem>();

        public ConcurrentDictionary<Guid, Projectile> MapProjectiles { get; } = new ConcurrentDictionary<Guid, Projectile>();
        public Projectile[] MapProjectilesCached = new Projectile[0];
        public long LastProjectileUpdateTime = -1;
        public ConcurrentDictionary<Guid, MapTrapInstance> MapTraps = new ConcurrentDictionary<Guid, MapTrapInstance>();
        public MapTrapInstance[] MapTrapsCached = new MapTrapInstance[0];

        private MapEntityMovements mEntityMovements = new MapEntityMovements();
        private MapActionMessages mActionMessages = new MapActionMessages();
        private MapAnimations mMapAnimations = new MapAnimations();

        public MapProcessingLayer(MapInstance map, Guid instanceLayer)
        {
            mMap = map;
            InstanceLayer = instanceLayer;
            Id = Guid.NewGuid();

            Initialize();
        }

        public bool IsDisposed { get; protected set; }

        public void Initialize()
        {
            lock (GetMapProcessLock())
            {
                CacheMapBlocks();
                DespawnEverything();
                RespawnEverything();

                // TODO Alex: Events
                /*
                var events = new List<EventBase>();
                foreach (var evt in EventIds)
                {
                    var itm = EventBase.Get(evt);
                    if (itm != null)
                    {
                        events.Add(itm);
                    }
                }
                EventsCache = events;
                */
            }
        }

        public object GetMapProcessLock()
        {
            return mMapProcessLock;
        }

        public object GetMapLock()
        {
            return mMap.GetMapLock();
        }

        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
            }
        }

        public MapInstance GetMapInstance()
        {
            return mMap;
        }

        public void DespawnEverything()
        {
            DespawnNpcs();
            DespawnResources();
            DespawnProjectiles();
            DespawnTraps();
            DespawnItems();
        }

        public void RespawnEverything()
        {
            SpawnMapNpcs();
            SpawnAttributeItems(); // This must be done before spawning items or resources
            SpawnMapResources();
        }

        public void Update(long timeMs)
        {
            lock (GetMapProcessLock())
            {
                if (Options.Instance.Processing.MapUpdateInterval == Options.Instance.Processing.ProjectileUpdateInterval)
                {
                    UpdateProjectiles(timeMs);
                }

                UpdateItems(timeMs);
                UpdateEntities(timeMs);
                ProcessNpcRespawns();
                ProcessResourceRespawns();

                SendBatchedPacketsToPlayers();

                LastUpdateTime = timeMs;
            }
        }

        public void AddEntity(Entity en)
        {
            if (en != null && !en.IsDead() && en.InstanceLayer == InstanceLayer)
            {
                if (!mEntities.ContainsKey(en.Id))
                {
                    mEntities.TryAdd(en.Id, en);
                    if (en is Player plyr)
                    {
                        mPlayers.TryAdd(plyr.Id, plyr);
                    }
                    mCachedEntities = mEntities.Values.ToArray();
                }
            }
        }

        public void RemoveEntity(Entity en)
        {
            mEntities.TryRemove(en.Id, out var result);
            if (mPlayers.ContainsKey(en.Id))
            {
                mPlayers.TryRemove(en.Id, out var pResult);
            }
            mCachedEntities = mEntities.Values.ToArray();
        }

        public List<Entity> GetEntities(bool includeSurroundingMaps = false)
        {
            var entities = new List<Entity>();

            foreach (var en in mEntities)
                entities.Add(en.Value);

            // ReSharper disable once InvertIf
            if (includeSurroundingMaps)
            {
                foreach (var map in mMap.GetSurroundingMaps(false))
                {
                    // TODO Alex: Support all entities with one call
                    if (map.TryGetRelevantProcessingLayer(InstanceLayer, out var mapProcessingLayer))
                    {
                        entities.AddRange(mapProcessingLayer.GetEntities());
                    }
                    // Pickup entities that haven't been converted
                    entities.AddRange(map.GetEntities());
                }
            }

            return entities;
        }

        public void PlayerEnteredMap(Player player)
        {
            //Send Entity Info to Everyone and Everyone to the Entity
            player.Client?.SentMaps?.Clear();
            PacketSender.SendMapItems(player, mMap.Id);

            AddEntity(player);

            player.LastMapEntered = mMap.Id;
            var relevantMaps = mMap.GetSurroundingMaps(true);
            foreach (var map in relevantMaps)
            {
                if (map.Id == mMap.Id) // Map player is in
                {
                    SendMapEntitiesTo(player);
                } else if (map.TryGetRelevantProcessingLayer(InstanceLayer, out var mapProcessingLayer)) // Surrounding maps
                {
                    mapProcessingLayer.SendMapEntitiesTo(player);
                }
                // TODO Alex Items
                PacketSender.SendMapItems(player, map.Id);
            }
            PacketSender.SendEntityDataToProximity(player, player);
        }

        public void SendMapEntitiesTo(Player player)
        {
            if (player != null)
            {
                PacketSender.SendMapEntitiesTo(player, mEntities);
                if (player.MapId == mMap.Id && player.InstanceLayer == InstanceLayer)
                {
                    player.SendEvents();
                }
            }
        }

        private void SpawnAttributeItems()
        {
            ResourceSpawns.Clear();
            for (byte x = 0; x < Options.MapWidth; x++)
            {
                for (byte y = 0; y < Options.MapHeight; y++)
                {
                    if (mMap.Attributes[x, y] != null)
                    {
                        if (mMap.Attributes[x, y].Type == MapAttributes.Item)
                        {
                            SpawnAttributeItem(x, y);
                        }
                        else if (mMap.Attributes[x, y].Type == MapAttributes.Resource)
                        {
                            SpawnAttributeResource(x, y);
                        }
                    }
                }
            }
        }

        #region Players
        public bool HasPlayersOnMap()
        {
            return !mPlayers.IsEmpty;
        }

        public ICollection<Player> GetPlayersOnMap()
        {
            return mPlayers.Values;
        }

        public ICollection<Player> GetAllRelevantPlayers()
        {
            var allPlayers = new List<Player>();
            var surroundingMaps = mMap.GetSurroundingMaps();
            foreach (var map in surroundingMaps)
            {
                if (map.TryGetRelevantProcessingLayer(InstanceLayer, out var mapProcessingLayer, false))
                {
                    var adjoiningPlayers = mapProcessingLayer.GetPlayersOnMap();
                    if (adjoiningPlayers != null)
                    {
                        allPlayers.AddRange(adjoiningPlayers);
                    }
                }
            }
            allPlayers.AddRange(GetPlayersOnMap());
            return allPlayers;
        }
        #endregion

        #region NPCs
        private void SpawnMapNpcs()
        {
            for (var i = 0; i < mMap.Spawns.Count; i++)
            {
                SpawnMapNpc(i);
            }
        }

        private void SpawnMapNpc(int i)
        {
            byte x = 0;
            byte y = 0;
            byte dir = 0;
            var spawns = mMap.Spawns;
            var npcBase = NpcBase.Get(spawns[i].NpcId);
            if (npcBase != null)
            {
                MapNpcSpawn npcSpawnInstance;
                if (NpcSpawnInstances.ContainsKey(spawns[i]))
                {
                    npcSpawnInstance = NpcSpawnInstances[spawns[i]];
                }
                else
                {
                    npcSpawnInstance = new MapNpcSpawn();
                    NpcSpawnInstances.TryAdd(spawns[i], npcSpawnInstance);
                }

                if (spawns[i].Direction != NpcSpawnDirection.Random)
                {
                    dir = (byte)(spawns[i].Direction - 1);
                }
                else
                {
                    dir = (byte)Randomization.Next(0, 4);
                }

                if (spawns[i].X >= 0 && spawns[i].Y >= 0)
                {
                    npcSpawnInstance.Entity = SpawnNpc((byte)spawns[i].X, (byte)spawns[i].Y, dir, spawns[i].NpcId);
                }
                else
                {
                    for (var n = 0; n < 100; n++)
                    {
                        x = (byte)Randomization.Next(0, Options.MapWidth);
                        y = (byte)Randomization.Next(0, Options.MapHeight);
                        if (mMap.Attributes[x, y] == null || mMap.Attributes[x, y].Type == (int)MapAttributes.Walkable)
                        {
                            break;
                        }

                        x = 0;
                        y = 0;
                    }

                    npcSpawnInstance.Entity = SpawnNpc(x, y, dir, spawns[i].NpcId);
                }
            }
        }

        public Entity SpawnNpc(byte tileX, byte tileY, byte dir, Guid npcId, bool despawnable = false)
        {
            var npcBase = NpcBase.Get(npcId);
            if (npcBase != null)
            {
                var processLayer = this.InstanceLayer;
                var npc = new Npc(npcBase, despawnable)
                {
                    MapId = mMap.Id,
                    X = tileX,
                    Y = tileY,
                    Dir = dir,
                    InstanceLayer = processLayer
                };

                AddEntity(npc);
                PacketSender.SendEntityDataToProximity(npc);

                return npc;
            }

            return null;
        }

        private void DespawnNpcs()
        {
            //Kill all npcs spawned from this map
            lock (GetMapProcessLock())
            {
                foreach (var npcSpawn in NpcSpawnInstances)
                {
                    lock (npcSpawn.Value.Entity.EntityLock)
                    {
                        npcSpawn.Value.Entity.Die(false);
                    }
                }

                NpcSpawnInstances.Clear();

                //Kill any other npcs on this map (only players should remain)
                foreach (var entity in mEntities)
                {
                    if (entity.Value is Npc npc)
                    {
                        lock (npc.EntityLock)
                        {
                            npc.Die(false);
                        }
                    }
                }
            }
        }

        public void ClearEntityTargetsOf(Entity en)
        {
            foreach (var entity in mEntities)
            {
                if (entity.Value is Npc npc && npc.Target == en)
                {
                    npc.RemoveTarget();
                }
            }
        }
        #endregion

        #region Resources
        private void SpawnAttributeResource(byte x, byte y)
        {
            var tempResource = new ResourceSpawn()
            {
                ResourceId = ((MapResourceAttribute)mMap.Attributes[x, y]).ResourceId,
                X = x,
                Y = y,
                Z = ((MapResourceAttribute)mMap.Attributes[x, y]).SpawnLevel
            };

            ResourceSpawns.TryAdd(tempResource.Id, tempResource);
        }

        private void SpawnMapResources()
        {
            foreach (var spawn in ResourceSpawns)
            {
                SpawnMapResource(spawn.Value);
            }
        }

        private void SpawnMapResource(ResourceSpawn spawn)
        {
            int x = spawn.X;
            int y = spawn.Y;
            var id = Guid.Empty;
            MapResourceSpawn resourceSpawnInstance;
            if (ResourceSpawnInstances.ContainsKey(spawn))
            {
                resourceSpawnInstance = ResourceSpawnInstances[spawn];
            }
            else
            {
                resourceSpawnInstance = new MapResourceSpawn();
                ResourceSpawnInstances.TryAdd(spawn, resourceSpawnInstance);
            }

            if (resourceSpawnInstance.Entity == null)
            {
                var resourceBase = ResourceBase.Get(spawn.ResourceId);
                if (resourceBase != null)
                {
                    var res = new Resource(resourceBase);
                    resourceSpawnInstance.Entity = res;
                    res.X = spawn.X;
                    res.Y = spawn.Y;
                    res.Z = spawn.Z;
                    res.MapId = mMap.Id;
                    res.InstanceLayer = InstanceLayer;
                    id = res.Id;
                    AddEntity(res);
                }
            }
            else
            {
                id = resourceSpawnInstance.Entity.Id;
            }

            if (id != Guid.Empty)
            {
                resourceSpawnInstance.Entity.Spawn();
            }
        }

        private void DespawnResources()
        {
            //Kill all resources spawned from this map
            lock (GetMapLock())
            {
                foreach (var resourceSpawn in ResourceSpawnInstances)
                {
                    if (resourceSpawn.Value != null && resourceSpawn.Value.Entity != null)
                    {
                        resourceSpawn.Value.Entity.Destroy();
                        RemoveEntity(resourceSpawn.Value.Entity);
                    }
                }

                ResourceSpawnInstances.Clear();
            }
        }
        #endregion

        #region Items
        /// <summary>
        /// Add a map item to this map.
        /// </summary>
        /// <param name="x">The X location of this item.</param>
        /// <param name="y">The Y location of this item.</param>
        /// <param name="item">The <see cref="MapItem"/> to add to the map.</param>
        private void AddItem(MapItem item)
        {
            AllMapItems.TryAdd(item.UniqueId, item);

            if (TileItems[item.TileIndex] == null)
            {
                TileItems[item.TileIndex] = new ConcurrentDictionary<Guid, MapItem>();
            }

            TileItems[item.TileIndex]?.TryAdd(item.UniqueId, item);
        }

        /// <summary>
        /// Spawn an item to this map instance.
        /// </summary>
        /// <param name="x">The horizontal location of this item</param>
        /// <param name="y">The vertical location of this item.</param>
        /// <param name="item">The <see cref="Item"/> to spawn on the map.</param>
        /// <param name="amount">The amount of times to spawn this item to the map. Set to the <see cref="Item"/> quantity, overwrites quantity if stackable!</param>
        public void SpawnItem(int x, int y, Item item, int amount) => SpawnItem(x, y, item, amount, Guid.Empty);

        /// <summary>
        /// Spawn an item to this map instance.
        /// </summary>
        /// <param name="x">The horizontal location of this item</param>
        /// <param name="y">The vertical location of this item.</param>
        /// <param name="item">The <see cref="Item"/> to spawn on the map.</param>
        /// <param name="amount">The amount of times to spawn this item to the map. Set to the <see cref="Item"/> quantity, overwrites quantity if stackable!</param>
        /// <param name="owner">The player Id that will be the temporary owner of this item.</param>
        public void SpawnItem(int x, int y, Item item, int amount, Guid owner, bool sendUpdate = true)
        {
            if (item == null)
            {
                Log.Warn($"Tried to spawn {amount} of a null item at ({x}, {y}) in map {Id}.");

                return;
            }

            var itemDescriptor = ItemBase.Get(item.ItemId);
            if (itemDescriptor == null)
            {
                Log.Warn($"No item found for {item.ItemId}.");

                return;
            }

            // if we can stack this item or the user configured to drop items consolidated, simply spawn a single stack of it.
            // Does not count for Equipment and bags, these are ALWAYS their own separate item spawn. We don't want to lose data on that!
            if ((itemDescriptor.ItemType != ItemTypes.Equipment && itemDescriptor.ItemType != ItemTypes.Bag) &&
                (itemDescriptor.Stackable || Options.Loot.ConsolidateMapDrops))
            {
                // Does this item already exist on this tile? If so, get its value so we can simply consolidate the stack.
                var existingCount = 0;
                var existingItems = FindItemsAt(y * Options.MapWidth + x);
                var toRemove = new List<MapItem>();
                foreach (var exItem in existingItems)
                {
                    // If the Id and Owner matches, get its quantity and remove the item so we don't get multiple stacks.
                    if (exItem.ItemId == item.ItemId && exItem.Owner == owner)
                    {
                        existingCount += exItem.Quantity;
                        toRemove.Add(exItem);
                    }
                }

                var mapItem = new MapItem(item.ItemId, amount + existingCount, x, y, item.BagId, item.Bag)
                {
                    DespawnTime = Timing.Global.Milliseconds + Options.Loot.ItemDespawnTime,
                    Owner = owner,
                    OwnershipTime = Timing.Global.Milliseconds + Options.Loot.ItemOwnershipTime,
                    VisibleToAll = Options.Loot.ShowUnownedItems || owner == Guid.Empty
                };

                if (mapItem.TileIndex > Options.MapHeight * Options.MapWidth || mapItem.TileIndex < 0)
                {
                    return;
                }

                // Remove existing items if we need to.
                foreach (var reItem in toRemove)
                {
                    RemoveItem(reItem);
                    if (sendUpdate)
                    {
                        PacketSender.SendMapItemUpdate(mMap.Id, InstanceLayer, reItem, true);
                    }
                }

                // Drop the new item.
                AddItem(mapItem);
                if (sendUpdate)
                {
                    PacketSender.SendMapItemUpdate(mMap.Id, InstanceLayer, mapItem, false);
                }
            }
            else
            {
                // Oh boy here we go! Set quantity to 1 and drop multiple!
                for (var i = 0; i < amount; i++)
                {
                    var mapItem = new MapItem(item.ItemId, amount, x, y, item.BagId, item.Bag)
                    {
                        DespawnTime = Globals.Timing.Milliseconds + Options.Loot.ItemDespawnTime,
                        Owner = owner,
                        OwnershipTime = Globals.Timing.Milliseconds + Options.Loot.ItemOwnershipTime,
                        VisibleToAll = Options.Loot.ShowUnownedItems || owner == Guid.Empty
                    };

                    // If this is a piece of equipment, set up the stat buffs for it.
                    if (itemDescriptor.ItemType == ItemTypes.Equipment)
                    {
                        mapItem.SetupStatBuffs(item);
                    }

                    if (mapItem.TileIndex > Options.MapHeight * Options.MapWidth || mapItem.TileIndex < 0)
                    {
                        return;
                    }

                    AddItem(mapItem);
                }
                PacketSender.SendMapItemsToProximity(mMap.Id, this);
            }
        }

        /// <summary>
        /// Find a Map Item on this map based on its Unique Id;
        /// </summary>
        /// <param name="uniqueId">The Unique Id of the Map Item to look for.</param>
        /// <returns>Returns a <see cref="MapItem"/> if one is found with the desired Unique Id.</returns>
        public MapItem FindItem(Guid uniqueId)
        {
            if (AllMapItems.TryGetValue(uniqueId, out MapItem item))
            {
                return item;
            }
            return null;
        }

        /// <summary>
        /// /// Find all map items at a specificed location.
        /// </summary>
        /// <param name="tileIndex">The integer value representation of the tile.</param>
        /// <returns>Returns a <see cref="ICollection"/> of <see cref="MapItem"/></returns>
        public ICollection<MapItem> FindItemsAt(int tileIndex)
        {
            if (tileIndex < 0 || tileIndex >= Options.MapWidth * Options.MapHeight || TileItems[tileIndex] == null)
            {
                return Array.Empty<MapItem>();
            }
            return TileItems[tileIndex].Values;
        }

        public void RemoveItem(MapItem item, bool respawn = true)
        {
            if (item != null)
            {
                // Only try to handle respawns for items that have attribute spawn locations.
                if (item.AttributeSpawnX > -1)
                {
                    if (respawn)
                    {
                        var spawn = new MapItemSpawn()
                        {
                            AttributeSpawnX = item.X,
                            AttributeSpawnY = item.Y,
                            RespawnTime = Globals.Timing.Milliseconds + Options.Map.ItemAttributeRespawnTime
                        };
                        ItemRespawns.TryAdd(spawn.Id, spawn);
                    }
                }

                var oldOwner = item.Owner;
                AllMapItems.TryRemove(item.UniqueId, out MapItem removed);
                TileItems[item.TileIndex]?.TryRemove(item.UniqueId, out MapItem tileRemoved);
                if (TileItems[item.TileIndex]?.IsEmpty ?? false)
                {
                    TileItems[item.TileIndex] = null;
                }
                PacketSender.SendMapItemUpdate(mMap.Id, InstanceLayer, item, true, item.VisibleToAll, oldOwner);
            }
        }

        public void DespawnItems()
        {
            //Kill all items resting on map
            ItemRespawns.Clear();
            foreach (var item in AllMapItems.Values)
            {
                RemoveItem(item);
            }

            AllMapItems.Clear();
        }

        private void SpawnAttributeItem(int x, int y)
        {
            var item = ItemBase.Get(((MapItemAttribute)mMap.Attributes[x, y]).ItemId);
            if (item != null)
            {
                var mapItem = new MapItem(item.Id, ((MapItemAttribute)mMap.Attributes[x, y]).Quantity, x, y);
                mapItem.DespawnTime = -1;
                mapItem.AttributeSpawnX = x;
                mapItem.AttributeSpawnY = y;
                if (item.ItemType == ItemTypes.Equipment)
                {
                    mapItem.Quantity = 1;
                }
                AddItem(mapItem);
                PacketSender.SendMapItemUpdate(mMap.Id, InstanceLayer, mapItem, false);
            }
        }
        #endregion

        #region Projectiles & Traps
        //Spawn a projectile
        public void SpawnMapProjectile(
            Entity owner,
            ProjectileBase projectile,
            SpellBase parentSpell,
            ItemBase parentItem,
            Guid mapId,
            byte x,
            byte y,
            byte z,
            byte direction,
            Entity target
        )
        {
            var proj = new Projectile(owner, parentSpell, parentItem, projectile, mMap.Id, x, y, z, direction, target);
            proj.InstanceLayer = InstanceLayer;
            MapProjectiles.TryAdd(proj.Id, proj);
            MapProjectilesCached = MapProjectiles.Values.ToArray();
            PacketSender.SendEntityDataToProximity(proj);
        }

        public void DespawnProjectiles()
        {
            var guids = new List<Guid>();
            foreach (var proj in MapProjectiles)
            {
                if (proj.Value != null)
                {
                    guids.Add(proj.Value.Id);
                    proj.Value.Die();
                }
            }
            PacketSender.SendRemoveProjectileSpawns(mMap.Id, InstanceLayer, guids.ToArray(), null);
            MapProjectiles.Clear();
            MapProjectilesCached = new Projectile[0];
        }

        public void RemoveProjectile(Projectile en)
        {
            MapProjectiles.TryRemove(en.Id, out Projectile removed);
            MapProjectilesCached = MapProjectiles.Values.ToArray();
        }

        public void SpawnTrap(Entity owner, SpellBase parentSpell, byte x, byte y, byte z)
        {
            var trap = new MapTrapInstance(owner, parentSpell, mMap.Id, InstanceLayer, x, y, z);
            MapTraps.TryAdd(trap.Id, trap);
            MapTrapsCached = MapTraps.Values.ToArray();
        }

        public void DespawnTraps()
        {
            MapTraps.Clear();
            MapTrapsCached = new MapTrapInstance[0];
        }

        public void RemoveTrap(MapTrapInstance trap)
        {
            MapTraps.TryRemove(trap.Id, out MapTrapInstance removed);
            MapTrapsCached = MapTraps.Values.ToArray();
        }

        #endregion

        #region Collision
        public bool TileBlocked(int x, int y)
        {
            if (mMap.Attributes == null ||
                x < 0 || x >= mMap.Attributes.GetLength(0) ||
                y < 0 || y >= mMap.Attributes.GetLength(1))
            {
                return true;
            }

            //Check if tile is a blocked attribute
            if (mMap.Attributes[x, y] != null && (mMap.Attributes[x, y].Type == MapAttributes.Blocked ||
                mMap.Attributes[x, y].Type == MapAttributes.Animation && ((MapAnimationAttribute)mMap.Attributes[x, y]).IsBlock))
            {
                return true;
            }

            //See if there are any entities in the way
            var entities = GetEntities();
            for (var i = 0; i < entities.Count; i++)
            {
                if (entities[i] != null && !(entities[i] is Projectile))
                {
                    //If Npc or Player then blocked.. if resource then check
                    if (!entities[i].Passable && entities[i].X == x && entities[i].Y == y)
                    {
                        return true;
                    }
                }
            }

            //Check Global Events
            // TODO Alex: Global Events
            /*
            foreach (var globalEvent in GlobalEventInstances)
            {
                if (globalEvent.Value != null && globalEvent.Value.PageInstance != null)
                {
                    if (!globalEvent.Value.PageInstance.Passable &&
                        globalEvent.Value.PageInstance.X == x &&
                        globalEvent.Value.PageInstance.Y == y)
                    {
                        return true;
                    }
                }
            }
            */

            return false;
        }
        #endregion

        #region Packet Batching
        public void AddBatchedAnimation(PlayAnimationPacket packet)
        {
            mMapAnimations.Add(packet);
        }

        public void AddBatchedMovement(Entity en, bool correction, Player forPlayer)
        {
            mEntityMovements.Add(en, correction, forPlayer);
        }

        public void AddBatchedActionMessage(ActionMsgPacket packet)
        {
            mActionMessages.Add(packet);
        }
        #endregion

        #region Caching
        public void CacheMapBlocks()
        {
            var blocks = new List<BytePoint>();
            var npcBlocks = new List<BytePoint>();
            for (byte x = 0; x < Options.MapWidth; x++)
            {
                for (byte y = 0; y < Options.MapHeight; y++)
                {
                    if (mMap.Attributes[x, y] != null)
                    {
                        if (mMap.Attributes[x, y].Type == MapAttributes.Blocked ||
                            mMap.Attributes[x, y].Type == MapAttributes.GrappleStone ||
                            mMap.Attributes[x, y].Type == MapAttributes.Animation && ((MapAnimationAttribute)mMap.Attributes[x, y]).IsBlock)
                        {
                            blocks.Add(new BytePoint(x, y));
                            npcBlocks.Add(new BytePoint(x, y));
                        }
                        else if (mMap.Attributes[x, y].Type == MapAttributes.NpcAvoid)
                        {
                            npcBlocks.Add(new BytePoint(x, y));
                        }
                    }
                }
            }

            mMapBlocks = blocks.ToArray();
            mNpcMapBlocks = npcBlocks.ToArray();
        }

        public Entity[] GetCachedEntities()
        {
            return mCachedEntities;
        }

        public BytePoint[] GetCachedBlocks(bool isPlayer)
        {
            return isPlayer ? mMapBlocks : mNpcMapBlocks;
        }
        #endregion

        #region Updates
        private void UpdateEntities(long timeMs)
        {
            var surrMaps = mMap.GetSurroundingMaps(true);

            // Keep a list of all entities with changed vitals and statusses.
            var vitalUpdates = new List<Entity>();
            var statusUpdates = new List<Entity>();

            foreach (var en in mEntities)
            {
                //Let's see if and how long this map has been inactive, if longer than 30 seconds, regenerate everything on the map
                //TODO, take this 30 second value and throw it into the server config after I switch everything to json
                if (timeMs > LastUpdateTime + 30000)
                {
                    //Regen Everything & Forget Targets
                    if (en.Value is Resource || en.Value is Npc)
                    {
                        en.Value.RestoreVital(Vitals.Health);
                        en.Value.RestoreVital(Vitals.Mana);

                        if (en.Value is Npc npc)
                        {
                            npc.AssignTarget(null);
                        }
                        else
                        {
                            en.Value.Target = null;
                        }
                    }
                }

                en.Value.Update(timeMs);

                // Check to see if we need to send any entity vital and status updates for this entity.
                if (en.Value.VitalsUpdated)
                {
                    vitalUpdates.Add(en.Value);

                    // Send a party update if we're a player with a party.
                    if (en.Value is Player player)
                    {
                        for (var i = 0; i < player.Party.Count; i++)
                        {
                            PacketSender.SendPartyUpdateTo(player.Party[i], player);
                        }
                    }

                    en.Value.VitalsUpdated = false;
                }

                if (en.Value.StatusesUpdated)
                {
                    statusUpdates.Add(en.Value);

                    en.Value.StatusesUpdated = false;
                }
            }

            if (vitalUpdates.Count > 0)
            {
                PacketSender.SendMapEntityVitalUpdate(mMap, vitalUpdates.ToArray(), InstanceLayer);
            }

            if (statusUpdates.Count > 0)
            {
                PacketSender.SendMapEntityStatusUpdate(mMap, statusUpdates.ToArray(), InstanceLayer);
            }
        }

        private void ProcessNpcRespawns()
        {
            var spawns = mMap.Spawns;
            for (var i = 0; i < spawns.Count; i++)
            {
                if (NpcSpawnInstances.ContainsKey(spawns[i]))
                {
                    var npcSpawnInstance = NpcSpawnInstances[spawns[i]];
                    if (npcSpawnInstance != null && npcSpawnInstance.Entity.Dead)
                    {
                        if (npcSpawnInstance.RespawnTime == -1)
                        {
                            npcSpawnInstance.RespawnTime = Globals.Timing.Milliseconds +
                                                           ((Npc)npcSpawnInstance.Entity).Base.SpawnDuration -
                                                           (Globals.Timing.Milliseconds - LastUpdateTime);
                        }
                        else if (npcSpawnInstance.RespawnTime < Globals.Timing.Milliseconds)
                        {
                            SpawnMapNpc(i);
                            npcSpawnInstance.RespawnTime = -1;
                        }
                    }
                }
            }
        }

        private void ProcessResourceRespawns()
        {
            foreach (var spawn in ResourceSpawns)
            {
                if (ResourceSpawnInstances.ContainsKey(spawn.Value))
                {
                    var resourceSpawnInstance = ResourceSpawnInstances[spawn.Value];
                    if (resourceSpawnInstance.Entity != null && resourceSpawnInstance.Entity.IsDead())
                    {
                        if (resourceSpawnInstance.RespawnTime == -1)
                        {
                            resourceSpawnInstance.RespawnTime = Globals.Timing.Milliseconds +
                                                                resourceSpawnInstance.Entity.Base.SpawnDuration;
                        }
                        else if (resourceSpawnInstance.RespawnTime < Globals.Timing.Milliseconds)
                        {
                            // Check to see if this resource can be respawned, if there's an Npc or Player on it we shouldn't let it respawn yet..
                            // Unless of course the resource is walkable regardless.
                            var canSpawn = false;
                            if (resourceSpawnInstance.Entity.Base.WalkableBefore)
                            {
                                canSpawn = true;
                            }
                            else
                            {
                                // Check if this resource is currently stepped on
                                var spawnBlockers = GetEntities().Where(x => x is Player || x is Npc).ToArray();
                                if (!spawnBlockers.Any(e => e.X == resourceSpawnInstance.Entity.X && e.Y == resourceSpawnInstance.Entity.Y))
                                {
                                    canSpawn = true;
                                }
                            }

                            if (canSpawn)
                            {
                                SpawnMapResource(spawn.Value);
                                resourceSpawnInstance.RespawnTime = -1;
                            }
                        }
                    }
                }
            }
        }

        public void UpdateProjectiles(long timeMs)
        {
            var spawnDeaths = new List<KeyValuePair<Guid, int>>();
            var projDeaths = new List<Guid>();

            //Process all of the projectiles
            foreach (var proj in MapProjectilesCached)
            {
                proj.Update(projDeaths, spawnDeaths);
            }

            if (projDeaths.Count > 0 || spawnDeaths.Count > 0)
            {
                PacketSender.SendRemoveProjectileSpawns(mMap.Id, InstanceLayer, projDeaths.ToArray(), spawnDeaths.ToArray());
            }

            //Process all of the traps
            foreach (var trap in MapTrapsCached)
            {
                trap.Update();
            }

            LastProjectileUpdateTime = timeMs;
        }

        public void UpdateItems(long timeMs)
        {
            foreach (var mapItem in AllMapItems.Values)
            {
                // Should this item be visible to everyone now?
                if (!mapItem.VisibleToAll && mapItem.OwnershipTime < timeMs)
                {
                    mapItem.VisibleToAll = true;
                    PacketSender.SendMapItemUpdate(mMap.Id, InstanceLayer, mapItem, false);
                }

                // Do we need to delete this item?
                if (mapItem.DespawnTime != -1 && mapItem.DespawnTime < timeMs)
                {
                    RemoveItem(mapItem);
                }
            }

            foreach (var itemRespawn in ItemRespawns.Values)
            {
                if (itemRespawn.RespawnTime < timeMs)
                {
                    SpawnAttributeItem(itemRespawn.AttributeSpawnX, itemRespawn.AttributeSpawnY);
                    ItemRespawns.TryRemove(itemRespawn.Id, out MapItemSpawn spawn);
                }
            }
        }

        private void SendBatchedPacketsToPlayers()
        {
            var surrMaps = mMap.GetSurroundingMaps(true);
            var nearbyPlayers = new HashSet<Player>();

            // Get all players in surrounding and current maps
            foreach (var map in surrMaps)
            {
                if (map != null && map.TryGetRelevantProcessingLayer(InstanceLayer, out var mapProcessingLayer))
                {
                    foreach (var plyr in mapProcessingLayer.GetPlayersOnMap())
                    {
                        nearbyPlayers.Add(plyr);
                    }
                }
            }

            // And send batched packets out to all nearby players
            mEntityMovements.SendPackets(nearbyPlayers);
            mActionMessages.SendPackets(nearbyPlayers);
            mMapAnimations.SendPackets(nearbyPlayers);
        }
        #endregion
    }
}
