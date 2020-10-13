using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect.Logging;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Database;
using Intersect.Server.Entities;
using Intersect.Server.Entities.Events;
using Intersect.Server.General;
using Intersect.Server.Networking;
using Intersect.Utilities;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Server.Maps
{

    public class MapInstance : MapBase
    {

        private static MapInstances sLookup;

        [NotNull, NotMapped] private readonly ConcurrentDictionary<Guid,Entity> mEntities = new ConcurrentDictionary<Guid, Entity>();

        //Does the map have a player on or nearby it?
        [JsonIgnore] [NotMapped] public bool Active;

        [JsonIgnore] [NotMapped]
        public ConcurrentDictionary<EventBase, Event> GlobalEventInstances = new ConcurrentDictionary<EventBase, Event>();

        [JsonIgnore] [NotMapped] public List<MapItemSpawn> ItemRespawns = new List<MapItemSpawn>();

        [JsonIgnore] [NotMapped] public long LastUpdateTime = -1;

        //Location of Map in the current grid
        [JsonIgnore] [NotMapped] public int MapGrid;

        [JsonIgnore] [NotMapped] public int MapGridX = -1;

        [JsonIgnore] [NotMapped] public int MapGridY = -1;

        //Traps
        [JsonIgnore] [NotMapped] public List<MapTrapInstance> MapTraps = new List<MapTrapInstance>();

        [NotMapped] private BytePoint[] mMapBlocks = new BytePoint[0];

        private BytePoint[] mNpcMapBlocks = new BytePoint[0];

        private ConcurrentDictionary<Guid, Player> mPlayers = new ConcurrentDictionary<Guid, Player>();

        [JsonIgnore] [NotMapped]
        public Dictionary<NpcSpawn, MapNpcSpawn> NpcSpawnInstances = new Dictionary<NpcSpawn, MapNpcSpawn>();

        [JsonIgnore] [NotMapped]
        public Dictionary<ResourceSpawn, MapResourceSpawn> ResourceSpawnInstances =
            new Dictionary<ResourceSpawn, MapResourceSpawn>();

        //Temporary Values
        [JsonIgnore] [NotMapped] public List<Guid> SurroundingMaps = new List<Guid>();

        [JsonIgnore] [NotMapped] public long TileAccessTime;

        [JsonIgnore] [NotMapped] public long UpdateDelay = 75;

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

        [NotNull]
        [JsonIgnore]
        [NotMapped]
        public List<MapItem> MapItems { get; } = new List<MapItem>();

        //Projectiles
        [JsonIgnore]
        [NotMapped]
        [NotNull]
        public List<Projectile> MapProjectiles { get; } = new List<Projectile>();

        [NotMapped]
        [JsonIgnore]
        public List<ResourceSpawn> ResourceSpawns { get; set; } = new List<ResourceSpawn>();

        [NotNull]
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
                            Attributes[x, y].Type == MapAttributes.GrappleStone)
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
        public void SpawnItem(int x, int y, Item item, int amount, Guid owner)
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
            if (itemDescriptor.Stackable || Options.Loot.ConsolidateMapDrops)
            {
                var mapItem = new MapItem(item.ItemId, amount, item.BagId, item.Bag) {
                    X = x,
                    Y = y,
                    DespawnTime = Globals.Timing.Milliseconds + Options.Loot.ItemDespawnTime,
                    Owner = owner,
                    OwnershipTime = Globals.Timing.Milliseconds + Options.Loot.ItemOwnershipTime,
                    VisibleToAll = Options.Loot.ShowUnownedItems
                };

                // If this is a piece of equipment, set up the stat buffs for it.
                if (itemDescriptor.ItemType == ItemTypes.Equipment)
                {
                    mapItem.SetupStatBuffs(item);
                }

                MapItems.Add(mapItem);
                PacketSender.SendMapItemUpdate(Id, MapItems.Count - 1);
            }
            else
            {
                // Oh boy here we go! Set quantity to 1 and drop multiple!
                for (var i = 0; i < amount; i++)
                {
                    var mapItem = new MapItem(item.ItemId, amount, item.BagId, item.Bag) {
                        X = x,
                        Y = y,
                        DespawnTime = Globals.Timing.Milliseconds + Options.Loot.ItemDespawnTime,
                        Owner = owner,
                        OwnershipTime = Globals.Timing.Milliseconds + Options.Loot.ItemOwnershipTime,
                        VisibleToAll = Options.Loot.ShowUnownedItems
                    };

                    // If this is a piece of equipment, set up the stat buffs for it.
                    if (itemDescriptor.ItemType == ItemTypes.Equipment)
                    {
                        mapItem.SetupStatBuffs(item);
                    }

                    MapItems.Add(mapItem);
                }
                PacketSender.SendMapItemsToProximity(Id);
            }
            
        }

        private void SpawnAttributeItem(int x, int y)
        {
            var item = ItemBase.Get(((MapItemAttribute) Attributes[x, y]).ItemId);
            if (item != null)
            {
                MapItems.Add(
                    new MapItem(
                        ((MapItemAttribute) Attributes[x, y]).ItemId, ((MapItemAttribute) Attributes[x, y]).Quantity
                    )
                );

                MapItems[MapItems.Count - 1].X = x;
                MapItems[MapItems.Count - 1].Y = y;
                MapItems[MapItems.Count - 1].DespawnTime = -1;
                MapItems[MapItems.Count - 1].AttributeSpawnX = x;
                MapItems[MapItems.Count - 1].AttributeSpawnY = y;
                if (item.ItemType == ItemTypes.Equipment)
                {
                    MapItems[MapItems.Count - 1].Quantity = 1;
                    var r = new Random();
                    for (var i = 0; i < (int) Stats.StatCount; i++)
                    {
                        MapItems[MapItems.Count - 1].StatBuffs[i] = r.Next(-1 * item.StatGrowth, item.StatGrowth + 1);
                    }
                }

                PacketSender.SendMapItemUpdate(Id, MapItems.Count - 1);
            }
        }

        public void RemoveItem(int index, bool respawn = true)
        {
            lock (MapItems)
            {
                if (index < MapItems.Count && MapItems[index] != null)
                {
                    MapItems[index].ItemId = Guid.Empty;
                    PacketSender.SendMapItemUpdate(Id, index);
                    if (respawn)
                    {
                        if (MapItems[index].AttributeSpawnX > -1)
                        {
                            ItemRespawns.Add(new MapItemSpawn());
                            ItemRespawns[ItemRespawns.Count - 1].AttributeSpawnX = MapItems[index].AttributeSpawnX;
                            ItemRespawns[ItemRespawns.Count - 1].AttributeSpawnY = MapItems[index].AttributeSpawnY;
                            ItemRespawns[ItemRespawns.Count - 1].RespawnTime =
                                Globals.Timing.Milliseconds + Options.Map.ItemAttributeRespawnTime;
                        }
                    }

                    MapItems[index] = null;
                }
            }
        }

        public void DespawnItems()
        {
            //Kill all items resting on map
            ItemRespawns.Clear();
            for (var i = 0; i < MapItems.Count; i++)
            {
                RemoveItem(i, false);
            }

            MapItems.Clear();
        }

        public void DespawnNpcsOf(NpcBase npcBase)
        {
            foreach (var entity in mEntities)
            {
                if (entity.Value is Npc npc && npc.Base == npcBase)
                {
                    npc.Die(0);
                }
            }
        }

        public void DespawnResourcesOf(ResourceBase resourceBase)
        {
            foreach (var entity in mEntities)
            {
                if (entity.Value is Resource res && res.Base == resourceBase)
                {
                    res.Die(0);
                }
            }
        }

        public void DespawnProjectilesOf(ProjectileBase projectileBase)
        {
            foreach (var entity in mEntities)
            {
                if (entity.Value is Projectile proj && proj.Base == projectileBase)
                {
                    proj.Die(0);
                }
            }
        }

        public void DespawnItemsOf(ItemBase itemBase)
        {
            for (var i = 0; i < MapItems.Count; i++)
            {
                if (MapItems[i] != null)
                {
                    if (ItemBase.Get(MapItems[i].ItemId) == itemBase)
                    {
                        RemoveItem(i, true);
                    }
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

            ResourceSpawns.Add(tempResource);
        }

        private void SpawnMapResources()
        {
            for (var i = 0; i < ResourceSpawns.Count; i++)
            {
                SpawnMapResource(i);
            }
        }

        private void SpawnMapResource(int i)
        {
            lock (GetMapLock())
            {
                int x = ResourceSpawns[i].X;
                int y = ResourceSpawns[i].Y;
                var id = Guid.Empty;
                MapResourceSpawn resourceSpawnInstance;
                if (ResourceSpawnInstances.ContainsKey(ResourceSpawns[i]))
                {
                    resourceSpawnInstance = ResourceSpawnInstances[ResourceSpawns[i]];
                }
                else
                {
                    resourceSpawnInstance = new MapResourceSpawn();
                    ResourceSpawnInstances.Add(ResourceSpawns[i], resourceSpawnInstance);
                }

                if (resourceSpawnInstance.Entity == null)
                {
                    var resourceBase = ResourceBase.Get(ResourceSpawns[i].ResourceId);
                    if (resourceBase != null)
                    {
                        var res = new Resource(resourceBase);
                        resourceSpawnInstance.Entity = res;
                        res.X = ResourceSpawns[i].X;
                        res.Y = ResourceSpawns[i].Y;
                        res.Z = ResourceSpawns[i].Z;
                        res.MapId = Id;
                        id = res.Id;
                        mEntities.TryAdd(res.Id, res);
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
                        mEntities.TryRemove(resourceSpawn.Value.Entity.Id, out var result);
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
                    NpcSpawnInstances.Add(Spawns[i], npcSpawnInstance);
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
                    npcSpawn.Value.Entity.Die(0);
                }

                NpcSpawnInstances.Clear();

                //Kill any other npcs on this map (only players should remain)
                foreach (var entity in mEntities)
                {
                    if (entity.Value is Npc npc)
                    {
                        npc.Die(0);
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
                    GlobalEventInstances.TryAdd(evt, new Event(evt.Id, evt, Id));
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
            lock (GetMapLock())
            {
                var proj = new Projectile(owner, parentSpell, parentItem, projectile, Id, x, y, z, direction, target);
                MapProjectiles.Add(proj);
                PacketSender.SendEntityDataToProximity(proj);
            }
        }

        public void DespawnProjectiles()
        {
            lock (GetMapLock())
            {
                //Clear Map Projectiles
                for (var i = 0; i < MapProjectiles.Count; i++)
                {
                    if (MapProjectiles[i] != null)
                    {
                        mEntities.TryRemove(MapProjectiles[i].Id, out var result);
                        MapProjectiles[i].Die();
                    }
                }

                MapProjectiles.Clear();
            }
        }

        public void SpawnTrap(Entity owner, SpellBase parentSpell, byte x, byte y, byte z)
        {
            lock (GetMapLock())
            {
                var trap = new MapTrapInstance(owner, parentSpell, Id, x, y, z);
                MapTraps.Add(trap);
            }
        }

        public void DespawnTraps()
        {
            lock (GetMapLock())
            {
                MapTraps.Clear();
            }
        }

        //Entity Processing
        public void AddEntity(Entity en)
        {
            if (en != null)
            {
                if (!mEntities.ContainsKey(en.Id))
                {
                    mEntities.TryAdd(en.Id, en);
                    if (en is Player plyr)
                    {
                        mPlayers.TryAdd(plyr.Id, plyr);
                    }
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
        }

        public void RemoveProjectile(Projectile en)
        {
            lock (GetMapLock())
            {
                MapProjectiles.Remove(en);
            }
        }

        public void RemoveTrap(MapTrapInstance trap)
        {
            lock (GetMapLock())
            {
                MapTraps.Remove(trap);
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

        //Update + Related Functions
        public void Update(long timeMs)
        {
            lock (GetMapLock())
            {
                //See if we should dispose of tile data
                if (TileAccessTime + 30000 < timeMs && TileData != null)
                {
                    //TileData = null;
                }

                //Process all of the projectiles
                for (var i = 0; i < MapProjectiles.Count; i++)
                {
                    MapProjectiles[i].Update();
                }

                //Process all of the traps
                for (var i = 0; i < MapTraps.Count; i++)
                {
                    MapTraps[i].Update();
                }

                if (!Active || CheckActive() == false || LastUpdateTime + UpdateDelay > timeMs)
                {
                    return;
                }

                //Process Items
                lock (MapItems)
                {

                    for (var i = 0; i < MapItems.Count; i++)
                    {
                        var mapItem = MapItems[i];
                        if (mapItem != null)
                        {
                            // Should this item be visible to everyone now?
                            if (!mapItem.VisibleToAll && mapItem.OwnershipTime < timeMs)
                            {
                                mapItem.VisibleToAll = true;
                                PacketSender.SendMapItemUpdate(Id, i);
                            }

                            // Do we need to delete this item?
                            if (mapItem.DespawnTime != -1 && mapItem.DespawnTime < timeMs)
                            {
                                RemoveItem(i);
                            }
                        }

                    }

                    for (var i = 0; i < ItemRespawns.Count; i++)
                    {
                        var itemRespawn = ItemRespawns[i];
                        if (itemRespawn.RespawnTime < timeMs)
                        {
                            SpawnAttributeItem(itemRespawn.AttributeSpawnX, itemRespawn.AttributeSpawnY);
                            ItemRespawns.RemoveAt(i);
                        }
                    }

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
                                en.Value.Target = null;
                            }
                        }

                        en.Value.Update(timeMs);
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
                    for (var i = 0; i < ResourceSpawns.Count; i++)
                    {
                        if (ResourceSpawnInstances.ContainsKey(ResourceSpawns[i]))
                        {
                            var resourceSpawnInstance = ResourceSpawnInstances[ResourceSpawns[i]];
                            if (resourceSpawnInstance.Entity != null && resourceSpawnInstance.Entity.IsDead)
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
                                        SpawnMapResource(i);
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
                                    var eventInstance = player.FindEvent(evts[i].GlobalPageInstance[x]);
                                    if (eventInstance != null && eventInstance.CallStack.Count > 0)
                                    {
                                        active = true;
                                    }
                                }
                            }

                            evts[i].GlobalPageInstance[x].Update(active, timeMs);
                        }
                    }
                }

                LastUpdateTime = timeMs;
            }
        }

        [NotNull]
        public List<MapInstance> GetSurroundingMaps(bool includingSelf = false)
        {
            Debug.Assert(Lookup != null, "Lookup != null");
            lock (GetMapLock())
            {
                var maps = SurroundingMaps?.Select(mapNum => Lookup.Get<MapInstance>(mapNum))
                               .Where(map => map != null)
                               .ToList() ??
                           new List<MapInstance>();

                if (includingSelf)
                {
                    maps.Add(this);
                }

                return maps;
            }
        }

        public List<Guid> GetSurroundingMapIds(bool includingSelf = false)
        {
            var maps = new List<Guid>();
            if (includingSelf)
            {
                maps.Add(Id);
            }

            maps.AddRange(SurroundingMaps.ToArray());

            return maps;
        }

        private bool CheckActive()
        {
            if (GetPlayersOnMap()?.Count > 0)
            {
                return true;
            }

            var surroundingMaps = GetSurroundingMaps(true);
            if (surroundingMaps?.Count > 0)
            {
                foreach (var t in surroundingMaps)
                {
                    var map = t;
                    if (map == null)
                    {
                        continue;
                    }

                    if (Monitor.TryEnter(map.GetMapLock(), new TimeSpan(0, 0, 0, 0, 1)))
                    {
                        try
                        {
                            if (map.GetPlayersOnMap()?.Count > 0)
                            {
                                return true;
                            }
                        }
                        finally
                        {
                            Monitor.Exit(map.GetMapLock());
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            Active = false;

            return false;
        }

        [NotNull]
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

        public ConcurrentDictionary<Guid,Entity> GetEntitiesDictionary()
        {
            return mEntities;
        }

        public ICollection<Player> GetPlayersOnMap()
        {
            return mPlayers.Values;
        }

        public void PlayerEnteredMap(Player player)
        {
            lock (GetMapLock())
            {
                Active = true;

                //Send Entity Info to Everyone and Everyone to the Entity
                SendMapEntitiesTo(player);
                player.Client?.SentMaps?.Clear();
                PacketSender.SendMapItems(player, Id);
                AddEntity(player);
                player.LastMapEntered = Id;
                if (SurroundingMaps.Count <= 0)
                {
                    return;
                }

                foreach (var t in SurroundingMaps)
                {
                    Lookup.Get<MapInstance>(t).Active = true;
                    Lookup.Get<MapInstance>(t).SendMapEntitiesTo(player);
                    PacketSender.SendMapItems(player, t);
                }

                PacketSender.SendEntityDataToProximity(player, player);
            }
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

            DbInterface.SaveGameDatabase();
        }

        public bool TileBlocked(int x, int y)
        {
            //Check if tile is a blocked attribute
            if (Attributes[x, y] != null && Attributes[x, y].Type == MapAttributes.Blocked)
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

        public override void Delete()
        {
            Lookup?.Delete(this);
        }

    }

}
