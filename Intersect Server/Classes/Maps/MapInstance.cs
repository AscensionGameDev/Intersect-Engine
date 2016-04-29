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
using System.IO;
using Intersect_Library;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps;
using Intersect_Server.Classes.Entities;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Items;
using Intersect_Server.Classes.Networking;


namespace Intersect_Server.Classes.Maps
{
    public class MapInstance : Intersect_Library.GameObjects.Maps.MapStruct
    {
        //Core
        public const string Version = "0.0.0.1";

        //Temporary Values
        public List<int> SurroundingMaps = new List<int>();
        public List<MapItemInstance> MapItems = new List<MapItemInstance>();
        public List<MapItemRespawn> ItemRespawns = new List<MapItemRespawn>();
        public List<Entity> Entities = new List<Entity>();
        public List<EventInstance> GlobalEvents = new List<EventInstance>();
        public Dictionary<EventStruct, EventInstance> GlobalEventInstances = new Dictionary<EventStruct, EventInstance>(); 
        public Dictionary<NpcSpawn,MapNpcSpawnInstance> NpcSpawnInstances = new Dictionary<NpcSpawn, MapNpcSpawnInstance>();
        public Dictionary<ResourceSpawn, MapResourceSpawnInstance> ResourceSpawnInstances = new Dictionary<ResourceSpawn, MapResourceSpawnInstance>();

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
        public MapInstance(int mapNum) : base(mapNum,false)
        {
            if (mapNum == -1)
            {
                return;
            }
            MyMapNum = mapNum;
        }

        //Saving/Loading
        public void Save(bool newMap = false)
        {
            File.WriteAllBytes("resources/maps/" + MyMapNum + ".map", base.GetMapData(false));
        }

        public override bool Load(byte[] packet)
        {
            return Load(packet,-1);
        }

