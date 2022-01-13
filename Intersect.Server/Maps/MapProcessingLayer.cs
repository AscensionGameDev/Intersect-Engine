using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.Logging;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Entities.Events;
using Intersect.Server.General;
using Intersect.Server.Networking;
using Intersect.Utilities;
using Intersect.Server.Entities;
using Intersect.Server.Classes.Maps;

namespace Intersect.Server.Maps
{
    /// <summary>
    /// A <see cref="MapProcessingLayer"/> exists to process map updates, but on a "layered" system, where a single map can be processing 
    /// differently for each layer that exists upon it.
    /// <remarks>
    /// <para>
    /// A <see cref="MapInstance"/> contains a list <see cref="MapInstance.mMapProcessingLayers"/>, each one responsible for sending the necessary
    /// packet updates to the players that exist on that layer.
    /// </para>
    /// <para>
    /// By taking the processing out of the <see cref="MapInstance"/> itself, we can support map "Instancing" - ergo, the ability to process
    /// the same map differently for different players/parties. This allows for concepts such as dungeons and personal cutscenes, minigames,
    /// duel arenas, etc etc.
    /// </para>
    /// <para>
    /// A <see cref="Player"/> is the sole creator of MapProcessingLayers. A new layer will be created (via a call to <see cref="MapInstance.TryCreateProcessingLayer(Guid, out MapProcessingLayer)"/>)
    /// when a player:
    /// <list type="number">
    /// <item>
    /// Warps to a new map, or their <see cref="Entity.InstanceLayer"/> has otherwise changed.
    /// </item> 
    /// <item>
    /// Walks across a map boundary and fetches new maps from <see cref="MapInstance.GetSurroundingMaps(bool)"/>.
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// A <see cref="MapProcessingLayer"/> needs to know when it is ready to be cleaned up. A MapInstance can be marked for cleanup if there are no players on itself
    /// or any of its surrounding layers, and at least <see cref="Options.TimeUntilMapCleanup"/> ms have passed since the last player was on the map/its neighbors.
    /// </para>
    /// </remarks>
    /// </summary>
    public class MapProcessingLayer : IDisposable
    {
        private MapInstance mMapInstance;

        /// <summary>
        /// The last time the layer's properties were updated and packets were sent to players
        /// </summary>
        private long mLastUpdateTime;

        /// <summary>
        /// Used to determine when to process the layer (update properties & send packets to players.
        /// <remarks>
        /// This is flipped whenever we have NO players on this or surrounding processing layers.
        /// </remarks>
        /// </summary>
        private bool mIsProcessing;

        //SyncLock
        protected object mMapProcessLock = new object();

        /// <summary>
        /// A unique identifier referring to this processing layer alone.
        /// <remarks>
        /// Note that this is NOT the Instance Layer identifier - that is <see cref="InstanceLayer"/>
        /// </remarks>
        /// </summary>
        public Guid Id;

        /// <summary>
        /// An ID referring to which layer this processer belongs to.
        /// <remarks>
        /// Entities/Items/Etc with that share an <see cref="Entity.InstanceLayer"/> with <see cref="MapProcessingLayer.InstanceLayer"/> 
        /// will be processed and fed packets by that processer.
        /// </remarks>
        /// </summary>
        public Guid InstanceLayer;

        /// <summary>
        /// The last time the <see cref="Core.LogicService.LogicThread"/> made a call to <see cref="Update(long)"/>.
        /// <remarks>
        /// This is used to determine when enough time has passed to cleanup this layer (remove it from it's parent <see cref="MapInstance"/>).
        /// </remarks>
        /// </summary>
        public long LastRequestedUpdateTime;

        /// <summary>
        /// When the <see cref="Core.LogicService.LogicThread"/> started processing this layer.
        /// </summary>
        public long UpdateQueueStart = -1;

