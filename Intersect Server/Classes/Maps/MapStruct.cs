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
        private readonly TileArray[] _layers = new TileArray[Constants.LayerCount];
        public Attribute[,] Attributes = new Attribute[Constants.MapWidth, Constants.MapHeight];
        public List<Light> Lights = new List<Light>();
        public List<EventStruct> Events = new List<EventStruct>();

        //Properties
        private string _music = "None";
        private string _sound = "None";
        public List<NpcSpawn> Spawns = new List<NpcSpawn>();
        public bool IsIndoors;

        //Temporary Values
        public List<int> SurroundingMaps = new List<int>();
        public List<MapItemInstance> MapItems = new List<MapItemInstance>();
        public List<MapItemRespawn> ItemRespawns = new List<MapItemRespawn>();
        public List<Npc> Npcs = new List<Npc>();

        //Location of Map in the current grid
        public int MapGrid;
        public int MapGridX;
        public int MapGridY;

        //Does the map have a player on or nearby it?
        public bool Active;

        //Data Caching
        public byte[] MapGameData;
        public byte[] MapData;
        
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
                for (var x = 0; x < Constants.MapWidth; x++)
                {
                    for (var y = 0; y < Constants.MapHeight; y++)
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
            for (var i = 0; i < Constants.LayerCount; i++)
            {
                for (var x = 0; x < Constants.MapWidth; x++)
                {
                    for (var y = 0; y < Constants.MapHeight; y++)
                    {
                        bf.WriteInteger(_layers[i].Tiles[x, y].TilesetIndex);
                        bf.WriteInteger(_layers[i].Tiles[x, y].X);
                        bf.WriteInteger(_layers[i].Tiles[x, y].Y);
                        bf.WriteByte(_layers[i].Tiles[x, y].Autotile);
                    }
                }
            }
            for (var x = 0; x < Constants.MapWidth; x++)
            {
                for (var y = 0; y < Constants.MapHeight; y++)
                {
                    bf.WriteInteger(Attributes[x, y].value);
                    bf.WriteInteger(Attributes[x, y].data1);
                    bf.WriteInteger(Attributes[x, y].data2);
                    bf.WriteInteger(Attributes[x, y].data3);
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
            for (var i = 0; i < Constants.LayerCount; i++)
            {
                for (var x = 0; x < Constants.MapWidth; x++)
                {
                    for (var y = 0; y < Constants.MapHeight; y++)
                    {
                        _layers[i].Tiles[x, y].TilesetIndex = bf.ReadInteger();
                        _layers[i].Tiles[x, y].X = bf.ReadInteger();
                        _layers[i].Tiles[x, y].Y = bf.ReadInteger();
                        _layers[i].Tiles[x, y].Autotile = bf.ReadByte();
                    }
                }
            }
            for (var x = 0; x < Constants.MapWidth; x++)
            {
                for (var y = 0; y < Constants.MapHeight; y++)
                {
                    Attributes[x, y].value = bf.ReadInteger();
                    Attributes[x, y].data1 = bf.ReadInteger();
                    Attributes[x, y].data2 = bf.ReadInteger();
                    Attributes[x, y].data3 = bf.ReadInteger();
                }
            }
            var lCount = bf.ReadInteger();
            Lights.Clear();
            for (var i = 0; i < lCount; i++)
            {
                Lights.Add(new Light(bf));
            }
            Revision = bf.ReadInteger();
            Deleted = (int)bf.ReadLong();
            MapGameData = packet.Skip(0).Take(bf.Pos()).ToArray();
            Events.Clear();
            var eCount = bf.ReadInteger();
            for (var i = 0; i < eCount; i++)
            {
                Events.Add(new EventStruct(bf));
            }

            //Clear Map Items
            for (int i = 0; i < MapItems.Count; i++)
            {
                MapItems[i].ItemNum = -1;
                PacketSender.SendMapItemUpdate(MyMapNum, i);
                MapItems.RemoveAt(i);
            }
            ItemRespawns.Clear();
            SpawnAttributeItems();
            //Save();
        }

        //Items
        private void SpawnAttributeItems()
        {
            for (int x = 0; x < Constants.MapWidth; x++)
            {
                for (int y = 0; y < Constants.MapHeight; y++)
                {
                    if (Attributes[x, y].value == (int)Enums.MapAttributes.Item)
                    {
                        SpawnAttributeItem(x, y);
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
            if (Globals.GameItems[MapItems[MapItems.Count - 1].ItemNum].Type >= (int)Enums.ItemTypes.Equipment)
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
            MapItems[MapItems.Count - 1].DespawnTime = Environment.TickCount + Constants.ItemDespawnTime;
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

        //Npcs
        private void SpawnMapNpcs()
        {

        }

        //Update + Related Functions
        public void Update()
        {
            if (CheckActive() == false) { return; }
            //Process Items
            lock (MapItems)
            {
                for (int i = 0; i < MapItems.Count; i++)
                {
                    if (MapItems[i].DespawnTime < Environment.TickCount)
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
                if (t.EntityIndex <= -1) continue;
                if (((Player) Globals.Entities[t.EntityIndex]) == null) continue;
                if (!((Player) Globals.Entities[t.EntityIndex]).InGame) continue;
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
                if (t.EntityIndex <= -1) continue;
                if (((Player)Globals.Entities[t.EntityIndex]) == null) continue;
                if (!((Player)Globals.Entities[t.EntityIndex]).InGame) continue;
                if (Globals.Entities[t.EntityIndex].CurrentMap == MyMapNum)
                {
                    Players.Add(t.ClientIndex);
                }
            }
            return Players;
        }
        public void PlayerEnteredMap()
        {
            Active = true;
            if (SurroundingMaps.Count <= 0) return;
            foreach (var t in SurroundingMaps)
            {
                Globals.GameMaps[t].Active = true;
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
    }

    class TileArray
    {
        public Tile[,] Tiles = new Tile[Constants.MapWidth, Constants.MapHeight];
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
    }
#endregion
}


