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

namespace Intersect_Server.Classes
{
    public class Npc : Entity
    {
        //Targetting
        public Entity MyTarget = null;
        readonly TargetLocation _tileTarget = null;

        //Pathfinding
        Thread _findPath;
        TargetLocation _pathFindingLocation;
        List<int> _directions = new List<int>();
        bool _pathfinding;

        //Moving
        public long LastRandomMove;
        
        //Respawn
        public long RespawnTime;

        public Npc(NpcStruct myBase) : base() {
            MyName = myBase.Name;
            MySprite = myBase.Sprite;
            myBase.Stat.CopyTo(Stat,0);
            myBase.MaxVital.CopyTo(Vital, 0);
            myBase.MaxVital.CopyTo(MaxVital, 0);
        }

        public override void Die()
        {
            base.Die();
            PacketSender.SendEntityLeave(MyIndex,0,CurrentMap);
            Globals.Entities[MyIndex] = null;
        }

        public void Update()
        {
            if (MoveTimer >= Environment.TickCount) return;
            var targetMap = -1;
            var targetX = 0;
            var targetY = 0;
            //Check if there is a target, if so, run their ass down.
            if (MyTarget != null)
            {
                targetMap = MyTarget.CurrentMap;
                targetX = MyTarget.CurrentX;
                targetY = MyTarget.CurrentY;

            }

            if (targetMap == -1) {
                if (_tileTarget != null) {

                }
            }

            if (targetMap > - 1)
            {
                //Check if target map is on one of the surrounding maps, if not then we are not even going to look.
                if (targetMap != CurrentMap)
                {
                    if (Globals.GameMaps[CurrentMap].SurroundingMaps.Count > 0)
                    {
                        for (var x = 0; x < Globals.GameMaps[CurrentMap].SurroundingMaps.Count; x++)
                        {
                            if (Globals.GameMaps[CurrentMap].SurroundingMaps[x] == targetMap)
                            {
                                break;
                            }
                            if (x == Globals.GameMaps[CurrentMap].SurroundingMaps.Count - 1)
                            {
                                targetMap = -1;
                            }
                        }
                    }
                    else
                    {
                        targetMap = -1;
                    }
                }
            }

            if (targetMap > -1){
                if (_pathFindingLocation != null)
                {
                    if (targetMap != _pathFindingLocation.TargetMap || targetX != _pathFindingLocation.TargetX || targetY != _pathFindingLocation.TargetY)
                    {
                        _pathFindingLocation = null;
                    }
                }

                if (_pathFindingLocation != null)
                {
                    if (!_pathfinding)
                    {
                        if (_directions != null)
                        {
                            if (_directions.Count > 0)
                            {
                                if (!CanMove(_directions[0]))
                                {
                                    Move(_directions[0],null);
                                    _directions.RemoveAt(0);
                                    if (_directions.Count == 0)
                                    {
                                        _pathFindingLocation = null;
                                    }
                                }
                                else
                                {
                                    _pathFindingLocation = null;
                                }
                            }
                            else
                            {
                                _pathFindingLocation = null;
                            }
                        }
                    }
                }
                else
                {
                    _pathFindingLocation = new TargetLocation(targetMap, targetX, targetY);
                    _findPath = new Thread(PathFind);
                    _findPath.Start();
                    _pathfinding = true;
                }
            }

            //Move randomly
            if (targetMap != -1) return;
            if (LastRandomMove >= Environment.TickCount) return;
            var i = Globals.Rand.Next(0, 1);
            if (i == 0)
            {
                i = Globals.Rand.Next(0, 4);
                if (!CanMove(i))
                {
                    Move(i,null);
                }
            }
            LastRandomMove = Environment.TickCount + Globals.Rand.Next(1000, 3000);
        }

