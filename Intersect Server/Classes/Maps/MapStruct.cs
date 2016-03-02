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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Intersect_Server.Classes.Entities;

namespace Intersect_Server.Classes
{
    public class MapStruct
    {
        //Core
        public const string Version = "0.0.0.1";

        public string MyName = "New Map";
        public int Up = -1;
        public int Down = -1;
        public int Left = -1;
        public int Right = -1;
        public int MyMapNum;
        public int Deleted;
        public int Revision;

        //Core Data
        private byte[] tileData;
        public Attribute[,] Attributes = new Attribute[Globals.MapWidth, Globals.MapHeight];
        public List<Light> Lights = new List<Light>();
        public List<EventStruct> Events = new List<EventStruct>();
        public List<NpcSpawn> Spawns = new List<NpcSpawn>();
        public List<ResourceSpawn> ResourceSpawns = new List<ResourceSpawn>();

        //Properties
        public string _music = "None";
        public string _sound = "None";
        public bool IsIndoors;
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
        public byte ZoneType = 0; //Everything goes, 1 is safe, add more later
        public int PlayerLightSize = 300;
        public byte PlayerLightIntensity = 255;
        public float PlayerLightExpand = 0f;
        public Color PlayerLightColor = Color.White;
        public string OverlayGraphic = "None";


        //Temporary Values
        public List<int> SurroundingMaps = new List<int>();
        public List<MapItemInstance> MapItems = new List<MapItemInstance>();
        public List<MapItemRespawn> ItemRespawns = new List<MapItemRespawn>();
        public List<Entity> Entities = new List<Entity>();
        public List<EventInstance> GlobalEvents = new List<EventInstance>(); 

        //Caching Values
        //In order to keep the memory footprint down, if a map hasn't been requested for over 20 seconds then we will drop the data to be sent.
        private byte[] _clientMapData = null;
        private byte[] _editorMapData = null;
        private long _lastAccessTime = 0;

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
        public MapStruct(int mapNum)
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
            byte[] MapGameData = null;
            var bf = new ByteBuffer();
            var tileBuffer = new ByteBuffer();
            bf.WriteInteger(Deleted);
            if (Deleted != 1)
            {
                bf.WriteString(Version);
                bf.WriteString(MyName);
                bf.WriteInteger(Revision);
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
                bf.WriteByte(ZoneType);
                bf.WriteString(OverlayGraphic);
                bf.WriteInteger(PlayerLightSize);
                bf.WriteDouble(PlayerLightExpand);
                bf.WriteByte(PlayerLightIntensity);
                bf.WriteByte(PlayerLightColor.R);
                bf.WriteByte(PlayerLightColor.G);
                bf.WriteByte(PlayerLightColor.B);

                if (tileData == null)
                {
                    if (newMap || !File.Exists("Resources/Maps/" + MyMapNum + ".tiles"))
                    {
                        //New map. We need to generate the tile data.
                        //We zero everything out
                        ByteBuffer tmpBuffer = new ByteBuffer();
                        Tile fakeTile = new Tile();
                        for (int i = 0; i < Constants.LayerCount; i++)
                        {
                            for (int x = 0; x < Globals.MapWidth; x++)
                            {
                                for (int y = 0; y < Globals.MapHeight; y++)
                                {
                                    tileBuffer.WriteInteger(fakeTile.TilesetIndex);
                                    tileBuffer.WriteInteger(fakeTile.X);
                                    tileBuffer.WriteInteger(fakeTile.Y);
                                    tileBuffer.WriteByte(fakeTile.Autotile);
                                }
                            }
                        }
                        tileData = tileBuffer.ToArray();
                        tileBuffer.Dispose();
                    }
                    else
                    {
                        tileData = File.ReadAllBytes("Resources/Maps/" + MyMapNum + ".tiles");
                    }
                }
                File.WriteAllBytes("Resources/Maps/" + MyMapNum + ".tiles", tileData);
                bf.WriteBytes(tileData);

                for (var x = 0; x < Globals.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.MapHeight; y++)
                    {
                        if (Attributes[x, y] != null && Attributes[x, y].value > 0)
                        {
                            bf.WriteInteger(Attributes[x, y].value);
                            bf.WriteInteger(Attributes[x, y].data1);
                            bf.WriteInteger(Attributes[x, y].data2);
                            bf.WriteInteger(Attributes[x, y].data3);
                            bf.WriteString(Attributes[x, y].data4);
                        }
                        else
                        {
                            bf.WriteInteger(0);
                        }
                    }
                }
                bf.WriteInteger(Lights.Count);
                foreach (var t in Lights)
                {
                    bf.WriteBytes(t.LightData());
                }
                MapGameData = bf.ToArray();