        // Players & Entities
        private ConcurrentDictionary<Guid, Player> mPlayers = new ConcurrentDictionary<Guid, Player>();
        private readonly ConcurrentDictionary<Guid, Entity> mEntities = new ConcurrentDictionary<Guid, Entity>();
        private Entity[] mCachedEntities = new Entity[0];
        private MapEntityMovements mEntityMovements = new MapEntityMovements();

        // NPCs
        public ConcurrentDictionary<NpcSpawn, MapNpcSpawn> NpcSpawnInstances = new ConcurrentDictionary<NpcSpawn, MapNpcSpawn>();

        // Items
        public ConcurrentDictionary<Guid, MapItemSpawn> ItemRespawns = new ConcurrentDictionary<Guid, MapItemSpawn>();
        public ConcurrentDictionary<Guid, MapItem>[] TileItems { get; } = new ConcurrentDictionary<Guid, MapItem>[Options.Instance.MapOpts.Width * Options.Instance.MapOpts.Height];
        public ConcurrentDictionary<Guid, MapItem> AllMapItems { get; } = new ConcurrentDictionary<Guid, MapItem>();

        // Resources
        public ConcurrentDictionary<ResourceSpawn, MapResourceSpawn> ResourceSpawnInstances = new ConcurrentDictionary<ResourceSpawn, MapResourceSpawn>();
        public ConcurrentDictionary<Guid, ResourceSpawn> ResourceSpawns { get; set; } = new ConcurrentDictionary<Guid, ResourceSpawn>();

        // Projectiles & Traps
        public ConcurrentDictionary<Guid, Projectile> MapProjectiles { get; } = new ConcurrentDictionary<Guid, Projectile>();
        public Projectile[] MapProjectilesCached = new Projectile[0];
        public long LastProjectileUpdateTime = -1;
        public ConcurrentDictionary<Guid, MapTrapInstance> MapTraps = new ConcurrentDictionary<Guid, MapTrapInstance>();
        public MapTrapInstance[] MapTrapsCached = new MapTrapInstance[0];

        // Collision
        private BytePoint[] mMapBlocks = Array.Empty<BytePoint>();
        private BytePoint[] mNpcMapBlocks = Array.Empty<BytePoint>();

        // Events
        /* Processing Layers have Global Events - these are global to the PROCESSING LAYER, NOT to the MapInstance.
         * As of the initial "Add Instancing" refactor, this is what the stock "Is Global?" event tick box in the editor
         * will enable - an "Instance-Global" event */
        public ConcurrentDictionary<EventBase, Event> GlobalEventInstances = new ConcurrentDictionary<EventBase, Event>();
        public List<EventBase> EventsCache = new List<EventBase>();

        // Animations & Text
        private MapActionMessages mActionMessages = new MapActionMessages();
        private MapAnimations mMapAnimations = new MapAnimations();

        public MapProcessingLayer(MapInstance map, Guid instanceLayer)
        {
            mMapInstance = map;
            InstanceLayer = instanceLayer;
            Id = Guid.NewGuid();

            Initialize();
        }

        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Initializes the processing layer for processing - called in the constructor. Essentially refreshes the layer
        /// so that it can give everything it has to offer to the player by the time they arrive in it
        /// </summary>
        public void Initialize()
        {
            lock (GetMapProcessLock())
            {
                mIsProcessing = true;

                CacheMapBlocks();
                DespawnEverything();
                RespawnEverything();
            }
        }

        public bool ShouldBeCleaned()
        {
            return (!mIsProcessing && LastRequestedUpdateTime > mLastUpdateTime + Options.TimeUntilMapCleanup);
        }

        public void RemoveLayerFromMapInstance()
        {
            lock (GetMapProcessLock())
            {
                mMapInstance.DisposeLayerWithId(InstanceLayer);
            }
        }

        public object GetMapProcessLock()
        {
            return mMapProcessLock;
        }

        /// <summary>
        /// Gets our _parent's_ map lock.
        /// </summary>
        /// <returns>The parent map instance's map lock</returns>
        public object GetMapInstanceLock()
        {
            return mMapInstance.GetMapLock();
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
            return mMapInstance;
        }

