using System;

namespace IntersectServer
{

    public class Entity
    {
        //Core Values
        public int myIndex;
        public string myName = "";
        public string mySprite = "";
        public int isEvent = 0;
        public int passable = 0;
        public int hideName = 0;

        //Location Info
        public int currentX = 0;
        public int currentY = 0;
        public int currentMap = -1;
        public int dir = 0;
        
        //Vitals & Stats
        public int[] maxVital = new int[2];
        public int[] vital = new int[2];
        public int[] stat = new int[3];

        public long moveTimer;
        

        public Entity(int index)
        {
            myIndex = index;
            //HP
            maxVital[0] = 100;
            vital[0] = 100;
            //MP
            maxVital[1] = 100;
            vital[1] = 100;
            //ATK
            stat[0] = 23;
            //DEF
            stat[1] = 16;
            //SPD
            stat[2] = 20;

        }

        public bool CanMove(int moveDir)
        {
            int tmpX = currentX;
            int tmpY = currentY;
            switch (moveDir)
            {
                case 0:
                    tmpY--;
                    break;
                case 1:
                    tmpY++;
                    break;
                case 2:
                    tmpX--;
                    break;
                case 3:
                    tmpX++;
                    break;
            }
            int tmpMap = currentMap;
            try
            {
                if (tmpX < 0)
                {
                    tmpX = 29 - (tmpX * -1);
                    if (tmpY < 0)
                    {
                        tmpY = 29 - (tmpY * -1);
                        if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY - 1] > -1)
                        {
                            if (GlobalVariables.GameMaps[Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY - 1]].blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY - 1];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (tmpY > 29)
                    {
                        tmpY = tmpY - 29;
                        if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY + 1] > -1)
                        {
                            if (GlobalVariables.GameMaps[Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY + 1]].blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY + 1];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY] > -1)
                        {
                            if (GlobalVariables.GameMaps[Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY]].blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY];
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else if (tmpX > 29)
                {
                    tmpX = tmpX - 29;
                    if (tmpY < 0)
                    {
                        tmpY = 29 - (tmpY * -1);
                        if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY - 1] > -1)
                        {
                            if (GlobalVariables.GameMaps[Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY - 1]].blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY - 1];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (tmpY > 29)
                    {
                        tmpY = tmpY - 29;
                        if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY + 1] > -1)
                        {
                            if (GlobalVariables.GameMaps[Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY + 1]].blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY + 1];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY] > -1)
                        {
                            if (GlobalVariables.GameMaps[Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY]].blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY];
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else if (tmpY < 0)
                {
                    tmpY = 29 - (tmpY * -1);
                    if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX, GlobalVariables.GameMaps[tmpMap].mapGridY - 1] > -1)
                    {
                        if (GlobalVariables.GameMaps[Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX, GlobalVariables.GameMaps[tmpMap].mapGridY - 1]].blocked[tmpX, tmpY] == 1)
                        {
                            return true;

                        }
                        tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX, GlobalVariables.GameMaps[tmpMap].mapGridY - 1];
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (tmpY > 29)
                {
                    tmpY = tmpY - 29;
                    if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX, GlobalVariables.GameMaps[tmpMap].mapGridY + 1] > -1)
                    {
                        if (GlobalVariables.GameMaps[Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX, GlobalVariables.GameMaps[tmpMap].mapGridY + 1]].blocked[tmpX, tmpY] == 1)
                        {
                            return true;
                        }
                        tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX, GlobalVariables.GameMaps[tmpMap].mapGridY + 1];
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX, GlobalVariables.GameMaps[tmpMap].mapGridY] > -1)
                    {
                        if (GlobalVariables.GameMaps[Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX, GlobalVariables.GameMaps[tmpMap].mapGridY]].blocked[tmpX, tmpY] == 1)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                for (int i = 0; i < GlobalVariables.entities.Count; i++)
                {
                    if (GlobalVariables.entities[i] != null)
                    {
                        if (GlobalVariables.entities[i].currentMap == tmpMap && GlobalVariables.entities[i].currentX == tmpX && GlobalVariables.entities[i].currentY == tmpY)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return true;

            }
        }

        public void Move(int moveDir, Client client)
        {
            int tmpX = currentX;
            int tmpY = currentY;
            switch (moveDir)
            {
                case 0:
                    tmpY--;
                    break;
                case 1:
                    tmpY++;
                    break;
                case 2:
                    tmpX--;
                    break;
                case 3:
                    tmpX++;
                    break;
            }
            int tmpMap = currentMap;
            try
            {
                if (tmpX < 0)
                {
                    tmpX = 29 - (tmpX * -1);
                    if (tmpY < 0)
                    {
                        tmpY = 29 - (tmpY * -1);
                        if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY - 1] > -1)
                        {
                            tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY - 1];
                        }
                        else
                        {
                            return ;
                        }
                    }
                    else if (tmpY > 29)
                    {
                        tmpY = tmpY - 29;
                        if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY + 1] > -1)
                        {
                            tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY + 1];
                        }
                        else
                        {
                            return ;
                        }
                    }
                    else
                    {
                        if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY] > -1)
                        {
                            tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX - 1, GlobalVariables.GameMaps[tmpMap].mapGridY];
                        }
                        else
                        {
                            return ;
                        }
                    }
                }
                else if (tmpX > 29)
                {
                    tmpX = tmpX - 29;
                    if (tmpY < 0)
                    {
                        tmpY = 29 - (tmpY * -1);
                        if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY - 1] > -1)
                        {
                            tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY - 1];
                        }
                        else
                        {
                            return ;
                        }
                    }
                    else if (tmpY > 29)
                    {
                        tmpY = tmpY - 29;
                        if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY + 1] > -1)
                        {
                            tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY + 1];
                        }
                        else
                        {
                            return ;
                        }
                    }
                    else
                    {
                        if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY] > -1)
                        {
                            tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX + 1, GlobalVariables.GameMaps[tmpMap].mapGridY];
                        }
                        else
                        {
                            return ;
                        }
                    }
                }
                else if (tmpY < 0)
                {
                    tmpY = 29 - (tmpY * -1);
                    if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX, GlobalVariables.GameMaps[tmpMap].mapGridY - 1] > -1)
                    {
                        tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX, GlobalVariables.GameMaps[tmpMap].mapGridY - 1];
                    }
                    else
                    {
                        return ;
                    }
                }
                else if (tmpY > 29)
                {
                    tmpY = tmpY - 29;
                    if (Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX, GlobalVariables.GameMaps[tmpMap].mapGridY + 1] > -1)
                    {
                        tmpMap = Database.mapGrids[GlobalVariables.GameMaps[tmpMap].mapGrid].myGrid[GlobalVariables.GameMaps[tmpMap].mapGridX, GlobalVariables.GameMaps[tmpMap].mapGridY + 1];
                    }
                    else
                    {
                        return ;
                    }
                }
                currentX = tmpX;
                currentY = tmpY;
                currentMap = tmpMap;
                dir = moveDir;
                if (client == null)
                {
                    PacketSender.SendEntityMove(myIndex, isEvent, this);
                }
                else
                {
                    PacketSender.SendEntityMoveTo(client,myIndex, isEvent, this);
                }
                if (stat[2] == 0) { stat[2] = 1; }
                moveTimer = Environment.TickCount + (int)((1.0 / (stat[2] / 10)) * 1000);
            }
            catch
            {
                return ;

            }
        }

        public void TryAttack(int enemyIndex)
        {
            int dmg = 0;
            if (GlobalVariables.entities[enemyIndex] != null) { 
                if (isOneBlockAway(enemyIndex))
                {
                    //No Matter what, if we attack the entitiy, make them chase us
                    if (GlobalVariables.entities[enemyIndex].GetType() == typeof(NPC))
                    {
                        ((NPC)GlobalVariables.entities[enemyIndex]).myTarget = this;
                    }
                    dmg = GlobalVariables.entities[enemyIndex].stat[1] - stat[0];
                    if (dmg >= 0)
                    {
                        //Did nothing
                    }
                    else
                    {
                        GlobalVariables.entities[enemyIndex].vital[0] += dmg;
                        if (GlobalVariables.entities[enemyIndex].vital[0] <= 0)
                        {
                            //Dead entity
                            GlobalVariables.entities[enemyIndex].Die();
                        }
                        else
                        {
                            //Hit him, make him mad and send the vital update.
                            PacketSender.SendEntityVitals(enemyIndex,0, GlobalVariables.entities[enemyIndex]);
                        }
                    }
                }
            }
        }

        bool isOneBlockAway(int enemyIndex)
        {

            return true;
        }

        public void ChangeDir(int dir)
        {
            this.dir = dir;
            PacketSender.SendEntityDir(myIndex,isEvent);
        }

        public virtual void Die()
        {

        }

        public void Reset()
        {
            vital[0] = maxVital[0];
            vital[1] = maxVital[1];
        }

        public virtual void Warp(int newMap, int newX, int newY)
        {
            Warp(newMap, newX, newY, dir);
        }

        public virtual void Warp(int newMap, int newX, int newY, int newDir)
        {
            
        }
    }

}

