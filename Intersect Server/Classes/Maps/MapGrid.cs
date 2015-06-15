namespace Intersect_Server.Classes
{
	public class MapGrid
	{
		public int[,] MyGrid;
		public int[] MyMaps;
		private int[] _tmpMaps;
		private readonly int _myIndex;
		public long Width;
		public long Height;
		public MapGrid (int startMap, int myGridIndex)
		{
			MyMaps = new int[1];
			MyMaps [0] = startMap;
			_myIndex = myGridIndex;
			MyGrid = new int[Globals.MapCount * 2 + 1, Globals.MapCount * 2 + 1];
			for (var x = 0; x < Globals.MapCount * 2 + 1; x++) {
				for (var y = 0; y < Globals.MapCount * 2 + 1; y++) {
					MyGrid [x, y] = -1;
				}
			}
			Width = Globals.MapCount * 2 + 1;
			Height = Width;
			MyGrid [Globals.MapCount, Globals.MapCount] = startMap;
			Globals.GameMaps [startMap].MapGrid = myGridIndex;
			Globals.GameMaps [startMap].MapGridX = Globals.MapCount;
			Globals.GameMaps [startMap].MapGridY = Globals.MapCount;
			if (Globals.GameMaps [startMap].Up > -1) {
				AddMap (Globals.GameMaps [startMap].Up, Globals.MapCount, Globals.MapCount - 1);
			}
			if (Globals.GameMaps [startMap].Down > -1) {
				AddMap (Globals.GameMaps [startMap].Down, Globals.MapCount, Globals.MapCount + 1);
			}
			if (Globals.GameMaps [startMap].Left > -1) {
				AddMap (Globals.GameMaps [startMap].Left, Globals.MapCount - 1, Globals.MapCount);
			}
			if (Globals.GameMaps [startMap].Right > -1) {
				AddMap (Globals.GameMaps [startMap].Right, Globals.MapCount + 1, Globals.MapCount );
			}

		}

		private void AddMap(int mapNum, int x, int y) {
		    if (HasMap(mapNum)) return;
		    MyGrid [x,y] = mapNum;
		    Globals.GameMaps [mapNum].MapGrid = _myIndex;
		    Globals.GameMaps [mapNum].MapGridX = x;
		    Globals.GameMaps [mapNum].MapGridY = y;
		    _tmpMaps = (int[])MyMaps.Clone();
		    MyMaps = new int[_tmpMaps.Length + 1];
		    _tmpMaps.CopyTo (MyMaps, 0);
		    MyMaps [MyMaps.Length - 1] = mapNum;
		    if (Globals.GameMaps [mapNum].Up > -1) {
		        AddMap (Globals.GameMaps [mapNum].Up, x, y - 1);
		    }
		    if (Globals.GameMaps [mapNum].Down > -1) {
		        AddMap (Globals.GameMaps [mapNum].Down, x, y + 1);
		    }
		    if (Globals.GameMaps [mapNum].Left > -1) {
		        AddMap (Globals.GameMaps [mapNum].Left, x - 1, y );
		    }
		    if (Globals.GameMaps [mapNum].Right > -1) {
		        AddMap (Globals.GameMaps [mapNum].Right, x + 1, y);
		    }
		}

		public bool HasMap(int mapNum) {
		    if (MyMaps.Length <= 0) return false;
		    foreach (var t in MyMaps)
		    {
		        if (t == mapNum) {
		            return true;
		        }
		    }
		    return false;
		}
	}
}