        /// <summary>
        /// Destroys the processable elements of the layer - RespawnEverything() to bring them back, and begin processing them
        /// in the update loop once more
        /// </summary>
        public void DespawnEverything()
        {
            DespawnNpcs();
            DespawnResources();
            DespawnProjectiles();
            DespawnTraps();
            DespawnItems();
            DespawnGlobalEvents();
        }

        /// <summary>
        /// Respawns everything - if this isn't called, the map will never begin processing anything in it's update loop, as
        /// it will have nothing to process. Note this also refreshes the event cache - refreshing events, global and local
        /// alike, when a MPL is refreshed.
        /// </summary>
        public void RespawnEverything()
        {
            SpawnMapNpcs();
            SpawnAttributeItems(); // This must be done before spawning items or resources
            SpawnMapResources();
            RefreshEventsCache();
            SpawnGlobalEvents();
        }

        /// <summary>
        /// The update loop - runs everytime the job in the LogicThread class executes it. Takes care of:
        /// - Checking if items need to spawn/be despawned
        /// - Move/process entity actions, regen
        /// - Determine if NPCs need respawned/regen'd
        /// - Determine if Resources need respawned/regen'd
        /// - Update global event processing
        /// </summary>
        /// <param name="timeMs">The current time the update was called. Used for determining updates to things such as respawn timers.</param>
        public void Update(long timeMs)
        {
            lock (GetMapProcessLock())
            {
                if (mIsProcessing)
                {
                    if (Options.Instance.Processing.MapUpdateInterval == Options.Instance.Processing.ProjectileUpdateInterval)
                    {
                        UpdateProjectiles(timeMs);
                    }

                    UpdateItems(timeMs);
                    UpdateEntities(timeMs);
                    ProcessNpcRespawns();
                    ProcessResourceRespawns();
                    UpdateGlobalEvents(timeMs);

                    SendBatchedPacketsToPlayers();
                    mLastUpdateTime = timeMs;
                }

                // If there are no players on this or surrounding processing layers, stop processing updates.
                mIsProcessing = GetPlayersOnLayer(true).Any();
                LastRequestedUpdateTime = timeMs;
            }
        }

        /// <summary>
        /// Adds an entity to the MapProcessingLayer so that the MPL knows to process them.
        /// </summary>
        /// <param name="en">The entity to add.</param>
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

        /// <summary>
        /// Removes an entity. After removal, that entity will no longer be updated by the processing layer.
        /// If this entity is a player, they will experience a complete desync from the server, as the map
        /// would not be sending them update packets any longer - so be mindful of what you're removing and when.
        /// </summary>
        /// <param name="en">The entity to remove.</param>
        public void RemoveEntity(Entity en)
        {
            mEntities.TryRemove(en.Id, out var result);
            if (mPlayers.ContainsKey(en.Id))
            {
                mPlayers.TryRemove(en.Id, out var pResult);
            }
            mCachedEntities = mEntities.Values.ToArray();
        }

        /// <summary>
        /// Returns a list of all the entities (NPCs, Resources, Players, EventPageInstances, Projectiles) being processed by this map.
        /// </summary>
        /// <param name="includeSurroundingMaps">Whether we should look at surrounding maps with a shared processing ID
        /// while acquiring this list.</param>
        /// <returns>A list of Entities</returns>
        public List<Entity> GetEntities(bool includeSurroundingMaps = false)
        {
            var entities = new List<Entity>();

            foreach (var en in mEntities)
                entities.Add(en.Value);

            // ReSharper disable once InvertIf
            if (includeSurroundingMaps)
            {
                foreach (var map in mMapInstance.GetSurroundingMaps(false))
                {
                    if (map.TryGetProcesingLayerWithId(InstanceLayer, out var mapProcessingLayer))
                    {
                        entities.AddRange(mapProcessingLayer.GetEntities());
                    }
                }
            }

            return entities;
        }

