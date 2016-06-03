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
        public List<Entity> Entities = new List<Entity>();
        public List<EventInstance> GlobalEvents = new List<EventInstance>();
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

        //SyncLock
        private Object _mapLock = new Object();

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

        public override void Load(byte[] packet)
        {
            Load(packet, -1);
        }

        public void Load(byte[] packet, int keepRevision = -1)
        {
            lock (_mapLock)
            {
                //Clear Map Npcs
                for (int i = 0; i < Spawns.Count; i++)
                {
                    if (NpcSpawnInstances.ContainsKey(Spawns[i]))
                    {
                        MapNpcSpawn npcSpawnInstance = NpcSpawnInstances[Spawns[i]];
                        if (npcSpawnInstance.Entity != null)
                        {
                            Entities.Remove(npcSpawnInstance.Entity);
                            npcSpawnInstance.Entity.Die();
                        }
                    }
                }

                //Clear Map Items
                for (int i = 0; i < MapItems.Count; i++)
                {
                    MapItems[i].ItemNum = -1;
                    PacketSender.SendMapItemUpdate(MyMapNum, i);
                    MapItems.RemoveAt(i);
                }
                ItemRespawns.Clear();
                //Clear Map Resources
                for (int i = 0; i < ResourceSpawns.Count; i++)
                {
                    if (ResourceSpawnInstances.ContainsKey(ResourceSpawns[i]))
                    {
                        MapResourceSpawn resourceSpawnInstance = ResourceSpawnInstances[ResourceSpawns[i]];
                        if (resourceSpawnInstance.Entity != null)
                        {
                            Entities.Remove(resourceSpawnInstance.Entity);
                            resourceSpawnInstance.Entity.Die();
                        }
                    }
                }
                ResourceSpawns.Clear();
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

                base.Load(packet);
                if (keepRevision > -1) Revision = keepRevision;

                SpawnAttributeItems();
                SpawnGlobalEvents();
                SpawnMapNpcs();
                SpawnMapResources();
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
                MapItems[MapItems.Count - 1].DespawnTime = Environment.TickCount + Options.ItemDespawnTime;
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
        public void RemoveItem(int index)
        {
            MapItems[index].ItemNum = -1;
            PacketSender.SendMapItemUpdate(MyMapNum, index);
            if (MapItems[index].AttributeSpawnX > -1)
            {
                ItemRespawns.Add(new MapItemSpawn());
                ItemRespawns[ItemRespawns.Count - 1].AttributeSpawnX = MapItems[index].AttributeSpawnX;
                ItemRespawns[ItemRespawns.Count - 1].AttributeSpawnY = MapItems[index].AttributeSpawnY;
                ItemRespawns[ItemRespawns.Count - 1].RespawnTime = Environment.TickCount + Options.ItemRespawnTime;
            }
            MapItems.RemoveAt(index);
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

        public Entity SpawnNpc(int tileX, int tileY, int dir, int npcNum)
        {
            var npcBase = NpcBase.GetNpc(npcNum);
            if (npcBase != null)
            {
                int index = Globals.FindOpenEntity();
                int Z = 0;
                Globals.Entities[index] = new Npc(index, npcBase);
                Globals.Entities[index].CurrentMap = MyMapNum;
                Globals.Entities[index].CurrentX = tileX;
                Globals.Entities[index].CurrentY = tileY;

                //Give NPC Drops
                for (int n = 0; n < Options.MaxNpcDrops; n++)
                {
                    if (Globals.Rand.Next(1, 101) <= npcBase.Drops[n].Chance)
                    {
                        Globals.Entities[index].Inventory[Z].ItemNum = npcBase.Drops[n].ItemNum;
                        Globals.Entities[index].Inventory[Z].ItemVal = npcBase.Drops[n].Amount;
                        Z = Z + 1;
                    }
                }

                Entities.Add((Npc)Globals.Entities[index]);
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
                    GlobalEventInstances.Add(evt.Value,new EventInstance(evt.Value,evt.Key,MyMapNum));
            }
        }
        private void DespawnGlobalEvents()
        {

        }
        public EventInstance GetGlobalEventInstance(EventBase baseEvent)
        {
            if (GlobalEventInstances.ContainsKey(baseEvent))
            {
                return GlobalEventInstances[baseEvent];
            }
            return null;
        }


        //Spawn a projectile
        public void SpawnMapProjectile(Entity owner, ProjectileBase projectile, int Map, int X, int Y, int Z, int Direction, int IsSpell = -1, int Target = 0)
        {
            int n = Globals.FindOpenEntity();
            MapProjectiles.Add(new Projectile(n, owner, projectile, Map, X, Y, Z, Direction, IsSpell, Target));
            Globals.Entities[n] = MapProjectiles[MapProjectiles.Count - 1];

            Entities.Add(Globals.Entities[n]);
            PacketSender.SendEntityDataToProximity(n, (int)EntityTypes.Projectile, ((Projectile)Globals.Entities[n]).Data(), Globals.Entities[n]);
        }

        //Entity Processing
        public void AddEntity(Entity en)
        {
            if (Entities.IndexOf(en) == -1)
            {
                Entities.Add(en);
            }
        }
        public void RemoveEntity(Entity en)
        {
            Entities.Remove(en);
        }
        public void RemoveProjectile(Projectile en)
        {
            MapProjectiles.Remove(en);
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
                        if (MapItems[i].DespawnTime != -1 && MapItems[i].DespawnTime < Environment.TickCount)
                        {
                            RemoveItem(i);
                        }
                    }
                    for (int i = 0; i < ItemRespawns.Count; i++)
                    {
                        if (ItemRespawns[i].RespawnTime < Environment.TickCount)
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
                            //Buffs/Debuffs
                            for (int n = 0; n < (int)Stats.StatCount; n++)
                            {
                                Entities[i].Stat[n].Update();
                            }

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
                            if (!Globals.Entities.Contains(npcSpawnInstance.Entity))
                            {
                                if (npcSpawnInstance.RespawnTime == -1)
                                {
                                    npcSpawnInstance.RespawnTime = Environment.TickCount +
                                                                   ((Npc) npcSpawnInstance.Entity).MyBase.SpawnDuration*
                                                                   1000;
                                }
                                else if (npcSpawnInstance.RespawnTime < Environment.TickCount)
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
                                    resourceSpawnInstance.RespawnTime = Environment.TickCount +
                                                                        resourceSpawnInstance.Entity.MyBase
                                                                            .SpawnDuration*1000;
                                }
                                else if (resourceSpawnInstance.RespawnTime < Environment.TickCount)
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
                    for (int i = 0; i < GlobalEvents.Count; i++)
                    {
                        for (int x = 0; x < GlobalEvents[i].GlobalPageInstance.Length; x++)
                        {
                            GlobalEvents[i].GlobalPageInstance[x].Update();
                        }
                    }
                }
            }
        }
        private bool CheckActive()
        {
            if (PlayersOnMap(MyMapNum))
            {
                return true;
            }
            else
            {
                if (SurroundingMaps.Count > 0)
                {
                    foreach (var t in SurroundingMaps)
                    {
                        if (PlayersOnMap(t))
                        {
                            return true;
                        }
                    }
                }
            }
            Active = false;
            return false;
        }
        private static bool PlayersOnMap(int mapNum)
        {
            if (Globals.Clients.Count <= 0) return false;
            foreach (var t in Globals.Clients)
            {
                if (t == null) continue;
                if (t.EntityIndex <= -1 || t.Entity == null) continue;
                if (((Player)Globals.Entities[t.EntityIndex]) == null) continue;
                if (!((Player)Globals.Entities[t.EntityIndex]).InGame) continue;
                if (Globals.Entities[t.EntityIndex].CurrentMap == mapNum)
                {
                    return true;
                }
            }
            return false;
        }
        public List<int> GetPlayersOnMap()
        {
            List<int> Players = new List<int>();
            if (Globals.Clients.Count <= 0) return Players;
            foreach (var t in Globals.Clients)
            {
                if (t == null) continue;
                if (t.EntityIndex <= -1 || t.Entity == null) continue;
                if (((Player)Globals.Entities[t.EntityIndex]) == null) continue;
                if (!((Player)Globals.Entities[t.EntityIndex]).InGame) continue;
                if (Globals.Entities[t.EntityIndex].CurrentMap == MyMapNum)
                {
                    Players.Add(t.ClientIndex);
                }
            }
            return Players;
        }
        public void PlayerEnteredMap(Client client)
        {
            Active = true;
            //Send Entity Info to Everyone and Everyone to the Entity
            SendMapEntitiesTo(client);
            PacketSender.SendMapItems(client, MyMapNum);
            Entities.Add(client.Entity);
            if (SurroundingMaps.Count <= 0) return;
            foreach (var t in SurroundingMaps)
            {
                MapInstance.GetMap(t).Active = true;
                MapInstance.GetMap(t).SendMapEntitiesTo(client);
                PacketSender.SendMapItems(client, t);
            }
        }
        public void SendMapEntitiesTo(Client client)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] != null && Entities[i] != client.Entity)
                {
                    if (Globals.Entities.IndexOf(Entities[i]) > -1)
                    {
                        if (Entities[i].GetType() == typeof(Player))
                        {
                            PacketSender.SendEntityDataTo(client, Entities[i].MyIndex, (int)EntityTypes.Player, ((Player)Entities[i]).Data(), Entities[i]);
                        }
                        else if (Entities[i].GetType() == typeof(Resource))
                        {
                            PacketSender.SendEntityDataTo(client, Entities[i].MyIndex, (int)EntityTypes.Resource,
                                ((Resource)Entities[i]).Data(), Entities[i]);
                        }
                        else if (Entities[i].GetType() == typeof(Projectile))
                        {
                            PacketSender.SendEntityDataTo(client, Entities[i].MyIndex, (int)EntityTypes.Projectile,
                                ((Projectile)Entities[i]).Data(), Entities[i]);
                        }
                        else
                        {
                            PacketSender.SendEntityDataTo(client, Entities[i].MyIndex, (int)EntityTypes.GlobalEntity,
                                (Entities[i]).Data(), Entities[i]);
                        }
                    }
                }
            }
            ((Player)client.Entity).SendEvents();
        }

        public void ClearConnections(int side = -1)
        {
            if (side == -1 || side == (int)Directions.Up) Up = -1;
            if (side == -1 || side == (int)Directions.Down) Down = -1;
            if (side == -1 || side == (int)Directions.Left) Left = -1;
            if (side == -1 || side == (int)Directions.Right) Right = -1;
            Database.SaveGameObject(this);
        }

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

