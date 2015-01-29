using System;

namespace IntersectServer
{
	public class MapGrid
	{
		public int[,] myGrid;
		public int[] myMaps;
		private int[] tmpMaps;
		private int myIndex;
		public long width;
		public long height;
		public MapGrid (int startMap, int myGridIndex)
		{
			myMaps = new int[1];
			myMaps [0] = startMap;
			myIndex = myGridIndex;
			myGrid = new int[GlobalVariables.mapCount * 2 + 1, GlobalVariables.mapCount * 2 + 1];
			for (int x = 0; x < GlobalVariables.mapCount * 2 + 1; x++) {
				for (int y = 0; y < GlobalVariables.mapCount * 2 + 1; y++) {
					myGrid [x, y] = -1;
				}
			}
			width = GlobalVariables.mapCount * 2 + 1;
			height = width;
			myGrid [GlobalVariables.mapCount, GlobalVariables.mapCount] = startMap;
			GlobalVariables.GameMaps [startMap].mapGrid = myGridIndex;
			GlobalVariables.GameMaps [startMap].mapGridX = GlobalVariables.mapCount;
			GlobalVariables.GameMaps [startMap].mapGridY = GlobalVariables.mapCount;
			if (GlobalVariables.GameMaps [startMap].up > -1) {
				addMap (GlobalVariables.GameMaps [startMap].up, GlobalVariables.mapCount, GlobalVariables.mapCount - 1);
			}
			if (GlobalVariables.GameMaps [startMap].down > -1) {
				addMap (GlobalVariables.GameMaps [startMap].down, GlobalVariables.mapCount, GlobalVariables.mapCount + 1);
			}
			if (GlobalVariables.GameMaps [startMap].left > -1) {
				addMap (GlobalVariables.GameMaps [startMap].left, GlobalVariables.mapCount - 1, GlobalVariables.mapCount);
			}
			if (GlobalVariables.GameMaps [startMap].right > -1) {
				addMap (GlobalVariables.GameMaps [startMap].right, GlobalVariables.mapCount + 1, GlobalVariables.mapCount );
			}

		}

		private void addMap(int mapNum, int x, int y) {
			if (!hasMap (mapNum)) {
				myGrid [x,y] = mapNum;
				GlobalVariables.GameMaps [mapNum].mapGrid = myIndex;
				GlobalVariables.GameMaps [mapNum].mapGridX = x;
				GlobalVariables.GameMaps [mapNum].mapGridY = y;
				tmpMaps = (int[])myMaps.Clone();
				myMaps = new int[tmpMaps.Length + 1];
				tmpMaps.CopyTo (myMaps, 0);
				myMaps [myMaps.Length - 1] = mapNum;
				if (GlobalVariables.GameMaps [mapNum].up > -1) {
					addMap (GlobalVariables.GameMaps [mapNum].up, x, y - 1);
				}
				if (GlobalVariables.GameMaps [mapNum].down > -1) {
					addMap (GlobalVariables.GameMaps [mapNum].down, x, y + 1);
				}
				if (GlobalVariables.GameMaps [mapNum].left > -1) {
					addMap (GlobalVariables.GameMaps [mapNum].left, x - 1, y );
				}
				if (GlobalVariables.GameMaps [mapNum].right > -1) {
					addMap (GlobalVariables.GameMaps [mapNum].right, x + 1, y);
				}


			}
		}

		public bool hasMap(int mapNum) {
			if (myMaps.Length > 0) {
				for (int i = 0; i < myMaps.Length; i++) {
					if (myMaps [i] == mapNum) {
						return true;
					}
				}
			}
			return false;
		}
	}
}