        /// <summary>
        /// Sends all the information a Player must receive when they've entered this MPL.
        /// </summary>
        /// <param name="player">The player who's entered</param>
        public void PlayerEnteredMap(Player player)
        {
            //Send Entity Info to Everyone and Everyone to the Entity
            player.Client?.SentMaps?.Clear();
            PacketSender.SendMapItems(player, mMapInstance.Id);

            AddEntity(player);

            player.LastMapEntered = mMapInstance.Id;
            var relevantMaps = mMapInstance.GetSurroundingMaps(true);
            foreach (var map in relevantMaps)
            {
                if (map.Id == mMapInstance.Id) // Map player is in
                {
                    SendMapEntitiesTo(player);
                } else if (map.TryGetProcesingLayerWithId(InstanceLayer, out var mapProcessingLayer)) // Surrounding maps
                {
                    mapProcessingLayer.SendMapEntitiesTo(player);
                }
                PacketSender.SendMapItems(player, map.Id);
            }
            PacketSender.SendEntityDataToProximity(player, player);
        }

        /// <summary>
        /// Sends entities on the map to some player.
        /// </summary>
        /// <param name="player">The player to send entities to</param>
        public void SendMapEntitiesTo(Player player)
        {
            if (player != null)
            {
                PacketSender.SendMapEntitiesTo(player, mEntities);
                if (player.MapId == mMapInstance.Id && player.InstanceLayer == InstanceLayer)
                {
                    player.SendEvents();
                }
            }
        }

        #region Players
        /// <summary>
        /// Gets all the players on a map layer
        /// </summary>
        /// <param name="includeSurrounding"></param>
        /// <returns></returns>
        public ICollection<Player> GetPlayersOnLayer(bool includeSurrounding = false)
        {
            if (!includeSurrounding)
            {
                return mPlayers.Values;
            } else
            {
                var allPlayers = new List<Player>(mPlayers.Values);
                var surroundingMaps = mMapInstance.GetSurroundingMaps();
                foreach (var map in surroundingMaps)
                {
                    if (map.TryGetProcesingLayerWithId(InstanceLayer, out var mapProcessingLayer, false))
                    {
                        var adjoiningPlayers = mapProcessingLayer.GetPlayersOnLayer(false);
                        if (adjoiningPlayers != null)
                        {
                            allPlayers.AddRange(adjoiningPlayers);
                        }
                    }
                }
                return allPlayers;
            }
        }
        #endregion

        #region NPCs
        /// <summary>
        /// Spawn map NPCs that the MapInstance has in it's list of spawns
        /// </summary>
        private void SpawnMapNpcs()
        {
            for (var i = 0; i < mMapInstance.Spawns.Count; i++)
            {
                SpawnMapNpc(i);
            }
        }

