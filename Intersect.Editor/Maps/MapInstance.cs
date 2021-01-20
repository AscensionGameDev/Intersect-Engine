using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Intersect.Compression;
using Intersect.Editor.Classes.Maps;
using Intersect.Editor.Core;
using Intersect.Editor.Entities;
using Intersect.Editor.General;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Newtonsoft.Json;

namespace Intersect.Editor.Maps
{

    public class MapInstance : MapBase, IGameObject<Guid, MapInstance>
    {

        private static MapInstances sLookup;

        //Map Attributes
        private Dictionary<MapAttribute, Animation> mAttributeAnimInstances = new Dictionary<MapAttribute, Animation>();

        private MapSaveState mLoadedState;

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

        public new static MapInstances Lookup => sLookup = sLookup ?? new MapInstances(MapBase.Lookup);

        //World Position
        public int MapGridX { get; set; }

        public int MapGridY { get; set; }

        public void Load(string mapJson, bool import = false, bool clearEvents = true)
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
                if (clearEvents)
                {
                    LocalEvents.Clear();
                    foreach (var id in EventIds)
                    {
                        var evt = EventBase.Get(id);
                        LocalEvents.Add(id, evt);
                    }
                }
            }
        }

        public void LoadTileData(byte[] packet)
        {
            lock (MapLock)
            {
                Layers = JsonConvert.DeserializeObject<Dictionary<string, Tile[,]>>(LZ4.UnPickleString(packet), mJsonSerializerSettings);
                foreach (var layer in Options.Instance.MapOpts.Layers.All)
                {
                    if (!Layers.ContainsKey(layer))
                    {
                        Layers.Add(layer, new Tile[Options.MapWidth, Options.MapHeight]);
                    }
                }
            }

            InitAutotiles();
        }

        public void SaveStateAsUnchanged()
        {
            mLoadedState = SaveInternal();
        }

        public void LoadInternal(MapSaveState state, bool import = false)
        {
            LocalEvents.Clear();
            LocalEventsJson = state.EventData;
            Load(state.Metadata, import, false);
            LoadTileData(state.Tiles);
            AttributeData = state.Attributes;
        }

        public MapSaveState SaveInternal()
        {
            return new MapSaveState(JsonData, GenerateTileData(), AttributeData, LocalEventsJson);
        }

        public byte[] GenerateTileData()
        {
            return LZ4.PickleString(JsonConvert.SerializeObject(Layers, Formatting.None, mJsonSerializerSettings));
        }

        public bool Changed()
        {
            if (mLoadedState != null)
            {
                var newData = SaveInternal();

                return !newData.Matches(mLoadedState);
            }

            return true;
        }

        //Attribute/Animations
        public Animation GetAttributeAnimation(MapAttribute attr, Guid animId)
        {
            if (attr == null)
            {
                return null;
            }

            if (!mAttributeAnimInstances.ContainsKey(attr))
            {
                mAttributeAnimInstances.Add(attr, new Animation(AnimationBase.Get(animId), true));
            }

            return mAttributeAnimInstances[attr];
        }

        public void SetAttributeAnimation(MapAttribute attribute, Animation animationInstance)
        {
            if (mAttributeAnimInstances.ContainsKey(attribute))
            {
                mAttributeAnimInstances[attribute] = animationInstance;
            }
        }

        public override byte[] GetAttributeData()
        {
            return LZ4.PickleString(JsonConvert.SerializeObject(Attributes, Formatting.None, mJsonSerializerSettings));
        }

        public void Update()
        {
            if (Globals.MapsToScreenshot.Contains(Id))
            {
                if (Globals.MapGrid != null && Globals.MapGrid.Loaded)
                {
                    if (Globals.MapGrid.Contains(Id))
                    {
                        for (var y = Globals.CurrentMap.MapGridY + 1; y >= Globals.CurrentMap.MapGridY - 1; y--)
                        {
                            for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 &&
                                    x < Globals.MapGrid.GridWidth &&
                                    y >= 0 &&
                                    y < Globals.MapGrid.GridHeight &&
                                    Globals.MapGrid.Grid[x, y].MapId != Guid.Empty)
                                {
                                    var needMap = Lookup.Get(Globals.MapGrid.Grid[x, y].MapId);
                                    if (needMap == null)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    //We have everything, let's screenshot!
                    var prevMap = Globals.CurrentMap;
                    Globals.CurrentMap = this;
                    using (var ms = new MemoryStream())
                    {
                        lock (Graphics.GraphicsLock)
                        {
                            var screenshotTexture = Graphics.ScreenShotMap();
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
                        for (var y = Globals.CurrentMap.MapGridY + 1; y >= Globals.CurrentMap.MapGridY - 1; y--)
                        {
                            for (var x = Globals.CurrentMap.MapGridX - 1; x <= Globals.CurrentMap.MapGridX + 1; x++)
                            {
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight)
                                {
                                    if (Globals.MapGrid.Grid[x, y].MapId == Id)
                                    {
                                        return;
                                    }
                                }
                            }
                        }

                        Delete();
                    }
                }
            }
        }

        //Helper Functions
        public override MapBase[,] GenerateAutotileGrid()
        {
            var mapBase = new MapBase[3, 3];
            if (Globals.MapGrid != null && Globals.MapGrid.Contains(Id))
            {
                for (var x = -1; x <= 1; x++)
                {
                    for (var y = -1; y <= 1; y++)
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
                for (var x = -1; x <= 1; x++)
                {
                    for (var y = -1; y <= 1; y++)
                    {
                        var x1 = MapGridX + x;
                        var y1 = MapGridY + y;
                        if (x1 >= 0 && y1 >= 0 && x1 < Globals.MapGrid.GridWidth && y1 < Globals.MapGrid.GridHeight)
                        {
                            var map = Lookup.Get<MapInstance>(Globals.MapGrid.Grid[x1, y1].MapId);
                            if (map != null && map != this)
                            {
                                map.InitAutotiles();
                            }
                        }
                    }
                }
            }
        }

        public EventBase FindEventAt(int x, int y)
        {
            if (LocalEvents.Count <= 0)
            {
                return null;
            }

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
            if (Lights.Count <= 0)
            {
                return null;
            }

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
            if (Spawns.Count <= 0)
            {
                return null;
            }

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

        public override void Delete()
        {
            Lookup?.Delete(this);
        }

        public void Dispose()
        {
        }

    }

}