                // Save Map Npcs
                bf.WriteInteger(Spawns.Count);
                for (var i = 0; i < Spawns.Count; i++)
                {
                    bf.WriteInteger(Spawns[i].NpcNum);
                    bf.WriteInteger(Spawns[i].X);
                    bf.WriteInteger(Spawns[i].Y);
                    bf.WriteInteger(Spawns[i].Dir);
                }

                bf.WriteInteger(Events.Count);
                foreach (var t in Events)
                {
                    bf.WriteBytes(t.EventData());
                }
            }
            _lastAccessTime = Environment.TickCount;
            if (MapGameData != null)
            {
                _clientMapData = MapGameData;
                File.WriteAllBytes("Resources/Maps/" + MyMapNum + ".cmap", _clientMapData);
            }
            _editorMapData = bf.ToArray();
            File.WriteAllBytes("Resources/Maps/" + MyMapNum + ".map", _editorMapData);
            bf.Dispose();
        }
        public bool Load(byte[] packet, int keepRevision = -1)
        {
            lock (_mapLock)
            {
                var npcCount = 0;
                NpcSpawn TempNpc = new NpcSpawn();
                var bf = new ByteBuffer();
                bf.WriteBytes(packet);
                Deleted = bf.ReadInteger();
                if (Deleted == 1) return false;

                string loadedVersion = bf.ReadString();
                if (loadedVersion != Version)
                    throw new Exception("Failed to load Map #" + MyMapNum + ". Loaded Version: " + loadedVersion + " Expected Version: " + Version);
                MyName = bf.ReadString();
                Revision = bf.ReadInteger();
                if (keepRevision > -1) Revision = keepRevision;
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
                ZoneType = bf.ReadByte();
                OverlayGraphic = bf.ReadString();
                PlayerLightSize = bf.ReadInteger();
                PlayerLightExpand = (float)bf.ReadDouble();
                PlayerLightIntensity = bf.ReadByte();
                PlayerLightColor = Color.FromArgb(bf.ReadByte(), bf.ReadByte(), bf.ReadByte());

                //Server Doesn't care about visuals.. just read the tile chunk into a byte array
                //We read the TilesetIndex (int), X (int), Y (int) and Autotile (byte) for everyt tile of every later.
                //Meaning we need to read (Layers * Width * Height) * ( 4 (int) + 4 (int) + 4 (int) + 1 (byte)) bytes.
                tileData = bf.ReadBytes(Constants.LayerCount * Globals.MapWidth * Globals.MapHeight * 13);

                for (var x = 0; x < Globals.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.MapHeight; y++)
                    {
                        int attributeType = bf.ReadInteger();
                        if (attributeType > 0)
                        {
                            Attributes[x, y] = new Attribute();
                            Attributes[x, y].value = attributeType;
                            Attributes[x, y].data1 = bf.ReadInteger();
                            Attributes[x, y].data2 = bf.ReadInteger();
                            Attributes[x, y].data3 = bf.ReadInteger();
                            Attributes[x, y].data4 = bf.ReadString();
                        }
                        else
                        {
                            Attributes[x, y] = null;
                        }
                    }
                }
                var lCount = bf.ReadInteger();
                Lights.Clear();
                for (var i = 0; i < lCount; i++)
                {
                    Lights.Add(new Light(bf));
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

                Events.Clear();
                var eCount = bf.ReadInteger();
                for (var i = 0; i < eCount; i++)
                {
                    Events.Add(new EventStruct(i, bf));
                }


                //Clear Map Npcs
                for (int i = 0; i < Spawns.Count; i++)
                {
                    if (Spawns[i].Entity != null)
                    {
                        Entities.Remove(Spawns[i].Entity);
                        Spawns[i].Entity.Die();
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
                SpawnGlobalEvents();
                SpawnMapNpcs();
                SpawnMapResources();
                Save();
                tileData = null;
                return true;
            }
        }

        //Get Map Data
        public byte[] GetClientMapData()
        {
            if (_clientMapData == null)
            {
                _clientMapData = File.ReadAllBytes("Resources/Maps/" + MyMapNum + ".cmap");
            }
            _lastAccessTime = Environment.TickCount;
            return _clientMapData;
        }

        public byte[] GetEditorMapData()
        {
            if (_editorMapData == null)
            {
                _editorMapData = File.ReadAllBytes("Resources/Maps/" + MyMapNum + ".map");
            }
            _lastAccessTime = Environment.TickCount;
            return _editorMapData;
        }

        //Items & Resources
        private void SpawnAttributeItems()
        {
            ResourceSpawns.Clear();
            for (int x = 0; x < Globals.MapWidth; x++)
            {
                for (int y = 0; y < Globals.MapHeight; y++)
                {
                    if (Attributes[x, y] != null)
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
        }
        public void SpawnItem(int x, int y, ItemInstance item, int amount)
        {
            MapItems.Add(new MapItemInstance(item.ItemNum,item.ItemVal));
            MapItems[MapItems.Count - 1].X = x;
            MapItems[MapItems.Count - 1].Y = y;
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
            MapItems.Add(new MapItemInstance(Attributes[x, y].data1, Attributes[x, y].data2));
            MapItems[MapItems.Count - 1].X = x;
            MapItems[MapItems.Count - 1].Y = y;
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

        //Events
        private void SpawnGlobalEvents()
        {
            for (int i = 0; i < Events.Count; i++)
            {
                if (Events[i].IsGlobal == 1)
                {
                    Events[i].GlobalInstance = new EventInstance(Events[i], i, MyMapNum);
                    GlobalEvents.Add(Events[i].GlobalInstance);
                }
            }
        }
        private void DespawnGlobalEvents()
        {
            
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
                if (_clientMapData != null || _editorMapData != null)
                {
                    if (_lastAccessTime + 20000 < Environment.TickCount)
                    {
                        _clientMapData = null;
                        _editorMapData = null;
                    }
                }
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
                            if (Entities[i].GetType() == typeof(Npc))
                            {
                                ((Npc)Entities[i]).Update();
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
                                                        Globals.GameNpcs[Spawns[i].NpcNum].SpawnDuration * 1000;
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
                                                                    .SpawnDuration * 1000;
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
                    //Process all global events
                    for (int i = 0; i < GlobalEvents.Count; i++)
                    {
                        for (int x = 0; x < GlobalEvents[i].GlobalPageInstance.Length; x++)
                        {
                            GlobalEvents[i].GlobalPageInstance[x].Update(null);
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
            ((Player) client.Entity).SendEvents();
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

    class TileArray
    {
        public Tile[,] Tiles = new Tile[Globals.MapWidth, Globals.MapHeight];
    }

    class Tile
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
        public byte Intensity = 255;
        public int Size = 0;
        public float Expand = 0f;
        public System.Drawing.Color Color = System.Drawing.Color.White;

        public Light()
        {
            TileX = -1;
            TileY = -1;
        }
        public Light(int x, int y)
        {
            TileX = x;
            TileY = y;
        }

        public Light(Light copy)
        {
            TileX = copy.TileX;
            TileY = copy.TileY;
            OffsetX = copy.OffsetX;
            OffsetY = copy.OffsetY;
            Intensity = copy.Intensity;
            Size = copy.Size;
            Expand = copy.Expand;
            Color = System.Drawing.Color.FromArgb(copy.Color.R, copy.Color.G, copy.Color.B);
        }
        public Light(ByteBuffer myBuffer)
        {
            OffsetX = myBuffer.ReadInteger();
            OffsetY = myBuffer.ReadInteger();
            TileX = myBuffer.ReadInteger();
            TileY = myBuffer.ReadInteger();
            Intensity = myBuffer.ReadByte();
            Size = myBuffer.ReadInteger();
            Expand = (float)myBuffer.ReadDouble();
            Color = System.Drawing.Color.FromArgb(myBuffer.ReadByte(), myBuffer.ReadByte(), myBuffer.ReadByte());

        }
        public byte[] LightData()
        {
            var myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(OffsetX);
            myBuffer.WriteInteger(OffsetY);
            myBuffer.WriteInteger(TileX);
            myBuffer.WriteInteger(TileY);
            myBuffer.WriteByte(Intensity);
            myBuffer.WriteInteger(Size);
            myBuffer.WriteDouble(Expand);
            myBuffer.WriteByte(Color.R);
            myBuffer.WriteByte(Color.G);
            myBuffer.WriteByte(Color.B);
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


