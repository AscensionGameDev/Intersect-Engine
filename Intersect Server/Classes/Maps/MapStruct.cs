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
using System.Linq;

namespace Intersect_Server.Classes
{
    public class MapStruct
    {
        //Core
        public string MyName = "New Map";
        public int Up = -1;
        public int Down = -1;
        public int Left = -1;
        public int Right = -1;
        public int MyMapNum;
        public int Deleted;
        public int Revision;

        //Core Data
        public TileArray[] _layers = new TileArray[Constants.LayerCount];
        public Attribute[,] Attributes = new Attribute[Globals.MapWidth, Globals.MapHeight];
        public List<Light> Lights = new List<Light>();
        public List<EventStruct> Events = new List<EventStruct>();

        //Properties
        public string _music = "None";
        public string _sound = "None";
        public List<NpcSpawn> Spawns = new List<NpcSpawn>();
        public bool IsIndoors;

        //Resources
        public List<ResourceSpawn> ResourceSpawns = new List<ResourceSpawn>();

        //Visual Effect Properties
        public string Panorama = "None";
        public string Fog = "None";
        public int FogXSpeed = 0;
        public int FogYSpeed = 0;
        public int FogTransaprency = 0;
        public int RHue = 0;
        public int GHue = 0;
        public int BHue = 0;
        public int AHue = 0;
        public int Brightness = 100;


        //Temporary Values
        public List<int> SurroundingMaps = new List<int>();
        public List<MapItemInstance> MapItems = new List<MapItemInstance>();
        public List<MapItemRespawn> ItemRespawns = new List<MapItemRespawn>();
        public List<Entity> Entities = new List<Entity>();

        //Projectiles
        public List<Projectile> MapProjectiles = new List<Projectile>();

        //Location of Map in the current grid
        public int MapGrid;
        public int MapGridX = -1;
        public int MapGridY = -1;

        //Does the map have a player on or nearby it?
        public bool Active;

        //Data Caching
        public byte[] MapGameData;
        public byte[] MapData;

        //SyncLock
        private Object _mapLock = new Object();

        //Init
        public MapStruct(int mapNum)
        {
            if (mapNum == -1)
            {
                return;
            }
            MyMapNum = mapNum;
            for (var i = 0; i < Constants.LayerCount; i++)
            {
                _layers[i] = new TileArray();
                for (var x = 0; x < Globals.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.MapHeight; y++)
                    {
                        _layers[i].Tiles[x, y] = new Tile();
                        if (i == 0) { Attributes[x, y] = new Attribute(); }
                    }
                }
            }
        }

