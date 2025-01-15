using Intersect.Enums;
using Intersect.Server.Database;

namespace Intersect.Server.Maps;


public partial class TileHelper
{

    private Guid mMapId;

    private int mTileX;

    private int mTileY;

    /// <summary>
    ///     Creates a new tile helper instance in a position given.
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="tileX"></param>
    /// <param name="tileY"></param>
    public TileHelper(Guid mapId, int tileX, int tileY)
    {
        mMapId = mapId;
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

        return TryFix(xOffset != 0 && yOffset != 0);
    }

    public bool TryFix(bool diagonal = false)
    {
        var oldTileX = mTileX;
        var oldTileY = mTileY;
        if (Fix(diagonal))
        {
            return true;
        }

        mTileX = oldTileX;
        mTileY = oldTileY;

        return false;
    }

    public bool Matches(TileHelper other)
    {
        if (GetMapId() == other.GetMapId() && GetX() == other.GetX() && GetY() == other.GetY())
        {
            return true;
        }

        return false;
    }

    private bool TransitionMaps(int direction)
    {
        var map = MapController.Get(mMapId);
        if (map == null)
        {
            return false;
        }

        var grid = DbInterface.GetGrid(map.MapGrid);
        if (grid == null)
        {
            return false;
        }

        var gridX = map.MapGridX;
        var gridY = map.MapGridY;
        switch (direction)
        {
            case (int) Direction.Up:
                if (gridY > 0 && grid.MapIdGrid[gridX, gridY - 1] != Guid.Empty)
                {
                    mMapId = grid.MapIdGrid[gridX, gridY - 1];
                    mTileY += Options.MapHeight;

                    return true;
                }

                return false;
            case (int) Direction.Down:
                if (gridY + 1 < grid.Height && grid.MapIdGrid[gridX, gridY + 1] != Guid.Empty)
                {
                    mMapId = grid.MapIdGrid[gridX, gridY + 1];
                    mTileY -= Options.MapHeight;

                    return true;
                }

                return false;
            case (int) Direction.Left:
                if (gridX > 0 && grid.MapIdGrid[gridX - 1, gridY] != Guid.Empty)
                {
                    mMapId = grid.MapIdGrid[gridX - 1, gridY];
                    mTileX += Options.MapWidth;

                    return true;
                }

                return false;
            case (int) Direction.Right:
                if (gridX + 1 < grid.Width && grid.MapIdGrid[gridX + 1, gridY] != Guid.Empty)
                {
                    mMapId = grid.MapIdGrid[gridX + 1, gridY];
                    mTileX -= Options.MapWidth;

                    return true;
                }

                return false;
            default:
                return false;
        }
    }

    private bool Fix(bool diagonal = false)
    {
        if (!MapController.Lookup.Keys.Contains(mMapId))
        {
            return false;
        }

        var transitionFailure = false;

        while (mTileX < 0)
        {
            if (TransitionMaps((int)Direction.Left))
            {
                continue;
            }

            if (!diagonal)
            {
                return false;
            }

            transitionFailure = true;
        }

        while (mTileY < 0)
        {
            if (TransitionMaps((int)Direction.Up))
            {
                continue;
            }

            if (!diagonal)
            {
                return false;
            }

            transitionFailure = true;
        }

        while (mTileX >= Options.MapWidth)
        {
            if (TransitionMaps((int)Direction.Right))
            {
                continue;
            }

            if (!diagonal)
            {
                return false;
            }

            transitionFailure = true;
        }

        while (mTileY >= Options.MapHeight)
        {
            if (TransitionMaps((int)Direction.Down))
            {
                continue;
            }

            if (!diagonal)
            {
                return false;
            }

            transitionFailure = true;
        }

        return !transitionFailure || Fix(false);
    }

    public Guid GetMapId()
    {
        return mMapId;
    }

    public MapController GetMap()
    {
        return MapController.Get(mMapId);
    }

    public byte GetX()
    {
        return (byte) mTileX;
    }

    public byte GetY()
    {
        return (byte) mTileY;
    }

    public static bool IsTileValid(Guid mapId, int tileX, int tileY)
    {
        if (!MapController.Lookup.Keys.Contains(mapId))
        {
            return false;
        }

        if (tileX < 0 || tileX >= Options.MapWidth)
        {
            return false;
        }

        if (tileY < 0 || tileY >= Options.MapHeight)
        {
            return false;
        }

        return true;
    }

}
