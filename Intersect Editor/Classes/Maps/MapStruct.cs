using System;
using System.Collections.Generic;
using System.Drawing;

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
        public Attribute[,] Attributes = new Attribute[Constants.MapWidth, Constants.MapHeight];
        public List<EventStruct> Events = new List<EventStruct>();
        public List<Light> Lights = new List<Light>();

        //Properties
        public List<NpcSpawn> Spawns = new List<NpcSpawn>();
        public string Bgm;
        public bool IsIndoors;

        //Temporary Values
        public MapAutotiles Autotiles;

        //Init
        public MapStruct(int mapNum, byte[] mapData)
        {
            MyMapNum = mapNum;
            for (var i = 0; i < Constants.LayerCount; i++)
            {
                Layers[i] = new TileArray();
                for (var x = 0; x < Constants.MapWidth; x++)
                {
                    for (var y = 0; y < Constants.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y] = new Tile();
                        if (i == 0) { Attributes[x, y] = new Attribute(); }
                    }
                }
            }
            Load(mapData);
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
            bf.WriteString(Bgm);
            bf.WriteInteger(Convert.ToInt32(IsIndoors));
            for (var i = 0; i < Constants.LayerCount; i++)
            {
                for (var x = 0; x < Constants.MapWidth; x++)
                {
                    for (var y = 0; y < Constants.MapHeight; y++)
                    {
                        bf.WriteInteger(Layers[i].Tiles[x, y].TilesetIndex);
                        bf.WriteInteger(Layers[i].Tiles[x, y].X);
                        bf.WriteInteger(Layers[i].Tiles[x, y].Y);
                        bf.WriteByte(Layers[i].Tiles[x, y].Autotile);
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
            bf.WriteInteger(Revision + 1);
            bf.WriteLong(0); //Never deleted.
            bf.WriteInteger(Events.Count);
            foreach (var t in Events)
            {
                bf.WriteBytes(t.EventData());
            }
            return bf.ToArray();
        }
        public void Load(byte[] myArr)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(myArr);
            MyName = bf.ReadString();
            Up = bf.ReadInteger();
            Down = bf.ReadInteger();
            Left = bf.ReadInteger();
            Right = bf.ReadInteger();
            Bgm = bf.ReadString();
            IsIndoors = Convert.ToBoolean(bf.ReadInteger());
            for (var i = 0; i < Constants.LayerCount; i++)
            {
                for (var x = 0; x < Constants.MapWidth; x++)
                {
                    for (var y = 0; y < Constants.MapHeight; y++)
                    {
                        Layers[i].Tiles[x, y].TilesetIndex = bf.ReadInteger();
                        Layers[i].Tiles[x, y].X = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Y = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Autotile = bf.ReadByte();
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
            for (var i = 0; i < lCount; i++)
            {
                Lights.Add(new Light(bf));
            }
            Revision = bf.ReadInteger();
            bf.ReadLong();
            Events.Clear();
            var eCount = bf.ReadInteger();
            for (var i = 0; i < eCount; i++)
            {
                Events.Add(new EventStruct(bf));
            }
            Autotiles = new MapAutotiles(this);
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
    }

    public class Attribute
    {
        public int value;
        public int data1;
        public int data2;
        public int data3;
    }

    public class TileArray {
        public Tile[,] Tiles = new Tile[Constants.MapWidth ,Constants.MapHeight];
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
}

