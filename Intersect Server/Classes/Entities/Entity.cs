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

namespace Intersect_Server.Classes
{

    public class Entity
    {
        //Core Values
        public int MyIndex;
        public string MyName = "";
        public string MySprite = "";
        public int Passable = 0;
        public int HideName = 0;

        //Extras
        public string Face = "";

        //Location Info
        public int CurrentX;
        public int CurrentY;
        public int CurrentZ = 0;
        public int CurrentMap = -1;
        public int Dir;
        
        //Vitals & Stats
        public int[] MaxVital = new int[(int)Enums.Vitals.VitalCount];
        public int[] Vital = new int[(int)Enums.Vitals.VitalCount];
        public int[] Stat = new int[(int)Enums.Stats.StatCount];

        //Combat Status
        public long CastTime = 0;
        public int SpellCastSlot = 0;

        //Inventory
        public ItemInstance[] Inventory = new ItemInstance[Constants.MaxInvItems];

        //Spells
        public SpellInstance[] Spells = new SpellInstance[Constants.MaxPlayerSkills];

        public long MoveTimer;
        
        //Initialization
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

            for (int i = 0; i < Constants.MaxInvItems; i++)
            {
                Inventory[i] = new ItemInstance();
            }
        }

        //Movement
        public int CanMove(int moveDir)
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
                case 4:
                    tmpY--;
                    tmpX--;
                    break;
                case 5:
                    tmpY--;
                    tmpX++;
                    break;
                case 6:
                    tmpY++;
                    tmpX--;
                    break;
                case 7:
                    tmpY++;
                    tmpX++;
                    break;
            }
            var tmpMap = CurrentMap;
            try
            {
                if (tmpX < 0)
                {
                    tmpX = (Globals.MapWidth - 1) - (tmpX * -1);
                    if (tmpY < 0)
                    {
                        tmpY = (Globals.MapHeight - 1) - (tmpY * -1);
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY - 1] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY - 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return 1;
                            }
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY - 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.NPCAvoid && this.GetType() == typeof(Npc))
                            {
                                return 1;
                            }
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY - 1];
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else if (tmpY > (Globals.MapHeight - 1))
                    {
                        tmpY = tmpY - (Globals.MapHeight - 1);
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY + 1] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY + 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return 1;
                            }
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY + 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.NPCAvoid && this.GetType() == typeof(Npc))
                            {
                                return 1;
                            }
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY + 1];
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return 1;
                            }
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.NPCAvoid && this.GetType() == typeof(Npc))
                            {
                                return 1;
                            }
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY];
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
                else if (tmpX > (Globals.MapWidth - 1))
                {
                    tmpX = tmpX - (Globals.MapWidth - 1);
                    if (tmpY < 0)
                    {
                        tmpY = (Globals.MapHeight - 1) - (tmpY * -1);
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY - 1] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY - 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return 1;
                            }
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY - 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.NPCAvoid && this.GetType() == typeof(Npc))
                            {
                                return 1;
                            }
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY - 1];
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else if (tmpY > (Globals.MapHeight - 1))
                    {
                        tmpY = tmpY - (Globals.MapHeight - 1);
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY + 1] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY + 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return 1;
                            }
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY + 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.NPCAvoid && this.GetType() == typeof(Npc))
                            {
                                return 1;
                            }
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY + 1];
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY] > -1)
                        {
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                            {
                                return 1;
                            }
                            if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.NPCAvoid && this.GetType() == typeof(Npc))
                            {
                                return 1;
                            }
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY];
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
                else if (tmpY < 0)
                {
                    tmpY = (Globals.MapHeight - 1) - (tmpY * -1);
                    if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY - 1] > -1)
                    {
                        if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY - 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                        {
                            return 1;
                        }
                        if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY - 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.NPCAvoid && this.GetType() == typeof(Npc))
                        {
                            return 1;
                        }
                        tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY - 1];
                    }
                    else
                    {
                        return 1;
                    }
                }
                else if (tmpY > (Globals.MapHeight - 1))
                {
                    tmpY = tmpY - (Globals.MapHeight - 1);
                    if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY + 1] > -1)
                    {
                        if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY + 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                        {
                            return 1;
                        }
                        if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY + 1]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.NPCAvoid && this.GetType() == typeof(Npc))
                        {
                            return 1;
                        }
                        tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY + 1];
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY] > -1)
                    {
                        if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.Blocked)
                        {
                            return 1;
                        }
                        if (Globals.GameMaps[Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY]].Attributes[tmpX, tmpY].value == (int)Enums.MapAttributes.NPCAvoid && this.GetType() == typeof(Npc))
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        return 1;
                    }
                }
                foreach (Entity t in Globals.Entities)
                {
                    if (t == null) continue;
                    if (t.CurrentMap == tmpMap && t.CurrentX == tmpX && t.CurrentY == tmpY && t.CurrentZ == CurrentZ && t.Passable == 0)
                    {
                        //Set a target if a projectile
                        if (this.GetType() == typeof(Projectile))
                        {
                            ((Projectile)this).Target = t.MyIndex;
                        }
                        if (t.GetType() == typeof(Player))
                        {
                            return 2;
                        }
                        else if (t.GetType() == typeof(Npc))
                        {
                            return 3;
                        }
                        else if (t.GetType() == typeof(Resource))
                        {
                            return 4;
                        }
                        else if (t.GetType() == typeof(Event))
                        {
                            return 5;
                        }
                    }
                }
                return 0;
            }
            catch
            {
                return 1;

            }
        }
        public void Move(int moveDir, Client client)
        {
            var tmpX = CurrentX;
            var tmpY = CurrentY;
            switch (moveDir)
            {
                case 0: //Up
                    tmpY--;
                    break;
                case 1: //Down
                    tmpY++;
                    break;
                case 2: //Left
                    tmpX--;
                    break;
                case 3: //Right
                    tmpX++;
                    break;
                case 4: //UpLeft
                    tmpY--;
                    tmpX--;
                    break;
                case 5: //UpRight
                    tmpY--;
                    tmpX++;
                    break;
                case 6: //DownLeft
                    tmpY++;
                    tmpX--;
                    break;
                case 7: //DownRight
                    tmpY++;
                    tmpX++;
                    break;
            }
            var tmpMap = CurrentMap;
            try
            {
                if (tmpX < 0)
                {
                    tmpX = (Globals.MapWidth - 1) - (tmpX * -1);
                    if (tmpY < 0)
                    {
                        tmpY = (Globals.MapHeight - 1) - (tmpY * -1);
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY - 1] > -1)
                        {
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX - 1, Globals.GameMaps[tmpMap].MapGridY - 1];
                        }
                        else
                        {
                            return ;
                        }
                    }
                    else if (tmpY > (Globals.MapHeight - 1))
                    {
                        tmpY = tmpY - (Globals.MapHeight - 1);
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
                else if (tmpX > (Globals.MapWidth - 1))
                {
                    tmpX = tmpX - (Globals.MapWidth - 1);
                    if (tmpY < 0)
                    {
                        tmpY = (Globals.MapHeight - 1) - (tmpY * -1);
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY - 1] > -1)
                        {
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + 1, Globals.GameMaps[tmpMap].MapGridY - 1];
                        }
                        else
                        {
                            return ;
                        }
                    }
                    else if (tmpY > (Globals.MapHeight - 1))
                    {
                        tmpY = tmpY - (Globals.MapHeight - 1);
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
                    tmpY = (Globals.MapHeight - 1) - (tmpY * -1);
                    if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY - 1] > -1)
                    {
                        tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX, Globals.GameMaps[tmpMap].MapGridY - 1];
                    }
                    else
                    {
                        return ;
                    }
                }
                else if (tmpY > (Globals.MapHeight - 1))
                {
                    tmpY = tmpY - (Globals.MapHeight - 1);
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
                if (this.GetType() == typeof (Event))
                {
                    if (client != null) {PacketSender.SendEntityMoveTo(client, MyIndex, (int)Enums.EntityTypes.LocalEvent, this);}
                }
                else
                {
                    PacketSender.SendEntityMove(MyIndex, (int)Enums.EntityTypes.GlobalEntity, this);
                }
                if (Stat[2] == 0) { Stat[2] = 1; }
                MoveTimer = Environment.TickCount + (int)((1.0 / (Stat[2] / 10f)) * 1000);
            }
            catch (Exception)
            {
                //ignore
            }
        }

        // Change the dimension if the player is on a gateway
        public void TryToChangeDimension()
        {
            if (CurrentX < Globals.MapWidth && CurrentX >= 0)
            {
                if (CurrentY < Globals.MapHeight && CurrentY >= 0)
                {
                    if (Globals.GameMaps[CurrentMap].Attributes[CurrentX, CurrentY].value == (int)Enums.MapAttributes.ZDimension)
                    {
                        if (Globals.GameMaps[CurrentMap].Attributes[CurrentX, CurrentY].data1 > 0)
                        {
                            CurrentZ = Globals.GameMaps[CurrentMap].Attributes[CurrentX, CurrentY].data1 - 1;
                        }
                    }
                }
            }
        }

        public void ChangeDir(int dir)
        {
            Dir = dir;
            if (this.GetType() == typeof (Event))
            {
                PacketSender.SendEntityDirTo(  ((Event)this).Client  ,MyIndex, (int)Enums.EntityTypes.LocalEvent,Dir);
            }
            else
            {
                PacketSender.SendEntityDir(MyIndex, (int)Enums.EntityTypes.GlobalEntity,Dir,CurrentMap);
            }
        }

        //Combat
        public void TryAttack(int enemyIndex, bool isProjectile = false, bool isSpell = false)
        {
            if (Globals.Entities[enemyIndex] == null) return;
            if (!IsOneBlockAway(enemyIndex) && isProjectile == false && isSpell == false) return;
            //If Entity is resource, check for the correct tool.
            if (Globals.Entities[enemyIndex].GetType() == typeof(Resource))
            {
                // Check that a resource is actually required.
                if (Globals.GameResources[((Resource)Globals.Entities[enemyIndex]).ResourceNum].Tool > 0)
                {
                    if (((Player)Globals.Entities[MyIndex]).Equipment[2] < 0)
                    {
                        PacketSender.SendPlayerMsg(((Player)Globals.Entities[MyIndex]).MyClient, "You require a " + Enums.ToolTypes[Globals.GameResources[((Resource)Globals.Entities[enemyIndex]).ResourceNum].Tool] + " to interact with this resource.");
                        return;
                    }
                    if (Globals.GameResources[((Resource)Globals.Entities[enemyIndex]).ResourceNum].Tool != Globals.GameItems[Inventory[((Player)Globals.Entities[MyIndex]).Equipment[2]].ItemNum].Tool)
                    {
                        PacketSender.SendPlayerMsg(((Player)Globals.Entities[MyIndex]).MyClient, "You require a " + Enums.ToolTypes[Globals.GameResources[((Resource)Globals.Entities[enemyIndex]).ResourceNum].Tool] + " to interact with this resource.");
                        return;
                    }
                }
            }
            //No Matter what, if we attack the entitiy, make them chase us
            if (Globals.Entities[enemyIndex].GetType() == typeof(Npc))
            {
                ((Npc)Globals.Entities[enemyIndex]).MyTarget = this;

                //Check if there are any guards nearby
                for (int n = 0; n < Globals.GameMaps[CurrentMap].Entities.Count; n++)
                {
                    if (Globals.GameMaps[CurrentMap].Entities[n].GetType() == typeof(Npc))
                    {
                        if (((Npc)Globals.GameMaps[CurrentMap].Entities[n]).Behaviour == 3) // Type guard
                        {
                            int x = Globals.GameMaps[CurrentMap].Entities[n].CurrentX - ((Npc)Globals.GameMaps[CurrentMap].Entities[n]).Range;
                            int y = Globals.GameMaps[CurrentMap].Entities[n].CurrentY - ((Npc)Globals.GameMaps[CurrentMap].Entities[n]).Range;
                            int xMax = Globals.GameMaps[CurrentMap].Entities[n].CurrentX + ((Npc)Globals.GameMaps[CurrentMap].Entities[n]).Range;
                            int yMax = Globals.GameMaps[CurrentMap].Entities[n].CurrentY + ((Npc)Globals.GameMaps[CurrentMap].Entities[n]).Range;

                            //Check that not going out of the map boundaries
                            if (x < 0) x = 0;
                            if (y < 0) y = 0;
                            if (xMax >= Globals.MapWidth) xMax = Globals.MapWidth;
                            if (yMax >= Globals.MapHeight) yMax = Globals.MapHeight;

                            if (x < Globals.Entities[MyIndex].CurrentX && xMax > Globals.Entities[MyIndex].CurrentX)
                            {
                                if (y < Globals.Entities[MyIndex].CurrentY && yMax > Globals.Entities[MyIndex].CurrentY)
                                {
                                    // In range, so make a target
                                    ((Npc)Globals.GameMaps[CurrentMap].Entities[n]).MyTarget = Globals.Entities[MyIndex];
                                }
                            }
                        }
                    }
                }
            }
            double dmg = (Stat[(int)Enums.Stats.Attack] * ((double)100 / (100 + (double)(Globals.Entities[enemyIndex].Stat[(int)Enums.Stats.Defense] * 2)))) + Globals.Rand.Next(0, 3);

            if (dmg <= 0) dmg = 1; // Always do damage.

            Globals.Entities[enemyIndex].Vital[(int)Enums.Vitals.Health] -= (int)dmg;

            //Dead entity check
            if (Globals.Entities[enemyIndex].Vital[(int)Enums.Vitals.Health] <= 0)
            {
                //Check if a resource, if so spawn item drops differently.
                if (Globals.Entities[enemyIndex].GetType() == typeof(Resource))
                {
                    ((Resource)Globals.Entities[enemyIndex]).SpawnResourceItems(MyIndex);
                }
                Globals.Entities[enemyIndex].Die();
            }
            else
            {
                //Hit him, make him mad and send the vital update.
                PacketSender.SendEntityVitals(enemyIndex,(int)Enums.EntityTypes.GlobalEntity, Globals.Entities[enemyIndex]);
            }
            // Add a timer before able to make the next move.
            if (Globals.Entities[MyIndex].GetType() == typeof(Npc))
            {
                ((Npc)Globals.Entities[MyIndex]).MoveTimer = Environment.TickCount + (int)((1.0 / (Stat[2] / 10f)) * 1000);
            }
        }

        //Check if the target is either up, down, left or right of the target on the correct Z dimension.
        bool IsOneBlockAway(int enemyIndex)
        {
            if (Globals.Entities[enemyIndex].CurrentZ == CurrentZ)
            {
                if (Globals.Entities[enemyIndex].CurrentY == CurrentY)
                {
                    if (Globals.Entities[enemyIndex].CurrentX == CurrentX - 1)
                    {
                        return true;
                    }
                    else if (Globals.Entities[enemyIndex].CurrentX == CurrentX + 1)
                    {
                        return true;
                    }
                }
                if (Globals.Entities[enemyIndex].CurrentX == CurrentX)
                {
                    if (Globals.Entities[enemyIndex].CurrentY == CurrentY - 1)
                    {
                        return true;
                    }
                    else if (Globals.Entities[enemyIndex].CurrentY == CurrentY + 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //Spawning/Dying
        public virtual void Die(bool dropitems = false)
        {
            if (dropitems)
            {
                // Drop items
                for (int i = 0; i < Constants.MaxInvItems; i++)
                {
                    if (Inventory[i].ItemNum >= 0)
                    {
                        Globals.GameMaps[CurrentMap].SpawnItem(CurrentX, CurrentY, Inventory[i], Inventory[i].ItemVal);
                    }
                }
            }
        }
        public void Reset()
        {
            for (var i = 0; i < (int) Enums.Vitals.VitalCount; i++)
            {
                Vital[i] = MaxVital[i];
            }
        }

        //Empty virtual functions for players
        public virtual void Warp(int newMap, int newX, int newY)
        {
            Warp(newMap, newX, newY, Dir);
        }
        public virtual void Warp(int newMap, int newX, int newY, int newDir)
        {
            
        }

        public void CastSpell(int SpellNum, int SpellSlot = -1)
        {
            switch (Globals.GameSpells[SpellNum].Type)
            {
                case (int)Enums.SpellTypes.CombatSpell:

                    switch (Globals.GameSpells[SpellNum].TargetType)
                    {
                        case (int)Enums.TargetTypes.Self:

                            break;
                        case (int)Enums.TargetTypes.Single:

                            break;
                        case (int)Enums.TargetTypes.AoE:

                            break;
                        case (int)Enums.TargetTypes.Projectile:
                            Globals.GameMaps[CurrentMap].SpawnMapProjectile(MyIndex, this.GetType(), Globals.GameSpells[SpellNum].Data4 - 1, CurrentMap, CurrentX, CurrentY, CurrentZ, Dir);
                            break;
                        default:
                            break;
                    }

                    break;
                case (int)Enums.SpellTypes.Warp:
                    if (GetType() == typeof(Player))
                    {
                        Warp(Globals.GameSpells[SpellNum].Data1, Globals.GameSpells[SpellNum].Data2, Globals.GameSpells[SpellNum].Data3, Globals.GameSpells[SpellNum].Data4);
                    }
                    break;
                case (int)Enums.SpellTypes.Dash:

                    break;
                default:
                    break;
            }
            if (SpellSlot >= 0 && SpellSlot < Constants.MaxPlayerSkills)
            {
                Spells[SpellSlot].SpellCD = Environment.TickCount + (Globals.GameSpells[SpellNum].CooldownDuration * 100);
                if (GetType() == typeof(Player))
                {
                    PacketSender.SendSpellCooldown(((Player)Globals.Entities[MyIndex]).MyClient, SpellSlot);
                }
            }
        }

        //Serializing Data
        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteString(MyName);
            bf.WriteString(MySprite);
            bf.WriteString(Face);
            bf.WriteInteger(CurrentX);
            bf.WriteInteger(CurrentY);
            bf.WriteInteger(CurrentMap);
            bf.WriteInteger(CurrentZ);
            return bf.ToArray();
        }
    }

}

