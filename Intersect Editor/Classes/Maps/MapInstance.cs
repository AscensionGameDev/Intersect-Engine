using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using Intersect.Editor.Classes.Entities;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;

namespace Intersect.Editor.Classes.Maps
{
    public class MapInstance : MapBase, IGameObject<int, MapInstance>
    {
        private static MapInstances sLookup;

        public new static MapInstances Lookup => (sLookup = (sLookup ?? new MapInstances(MapBase.Lookup)));

        //Map Attributes
        private Dictionary<Attribute, AnimationInstance> _attributeAnimInstances =
            new Dictionary<Attribute, AnimationInstance>();

        private byte[] loadedData;

        public MapInstance(int mapNum) : base(mapNum, false)
        {
            lock (GetMapLock())
            {
                Autotiles = new MapAutotiles(this);
            }
        }

        public MapInstance(MapBase mapStruct) : base(mapStruct)
        {
            lock (GetMapLock())
            {
                Autotiles = new MapAutotiles(this);
                if (typeof(MapInstance) == mapStruct.GetType())
                {
                    MapGridX = ((MapInstance) mapStruct).MapGridX;
                    MapGridY = ((MapInstance) mapStruct).MapGridY;
                }
                InitAutotiles();
            }
        }

        //World Position
        public int MapGridX { get; set; }
        public int MapGridY { get; set; }

