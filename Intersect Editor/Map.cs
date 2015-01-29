using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace IntersectEditor
{
    public class Map
    {
        private const int layerCount = 5;
        public TileArray[] Layers = new TileArray[Constants.LAYER_COUNT];
        public int myMapNum = 0;
        public string myName = "New Map";
        public int up = -1;
        public int down = -1;
        public int left = -1;
        public int right = -1;
        public string BGM;
        private bool mapLoaded = false;
        public int[,] blocked = new int[Constants.MAP_WIDTH, Constants.MAP_HEIGHT];
        public MapAutotiles autotiles;
        public int revision;
        public List<Event> Events = new List<Event>();
        public List<Light> Lights = new List<Light>();
        public bool isIndoors;
        public Map(int mapNum, byte[] mapData)
        {
            myMapNum = mapNum;
            for (int i = 0; i < Constants.LAYER_COUNT; i++)
            {
                Layers[i] = new TileArray();
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        Layers[i].Tiles[x, y] = new Tile();
                    }
                }
            }
            Load(mapData);
        }

        public byte[] Save()
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteString(myName);
            bf.WriteInteger(up);
            bf.WriteInteger(down);
            bf.WriteInteger(left);
            bf.WriteInteger(right);
            bf.WriteString(BGM);
            bf.WriteInteger(Convert.ToInt32(isIndoors));
            for (int i = 0; i < Constants.LAYER_COUNT; i++)
            {
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        bf.WriteInteger(Layers[i].Tiles[x, y].tilesetIndex);
                        bf.WriteInteger(Layers[i].Tiles[x, y].x);
                        bf.WriteInteger(Layers[i].Tiles[x, y].y);
                        bf.WriteByte(Layers[i].Tiles[x, y].Autotile);
                    }
                }
            }
            for (int x = 0; x < Constants.MAP_WIDTH; x++)
            {
                for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                {
                    bf.WriteInteger(blocked[x,y]);
                }
            }
            bf.WriteInteger(Lights.Count);
            for (int i = 0; i < Lights.Count; i++)
            {
                bf.WriteBytes(Lights[i].LightData());
            }
            bf.WriteInteger(revision + 1);
            bf.WriteLong(0); //Never deleted.
            bf.WriteInteger(Events.Count);
            for (int i = 0; i < Events.Count; i++)
            {
                bf.WriteBytes(Events[i].EventData());
            }
            return bf.ToArray();
        }

        public void Load(byte[] myArr)
        {
            ByteBuffer bf = new ByteBuffer();
            bf.WriteBytes(myArr);
            myName = bf.ReadString();
            up = bf.ReadInteger();
            down = bf.ReadInteger();
            left = bf.ReadInteger();
            right = bf.ReadInteger();
            BGM = bf.ReadString();
            isIndoors = Convert.ToBoolean(bf.ReadInteger());
            for (int i = 0; i < Constants.LAYER_COUNT; i++)
            {
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        Layers[i].Tiles[x, y].tilesetIndex = bf.ReadInteger();
                        Layers[i].Tiles[x, y].x = bf.ReadInteger();
                        Layers[i].Tiles[x, y].y = bf.ReadInteger();
                        Layers[i].Tiles[x, y].Autotile = bf.ReadByte();
                    }
                }
            }
            for (int x = 0; x < Constants.MAP_WIDTH; x++)
            {
                for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                {
                    blocked[x,y] = bf.ReadInteger();
                }
            }
            int lCount = bf.ReadInteger();
            for (int i = 0; i < lCount; i++)
            {
                Lights.Add(new Light(bf));
            }
            revision = bf.ReadInteger();
            bf.ReadLong();
            Events.Clear();
            int eCount = bf.ReadInteger();
            for (int i = 0; i < eCount; i++)
            {
                Events.Add(new Event(bf));
            }
            autotiles = new MapAutotiles(this);
            autotiles.initAutotiles();
            mapLoaded = true;

        }

        public Event FindEventAt(int x, int y)
        {
            if (Events.Count > 0)
            {
                for (int i = 0; i < Events.Count; i++)
                {
                    if (Events[i].deleted != 1)
                    {
                        if (Events[i].spawnX == x && Events[i].spawnY == y)
                        {
                            return Events[i];
                        }
                    }
                }
            }
            return null;
        }

        public Light FindLightAt(int x, int y)
        {
            if (Lights.Count > 0)
            {
                for (int i = 0; i < Lights.Count; i++)
                {
                        if (Lights[i].tileX == x && Lights[i].tileY == y)
                        {
                            return Lights[i];
                        }
                }
            }
            return null;
        }
    }

    public class TileArray {
        public Tile[,] Tiles = new Tile[Constants.MAP_WIDTH ,Constants.MAP_HEIGHT];
    }

    public class Tile {
        public int tilesetIndex = -1;
        public int x;
        public int y;
        public byte Autotile;
    }

    public class MapRef
    {
        public string MapName = "";
        public int deleted = 0;
    }

    public class Light
    {
        public int offsetX = 0;
        public int offsetY = 0;
        public int tileX;
        public int tileY;
        public double intensity = 1;
        public int range = 20;
        public System.Drawing.Bitmap graphic;
        public Light(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
        public Light(ByteBuffer myBuffer)
        {
            offsetX = myBuffer.ReadInteger();
            offsetY = myBuffer.ReadInteger();
            tileX = myBuffer.ReadInteger();
            tileY = myBuffer.ReadInteger();
            intensity = myBuffer.ReadDouble();
            range = myBuffer.ReadInteger();
        }
        public byte[] LightData()
        {
            ByteBuffer myBuffer = new ByteBuffer();
            myBuffer.WriteInteger(offsetX);
            myBuffer.WriteInteger(offsetY);
            myBuffer.WriteInteger(tileX);
            myBuffer.WriteInteger(tileY);
            myBuffer.WriteDouble(intensity);
            myBuffer.WriteInteger(range);
            return myBuffer.ToArray();
        }
    }
}

