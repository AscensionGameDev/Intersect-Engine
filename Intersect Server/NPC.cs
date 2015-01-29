using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace IntersectServer
{
    class NPC : Entity
    {
        //Targetting
        public Entity myTarget = null;
        TargetLocation tileTarget = null;

        //Pathfinding
        Thread findPath;
        TargetLocation pathFindingLocation = null;
        List<int> directions = new List<int>();
        bool pathfinding = false;

        //Moving
        public long lastRandomMove;

        public NPC(int index): base(index)
        {

        }

        public NPC(int index, NPCBase myBase) : base(index) {
            base.myName = myBase.myName;
            base.mySprite = myBase.mySprite;
            myBase.stat.CopyTo(base.stat,0);
            myBase.vital.CopyTo(base.vital, 0);
            myBase.maxVital.CopyTo(base.maxVital, 0);
        }

        public override void Die()
        {
            base.Die();
            PacketSender.SendEntityLeave(myIndex,0);
            GlobalVariables.entities[myIndex] = null;
        }

        public void Update()
        {
            int i;
            if (base.moveTimer < Environment.TickCount)
            {
                int targetMap = -1;
                int targetX = 0;
                int targetY = 0;
                //Check if there is a target, if so, run their ass down.
                if (myTarget != null)
                {
                    targetMap = myTarget.currentMap;
                    targetX = myTarget.currentX;
                    targetY = myTarget.currentY;

                }

                if (targetMap == -1) {
                    if (tileTarget != null) {

                    }
                }

                if (targetMap > - 1)
                {
                    //Check if target map is on one of the surrounding maps, if not then we are not even going to look.
                    if (targetMap != currentMap)
                    {
                        if (GlobalVariables.GameMaps[currentMap].surroundingMaps.Count > 0)
                        {
                            for (int x = 0; x < GlobalVariables.GameMaps[currentMap].surroundingMaps.Count; x++)
                            {
                                if (GlobalVariables.GameMaps[currentMap].surroundingMaps[x] == targetMap)
                                {
                                    break;
                                }
                                else
                                {
                                    if (x == GlobalVariables.GameMaps[currentMap].surroundingMaps.Count - 1)
                                    {
                                        targetMap = -1;
                                    }
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
                    if (pathFindingLocation != null)
                    {
                        if (targetMap != pathFindingLocation.targetMap || targetX != pathFindingLocation.targetX || targetY != pathFindingLocation.targetY)
                        {
                            pathFindingLocation = null;
                        }
                    }

                    if (pathFindingLocation != null)
                    {
                        if (!pathfinding)
                        {
                            if (directions != null)
                            {
                                if (directions.Count > 0)
                                {
                                    if (!base.CanMove(directions[0]))
                                    {
                                        base.Move(directions[0],null);
                                        directions.RemoveAt(0);
                                        if (directions.Count == 0)
                                        {
                                            pathFindingLocation = null;
                                        }
                                    }
                                    else
                                    {
                                        pathFindingLocation = null;
                                    }
                                }
                                else
                                {
                                    pathFindingLocation = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        pathFindingLocation = new TargetLocation(targetMap, targetX, targetY);
                        findPath = new Thread(() => pathFind());
                        findPath.Start();
                        pathfinding = true;
                    }
                }

                //Move randomly
                if (targetMap == -1)
                {
                    if (lastRandomMove < Environment.TickCount)
                    {
                        i = GlobalVariables.rand.Next(0, 1);
                        if (i == 0)
                        {
                            i = GlobalVariables.rand.Next(0, 4);
                            if (!base.CanMove(i))
                            {
                                base.Move(i,null);
                            }
                        }
                        lastRandomMove = Environment.TickCount + GlobalVariables.rand.Next(1000, 3000);
                    }
                }
            }
        }

        private void pathFind()
        {
            int targetX = -1;
            int targetY = -1;
            int startX = -1;
            int startY = -1;
            Point currentTile;
            directions = new List<int>();
            List<Point> openList = new List<Point>();
            List<Point> closedList = new List<Point>();
            List<Point> adjSquares = new List<Point>();
            int myGrid = GlobalVariables.GameMaps[currentMap].mapGrid;
            for (int x = GlobalVariables.GameMaps[currentMap].mapGridX - 1; x <= GlobalVariables.GameMaps[currentMap].mapGridX + 1; x++)
            {
                for (int y = GlobalVariables.GameMaps[currentMap].mapGridY - 1; y <= GlobalVariables.GameMaps[currentMap].mapGridY + 1; y++)
                {
                        if (Database.mapGrids[myGrid].myGrid[x, y] > -1)
                        {
                            for (int i = 0; i < GlobalVariables.entities.Count; i++)
                            {
                                if (GlobalVariables.entities[i] != null)
                                {
                                    if (i != base.myIndex && GlobalVariables.entities[i] != myTarget)
                                    {
                                        if (GlobalVariables.entities[i].currentMap == Database.mapGrids[myGrid].myGrid[x, y])
                                        {
                                            closedList.Add(new Point((x - GlobalVariables.GameMaps[currentMap].mapGridX + 1) * 30 + GlobalVariables.entities[i].currentX, (y - GlobalVariables.GameMaps[currentMap].mapGridY + 1) * 30 + GlobalVariables.entities[i].currentY, -1, 0));
                                        }
                                    }
                                }
                            }
                                for (int x1 = 0; x1 < 30; x1++)
                                {
                                    for (int y1 = 0; y1 < 30; y1++)
                                    {
                                        if (GlobalVariables.GameMaps[Database.mapGrids[myGrid].myGrid[x, y]].blocked[x1, y1] == 1)
                                        {
                                            closedList.Add(new Point((x - GlobalVariables.GameMaps[currentMap].mapGridX + 1) * 30 + x1, (y - GlobalVariables.GameMaps[currentMap].mapGridY + 1) * 30 + y1, -1, 0));
                                        }
                                        if (Database.mapGrids[myGrid].myGrid[x, y] == pathFindingLocation.targetMap && x1 == pathFindingLocation.targetX && y1 == pathFindingLocation.targetY)
                                        {
                                            targetX = (x - GlobalVariables.GameMaps[currentMap].mapGridX + 1) * 30 + x1;
                                            targetY = (y - GlobalVariables.GameMaps[currentMap].mapGridY + 1) * 30 + y1;
                                        }
                                        if (Database.mapGrids[myGrid].myGrid[x, y] == currentMap && x1 == currentX && y1 == currentY)
                                        {
                                            startX = (x - GlobalVariables.GameMaps[currentMap].mapGridX + 1) * 30 + x1;
                                            startY = (y - GlobalVariables.GameMaps[currentMap].mapGridY + 1) * 30 + y1;
                                        }
                                    }
                                }
                        }
                        else
                        {
                            for (int x1 = 0; x1 < 30; x1++)
                            {
                                for (int y1 = 0; y1 < 30; y1++)
                                {
                                    closedList.Add(new Point((x - GlobalVariables.GameMaps[currentMap].mapGridX + 1) * 30 + x1, (y - GlobalVariables.GameMaps[currentMap].mapGridY + 1) * 30 + y1,-1,0));
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
                    currentTile = findLowestF(openList);
                    closedList.Add(currentTile);
                    openList.Remove(currentTile);

                    for (int i = 0; i < closedList.Count; i++)
                    {
                        if (closedList[i].x == targetX && closedList[i].y == targetY)
                        {
                            //path found
                            System.Diagnostics.Debug.Print("Found a path!!!");
                            currentTile = closedList[i];
                            while (currentTile.g > 0)
                            {
                                for (int x = 0; x < closedList.Count; x++)
                                {
                                    if (closedList[x].g == currentTile.g - 1)
                                    {
                                        if (currentTile.x > closedList[x].x)
                                        {
                                            directions.Insert(0, 3);
                                        }
                                        else if (currentTile.x < closedList[x].x)
                                        {
                                            directions.Insert(0, 2);
                                        }
                                        else if (currentTile.y > closedList[x].y)
                                        {
                                            directions.Insert(0, 1);
                                        }
                                        else if (currentTile.y < closedList[x].y)
                                        {
                                            directions.Insert(0, 0);
                                        }
                                        currentTile = closedList[x];
                                        break;
                                    }
                                }
                            }
                            pathfinding = false;
                            return;
                        }
                    }

                    adjSquares.Clear();
                    if (currentTile.x > 0)
                    {
                        adjSquares.Add(new Point(currentTile.x - 1, currentTile.y, 0, 0));
                    }
                    if (currentTile.y > 0)
                    {
                        adjSquares.Add(new Point(currentTile.x , currentTile.y - 1, 0, 0));
                    }
                    if (currentTile.x < 89)
                    {
                        adjSquares.Add(new Point(currentTile.x + 1, currentTile.y, 0, 0));
                    }
                    if (currentTile.y < 89)
                    {
                        adjSquares.Add(new Point(currentTile.x , currentTile.y + 1, 0, 0));
                    }

                    for (int i = 0; i < adjSquares.Count; i++)
                    {
                        for (int x = 0; x < closedList.Count; x++)
                        {
                            if (closedList[x].x == adjSquares[i].x && closedList[x].y == adjSquares[i].y)
                            {
                                //Continue - already in the closed list
                                break;
                            }
                            else
                            {
                                if (x == closedList.Count - 1)
                                {
                                    if (openList.Count > 0)
                                    {
                                        //If not in the closed list then add or update the open list
                                        for (int y = 0; y < openList.Count; y++)
                                        {
                                            if (openList[y].x == adjSquares[i].x && openList[y].y == adjSquares[i].y)
                                            {
                                                //Update if the points are better
                                                Point newPoint = new Point(adjSquares[i].x, adjSquares[i].y, currentTile.g + 1, Math.Abs(targetX - adjSquares[i].x) + Math.Abs(targetY - adjSquares[i].y));
                                                if (newPoint.f < openList[y].f)
                                                {
                                                    openList.RemoveAt(y);
                                                    openList.Add(newPoint);
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                if (y == openList.Count - 1)
                                                {
                                                    //add to the open list
                                                    openList.Add(new Point(adjSquares[i].x, adjSquares[i].y, currentTile.g + 1, Math.Abs(targetX - adjSquares[i].x) + Math.Abs(targetY - adjSquares[i].y)));
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //add to the open list
                                        openList.Add(new Point(adjSquares[i].x, adjSquares[i].y, currentTile.g + 1, Math.Abs(targetX - adjSquares[i].x) + Math.Abs(targetY - adjSquares[i].y)));
                                    }
                                }
                            }
                        }
                    }



                } while (openList.Count > 0);
            }
            pathfinding = false;
        }

        private Point findLowestF(List<Point> openList)
        {
            Point solution = openList[0];
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f < solution.f)
                {
                    solution = openList[i];
                }   
            }
                return solution;
        }
    }
}

class TargetLocation {
    public int targetX;
    public int targetY;
    public int targetMap;
    public TargetLocation(int map, int x, int y) {
        targetMap = map;
        targetX = x;
        targetY = y;
    }
}

public class Point
{
    public int x;
    public int y;
    public int f;
    public int h;
    public int g;
    public Point(int x, int y, int g, int h)
    {
        this.x = x;
        this.y = y;
        this.g = g;
        this.h = h;
        this.f = g + h;
    }
}
