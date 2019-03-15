using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using Intersect.Editor.Entities;
using Intersect.Editor.General;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;

using Newtonsoft.Json;

using MapAttribute = Intersect.GameObjects.Maps.MapAttribute;

namespace Intersect.Editor.Maps
{
    public class MapInstance : MapBase, IGameObject<Guid, MapInstance>
    {
        private static MapInstances sLookup;

        //Map Attributes
        private Dictionary<MapAttribute, AnimationInstance> mAttributeAnimInstances = new Dictionary<MapAttribute, AnimationInstance>();

        private byte[] mLoadedData;

        public MapInstance(Guid id) : base(id)
        {
            lock (MapLock)
            {
                Autotiles = new MapAutotiles(this);
            }
        }

        public MapInstance(MapBase mapStruct) : base(mapStruct)
        {
            lock (MapLock)
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

        public new static MapInstances Lookup => (sLookup = (sLookup ?? new MapInstances(MapBase.Lookup)));

        //World Position
        public int MapGridX { get; set; }

        public int MapGridY { get; set; }

        public void Load(string mapJson, bool import = false)
        {
            lock (MapLock)
            {
                var up = Up;
                var down = Down;
                var left = Left;
                var right = Right;
                base.Load(mapJson);
                if (import)
                {
                    Up = up;
                    Down = down;
                    Left = left;
                    Right = right;
                }
                Autotiles = new MapAutotiles(this);

                //Initialize Local Events
                LocalEvents.Clear();
                foreach (var id in EventIds)
                {
                    var evt = EventBase.Get(id);
                    LocalEvents.Add(id,evt);
                }
            }
        }

        public void LoadTileData(byte[] packet)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(Compression.DecompressPacket(packet));
            lock (MapLock)
            {
                Layers = new TileArray[Options.LayerCount];
                for (var i = 0; i < Options.LayerCount; i++)
                {
                    Layers[i].Tiles = new Tile[Options.MapWidth, Options.MapHeight];
                    for (var x = 0; x < Options.MapWidth; x++)
                    {
                        for (var y = 0; y < Options.MapHeight; y++)
                        {
                            Layers[i].Tiles[x, y].TilesetId = bf.ReadGuid();
                            Layers[i].Tiles[x, y].X = bf.ReadInteger();
                            Layers[i].Tiles[x, y].Y = bf.ReadInteger();
                            Layers[i].Tiles[x, y].Autotile = bf.ReadByte();
                        }
                    }
                }
            }
            bf.Dispose();
			InitAutotiles();
		}

        public void SaveStateAsUnchanged()
        {
            mLoadedData = SaveInternal();
        }

        public void LoadInternal(byte[] myArr, bool import = false)
        {
            var bf = new ByteBuffer();
            bf.WriteBytes(myArr);
            var mapJson = bf.ReadString();
            var tileDataLength = bf.ReadInteger();
            var tileData = bf.ReadBytes(tileDataLength);
            var attributeDataLength = bf.ReadInteger();
            var attributeData = bf.ReadBytes(attributeDataLength);
            Load(mapJson, import);
            LoadTileData(tileData);
            AttributeData = attributeData;
            bf.Dispose();
        }

