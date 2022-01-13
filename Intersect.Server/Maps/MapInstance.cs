using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Intersect.Compression;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;
using Intersect.Logging;
using Intersect.Server.Database;
using Intersect.Server.Entities;
using Intersect.Server.General;
using Intersect.Server.Networking;

using Newtonsoft.Json;

namespace Intersect.Server.Maps
{

    /// <summary>
    /// A <see cref="MapInstance"/> exists to provide a reference to some <see cref="Map"/> and it's neighbors, as well as give
    /// hooks to the Instance's list of <see cref="MapProcessingLayer"/>.
    /// <remarks>
    /// A Map Instance is generally not responsible for sending packets to players, that is the job of the Instance's Map Processing Layers.
    /// However, the exception to that case is whenever we want to perform some action for ALL players on a map, regardless of
    /// whatever processing layer they are currently on. In those cases, that logic should live here, and generally means iterating
    /// over the map processing layers that belong to this <see cref="MapInstance"/>.
    /// <para>
    /// A good example of this can be seen in the Respawn/Despawning functions of a <see cref="MapInstance"/> - these functions are called
    /// when we update a Map from the editor, or otherwise need to refresh what a <see cref="Map"/> looks like under the hood, and the
    /// methods go out to each processing layer to refresh the packets that are being sent to the player/instances that are being processed
    /// by the <see cref="MapProcessingLayer"/>.
    /// </para>
    /// </remarks>
    /// </summary>
    public partial class MapInstance : MapBase
    {
        private ConcurrentDictionary<Guid, MapProcessingLayer> mMapProcessingLayers = new ConcurrentDictionary<Guid, MapProcessingLayer>();

        private static MapInstances sLookup;

        //Location of Map in the current grid
        [JsonIgnore] [NotMapped] public int MapGrid;

        [JsonIgnore] [NotMapped] public int MapGridX = -1;

        [JsonIgnore] [NotMapped] public int MapGridY = -1;

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
                DespawnAllProcessingLayers();
                RespawnAllProcessingLayers();
            }
        }

        public void Load(string json, int keepRevision = -1)
        {
            lock (mMapLock)
            {
                DespawnAllProcessingLayers();
                base.Load(json);
                if (keepRevision > -1)
                {
                    Revision = keepRevision;
                }
            }
        }

        public void DespawnNpcAcrossProcessingLayers(NpcBase npcBase)
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

        public void DespawnResourceAcrossProcessingLayers(ResourceBase resourceBase)
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

        public void DespawnProjectileAcrossProcessingLayers(ProjectileBase projectileBase)
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

        public void DespawnItemAcrossProcessingLayers(ItemBase itemBase)
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

        public MapInstance[] GetSurroundingMaps(bool includingSelf = false)
        {
            return includingSelf ? mSurroundingMapsWithSelf : mSurroundingMaps;
        }

        public Guid[] GetSurroundingMapIds(bool includingSelf = false)
        {
            return includingSelf ? mSurroundingMapsIdsWithSelf : mSurroundingMapIds;
        }

        public ICollection<Player> GetPlayersOnAllLayers()
        {
            var players = new List<Player>();
            foreach (var layer in mMapProcessingLayers.Keys.ToList())
            {
                players.AddRange(mMapProcessingLayers[layer].GetPlayersOnLayer());
            }
            return players;
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

        #region Map Processing Layer management
        public void DespawnAllProcessingLayers()
        {
            foreach (var mpl in mMapProcessingLayers.Values)
            {
                mpl.DespawnEverything();
            }
        }

        public void RespawnAllProcessingLayers()
        {
            foreach (var mpl in mMapProcessingLayers.Values)
            {
                mpl.RespawnEverything();
            }
        }

        /// <summary>
        /// Creates a processing layer
        /// </summary>
        /// <param name="processingLayerId"></param>
        /// <returns>Whether or not we needed to create a new processing layer</returns>
        public bool TryCreateProcessingLayer(Guid processingLayerId, out MapProcessingLayer newLayer)
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
            }
            else
            {
                return false;
            }
        }

        public List<MapProcessingLayer> GetAllProcessingLayers()
        {
            return mMapProcessingLayers.Values.ToList();
        }

        public bool TryGetProcesingLayerWithId(Guid instanceLayer, out MapProcessingLayer mpl, bool createIfNew = false)
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
                    if (TryCreateProcessingLayer(instanceLayer, out var newProcessingLayer))
                    {
                        mpl = newProcessingLayer;
                        return true;
                    }
                }
            }
            return false;
        }

        public void RemoveEntityFromAllSurroundingMapsInLayer(Entity entity, Guid instanceLayer)
        {
            foreach (var map in GetSurroundingMaps(true))
            {
                if (map != null && map.TryGetProcesingLayerWithId(instanceLayer, out var mapProcessingLayer))
                {
                    mapProcessingLayer.RemoveEntity(entity);
                }
            }
        }

        public void DisposeLayerWithId(Guid instanceLayer)
        {
            lock (GetMapLock())
            {
                if (mMapProcessingLayers[instanceLayer] != null)
                {
                    if (mMapProcessingLayers.TryRemove(instanceLayer, out var removedLayer))
                    {
                        removedLayer.Dispose();
                        Log.Debug($"Cleaning up MPL {instanceLayer} for map: {Name}");
                    }
                }
            }
        }

        public void TryRemoveDeadProcessingLayers()
        {
            lock (GetMapLock())
            {
                // Removes all processing layers that don't have active players on themselves or any adjoining layers
                var deadLayers = mMapProcessingLayers.Where(kv => kv.Value.GetPlayersOnLayer(true).Count <= 0).ToList();
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
                foreach (var player in layer.GetPlayersOnLayer())
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
        #endregion

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

        ~MapInstance()
        {
            ClearAllProcessingLayers();
        }
    }
}
