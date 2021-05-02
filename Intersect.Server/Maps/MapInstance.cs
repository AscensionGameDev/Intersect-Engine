using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Intersect.Compression;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.Logging;
using Intersect.Network.Packets.Server;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Database;
using Intersect.Server.Entities;
using Intersect.Server.Entities.Events;
using Intersect.Server.General;
using Intersect.Server.Networking;
using Intersect.Utilities;

using Newtonsoft.Json;

namespace Intersect.Server.Maps
{

    public partial class MapInstance : MapBase
    {

        private static MapInstances sLookup;

        [NotMapped] private readonly ConcurrentDictionary<Guid,Entity> mEntities = new ConcurrentDictionary<Guid, Entity>();

        private Entity[] mCachedEntities = new Entity[0];

        [JsonIgnore] [NotMapped]
        public ConcurrentDictionary<EventBase, Event> GlobalEventInstances = new ConcurrentDictionary<EventBase, Event>();

        [JsonIgnore] [NotMapped] public ConcurrentDictionary<Guid, MapItemSpawn> ItemRespawns = new ConcurrentDictionary<Guid, MapItemSpawn>();

        [JsonIgnore] [NotMapped] public long LastUpdateTime = -1;

        [JsonIgnore] [NotMapped] public long UpdateQueueStart = -1;

        [JsonIgnore] [NotMapped] public long LastProjectileUpdateTime = -1;

        //Location of Map in the current grid
        [JsonIgnore] [NotMapped] public int MapGrid;

        [JsonIgnore] [NotMapped] public int MapGridX = -1;

        [JsonIgnore] [NotMapped] public int MapGridY = -1;

        //Traps
        [JsonIgnore] [NotMapped] public ConcurrentDictionary<Guid, MapTrapInstance> MapTraps = new ConcurrentDictionary<Guid, MapTrapInstance>();

        [JsonIgnore]
        [NotMapped]
        public MapTrapInstance[] MapTrapsCached = new MapTrapInstance[0];

        [NotMapped] private BytePoint[] mMapBlocks = Array.Empty<BytePoint>();

        private BytePoint[] mNpcMapBlocks = Array.Empty<BytePoint>();

        private ConcurrentDictionary<Guid, Player> mPlayers = new ConcurrentDictionary<Guid, Player>();

        [JsonIgnore] [NotMapped]
        public ConcurrentDictionary<NpcSpawn, MapNpcSpawn> NpcSpawnInstances = new ConcurrentDictionary<NpcSpawn, MapNpcSpawn>();

        [JsonIgnore] [NotMapped]
        public ConcurrentDictionary<ResourceSpawn, MapResourceSpawn> ResourceSpawnInstances = new ConcurrentDictionary<ResourceSpawn, MapResourceSpawn>();

        //Temporary Values
        private Guid[] mSurroundingMapIds = new Guid[0];
        private Guid[] mSurroundingMapsIdsWithSelf = new Guid[0];
        private MapInstance[] mSurroundingMaps = new MapInstance[0];
        private MapInstance[] mSurroundingMapsWithSelf = new MapInstance[0];
        private MapEntityMovements mEntityMovements = new MapEntityMovements();
        private MapActionMessages mActionMessages = new MapActionMessages();
        private MapAnimations mMapAnimations = new MapAnimations();

        [JsonIgnore]
        [NotMapped]
        public Guid[] SurroundingMapIds
        {
            get => mSurroundingMapIds;

            set
            {
                lock (GetMapLock())
                {
                    mSurroundingMapIds = value;
                    var surroundingMapsIdsWithSelf = new List<Guid>(value);
                    surroundingMapsIdsWithSelf.Add(Id);
                    mSurroundingMapsIdsWithSelf = surroundingMapsIdsWithSelf.ToArray();
                }
            }
        }

