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
using Intersect.Server.Networking;

using Newtonsoft.Json;

namespace Intersect.Server.Maps
{

    /// <summary>
    /// A <see cref="MapController"/> exists to provide a reference to some <see cref="Map"/> and it's neighbors, as well as give
    /// hooks to the Instance's list of <see cref="MapInstance"/>.
    /// <remarks>
    /// A Map Instance is generally not responsible for sending packets to players, that is the job of the Instance's Map Processing Layers.
    /// However, the exception to that case is whenever we want to perform some action for ALL players on a map, regardless of
    /// whatever processing layer they are currently on. In those cases, that logic should live here, and generally means iterating
    /// over the map processing layers that belong to this <see cref="MapController"/>.
    /// <para>
    /// A good example of this can be seen in the Respawn/Despawning functions of a <see cref="MapController"/> - these functions are called
    /// when we update a Map from the editor, or otherwise need to refresh what a <see cref="Map"/> looks like under the hood, and the
    /// methods go out to each processing layer to refresh the packets that are being sent to the player/instances that are being processed
    /// by the <see cref="MapInstance"/>.
    /// </para>
    /// </remarks>
    /// </summary>
    public partial class MapController : MapBase
    {
        private ConcurrentDictionary<Guid, MapInstance> mInstances = new ConcurrentDictionary<Guid, MapInstance>();

        private static MapControllers sLookup;

        //Location of Map in the current grid
        [JsonIgnore] [NotMapped] public int MapGrid;

        [JsonIgnore] [NotMapped] public int MapGridX = -1;

        [JsonIgnore] [NotMapped] public int MapGridY = -1;

        //Temporary Values
        private Guid[] mSurroundingMapIds = new Guid[0];
        private Guid[] mSurroundingMapsIdsWithSelf = new Guid[0];
        private MapController[] mSurroundingMaps = new MapController[0];
        private MapController[] mSurroundingMapsWithSelf = new MapController[0];

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
        public MapController[] SurroundingMaps
        {
            get => mSurroundingMaps;

            set
            {
                lock (GetMapLock())
                {
                    mSurroundingMaps = value;
                    var surroundingMapsWithSelf = new List<MapController>(value);
                    surroundingMapsWithSelf.Add(this);
                    mSurroundingMapsWithSelf = surroundingMapsWithSelf.ToArray();
                }
            }
        }

        //EF
        public MapController() : base()
        {
            Name = "New Map";
            Layers = null;
        }

        //For New Maps!
        public MapController(bool newMap = false) : base(Guid.NewGuid())
        {
            Name = "New Map";
            Layers = null;
        }

        [JsonConstructor]
        public MapController(Guid id) : base(id)
        {
            if (id == Guid.Empty)
            {
                return;
            }

            Layers = null;
        }

        public new static MapControllers Lookup => sLookup = sLookup ?? new MapControllers(MapBase.Lookup);

        public static MapController Get(Guid id)
        {
            return Lookup.Get<MapController>(id);
        }

        public static bool TryGetInstanceFromMap(Guid mapControllerId, Guid instanceId, out MapInstance mapInstance, bool createIfNew = false)
        {
            mapInstance = null;
            var controller = Get(mapControllerId);
            if (controller != null && controller.TryGetInstance(instanceId, out mapInstance, createIfNew))
            {
                return mapInstance != null;
            }
            else
            {
                return false;
            }
        }

