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
using System.Windows.Forms;

namespace Intersect_Editor.Classes
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
        public long Deleted;
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
        public byte ZoneType = 0; //Everything goes, 1 is safe, add more later
        public int PlayerLightSize = 300;
        public byte PlayerLightIntensity = 255;
        public float PlayerLightExpand = 0f;
        public Color PlayerLightColor = Color.White;
        public string OverlayGraphic = "None";


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
                        Attributes[x, y].animInstance = mapcopy.Attributes[x, y].animInstance;
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
                Events.Add(new EventStruct(Events.Count, bf));
            }
            Autotiles = new MapAutotiles(this);
            Autotiles.InitAutotiles();
        }

        //Saving/Loading
        public byte[] Save()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(0); //Never deleted
            bf.WriteString(Version);
            bf.WriteString(MyName);
            bf.WriteInteger(Revision);
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
            bf.WriteByte(ZoneType);
            bf.WriteString(OverlayGraphic);
            bf.WriteInteger(PlayerLightSize);
            bf.WriteDouble(PlayerLightExpand);
            bf.WriteByte(PlayerLightIntensity);
            bf.WriteByte(PlayerLightColor.R);
            bf.WriteByte(PlayerLightColor.G);
            bf.WriteByte(PlayerLightColor.B);

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
                    if (Attributes[x, y].value > 0)
                    {
                        bf.WriteInteger(Attributes[x, y].data1);
                        bf.WriteInteger(Attributes[x, y].data2);
                        bf.WriteInteger(Attributes[x, y].data3);
                        bf.WriteString(Attributes[x, y].data4);
                    }
                }
            }
            bf.WriteInteger(Lights.Count);
            foreach (var t in Lights)
            {
                bf.WriteBytes(t.LightData());
            }

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
            return bf.ToArray();
        }
        public void Load(byte[] myArr, bool import = false)
        {
            var npcCount = 0;
            NpcSpawn TempNpc = new NpcSpawn();
            var bf = new ByteBuffer();
            bf.WriteBytes(myArr);
            Deleted = bf.ReadInteger();
            if (Deleted == 0)
            {
                string loadedVersion = bf.ReadString();
                if (loadedVersion != Version)
                    throw new Exception("Failed to load Map #" + MyMapNum + ". Loaded Version: " + loadedVersion + " Expected Version: " + Version);

                MyName = bf.ReadString();
                Revision = bf.ReadInteger();
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
                ZoneType = bf.ReadByte();
                OverlayGraphic = bf.ReadString();
                PlayerLightSize = bf.ReadInteger();
                PlayerLightExpand = (float)bf.ReadDouble();
                PlayerLightIntensity = bf.ReadByte();
                PlayerLightColor = Color.FromArgb(bf.ReadByte(), bf.ReadByte(), bf.ReadByte());

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
                        int attributeType = bf.ReadInteger();
                        if (attributeType > 0)
                        {
                            Attributes[x, y].value = attributeType;
                            Attributes[x, y].data1 = bf.ReadInteger();
                            Attributes[x, y].data2 = bf.ReadInteger();
                            Attributes[x, y].data3 = bf.ReadInteger();
                            Attributes[x, y].data4 = bf.ReadString();
                        }
                        else
                        {
                            Attributes[x, y].value = 0;
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
                Autotiles.InitAutotiles();
                UpdateAdjacentAutotiles();
            }
        }

        //Helper Functions
        private void UpdateAdjacentAutotiles()
        {
            if (Up > -1 && Up < Globals.GameMaps.Length && Globals.GameMaps[Up] != null)
            {
                for (int x = 0; x < Globals.MapWidth; x++)
                {
                    for (int y = Globals.MapHeight - 1; y < Globals.MapHeight; y++)
                    {
                        Globals.GameMaps[Up].Autotiles.UpdateAutoTiles(x, y);
                    }
                }
            }
            if (Down > -1 && Up < Globals.GameMaps.Length && Globals.GameMaps[Down] != null)
            {
                for (int x = 0; x < Globals.MapWidth; x++)
                {
                    for (int y = 0; y < 1; y++)
                    {
                        Globals.GameMaps[Down].Autotiles.UpdateAutoTiles(x, y);
                    }
                }
            }
            if (Left > -1 && Left < Globals.GameMaps.Length && Globals.GameMaps[Left] != null)
            {
                for (int x = Globals.MapWidth - 1; x < Globals.MapWidth; x++)
                {
                    for (int y = 0; y < Globals.MapHeight; y++)
                    {
                        Globals.GameMaps[Left].Autotiles.UpdateAutoTiles(x, y);
                    }
                }
            }
            if (Right > -1 && Left < Globals.GameMaps.Length && Globals.GameMaps[Right] != null)
            {
                for (int x = 0; x < 1; x++)
                {
                    for (int y = 0; y < Globals.MapHeight; y++)
                    {
                        Globals.GameMaps[Right].Autotiles.UpdateAutoTiles(x, y);
                    }
                }
            }
        }
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
        public AnimationInstance animInstance;
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
        public byte Intensity = 255;
        public int Size = 0;
        public float Expand = 0f;
        public System.Drawing.Color Color = System.Drawing.Color.White;
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
            Color = Color.FromArgb(copy.Color.R, copy.Color.G, copy.Color.B);
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
            Color = Color.FromArgb(myBuffer.ReadByte(), myBuffer.ReadByte(), myBuffer.ReadByte());

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