        [JsonIgnore]
        [NotMapped]
        public MapInstance[] SurroundingMaps
        {
            get => mSurroundingMaps;

            set
            {
                lock (GetMapLock())
                {
                    mSurroundingMaps = value;
                    var surroundingMapsWithSelf = new List<MapInstance>(value);
                    surroundingMapsWithSelf.Add(this);
                    mSurroundingMapsWithSelf = surroundingMapsWithSelf.ToArray();
                }
            }
        }

        //EF
        public MapInstance() : base()
        {
            Name = "New Map";
            Layers = null;
        }

        //For New Maps!
        public MapInstance(bool newMap = false) : base(Guid.NewGuid())
        {
            Name = "New Map";
            Layers = null;
        }

        [JsonConstructor]
        public MapInstance(Guid id) : base(id)
        {
            if (id == Guid.Empty)
            {
                return;
            }

            Layers = null;
        }

        [JsonIgnore]
        [NotMapped]
        public ConcurrentDictionary<Guid, MapItem>[] TileItems { get; } = new ConcurrentDictionary<Guid, MapItem>[Options.Instance.MapOpts.Width * Options.Instance.MapOpts.Height];

        [JsonIgnore]
        [NotMapped]
        public ConcurrentDictionary<Guid, MapItem> AllMapItems { get; } = new ConcurrentDictionary<Guid, MapItem>();

        //Projectiles
        [JsonIgnore]
        [NotMapped]
        public ConcurrentDictionary<Guid, Projectile> MapProjectiles { get; } = new ConcurrentDictionary<Guid, Projectile>();

        [JsonIgnore]
        [NotMapped]
        public Projectile[] MapProjectilesCached = new Projectile[0];

        [NotMapped]
        [JsonIgnore]
        public ConcurrentDictionary<Guid, ResourceSpawn> ResourceSpawns { get; set; } = new ConcurrentDictionary<Guid, ResourceSpawn>();

        public new static MapInstances Lookup => sLookup = sLookup ?? new MapInstances(MapBase.Lookup);

        //GameObject Functions

        public object GetMapLock()
        {
            return mMapLock;
        }

        public override void Load(string json, bool keepCreationTime = true)
        {
            Load(json, -1);
        }

        public void Initialize()
        {
            lock (mMapLock)
            {
                CacheMapBlocks();
                DespawnEverything();
                RespawnEverything();

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
            }
        }

        public void Load(string json, int keepRevision = -1)
        {
            lock (mMapLock)
            {
                DespawnEverything();
                base.Load(json);
                if (keepRevision > -1)
                {
                    Revision = keepRevision;
                }
            }
        }

        private void CacheMapBlocks()
        {
            var blocks = new List<BytePoint>();
            var npcBlocks = new List<BytePoint>();
            for (byte x = 0; x < Options.MapWidth; x++)
            {
                for (byte y = 0; y < Options.MapHeight; y++)
                {
                    if (Attributes[x, y] != null)
                    {
                        if (Attributes[x, y].Type == MapAttributes.Blocked ||
                            Attributes[x, y].Type == MapAttributes.GrappleStone ||
                            Attributes[x,y].Type == MapAttributes.Animation && ((MapAnimationAttribute)Attributes[x,y]).IsBlock) 
                        {
                            blocks.Add(new BytePoint(x, y));
                            npcBlocks.Add(new BytePoint(x, y));
                        }
                        else if (Attributes[x, y].Type == MapAttributes.NpcAvoid)
                        {
                            npcBlocks.Add(new BytePoint(x, y));
                        }
                    }
                }
            }

            mMapBlocks = blocks.ToArray();
            mNpcMapBlocks = npcBlocks.ToArray();
        }

        public BytePoint[] GetCachedBlocks(bool isPlayer)
        {
            if (isPlayer)
            {
                return mMapBlocks;
            }

            return mNpcMapBlocks;
        }

