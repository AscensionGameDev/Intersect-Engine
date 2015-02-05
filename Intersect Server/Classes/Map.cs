using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Intersect_Server.Classes
{
    public class Map
    {
        public string MyName = "New Map";
        public int Up = -1;
        public int Down = -1;
        public int Left = -1;
        public int Right = -1;
        private string _bgm = "";
        private readonly TileArray[] _layers = new TileArray[Constants.LayerCount];
        public int MyMapNum;
        public int Deleted;
        public byte[] MapGameData;
        public byte[] MapData;
        public int[,] Blocked = new int[Constants.MapWidth, Constants.MapHeight];
        public int MapGrid;
        public int MapGridX;
        public int MapGridY;
        public bool Active;
        public int Revision;
        public List<int> SurroundingMaps = new List<int>();
        public List<Event> Events = new List<Event>();
        public List<Light> Lights = new List<Light>();
        public bool IsIndoors;
        public Map(int mapNum)
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
                    }
                }
            }
        }

        public void Save()
        {
            var bf = new ByteBuffer();
            bf.WriteString(MyName);
            bf.WriteInteger(Up);
            bf.WriteInteger(Down);
            bf.WriteInteger(Left);
            bf.WriteInteger(Right);
            bf.WriteString(_bgm);
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
                    bf.WriteInteger(Blocked[x, y]);
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
            _bgm = bf.ReadString();
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
                    Blocked[x, y] = bf.ReadInteger();
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
                Events.Add(new Event(bf));
            }
            Save();
        }

        public void Update()
        {
            if (CheckActive() == false) { return; }
            foreach (var t in Globals.Entities)
            {
                if (t == null) continue;
                if (t.GetType() == typeof(Npc))
                {
                    if (t.CurrentMap == MyMapNum)
                    {
                        ((Npc)t).Update();
                    }
                }
                else if (t.GetType() == typeof(Player))
                {
                    if (t.CurrentMap == MyMapNum)
                    {
                        ((Player)t).Update();
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
                if (t.entityIndex <= -1) continue;
                if (((Player) Globals.Entities[t.entityIndex]) == null) continue;
                if (!((Player) Globals.Entities[t.entityIndex]).InGame) continue;
                if (Globals.Entities[t.entityIndex].CurrentMap == mapNum)
                {
                    return true;
                }
            }
            return false;
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
}


