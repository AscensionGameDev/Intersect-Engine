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
using System.Threading;
using System.Threading.Tasks;
using Intersect_Library;
using Intersect_Server.Classes.Core;
using Intersect_Server.Classes.Entities;
using Intersect_Server.Classes.Maps;
using Intersect_Server.Classes.Misc.Pathfinding;
using Intersect_Server.Classes.General;

namespace Intersect_Server.Classes.Misc
{
    public class Pathfinder
    {
        private Entity _sourceEntity;
        private List<int> _directions = new List<int>();
        private bool _pathFinding = false;
        private PathfinderTarget _target;
        private object _targetLock = new object();
        private Task _pathFindingTask;
        private int _failures = 0;
        private long _pathfingingTime = 0;
        public Pathfinder(Entity sourceEntity)
        {
            _sourceEntity = sourceEntity;
            _target = null;
            _pathFindingTask = new Task(() => PathFind());
        }

        public PathfinderTarget GetTarget()
        {
            return _target;
        }

        public void SetTarget(PathfinderTarget target)
        {
            lock (_targetLock)
            {
                _directions.Clear();
                _target = target;
                if (_pathFindingTask.IsCompleted)
                {
                    _pathFindingTask.Dispose();
                    _pathFindingTask = new Task(() => PathFind());
                    _pathfingingTime = Globals.System.GetTimeMs() +
                                       Globals.Rand.Next(_failures*1000 + 500, _failures*2000 + 500);
                }
                else
                {
                    if (_pathFindingTask.Status == TaskStatus.Created && Globals.System.GetTimeMs() > _pathfingingTime) _pathFindingTask.Start();
                }
            }
        }

        public int GetMove()
        {
            if (_directions.Count > 0)
            {
                return _directions[0];
            }
            return -1;
        }

        public void RemoveMove()
        {
            if (_directions.Count > 0)
            {
                _directions.RemoveAt(0);
            }
        }

