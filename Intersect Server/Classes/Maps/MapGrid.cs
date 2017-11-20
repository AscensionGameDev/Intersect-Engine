using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Server.Classes.Maps
{
    using Database = Intersect.Server.Classes.Core.Database;

    public class MapGrid
    {
        private readonly int mMyIndex;
        private Point mBotRight = new Point(0, 0);
        private int[] mTmpMaps;
        private Point mTopLeft = new Point(0, 0);
        public long Height;
        public int[,] MyGrid;
        public List<int> MyMaps = new List<int>();
        public long Width;
        public long XMax;
        public long XMin;
        public long YMax;
        public long YMin;

        public MapGrid(int startMap, int myGridIndex)
        {
            mMyIndex = myGridIndex;
            MapInstance.Lookup.Get<MapInstance>(startMap).MapGrid = myGridIndex;
            MapInstance.Lookup.Get<MapInstance>(startMap).MapGridX = 0;
            MapInstance.Lookup.Get<MapInstance>(startMap).MapGridY = 0;
            MyMaps.Clear();
            CalculateBounds(MapInstance.Lookup.Get<MapInstance>(startMap), 0, 0);

            Width = mBotRight.X - mTopLeft.X + 1;
            Height = mBotRight.Y - mTopLeft.Y + 1;
            int xoffset = mTopLeft.X;
            int yoffset = mTopLeft.Y;
            XMin = mTopLeft.X - xoffset;
            YMin = mTopLeft.Y - yoffset;
            XMax = mBotRight.X - xoffset + 1;
            YMax = mBotRight.Y - yoffset + 1;
            MyGrid = new int[Width, Height];
            List<int> tmpMaps = new List<int>();
            tmpMaps.AddRange(MyMaps.ToArray());
            for (var x = XMin; x < XMax; x++)
            {
                for (var y = YMin; y < YMax; y++)
                {
                    MyGrid[x, y] = -1;
                    for (int i = 0; i < tmpMaps.Count; i++)
                    {
                        if (MapInstance.Lookup.Get<MapInstance>(tmpMaps[i]).MapGridX + Math.Abs(mTopLeft.X) == x &&
                            MapInstance.Lookup.Get<MapInstance>(tmpMaps[i]).MapGridY + Math.Abs(mTopLeft.Y) == y)
                        {
                            MyGrid[x, y] = tmpMaps[i];
                            MapInstance.Lookup.Get<MapInstance>(tmpMaps[i]).MapGrid = myGridIndex;
                            MapInstance.Lookup.Get<MapInstance>(tmpMaps[i]).MapGridX = (int) x;
                            MapInstance.Lookup.Get<MapInstance>(tmpMaps[i]).MapGridY = (int) y;
                            tmpMaps.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            foreach (var s in tmpMaps)
            {
                MyMaps.Remove(s);
            }
        }

        private void CalculateBounds(MapInstance map, int x, int y)
        {
            if (HasMap(map.Index, true))
            {
                return;
            }
            MyMaps.Add(map.Index);
            map.MapGridX = x;
            map.MapGridY = y;
            if (x < mTopLeft.X)
            {
                mTopLeft.X = x;
            }
            if (y < mTopLeft.Y)
            {
                mTopLeft.Y = y;
            }
            if (x > mBotRight.X)
            {
                mBotRight.X = x;
            }
            if (y > mBotRight.Y)
            {
                mBotRight.Y = y;
            }
            if (MapInstance.Lookup.IndexKeys.Contains(map.Up) &&
                MapInstance.Lookup.Get<MapInstance>(map.Up).Down == map.Index)
            {
                CalculateBounds(MapInstance.Lookup.Get<MapInstance>(map.Up), x, y - 1);
            }
            if (MapInstance.Lookup.IndexKeys.Contains(map.Down) &&
                MapInstance.Lookup.Get<MapInstance>(map.Down).Up == map.Index)
            {
                CalculateBounds(MapInstance.Lookup.Get<MapInstance>(map.Down), x, y + 1);
            }
            if (MapInstance.Lookup.IndexKeys.Contains(map.Left) &&
                MapInstance.Lookup.Get<MapInstance>(map.Left).Right == map.Index)
            {
                CalculateBounds(MapInstance.Lookup.Get<MapInstance>(map.Left), x - 1, y);
            }
            if (MapInstance.Lookup.IndexKeys.Contains(map.Right) &&
                MapInstance.Lookup.Get<MapInstance>(map.Right).Left == map.Index)
            {
                CalculateBounds(MapInstance.Lookup.Get<MapInstance>(map.Right), x + 1, y);
            }
        }

        public bool HasMap(int mapNum, bool parent = false)
        {
            if (MyMaps.Contains(mapNum)) return true;
            if (!parent) return false;
            return Database.MapGrids.Any(t => t.HasMap(mapNum));
        }
    }
}