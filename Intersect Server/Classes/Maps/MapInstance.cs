
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps;
using Intersect_Server.Classes.Core;
using Intersect_Server.Classes.Entities;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Items;
using Intersect_Server.Classes.Networking;


namespace Intersect_Server.Classes.Maps
{
    public class MapInstance : MapBase
    {
        //Core
        public new const GameObject Type = GameObject.Map;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();

        //Temporary Values
        public List<int> SurroundingMaps = new List<int>();
        public List<MapItemInstance> MapItems = new List<MapItemInstance>();
        public List<MapItemSpawn> ItemRespawns = new List<MapItemSpawn>();
        private List<Entity> Entities = new List<Entity>();
        private List<Player> Players = new List<Player>();
        public Dictionary<EventBase, EventInstance> GlobalEventInstances = new Dictionary<EventBase, EventInstance>();
        public Dictionary<NpcSpawn, MapNpcSpawn> NpcSpawnInstances = new Dictionary<NpcSpawn, MapNpcSpawn>();
        public Dictionary<ResourceSpawn, MapResourceSpawn> ResourceSpawnInstances = new Dictionary<ResourceSpawn, MapResourceSpawn>();

        //Projectiles
        public List<Projectile> MapProjectiles = new List<Projectile>();

        private byte[] ClientMapData = null;

        //Location of Map in the current grid
        public int MapGrid;
        public int MapGridX = -1;
        public int MapGridY = -1;

        //Does the map have a player on or nearby it?
        public bool Active;

        //Init
        public MapInstance() : base(-1, false)
        {
            Autotiles = new MapAutotiles(this);
        }
        public MapInstance(int mapNum) : base(mapNum, false)
        {
            if (mapNum == -1)
            {
                return;
            }
            MyMapNum = mapNum;
            Autotiles = new MapAutotiles(this);
        }
        public object GetMapLock()
        {
            return _mapLock;
        }

        public override void Load(byte[] packet)
        {
            Load(packet, -1);
        }

        public void Load(byte[] packet, int keepRevision = -1)
        {
            lock (_mapLock)
            {
                ClientMapData = null;
                DespawnEverything();
                base.Load(packet);
                if (keepRevision > -1) Revision = keepRevision;
                RespawnEverything();
            }
        }