        //Items & Resources
        private void SpawnAttributeItems()
        {
            ResourceSpawns.Clear();
            for (byte x = 0; x < Options.MapWidth; x++)
            {
                for (byte y = 0; y < Options.MapHeight; y++)
                {
                    if (Attributes[x, y] != null)
                    {
                        if (Attributes[x, y].Type == MapAttributes.Item)
                        {
                            SpawnAttributeItem(x, y);
                        }
                        else if (Attributes[x, y].Type == MapAttributes.Resource)
                        {
                            SpawnAttributeResource(x, y);
                        }
                    }
                }
            }
        }

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

                // Remove existing items if we need to.
                foreach (var reItem in toRemove)
                {
                    RemoveItem(reItem);
                    if (sendUpdate)
                    {
                        PacketSender.SendMapItemUpdate(Id, reItem, true);
                    }
                }

                // Drop the new item.
                AddItem(mapItem);
                if (sendUpdate)
                {
                    PacketSender.SendMapItemUpdate(Id, mapItem, false);
                }
            }
            else
            {
                // Oh boy here we go! Set quantity to 1 and drop multiple!
                for (var i = 0; i < amount; i++)
                {
                    var mapItem = new MapItem(item.ItemId, amount, x, y, item.BagId, item.Bag) {
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

                    AddItem(mapItem);
                }
                PacketSender.SendMapItemsToProximity(Id);
            }
            
        }

        private void SpawnAttributeItem(int x, int y)
        {
            var item = ItemBase.Get(((MapItemAttribute) Attributes[x, y]).ItemId);
            if (item != null)
            {
                var mapItem = new MapItem(item.Id, ((MapItemAttribute)Attributes[x, y]).Quantity, x, y);
                mapItem.DespawnTime = -1;
                mapItem.AttributeSpawnX = x;
                mapItem.AttributeSpawnY = y;
                if (item.ItemType == ItemTypes.Equipment)
                {
                    mapItem.Quantity = 1;
                }
                AddItem(mapItem);
                PacketSender.SendMapItemUpdate(Id, mapItem, false);
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
                PacketSender.SendMapItemUpdate(Id, item, true, item.VisibleToAll, oldOwner);
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

        public void DespawnNpcsOf(NpcBase npcBase)
        {
            foreach (var entity in mEntities)
            {
                if (entity.Value is Npc npc && npc.Base == npcBase)
                {
                    lock (npc.EntityLock)
                    {
                        npc.Die(0);
                    }
                }
            }
        }

        public void DespawnResourcesOf(ResourceBase resourceBase)
        {
            foreach (var entity in mEntities)
            {
                if (entity.Value is Resource res && res.Base == resourceBase)
                {
                    lock (res.EntityLock)
                    {
                        res.Die(0);
                    }
                }
            }
        }

        public void DespawnProjectilesOf(ProjectileBase projectileBase)
        {
            var guids = new List<Guid>();
            foreach (var entity in mEntities)
            {
                if (entity.Value is Projectile proj && proj.Base == projectileBase)
                {
                    lock (proj.EntityLock)
                    {
                        guids.Add(proj.Id);
                        proj.Die(0);
                    }
                }
            }
            PacketSender.SendRemoveProjectileSpawns(Id, guids.ToArray(), null);
        }

        public void DespawnItemsOf(ItemBase itemBase)
        {
            foreach (var item in AllMapItems.Values)
            {
                if (ItemBase.Get(item.ItemId) == itemBase)
                {
                    RemoveItem(item);
                }
            }
        }

        // Resources
        private void SpawnAttributeResource(byte x, byte y)
        {
            var tempResource = new ResourceSpawn()
            {
                ResourceId = ((MapResourceAttribute) Attributes[x, y]).ResourceId,
                X = x,
                Y = y,
                Z = ((MapResourceAttribute) Attributes[x, y]).SpawnLevel
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
                    res.MapId = Id;
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
                        resourceSpawn.Value.Entity.Destroy(0);
                        RemoveEntity(resourceSpawn.Value.Entity);
                    }
                }

                ResourceSpawnInstances.Clear();
            }
        }

