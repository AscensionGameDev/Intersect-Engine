using System.Collections.Generic;
using System.Linq;
using Intersect_Editor.Classes.Entities;
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Library.GameObjects.Events;
using Intersect_Library.GameObjects.Maps;

namespace Intersect_Editor.Classes.Maps
{
    public class MapInstance : MapBase
    {
        //Map Attributes
        public new const GameObject Type = GameObject.Map;
        protected static Dictionary<int, DatabaseObject> Objects = new Dictionary<int, DatabaseObject>();
        private Dictionary<Intersect_Library.GameObjects.Maps.Attribute, AnimationInstance> _attributeAnimInstances = new Dictionary<Intersect_Library.GameObjects.Maps.Attribute, AnimationInstance>();


        public MapInstance(int mapNum) : base(mapNum, false)
        {
            Autotiles = new MapAutotiles(this);
            Autotiles.InitAutotiles(MapBase.GetObjects());
        }

        public MapInstance(MapBase mapStruct) : base(mapStruct)
        {
            Autotiles = new MapAutotiles(this);
            Autotiles.InitAutotiles(MapBase.GetObjects());
        }

        public void Load(byte[] myArr, bool import = false)
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
            Autotiles.InitAutotiles(MapBase.GetObjects());
            UpdateAdjacentAutotiles();
        }

        //Attribute/Animations
        public AnimationInstance GetAttributeAnimation(Intersect_Library.GameObjects.Maps.Attribute attr, int animNum)
        {
            if (!_attributeAnimInstances.ContainsKey(attr))
            {
                _attributeAnimInstances.Add(attr, new AnimationInstance(AnimationBase.GetAnim(animNum), true));
            }
            return _attributeAnimInstances[attr];
        }
        public void SetAttributeAnimation(Intersect_Library.GameObjects.Maps.Attribute attribute, AnimationInstance animationInstance)
        {
            if (_attributeAnimInstances.ContainsKey(attribute))
            {
                _attributeAnimInstances[attribute] = animationInstance;
            }
        }

        //Helper Functions
        public void UpdateAdjacentAutotiles()
        {
            if (GetMap(Up) != null)
            {
                for (int x = 0; x < Options.MapWidth; x++)
                {
                    for (int y = Options.MapHeight - 1; y < Options.MapHeight; y++)
                    {
                        GetMap(Up).Autotiles.UpdateAutoTiles(x, y, MapBase.GetObjects());
                    }
                }
            }
            if (GetMap(Down) != null)
            {
                for (int x = 0; x < Options.MapWidth; x++)
                {
                    for (int y = 0; y < 1; y++)
                    {
                        GetMap(Down).Autotiles.UpdateAutoTiles(x, y, MapBase.GetObjects());
                    }
                }
            }
            if (GetMap(Left) != null)
            {
                for (int x = Options.MapWidth - 1; x < Options.MapWidth; x++)
                {
                    for (int y = 0; y < Options.MapHeight; y++)
                    {
                        GetMap(Left).Autotiles.UpdateAutoTiles(x, y, MapBase.GetObjects());
                    }
                }
            }
            if (GetMap(Right) != null)
            {
                for (int x = 0; x < 1; x++)
                {
                    for (int y = 0; y < Options.MapHeight; y++)
                    {
                        GetMap(Right).Autotiles.UpdateAutoTiles(x, y, MapBase.GetObjects());
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

        public override byte[] GetData()
        {
            return GetMapData(false);
        }

        public override GameObject GetGameObjectType()
        {
            return Type;
        }

        public new static MapInstance GetMap(int index)
        {
            if (Objects.ContainsKey(index))
            {
                return (MapInstance)Objects[index];
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
            Objects.Remove(GetId());
            MapBase.GetObjects().Remove(GetId());
        }
        public static void ClearObjects()
        {
            Objects.Clear();
            MapBase.ClearObjects();
        }
        public static void AddObject(int index, DatabaseObject obj)
        {
            Objects.Add(index, obj);
            MapBase.Objects.Add(index, (MapBase)obj);
        }
        public static int ObjectCount()
        {
            return Objects.Count;
        }
        public static Dictionary<int, MapInstance> GetObjects()
        {
            Dictionary<int, MapInstance> objects = Objects.ToDictionary(k => k.Key, v => (MapInstance)v.Value);
            return objects;
        }

        public void Dispose()
        {

        }
    }
}