        public static List<MapInstance> GetSurroundingMapInstances(Guid mapControllerId, Guid instanceId, bool includeSelf, bool createIfNew = false)
        {
            List<MapInstance> instances = new List<MapInstance>();
            var controller = Get(mapControllerId);
            if (controller != null)
            {
                foreach (var controllerId in controller.GetSurroundingMapIds(includeSelf).ToList())
                {
                    if (TryGetInstanceFromMap(controllerId, instanceId, out var instance))
                    {
                        instances.Add(instance);
                    }
                }
            }
            return instances;
        }

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
                DespawnAllInstances();
                RespawnAllInstances();
            }
        }

        public void Load(string json, int keepRevision = -1)
        {
            lock (mMapLock)
            {
                DespawnAllInstances();
                base.Load(json);
                if (keepRevision > -1)
                {
                    Revision = keepRevision;
                }
            }
        }

        public void DespawnNpcAcrossInstances(NpcBase npcBase)
        {
            foreach (var entity in GetEntitiesOnAllInstances())
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

        public void DespawnResourceAcrossInstances(ResourceBase resourceBase)
        {
            foreach (var entity in GetEntitiesOnAllInstances())
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

        public void DespawnProjectileAcrossInstances(ProjectileBase projectileBase)
        {
            var guids = new List<Guid>();
            foreach (var entity in GetEntitiesOnAllInstances())
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

        public void DespawnItemAcrossInstances(ItemBase itemBase)
        {
            foreach(var mapInstance in mInstances.Values)
            {
                foreach (var item in mapInstance.AllMapItems.Values)
                {
                    if (ItemBase.Get(item.ItemId) == itemBase)
                    {
                        mapInstance.RemoveItem(item);
                    }
                }
            }
        }

        public MapController[] GetSurroundingMaps(bool includingSelf = false)
        {
            return includingSelf ? mSurroundingMapsWithSelf : mSurroundingMaps;
        }

        public Guid[] GetSurroundingMapIds(bool includingSelf = false)
        {
            return includingSelf ? mSurroundingMapsIdsWithSelf : mSurroundingMapIds;
        }

        public ICollection<Player> GetPlayersOnAllInstances()
        {
            var players = new List<Player>();
            foreach (var layer in mInstances.Keys.ToList())
            {
                players.AddRange(mInstances[layer].GetPlayers());
            }
            return players;
        }

        public ICollection<Entity> GetEntitiesOnAllInstances()
        {
            var entities = new List<Entity>();
            foreach (var mapInstance in mInstances)
            {
                entities.AddRange(mapInstance.Value.GetEntities());
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

        #region Map Instance Management
        public void DespawnAllInstances()
        {
            foreach (var mapInstance in mInstances.Values)
            {
                mapInstance.DespawnEverything();
            }
        }

        public void RespawnAllInstances()
        {
            foreach (var mapInstance in mInstances.Values)
            {
                mapInstance.RespawnEverything();
            }
        }

        /// <summary>
        /// Creates a processing layer
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns>Whether or not we needed to create a new processing layer</returns>
        public bool TryCreateInstance(Guid instanceId, out MapInstance newLayer)
        {
            newLayer = null;
            lock (GetMapLock())
            {
                if (!mInstances.ContainsKey(instanceId))
                {
                    Log.Debug($"Creating new processing layer {instanceId} for map {Name}");
                    mInstances[instanceId] = new MapInstance(this, instanceId);
                    newLayer = mInstances[instanceId];
                    return true;
                }

                return false;
            }
        }

        public bool TryGetInstance(Guid instanceLayer, out MapInstance instance, bool createIfNew = false)
        {
            lock (GetMapLock())
            {
                instance = null;
                if (mInstances.TryGetValue(instanceLayer, out instance))
                {
                    return true;
                }
                else
                {
                    if (createIfNew)
                    {
                        if (TryCreateInstance(instanceLayer, out instance))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public void RemoveEntityFromAllSurroundingMapsInInstance(Entity entity, Guid instanceLayer)
        {
            foreach (var instance in GetSurroundingMapInstances(Id, instanceLayer, true))
            {
                instance.RemoveEntity(entity);
            }
        }

        public void DisposeInstanceWithId(Guid instanceLayer)
        {
            lock (GetMapLock())
            {
                if (mInstances[instanceLayer] != null)
                {
                    if (mInstances.TryRemove(instanceLayer, out var removedLayer))
                    {
                        removedLayer.Dispose();
                        Log.Debug($"Cleaning up Instance {instanceLayer} for map: {Name}");
                    }
                }
            }
        }

        public List<Player> GetPlayersOnSharedInstance(Guid instanceLayer, Entity except)
        {
            var entitiesOnSharedLayer = new List<Player>();

            if (mInstances.TryGetValue(instanceLayer, out var layer))
            {
                foreach (var player in layer.GetPlayers())
                {
                    if (player != except && player.MapInstanceId == instanceLayer)
                    {
                        entitiesOnSharedLayer.Add(player);
                    }
                }
            }

            return entitiesOnSharedLayer;
        }

        public void ClearAllInstances()
        {
            mInstances.Clear();
        }
        #endregion

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

        public Dictionary<MapController, List<int>> FindSurroundingTiles(Point location, int distance)
        {
            // Loop through all locations surrounding us to get valid tiles.
            var locations = new Dictionary<MapController, List<int>>();
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
                            currentMap = MapController.Get(currentMap.Left);
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
                            currentMap = MapController.Get(currentMap.Right);
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
                            currentMap = MapController.Get(currentMap.Up);
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
                            currentMap = MapController.Get(currentMap.Down);
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

        ~MapController()
        {
            ClearAllInstances();
        }
    }
}
