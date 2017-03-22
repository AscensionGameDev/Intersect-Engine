

using System;
using System.Collections.Generic;
using Intersect_Library;
using Intersect_Server.Classes.Core;

namespace Intersect_Server.Classes.Maps
{
	public class MapGrid
	{
		public int[,] MyGrid;
		public List<int> MyMaps = new List<int>();
		private int[] _tmpMaps;
		private readonly int _myIndex;
        private Point _topLeft = new Point(0, 0);
        private Point _botRight = new Point(0, 0);
        public long Width;
        public long Height;
        public long XMin;
        public long YMin;
        public long XMax;
        public long YMax;
		public MapGrid (int startMap, int myGridIndex)
		{
			_myIndex = myGridIndex;
            MapInstance.GetMap(startMap).MapGrid = myGridIndex;
            MapInstance.GetMap(startMap).MapGridX = 0;
            MapInstance.GetMap(startMap).MapGridY = 0;
            MyMaps.Clear();
            CalculateBounds(MapInstance.GetMap(startMap), 0, 0);

            Width = _botRight.X - _topLeft.X + 1;
            Height = _botRight.Y - _topLeft.Y + 1;
            int xoffset = _topLeft.X;
            int yoffset = _topLeft.Y;
            XMin = _topLeft.X - xoffset;
            YMin = _topLeft.Y - yoffset;
            XMax = _botRight.X - xoffset + 1;
            YMax = _botRight.Y - yoffset + 1;
            MyGrid = new int[Width,Height];
		    List<int> tmpMaps = new List<int>();
            tmpMaps.AddRange(MyMaps.ToArray());
            for (var x = XMin; x < XMax; x++)
            {
				for (var y = YMin; y < YMax; y++) {
                    MyGrid[x, y] = -1;
                    for (int i = 0; i < tmpMaps.Count; i++)
                    {
                        if (MapInstance.GetMap(tmpMaps[i]).MapGridX + Math.Abs(_topLeft.X) == x && MapInstance.GetMap(tmpMaps[i]).MapGridY + Math.Abs(_topLeft.Y) == y)
                        {
                            MyGrid[x, y] = tmpMaps[i];
                            MapInstance.GetMap(tmpMaps[i]).MapGrid = myGridIndex;
                            MapInstance.GetMap(tmpMaps[i]).MapGridX = (int)x;
                            MapInstance.GetMap(tmpMaps[i]).MapGridY = (int)y;
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
            if (HasMap(map.Id,true)) { return; }
            MyMaps.Add(map.Id);
            map.MapGridX = x;
            map.MapGridY = y;
            if (x < _topLeft.X) {_topLeft.X = x;}
            if (y < _topLeft.Y) {_topLeft.Y = y;}
            if (x > _botRight.X) {_botRight.X = x;}
            if (y > _botRight.Y) { _botRight.Y = y;}
            if (MapInstance.GetObjects().ContainsKey(map.Up) && MapInstance.GetMap(map.Up).Down == map.Id)
            {
                CalculateBounds(MapInstance.GetMap(map.Up), x, y - 1);
            }
            if (MapInstance.GetObjects().ContainsKey(map.Down) && MapInstance.GetMap(map.Down).Up == map.Id)
            {
                CalculateBounds(MapInstance.GetMap(map.Down), x, y + 1);
            }
            if (MapInstance.GetObjects().ContainsKey(map.Left) && MapInstance.GetMap(map.Left).Right == map.Id)
            {
                CalculateBounds(MapInstance.GetMap(map.Left), x - 1, y);
            }
            if (MapInstance.GetObjects().ContainsKey(map.Right) && MapInstance.GetMap(map.Right).Left == map.Id)
            {
                CalculateBounds(MapInstance.GetMap(map.Right), x + 1, y);
            }
        }

		public bool HasMap(int mapNum, bool parent = false)
		{
		    bool has = false;
		    has = MyMaps.Contains(mapNum);
		    if (has) return true;
		    if (parent)
		    {
		        for (int i = 0; i < Database.MapGrids.Count; i++)
		        {
		            if (Database.MapGrids[i].HasMap(mapNum))
		            {
		                return true;
		            }
		        }
		    }
		    return has;
		}
	}
}

