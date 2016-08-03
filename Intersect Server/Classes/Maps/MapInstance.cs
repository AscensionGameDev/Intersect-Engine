/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/
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

        //Location of Map in the current grid
        public int MapGrid;
        public int MapGridX = -1;
        public int MapGridY = -1;

        //Does the map have a player on or nearby it?
        public bool Active;

        //Init
        public MapInstance() : base(-1, false)
        {

        }
        public MapInstance(int mapNum) : base(mapNum, false)
        {
            if (mapNum == -1)
            {
                return;
            }
            MyMapNum = mapNum;
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
                DespawnEverything();
                base.Load(packet);
                if (keepRevision > -1) Revision = keepRevision;
                RespawnEverything();
            }
        }

        //Get Map Data
        public byte[] GetClientMapData()
        {
            return base.GetMapData(true);
        }
        public byte[] GetEditorMapData()
        {
            return base.GetMapData(false);
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
                MapItems.Add(new MapItemInstance(item.ItemNum, item.ItemVal));
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
                MapItems.Add(new MapItemInstance(Attributes[x, y].data1, Attributes[x, y].data2));
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
                if (ItemBase.GetItem(MapItems[i].ItemNum) == itemBase)
                {
                    MapItems.RemoveAt(i);
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
                resourceSpawn.Value.Entity.Die(false);
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
            foreach (var entity in Entities)
            {
                if (entity.GetType() == typeof(Npc))
                {
                    entity.Die(false);
                }
            }
        }
        public Entity SpawnNpc(int tileX, int tileY, int dir, int npcNum)
        {
            var npcBase = NpcBase.GetNpc(npcNum);
            if (npcBase != null)
            {
                int index = Globals.FindOpenEntity();
                Globals.Entities[index] = new Npc(index, npcBase);
                Globals.Entities[index].CurrentMap = MyMapNum;
                Globals.Entities[index].CurrentX = tileX;
                Globals.Entities[index].CurrentY = tileY;

                //Give NPC Drops
                for (int n = 0; n < Options.MaxNpcDrops; n++)
                {
                    if (Globals.Rand.Next(1, 101) <= npcBase.Drops[n].Chance)
                    {
                        Globals.Entities[index].Inventory.Add(new ItemInstance(npcBase.Drops[n].ItemNum, npcBase.Drops[n].Amount));
                    }
                }

                AddEntity((Npc)Globals.Entities[index]);
                PacketSender.SendEntityDataToProximity(index, (int)EntityTypes.GlobalEntity,
                    Globals.Entities[index].Data(), Globals.Entities[index]);
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
        public void SpawnMapProjectile(Entity owner, ProjectileBase projectile, int Map, int X, int Y, int Z, int Direction, int IsSpell = -1, int Target = 0)
        {
            lock (GetMapLock())
            {
                int n = Globals.FindOpenEntity();
                MapProjectiles.Add(new Projectile(n, owner, projectile, Map, X, Y, Z, Direction, IsSpell, Target));
                Globals.Entities[n] = MapProjectiles[MapProjectiles.Count - 1];

                AddEntity(Globals.Entities[n]);
                PacketSender.SendEntityDataToProximity(n, (int)EntityTypes.Projectile, ((Projectile)Globals.Entities[n]).Data(), Globals.Entities[n]);
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
                    Players.Remove((Player)en);
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
                        ((Npc)entity).MyTarget = null;
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
                if (SurroundingMaps.Count > 0)
                {
                    foreach (var t in SurroundingMaps)
                    {
                        var map = MapInstance.GetMap(t);
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
        public void PlayerEnteredMap(Client client)
        {
            lock (GetMapLock())
            {
                Active = true;
                //Send Entity Info to Everyone and Everyone to the Entity
                SendMapEntitiesTo(client.Entity);
                PacketSender.SendMapItems(client, MyMapNum);
                AddEntity(client.Entity);
                if (SurroundingMaps.Count <= 0) return;
                foreach (var t in SurroundingMaps)
                {
                    MapInstance.GetMap(t).Active = true;
                    MapInstance.GetMap(t).SendMapEntitiesTo(client.Entity);
                    PacketSender.SendMapItems(client, t);
                }
            }
        }
        public void SendMapEntitiesTo(Player player)
        {
            if (player != null)
            {
                for (int i = 0; i < Entities.Count; i++)
                {
                    if (Entities[i] != null && Entities[i] != player)
                    {
                        if (Globals.Entities.IndexOf(Entities[i]) > -1)
                        {
                            if (Entities[i].GetType() == typeof(Player))
                            {
                                PacketSender.SendEntityDataTo(player.MyClient, Entities[i].MyIndex, (int) EntityTypes.Player,
                                    ((Player) Entities[i]).Data(), Entities[i]);
                            }
                            else if (Entities[i].GetType() == typeof(Resource))
                            {
                                PacketSender.SendEntityDataTo(player.MyClient, Entities[i].MyIndex, (int) EntityTypes.Resource,
                                    ((Resource) Entities[i]).Data(), Entities[i]);
                            }
                            else if (Entities[i].GetType() == typeof(Projectile))
                            {
                                PacketSender.SendEntityDataTo(player.MyClient, Entities[i].MyIndex, (int) EntityTypes.Projectile,
                                    ((Projectile) Entities[i]).Data(), Entities[i]);
                            }
                            else
                            {
                                PacketSender.SendEntityDataTo(player.MyClient, Entities[i].MyIndex,
                                    (int) EntityTypes.GlobalEntity,
                                    (Entities[i]).Data(), Entities[i]);
                            }
                        }
                    }
                }
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