        private void PathFind()
        {
            var targetX = -1;
            var targetY = -1;
            var startX = -1;
            var startY = -1;
            _directions = new List<int>();
            var openList = new List<Point>();
            var closedList = new List<Point>();
            var adjSquares = new List<Point>();
            var myGrid = Globals.GameMaps[CurrentMap].MapGrid;
            for (var x = Globals.GameMaps[CurrentMap].MapGridX - 1; x <= Globals.GameMaps[CurrentMap].MapGridX + 1; x++)
            {
                for (var y = Globals.GameMaps[CurrentMap].MapGridY - 1; y <= Globals.GameMaps[CurrentMap].MapGridY + 1; y++)
                {
                    if (Database.MapGrids[myGrid].MyGrid[x, y] > -1)
                    {
                        for (var i = 0; i < Globals.Entities.Count; i++)
                        {
                            if (Globals.Entities[i] != null)
                            {
                                if (i != MyIndex && Globals.Entities[i] != MyTarget)
                                {
                                    if (Globals.Entities[i].CurrentMap == Database.MapGrids[myGrid].MyGrid[x, y])
                                    {
                                        closedList.Add(new Point((x - Globals.GameMaps[CurrentMap].MapGridX + 1) * 30 + Globals.Entities[i].CurrentX, (y - Globals.GameMaps[CurrentMap].MapGridY + 1) * 30 + Globals.Entities[i].CurrentY, -1, 0));
                                    }
                                }
                            }
                        }
                        for (var x1 = 0; x1 < 30; x1++)
                        {
                            for (var y1 = 0; y1 < 30; y1++)
                            {
                                if (Globals.GameMaps[Database.MapGrids[myGrid].MyGrid[x, y]].Attributes[x1, y1].value == (int)Enums.MapAttributes.Blocked)
                                {
                                    closedList.Add(new Point((x - Globals.GameMaps[CurrentMap].MapGridX + 1) * 30 + x1, (y - Globals.GameMaps[CurrentMap].MapGridY + 1) * 30 + y1, -1, 0));
                                }
                                if (Database.MapGrids[myGrid].MyGrid[x, y] == _pathFindingLocation.TargetMap && x1 == _pathFindingLocation.TargetX && y1 == _pathFindingLocation.TargetY)
                                {
                                    targetX = (x - Globals.GameMaps[CurrentMap].MapGridX + 1) * 30 + x1;
                                    targetY = (y - Globals.GameMaps[CurrentMap].MapGridY + 1) * 30 + y1;
                                }
                                if (Database.MapGrids[myGrid].MyGrid[x, y] == CurrentMap && x1 == CurrentX && y1 == CurrentY)
                                {
                                    startX = (x - Globals.GameMaps[CurrentMap].MapGridX + 1) * 30 + x1;
                                    startY = (y - Globals.GameMaps[CurrentMap].MapGridY + 1) * 30 + y1;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (var x1 = 0; x1 < 30; x1++)
                        {
                            for (var y1 = 0; y1 < 30; y1++)
                            {
                                closedList.Add(new Point((x - Globals.GameMaps[CurrentMap].MapGridX + 1) * 30 + x1, (y - Globals.GameMaps[CurrentMap].MapGridY + 1) * 30 + y1,-1,0));
                            }
                        }
                    }
                }
            }

            if (startX > -1 && targetX > -1)
            {
                openList.Add(new Point(startX, startY, 0, 0));
                do
                {
                    var currentTile = FindLowestF(openList);
                    closedList.Add(currentTile);
                    openList.Remove(currentTile);

                    foreach (Point t in closedList)
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
                        _pathfinding = false;
                        return;
                    }

                    adjSquares.Clear();
                    if (currentTile.X > 0)
                    {
                        adjSquares.Add(new Point(currentTile.X - 1, currentTile.Y, 0, 0));
                    }
                    if (currentTile.Y > 0)
                    {
                        adjSquares.Add(new Point(currentTile.X , currentTile.Y - 1, 0, 0));
                    }
                    if (currentTile.X < 89)
                    {
                        adjSquares.Add(new Point(currentTile.X + 1, currentTile.Y, 0, 0));
                    }
                    if (currentTile.Y < 89)
                    {
                        adjSquares.Add(new Point(currentTile.X , currentTile.Y + 1, 0, 0));
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
                                        //Update if the points are better
                                        var newPoint = new Point(t.X, t.Y, currentTile.G + 1, Math.Abs(targetX - t.X) + Math.Abs(targetY - t.Y));
                                        if (newPoint.F < openList[y].F)
                                        {
                                            openList.RemoveAt(y);
                                            openList.Add(newPoint);
                                        }
                                        break;
                                    }
                                    if (y == openList.Count - 1)
                                    {
                                        //add to the open list
                                        openList.Add(new Point(t.X, t.Y, currentTile.G + 1, Math.Abs(targetX - t.X) + Math.Abs(targetY - t.Y)));
                                    }
                                }
                            }
                            else
                            {
                                //add to the open list
                                openList.Add(new Point(t.X, t.Y, currentTile.G + 1, Math.Abs(targetX - t.X) + Math.Abs(targetY - t.Y)));
                            }
                        }
                    }
                } while (openList.Count > 0);
            }
            _pathfinding = false;
        }

        private static Point FindLowestF(IList<Point> openList)
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

    class TargetLocation {
        public int TargetX;
        public int TargetY;
        public int TargetMap;
        public TargetLocation(int map, int x, int y) {
            TargetMap = map;
            TargetX = x;
            TargetY = y;
        }
    }

    public class Point
    {
        public int X;
        public int Y;
        public int F;
        public int H;
        public int G;
        public Point(int x, int y, int g, int h)
        {
            X = x;
            Y = y;
            G = g;
            H = h;
            F = g + h;
        }
    }
}