using Intersect.Enums;
using Intersect.Server.Classes.Core;

namespace Intersect.Server.Classes.Maps
{
    using Database = Intersect.Server.Classes.Core.Database;

    public class TileHelper
    {
        private int mMapNum;
        private int mTileX;
        private int mTileY;

        /// <summary>
        ///     Creates a new tile helper instance in a position given.
        /// </summary>
        /// <param name="mapNum"></param>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        public TileHelper(int mapNum, int tileX, int tileY)
        {
            mMapNum = mapNum;
            mTileX = tileX;
            mTileY = tileY;
        }

        /// <summary>
        ///     Moves our tile and then attempts to adjust the map location of we walked out of bounds. Will return true if the
        ///     position is valid. False if not.
        /// </summary>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        /// <returns></returns>
        public bool Translate(int xOffset, int yOffset)
        {
            mTileX += xOffset;
            mTileY += yOffset;
            return TryFix();
        }

        public bool TryFix()
        {
            int oldTileX = mTileX;
            int oldTileY = mTileY;
            if (Fix()) return true;
            mTileX = oldTileX;
            mTileY = oldTileY;
            return false;
        }

        public bool Matches(TileHelper other)
        {
            if (GetMap() == other.GetMap() && GetX() == other.GetX() && GetY() == other.GetY()) return true;
            return false;
        }

        private bool TransitionMaps(int direction)
        {
            if (!MapInstance.Lookup.IndexKeys.Contains(mMapNum)) return false;
            int grid = MapInstance.Lookup.Get<MapInstance>(mMapNum).MapGrid;
            int gridX = MapInstance.Lookup.Get<MapInstance>(mMapNum).MapGridX;
            int gridY = MapInstance.Lookup.Get<MapInstance>(mMapNum).MapGridY;
            switch (direction)
            {
                case (int) Directions.Up:
                    if (gridY > 0 && Database.MapGrids[grid].MyGrid[gridX, gridY - 1] > -1)
                    {
                        mMapNum = Database.MapGrids[grid].MyGrid[gridX, gridY - 1];
                        mTileY += Options.MapHeight;
                        return true;
                    }
                    return false;
                case (int) Directions.Down:
                    if (gridY + 1 < Database.MapGrids[grid].Height &&
                        Database.MapGrids[grid].MyGrid[gridX, gridY + 1] > -1)
                    {
                        mMapNum = Database.MapGrids[grid].MyGrid[gridX, gridY + 1];
                        mTileY -= Options.MapHeight;
                        return true;
                    }
                    return false;
                case (int) Directions.Left:
                    if (gridX > 0 && Database.MapGrids[grid].MyGrid[gridX - 1, gridY] > -1)
                    {
                        mMapNum = Database.MapGrids[grid].MyGrid[gridX - 1, gridY];
                        mTileX += Options.MapWidth;
                        return true;
                    }
                    return false;
                case (int) Directions.Right:
                    if (gridX + 1 < Database.MapGrids[grid].Width &&
                        Database.MapGrids[grid].MyGrid[gridX + 1, gridY] > -1)
                    {
                        mMapNum = Database.MapGrids[grid].MyGrid[gridX + 1, gridY];
                        mTileX -= Options.MapWidth;
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        private bool Fix()
        {
            if (!MapInstance.Lookup.IndexKeys.Contains(mMapNum)) return false;
            MapInstance curMap = MapInstance.Lookup.Get<MapInstance>(mMapNum);
            while (mTileX < 0)
            {
                if (!TransitionMaps((int) Directions.Left)) return false;
            }
            while (mTileY < 0)
            {
                if (!TransitionMaps((int) Directions.Up)) return false;
            }
            while (mTileX >= Options.MapWidth)
            {
                if (!TransitionMaps((int) Directions.Right)) return false;
            }
            while (mTileY >= Options.MapHeight)
            {
                if (!TransitionMaps((int) Directions.Down)) return false;
            }
            return true;
        }

        public int GetMap()
        {
            return mMapNum;
        }

        public int GetX()
        {
            return mTileX;
        }

        public int GetY()
        {
            return mTileY;
        }

        public static bool IsTileValid(int mapNum, int tileX, int tileY)
        {
            if (!MapInstance.Lookup.IndexKeys.Contains(mapNum)) return false;
            if (tileX < 0 || tileX >= Options.MapWidth) return false;
            if (tileY < 0 || tileY >= Options.MapHeight) return false;
            return true;
        }
    }
}