        public bool Load(byte[] packet, int keepRevision = -1)
        {
            lock (_mapLock)
            {
                var result = base.Load(packet);
                if (keepRevision > -1) Revision = keepRevision;
                if (result)
                {
                    //Clear Map Npcs
                    for (int i = 0; i < Spawns.Count; i++)
                    {
                        if (NpcSpawnInstances.ContainsKey(Spawns[i]))
                        {
                            MapNpcSpawnInstance npcSpawnInstance = NpcSpawnInstances[Spawns[i]];
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
                            MapResourceSpawnInstance resourceSpawnInstance = ResourceSpawnInstances[ResourceSpawns[i]];
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
                    SpawnAttributeItems();
                    SpawnGlobalEvents();
                    SpawnMapNpcs();
                    SpawnMapResources();
                    Save();
                }
                return result;
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
            MapItems.Add(new MapItemInstance(item.ItemNum, item.ItemVal));
            MapItems[MapItems.Count - 1].X = x;
            MapItems[MapItems.Count - 1].Y = y;
            MapItems[MapItems.Count - 1].DespawnTime = Environment.TickCount + Options.ItemDespawnTime;
            if (Globals.GameItems[MapItems[MapItems.Count - 1].ItemNum].Type == (int)ItemTypes.Equipment)
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
        private void SpawnAttributeItem(int x, int y)
        {
            MapItems.Add(new MapItemInstance(Attributes[x, y].data1, Attributes[x, y].data2));
            MapItems[MapItems.Count - 1].X = x;
            MapItems[MapItems.Count - 1].Y = y;
            MapItems[MapItems.Count - 1].DespawnTime = -1;
            MapItems[MapItems.Count - 1].AttributeSpawnX = x;
            MapItems[MapItems.Count - 1].AttributeSpawnY = y;
            if (Globals.GameItems[MapItems[MapItems.Count - 1].ItemNum].Type == (int)ItemTypes.Equipment)
            {
                MapItems[MapItems.Count - 1].ItemVal = 1;
                Random r = new Random();
                for (int i = 0; i < (int)Stats.StatCount; i++)
                {
                    MapItems[MapItems.Count - 1].StatBoost[i] = r.Next(-1 * Globals.GameItems[MapItems[MapItems.Count - 1].ItemNum].StatGrowth, Globals.GameItems[MapItems[MapItems.Count - 1].ItemNum].StatGrowth + 1);
                }
            }
            PacketSender.SendMapItemUpdate(MyMapNum, MapItems.Count - 1);
        }
        public void RemoveItem(int index)
        {
            MapItems[index].ItemNum = -1;
            PacketSender.SendMapItemUpdate(MyMapNum, index);
            if (MapItems[index].AttributeSpawnX > -1)
            {
                ItemRespawns.Add(new MapItemRespawn());
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
            int Z = 0;
            int index = -1;
            MapResourceSpawnInstance resourceSpawnInstance;
            if (ResourceSpawnInstances.ContainsKey(ResourceSpawns[i]))
            {
                resourceSpawnInstance = ResourceSpawnInstances[ResourceSpawns[i]];
            }
            else
            {
                resourceSpawnInstance = new MapResourceSpawnInstance();
                ResourceSpawnInstances.Add(ResourceSpawns[i],resourceSpawnInstance);
            }
            if (resourceSpawnInstance.Entity == null)
            {
                index = Globals.FindOpenEntity();
                Globals.Entities[index] = new Resource(index, ResourceSpawns[i].ResourceNum);
                resourceSpawnInstance.Entity = (Resource)Globals.Entities[index];
                Globals.Entities[index].CurrentX = ResourceSpawns[i].X;
                Globals.Entities[index].CurrentY = ResourceSpawns[i].Y;
                Globals.Entities[index].CurrentMap = MyMapNum;
                Entities.Add((Resource)Globals.Entities[index]);
            }
            else
            {
                index = resourceSpawnInstance.Entity.MyIndex;
            }

            ((Resource)Globals.Entities[index]).Spawn();
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
            if (Spawns[i].NpcNum < 0 || Globals.GameNpcs[Spawns[i].NpcNum].Sprite == "None" ||
                Globals.GameNpcs[Spawns[i].NpcNum].Name == "") return;
            MapNpcSpawnInstance npcSpawnInstance;
            if (NpcSpawnInstances.ContainsKey(Spawns[i]))
            {
                npcSpawnInstance = NpcSpawnInstances[Spawns[i]];
            }
            else
            {
                npcSpawnInstance = new MapNpcSpawnInstance();
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

        public Entity SpawnNpc(int tileX, int tileY, int dir, int npcNum)
        {
            int index = Globals.FindOpenEntity();
            int Z = 0;
            Globals.Entities[index] = new Npc(index, Globals.GameNpcs[npcNum]);
            Globals.Entities[index].CurrentMap = MyMapNum;
            Globals.Entities[index].CurrentX = tileX;
            Globals.Entities[index].CurrentY = tileY;

            //Give NPC Drops
            for (int n = 0; n < Options.MaxNpcDrops; n++)
            {
                if (Globals.Rand.Next(1, 101) <= Globals.GameNpcs[npcNum].Drops[n].Chance)
                {
                    Globals.Entities[index].Inventory[Z].ItemNum = Globals.GameNpcs[npcNum].Drops[n].ItemNum;
                    Globals.Entities[index].Inventory[Z].ItemVal = Globals.GameNpcs[npcNum].Drops[n].Amount;
                    Z = Z + 1;
                }
            }

            Entities.Add((Npc)Globals.Entities[index]);
            PacketSender.SendEntityDataToProximity(index, (int)EntityTypes.GlobalEntity, Globals.Entities[index].Data(), Globals.Entities[index]);
            return (Npc)Globals.Entities[index];
        }

        //Events
        private void SpawnGlobalEvents()
        {
            GlobalEventInstances.Clear();
            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].IsGlobal == 1)
                {
                    GlobalEventInstances.Add(Events[i], new EventInstance(Events[i], i, MyMapNum));
                }
            }
        }
        private void DespawnGlobalEvents()
        {

        }
        public EventInstance GetGlobalEventInstance(EventStruct baseEvent)
        {
            if (GlobalEventInstances.ContainsKey(baseEvent))
            {
                return GlobalEventInstances[baseEvent];
            }
            return null;
        }


        //Spawn a projectile
        public void SpawnMapProjectile(Entity owner, int projectileNum, int Map, int X, int Y, int Z, int Direction, int IsSpell = -1, int Target = 0)
        {
            int n = Globals.FindOpenEntity();
            MapProjectiles.Add(new Projectile(n, owner, projectileNum, Map, X, Y, Z, Direction, IsSpell, Target));
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
                        MapNpcSpawnInstance npcSpawnInstance = NpcSpawnInstances[Spawns[i]];
                        if (!Globals.Entities.Contains(npcSpawnInstance.Entity))
                        {
                            if (npcSpawnInstance.RespawnTime == -1)
                            {
                                npcSpawnInstance.RespawnTime = Environment.TickCount +
                                                        Globals.GameNpcs[Spawns[i].NpcNum].SpawnDuration * 1000;
                            }
                            else if (npcSpawnInstance.RespawnTime < Environment.TickCount)
                            {
                                SpawnMapNpc(i);
                                npcSpawnInstance.RespawnTime = -1;
                            }
                        }
                    }
                    //Process Resource Respawns
                    for (int i = 0; i < ResourceSpawns.Count; i++)
                    {
                        MapResourceSpawnInstance resourceSpawnInstance = ResourceSpawnInstances[ResourceSpawns[i]];
                        if (resourceSpawnInstance.Entity != null && resourceSpawnInstance.Entity.IsDead)
                        {
                            if (resourceSpawnInstance.RespawnTime == -1)
                            {
                                resourceSpawnInstance.RespawnTime = Environment.TickCount +
                                                                Globals.GameResources[ResourceSpawns[i].ResourceNum]
                                                                    .SpawnDuration * 1000;
                            }
                            else if (resourceSpawnInstance.RespawnTime < Environment.TickCount)
                            {
                                SpawnMapResource(i);
                                resourceSpawnInstance.RespawnTime = -1;
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
                Globals.GameMaps[t].Active = true;
                Globals.GameMaps[t].SendMapEntitiesTo(client);
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
            if (Deleted == 0)
            {
                if (side == -1 || side == (int)Directions.Up) Up = -1;
                if (side == -1 || side == (int)Directions.Down) Down = -1;
                if (side == -1 || side == (int)Directions.Left) Left = -1;
                if (side == -1 || side == (int)Directions.Right) Right = -1;
                Save();
            }
        }
    }
}

