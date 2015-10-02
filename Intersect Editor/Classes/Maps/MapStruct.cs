/*
    Intersect Game Engine (Editor)
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
using System.Drawing.Drawing2D;

namespace Intersect_Editor.Classes
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
        public TileArray[] Layers = new TileArray[Constants.LayerCount];
        public Attribute[,] Attributes = new Attribute[Globals.MapWidth, Globals.MapHeight];
        public List<EventStruct> Events = new List<EventStruct>();
        public List<Light> Lights = new List<Light>();

        //Properties
        public List<NpcSpawn> Spawns = new List<NpcSpawn>();
        public string Music = "";
        public string Sound = "";
        public bool IsIndoors;

        //Visual Effect Properties
        public string Panorama = "None";
        public string Fog = "None";
        public int FogXSpeed = 0;
        public int FogYSpeed = 0;
        public int FogTransparency = 0;
        public int RHue = 0;
        public int GHue = 0;
        public int BHue = 0;
        public int AHue = 0;
        public int Brightness = 100;


        //Temporary Values
        public MapAutotiles Autotiles;

        //Init
        public MapStruct(int mapNum, byte[] mapData)
        {
            MyMapNum = mapNum;
            for (var i = 0; i < Constants.LayerCount; i++)
            {
                Layers[i] = new TileArray();
                for (var x = 0; x < Globals.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y] = new Tile();
                        if (i == 0) { Attributes[x, y] = new Attribute(); }
                    }
                }
            }
            Autotiles = new MapAutotiles(this);
            Autotiles.InitAutotiles();
            Load(mapData);
        }

        public MapStruct(MapStruct mapcopy)
        {
            ByteBuffer bf = new ByteBuffer();
            MyName = mapcopy.MyName;
            Brightness = mapcopy.Brightness;
            IsIndoors = mapcopy.IsIndoors;
            for (var i = 0; i < Constants.LayerCount; i++)
            {
                Layers[i] = new TileArray();
                for (var x = 0; x < Globals.MapWidth; x++)
                {
                    for (var y = 0; y < Globals.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y] = new Tile();
                        Layers[i].Tiles[x, y].TilesetIndex = mapcopy.Layers[i].Tiles[x, y].TilesetIndex;
                        Layers[i].Tiles[x, y].X = mapcopy.Layers[i].Tiles[x, y].X;
                        Layers[i].Tiles[x, y].Y = mapcopy.Layers[i].Tiles[x, y].Y;
                        Layers[i].Tiles[x, y].Autotile = mapcopy.Layers[i].Tiles[x, y].Autotile;
                        if (i == 0) { Attributes[x, y] = new Attribute(); }
                        Attributes[x, y].value = mapcopy.Attributes[x, y].value;
                        Attributes[x, y].data1 = mapcopy.Attributes[x, y].data1;
                        Attributes[x, y].data2 = mapcopy.Attributes[x, y].data2;
                        Attributes[x, y].data3 = mapcopy.Attributes[x, y].data3;
                        Attributes[x, y].data4 = mapcopy.Attributes[x, y].data4;
                    }
                }
            }
            for (var i = 0; i < mapcopy.Spawns.Count; i++)
            {
                Spawns.Add(new NpcSpawn(mapcopy.Spawns[i]));
            }
            for (var i = 0; i < mapcopy.Lights.Count; i++)
            {
                Lights.Add(new Light(mapcopy.Lights[i]));
            }
            for (var i = 0; i < mapcopy.Events.Count; i++)
            {
                bf.WriteBytes(mapcopy.Events[i].EventData());
                Events.Add(new EventStruct(bf));
            }
            Autotiles = new MapAutotiles(this);
            Autotiles.InitAutotiles();
        }

        //Saving/Loading
        public byte[] Save()
        {
            var bf = new ByteBuffer();
            bf.WriteString(MyName);
            bf.WriteInteger(Up);
            bf.WriteInteger(Down);
            bf.WriteInteger(Left);
            bf.WriteInteger(Right);
            bf.WriteString(Music);
            bf.WriteString(Sound);
            bf.WriteInteger(Convert.ToInt32(IsIndoors));
            bf.WriteString(Panorama);
            bf.WriteString(Fog);
            bf.WriteInteger(FogXSpeed);
            bf.WriteInteger(FogYSpeed);
            bf.WriteInteger(FogTransparency);
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
                        bf.WriteInteger(Layers[i].Tiles[x, y].TilesetIndex);
                        bf.WriteInteger(Layers[i].Tiles[x, y].X);
                        bf.WriteInteger(Layers[i].Tiles[x, y].Y);
                        bf.WriteByte(Layers[i].Tiles[x, y].Autotile);
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
            bf.WriteInteger(Revision + 1);
            bf.WriteLong(0); //Never deleted.
            bf.WriteInteger(Events.Count);
            foreach (var t in Events)
            {
                bf.WriteBytes(t.EventData());
            }
            return bf.ToArray();
        }
        public void Load(byte[] myArr, bool import = false)
        {
            var npcCount = 0;
            NpcSpawn TempNpc = new NpcSpawn();

            var bf = new ByteBuffer();
            bf.WriteBytes(myArr);
            MyName = bf.ReadString();
            if (!import)
            {
                Up = bf.ReadInteger();
                Down = bf.ReadInteger();
                Left = bf.ReadInteger();
                Right = bf.ReadInteger();
            }
            else
            {
                bf.ReadInteger();
                bf.ReadInteger();
                bf.ReadInteger();
                bf.ReadInteger();
            }
            Music = bf.ReadString();
            Sound = bf.ReadString();
            IsIndoors = Convert.ToBoolean(bf.ReadInteger());
            Panorama = bf.ReadString();
            Fog = bf.ReadString();
            FogXSpeed = bf.ReadInteger();
            FogYSpeed = bf.ReadInteger();
            FogTransparency = bf.ReadInteger();
            RHue = bf.ReadInteger();
            GHue = bf.ReadInteger();
            BHue = bf.ReadInteger();
            AHue = bf.ReadInteger();
            Brightness = bf.ReadInteger();

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
                        Layers[i].Tiles[x, y].TilesetIndex = bf.ReadInteger();
                        Layers[i].Tiles[x, y].X = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Y = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Autotile = bf.ReadByte();
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
            for (var i = 0; i < lCount; i++)
            {
                Lights.Add(new Light(bf));
            }
            Revision = bf.ReadInteger();
            Deleted = bf.ReadInteger();
            if (Deleted == 1)
            {
                Layers = null;
                return;
            }
            Events.Clear();
            var eCount = bf.ReadInteger();
            for (var i = 0; i < eCount; i++)
            {
                Events.Add(new EventStruct(bf));
            }
            Autotiles.InitAutotiles();
        }

        //Helper Functions
        public EventStruct FindEventAt(int x, int y)
        {
            if (Events.Count <= 0) return null;
            foreach (var t in Events)
            {
                if (t.Deleted == 1) continue;
                if (t.SpawnX == x && t.SpawnY == y)
                {
                    return t;
                }
            }
            return null;
        }
        public Light FindLightAt(int x, int y)
        {
            if (Lights.Count <= 0) return null;
            foreach (var t in Lights)
            {
                if (t.TileX == x && t.TileY == y)
                {
                    return t;
                }
            }
            return null;
        }
        public NpcSpawn FindSpawnAt(int x, int y)
        {
            if (Spawns.Count <= 0) return null;
            foreach (var t in Spawns)
            {
                if (t.X == x && t.Y == y)
                {
                    return t;
                }
            }
            return null;
        }
    }

    public class Attribute
    {
        public int value;
        public int data1;
        public int data2;
        public int data3;
        public string data4 = "";
    }

    public class TileArray {
        public Tile[,] Tiles = new Tile[Globals.MapWidth ,Globals.MapHeight];
    }

    public class Tile {
        public int TilesetIndex = -1;
        public int X;
        public int Y;
        public byte Autotile;
    }

    public class MapRef
    {
        public string MapName = "";
        public int Deleted = 0;
    }

    public class Light
    {
        public int OffsetX;
        public int OffsetY;
        public int TileX;
        public int TileY;
        public double Intensity = 1;
        public int Range = 20;
        public Bitmap Graphic;
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
            Range = copy.Range;
        }
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

        public NpcSpawn()
        {
            
        }

        public NpcSpawn(NpcSpawn copy)
        {
            NpcNum = copy.NpcNum;
            X = copy.X;
            Y = copy.Y;
            Dir = copy.Dir;
        }
    }
}

