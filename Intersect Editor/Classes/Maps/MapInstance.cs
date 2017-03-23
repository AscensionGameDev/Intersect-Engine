using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Intersect;
using Intersect.GameObjects;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;
using Intersect_Editor.Classes.Entities;

namespace Intersect_Editor.Classes.Maps
{
    public class MapInstance : MapBase
    {
        //Map Attributes
        public new const GameObject OBJECT_TYPE = GameObject.Map;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();
        private static object objectsLock = new object();

        private Dictionary<Attribute, AnimationInstance> _attributeAnimInstances =
            new Dictionary<Attribute, AnimationInstance>();

        private byte[] loadedData;

        public MapInstance(int mapNum) : base(mapNum, false)
        {
            Autotiles = new MapAutotiles(this);
        }

        public MapInstance(MapBase mapStruct) : base(mapStruct)
        {
            Autotiles = new MapAutotiles(this);
            if (typeof(MapInstance) == mapStruct.GetType())
            {
                MapGridX = ((MapInstance) mapStruct).MapGridX;
                MapGridY = ((MapInstance) mapStruct).MapGridY;
            }
            InitAutotiles();
        }

        //World Position
        public int MapGridX { get; set; }
        public int MapGridY { get; set; }

        public void Load(byte[] myArr, bool import = false)
        {
            var up = Up;
            var down = Down;
            var left = Left;
            var right = Right;
            loadedData = myArr;
            base.Load(myArr);
            if (import)
            {
                Up = up;
                Down = down;
                Left = left;
                Right = right;
            }
        }

        public bool Changed()
        {
            if (loadedData != null)
            {
                var newData = GetMapData(false);
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
                _attributeAnimInstances.Add(attr, new AnimationInstance(AnimationBase.GetAnim(animNum), true));
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
                                if (x >= 0 && x < Globals.MapGrid.GridWidth && y >= 0 && y < Globals.MapGrid.GridHeight &&
                                    Globals.MapGrid.Grid[x, y].mapnum > -1)
                                {
                                    var needMap = GetMap(Globals.MapGrid.Grid[x, y].mapnum);
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
                                    if (Globals.MapGrid.Grid[x, y].mapnum == Id) return;
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
                                mapBase[x + 1, y + 1] = GetMap(Globals.MapGrid.Grid[x1, y1].mapnum);
                            }
                        }
                    }
                }
            }
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
                            var map = GetMap(Globals.MapGrid.Grid[x1, y1].mapnum);
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

        public override GameObject GameObjectType
        {
            get { return OBJECT_TYPE; }
        }

        public new static MapInstance GetMap(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (MapInstance) Objects[index];
            }
            return null;
        }

        public static DatabaseObject Get(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return Objects[index];
            }
            return null;
        }

        public override void Delete()
        {
            lock (objectsLock)
            {
                Objects.Remove(Id);
                MapBase.GetObjects().Remove(Id);
            }
        }

        public static void ClearObjects()
        {
            lock (objectsLock)
            {
                Objects.Clear();
                MapBase.ClearObjects();
            }
        }

        public static void AddObject(int index, DatabaseObject obj)
        {
            lock (objectsLock)
            {
                Objects.Add(index, obj);
                MapBase.Objects.Add(index, (MapBase) obj);
            }
        }

        public static int ObjectCount()
        {
            return Objects.Count;
        }

        public static Dictionary<int, MapInstance> GetObjects()
        {
            lock (objectsLock)
            {
                Dictionary<int, MapInstance> objects = Objects.ToDictionary(k => k.Key, v => (MapInstance) v.Value);
                return objects;
            }
        }

        public void Dispose()
        {
        }
    }
}