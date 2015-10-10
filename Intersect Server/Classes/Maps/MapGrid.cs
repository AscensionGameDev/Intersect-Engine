/*
    Intersect Game Engine (Server)
    Copyright (C) 2015  JC Snider, Joe Bridges
    
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com 

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
namespace Intersect_Server.Classes
{
	public class MapGrid
	{
		public int[,] MyGrid;
		public List<int> MyMaps = new List<int>();
		private int[] _tmpMaps;
		private readonly int _myIndex;
        private System.Drawing.Point _topLeft = new System.Drawing.Point(0, 0);
        private System.Drawing.Point _botRight = new System.Drawing.Point(0, 0);
        public long Width;
        public long Height;
        public long XMin;
        public long YMin;
        public long XMax;
        public long YMax;
		public MapGrid (int startMap, int myGridIndex)
		{
			_myIndex = myGridIndex;
            Globals.GameMaps[startMap].MapGrid = myGridIndex;
            Globals.GameMaps[startMap].MapGridX = 0;
            Globals.GameMaps[startMap].MapGridY = 0;
            MyMaps.Clear();
            CalculateBounds(Globals.GameMaps[startMap], 0, 0);

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
                        if (Globals.GameMaps[tmpMaps[i]].MapGridX + Math.Abs(_topLeft.X) == x && Globals.GameMaps[tmpMaps[i]].MapGridY + Math.Abs(_topLeft.Y) == y)
                        {
                            MyGrid[x, y] = tmpMaps[i];
                            Globals.GameMaps[tmpMaps[i]].MapGrid = myGridIndex;
                            Globals.GameMaps[tmpMaps[i]].MapGridX = (int)x;
                            Globals.GameMaps[tmpMaps[i]].MapGridY = (int)y;
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

        private void CalculateBounds(MapStruct map, int x, int y)
        {
            if (HasMap(map.MyMapNum,true)) { return; }
            if (map.Deleted > 0) { return; }
            MyMaps.Add(map.MyMapNum);
            map.MapGridX = x;
            map.MapGridY = y;
            if (x < _topLeft.X) {_topLeft.X = x;}
            if (y < _topLeft.Y) {_topLeft.Y = y;}
            if (x > _botRight.X) {_botRight.X = x;}
            if (y > _botRight.Y) { _botRight.Y = y;}
            if (map.Up > -1 && map.Up < Globals.GameMaps.Length && Globals.GameMaps[map.Up] != null && Globals.GameMaps[map.Up].Deleted == 0 && Globals.GameMaps[map.Up].Down == map.MyMapNum)
            {
                CalculateBounds(Globals.GameMaps[map.Up], x, y - 1);
            }
            if (map.Down > -1 && map.Down < Globals.GameMaps.Length && Globals.GameMaps[map.Down] != null && Globals.GameMaps[map.Down].Deleted == 0 && Globals.GameMaps[map.Down].Up == map.MyMapNum)
            {
                CalculateBounds(Globals.GameMaps[map.Down], x, y + 1);
            }
            if (map.Left > -1 && map.Left < Globals.GameMaps.Length && Globals.GameMaps[map.Left] != null && Globals.GameMaps[map.Left].Deleted == 0 && Globals.GameMaps[map.Left].Right == map.MyMapNum)
            {
                CalculateBounds(Globals.GameMaps[map.Left], x - 1, y);
            }
            if (map.Right > -1 && map.Right < Globals.GameMaps.Length && Globals.GameMaps[map.Right] != null && Globals.GameMaps[map.Right].Deleted == 0 && Globals.GameMaps[map.Right].Left == map.MyMapNum)
            {
                CalculateBounds(Globals.GameMaps[map.Right], x + 1, y);
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