        //Npcs
        private void SpawnMapNpcs()
        {
            for (var i = 0; i < Spawns.Count; i++)
            {
                SpawnMapNpc(i);
            }
        }

        private void SpawnMapNpc(int i)
        {
            byte x = 0;
            byte y = 0;
            byte dir = 0;
            var npcBase = NpcBase.Get(Spawns[i].NpcId);
            if (npcBase != null)
            {
                MapNpcSpawn npcSpawnInstance;
                if (NpcSpawnInstances.ContainsKey(Spawns[i]))
                {
                    npcSpawnInstance = NpcSpawnInstances[Spawns[i]];
                }
                else
                {
                    npcSpawnInstance = new MapNpcSpawn();
                    NpcSpawnInstances.TryAdd(Spawns[i], npcSpawnInstance);
                }

                if (Spawns[i].Direction != NpcSpawnDirection.Random)
                {
                    dir = (byte) (Spawns[i].Direction - 1);
                }
                else
                {
                    dir = (byte)Randomization.Next(0, 4);
                }

                if (Spawns[i].X >= 0 && Spawns[i].Y >= 0)
                {
                    npcSpawnInstance.Entity = SpawnNpc((byte) Spawns[i].X, (byte) Spawns[i].Y, dir, Spawns[i].NpcId);
                }
                else
                {
                    for (var n = 0; n < 100; n++)
                    {
                        x = (byte)Randomization.Next(0, Options.MapWidth);
                        y = (byte)Randomization.Next(0, Options.MapHeight);
                        if (Attributes[x, y] == null || Attributes[x, y].Type == (int) MapAttributes.Walkable)
                        {
                            break;
                        }

                        x = 0;
                        y = 0;
                    }

                    npcSpawnInstance.Entity = SpawnNpc(x, y, dir, Spawns[i].NpcId);
                }
            }
        }