        //Autotile Caching (So Clients Don't have to calculate it)
        public MapBase[,] GenerateAutotileGrid()
        {
            MapBase[,] mapBase = new MapBase[3, 3];
            if (MapGrid >= 0)
            {
                if (Database.MapGrids[MapGrid] != null && Database.MapGrids[MapGrid].MyMaps.Contains(GetId()))
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            var x1 = MapGridX + x;
                            var y1 = MapGridY + y;
                            if (x1 >= 0 && y1 >= 0 && x1 < Database.MapGrids[MapGrid].Width &&
                                y1 < Database.MapGrids[MapGrid].Height)
                            {
                                if (x == 0 && y == 0)
                                {
                                    mapBase[x + 1, y + 1] = this;
                                }
                                else
                                {
                                    mapBase[x + 1, y + 1] = MapInstance.GetMap(Database.MapGrids[MapGrid].MyGrid[x1, y1]);
                                }
                            }
                        }
                    }
                }
            }
            return mapBase;
        }
        public void UpdateSurroundingTiles()
        {
            var grid = GenerateAutotileGrid();
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (grid[x, y] != null)
                    {
                        ((MapInstance) grid[x, y]).InitAutotiles();
                    }
                }
            }
        }
        public void InitAutotiles()
        {
            lock (GetMapLock())
            {
                Autotiles.InitAutotiles(GenerateAutotileGrid());
            }
        }

        //Get Map Data
        public byte[] GetClientMapData()
        {
            if (ClientMapData == null)
            {
                ClientMapData = base.GetMapData(true);
            }
            return ClientMapData;
        }

        //Items & Resources
        private void SpawnAttributeItems()
        {
            ResourceSpawns.Clear();
            for (int x = 0; x < Options.MapWidth; x++)
            {
                for (int y = 0; y < Options.MapHeight; y++)
                {
                    if (Attributes[x, y] != null)
                    {
                        if (Attributes[x, y].value == (int)MapAttributes.Item)
                        {
                            SpawnAttributeItem(x, y);
                        }
                        else if (Attributes[x, y].value == (int)MapAttributes.Resource)
                        {
                            SpawnAttributeResource(x, y);
                        }
                    }
                }
            }
        }
        public void SpawnItem(int x, int y, ItemInstance item, int amount)
        {
            var itemBase = ItemBase.GetItem(item.ItemNum);
            if (itemBase != null)
            {
                MapItems.Add(new MapItemInstance(item.ItemNum, item.ItemVal, item.BagId));
                MapItems[MapItems.Count - 1].X = x;
                MapItems[MapItems.Count - 1].Y = y;
                MapItems[MapItems.Count - 1].DespawnTime = Globals.System.GetTimeMs() + ServerOptions.ItemDespawnTime;
                if (itemBase.ItemType == (int)ItemTypes.Equipment)
                {
                    MapItems[MapItems.Count - 1].ItemVal = 1;
                    for (int i = 0; i < (int)Stats.StatCount; i++)
                    {
                        MapItems[MapItems.Count - 1].StatBoost[i] = item.StatBoost[i];
                    }
                }
                else
                {
                    MapItems[MapItems.Count - 1].ItemVal = amount;
                }
                PacketSender.SendMapItemUpdate(MyMapNum, MapItems.Count - 1);
            }
        }
        private void SpawnAttributeItem(int x, int y)
        {
            var item = ItemBase.GetItem(Attributes[x, y].data1);
            if (item != null)
            {
                MapItems.Add(new MapItemInstance(Attributes[x, y].data1, Attributes[x, y].data2, -1));
                MapItems[MapItems.Count - 1].X = x;
                MapItems[MapItems.Count - 1].Y = y;
                MapItems[MapItems.Count - 1].DespawnTime = -1;
                MapItems[MapItems.Count - 1].AttributeSpawnX = x;
                MapItems[MapItems.Count - 1].AttributeSpawnY = y;
                if (item.ItemType == (int)ItemTypes.Equipment)
                {
                    MapItems[MapItems.Count - 1].ItemVal = 1;
                    Random r = new Random();
                    for (int i = 0; i < (int)Stats.StatCount; i++)
                    {
                        MapItems[MapItems.Count - 1].StatBoost[i] = r.Next(-1 * item.StatGrowth, item.StatGrowth + 1);
                    }
                }
                PacketSender.SendMapItemUpdate(MyMapNum, MapItems.Count - 1);
            }
        }
        public void RemoveItem(int index, bool respawn = true)
        {
            if (index < MapItems.Count && MapItems[index] != null)
            {
                MapItems[index].ItemNum = -1;
                PacketSender.SendMapItemUpdate(MyMapNum, index);
                if (respawn)
                {
                    if (MapItems[index].AttributeSpawnX > -1)
                    {
                        ItemRespawns.Add(new MapItemSpawn());
                        ItemRespawns[ItemRespawns.Count - 1].AttributeSpawnX = MapItems[index].AttributeSpawnX;
                        ItemRespawns[ItemRespawns.Count - 1].AttributeSpawnY = MapItems[index].AttributeSpawnY;
                        ItemRespawns[ItemRespawns.Count - 1].RespawnTime = Globals.System.GetTimeMs() +
                                                                           ServerOptions.ItemRespawnTime;
                    }
                }
                MapItems[index] = null;
            }
        }
        public void DespawnItems()
        {
            //Kill all items resting on map
            ItemRespawns.Clear();
            for (int i = 0; i < MapItems.Count; i++)
            {
                RemoveItem(i, false);
            }
            MapItems.Clear();
        }
        public void DespawnItemsOf(ItemBase itemBase)
        {
            for (int i = 0; i < MapItems.Count; i++)
            {
                if (MapItems[i] != null)
                {
                    if (ItemBase.GetItem(MapItems[i].ItemNum) == itemBase)
                    {
                        RemoveItem(i, true);
                    }
                }
            }
        }

        // Resources
        private void SpawnAttributeResource(int x, int y)
        {
            var tempResource = new ResourceSpawn();
            tempResource.ResourceNum = Attributes[x, y].data1;
            tempResource.X = x;
            tempResource.Y = y;
            tempResource.Z = Attributes[x, y].data2;
            ResourceSpawns.Add(tempResource);
        }
        private void SpawnMapResources()
        {
            for (int i = 0; i < ResourceSpawns.Count; i++)
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
                int index = -1;
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
                    var resourceBase = ResourceBase.GetResource(ResourceSpawns[i].ResourceNum);
                    if (resourceBase != null)
                    {
                        index = Globals.FindOpenEntity();
                        Globals.Entities[index] = new Resource(index, resourceBase);
                        resourceSpawnInstance.Entity = (Resource)Globals.Entities[index];
                        Globals.Entities[index].CurrentX = ResourceSpawns[i].X;
                        Globals.Entities[index].CurrentY = ResourceSpawns[i].Y;
                        Globals.Entities[index].CurrentZ = ResourceSpawns[i].Z;
                        Globals.Entities[index].CurrentMap = MyMapNum;
                        Entities.Add((Resource)Globals.Entities[index]);
                    }
                }
                else
                {
                    index = resourceSpawnInstance.Entity.MyIndex;
                }
                if (index > -1)
                {
                    ((Resource)Globals.Entities[index]).Spawn();
                }
            }
        }
        private void DespawnResources()
        {
            //Kill all resources spawned from this map
            foreach (var resourceSpawn in ResourceSpawnInstances)
            {
                if (resourceSpawn.Value != null && resourceSpawn.Value.Entity != null)
                {
                    resourceSpawn.Value.Entity.Destroy(false);
                    Entities.Remove(resourceSpawn.Value.Entity);
                }
            }
            ResourceSpawnInstances.Clear();
        }

        //Npcs
        private void SpawnMapNpcs()
        {
            for (int i = 0; i < Spawns.Count; i++)
            {
                SpawnMapNpc(i);
            }
        }
        private void SpawnMapNpc(int i)
        {
            int X = 0;
            int Y = 0;
            int dir = 0;
            var npcBase = NpcBase.GetNpc(Spawns[i].NpcNum);
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
                if (Spawns[i].Dir >= 0)
                {
                    dir = Spawns[i].Dir;
                }
                else
                {
                    dir = Globals.Rand.Next(0, 4);
                }
                if (Spawns[i].X >= 0 && Spawns[i].Y >= 0)
                {
                    npcSpawnInstance.Entity = SpawnNpc(Spawns[i].X, Spawns[i].Y, dir, Spawns[i].NpcNum);
                }
                else
                {
                    for (int n = 0; n < 100; n++)
                    {
                        X = Globals.Rand.Next(1, Options.MapWidth);
                        Y = Globals.Rand.Next(1, Options.MapHeight);
                        if (Attributes[X, Y] == null || Attributes[X, Y].value == (int)MapAttributes.Walkable)
                        {
                            break;
                        }
                        X = 0;
                        Y = 0;
                    }
                    npcSpawnInstance.Entity = SpawnNpc(X, Y, dir, Spawns[i].NpcNum);
                }
            }
        }
        private void DespawnNpcs()
        {
            //Kill all npcs spawned from this map
            foreach (var npcSpawn in NpcSpawnInstances)
            {
                npcSpawn.Value.Entity.Die(false);
            }
            NpcSpawnInstances.Clear();
            Spawns.Clear();
            //Kill any other npcs on this map (only players should remain)
            var entities = Entities.ToArray();
            foreach (var entity in entities)
            {
                if (entity.GetType() == typeof(Npc))
                {
                    entity.Die(false);
                }
            }
        }
        public Entity SpawnNpc(int tileX, int tileY, int dir, int npcNum, bool despawnable = false)
        {
            var npcBase = NpcBase.GetNpc(npcNum);
            if (npcBase != null)
            {
                int index = Globals.FindOpenEntity();
                Globals.Entities[index] = new Npc(index, npcBase, despawnable);
                Globals.Entities[index].CurrentMap = MyMapNum;
                Globals.Entities[index].CurrentX = tileX;
                Globals.Entities[index].CurrentY = tileY;

                //Give NPC Drops
                for (int n = 0; n < Options.MaxNpcDrops; n++)
                {
                    if (Globals.Rand.Next(1, 101) <= npcBase.Drops[n].Chance)
                    {
                        Globals.Entities[index].Inventory.Add(new ItemInstance(npcBase.Drops[n].ItemNum, npcBase.Drops[n].Amount, -1));
                    }
                }

                AddEntity((Npc)Globals.Entities[index]);
                PacketSender.SendEntityDataToProximity(Globals.Entities[index]);
                return (Npc)Globals.Entities[index];
            }
            return null;
        }

        //Events
        private void SpawnGlobalEvents()
        {
            GlobalEventInstances.Clear();
            foreach (var evt in Events)
            {
                if (evt.Value.IsGlobal == 1)
                {
                    GlobalEventInstances.Add(evt.Value, new EventInstance(evt.Value, evt.Key, MyMapNum));
                }
            }
        }
        private void DespawnGlobalEvents()
        {
            //Kill global events on map (make sure processing stops for online players)
            //Gonna rely on GC for now
            GlobalEventInstances.Clear();
        }
        public EventInstance GetGlobalEventInstance(EventBase baseEvent)
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
                for (int i = 0; i < GlobalEventInstances[baseEvent].GlobalPageInstance.Count(); i++)
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
        public void SpawnMapProjectile(Entity owner, ProjectileBase projectile, SpellBase parentSpell, ItemBase parentItem, int Map, int X, int Y, int Z, int Direction, int Target = 0)
        {
            lock (GetMapLock())
            {
                int n = Globals.FindOpenEntity();
                MapProjectiles.Add(new Projectile(n, owner, parentSpell,parentItem, projectile, Map, X, Y, Z, Direction, Target));
                Globals.Entities[n] = MapProjectiles[MapProjectiles.Count - 1];
                PacketSender.SendEntityDataToProximity(Globals.Entities[n]);
            }
        }
        public void DespawnProjectiles()
        {
            lock (GetMapLock())
            {
                //Clear Map Projectiles
                for (int i = 0; i < MapProjectiles.Count; i++)
                {
                    if (MapProjectiles[i] != null)
                    {
                        Entities.Remove(MapProjectiles[i]);
                        MapProjectiles[i].Die();
                    }
                }
                MapProjectiles.Clear();
            }
        }

        //Entity Processing
        public void AddEntity(Entity en)
        {
            if (en != null)
            {
                lock (GetMapLock())
                {
                    if (Entities.IndexOf(en) == -1)
                    {
                        Entities.Add(en);
                        if (en.GetType() == typeof(Player))
                        {
                            Players.Add((Player) en);
                        }
                    }
                }
            }
        }
        public void RemoveEntity(Entity en)
        {
            lock (GetMapLock())
            {
                Entities.Remove(en);
                if (Players.Contains(en))
                {
                    Players.Remove((Player) en);
                }
            }
        }
        public void RemoveProjectile(Projectile en)
        {
            lock (GetMapLock())
            {
                MapProjectiles.Remove(en);
            }
        }
        public void ClearEntityTargetsOf(Entity en)
        {
            lock (GetMapLock())
            {
                foreach (var entity in Entities)
                {
                    if (entity.GetType() == typeof(Npc) && ((Npc)entity).MyTarget == en)
                    {
                        ((Npc)entity).RemoveTarget();
                    }
                }
            }
        }

        //Update + Related Functions
        public void Update()
        {
            lock (_mapLock)
            {
                if (CheckActive() == false)
                {
                    return;
                }
                //Process Items
                lock (MapItems)
                {
                    for (int i = 0; i < MapItems.Count; i++)
                    {
                        if (MapItems[i] != null && MapItems[i].DespawnTime != -1 && MapItems[i].DespawnTime < Globals.System.GetTimeMs())
                        {
                            RemoveItem(i);
                        }
                    }
                    for (int i = 0; i < ItemRespawns.Count; i++)
                    {
                        if (ItemRespawns[i].RespawnTime < Globals.System.GetTimeMs())
                        {
                            SpawnAttributeItem(ItemRespawns[i].AttributeSpawnX, ItemRespawns[i].AttributeSpawnY);
                            ItemRespawns.RemoveAt(i);
                        }
                    }
                    //Process All Entites
                    for (int i = 0; i < Entities.Count; i++)
                    {
                        if (Entities[i] != null)
                        {
                            //Active Npcs On the Map
                            if (Entities[i].GetType() == typeof(Npc))
                            {
                                ((Npc)Entities[i]).Update();
                            }
                        }
                    }
                    //Process NPC Respawns
                    for (int i = 0; i < Spawns.Count; i++)
                    {
                        if (NpcSpawnInstances.ContainsKey(Spawns[i]))
                        {
                            MapNpcSpawn npcSpawnInstance = NpcSpawnInstances[Spawns[i]];
                            if (npcSpawnInstance != null && !Globals.Entities.Contains(npcSpawnInstance.Entity))
                            {
                                if (npcSpawnInstance.RespawnTime == -1)
                                {
                                    npcSpawnInstance.RespawnTime = Globals.System.GetTimeMs() +
                                                                   ((Npc) npcSpawnInstance.Entity).MyBase.SpawnDuration*
                                                                   1000;
                                }
                                else if (npcSpawnInstance.RespawnTime < Globals.System.GetTimeMs())
                                {
                                    SpawnMapNpc(i);
                                    npcSpawnInstance.RespawnTime = -1;
                                }
                            }
                        }
                    }
                    //Process Resource Respawns
                    for (int i = 0; i < ResourceSpawns.Count; i++)
                    {
                        if (ResourceSpawnInstances.ContainsKey(ResourceSpawns[i]))
                        {
                            MapResourceSpawn resourceSpawnInstance = ResourceSpawnInstances[ResourceSpawns[i]];
                            if (resourceSpawnInstance.Entity != null && resourceSpawnInstance.Entity.IsDead)
                            {
                                if (resourceSpawnInstance.RespawnTime == -1)
                                {
                                    resourceSpawnInstance.RespawnTime = Globals.System.GetTimeMs() +
                                                                        resourceSpawnInstance.Entity.MyBase
                                                                            .SpawnDuration*1000;
                                }
                                else if (resourceSpawnInstance.RespawnTime < Globals.System.GetTimeMs())
                                {
                                    SpawnMapResource(i);
                                    resourceSpawnInstance.RespawnTime = -1;
                                }
                            }
                        }
                    }
                    //Process all of the projectiles
                    for (int i = 0; i < MapProjectiles.Count; i++)
                    {
                        MapProjectiles[i].Update();
                    }
                    //Process all global events
                    var evts = GlobalEventInstances.Values.ToList();
                    for (int i = 0; i < evts.Count; i++)
                    {
                        for (int x = 0; x < evts[i].GlobalPageInstance.Length; x++)
                        {
                            //Gotta figure out if any players are interacting with this event.
                            var active = false;
                            foreach (var map in GetSurroundingMaps(true))
                            {
                                foreach (var player in map.GetPlayersOnMap())
                                {
                                    var eventInstance = player.GetEventFromPageInstance(evts[i].GlobalPageInstance[x]);
                                    if (eventInstance != null && eventInstance.CallStack.Count > 0) active = true;
                                }
                            }
                            evts[i].GlobalPageInstance[x].Update(active);
                        }
                    }
                }
            }
        }

        public List<MapInstance> GetSurroundingMaps(bool includingSelf)
        {
            var maps = new List<MapInstance>();
            if (includingSelf) maps.Add(this);
            for (int i = 0; i < SurroundingMaps.Count; i++)
            {
                var map = MapInstance.GetMap(SurroundingMaps[i]);
                if (map != null) maps.Add(map);
            }
            return maps;
        } 
        private bool CheckActive()
        {
            if (GetPlayersOnMap().Count > 0)
            {
                return true;
            }
            else
            {
                var surroundingMaps = GetSurroundingMaps(true);
                if (surroundingMaps.Count > 0)
                {
                    foreach (var t in surroundingMaps)
                    {
                        var map = t;
                        if (map != null)
                        {
                            if (Monitor.TryEnter(map.GetMapLock(), new TimeSpan(0,0,0,0,1)))
                            {
                                try
                                {
                                    if (map.GetPlayersOnMap().Count > 0)
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
                }
            }
            Active = false;
            return false;
        }
        public List<Entity> GetEntities()
        {
            var entities = new List<Entity>();
            entities.AddRange(Entities.ToArray());
            return entities;
        }
        public List<Player> GetPlayersOnMap()
        {
            List<Player> players = new List<Player>();
            players.AddRange(Players.ToArray());
            return players;
        }
        public void PlayerEnteredMap(Player player)
        {
            lock (GetMapLock())
            {
                Active = true;
                //Send Entity Info to Everyone and Everyone to the Entity
                SendMapEntitiesTo(player);
                PacketSender.SendMapItems(player.MyClient, MyMapNum);
                AddEntity(player);
                player.LastMapEntered = this.GetId();
                if (SurroundingMaps.Count <= 0) return;
                foreach (var t in SurroundingMaps)
                {
                    MapInstance.GetMap(t).Active = true;
                    MapInstance.GetMap(t).SendMapEntitiesTo(player);
                    PacketSender.SendMapItems(player.MyClient, t);
                }
                PacketSender.SendEntityDataToProximity(player);
            }
        }
        public void SendMapEntitiesTo(Player player)
        {
            if (player != null)
            {
                PacketSender.SendMapEntitiesTo(player.MyClient, Entities);
                player.SendEvents();
            }
        }
        public void ClearConnections(int side = -1)
        {
            if (side == -1 || side == (int)Directions.Up) Up = -1;
            if (side == -1 || side == (int)Directions.Down) Down = -1;
            if (side == -1 || side == (int)Directions.Left) Left = -1;
            if (side == -1 || side == (int)Directions.Right) Right = -1;
            Database.SaveGameObject(this);
        }

        public bool TileBlocked(int x, int y)
        {
            //Check if tile is a blocked attribute
            if (Attributes[x, y] != null && Attributes[x,y].value == (int)MapAttributes.Blocked)
            {
                return true;
            }
            //See if there are any entities in the way
            var entities = GetEntities();
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] != null && entities[i].GetType() != typeof(Projectile))
                {
                    //If Npc or Player then blocked.. if resource then check
                    if (entities[i].Passable == 0  && entities[i].CurrentX == x && entities[i].CurrentY == y)
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
                    if (globalEvent.Value.PageInstance.Passable == 0 && globalEvent.Value.PageInstance.CurrentX == x &&
                        globalEvent.Value.PageInstance.CurrentY == y)
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
            DespawnGlobalEvents();
        }
        public void RespawnEverything()
        {
            SpawnAttributeItems();
            SpawnGlobalEvents();
            SpawnMapNpcs();
            SpawnMapResources();
        }

        //GameObject Functions
        public override byte[] GetData()
        {
            ClientMapData = null;
            return GetMapData(false);
        }
        public override string GetTable()
        {
            return DatabaseTable;
        }
        public override GameObject GetGameObjectType()
        {
            return Type;
        }
        public new static MapInstance GetMap(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (MapInstance)Objects[index];
            }
            return null;
        }
        public static DatabaseObject Get(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return Objects[index];
            }
            return null;
        }
        public override void Delete()
        {
            Objects.Remove(GetId());
            MapBase.Objects.Remove(GetId());
        }
        public static void ClearObjects()
        {
            Objects.Clear();
            MapBase.ClearObjects();
        }
        public static void AddObject(int index, DatabaseObject obj)
        {
            Objects.Add(index, obj);
            MapBase.Objects.Add(index, (MapBase)obj);
        }
        public static int ObjectCount()
        {
            return Objects.Count;
        }
        public static Dictionary<int, MapInstance> GetObjects()
        {
            Dictionary<int, MapInstance> objects = Objects.ToDictionary(k => k.Key, v => (MapInstance)v.Value);
            return objects;
        }
    }
}