        public void Load(byte[] myArr, bool import = false)
        {
            lock (GetMapLock())
            {
                var up = Up;
                var down = Down;
                var left = Left;
                var right = Right;
                base.Load(myArr);
                if (import)
                {
                    Up = up;
                    Down = down;
                    Left = left;
                    Right = right;
                }
            }
        }
        public void LoadTileData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(packet);
            lock (GetMapLock())
            {
                Layers = new TileArray[Options.LayerCount];
                for (var i = 0; i < Options.LayerCount; i++)
                {
                    Layers[i].Tiles = new Tile[Options.MapWidth, Options.MapHeight];
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            Layers[i].Tiles[x, y].TilesetIndex = bf.ReadInteger();
                            Layers[i].Tiles[x, y].X = bf.ReadInteger();
                            Layers[i].Tiles[x, y].Y = bf.ReadInteger();
                            Layers[i].Tiles[x, y].Autotile = bf.ReadByte();
                        }
                    }
                }
            }
            bf.Dispose();
        }

        public void SaveStateAsUnchanged()
        {
            loadedData = SaveInternal();
        }

        public void LoadInternal(byte[] myArr, bool import = false)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(myArr);
            var mapDataLength = bf.ReadInteger();
            var mapData = bf.ReadBytes(mapDataLength);
            var tileDataLength = bf.ReadInteger();
            var tileData = bf.ReadBytes(tileDataLength);
            Load(mapData, import);
            LoadTileData(tileData);
            bf.Dispose();
        }

        public byte[] SaveInternal()
        {
            var bf = new ByteBuffer();
            var mapData = GetMapData(false);
            bf.WriteInteger(mapData.Length);
            bf.WriteBytes(mapData);
            var tileData = GenerateTileData();
            bf.WriteInteger(tileData.Length);
            bf.WriteBytes(tileData);
            return bf.ToArray();
        }

        public byte[] GenerateTileData()
        {
            var bf = new ByteBuffer();
            for (var i = 0; i < Options.LayerCount; i++)
            {
                for (var x = 0; x < Options.MapWidth; x++)
                {
                    for (var y = 0; y < Options.MapHeight; y++)
                    {
                        bf.WriteInteger(Layers[i].Tiles[x, y].TilesetIndex);
                        bf.WriteInteger(Layers[i].Tiles[x, y].X);
                        bf.WriteInteger(Layers[i].Tiles[x, y].Y);
                        bf.WriteByte(Layers[i].Tiles[x, y].Autotile);
                    }
                }
            }
            return bf.ToArray();
        }

        public bool Changed()
        {
            if (loadedData != null)
            {
                var newData = SaveInternal();
                if (newData.Length == loadedData.Length)
                {
                    for (int i = 0; i < newData.Length; i++)
                    {
                        if (newData[i] != loadedData[i])
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            return true;
        }

        //Attribute/Animations
        public AnimationInstance GetAttributeAnimation(Attribute attr, int animNum)
        {
            if (attr == null) return null;
            if (!_attributeAnimInstances.ContainsKey(attr))
            {
                _attributeAnimInstances.Add(attr, new AnimationInstance(AnimationBase.Lookup.Get<AnimationBase>(animNum), true));
            }
            return _attributeAnimInstances[attr];
        }

        public void SetAttributeAnimation(Attribute attribute, AnimationInstance animationInstance)
        {
            if (_attributeAnimInstances.ContainsKey(attribute))
            {
                _attributeAnimInstances[attribute] = animationInstance;
            }
        }

        public void Update()
        {
            if (Globals.MapsToScreenshot.Contains(Index))
            {
                if (Globals.MapGrid != null && Globals.MapGrid.Loaded)
                {
                    if (Globals.MapGrid.Contains(Index))
                    {
                        for (int y = Globals.CurrentMap.MapGridY + 1; y >= Globals.CurrentMap.MapGridY - 1; y--)
                        {
                            for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight &&
                                    Globals.MapGrid.Grid[x, y].mapnum > -1)
                                {
                                    var needMap = Lookup.Get(Globals.MapGrid.Grid[x, y].mapnum);
                                    if (needMap == null) return;
                                }
                            }
                        }
                    }
                    //We have everything, let's screenshot!
                    var prevMap = Globals.CurrentMap;
                    Globals.CurrentMap = this;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        lock (EditorGraphics.GraphicsLock)
                        {
                            var screenshotTexture = EditorGraphics.ScreenShotMap();
                            screenshotTexture.Save(ms, ImageFormat.Png);
                            ms.Close();
                        }
                        Database.SaveMapCache(Index, Revision, ms.ToArray());
                    }
                    Globals.CurrentMap = prevMap;
                    Globals.MapsToScreenshot.Remove(Index);

                    //See if this map is around our current map, if not let's delete it
                    if (Globals.CurrentMap != null && Globals.MapGrid != null && Globals.MapGrid.Loaded)
                    {
                        for (int y = Globals.CurrentMap.MapGridY + 1; y >= Globals.CurrentMap.MapGridY - 1; y--)
                        {
                            for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                                {
                                    if (Globals.MapGrid.Grid[x, y].mapnum == Index) return;
                                }
                            }
                        }
                        Delete();
                    }
                }
            }
        }

        //Helper Functions
        public MapBase[,] GenerateAutotileGrid()
        {
            MapBase[,] mapBase = new MapBase[3, 3];
            if (Globals.MapGrid != null && Globals.MapGrid.Contains(Index))
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        var x1 = MapGridX + x;
                        var y1 = MapGridY + y;
                        if (x1 >= 0 && y1 >= 0 && x1 < Globals.MapGrid.GridWidth && y1 < Globals.MapGrid.GridHeight)
                        {
                            if (x == 0 && y == 0)
                            {
                                mapBase[x + 1, y + 1] = this;
                            }
                            else
                            {
                                mapBase[x + 1, y + 1] = Lookup.Get<MapInstance>(Globals.MapGrid.Grid[x1, y1].mapnum);
                            }
                        }
                    }
                }
            }
            mapBase[1,1] = this;
            return mapBase;
        }

        public void InitAutotiles()
        {
            lock (GetMapLock())
            {
                Autotiles.InitAutotiles(GenerateAutotileGrid());
            }
        }

        public void UpdateAdjacentAutotiles()
        {
            if (Globals.MapGrid != null && Globals.MapGrid.Contains(Index))
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        var x1 = MapGridX + x;
                        var y1 = MapGridY + y;
                        if (x1 >= 0 && y1 >= 0 && x1 < Globals.MapGrid.GridWidth && y1 < Globals.MapGrid.GridHeight)
                        {
                            var map = Lookup.Get<MapInstance>(Globals.MapGrid.Grid[x1, y1].mapnum);
                            if (map != null && map != this) map.InitAutotiles();
                        }
                    }
                }
            }
        }

        public EventBase FindEventAt(int x, int y)
        {
            if (Events.Count <= 0) return null;
            foreach (var t in Events.Values)
            {
                if (t.SpawnX == x && t.SpawnY == y)
                {
                    return t;
                }
            }
            return null;
        }

        public LightBase FindLightAt(int x, int y)
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

        public override byte[] BinaryData => GetMapData(false);

        public override void Delete() => Lookup?.Delete(this);

        public void Dispose()
        {
        }
    }
}