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
    /// A Map Controller is generally not responsible for sending packets to players, that is the job of the Instances, stored in <see cref="mInstances"/>.
    /// However, the exception to that case is whenever we want to perform some action for ALL players on a map, regardless of
    /// whatever Instance they are currently on. In those cases, that logic should live here, and generally means iterating
    /// over the map instance that belong to this <see cref="MapController"/>.
    /// <para>
    /// A good example of this can be seen in the Respawn/Despawning functions of a <see cref="MapController"/> - these functions are called
    /// when we update a Map from the editor, or otherwise need to refresh what a <see cref="Map"/> looks like under the hood, and the
    /// methods go out to each instance to refresh the packets that are being sent to the player/instances that are being processed
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

        /// <summary>
        /// Contains a list of all surrounding Map (controller) IDs
        /// </summary>
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

        /// <summary>
        /// Contains a list of all surrounding maps (controllers)
        /// </summary>
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

        /// <summary>
        /// Quick reference for DB lookup of the relevant <see cref="MapBase"/>
        /// </summary>
        public new static MapControllers Lookup => sLookup = sLookup ?? new MapControllers(MapBase.Lookup);

        /// <summary>
        /// Quick reference to get a Map Controller from its <see cref="MapBase"/> ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MapController Get(Guid id)
        {
            return Lookup.Get<MapController>(id);
        }

        /// <summary>
        /// Tries to get a <see cref="MapInstance"/> when given an instanceId.
        /// </summary>
        /// <param name="mapControllerId">The id of the <see cref="MapController"/></param>
        /// <param name="instanceId">The instance ID we want - crucially, NOT the unique ID of the <see cref="MapInstance"/>. Matches with some entity's InstanceId</param>
        /// <param name="mapInstance">Out value for the successfully found <see cref="MapInstance"/></param>
        /// <param name="createIfNew">(Default: false) Whether or not to create an instance of a controller if we can't find an instance with the requested ID</param>
        /// <returns>True if successful in retrieving a <see cref="MapInstance"/>, false otherwise</returns>
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

        /// <summary>
        /// Gets each instance of a map's surrounding maps with a given instanceID
        /// </summary>
        /// <param name="mapControllerId">The relevant map controller ID</param>
        /// <param name="instanceId">The instance ID we're requesting from each map</param>
        /// <param name="includeSelf">Whether or not to include the map of which we're checking the surroundings of</param>
        /// <param name="createIfNew">Whether or not to create any instances that do not exist with the requested InstanceID on any of the surrounding maps</param>
        /// <returns>A list of <see cref="MapInstance"/>, containing the instances of the requested surrounding maps with the given instance ID.</returns>
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

        /// <summary>
        /// Refreshes all of this controller's instances
        /// </summary>
        public void Initialize()
        {
            lock (mMapLock)
            {
                DespawnAllInstances();
                RespawnAllInstances();
            }
        }

        /// <summary>
        /// Load this map from JSON
        /// </summary>
        /// <param name="json">The JSON containing map data</param>
        /// <param name="keepRevision">The revision number</param>
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

        /// <summary>
        /// Despawns NPCs of a given <see cref="NpcBase"/> from all instances of this controller.
        /// </summary>
        /// <param name="npcBase">The <see cref="NpcBase"/> to despawn</param>
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

        /// <summary>
        /// Despawns resources of a given <see cref="ResourceBase"/> from all instances of this controller.
        /// </summary>
        /// <param name="resourceBase">The <see cref="ResourceBase"/> to despawn</param>
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

        /// <summary>
        /// Despawns projectiles of a given <see cref="ProjectileBase"/> from all instances of this controller.
        /// </summary>
        /// <param name="projectileBase">The <see cref="ProjectileBase"/> to despawn</param>
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

        /// <summary>
        /// Despawns items of a given <see cref="ItemBase"/> from all instances of this controller.
        /// </summary>
        /// <param name="itemBase">The <see cref="ItemBase"/> to despawn</param>
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

        /// <summary>
        /// Gets an array of MapControllers out of the maps surrounding this one
        /// </summary>
        /// <param name="includingSelf">Whether or not to also include the calling controller in the array</param>
        /// <returns>An array of MapControllers out of the maps surrounding this one</returns>
        public MapController[] GetSurroundingMaps(bool includingSelf = false)
        {
            return includingSelf ? mSurroundingMapsWithSelf : mSurroundingMaps;
        }

        /// <summary>
        /// Gets an array of Guids out of the map IDs surrounding this one
        /// </summary>
        /// <param name="includingSelf">Whether or not to also include the calling controller in the array</param>
        /// <returns>An array of Guids out of the maps surrounding this one</returns>
        public Guid[] GetSurroundingMapIds(bool includingSelf = false)
        {
            return includingSelf ? mSurroundingMapsIdsWithSelf : mSurroundingMapIds;
        }

        /// <summary>
        /// Gets every player that is on every instance of this map controller
        /// </summary>
        /// <returns>A generic collection of <see cref="Player"/>s</returns>
        public ICollection<Player> GetPlayersOnAllInstances()
        {
            var players = new List<Player>();
            foreach (var instance in mInstances.Keys.ToList())
            {
                players.AddRange(mInstances[instance].GetPlayers());
            }
            return players;
        }

        /// <summary>
        /// Gets every entity that is on every instance of this map controller
        /// </summary>
        /// <returns>A generic collection of <see cref="Entity"/>s</returns>s
        public ICollection<Entity> GetEntitiesOnAllInstances()
        {
            var entities = new List<Entity>();
            foreach (var mapInstance in mInstances)
            {
                entities.AddRange(mapInstance.Value.GetEntities());
            }
            return entities;
        }

        /// <summary>
        /// Destroys connections to other maps from this controller
        /// </summary>
        /// <param name="side">Which side to destroy connections from. -1 will clear all directionsm and is the default</param>
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
        /// <summary>
        /// Despawns all entities/items/projectiles on each instance that belongs to this controller
        /// </summary>
        public void DespawnAllInstances()
        {
            foreach (var mapInstance in mInstances.Values)
            {
                mapInstance.DespawnEverything();
            }
        }

        /// <summary>
        /// Respawns all entities/items/projectiles on each instance that belongs to this controller.
        /// </summary>
        public void RespawnAllInstances()
        {
            foreach (var mapInstance in mInstances.Values)
            {
                mapInstance.RespawnEverything();
            }
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns>Whether or not we needed to create a new map instance</returns>
        public bool TryCreateInstance(Guid instanceId, out MapInstance newLayer)
        {
            newLayer = null;
            lock (GetMapLock())
            {
                if (!mInstances.ContainsKey(instanceId))
                {
                    Log.Debug($"Creating new instance with ID {instanceId} for map {Name}");
                    mInstances[instanceId] = new MapInstance(this, instanceId);
                    mInstances[instanceId].Initialize();
                    newLayer = mInstances[instanceId];
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Attempts to get an instance with some given instance ID
        /// </summary>
        /// <param name="mapInstanceId">The id of the instance - NOT the unique id of a <see cref="MapInstance"/></param>
        /// <param name="instance">Out value - the mapInstance we find. Null if not found</param>
        /// <param name="createIfNew">Whether or not to create an instance if we can't find one</param>
        /// <returns>True if successful in finding the requested instance, false otherwise</returns>
        public bool TryGetInstance(Guid mapInstanceId, out MapInstance instance, bool createIfNew = false)
        {
            instance = null;
            if (mInstances.TryGetValue(mapInstanceId, out instance))
            {
                return true;
            }
            else
            {
                if (createIfNew)
                {
                    if (TryCreateInstance(mapInstanceId, out instance))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a requested <see cref="Entity"/> from a requested instance on all surrounding maps, including this one
        /// </summary>
        /// <param name="entity">The entity we wish to remove</param>
        /// <param name="mapInstanceId">The instance we're removing the entity from</param>
        public void RemoveEntityFromAllSurroundingMapsInInstance(Entity entity, Guid mapInstanceId)
        {
            foreach (var instance in GetSurroundingMapInstances(Id, mapInstanceId, true))
            {
                instance.RemoveEntity(entity);
            }
        }

        /// <summary>
        /// Disposes of an instance from this controllers <see cref="mInstances"/>
        /// </summary>
        /// <param name="mapInstanceId">The instance ID to dispose of</param>
        public void DisposeInstanceWithId(Guid mapInstanceId)
        {
            lock (GetMapLock())
            {
                if (mInstances[mapInstanceId] != null)
                {
                    if (mInstances.TryRemove(mapInstanceId, out var removedInstance))
                    {
                        removedInstance.Dispose();
                        Log.Debug($"Cleaning up Instance {mapInstanceId} for map: {Name}");
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of players that are on the same instance
        /// </summary>
        /// <param name="mapInstanceId">The instance ID to check for</param>
        /// <param name="except">An entity that we do NOT wish to include in the returned list</param>
        /// <returns>A list of players on this map who share the same instance ID</returns>
        public List<Player> GetPlayersOnSharedInstance(Guid mapInstanceId, Entity except)
        {
            var entitiesOnSharedLayer = new List<Player>();

            if (mInstances.TryGetValue(mapInstanceId, out var instance))
            {
                foreach (var player in instance.GetPlayers())
                {
                    if (player != except && player.MapInstanceId == mapInstanceId)
                    {
                        entitiesOnSharedLayer.Add(player);
                    }
                }
            }

            return entitiesOnSharedLayer;
        }

        /// <summary>
        /// Clears <see cref="mInstances"/>
        /// </summary>
        public void ClearAllInstances()
        {
            mInstances.Clear();
        }
        #endregion

        /// <summary>
        /// Gets rid of any orphaned tile layers
        /// </summary>
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

        /// <summary>
        /// Removes this map from the database
        /// </summary>
        public override void Delete()
        {
            Lookup?.Delete(this);
        }

        /// <summary>
        /// Get back a list of tiles surrounding a requested tile, with a given distance
        /// </summary>
        /// <param name="location">The tile to search for</param>
        /// <param name="distance">The distance from the requested tile to search out from</param>
        /// <returns>A dictionary of tiles on their own maps</returns>
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

        /// <summary>
        /// A destructor, responsible for making sure that if a map controller is removed, all of its instances are as well
        /// </summary>
        ~MapController()
        {
            ClearAllInstances();
        }
    }
}
