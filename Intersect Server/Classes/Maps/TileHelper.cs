using System;
using Intersect_Library;
using Intersect_Server.Classes.Core;
using Intersect_Server.Classes.General;


namespace Intersect_Server.Classes.Maps
{
    public class TileHelper
    {
        private int _mapNum;
        private int _tileX;
        private int _tileY;

        /// <summary>
        /// Creates a new tile helper instance in a position given.
        /// </summary>
        /// <param name="mapNum"></param>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        public TileHelper(int mapNum, int tileX, int tileY)
        {
            _mapNum = mapNum;
            _tileX = tileX;
            _tileY = tileY;
        }

        /// <summary>
        /// Moves our tile and then attempts to adjust the map location of we walked out of bounds. Will return true if the position is valid. False if not.
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <returns></returns>
        public bool Translate(int xOffset, int yOffset)
        {
            _tileX += xOffset;
            _tileY += yOffset;
            return TryFix();
        }

        public bool TryFix()
        {
            int oldTileX = _tileX;
            int oldTileY = _tileY;
            if (Fix()) return true;
            _tileX = oldTileX;
            _tileY = oldTileY;
            return false;
        }

        private bool TransitionMaps(int direction)
        {
            if (!Globals.GameMaps.ContainsKey(_mapNum)) return false;
            int Grid = Globals.GameMaps[_mapNum].MapGrid;
            int GridX = Globals.GameMaps[_mapNum].MapGridX;
            int GridY = Globals.GameMaps[_mapNum].MapGridY;
            switch (direction)
            {
                case (int)Directions.Up:
                    if (GridY > 0 && Database.MapGrids[Grid].MyGrid[GridX, GridY-1] > -1)
                    {
                        _mapNum = Database.MapGrids[Grid].MyGrid[GridX, GridY - 1];
                        _tileY += Options.MapHeight;
                        return true;
                    }
                    return false;
                case (int)Directions.Down:
                    if (GridY + 1 < Database.MapGrids[Grid].Height && Database.MapGrids[Grid].MyGrid[GridX, GridY + 1] > -1)
                    {
                        _mapNum = Database.MapGrids[Grid].MyGrid[GridX, GridY + 1];
                        _tileY -= Options.MapHeight;
                        return true;
                    }
                    return false;
                case (int)Directions.Left:
                    if (GridX > 0 && Database.MapGrids[Grid].MyGrid[GridX - 1, GridY] > -1)
                    {
                        _mapNum = Database.MapGrids[Grid].MyGrid[GridX - 1, GridY];
                        _tileX += Options.MapWidth;
                        return true;
                    }
                    return false;
                case (int)Directions.Right:
                    if (GridX + 1 < Database.MapGrids[Grid].Width && Database.MapGrids[Grid].MyGrid[GridX + 1, GridY] > -1)
                    {
                        _mapNum = Database.MapGrids[Grid].MyGrid[GridX + 1, GridY];
                        _tileX -= Options.MapWidth;
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        private bool Fix()
        {
            if (!Globals.GameMaps.ContainsKey(_mapNum)) return false;
            MapInstance curMap = Globals.GameMaps[_mapNum];
            while (_tileX < 0)
            {
                if (!TransitionMaps((int) Directions.Left)) return false;
            }
            while (_tileY < 0)
            {
                if (!TransitionMaps((int)Directions.Up)) return false;
            }
            while (_tileX >= Options.MapWidth)
            {
                if (!TransitionMaps((int)Directions.Right)) return false;
            }
            while (_tileY >= Options.MapHeight)
            {
                if (!TransitionMaps((int)Directions.Down)) return false;
            }
            return true;
        }

        public int GetMap()
        {
            return _mapNum;
        }

        public int GetX()
        {
            return _tileX;
        }

        public int GetY()
        {
            return _tileY;
        }

        public static Boolean IsTileValid(int mapNum, int tileX, int tileY)
        {
            if (!Globals.GameMaps.ContainsKey(mapNum)) return false;
            if (tileX < 0 || tileX > Options.MapWidth) return false;
            if (tileY < 0 || tileY > Options.MapHeight) return false;
            return true;
        }
    }
}