        /// <summary>
        /// Spawns a map NPC from the list given to us by out MapInstance, at some
        /// given index.
        /// </summary>
        /// <param name="i">Index of the NPC in the Map Instance's Spawns list</param>
        private void SpawnMapNpc(int i)
        {
            byte x = 0;
            byte y = 0;
            byte dir = 0;
            var spawns = mMapInstance.Spawns;
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
                        if (mMapInstance.Attributes[x, y] == null || mMapInstance.Attributes[x, y].Type == (int)MapAttributes.Walkable)
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

        /// <summary>
        /// Physically places an NPC on our map layer
        /// </summary>
        /// <param name="tileX">X Spawn Co-ordinate</param>
        /// <param name="tileY">Y Spawn Co-ordinate</param>
        /// <param name="dir">Direction to face after spawn</param>
        /// <param name="npcId">NPC Entity ID to spawn</param>
        /// <param name="despawnable">Whether or not this NPC can be despawned (for example, if spawned via event command)</param>
        /// <returns></returns>
        public Entity SpawnNpc(byte tileX, byte tileY, byte dir, Guid npcId, bool despawnable = false)
        {
            var npcBase = NpcBase.Get(npcId);
            if (npcBase != null)
            {
                var processLayer = this.InstanceLayer;
                var npc = new Npc(npcBase, despawnable)
                {
                    MapId = mMapInstance.Id,
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

        /// <summary>
        /// Despawns all NPCs, and removes them from our dictionary of Spawn Instances.
        /// </summary>
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

        /// <summary>
        /// Clears the given entity of any targets that an NPC has on them. For AI purposes.
        /// </summary>
        /// <param name="en">The entitiy to clear targets of.</param>
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
        /// <summary>
        /// Spawns a map resource attribute at its editor-given location
        /// </summary>
        /// <param name="x">X co-ordinate to spawn at</param>
        /// <param name="y">Y co-ordinate to spawn at</param>
        private void SpawnAttributeResource(byte x, byte y)
        {
            var tempResource = new ResourceSpawn()
            {
                ResourceId = ((MapResourceAttribute)mMapInstance.Attributes[x, y]).ResourceId,
                X = x,
                Y = y,
                Z = ((MapResourceAttribute)mMapInstance.Attributes[x, y]).SpawnLevel
            };

            ResourceSpawns.TryAdd(tempResource.Id, tempResource);
        }

        /// <summary>
        /// Freshly spawns all map resources within our list of Resource Spawns.
        /// </summary>
        private void SpawnMapResources()
        {
            foreach (var spawn in ResourceSpawns)
            {
                SpawnMapResource(spawn.Value);
            }
        }

        /// <summary>
        /// Spawns the given resource, and adds it to our list for processing
        /// </summary>
        /// <param name="spawn"></param>
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
                    res.MapId = mMapInstance.Id;
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

        /// <summary>
        /// Despawns all resources, and clears our list of spawn instances
        /// </summary>
        private void DespawnResources()
        {
            //Kill all resources spawned from this map
            lock (GetMapInstanceLock())
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
                        PacketSender.SendMapItemUpdate(mMapInstance.Id, InstanceLayer, reItem, true);
                    }
                }

                // Drop the new item.
                AddItem(mapItem);
                if (sendUpdate)
                {
                    PacketSender.SendMapItemUpdate(mMapInstance.Id, InstanceLayer, mapItem, false);
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
                PacketSender.SendMapItemsToProximity(mMapInstance.Id, this);
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

        /// <summary>
        /// Removes some item from this processing layer
        /// </summary>
        /// <param name="item">The item to remove from the layer</param>
        /// <param name="respawn">Whether or not this item will respawn</param>
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
                PacketSender.SendMapItemUpdate(mMapInstance.Id, InstanceLayer, item, true, item.VisibleToAll, oldOwner);
            }
        }

        /// <summary>
        /// Despawns all items, and removes them from our list of them.
        /// </summary>
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

        /// <summary>
        /// Spawns an item that was placed via editor Item map attribute
        /// </summary>
        /// <param name="x">X co-ordinate to spawn</param>
        /// <param name="y">Y co-ordinate to spawn</param>
        private void SpawnAttributeItem(int x, int y)
        {
            var item = ItemBase.Get(((MapItemAttribute)mMapInstance.Attributes[x, y]).ItemId);
            if (item != null)
            {
                var mapItem = new MapItem(item.Id, ((MapItemAttribute)mMapInstance.Attributes[x, y]).Quantity, x, y);
                mapItem.DespawnTime = -1;
                mapItem.AttributeSpawnX = x;
                mapItem.AttributeSpawnY = y;
                if (item.ItemType == ItemTypes.Equipment)
                {
                    mapItem.Quantity = 1;
                }
                AddItem(mapItem);
                PacketSender.SendMapItemUpdate(mMapInstance.Id, InstanceLayer, mapItem, false);
            }
        }

        /// <summary>
        /// Spawns map atribute items/resources
        /// </summary>
        private void SpawnAttributeItems()
        {
            ResourceSpawns.Clear();
            for (byte x = 0; x < Options.MapWidth; x++)
            {
                for (byte y = 0; y < Options.MapHeight; y++)
                {
                    if (mMapInstance.Attributes[x, y] != null)
                    {
                        if (mMapInstance.Attributes[x, y].Type == MapAttributes.Item)
                        {
                            SpawnAttributeItem(x, y);
                        }
                        else if (mMapInstance.Attributes[x, y].Type == MapAttributes.Resource)
                        {
                            SpawnAttributeResource(x, y);
                        }
                    }
                }
            }
        }
        #endregion

        #region Projectiles & Traps
        /// <summary>
        /// Spawn a projectile on the map layer
        /// </summary>
        /// <param name="owner">Who spawned the projectile</param>
        /// <param name="projectile">Which projectile to spawn</param>
        /// <param name="parentSpell">If the projectile was spawned via spell</param>
        /// <param name="parentItem">If the projectile was spawned via item</param>
        /// <param name="mapId">What MapInstance the projectile was spawned on</param>
        /// <param name="x">X co-ordinate to spawn at</param>
        /// <param name="y">Y co-ordinate ot spawn at</param>
        /// <param name="z">Z co-ordinate to spawn at, if enabled in config</param>
        /// <param name="direction">Direction in which to spawn</param>
        /// <param name="target">The target of the projectil</param>
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
            var proj = new Projectile(owner, parentSpell, parentItem, projectile, mMapInstance.Id, x, y, z, direction, target);
            proj.InstanceLayer = InstanceLayer;
            MapProjectiles.TryAdd(proj.Id, proj);
            MapProjectilesCached = MapProjectiles.Values.ToArray();
            PacketSender.SendEntityDataToProximity(proj);
        }

        /// <summary>
        /// Despawns all projectiles on the layer.
        /// </summary>
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
            PacketSender.SendRemoveProjectileSpawns(mMapInstance.Id, InstanceLayer, guids.ToArray(), null);
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
            var trap = new MapTrapInstance(owner, parentSpell, mMapInstance.Id, InstanceLayer, x, y, z);
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

        #region Events
        private void SpawnGlobalEvents()
        {
            GlobalEventInstances.Clear();
            foreach (var id in mMapInstance.EventIds)
            {
                var evt = EventBase.Get(id);
                if (evt != null && evt.Global)
                {
                    GlobalEventInstances.TryAdd(evt, new Event(evt.Id, evt, mMapInstance, InstanceLayer));
                }
            }
        }

        private void DespawnGlobalEvents()
        {
            //Kill global events on map (make sure processing stops for online players)
            //Gonna rely on GC for now
            foreach (var evt in GlobalEventInstances.ToArray())
            {
                foreach (var player in GetPlayersOnLayer(true))
                {
                    player.RemoveEvent(evt.Value.BaseEvent.Id);
                }
            }

            GlobalEventInstances.Clear();
        }

        public Event GetGlobalEventInstance(EventBase baseEvent)
        {
            if (GlobalEventInstances.ContainsKey(baseEvent))
            {
                return GlobalEventInstances[baseEvent];
            }

            return null;
        }

        public bool FindEvent(EventBase baseEvent, EventPageInstance globalClone)
        {
            if (GlobalEventInstances.ContainsKey(baseEvent))
            {
                for (var i = 0; i < GlobalEventInstances[baseEvent].GlobalPageInstance.Length; i++)
                {
                    if (GlobalEventInstances[baseEvent].GlobalPageInstance[i] == globalClone)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void RefreshEventsCache()
        {
            var events = new List<EventBase>();
            foreach (var evt in mMapInstance.EventIds)
            {
                var itm = EventBase.Get(evt);
                if (itm != null)
                {
                    events.Add(itm);
                }
            }
            EventsCache = events;
        }
        #endregion

        #region Collision
        public bool TileBlocked(int x, int y)
        {
            if (mMapInstance.Attributes == null ||
                x < 0 || x >= mMapInstance.Attributes.GetLength(0) ||
                y < 0 || y >= mMapInstance.Attributes.GetLength(1))
            {
                return true;
            }

            //Check if tile is a blocked attribute
            if (mMapInstance.Attributes[x, y] != null && (mMapInstance.Attributes[x, y].Type == MapAttributes.Blocked ||
                mMapInstance.Attributes[x, y].Type == MapAttributes.Animation && ((MapAnimationAttribute)mMapInstance.Attributes[x, y]).IsBlock))
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
                    if (mMapInstance.Attributes[x, y] != null)
                    {
                        if (mMapInstance.Attributes[x, y].Type == MapAttributes.Blocked ||
                            mMapInstance.Attributes[x, y].Type == MapAttributes.GrappleStone ||
                            mMapInstance.Attributes[x, y].Type == MapAttributes.Animation && ((MapAnimationAttribute)mMapInstance.Attributes[x, y]).IsBlock)
                        {
                            blocks.Add(new BytePoint(x, y));
                            npcBlocks.Add(new BytePoint(x, y));
                        }
                        else if (mMapInstance.Attributes[x, y].Type == MapAttributes.NpcAvoid)
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
            var surrMaps = mMapInstance.GetSurroundingMaps(true);

            // Keep a list of all entities with changed vitals and statusses.
            var vitalUpdates = new List<Entity>();
            var statusUpdates = new List<Entity>();

            foreach (var en in mEntities)
            {
                //Let's see if and how long this map has been inactive, if longer than X seconds, regenerate everything on the map
                if (timeMs > mLastUpdateTime + Options.TimeUntilMapCleanup)
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
                PacketSender.SendMapEntityVitalUpdate(mMapInstance, vitalUpdates.ToArray(), InstanceLayer);
            }

            if (statusUpdates.Count > 0)
            {
                PacketSender.SendMapEntityStatusUpdate(mMapInstance, statusUpdates.ToArray(), InstanceLayer);
            }
        }

        private void ProcessNpcRespawns()
        {
            var spawns = mMapInstance.Spawns;
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
                                                           (Globals.Timing.Milliseconds - mLastUpdateTime);
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
                PacketSender.SendRemoveProjectileSpawns(mMapInstance.Id, InstanceLayer, projDeaths.ToArray(), spawnDeaths.ToArray());
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
                    PacketSender.SendMapItemUpdate(mMapInstance.Id, InstanceLayer, mapItem, false);
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

        private void UpdateGlobalEvents(long timeMs)
        {
            var evts = GlobalEventInstances.Values.ToList();
            for (var i = 0; i < evts.Count; i++)
            {
                //Only do movement processing on the first page.
                //This is because global events need to keep all of their pages at the same tile
                //Think about a global event moving randomly that needed to turn into a warewolf and back (separate pages)
                //If they were in different tiles the transition would make the event jump
                //Something like described here: https://www.ascensiongamedev.com/community/bug_tracker/intersect/events-randomly-appearing-and-disappearing-r983/
                for (var x = 0; x < evts[i].GlobalPageInstance.Length; x++)
                {
                    //Gotta figure out if any players are interacting with this event.
                    var active = false;
                    foreach (var player in GetPlayersOnLayer(true))
                    {
                        var eventInstance = player.FindGlobalEventInstance(evts[i].GlobalPageInstance[x]);
                        if (eventInstance != null && eventInstance.CallStack.Count > 0)
                        {
                            active = true;
                        }
                    }

                    evts[i].GlobalPageInstance[x].Update(active, timeMs);
                }
            }
        }

        private void SendBatchedPacketsToPlayers()
        {
            var surrMaps = mMapInstance.GetSurroundingMaps(true);
            var nearbyPlayers = new HashSet<Player>();

            // Get all players in surrounding and current maps
            foreach (var map in surrMaps)
            {
                if (map != null && map.TryGetProcesingLayerWithId(InstanceLayer, out var mapProcessingLayer))
                {
                    foreach (var plyr in mapProcessingLayer.GetPlayersOnLayer())
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
