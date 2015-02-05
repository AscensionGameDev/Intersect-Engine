using System;

namespace Intersect_Server.Classes
{

    public class Entity
    {
        //Core Values
        public int MyIndex;
        public string MyName = "";
        public string MySprite = "";
        public int IsEvent = 0;
        public int Passable = 0;
        public int HideName = 0;

        //Location Info
        public int CurrentX;
        public int CurrentY;
        public int CurrentMap = -1;
        public int Dir;
        
        //Vitals & Stats
        public int[] MaxVital = new int[(int)Enums.Vitals.VitalCount];
        public int[] Vital = new int[(int)Enums.Vitals.VitalCount];
        public int[] Stat = new int[(int)Enums.Stats.StatCount];

        public long MoveTimer;
        

        public Entity(int index)
        {
            MyIndex = index;
            //HP
            MaxVital[(int)Enums.Vitals.Health] = 100;
            Vital[(int)Enums.Vitals.Health] = 100;
            //MP
            MaxVital[(int)Enums.Vitals.Health] = 100;
            Vital[(int)Enums.Vitals.Health] = 100;
            //ATK
            Stat[(int)Enums.Stats.Attack] = 23;
            //Ability
            Stat[(int)Enums.Stats.AbilityPower] = 16;
            //Def
            Stat[(int)Enums.Stats.Defense] = 23;
            //MR
            Stat[(int)Enums.Stats.MagicResist] = 16;
            //SPD
            Stat[(int)Enums.Stats.Speed] = 20;

        }

        public bool CanMove(int moveDir)
        {
            var tmpX = CurrentX;
            var tmpY = CurrentY;
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
            var tmpMap = CurrentMap;
            try
            {
                if (tmpX < 0)
                {
                    tmpX = 29 - (tmpX * -1);
                    if (tmpY < 0)
                    {
                        tmpY = 29 - (tmpY * -1);
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY - 1] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY - 1]].Blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY - 1];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (tmpY > 29)
                    {
                        tmpY = tmpY - 29;
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY + 1] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY + 1]].Blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY + 1];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY]].Blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY];
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
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY - 1] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY - 1]].Blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY - 1];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (tmpY > 29)
                    {
                        tmpY = tmpY - 29;
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY + 1] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY + 1]].Blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY + 1];
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY]].Blocked[tmpX, tmpY] == 1)
                            {
                                return true;
                            }
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY];
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
                    if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY - 1] > -1)
                    {
                        if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY - 1]].Blocked[tmpX, tmpY] == 1)
                        {
                            return true;

                        }
                        tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY - 1];
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (tmpY > 29)
                {
                    tmpY = tmpY - 29;
                    if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY + 1] > -1)
                    {
                        if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY + 1]].Blocked[tmpX, tmpY] == 1)
                        {
                            return true;
                        }
                        tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY + 1];
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY] > -1)
                    {
                        if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY]].Blocked[tmpX, tmpY] == 1)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                foreach (Entity t in Globals.Entities)
                {
                    if (t == null) continue;
                    if (t.CurrentMap == tmpMap && t.CurrentX == tmpX && t.CurrentY == tmpY)
                    {
                        return true;
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
            var tmpX = CurrentX;
            var tmpY = CurrentY;
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
            var tmpMap = CurrentMap;
            try
            {
                if (tmpX < 0)
                {
                    tmpX = 29 - (tmpX * -1);
                    if (tmpY < 0)
                    {
                        tmpY = 29 - (tmpY * -1);
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY - 1] > -1)
                        {
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY - 1];
                        }
                        else
                        {
                            return ;
                        }
                    }
                    else if (tmpY > 29)
                    {
                        tmpY = tmpY - 29;
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY + 1] > -1)
                        {
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY + 1];
                        }
                        else
                        {
                            return ;
                        }
                    }
                    else
                    {
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY] > -1)
                        {
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY];
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
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY - 1] > -1)
                        {
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY - 1];
                        }
                        else
                        {
                            return ;
                        }
                    }
                    else if (tmpY > 29)
                    {
                        tmpY = tmpY - 29;
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY + 1] > -1)
                        {
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY + 1];
                        }
                        else
                        {
                            return ;
                        }
                    }
                    else
                    {
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY] > -1)
                        {
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY];
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
                    if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY - 1] > -1)
                    {
                        tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY - 1];
                    }
                    else
                    {
                        return ;
                    }
                }
                else if (tmpY > 29)
                {
                    tmpY = tmpY - 29;
                    if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY + 1] > -1)
                    {
                        tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY + 1];
                    }
                    else
                    {
                        return ;
                    }
                }
                CurrentX = tmpX;
                CurrentY = tmpY;
                CurrentMap = tmpMap;
                Dir = moveDir;
                if (client == null)
                {
                    PacketSender.SendEntityMove(MyIndex, IsEvent, this);
                }
                else
                {
                    PacketSender.SendEntityMoveTo(client,MyIndex, IsEvent, this);
                }
                if (Stat[2] == 0) { Stat[2] = 1; }
                MoveTimer = Environment.TickCount + (int)((1.0 / (Stat[2] / 10f)) * 1000);
            }
            catch (Exception)
            {
                //ignore
            }
        }

        public void TryAttack(int enemyIndex)
        {
            if (Globals.Entities[enemyIndex] == null) return;
            if (!IsOneBlockAway(enemyIndex)) return;
            //No Matter what, if we attack the entitiy, make them chase us
            if (Globals.Entities[enemyIndex].GetType() == typeof(Npc))
            {
                ((Npc)Globals.Entities[enemyIndex]).MyTarget = this;
            }
            var dmg = Globals.Entities[enemyIndex].Stat[(int)Enums.Stats.Defense] - Stat[(int)Enums.Stats.Attack];
            if (dmg >= 0)
            {
                //Did nothing
            }
            else
            {
                Globals.Entities[enemyIndex].Vital[(int)Enums.Vitals.Health] += dmg;
                if (Globals.Entities[enemyIndex].Vital[(int)Enums.Vitals.Health] <= 0)
                {
                    //Dead entity
                    Globals.Entities[enemyIndex].Die();
                }
                else
                {
                    //Hit him, make him mad and send the vital update.
                    PacketSender.SendEntityVitals(enemyIndex,0, Globals.Entities[enemyIndex]);
                }
            }
        }

        bool IsOneBlockAway(int enemyIndex)
        {
            //TODO
            return true;
        }

        public void ChangeDir(int dir)
        {
            Dir = dir;
            PacketSender.SendEntityDir(MyIndex,IsEvent);
        }

        public virtual void Die()
        {

        }

        public void Reset()
        {
            for (var i = 0; i < (int) Enums.Vitals.VitalCount; i++)
            {
                Vital[i] = MaxVital[i];
            }
        }

        public virtual void Warp(int newMap, int newX, int newY)
        {
            Warp(newMap, newX, newY, Dir);
        }

        public virtual void Warp(int newMap, int newX, int newY, int newDir)
        {
            
        }
    }

}