        private void PathFind()
        {
                var targetX = -1;
                var targetY = -1;
                var startX = -1;
                var startY = -1;
                _directions = new List<int>();
                var openList = new List<PathfinderPoint>();
                var closedList = new List<PathfinderPoint>();
                var adjSquares = new List<PathfinderPoint>();
                var map = MapInstance.GetMap(_sourceEntity.CurrentMap);
                if (map != null)
                {
                    var myGrid = map.MapGrid;
                    _pathFinding = true;
                    var start = Globals.System.GetTimeMs();
                    var foundPath = false;
                    if (_target != null)
                    {
                        var target = _target;
                        //Loop through surrouding maps to generate a array of open and blocked points.
                        for (var x = MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridX - 1; x <= MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridX + 1; x++)
                        {
                            if (x == -1 || x >= Database.MapGrids[myGrid].Width) continue;
                            for (var y = MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridY - 1; y <= MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridY + 1; y++)
                            {
                                if (y == -1 || y >= Database.MapGrids[myGrid].Height) continue;
                                if (Database.MapGrids[myGrid].MyGrid[x, y] > -1)
                                {
                                    var mapEntities = MapInstance.GetMap(Database.MapGrids[myGrid].MyGrid[x, y]).GetEntities();
                                    for (var i = 0; i < mapEntities.Count; i++)
                                    {
                                        lock (_targetLock)
                                        {
                                            if (_target != null)
                                            {
                                                if (mapEntities[i] != null && mapEntities[i].GetType() != typeof(Projectile))
                                                {
                                                    if (i != _sourceEntity.MyIndex && (mapEntities[i].CurrentMap != _target.TargetMap || mapEntities[i].CurrentY != _target.TargetY || mapEntities[i].CurrentX != _target.TargetX))
                                                    {
                                                        if (mapEntities[i] != null)
                                                        {
                                                            if (mapEntities[i].CurrentMap == Database.MapGrids[myGrid].MyGrid[x, y])
                                                            {
                                                                closedList.Add(
                                                                    new PathfinderPoint(
                                                                        (x - MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridX + 1) *
                                                                        Options.MapWidth + mapEntities[i].CurrentX,
                                                                        (y - MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridY + 1) *
                                                                        Options.MapHeight + mapEntities[i].CurrentY, -1, 0));
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    for (var x1 = 0; x1 < Options.MapWidth; x1++)
                                    {
                                        for (var y1 = 0; y1 < Options.MapHeight; y1++)
                                        {
                                            lock (_targetLock)
                                            {
                                                if (_target != null)
                                                {
                                                    if (MapInstance.GetMap(Database.MapGrids[myGrid].MyGrid[x, y]).Attributes[x1, y1] != null)
                                                    {
                                                        if (
                                                            MapInstance.GetMap(Database.MapGrids[myGrid].MyGrid[x, y]).Attributes[x1, y1]
                                                                .value == (int)MapAttributes.Blocked ||
                                                            MapInstance.GetMap(Database.MapGrids[myGrid].MyGrid[x, y]).Attributes[x1, y1]
                                                                .value == (int)MapAttributes.NPCAvoid)
                                                        {
                                                            closedList.Add(
                                                                new PathfinderPoint(
                                                                    (x - MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridX + 1) *
                                                                    Options.MapWidth + x1,
                                                                    (y - MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridY + 1) *
                                                                    Options.MapHeight + y1, -1, 0));
                                                        }
                                                    }
                                                    if (Database.MapGrids[myGrid].MyGrid[x, y] == _target.TargetMap && x1 == _target.TargetX && y1 == _target.TargetY)
                                                    {
                                                        targetX = (x - MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridX + 1) *
                                                                  Options.MapWidth + x1;
                                                        targetY = (y - MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridY + 1) *
                                                                  Options.MapHeight + y1;
                                                    }
                                                    if (Database.MapGrids[myGrid].MyGrid[x, y] == _sourceEntity.CurrentMap &&
                                                        x1 == _sourceEntity.CurrentX && y1 == _sourceEntity.CurrentY)
                                                    {
                                                        startX = (x - MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridX + 1) *
                                                                 Options.MapWidth + x1;
                                                        startY = (y - MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridY + 1) *
                                                                 Options.MapHeight + y1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (var x1 = 0; x1 < Options.MapWidth; x1++)
                                    {
                                        for (var y1 = 0; y1 < Options.MapHeight; y1++)
                                        {
                                            closedList.Add(new PathfinderPoint((x - MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridX + 1) * Options.MapWidth + x1, (y - MapInstance.GetMap(_sourceEntity.CurrentMap).MapGridY + 1) * Options.MapHeight + y1, -1, 0));
                                        }
                                    }
                                }
                            }
                        }

                        if (startX > -1 && targetX > -1)
                        {
                            openList.Add(new PathfinderPoint(startX, startY, 0, 0));
                            do
                            {
                                var currentTile = FindLowestF(openList);
                                closedList.Add(currentTile);
                                openList.Remove(currentTile);

                                foreach (PathfinderPoint t in closedList)
                                {
                                    if (t.X != targetX || t.Y != targetY) continue;
                                    //path found
                                    currentTile = t;
                                    while (currentTile.G > 0)
                                    {
                                        foreach (var t1 in closedList)
                                        {
                                            if (t1.G != currentTile.G - 1) continue;
                                            if (currentTile.X > t1.X)
                                            {
                                                _directions.Insert(0, 3);
                                            }
                                            else if (currentTile.X < t1.X)
                                            {
                                                _directions.Insert(0, 2);
                                            }
                                            else if (currentTile.Y > t1.Y)
                                            {
                                                _directions.Insert(0, 1);
                                            }
                                            else if (currentTile.Y < t1.Y)
                                            {
                                                _directions.Insert(0, 0);
                                            }
                                            currentTile = t1;
                                            break;
                                        }
                                    }
                                    foundPath = true;
                                    _failures = 0;
                                }

                                adjSquares.Clear();
                                if (currentTile.X > 0)
                                {
                                    adjSquares.Add(new PathfinderPoint(currentTile.X - 1, currentTile.Y, 0, 0));
                                }
                                if (currentTile.Y > 0)
                                {
                                    adjSquares.Add(new PathfinderPoint(currentTile.X, currentTile.Y - 1, 0, 0));
                                }
                                if (currentTile.X < 89)
                                {
                                    adjSquares.Add(new PathfinderPoint(currentTile.X + 1, currentTile.Y, 0, 0));
                                }
                                if (currentTile.Y < 89)
                                {
                                    adjSquares.Add(new PathfinderPoint(currentTile.X, currentTile.Y + 1, 0, 0));
                                }

                                foreach (var t in adjSquares)
                                {
                                    for (var x = 0; x < closedList.Count; x++)
                                    {
                                        if (closedList[x].X == t.X && closedList[x].Y == t.Y)
                                        {
                                            //Continue - already in the closed list
                                            break;
                                        }
                                        if (x != closedList.Count - 1) continue;
                                        if (openList.Count > 0)
                                        {
                                            //If not in the closed list then add or update the open list
                                            for (var y = 0; y < openList.Count; y++)
                                            {
                                                if (openList[y].X == t.X && openList[y].Y == t.Y)
                                                {
                                                    //Update if the PathfinderPoints are better
                                                    var newPathfinderPoint = new PathfinderPoint(t.X, t.Y, currentTile.G + 1, Math.Abs(targetX - t.X) + Math.Abs(targetY - t.Y));
                                                    if (newPathfinderPoint.F < openList[y].F)
                                                    {
                                                        openList.RemoveAt(y);
                                                        openList.Add(newPathfinderPoint);
                                                    }
                                                    break;
                                                }
                                                if (y == openList.Count - 1)
                                                {
                                                    //add to the open list
                                                    openList.Add(new PathfinderPoint(t.X, t.Y, currentTile.G + 1, Math.Abs(targetX - t.X) + Math.Abs(targetY - t.Y)));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //add to the open list
                                            openList.Add(new PathfinderPoint(t.X, t.Y, currentTile.G + 1, Math.Abs(targetX - t.X) + Math.Abs(targetY - t.Y)));
                                        }
                                    }
                                }
                            } while (openList.Count > 0 && foundPath == false && target == _target);
                        }
                        _pathFinding = false;
                        _failures++;
                    }
                }
        }

        private static PathfinderPoint FindLowestF(IList<PathfinderPoint> openList)
        {
            var solution = openList[0];
            foreach (var t in openList)
            {
                if (t.F < solution.F)
                {
                    solution = t;
                }
            }
            return solution;
        }
    }
}
