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
        public List<ItemInstance> Inventory = new List<ItemInstance>();

        //Spells
        public List<SpellInstance> Spells = new List<SpellInstance>();

        //Active Animations -- for events mainly
        public List<int> Animations = new List<int>();

        public long MoveTimer;

        public long CollisionIndex;

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
        }

        //Movement
        /// <summary>
        /// Determines if this entity can move in the direction given.
        /// Returns -5 if the tile is completely out of bounds.
        /// Returns -3 if a tile is blocked because of a Z dimension tile
        /// Returns -2 if a tile is blocked by a map attribute.
        /// Returns -1 for clear.
        /// Returns the type of entity that is blocking the way (if one exists)
        /// </summary>
        /// <param name="moveDir"></param>
        /// <returns></returns>
        public int CanMove(int moveDir)
        {
            var tmpX = CurrentX;
            var tmpY = CurrentY;
            var mapX = 0;
            var mapY = 0;
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
                case 4: //NW
                    tmpY--;
                    tmpX--;
                    break;
                case 5: //NE
                    tmpY--;
                    tmpX++;
                    break;
                case 6: //SW
                    tmpY++;
                    tmpX--;
                    break;
                case 7: //SE
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
                    mapX--;
                }

                if (tmpY < 0)
                {
                    tmpY = (Globals.MapHeight - 1) - (tmpY * -1);
                    mapY--;
                }

                if (tmpY > (Globals.MapHeight - 1))
                {
                    tmpY = tmpY - (Globals.MapHeight - 1);
                    mapY++;
                }

                if (tmpX > (Globals.MapWidth - 1))
                {
                    tmpX = tmpX - (Globals.MapWidth - 1);
                    mapX++;
                }

                if (Globals.GameMaps[tmpMap].MapGridX + mapX >= 0 &&
                    Globals.GameMaps[tmpMap].MapGridX + mapX < Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].Width)
                {
                    if (Globals.GameMaps[tmpMap].MapGridY + mapY >= 0 &&
                        Globals.GameMaps[tmpMap].MapGridY + mapY <
                        Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].Height)
                    {
                        if (Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + mapX, Globals.GameMaps[tmpMap].MapGridY + mapY] > -1)
                        {
                            Attribute tileAttribute =
                                Globals.GameMaps[
                                    Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[
                                        Globals.GameMaps[tmpMap].MapGridX + mapX, Globals.GameMaps[tmpMap].MapGridY + mapY]]
                                    .Attributes[tmpX, tmpY];
                            if (tileAttribute != null)
                            {
                                if (tileAttribute.value == (int)Enums.MapAttributes.Blocked) return -2;
                                if (tileAttribute.value == (int)Enums.MapAttributes.NPCAvoid && this.GetType() == typeof(Npc)) return -2;
                                if (tileAttribute.value == (int) Enums.MapAttributes.ZDimension &&
                                    tileAttribute.data2 - 1 == CurrentZ) return -3;
                            }
                            tmpMap = Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[Globals.GameMaps[tmpMap].MapGridX + mapX, Globals.GameMaps[tmpMap].MapGridY + mapY];
                        }
                        else
                        {
                            return -5;
                        }
                    }
                    else
                    {
                        return -5;
                    }
                }
                else
                {
                    return -5;
                }

                foreach (Entity en in Globals.Entities)
                {
                    if (en == null) continue;
                    if (en.CurrentMap == tmpMap && en.CurrentX == tmpX && en.CurrentY == tmpY && en.CurrentZ == CurrentZ && en.Passable == 0)
                    {
                        //Set a target if a projectile
                        CollisionIndex = en.MyIndex;
                        if (en.GetType() == typeof(Player))
                        {
                            return (int)Enums.EntityTypes.Player;
                        }
                        else if (en.GetType() == typeof(Npc))
                        {
                            return (int)Enums.EntityTypes.Player;
                        }
                        else if (en.GetType() == typeof(Resource))
                        {
                            return (int)Enums.EntityTypes.Resource;
                        }
                        else if (en.GetType() == typeof(EventPageInstance))
                        {
                            return (int)Enums.EntityTypes.Event;
                        }
                    }
                }
                return -1;
            }
            catch
            {
                return -2;

            }
        }
        public void Move(int moveDir, Client client)
        {
            var tmpX = CurrentX;
            var tmpY = CurrentY;
            var mapX = 0;
            var mapY = 0;
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
                    mapX--;
                }

                if (tmpY < 0)
                {
                    tmpY = (Globals.MapHeight - 1) - (tmpY * -1);
                    mapY--;
                }

                if (tmpY > (Globals.MapHeight - 1))
                {
                    tmpY = tmpY - (Globals.MapHeight - 1);
                    mapY++;
                }

                if (tmpX > (Globals.MapWidth - 1))
                {
                    tmpX = tmpX - (Globals.MapWidth - 1);
                    mapX++;
                }

                if (Globals.GameMaps[tmpMap].MapGridX + mapX >= 0 &&
                    Globals.GameMaps[tmpMap].MapGridX + mapX < Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].Width)
                {
                    if (Globals.GameMaps[tmpMap].MapGridY + mapY >= 0 &&
                        Globals.GameMaps[tmpMap].MapGridY + mapY <
                        Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].Height)
                    {

                        if (
                            Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[
                                Globals.GameMaps[tmpMap].MapGridX + mapX, Globals.GameMaps[tmpMap].MapGridY + mapY] > -1)
                        {
                            tmpMap =
                                Database.MapGrids[Globals.GameMaps[tmpMap].MapGrid].MyGrid[
                                    Globals.GameMaps[tmpMap].MapGridX + mapX, Globals.GameMaps[tmpMap].MapGridY + mapY];
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }


                CurrentX = tmpX;
                CurrentY = tmpY;
                CurrentMap = tmpMap;
                Dir = moveDir;
                if (this.GetType() == typeof(EventPageInstance))
                {
                    if (client != null)
                    {
                        PacketSender.SendEntityMoveTo(client, MyIndex, (int)Enums.EntityTypes.Event, this);
                    }
                    else
                    {
                        PacketSender.SendEntityMove(MyIndex, (int)Enums.EntityTypes.Event, this);
                    }
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
        public void ChangeDir(int dir)
        {
            Dir = dir;
            if (this.GetType() == typeof(EventPageInstance))
            {
                PacketSender.SendEntityDirTo(((EventPageInstance)this).Client, MyIndex, (int)Enums.EntityTypes.Event, Dir, CurrentMap);
            }
            else
            {
                PacketSender.SendEntityDir(MyIndex, (int)Enums.EntityTypes.GlobalEntity, Dir, CurrentMap);
            }
        }
        // Change the dimension if the player is on a gateway
        public void TryToChangeDimension()
        {
            if (CurrentX < Globals.MapWidth && CurrentX >= 0)
            {
                if (CurrentY < Globals.MapHeight && CurrentY >= 0)
                {
                    Attribute attribute = Globals.GameMaps[CurrentMap].Attributes[CurrentX, CurrentY];
                    if (attribute != null && attribute.value == (int)Enums.MapAttributes.ZDimension)
                    {
                        if (attribute.data1 > 0)
                        {
                            CurrentZ = attribute.data1 - 1;
                        }
                    }
                }
            }
        }

        //Misc
        public int GetDirectionTo(Entity target)
        {
            int xDiff = 0, yDiff = 0;
            int myGrid = Globals.GameMaps[CurrentMap].MapGrid;
            //Loop through surrouding maps to generate a array of open and blocked points.
            for (var x = Globals.GameMaps[CurrentMap].MapGridX - 1;
                x <= Globals.GameMaps[CurrentMap].MapGridX + 1;
                x++)
            {
                if (x == -1 || x >= Database.MapGrids[myGrid].Width) continue;
                for (var y = Globals.GameMaps[CurrentMap].MapGridY - 1;
                    y <= Globals.GameMaps[CurrentMap].MapGridY + 1;
                    y++)
                {
                    if (y == -1 || y >= Database.MapGrids[myGrid].Height) continue;
                    if (Database.MapGrids[myGrid].MyGrid[x, y] > -1 &&
                        Database.MapGrids[myGrid].MyGrid[x, y] == target.CurrentMap)
                    {
                        xDiff = (Globals.GameMaps[CurrentMap].MapGridX - x) * Globals.MapWidth + target.CurrentX -
                                CurrentX;
                        yDiff = (Globals.GameMaps[CurrentMap].MapGridY - y) * Globals.MapHeight + target.CurrentY -
                                CurrentY;
                        if (Math.Abs(xDiff) > Math.Abs(yDiff))
                        {
                            if (xDiff < 0) return (int)Enums.Directions.Left;
                            if (xDiff > 0) return (int)Enums.Directions.Right;
                        }
                        else
                        {
                            if (yDiff < 0) return (int)Enums.Directions.Up;
                            if (yDiff > 0) return (int)Enums.Directions.Down;
                        }
                    }
                }
            }

            return -1;
        }


        //Combat
        public void TryAttack(int enemyIndex, bool isProjectile = false, bool isSpell = false)
        {
            double dmg = 0;

            if (Globals.Entities[enemyIndex] == null) return;
            if (!IsOneBlockAway(enemyIndex) && isProjectile == false && isSpell == false) return;

            //If Entity is resource, check for the correct tool and make sure its not a spell cast.
            if (Globals.Entities[enemyIndex].GetType() == typeof(Resource))
            {
                if (isSpell) return;
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
                ((Npc)Globals.Entities[enemyIndex]).AssignTarget(MyIndex);

                //Check if there are any guards nearby
                //TODO Loop through CurrentMap - SurroundingMaps Entity List instead of global entity list.
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
                                    ((Npc)Globals.GameMaps[CurrentMap].Entities[n]).AssignTarget(MyIndex);
                                }
                            }
                        }
                    }
                }
            }

            //Check if magic or physical damage
            if (isSpell == false)
            {
                dmg = DamageCalculator(Stat[(int)Enums.Stats.Attack], Globals.Entities[enemyIndex].Stat[(int)Enums.Stats.Defense]);
            }
            else
            {
                dmg = DamageCalculator(Stat[(int)Enums.Stats.AbilityPower], Globals.Entities[enemyIndex].Stat[(int)Enums.Stats.MagicResist]);
            }

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
                PacketSender.SendEntityVitals(enemyIndex, (int)Enums.EntityTypes.GlobalEntity, Globals.Entities[enemyIndex]);
            }
            // Add a timer before able to make the next move.
            if (Globals.Entities[MyIndex].GetType() == typeof(Npc))
            {
                ((Npc)Globals.Entities[MyIndex]).MoveTimer = Environment.TickCount + (int)((1.0 / (Stat[2] / 10f)) * 1000);
            }
        }
        public void CastSpell(int SpellNum, int SpellSlot = -1)
        {
            switch (Globals.GameSpells[SpellNum].Type)
            {
                case (int)Enums.SpellTypes.CombatSpell:

                    switch (Globals.GameSpells[SpellNum].TargetType)
                    {
                        case (int)Enums.TargetTypes.Self:
                            if (Globals.GameSpells[SpellNum].HitAnimation > -1)
                            {
                                Animations.Add(Globals.GameSpells[SpellNum].HitAnimation);
                            }
                            TryAttack(MyIndex, false, true);
                            break;
                        case (int)Enums.TargetTypes.Single:

                            break;
                        case (int)Enums.TargetTypes.AoE:
                            HandleAoESpell(SpellNum);
                            break;
                        case (int)Enums.TargetTypes.Projectile:
                            Globals.GameMaps[CurrentMap].SpawnMapProjectile(this, Globals.GameSpells[SpellNum].Data4 - 1, CurrentMap, CurrentX, CurrentY, CurrentZ, Dir, true);
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

        private double DamageCalculator(int OffensiveStat, int DefensiveStat)
        {
            return (OffensiveStat * ((double)100 / (100 + (double)(DefensiveStat * 2)))) + Globals.Rand.Next(0, 3);
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
            for (var i = 0; i < (int)Enums.Vitals.VitalCount; i++)
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

        private void HandleAoESpell(int SpellNum)
        {
            for (int x = CurrentX - Globals.GameSpells[SpellNum].CastRange; x < CurrentX + Globals.GameSpells[SpellNum].CastRange; x++)
            {
                for (int y = CurrentY - Globals.GameSpells[SpellNum].CastRange; y < CurrentY + Globals.GameSpells[SpellNum].CastRange; x++)
                {
                    int tempMap = CurrentMap;
                    int x2 = x;
                    int y2 = y;

                    if (y < 0 && Globals.GameMaps[tempMap].Up > -1)
                    {
                        tempMap = Globals.GameMaps[tempMap].Up;
                        y2 = Globals.MapHeight + y;
                    }
                    else if (y > Globals.MapHeight - 1 && Globals.GameMaps[tempMap].Down > -1)
                    {
                        tempMap = Globals.GameMaps[tempMap].Down;
                        y2 = Globals.MapHeight - y;
                    }

                    if (x < 0 && Globals.GameMaps[tempMap].Left > -1)
                    {
                        tempMap = Globals.GameMaps[tempMap].Left;
                        x2 = Globals.MapWidth + x;
                    }
                    else if (x > Globals.MapWidth - 1 && Globals.GameMaps[tempMap].Right > -1)
                    {
                        tempMap = Globals.GameMaps[tempMap].Right;
                        x2 = Globals.MapWidth - x;
                    }

                    foreach (Entity t in Globals.Entities)
                    {
                        if (t == null) continue;
                        if (t.CurrentMap == tempMap && t.CurrentX == x2 && t.CurrentY == y2 && t.CurrentZ == CurrentZ)
                        {
                            if (t.GetType() == typeof(Player) || t.GetType() == typeof(Npc))
                            {
                                TryAttack(t.MyIndex, false, true);
                                if (Globals.GameSpells[SpellNum].HitAnimation > -1)
                                {
                                    t.Animations.Add(Globals.GameSpells[SpellNum].HitAnimation);
                                }
                            }
                        }
                    }
                }
            }
        }

        //Serializing Data
        public byte[] Data()
        {
            var bf = new ByteBuffer();
            bf.WriteInteger(CurrentMap);
            bf.WriteString(MyName);
            bf.WriteString(MySprite);
            bf.WriteString(Face);
            bf.WriteInteger(CurrentX);
            bf.WriteInteger(CurrentY);
            bf.WriteInteger(CurrentZ);
            bf.WriteInteger(Animations.Count);
            for (int i = 0; i < Animations.Count; i++)
            {
                bf.WriteInteger(Animations[i]);
            }
            return bf.ToArray();
        }
    }

}