        //Saving/Loading
        public void Save()
        {
            var bf = new ByteBuffer();
            bf.WriteString(MyName);
            bf.WriteInteger(Up);
            bf.WriteInteger(Down);
            bf.WriteInteger(Left);
            bf.WriteInteger(Right);
            bf.WriteString(_music);
            bf.WriteString(_sound);
            bf.WriteInteger(Convert.ToInt32(IsIndoors));
            bf.WriteString(Panorama);
            bf.WriteString(Fog);
            bf.WriteInteger(FogXSpeed);
            bf.WriteInteger(FogYSpeed);
            bf.WriteInteger(FogTransaprency);
            bf.WriteInteger(RHue);
            bf.WriteInteger(GHue);
            bf.WriteInteger(BHue);
            bf.WriteInteger(AHue);
            bf.WriteInteger(Brightness);

            // Save Map Npcs
            bf.WriteInteger(Spawns.Count);
            for (var i = 0; i < Spawns.Count; i++)
            {
                bf.WriteInteger(Spawns[i].NpcNum);
                bf.WriteInteger(Spawns[i].X);
                bf.WriteInteger(Spawns[i].Y);
                bf.WriteInteger(Spawns[i].Dir);
            }

            for (var i = 0; i < Constants.LayerCount; i++)
            {
                for (var x = 0; x < Globals.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.MapHeight; y++)
                    {
                        bf.WriteInteger(_layers[i].Tiles[x, y].TilesetIndex);
                        bf.WriteInteger(_layers[i].Tiles[x, y].X);
                        bf.WriteInteger(_layers[i].Tiles[x, y].Y);
                        bf.WriteByte(_layers[i].Tiles[x, y].Autotile);
                    }
                }
            }
            for (var x = 0; x < Globals.MapWidth; x++)
            {
                for (var y = 0; y < Globals.MapHeight; y++)
                {
                    bf.WriteInteger(Attributes[x, y].value);
                    bf.WriteInteger(Attributes[x, y].data1);
                    bf.WriteInteger(Attributes[x, y].data2);
                    bf.WriteInteger(Attributes[x, y].data3);
                    bf.WriteString(Attributes[x, y].data4);
                }
            }
            bf.WriteInteger(Lights.Count);
            foreach (var t in Lights)
            {
                bf.WriteBytes(t.LightData());
            }
            bf.WriteInteger(Revision);
            bf.WriteLong(Deleted);
            MapGameData = bf.ToArray();
            bf.WriteInteger(Events.Count);
            foreach (var t in Events)
            {
                bf.WriteBytes(t.EventData());
            }
            Stream stream = File.Create("Resources/Maps/" + MyMapNum + ".map");
            stream.Write(bf.ToArray(), 0, bf.ToArray().Length);
            stream.Close();
            MapData = bf.ToArray();
        }
        public void Load(byte[] packet)
        {
            lock (_mapLock)
            {
                var npcCount = 0;
                NpcSpawn TempNpc = new NpcSpawn();
                var bf = new ByteBuffer();
                bf.WriteBytes(packet);
                MapData = bf.ToArray();
                MyName = bf.ReadString();
                Up = bf.ReadInteger();
                Down = bf.ReadInteger();
                Left = bf.ReadInteger();
                Right = bf.ReadInteger();
                _music = bf.ReadString();
                _sound = bf.ReadString();
                IsIndoors = Convert.ToBoolean(bf.ReadInteger());
                Panorama = bf.ReadString();
                Fog = bf.ReadString();
                FogXSpeed = bf.ReadInteger();
                FogYSpeed = bf.ReadInteger();
                FogTransaprency = bf.ReadInteger();
                RHue = bf.ReadInteger();
                GHue = bf.ReadInteger();
                BHue = bf.ReadInteger();
                AHue = bf.ReadInteger();
                Brightness = bf.ReadInteger();

                //Clear Map Npcs
                for (int i = 0; i < Spawns.Count; i++)
                {
                    if (Spawns[i].Entity != null)
                    {
                        Entities.Remove(Spawns[i].Entity);
                        Spawns[i].Entity.Die();
                    }
                }
                // Load Map Npcs
                Spawns.Clear();
                npcCount = bf.ReadInteger();
                for (var i = 0; i < npcCount; i++)
                {
                    TempNpc = new NpcSpawn();
                    TempNpc.NpcNum = bf.ReadInteger();
                    TempNpc.X = bf.ReadInteger();
                    TempNpc.Y = bf.ReadInteger();
                    TempNpc.Dir = bf.ReadInteger();
                    Spawns.Add(TempNpc);
                }

                for (var i = 0; i < Constants.LayerCount; i++)
                {
                    for (var x = 0; x < Globals.MapWidth; x++)
                    {
                        for (var y = 0; y < Globals.MapHeight; y++)
                        {
                            _layers[i].Tiles[x, y].TilesetIndex = bf.ReadInteger();
                            _layers[i].Tiles[x, y].X = bf.ReadInteger();
                            _layers[i].Tiles[x, y].Y = bf.ReadInteger();
                            _layers[i].Tiles[x, y].Autotile = bf.ReadByte();
                        }
                    }
                }
                for (var x = 0; x < Globals.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.MapHeight; y++)
                    {
                        Attributes[x, y].value = bf.ReadInteger();
                        Attributes[x, y].data1 = bf.ReadInteger();
                        Attributes[x, y].data2 = bf.ReadInteger();
                        Attributes[x, y].data3 = bf.ReadInteger();
                        Attributes[x, y].data4 = bf.ReadString();
                    }
                }
                var lCount = bf.ReadInteger();
                Lights.Clear();
                for (var i = 0; i < lCount; i++)
                {
                    Lights.Add(new Light(bf));
                }
                Revision = bf.ReadInteger();
                Deleted = (int) bf.ReadLong();
                MapGameData = packet.Skip(0).Take(bf.Pos()).ToArray();
                Events.Clear();
                var eCount = bf.ReadInteger();
                for (var i = 0; i < eCount; i++)
                {
                    Events.Add(new EventStruct(i,bf));
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
                    if (ResourceSpawns[i].Entity != null)
                    {
                        Entities.Remove(ResourceSpawns[i].Entity);
                        ResourceSpawns[i].Entity.Die();
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
                SpawnMapNpcs();
                SpawnMapResources();
            }
        }

        //Items & Resources
        private void SpawnAttributeItems()
        {
            ResourceSpawns.Clear();
            for (int x = 0; x < Globals.MapWidth; x++)
            {
                for (int y = 0; y < Globals.MapHeight; y++)
                {
                    if (Attributes[x, y].value == (int)Enums.MapAttributes.Item)
                    {
                        SpawnAttributeItem(x, y);
                    }
                    else if (Attributes[x, y].value == (int)Enums.MapAttributes.Resource)
                    {
                        SpawnAttributeResource(x, y);
                    }
                }
            }
        }
        public void SpawnItem(int x, int y, ItemInstance item, int amount)
        {
            MapItems.Add(new MapItemInstance());
            MapItems[MapItems.Count - 1].X = x;
            MapItems[MapItems.Count - 1].Y = y;
            MapItems[MapItems.Count - 1].ItemNum = item.ItemNum;
            MapItems[MapItems.Count - 1].DespawnTime = Environment.TickCount + Constants.ItemDespawnTime;
            if (Globals.GameItems[MapItems[MapItems.Count - 1].ItemNum].Type == (int)Enums.ItemTypes.Equipment)
            {
                MapItems[MapItems.Count - 1].ItemVal = 1;
                for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
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
            MapItems.Add(new MapItemInstance());
            MapItems[MapItems.Count - 1].X = x;
            MapItems[MapItems.Count - 1].Y = y;
            MapItems[MapItems.Count - 1].ItemNum = Attributes[x, y].data1;
            MapItems[MapItems.Count - 1].DespawnTime = -1;
            MapItems[MapItems.Count - 1].AttributeSpawnX = x;
            MapItems[MapItems.Count - 1].AttributeSpawnY = y;
            if (Globals.GameItems[MapItems[MapItems.Count - 1].ItemNum].Type == (int)Enums.ItemTypes.Equipment)
            {
                MapItems[MapItems.Count - 1].ItemVal = 1;
                Random r = new Random();
                for (int i = 0; i < (int)Enums.Stats.StatCount; i++)
                {
                    MapItems[MapItems.Count - 1].StatBoost[i] = r.Next(-1 * Globals.GameItems[MapItems[MapItems.Count - 1].ItemNum].StatGrowth, Globals.GameItems[MapItems[MapItems.Count - 1].ItemNum].StatGrowth + 1);
                }
            }
            else
            {
                MapItems[MapItems.Count - 1].ItemVal = Attributes[x, y].data2;
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
                ItemRespawns[ItemRespawns.Count - 1].RespawnTime = Environment.TickCount + Constants.ItemRespawnTime;
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
            int Z = 0;
            if (
                Spawns[i].NpcNum < 0 || Globals.GameNpcs[Spawns[i].NpcNum].Sprite == "None" ||
                Globals.GameNpcs[Spawns[i].NpcNum].Name == "") return;
            int index = Globals.FindOpenEntity();
            Globals.Entities[index] = new Npc(index, Globals.GameNpcs[Spawns[i].NpcNum]);
            Spawns[i].Entity = Globals.Entities[index];
            if (Spawns[i].X >= 0 && Spawns[i].Y >= 0)
            {
                ((Npc)Globals.Entities[index]).CurrentX = Spawns[i].X;
                ((Npc)Globals.Entities[index]).CurrentY = Spawns[i].Y;
            }
            else
            {
                for (int n = 0; n < 100; n++)
                {
                    X = Globals.Rand.Next(1, Globals.MapWidth);
                    Y = Globals.Rand.Next(1, Globals.MapHeight);
                    if (Attributes[X, Y].value == (int)Enums.MapAttributes.Walkable)
                    {
                        break;
                    }
                    X = 0;
                    Y = 0;
                }
                ((Npc)Globals.Entities[index]).CurrentX = X;
                ((Npc)Globals.Entities[index]).CurrentY = Y;
            }
            if (Spawns[i].Dir >= 0)
            {
                ((Npc)Globals.Entities[index]).Dir = Spawns[i].Dir;
            }
            else
            {
                ((Npc)Globals.Entities[index]).Dir = Globals.Rand.Next(0, 4);
            }
            ((Npc)Globals.Entities[index]).CurrentMap = MyMapNum;

            //Give NPC Drops
            for (int n = 0; n < Constants.MaxNpcDrops; n++)
            {
                if (Globals.Rand.Next(1, 101) <= Globals.GameNpcs[Spawns[i].NpcNum].Drops[n].Chance)
                {
                    Globals.Entities[index].Inventory[Z].ItemNum = Globals.GameNpcs[Spawns[i].NpcNum].Drops[n].ItemNum;
                    Globals.Entities[index].Inventory[Z].ItemVal = Globals.GameNpcs[Spawns[i].NpcNum].Drops[n].Amount;
                    Z = Z + 1;
                }
            }

            Entities.Add((Npc)Globals.Entities[index]);
            PacketSender.SendEntityDataToProximity(index, (int)Enums.EntityTypes.GlobalEntity, Globals.Entities[index].Data(), Globals.Entities[index]);
        }

        //Resources
        //Npcs
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
            int index = Globals.FindOpenEntity();
            Globals.Entities[index] = new Resource(index, ResourceSpawns[i].ResourceNum);
            ResourceSpawns[i].Entity = Globals.Entities[index];
            Globals.Entities[index].CurrentX = ResourceSpawns[i].X;
            Globals.Entities[index].CurrentY = ResourceSpawns[i].Y;
            Globals.Entities[index].CurrentMap = MyMapNum;

            //Give Resource Drops
            for (int n = 0; n < Constants.MaxNpcDrops; n++)
            {
                if (Globals.Rand.Next(1, 101) <= Globals.GameResources[Attributes[x, y].data1].Drops[n].Chance)
                {
                    Globals.Entities[index].Inventory[Z].ItemNum = Globals.GameResources[Attributes[x, y].data1].Drops[n].ItemNum;
                    Globals.Entities[index].Inventory[Z].ItemVal = Globals.GameResources[Attributes[x, y].data1].Drops[n].Amount;
                    Z = Z + 1;
                }
            }

            Entities.Add((Resource)Globals.Entities[index]);
            PacketSender.SendEntityDataToProximity(index, (int)Enums.EntityTypes.Resource, Globals.Entities[index].Data(), Globals.Entities[index]);
        }

        //Spawn a projectile
        public void SpawnMapProjectile(int MyIndex, Type ownerType, int projectileNum, int Map, int X, int Y, int Z, int Direction, int Target = 0)
        {
            int n = Globals.FindOpenEntity();
            MapProjectiles.Add(new Projectile(n, MyIndex, this.GetType(), projectileNum, Map, X, Y, Z, Direction, Target));
            Globals.Entities[n] = MapProjectiles[MapProjectiles.Count - 1];

            Entities.Add(Globals.Entities[n]);
            PacketSender.SendEntityDataToProximity(n, (int)Enums.EntityTypes.Projectile, ((Projectile)Globals.Entities[n]).Data(), Globals.Entities[n]);
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
                            //Cast timers
                            if (Entities[i].CastTime != 0 && Entities[i].CastTime < Environment.TickCount)
                            {
                                Entities[i].CastSpell(Entities[i].Spells[Entities[i].SpellCastSlot].SpellNum, Entities[i].SpellCastSlot);
                                Entities[i].CastTime = 0;
                            }

                            //Active Npcs On the Map
                            if (Entities[i].GetType() == typeof (Npc))
                            {
                                ((Npc) Entities[i]).Update();
                            }
                        }
                    }
                    //Process NPC Respawns
                    for (int i = 0; i < Spawns.Count; i++)
                    {
                        if (!Globals.Entities.Contains(Spawns[i].Entity))
                        {
                            if (Spawns[i].RespawnTime == -1)
                            {
                                Spawns[i].RespawnTime = Environment.TickCount +
                                                        Globals.GameNpcs[Spawns[i].NpcNum].SpawnDuration*1000;
                            }
                            else if (Spawns[i].RespawnTime < Environment.TickCount)
                            {
                                SpawnMapNpc(i);
                                Spawns[i].RespawnTime = -1;
                            }
                        }
                    }
                    //Process Resource Respawns
                    for (int i = 0; i < ResourceSpawns.Count; i++)
                    {
                        if (!Globals.Entities.Contains(ResourceSpawns[i].Entity))
                        {
                            if (ResourceSpawns[i].RespawnTime == -1)
                            {
                                ResourceSpawns[i].RespawnTime = Environment.TickCount +
                                                                Globals.GameResources[ResourceSpawns[i].ResourceNum]
                                                                    .SpawnDuration*1000;
                            }
                            else if (ResourceSpawns[i].RespawnTime < Environment.TickCount)
                            {
                                SpawnMapResource(i);
                                ResourceSpawns[i].RespawnTime = -1;
                            }
                        }
                    }
                    //Process all of the projectiles
                    for (int i = 0; i < MapProjectiles.Count; i++)
                    {
                        MapProjectiles[i].Update();
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
            PacketSender.SendMapItems(client,MyMapNum);
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
                            PacketSender.SendEntityDataTo(client, Entities[i].MyIndex, (int)Enums.EntityTypes.Player, ((Player)Entities[i]).Data(), Entities[i]);
                        }
                        else if (Entities[i].GetType() == typeof(Resource))
                        {
                            PacketSender.SendEntityDataTo(client, Entities[i].MyIndex, (int)Enums.EntityTypes.Resource,
                                ((Resource)Entities[i]).Data(), Entities[i]);
                        }
                        else if (Entities[i].GetType() == typeof(Projectile))
                        {
                            PacketSender.SendEntityDataTo(client, Entities[i].MyIndex, (int)Enums.EntityTypes.Projectile,
                                ((Projectile)Entities[i]).Data(), Entities[i]);
                        }
                        else
                        {
                            PacketSender.SendEntityDataTo(client, Entities[i].MyIndex, (int)Enums.EntityTypes.GlobalEntity,
                                (Entities[i]).Data(), Entities[i]);
                        }
                    }
                }
            }
        }

    }

    #region "Extra Classes - Just for maps"
    public class Attribute
    {
        public int value;
        public int data1;
        public int data2;
        public int data3;
        public string data4 = "";
    }

    public class TileArray
    {
        public Tile[,] Tiles = new Tile[Globals.MapWidth, Globals.MapHeight];
    }

    public class Tile
    {
        public int TilesetIndex = -1;
        public int X;
        public int Y;
        public byte Autotile;
    }

    public class Light
    {
        public int OffsetX;
        public int OffsetY;
        public int TileX;
        public int TileY;
        public double Intensity;
        public int Range;
        public Light(ByteBuffer myBuffer)
        {
            OffsetX = myBuffer.ReadInteger();
            OffsetY = myBuffer.ReadInteger();
            TileX = myBuffer.ReadInteger();
            TileY = myBuffer.ReadInteger();
            Intensity = myBuffer.ReadDouble();
            Range = myBuffer.ReadInteger();
        }
        public byte[] LightData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(OffsetX);
            myBuffer.WriteInteger(OffsetY);
            myBuffer.WriteInteger(TileX);
            myBuffer.WriteInteger(TileY);
            myBuffer.WriteDouble(Intensity);
            myBuffer.WriteInteger(Range);
            return myBuffer.ToArray();
        }
    }

    public class NpcSpawn
    {
        public int NpcNum;
        public int X;
        public int Y;
        public int Dir;

        //Temporary Values
        //Npc Index
        public Entity Entity;
        public long RespawnTime = -1;
    }

    public class ResourceSpawn
    {
        public int ResourceNum;
        public int X;
        public int Y;

        //Temporary Values
        //Resource Index
        public int EntityIndex = -1;
        public Entity Entity;
        public long RespawnTime = -1;
    }

    #endregion
}


