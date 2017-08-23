using System;
using System.Collections.Generic;
using Intersect.Enums;
using Intersect.Server.Classes.Core;
using Intersect.Server.Classes.Entities;
using Intersect.Server.Classes.Maps;

namespace Intersect.Server.Classes.Misc.Pathfinding
{
    public enum PathfinderResult
    {
        Success,
        OutOfRange,
        NoPathToTarget,
        Failure, //No Map, No Target, Who Knows?
        Wait, //Pathfinder won't run due to recent failures and trying to conserve cpu
    }

    class Pathfinder
    {
        private int mConsecutiveFails;
        private Entity mEntity;
        private IEnumerable<PathNode> mPath;
        private PathfinderTarget mTarget;
        private long mWaitTime;

        public Pathfinder(Entity parent)
        {
            mEntity = parent;
        }

        public void SetTarget(PathfinderTarget target)
        {
            mTarget = target;
        }

        public PathfinderTarget GetTarget()
        {
            return mTarget;
        }

        public PathfinderResult Update(long timeMs)
        {
            //TODO: Pull this out into server config :) 
            var pathfindingRange =
                Math.Max(Options.MapWidth, Options.MapHeight); //Search as far as 1 map out.. maximum.
            //Do lots of logic eventually leading up to an A* pathfinding run if needed.
            PathfinderResult returnVal;
            PathNode[,] mapGrid;
            SpatialAStar aStar;
            var path = mPath;
            if (mWaitTime < timeMs)
            {
                var currentMap = MapInstance.Lookup.Get<MapInstance>(mEntity.CurrentMap);
                if (currentMap != null && mTarget != null)
                {
                    var myGrid = currentMap.MapGrid;
                    var gridX = currentMap.MapGridX;
                    var gridY = currentMap.MapGridY;

                    var targetFound = false;
                    var targetX = -1;
                    var targetY = -1;
                    var sourceX = Options.MapWidth + mEntity.CurrentX;
                    var sourceY = Options.MapHeight + mEntity.CurrentY;

                    //Loop through surrouding maps to see if our target is even around.
                    for (var x = gridX - 1; x <= gridX + 1; x++)
                    {
                        if (x == -1 || x >= Database.MapGrids[myGrid].Width) continue;
                        for (var y = gridY - 1; y <= gridY + 1; y++)
                        {
                            if (y == -1 || y >= Database.MapGrids[myGrid].Height) continue;
                            if (Database.MapGrids[myGrid].MyGrid[x, y] > -1)
                            {
                                if (Database.MapGrids[myGrid].MyGrid[x, y] == mTarget.TargetMap)
                                {
                                    targetX = (x - gridX + 1) * Options.MapWidth + mTarget.TargetX;
                                    targetY = (y - gridY + 1) * Options.MapHeight + mTarget.TargetY;
                                    targetFound = true;
                                }
                            }
                        }
                    }

                    if (targetFound)
                    {
                        if (AlongPath(mPath, targetX, targetY))
                        {
                            path = mPath;
                            returnVal = PathfinderResult.Success;
                        }
                        else
                        {
                            //See if the target is physically within range:
                            if (Math.Abs(sourceX - targetX) + Math.Abs(sourceY - targetY) < pathfindingRange)
                            {
                                //Doing good...
                                mapGrid = new PathNode[Options.MapWidth * 3, Options.MapHeight * 3];

                                for (int x = 0; x < Options.MapWidth * 3; x++)
                                {
                                    for (int y = 0; y < Options.MapHeight * 3; y++)
                                    {
                                        mapGrid[x, y] = new PathNode(x, y, false);
                                        if (x < sourceX - pathfindingRange || x > sourceX + pathfindingRange ||
                                            y < sourceY - pathfindingRange || y > sourceY + pathfindingRange)
                                        {
                                            mapGrid[x, y].IsWall = true;
                                        }
                                    }
                                }

                                //loop through all surrounding maps.. gather blocking elements, resources, players, npcs, global events, and local events (if this is a local event)
                                for (var x = gridX - 1; x <= gridX + 1; x++)
                                {
                                    if (x == -1 || x >= Database.MapGrids[myGrid].Width)
                                    {
                                        for (int y = 0; y < 3; y++)
                                        {
                                            FillArea(mapGrid, ((x + 1) - gridX) * Options.MapWidth,
                                                y * Options.MapHeight,
                                                Options.MapWidth, Options.MapHeight);
                                        }
                                        continue;
                                    }
                                    for (var y = gridY - 1; y <= gridY + 1; y++)
                                    {
                                        if (y == -1 || y >= Database.MapGrids[myGrid].Height)
                                        {
                                            FillArea(mapGrid, ((x + 1) - gridX) * Options.MapWidth,
                                                ((y + 1) - gridY) * Options.MapHeight, Options.MapWidth,
                                                Options.MapHeight);
                                            continue;
                                        }

                                        if (Database.MapGrids[myGrid].MyGrid[x, y] > -1)
                                        {
                                            var tmpMap =
                                                MapInstance.Lookup.Get<MapInstance>(Database.MapGrids[myGrid]
                                                    .MyGrid[x, y]);
                                            if (tmpMap != null)
                                            {
                                                //Copy the cached array of tile blocks
                                                var blocks =
                                                    tmpMap.GetCachedBlocks(mEntity.GetType() == typeof(Player));
                                                foreach (var block in blocks)
                                                {
                                                    mapGrid[
                                                        ((x + 1) - gridX) * Options.MapWidth + block.X,
                                                        ((y + 1) - gridY) * Options.MapHeight + block.Y].IsWall = true;
                                                }

                                                //Block of Players, Npcs, and Resources
                                                foreach (var en in tmpMap.GetEntities())
                                                {
                                                    mapGrid[
                                                                ((x + 1) - gridX) * Options.MapWidth + en.CurrentX,
                                                                ((y + 1) - gridY) * Options.MapHeight + en.CurrentY]
                                                            .IsWall =
                                                        true;
                                                }

                                                //Block Global Events if they are not passable.
                                                foreach (var en in tmpMap.GlobalEventInstances)
                                                {
                                                    if (en.Value != null)
                                                    {
                                                        mapGrid[
                                                                    ((x + 1) - gridX) * Options.MapWidth +
                                                                    en.Value.CurrentX,
                                                                    ((y + 1) - gridY) * Options.MapHeight +
                                                                    en.Value.CurrentY]
                                                                .IsWall =
                                                            true;
                                                    }
                                                }

                                                //If this is a local event then we gotta loop through all other local events for the player
                                                if (mEntity.GetType() == typeof(EventPageInstance))
                                                {
                                                    EventPageInstance ev = (EventPageInstance) mEntity;
                                                    if (ev.Passable == 0 && ev.Client != null)
                                                        //Make sure this is a local event
                                                    {
                                                        Player player = ev.Client.Entity;
                                                        if (player != null)
                                                        {
                                                            if (player.EventLookup.Values.Count > Options.MapWidth *
                                                                Options.MapHeight)
                                                            {
                                                                //Find all events on this map (since events can't switch maps)
                                                                for (int mapX = 0; mapX < Options.MapWidth; mapX++)
                                                                {
                                                                    for (int mapY = 0; mapY < Options.MapHeight; mapY++)
                                                                    {
                                                                        var evt = player.EventExists(ev.CurrentMap,
                                                                            mapX, mapY);
                                                                        if (evt != null)
                                                                        {
                                                                            if (evt.PageInstance != null &&
                                                                                evt.PageInstance.Passable == 0)
                                                                            {
                                                                                mapGrid[
                                                                                    ((x + 1) - gridX) * Options
                                                                                        .MapWidth +
                                                                                    evt.CurrentX,
                                                                                    ((y + 1) - gridY) * Options
                                                                                        .MapHeight +
                                                                                    evt.CurrentY].IsWall = true;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var playerEvents = player.EventLookup.Values;
                                                                foreach (var evt in playerEvents)
                                                                {
                                                                    if (evt != null && evt.PageInstance != null &&
                                                                        evt.PageInstance.Passable == 0)
                                                                    {
                                                                        mapGrid[
                                                                            ((x + 1) - gridX) * Options.MapWidth +
                                                                            evt.PageInstance.CurrentX,
                                                                            ((y + 1) - gridY) * Options.MapHeight +
                                                                            evt.PageInstance.CurrentY].IsWall = true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //Optionally, move the along path check down here.. see if each tile is still open before returning success.
                                //That would be more processor intensive but would also provide ai that recoginize blocks in their path quicker.

                                //Finally done.. let's get a path from the pathfinder.
                                mapGrid[targetX, targetY].IsWall = false;
                                aStar = new SpatialAStar(mapGrid);
                                path = aStar.Search(new Point(sourceX, sourceY), new Point(targetX, targetY), null);
                                if (path == null)
                                {
                                    returnVal = PathfinderResult.NoPathToTarget;
                                }
                                else
                                {
                                    returnVal = PathfinderResult.Success;
                                }
                            }
                            else
                            {
                                returnVal = PathfinderResult.OutOfRange;
                            }
                        }
                    }
                    else
                    {
                        returnVal = PathfinderResult.OutOfRange;
                    }
                }
                else
                {
                    mPath = null;
                    returnVal = PathfinderResult.Failure;
                }
            }
            else
            {
                returnVal = PathfinderResult.Wait;
            }
            switch (returnVal)
            {
                case PathfinderResult.Success:
                    //Use the same path for at least a second before trying again.
                    mWaitTime = timeMs + 200;
                    mConsecutiveFails = 0;
                    break;
                case PathfinderResult.OutOfRange:
                    //Npc might immediately find a new target. Give it a 500ms wait but make this wait grow if we keep finding targets out of range.
                    mConsecutiveFails++;
                    mWaitTime = timeMs + mConsecutiveFails * 500;
                    break;
                case PathfinderResult.NoPathToTarget:
                    //Wait 2 seconds and try again. This will move the npc randomly and might allow other npcs or players to get out of the way
                    mConsecutiveFails++;
                    mWaitTime = timeMs + 1000 + mConsecutiveFails * 500;
                    break;
                case PathfinderResult.Failure:
                    //Can try again in a second.. we don't waste much processing time on failures
                    mWaitTime = timeMs + 500;
                    mConsecutiveFails = 0;
                    break;
                case PathfinderResult.Wait:
                    //Nothing to do here.. we are already waiting.
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            mPath = path;
            return returnVal;
        }

        private void FillArea(PathNode[,] dest, int startX, int startY, int width, int height)
        {
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    dest[x, y].IsWall = true;
                }
            }
        }

        public bool AlongPath(IEnumerable<PathNode> path, int x, int y)
        {
            if (path == null) return false;
            var foundUs = false;
            var enm = path.GetEnumerator();
            while (enm.MoveNext())
            {
                if (enm.Current.X - Options.MapWidth == mEntity.CurrentX &&
                    enm.Current.Y - Options.MapHeight == mEntity.CurrentY)
                {
                    foundUs = true;
                }
                if (foundUs && enm.Current.X == x)
                {
                    if (enm.Current.Y == y || enm.Current.Y - 1 == y || enm.Current.Y + 1 == y)
                    {
                        enm.Dispose();
                        return true;
                    }
                }
                if (foundUs && enm.Current.Y == y)
                {
                    if (enm.Current.X == x || enm.Current.X - 1 == x || enm.Current.X + 1 == x)
                    {
                        enm.Dispose();
                        return true;
                    }
                }
            }
            enm.Dispose();
            return false;
        }

        public void PathFailed(long timeMs)
        {
            mPath = null;
            mConsecutiveFails++;
            mWaitTime = timeMs + 1000;
        }

        public int GetMove()
        {
            if (mPath == null) return -1;
            var enm = mPath.GetEnumerator();
            while (enm.MoveNext())
            {
                if (enm.Current.X - Options.MapWidth == mEntity.CurrentX &&
                    enm.Current.Y - Options.MapHeight == mEntity.CurrentY)
                {
                    if (enm.MoveNext())
                    {
                        var newX = enm.Current.X - Options.MapWidth;
                        var newY = enm.Current.Y - Options.MapHeight;
                        if (mEntity.CurrentX < newX)
                        {
                            enm.Dispose();
                            return (int) Directions.Right;
                        }
                        else if (mEntity.CurrentX > newX)
                        {
                            enm.Dispose();
                            return (int) Directions.Left;
                        }
                        else if (mEntity.CurrentY < newY)
                        {
                            enm.Dispose();
                            return (int) Directions.Down;
                        }
                        else if (mEntity.CurrentY > newY)
                        {
                            enm.Dispose();
                            return (int) Directions.Up;
                        }
                    }
                }
            }
            enm.Dispose();
            return -1;
        }
    }

    public class AStarSolver : SpatialAStar
    {
        public AStarSolver(PathNode[,] inGrid) : base(inGrid)
        {
        }

        protected override double Heuristic(PathNode inStart, PathNode inEnd)
        {
            return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
        }

        protected override double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            return Heuristic(inStart, inEnd);
        }
    }
}