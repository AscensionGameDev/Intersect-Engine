using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intersect;
using Intersect.GameObjects.Maps;
using Intersect_Server.Classes.Core;
using Intersect_Server.Classes.Entities;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Maps;

namespace Intersect_Server.Classes.Misc.Pathfinding
{
    class Pathfinder
    {
        private Entity mEntity;
        private PathfinderTarget mTarget;
        private IEnumerable<AStarNode> mPath;

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

        public bool Update()
        {
            //TODO: Pull this out into server config :) 
            var pathfindingRange = 20; //Search as far as 20 tiles out.. maximum.
            //Do lots of logic eventually leading up to an A* pathfinding run if needed.
            var returnVal = false;
            AStarNode[,] mapGrid;
            SpatialAStar<AStarNode, Object> aStar;
            IEnumerable<AStarNode> path = mPath;
            var currentMap = MapInstance.GetMap(mEntity.CurrentMap);
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

                    //See if the target is physically within range:
                    if (Math.Abs(sourceX - targetX) + Math.Abs(sourceY - targetY) < pathfindingRange)
                    {
                        //Doing good...
                        mapGrid = new AStarNode[Options.MapWidth * 3, Options.MapHeight * 3];

                        for (int x = 0; x < Options.MapWidth * 3; x++)
                        {
                            for (int y = 0; y < Options.MapHeight * 3; y++)
                            {
                                mapGrid[x, y] = new AStarNode();
                                mapGrid[x, y].X = x;
                                mapGrid[x, y].Y = y;
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
                                    FillArea(mapGrid, ((x + 1) - gridX) * Options.MapWidth, y * Options.MapHeight, Options.MapWidth, Options.MapHeight);
                                }
                                continue;
                            }
                            for (var y = gridY - 1; y <= gridY + 1; y++)
                            {
                                if (y == -1 || y >= Database.MapGrids[myGrid].Height)
                                {
                                    FillArea(mapGrid, ((x + 1) - gridX) * Options.MapWidth, ((y + 1) - gridY) * Options.MapHeight, Options.MapWidth, Options.MapHeight);
                                    continue;
                                }

                                if (Database.MapGrids[myGrid].MyGrid[x, y] > -1)
                                {
                                    var tmpMap = MapInstance.GetMap(Database.MapGrids[myGrid].MyGrid[x, y]);
                                    if (tmpMap != null)
                                    {
                                        //Copy the cached array of tile blocks
                                        var blocks = tmpMap.GetCachedBlocks(mEntity.GetType() == typeof(Player));
                                        foreach (var block in blocks)
                                        {
                                            mapGrid[((x + 1) - gridX) * Options.MapWidth + block.X, ((y + 1) - gridY) * Options.MapHeight + block.Y].IsWall = true;
                                        }

                                        //Block of Players, Npcs, and Resources
                                        foreach (var en in tmpMap.GetEntities())
                                        {
                                            mapGrid[((x + 1) - gridX) * Options.MapWidth + en.CurrentX, ((y + 1) - gridY) * Options.MapHeight + en.CurrentY].IsWall = true;
                                        }

                                        //Block Global Events if they are not passable.
                                        foreach (var en in tmpMap.GlobalEventInstances)
                                        {
                                            if (en.Value != null)
                                            {
                                                mapGrid[((x + 1) - gridX) * Options.MapWidth + en.Value.CurrentX, ((y + 1) - gridY) * Options.MapHeight + en.Value.CurrentY].IsWall = true;
                                            }
                                        }

                                        //If this is a local event then we gotta loop through all other local events for the player
                                        if (mEntity.GetType() == typeof(EventPageInstance))
                                        {
                                            EventPageInstance ev = (EventPageInstance)mEntity;
                                            if (ev.Passable == 0 && ev.Client != null) //Make sure this is a local event
                                            {
                                                Player player = ev.Client.Entity;
                                                if (player != null)
                                                {
                                                    var playerEvents = player.MyEvents;
                                                    foreach (var evt in playerEvents)
                                                    {
                                                        if (evt != null && evt.PageInstance != null &&
                                                            evt.PageInstance.Passable == 0)
                                                        {
                                                            mapGrid[((x + 1) - gridX) * Options.MapWidth + evt.PageInstance.CurrentX, ((y + 1) - gridY) * Options.MapHeight + evt.PageInstance.CurrentY].IsWall = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //Finally done.. let's get a path from the pathfinder.
                        mapGrid[targetX, targetY].IsWall = false;
                        aStar = new SpatialAStar<AStarNode, Object>(mapGrid);
                        path = aStar.Search(new Point(sourceX, sourceY), new Point(targetX, targetY), null);
                        returnVal = true;
                    }
                }
            }
            else
            {
                mPath = null;
            }
            mPath = path;
            return returnVal;
        }

        private void FillArea(AStarNode[,] dest, int startX, int startY, int width, int height)
        {
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    dest[x, y].IsWall = true;
                }
            }
        }

        public int GetMove()
        {
            if (mPath == null) return -1;
            var enm = mPath.GetEnumerator();
            while (enm.MoveNext())
            {
                if (enm.Current.X - Options.MapWidth == mEntity.CurrentX  && enm.Current.Y - Options.MapHeight == mEntity.CurrentY )
                {
                    if (enm.MoveNext())
                    {
                        var newX = enm.Current.X - Options.MapWidth;
                        var newY = enm.Current.Y - Options.MapHeight;
                        if (mEntity.CurrentX < newX)
                        {
                            return (int) Directions.Right;
                        }
                        else if (mEntity.CurrentX > newX)
                        {
                            return (int)Directions.Left;
                        }
                        else if (mEntity.CurrentY < newY)
                        {
                            return (int)Directions.Down;
                        }
                        else if (mEntity.CurrentY > newY)
                        {
                            return (int)Directions.Up;
                        }
                    }
                }
            } 
            return -1;
        }
    }

    public class AStarNode : IPathNode<Object>
    {
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public Boolean IsWall { get; set; }

        public bool IsWalkable(Object unused)
        {
            return !IsWall;
        }
    }

    public class AStarSolver<TPathNode, TUserContext> : SpatialAStar<TPathNode, TUserContext> where TPathNode : IPathNode<TUserContext>
    {
        protected override Double Heuristic(PathNode inStart, PathNode inEnd)
        {
            return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
        }

        protected override Double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            return Heuristic(inStart, inEnd);
        }

        public AStarSolver(TPathNode[,] inGrid)
            : base(inGrid)
        {
        }
    }
}
