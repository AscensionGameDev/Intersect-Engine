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
        private ConcurrentDictionary<Guid, MapProcessingLayer> mMapProcessingLayers = new ConcurrentDictionary<Guid, MapProcessingLayer>();

        private static MapInstances sLookup;

        [NotMapped] private readonly ConcurrentDictionary<Guid,Entity> mEntities = new ConcurrentDictionary<Guid, Entity>();

        private Entity[] mCachedEntities = new Entity[0];

        [JsonIgnore] [NotMapped]
        public ConcurrentDictionary<EventBase, Event> GlobalEventInstances = new ConcurrentDictionary<EventBase, Event>();

        [JsonIgnore] [NotMapped] public long LastUpdateTime = -1;

        [JsonIgnore] [NotMapped] public long UpdateQueueStart = -1;

        //Location of Map in the current grid
        [JsonIgnore] [NotMapped] public int MapGrid;

        [JsonIgnore] [NotMapped] public int MapGridX = -1;

        [JsonIgnore] [NotMapped] public int MapGridY = -1;

        private ConcurrentDictionary<Guid, Player> mPlayers = new ConcurrentDictionary<Guid, Player>();

        //Temporary Values
        private Guid[] mSurroundingMapIds = new Guid[0];
        private Guid[] mSurroundingMapsIdsWithSelf = new Guid[0];
        private MapInstance[] mSurroundingMaps = new MapInstance[0];
        private MapInstance[] mSurroundingMapsWithSelf = new MapInstance[0];

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
                foreach (var kv in mMapProcessingLayers)
                {
                    kv.Value.Initialize();
                }
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

        public void DespawnNpcsOfAcrossLayers(NpcBase npcBase)
        {
            foreach (var entity in GetEntitiesOnAllLayers())
            {
                if (entity is Npc npc && npc.Base == npcBase)
                {
                    lock (npc.EntityLock)
                    {
                        npc.Die(false);
                    }
                }
            }
        }

        public void DespawnResourcesOfAcrossLayers(ResourceBase resourceBase)
        {
            foreach (var entity in GetEntitiesOnAllLayers())
            {
                if (entity is Resource res && res.Base == resourceBase)
                {
                    lock (res.EntityLock)
                    {
                        res.Die(false);
                    }
                }
            }
        }

        public void DespawnProjectilesOfAcrossLayers(ProjectileBase projectileBase)
        {
            var guids = new List<Guid>();
            foreach (var entity in GetEntitiesOnAllLayers())
            {
                if (entity is Projectile proj && proj.Base == projectileBase)
                {
                    lock (proj.EntityLock)
                    {
                        guids.Add(proj.Id);
                        proj.Die(false);
                    }
                }
            }
            PacketSender.SendRemoveProjectileSpawnsFromAllLayers(Id, guids.ToArray(), null);
        }

        public void DespawnItemsOfAcrossLayers(ItemBase itemBase)
        {
            foreach(var mpl in mMapProcessingLayers.Values)
            {
                foreach (var item in mpl.AllMapItems.Values)
                {
                    if (ItemBase.Get(item.ItemId) == itemBase)
                    {
                        mpl.RemoveItem(item);
                    }
                }
            }
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

        //Update + Related Functions
        public void Update(long timeMs)
        {
            lock (GetMapLock())
            {
                var surrMaps = GetSurroundingMaps(true);

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
                        if (en.Value is Resource)
                        {
                            en.Value.RestoreVital(Vitals.Health);
                            en.Value.RestoreVital(Vitals.Mana);
                            en.Value.Target = null;
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

                UpdateProcessingInstances(Globals.Timing.Milliseconds);

                LastUpdateTime = timeMs;
            }
        }

        public void UpdateProjectiles(long timeMs)
        {
            // TODO Alex I don't think this will be necessary once processing layers are handled in their own jobs
            mMapProcessingLayers.Values.ToList().ForEach((mpl) =>
            {
                mpl.UpdateProjectiles(timeMs);
            });
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

        // TODO Alex; Rename this
        public ICollection<Player> GetPlayersOnMap()
        {
            return GetPlayersOnAllLayers(); // TODO Alex: Players are no longer held here
        }

        public ICollection<Entity> GetEntitiesOnAllLayers()
        {
            var entities = new List<Entity>();
            foreach (var mpl in mMapProcessingLayers)
            {
                entities.AddRange(mpl.Value.GetEntities());
            }
            return entities;
        }

        public Entity[] GetCachedEntitiesOnAllLayers()
        {
            var entities = new List<Entity>();
            foreach (var mpl in mMapProcessingLayers)
            {
                entities.AddRange(mpl.Value.GetCachedEntities());
            }
            return entities.ToArray();
        }

        // TODO Alex: I think this method will eventually be invalidated
        public ICollection<Player> GetPlayersOnAllLayers()
        {
            var players = new List<Player>();
            foreach (var layer in mMapProcessingLayers.Keys.ToList())
            {
                players.AddRange(mMapProcessingLayers[layer].GetPlayersOnMap());
            }
            return players;
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

        //Map Reseting Functions
        public void DespawnEverything()
        {
            foreach (var kv in mMapProcessingLayers)
            {
                kv.Value.DespawnEverything();
            }
            DespawnGlobalEvents();
        }

        public void RespawnEverything()
        {
            foreach (var kv in mMapProcessingLayers)
            {
                kv.Value.RespawnEverything();
            }
            SpawnGlobalEvents();
        }

        public static MapInstance Get(Guid id)
        {
            return Lookup.Get<MapInstance>(id);
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

        /// <summary>
        /// Creates a processing layer
        /// </summary>
        /// <param name="processingLayerId"></param>
        /// <returns>Whether or not we needed to create a new processing layer</returns>
        public bool TryCreateProcessingInstance(Guid processingLayerId, out MapProcessingLayer newLayer)
        {
            newLayer = null;
            lock (GetMapLock())
            {
                if (!mMapProcessingLayers.ContainsKey(processingLayerId))
                {
                    Log.Debug($"Creating new processing layer {processingLayerId} for map {Name}");
                    mMapProcessingLayers[processingLayerId] = new MapProcessingLayer(this, processingLayerId);
                    newLayer = mMapProcessingLayers[processingLayerId];
                    return true;
                }

                return false;
            }
        }

        public bool TryUpdateProcessingLayer(Guid instanceId, long timeMs)
        {
            if (mMapProcessingLayers.TryGetValue(instanceId, out var mpl))
            {
                mpl.Update(timeMs);
                return true;
            } else
            {
                return false;
            }
        }

        public void UpdateProcessingInstances(long timeMs)
        {
            foreach (var instance in mMapProcessingLayers.Keys.ToList())
            {
                if (mMapProcessingLayers.TryGetValue(instance, out var mpl))
                {
                    mpl.Update(timeMs);
                }
            }
        }

        public bool TryGetRelevantProcessingLayer(Guid instanceLayer, out MapProcessingLayer mpl, bool createIfNew = false)
        {
            mpl = null;
            if (mMapProcessingLayers.TryGetValue(instanceLayer, out var processingLayer))
            {
                mpl = processingLayer;
                return true;
            }
            else
            {
                if (createIfNew)
                {
                    if (TryCreateProcessingInstance(instanceLayer, out var newProcessingLayer))
                    {
                        mpl = newProcessingLayer;
                        return true;
                    }
                }
            }
            return false;
        }

        public void RemoveEntityFromAllRelevantMapsInLayer(Entity entity, Guid instanceLayer)
        {
            foreach(var map in GetSurroundingMaps(true))
            {
                map?.RemoveEntityFromAllMapsInLayer(entity, instanceLayer);
            }
        }

        // TODO Alex rename
        public void RemoveEntityFromAllMapsInLayer(Entity entity, Guid instanceLayer)
        {
            if (TryGetRelevantProcessingLayer(instanceLayer, out var mapProcessingLayer))
            {
                mapProcessingLayer.RemoveEntity(entity);
            }
        }

        public void CleanUpLayerIfInactive(Guid instanceLayer)
        {
            lock (GetMapLock())
            {
                if (mMapProcessingLayers[instanceLayer] != null && mMapProcessingLayers[instanceLayer].GetAllRelevantPlayers().Count <= 0)
                {
                    mMapProcessingLayers[instanceLayer].Dispose();
                    if (!mMapProcessingLayers.TryRemove(instanceLayer, out var removedLayer))
                    {
                        Log.Error($"Failed to cleanup layer {instanceLayer} of map {Name}");
                    } else
                    {
                        Log.Debug($"Cleaned up layer {instanceLayer} of map {Name}");
                    }
                }
            }
        }

        public void TryRemoveDeadProcessingLayers()
        {
            lock (GetMapLock())
            {
                // Removes all processing layers that don't have active players on themselves or any adjoining layers
                var deadLayers = mMapProcessingLayers.Where(kv => kv.Value.GetAllRelevantPlayers().Count <= 0).ToList();
                var layerCountPreCleanup = mMapProcessingLayers.Keys.Count;
                foreach (var instance in deadLayers)
                {
                    if (mMapProcessingLayers.TryRemove(instance.Key, out var removedLayer))
                    {
                        removedLayer.Dispose();
                        Log.Debug($"Cleaning up MPL {instance} for map: {Name}");
                    }
                }

                if (layerCountPreCleanup != mMapProcessingLayers.Keys.Count)
                {
                    Log.Debug($"There are now {mMapProcessingLayers.Keys.Count} layer(s) remaining for map: {Name}");
                }
            }
        }

        public List<Player> GetPlayersOnSharedLayers(Guid instanceLayer, Entity except)
        {
            var entitiesOnSharedLayer = new List<Player>();

            if (mMapProcessingLayers.TryGetValue(instanceLayer, out var layer))
            {
                foreach (var player in layer.GetPlayersOnMap())
                {
                    if (player != except && player.InstanceLayer == instanceLayer)
                    {
                        entitiesOnSharedLayer.Add(player);
                    }
                }
            }

            return entitiesOnSharedLayer;
        }

        public void ClearAllProcessingLayers()
        {
            mMapProcessingLayers.Clear();
        }
    }
}