        public byte[] SaveInternal()
        {
            var bf = new ByteBuffer();
            bf.WriteString(JsonData);
            var tileData = GenerateTileData();
            bf.WriteInteger(tileData.Length);
            bf.WriteBytes(tileData);
            var attributeData = AttributeData;
            bf.WriteInteger(attributeData.Length);
            bf.WriteBytes(attributeData);
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
                        bf.WriteGuid(Layers[i].Tiles[x, y].TilesetId);
                        bf.WriteInteger(Layers[i].Tiles[x, y].X);
                        bf.WriteInteger(Layers[i].Tiles[x, y].Y);
                        bf.WriteByte(Layers[i].Tiles[x, y].Autotile);
                    }
                }
            }
            return Compression.CompressPacket(bf.ToArray());
        }

        public bool Changed()
        {
            if (mLoadedData != null)
            {
                var newData = SaveInternal();
                if (newData.Length == mLoadedData.Length)
                {
                    for (int i = 0; i < newData.Length; i++)
                    {
                        if (newData[i] != mLoadedData[i])
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
        public AnimationInstance GetAttributeAnimation(MapAttribute attr, Guid animId)
        {
            if (attr == null) return null;
            if (!mAttributeAnimInstances.ContainsKey(attr))
            {
                mAttributeAnimInstances.Add(attr,
                    new AnimationInstance(AnimationBase.Get(animId), true));
            }
            return mAttributeAnimInstances[attr];
        }

        public void SetAttributeAnimation(MapAttribute attribute, AnimationInstance animationInstance)
        {
            if (mAttributeAnimInstances.ContainsKey(attribute))
            {
                mAttributeAnimInstances[attribute] = animationInstance;
            }
        }
        
        public override byte[] GetAttributeData()
        {
            return Compression.CompressPacket(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Attributes, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, ObjectCreationHandling = ObjectCreationHandling.Replace })));
        }

        public void Update()
        {
            if (Globals.MapsToScreenshot.Contains(Id))
            {
                if (Globals.MapGrid != null && Globals.MapGrid.Loaded)
                {
                    if (Globals.MapGrid.Contains(Id))
                    {
                        for (int y = Globals.CurrentMap.MapGridY + 1; y >= Globals.CurrentMap.MapGridY - 1; y--)
                        {
                            for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 &&
                                    y < Globals.MapGrid.GridHeight && Globals.MapGrid.Grid[x, y].MapId != Guid.Empty)
                                {
                                    var needMap = Lookup.Get(Globals.MapGrid.Grid[x, y].MapId);
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
                        Database.SaveMapCache(Id, Revision, ms.ToArray());
                    }
                    Globals.CurrentMap = prevMap;
                    Globals.MapsToScreenshot.Remove(Id);

                    //See if this map is around our current map, if not let's delete it
                    if (Globals.CurrentMap != null && Globals.MapGrid != null && Globals.MapGrid.Loaded)
                    {
                        for (int y = Globals.CurrentMap.MapGridY + 1; y >= Globals.CurrentMap.MapGridY - 1; y--)
                        {
                            for (int x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                                {
                                    if (Globals.MapGrid.Grid[x, y].MapId == Id) return;
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
            if (Globals.MapGrid != null && Globals.MapGrid.Contains(Id))
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
                                mapBase[x + 1, y + 1] = Lookup.Get<MapInstance>(Globals.MapGrid.Grid[x1, y1].MapId);
                            }
                        }
                    }
                }
            }
            mapBase[1, 1] = this;
            return mapBase;
        }

        public void InitAutotiles()
        {
            lock (MapLock)
            {
                Autotiles.InitAutotiles(GenerateAutotileGrid());
            }
        }

        public void UpdateAdjacentAutotiles()
        {
            if (Globals.MapGrid != null && Globals.MapGrid.Contains(Id))
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        var x1 = MapGridX + x;
                        var y1 = MapGridY + y;
                        if (x1 >= 0 && y1 >= 0 && x1 < Globals.MapGrid.GridWidth && y1 < Globals.MapGrid.GridHeight)
                        {
                            var map = Lookup.Get<MapInstance>(Globals.MapGrid.Grid[x1, y1].MapId);
                            if (map != null && map != this) map.InitAutotiles();
                        }
                    }
                }
            }
        }

        public EventBase FindEventAt(int x, int y)
        {
            if (LocalEvents.Count <= 0) return null;
            foreach (var t in LocalEvents.Values)
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

        public new static MapInstance Get(Guid id)
        {
            return MapInstance.Lookup.Get<MapInstance>(id);
        }

        public override void Delete() => Lookup?.Delete(this);

        public void Dispose()
        {
        }
    }
}