        private void DespawnNpcs()
        {
            //Kill all npcs spawned from this map
            lock (GetMapLock())
            {
                foreach (var npcSpawn in NpcSpawnInstances)
                {
                    lock (npcSpawn.Value.Entity.EntityLock)
                    {
                        npcSpawn.Value.Entity.Die(0);
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
                            npc.Die(0);
                        }
                    }
                }
            }
        }

        public Entity SpawnNpc(byte tileX, byte tileY, byte dir, Guid npcId, bool despawnable = false)
        {
            var npcBase = NpcBase.Get(npcId);
            if (npcBase != null)
            {
                var npc = new Npc(npcBase, despawnable)
                {
                    MapId = Id,
                    X = tileX,
                    Y = tileY,
                    Dir = dir
                };

                AddEntity(npc);
                PacketSender.SendEntityDataToProximity(npc);

                return npc;
            }

            return null;
        }

        //Events
        private void SpawnGlobalEvents()
        {
            GlobalEventInstances.Clear();
            foreach (var id in EventIds)
            {
                var evt = EventBase.Get(id);
                if (evt != null && evt.Global)
                {
                    GlobalEventInstances.TryAdd(evt, new Event(evt.Id, evt, this));
                }
            }
        }

        private void DespawnGlobalEvents()
        {
            //Kill global events on map (make sure processing stops for online players)
            //Gonna rely on GC for now
            var players = new List<Player>();
            foreach (var map in GetSurroundingMaps(true))
            {
                players.AddRange(map.GetPlayersOnMap().ToArray());
            }

            foreach (var evt in GlobalEventInstances.ToArray())
            {
                foreach (var player in players)
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
            var proj = new Projectile(owner, parentSpell, parentItem, projectile, Id, x, y, z, direction, target);
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
            PacketSender.SendRemoveProjectileSpawns(Id, guids.ToArray(), null);
            MapProjectiles.Clear();
            MapProjectilesCached = new Projectile[0];
        }

        public void SpawnTrap(Entity owner, SpellBase parentSpell, byte x, byte y, byte z)
        {
            var trap = new MapTrapInstance(owner, parentSpell, Id, x, y, z);
            MapTraps.TryAdd(trap.Id,trap);
            MapTrapsCached = MapTraps.Values.ToArray();
        }

        public void DespawnTraps()
        {
            MapTraps.Clear();
            MapTrapsCached = new MapTrapInstance[0];
        }

        //Entity Processing
        public void AddEntity(Entity en)
        {
            if (en != null && !en.IsDead())
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

        public void RemoveProjectile(Projectile en)
        {
            MapProjectiles.TryRemove(en.Id, out Projectile removed);
            MapProjectilesCached = MapProjectiles.Values.ToArray();
        }

        public void RemoveTrap(MapTrapInstance trap)
        {
            MapTraps.TryRemove(trap.Id, out MapTrapInstance removed);
            MapTrapsCached = MapTraps.Values.ToArray();
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

        //Update + Related Functions
        public void Update(long timeMs)
        {
            lock (GetMapLock())
            {
                var surrMaps = GetSurroundingMaps(true);

                if (Options.Instance.Processing.MapUpdateInterval == Options.Instance.Processing.ProjectileUpdateInterval)
                {
                    UpdateProjectiles(timeMs);
                }

                //Process Items
                foreach (var mapItem in AllMapItems.Values)
                {
                    // Should this item be visible to everyone now?
                    if (!mapItem.VisibleToAll && mapItem.OwnershipTime < timeMs)
                    {
                        mapItem.VisibleToAll = true;
                        PacketSender.SendMapItemUpdate(Id, mapItem, false);
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

                // Keep a list of all entities with changed vitals and statusses.
                var vitalUpdates = new List<Entity>();
                var statusUpdates = new List<Entity>();

                //Process All Entites
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
                    PacketSender.SendMapEntityVitalUpdate(this, vitalUpdates.ToArray());
                }

                if (statusUpdates.Count > 0)
                {
                    PacketSender.SendMapEntityStatusUpdate(this, statusUpdates.ToArray());
                }

                //Process NPC Respawns
                for (var i = 0; i < Spawns.Count; i++)
                {
                    if (NpcSpawnInstances.ContainsKey(Spawns[i]))
                    {
                        var npcSpawnInstance = NpcSpawnInstances[Spawns[i]];
                        if (npcSpawnInstance != null && npcSpawnInstance.Entity.Dead)
                        {
                            if (npcSpawnInstance.RespawnTime == -1)
                            {
                                npcSpawnInstance.RespawnTime = Globals.Timing.Milliseconds +
                                                               ((Npc) npcSpawnInstance.Entity).Base.SpawnDuration -
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

                //Process Resource Respawns
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

                //Process all global events
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
                        foreach (var map in GetSurroundingMaps(true))
                        {
                            foreach (var player in map.GetPlayersOnMap())
                            {
                                var eventInstance = player.FindGlobalEventInstance(evts[i].GlobalPageInstance[x]);
                                if (eventInstance != null && eventInstance.CallStack.Count > 0)
                                {
                                    active = true;
                                }
                            }
                        }

                        evts[i].GlobalPageInstance[x].Update(active, timeMs);
                    }
                }

                //Send Batched Movement Packet
                var nearbyPlayers = new HashSet<Player>();
                foreach (var map in surrMaps)
                {
                    foreach (var plyr in map.GetPlayersOnMap())
                    {
                        nearbyPlayers.Add(plyr);
                    }
                }

                mEntityMovements.SendPackets(nearbyPlayers);
                mActionMessages.SendPackets(nearbyPlayers);
                mMapAnimations.SendPackets(nearbyPlayers);

                LastUpdateTime = timeMs;
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
                PacketSender.SendRemoveProjectileSpawns(Id, projDeaths.ToArray(), spawnDeaths.ToArray());
            }

            //Process all of the traps
            foreach (var trap in MapTrapsCached)
            {
                trap.Update();
            }

            LastProjectileUpdateTime = timeMs;
        }

        public MapInstance[] GetSurroundingMaps(bool includingSelf = false)
        {
            return includingSelf ? mSurroundingMapsWithSelf : mSurroundingMaps;
        }

        public Guid[] GetSurroundingMapIds(bool includingSelf = false)
        {
            return includingSelf ? mSurroundingMapsIdsWithSelf : mSurroundingMapIds;
        }

        public ConcurrentDictionary<Guid, Entity> GetLocalEntities()
        {
            return mEntities;
        }

        public List<Entity> GetEntities(bool includeSurroundingMaps = false)
        {

            var entities = new List<Entity>();

            foreach (var en in mEntities)
                entities.Add(en.Value);

            // ReSharper disable once InvertIf
            if (includeSurroundingMaps)
            {
                foreach (var map in GetSurroundingMaps(false))
                {
                    entities.AddRange(map.GetEntities());
                }
            }

            return entities;
        }

        public Entity[] GetCachedEntities()
        {
            return mCachedEntities;
        }

        public ICollection<Player> GetPlayersOnMap()
        {
            return mPlayers.Values;
        }

        public bool HasPlayersOnMap()
        {
            return !mPlayers.IsEmpty;
        }

        public void PlayerEnteredMap(Player player)
        {
            //Send Entity Info to Everyone and Everyone to the Entity
            SendMapEntitiesTo(player);
            player.Client?.SentMaps?.Clear();
            PacketSender.SendMapItems(player, Id);

            AddEntity(player);

            player.LastMapEntered = Id;
            if (SurroundingMaps.Length <= 0)
            {
                return;
            }

            foreach (var t in SurroundingMaps)
            {
                t.SendMapEntitiesTo(player);
                PacketSender.SendMapItems(player, t.Id);
            }

            PacketSender.SendEntityDataToProximity(player, player);
        }

        public void SendMapEntitiesTo(Player player)
        {
            if (player != null)
            {
                PacketSender.SendMapEntitiesTo(player, mEntities);
                if (player.MapId == Id)
                {
                    player.SendEvents();
                }
            }
        }

        public void ClearConnections(int side = -1)
        {
            if (side == -1 || side == (int) Directions.Up)
            {
                Up = Guid.Empty;
            }

            if (side == -1 || side == (int) Directions.Down)
            {
                Down = Guid.Empty;
            }

            if (side == -1 || side == (int) Directions.Left)
            {
                Left = Guid.Empty;
            }

            if (side == -1 || side == (int) Directions.Right)
            {
                Right = Guid.Empty;
            }

            DbInterface.SaveGameObject(this);
        }

        public bool TileBlocked(int x, int y)
        {
            if (Attributes == null ||
                x < 0 || x >= Attributes.GetLength(0) ||
                y < 0 || y >= Attributes.GetLength(1))
            {
                return true;
            }

            //Check if tile is a blocked attribute
            if (Attributes[x, y] != null && (Attributes[x, y].Type == MapAttributes.Blocked ||
                Attributes[x,y].Type == MapAttributes.Animation && ((MapAnimationAttribute)Attributes[x,y]).IsBlock))
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

        //Map Reseting Functions
        public void DespawnEverything()
        {
            DespawnNpcs();
            DespawnItems();
            DespawnResources();
            DespawnProjectiles();
            DespawnTraps();
            DespawnGlobalEvents();
        }

        public void RespawnEverything()
        {
            SpawnAttributeItems();
            SpawnGlobalEvents();
            SpawnMapNpcs();
            SpawnMapResources();
        }

        public static MapInstance Get(Guid id)
        {
            return MapInstance.Lookup.Get<MapInstance>(id);
        }

        public void DestroyOrphanedLayers()
        {
            if (Layers == null && TileData != null)
            {
                Layers = JsonConvert.DeserializeObject<Dictionary<string, Tile[,]>>(LZ4.UnPickleString(TileData), mJsonSerializerSettings);
                foreach (var key in Layers.Keys.ToArray())
                {
                    if (!Options.Instance.MapOpts.Layers.All.Contains(key))
                    {
                        Layers.Remove(key);
                    }
                }
                TileData = LZ4.PickleString(JsonConvert.SerializeObject(Layers, Formatting.None, mJsonSerializerSettings));
                Layers = null;
                
            }
        }

        public override void Delete()
        {
            Lookup?.Delete(this);
        }

        public Dictionary<MapInstance, List<int>> FindSurroundingTiles(Point location, int distance)
        {
            // Loop through all locations surrounding us to get valid tiles.
            var locations = new Dictionary<MapInstance, List<int>>();
            for (var x = 0 - distance; x <= distance; x++)
            {
                for (var y = 0 - distance; y <= distance; y++)
                {
                    // Use these to keep track of our translation.
                    var currentMap = this;
                    var currentX = location.X + x;
                    var currentY = location.Y + y;

                    // Are we on a valid map at all?
                    if (currentMap == null)
                    {
                        break;
                    }

                    // Are we going to the map on our left?
                    if (currentX < 0)
                    {
                        var oldMap = currentMap;
                        if (currentMap.Left != Guid.Empty)
                        {
                            currentMap = MapInstance.Get(currentMap.Left);
                            if (currentMap == null)
                            {
                                currentMap = oldMap;
                                continue;
                            }

                            currentX = (Options.MapWidth + 1) + x;
                        }
                    }

                    // Are we going to the map on our right?
                    if (currentX >= Options.MapWidth)
                    {
                        var oldMap = currentMap;
                        if (currentMap.Right != Guid.Empty)
                        {
                            currentMap = MapInstance.Get(currentMap.Right);
                            if (currentMap == null)
                            {
                                currentMap = oldMap;
                                continue;
                            }

                            currentX = -1 + x;
                        }
                    }

                    // Are we going to the map up from us?
                    if (currentY < 0)
                    {
                        var oldMap = currentMap;
                        if (currentMap.Up != Guid.Empty)
                        {
                            currentMap = MapInstance.Get(currentMap.Up);
                            if (currentMap == null)
                            {
                                currentMap = oldMap;
                                continue;
                            }

                            currentY = (Options.MapHeight + 1) + y;
                        }
                    }

                    // Are we going to the map down from us?
                    if (currentY >= Options.MapHeight)
                    {
                        var oldMap = currentMap;
                        if (currentMap.Down != Guid.Empty)
                        {
                            currentMap = MapInstance.Get(currentMap.Down);
                            if (currentMap == null)
                            {
                                currentMap = oldMap;
                                continue;
                            }

                            currentY = -1 + y;
                        }
                    }

                    if (currentX < 0 || currentY < 0 || currentX >= Options.MapWidth || currentY >= Options.MapHeight)
                    {
                        continue;
                    }

                    if (!locations.ContainsKey(currentMap))
                    {
                        locations.Add(currentMap, new List<int>());
                    }
                    locations[currentMap].Add(currentY * Options.MapWidth + currentX);
                }
            }

            return locations;
        }

        #region"Packet Batching"
        public void AddBatchedMovement(Entity en, bool correction, Player forPlayer)
        {
            mEntityMovements.Add(en, correction, forPlayer);
        }

        public void AddBatchedActionMessage(ActionMsgPacket packet)
        {
            mActionMessages.Add(packet);
        }

        public void AddBatchedAnimation(PlayAnimationPacket packet)
        {
            mMapAnimations.Add(packet);
        }

        #endregion
    }

}
