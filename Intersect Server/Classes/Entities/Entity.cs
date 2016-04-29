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
using Intersect_Library;
using Intersect_Server.Classes.Core;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Items;
using Intersect_Server.Classes.Maps;
using Intersect_Server.Classes.Networking;
using Intersect_Server.Classes.Spells;
using Attribute = Intersect_Library.GameObjects.Maps.Attribute;

namespace Intersect_Server.Classes.Entities
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
        public int[] MaxVital = new int[(int)Vitals.VitalCount];
        public int[] Vital = new int[(int)Vitals.VitalCount];
        public EntityStat[] Stat = new EntityStat[(int)Stats.StatCount];

        //Combat Status
        public long CastTime = 0;
        public int SpellCastSlot = 0;

        //Inventory
        public List<ItemInstance> Inventory = new List<ItemInstance>();

        //Spells
        public List<SpellInstance> Spells = new List<SpellInstance>();

        //Active Animations -- for events mainly
        public List<int> Animations = new List<int>();

        //DoT/HoT Spells
        public List<DoTInstance> DoT = new List<DoTInstance>();

        //Status effects
        public List<StatusInstance> Status = new List<StatusInstance>();

        public long MoveTimer;

        public long CollisionIndex;

        public int Target = -1;

        //Initialization
        public Entity(int index)
        {
            for (int I = 0; I < (int)Stats.StatCount; I++ )
            {
                Stat[I] = new EntityStat(0);
            }

            MyIndex = index;
            //HP
            MaxVital[(int)Vitals.Health] = 100;
            Vital[(int)Vitals.Health] = 100;
            //MP
            MaxVital[(int)Vitals.Health] = 100;
            Vital[(int)Vitals.Health] = 100;
            //ATK
            Stat[(int)Stats.Attack].Stat = 23;
            //Ability
            Stat[(int)Stats.AbilityPower].Stat = 16;
            //Def
            Stat[(int)Stats.Defense].Stat = 23;
            //MR
            Stat[(int)Stats.MagicResist].Stat = 16;
            //SPD
            Stat[(int)Stats.Speed].Stat = 20;
        }

        public virtual void Update()
        {
            //Cast timers
            if (CastTime != 0 && CastTime < Environment.TickCount)
            {
                CastTime = 0;
                CastSpell(Spells[SpellCastSlot].SpellNum, SpellCastSlot);
            }
            //DoT/HoT timers
            for (int i = 0; i < DoT.Count; i++)
            {
                DoT[i].Tick();
            }
            //Status timers
            int count = Status.Count;
            for (int i = 0; i < Status.Count; i++)
            {
                Status[i].TryRemoveStatus();
            }
            //If there is a removal of a status, update it client sided.
            if (count > Status.Count) { PacketSender.SendEntityVitals(MyIndex, (int)EntityTypes.GlobalEntity, this); }
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
            var xOffset = 0;
            var yOffset = 0;
            var tile = new TileHelper(CurrentMap,CurrentX,CurrentY);
            switch (moveDir)
            {
                case 0: //Up
                    yOffset--;
                    break;
                case 1: //Down
                    yOffset++;
                    break;
                case 2: //Left
                    xOffset--;
                    break;
                case 3: //Right
                    xOffset++;
                    break;
                case 4: //NW
                    yOffset--;
                    xOffset--;
                    break;
                case 5: //NE
                    yOffset--;
                    xOffset++;
                    break;
                case 6: //SW
                    yOffset++;
                    xOffset--;
                    break;
                case 7: //SE
                    yOffset++;
                    xOffset++;
                    break;
            }

            if (tile.Translate(xOffset, yOffset))
            {
                Attribute tileAttribute = Globals.GameMaps[tile.GetMap()].Attributes[tile.GetX(), tile.GetY()];
                if (tileAttribute != null)
                {
                    if (tileAttribute.value == (int)MapAttributes.Blocked) return -2;
                    if (tileAttribute.value == (int)MapAttributes.NPCAvoid && this.GetType() == typeof(Npc)) return -2;
                    if (tileAttribute.value == (int)MapAttributes.ZDimension &&
                        tileAttribute.data2 - 1 == CurrentZ) return -3;
                }
            }
            else
            {
                return -5; //Out of Bounds
            }

            for (int i = 0; i < Globals.GameMaps[tile.GetMap()].Entities.Count; i++)
            {
                Entity en = Globals.GameMaps[tile.GetMap()].Entities[i];
                if (en.CurrentX == tile.GetX() && en.CurrentY == tile.GetY() && en.CurrentZ == CurrentZ && en.Passable == 0)
                {
                    //Set a target if a projectile
                    CollisionIndex = en.MyIndex;
                    if (en.GetType() == typeof(Player))
                    {
                        return (int)EntityTypes.Player;
                    }
                    else if (en.GetType() == typeof(Npc))
                    {
                        return (int)EntityTypes.Player;
                    }
                    else if (en.GetType() == typeof(Resource))
                    {
                        return (int)EntityTypes.Resource;
                    }
                    else if (en.GetType() == typeof(EventPageInstance))
                    {
                        return (int)EntityTypes.Event;
                    }
                }
            }

            return -1;
        }


        public void Move(int moveDir, Client client)
        {
            var xOffset = 0;
            var yOffset = 0;
            var tile = new TileHelper(CurrentMap, CurrentX, CurrentY);
            switch (moveDir)
            {
                case 0: //Up
                    yOffset--;
                    break;
                case 1: //Down
                    yOffset++;
                    break;
                case 2: //Left
                    xOffset--;
                    break;
                case 3: //Right
                    xOffset++;
                    break;
                case 4: //NW
                    yOffset--;
                    xOffset--;
                    break;
                case 5: //NE
                    yOffset--;
                    xOffset++;
                    break;
                case 6: //SW
                    yOffset++;
                    xOffset--;
                    break;
                case 7: //SE
                    yOffset++;
                    xOffset++;
                    break;
            }

            if (tile.Translate(xOffset, yOffset))
            {
                CurrentX = tile.GetX();
                CurrentY = tile.GetY();
                CurrentMap = tile.GetMap();
                Dir = moveDir;
                if (this.GetType() == typeof(EventPageInstance))
                {
                    if (client != null)
                    {
                        PacketSender.SendEntityMoveTo(client, MyIndex, (int)EntityTypes.Event, this);
                    }
                    else
                    {
                        PacketSender.SendEntityMove(MyIndex, (int)EntityTypes.Event, this);
                    }
                }
                else
                {
                    PacketSender.SendEntityMove(MyIndex, (int)EntityTypes.GlobalEntity, this);
                }
                MoveTimer = Environment.TickCount + (int)((1.0 / (Stat[2].Value() / 10f)) * 1000);
            }
        }

        public void ChangeDir(int dir)
        {
            Dir = dir;
            if (this.GetType() == typeof(EventPageInstance))
            {
                PacketSender.SendEntityDirTo(((EventPageInstance)this).Client, MyIndex, (int)EntityTypes.Event, Dir, CurrentMap);
            }
            else
            {
                PacketSender.SendEntityDir(MyIndex, (int)EntityTypes.GlobalEntity, Dir, CurrentMap);
            }
        }
        // Change the dimension if the player is on a gateway
        public void TryToChangeDimension()
        {
            if (CurrentX < Options.MapWidth && CurrentX >= 0)
            {
                if (CurrentY < Options.MapHeight && CurrentY >= 0)
                {
                    Attribute attribute = Globals.GameMaps[CurrentMap].Attributes[CurrentX, CurrentY];
                    if (attribute != null && attribute.value == (int)MapAttributes.ZDimension)
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
                        xDiff = (Globals.GameMaps[CurrentMap].MapGridX - x) * Options.MapWidth + target.CurrentX -
                                CurrentX;
                        yDiff = (Globals.GameMaps[CurrentMap].MapGridY - y) * Options.MapHeight + target.CurrentY -
                                CurrentY;
                        if (Math.Abs(xDiff) > Math.Abs(yDiff))
                        {
                            if (xDiff < 0) return (int)Directions.Left;
                            if (xDiff > 0) return (int)Directions.Right;
                        }
                        else
                        {
                            if (yDiff < 0) return (int)Directions.Up;
                            if (yDiff > 0) return (int)Directions.Down;
                        }
                    }
                }
            }

            return -1;
        }


        //Combat
        public void TryAttack(int enemyIndex, bool isProjectile = false, int isSpell = -1)
        {
            double dmg = 0;

            if (Globals.Entities[enemyIndex] == null) return;
            if (!IsOneBlockAway(enemyIndex) && isProjectile == false && isSpell == -1) return;

            //If Entity is resource, check for the correct tool and make sure its not a spell cast.
            if (Globals.Entities[enemyIndex].GetType() == typeof(Resource))
            {
                if (isSpell == -1) return;
                // Check that a resource is actually required.
                if (Globals.GameResources[((Resource)Globals.Entities[enemyIndex]).ResourceNum].Tool > -1 && Globals.GameResources[((Resource)Globals.Entities[enemyIndex]).ResourceNum].Tool < Options.ToolTypes.Count)
                {
                    if (((Player)Globals.Entities[MyIndex]).Equipment[2] < 0)
                    {
                        PacketSender.SendPlayerMsg(((Player)Globals.Entities[MyIndex]).MyClient, "You require a " + Options.ToolTypes[Globals.GameResources[((Resource)Globals.Entities[enemyIndex]).ResourceNum].Tool] + " to interact with this resource.");
                        return;
                    }
                    if (Globals.GameResources[((Resource)Globals.Entities[enemyIndex]).ResourceNum].Tool != Globals.GameItems[Inventory[((Player)Globals.Entities[MyIndex]).Equipment[2]].ItemNum].Tool)
                    {
                        PacketSender.SendPlayerMsg(((Player)Globals.Entities[MyIndex]).MyClient, "You require a " + Options.ToolTypes[Globals.GameResources[((Resource)Globals.Entities[enemyIndex]).ResourceNum].Tool] + " to interact with this resource.");
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
                            if (xMax >= Options.MapWidth) xMax = Options.MapWidth;
                            if (yMax >= Options.MapHeight) yMax = Options.MapHeight;

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
            if (isSpell == -1)
            {
                dmg = DamageCalculator(Stat[(int)Stats.Attack].Value(), Globals.Entities[enemyIndex].Stat[(int)Stats.Defense].Value());
                if (dmg <= 0) dmg = 1; // Always do damage.
            }
            else
            {
                // Handle different dmg formula for healing and damaging spells.
                if (Globals.GameSpells[isSpell].VitalDiff[(int)Vitals.Health] > 0)
                {
                    dmg = -Globals.GameSpells[isSpell].VitalDiff[(int)Vitals.Health];
                }
                else
                {
                    dmg = DamageCalculator(Stat[(int)Stats.AbilityPower].Value(), Globals.Entities[enemyIndex].Stat[(int)Stats.MagicResist].Value()) - Globals.GameSpells[isSpell].VitalDiff[(int)Vitals.Health];
                    if (dmg <= 0) dmg = 1; // Always do damage.
                }
                
                //Handle other stat debuffs/vitals.
                Globals.Entities[enemyIndex].Vital[(int)Vitals.Mana] += Globals.GameSpells[isSpell].VitalDiff[(int)Vitals.Mana];
                for (int i = 0; i < (int)Stats.StatCount; i++)
                {
                    Globals.Entities[enemyIndex].Stat[i].Buff.Add(new EntityBuff(Globals.GameSpells[isSpell].StatDiff[i], (Globals.GameSpells[isSpell].Data2 * 100)));
                }

                //Handle other status effects
                if (Globals.GameSpells[isSpell].Data3 > 0)
                {
                    Globals.Entities[enemyIndex].Status.Add(new StatusInstance(enemyIndex, Globals.GameSpells[isSpell].Data3, (Globals.GameSpells[isSpell].Data2 * 100), Globals.GameSpells[isSpell].Data5));
                }

                //Handle DoT/HoT spells]
                if (Globals.GameSpells[isSpell].Data1 > 0)
                {
                    for (int i = 0; i < Globals.Entities[enemyIndex].DoT.Count; i++)
                    {
                        if (Globals.Entities[enemyIndex].DoT[i].SpellNum == isSpell || Globals.Entities[enemyIndex].DoT[i].OwnerID == MyIndex)
                        {
                            return;
                        }
                    }
                    Globals.Entities[enemyIndex].DoT.Add(new DoTInstance(MyIndex, isSpell, enemyIndex));
                }
            }

            Globals.Entities[enemyIndex].Vital[(int)Vitals.Health] -= (int)dmg;

            //Check if after healing, greater than maximum hp.
            if (Globals.Entities[enemyIndex].Vital[(int)Vitals.Health] >= Globals.Entities[enemyIndex].MaxVital[(int)Vitals.Health]) 
            {
                Globals.Entities[enemyIndex].Vital[(int)Vitals.Health] = Globals.Entities[enemyIndex].MaxVital[(int)Vitals.Health];
            }

            //Dead entity check
            if (Globals.Entities[enemyIndex].Vital[(int)Vitals.Health] <= 0)
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
                PacketSender.SendEntityVitals(enemyIndex, (int)EntityTypes.GlobalEntity, Globals.Entities[enemyIndex]);
                PacketSender.SendEntityStats(enemyIndex, (int)EntityTypes.GlobalEntity, Globals.Entities[enemyIndex]);
            }
            // Add a timer before able to make the next move.
            if (Globals.Entities[MyIndex].GetType() == typeof(Npc))
            {
                ((Npc)Globals.Entities[MyIndex]).MoveTimer = Environment.TickCount + (int)((1.0 / (Stat[2].Value() / 10f)) * 1000);
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
                for (int i = 0; i < Options.MaxInvItems; i++)
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
            for (var i = 0; i < (int)Vitals.VitalCount; i++)
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
                case (int)SpellTypes.CombatSpell:

                    switch (Globals.GameSpells[SpellNum].TargetType)
                    {
                        case (int)SpellTargetTypes.Self:
                            if (Globals.GameSpells[SpellNum].HitAnimation > -1)
                            {
                                PacketSender.SendAnimationToProximity(Globals.GameSpells[SpellNum].HitAnimation, 1, MyIndex, CurrentMap, 0, 0, Dir); //Target Type 1 will be global entity
                            }
                            TryAttack(MyIndex, false, SpellNum);
                            break;
                        case (int)SpellTargetTypes.Single:
                            HandleAoESpell(SpellNum, Target);
                            break;
                        case (int)SpellTargetTypes.AoE:
                            HandleAoESpell(SpellNum);
                            break;
                        case (int)SpellTargetTypes.Projectile:
                            Globals.GameMaps[CurrentMap].SpawnMapProjectile(this, Globals.GameSpells[SpellNum].Data4 - 1, CurrentMap, CurrentX, CurrentY, CurrentZ, Dir, SpellNum, Target);
                            break;
                        default:
                            break;
                    }
                    break;
                case (int)SpellTypes.Warp:
                    if (GetType() == typeof(Player))
                    { 
                        Warp(Globals.GameSpells[SpellNum].Data1, Globals.GameSpells[SpellNum].Data2, Globals.GameSpells[SpellNum].Data3, Globals.GameSpells[SpellNum].Data4);
                    }
                    break;
                case (int)SpellTypes.WarpTo:
                    if (GetType() == typeof(Player))
                    {
                        HandleAoESpell(SpellNum, Target);
                    }
                    break;
                case (int)SpellTypes.Dash:

                    break;
                case (int)SpellTypes.Event:

                    break;
                default:
                    break;
            }
            if (SpellSlot >= 0 && SpellSlot < Options.MaxPlayerSkills)
            {
                Spells[SpellSlot].SpellCD = Environment.TickCount + (Globals.GameSpells[SpellNum].CooldownDuration * 100);
                if (GetType() == typeof(Player))
                {
                    PacketSender.SendSpellCooldown(((Player)Globals.Entities[MyIndex]).MyClient, SpellSlot);
                }
            }
        }

        private void HandleAoESpell(int SpellNum, int target = -1)
        {
            for (int x = CurrentX - Globals.GameSpells[SpellNum].CastRange; x < CurrentX + Globals.GameSpells[SpellNum].CastRange; x++)
            {
                for (int y = CurrentY - Globals.GameSpells[SpellNum].CastRange; y < CurrentY + Globals.GameSpells[SpellNum].CastRange; y++)
                {
                    int tempMap = CurrentMap;
                    int x2 = x;
                    int y2 = y;

                    if (y < 0 && Globals.GameMaps[tempMap].Up > -1)
                    {
                        tempMap = Globals.GameMaps[tempMap].Up;
                        y2 = Options.MapHeight + y;
                    }
                    else if (y > Options.MapHeight - 1 && Globals.GameMaps[tempMap].Down > -1)
                    {
                        tempMap = Globals.GameMaps[tempMap].Down;
                        y2 = Options.MapHeight - y;
                    }

                    if (x < 0 && Globals.GameMaps[tempMap].Left > -1)
                    {
                        tempMap = Globals.GameMaps[tempMap].Left;
                        x2 = Options.MapWidth + x;
                    }
                    else if (x > Options.MapWidth - 1 && Globals.GameMaps[tempMap].Right > -1)
                    {
                        tempMap = Globals.GameMaps[tempMap].Right;
                        x2 = Options.MapWidth - x;
                    }

                    for (int i = 0; i < Globals.GameMaps[tempMap].Entities.Count; i++ )
                    {
                        Entity t = Globals.GameMaps[tempMap].Entities[i];
                        if (t == null) continue;
                        if (t.GetType() == typeof(Player) || t.GetType() == typeof(Npc))
                        {
                            if (t.CurrentMap == tempMap && t.CurrentX == x2 && t.CurrentY == y2)
                            {
                                if ((target == -1 || target == t.MyIndex) && t.MyIndex != MyIndex)
                                {
                                    //Warp or attack.
                                    if (Globals.GameSpells[SpellNum].Type == (int)SpellTypes.CombatSpell)
                                    {
                                        TryAttack(t.MyIndex, false, SpellNum);
                                    }
                                    else
                                    {
                                        Warp(Globals.Entities[Target].CurrentMap, Globals.Entities[Target].CurrentX, Globals.Entities[Target].CurrentY, Dir);
                                    }
                                    if (Globals.GameSpells[SpellNum].HitAnimation > -1)
                                    {
                                        if (target > -1 && t.Vital[(int)Vitals.Health] > 0)
                                        {
                                            PacketSender.SendAnimationToProximity(Globals.GameSpells[SpellNum].HitAnimation, 1, target, tempMap, 0, 0, t.Dir); //Target Type 1 will be global entity
                                        }
                                        else
                                        {
                                            PacketSender.SendAnimationToProximity(Globals.GameSpells[SpellNum].HitAnimation, -1, -1, tempMap, x, y, Dir); //Target Type -1 will be tile based animation
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //Serializing Data
        public virtual byte[] Data()
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

    public class EntityStat
    {
        public int Stat = 0;
        public List<EntityBuff> Buff = new List<EntityBuff>();
        
        public EntityStat(int stat)
        {
            Stat = stat;
        }

        public int Value()
        {
            int s = Stat;

            for (int i = 0; i < Buff.Count; i++)
            {
                s += Buff[i].Buff;
            }

            if (s <= 0) s = 1; //No 0 or negative stats, will give errors elsewhere in the code (especially divide by 0 errors).
            return s;
        }

        public void Update()
        {
            for (int i = 0; i < Buff.Count; i++)
            {
                if (Buff[i].Duration <= Environment.TickCount)
                {
                    Buff.RemoveAt(i);
                }
            }
        }
    }

    public class EntityBuff
    {
        public int Buff = 0;
        public long Duration = 0;

        public EntityBuff(int buff, int duration)
        {
            Buff = buff;
            Duration = Environment.TickCount + duration;
        }
    }

    public class DoTInstance
    {
        public int SpellNum = -1;
        public int OwnerID = -1;
        public int TargetID = -1;
        public int Count = 0;
        private long Interval = 0;

        public DoTInstance(int ownerID, int spellNum, int targetID)
        {
            SpellNum = spellNum;
            OwnerID = ownerID;
            TargetID = targetID;
            Interval = Environment.TickCount + (Globals.GameSpells[SpellNum].Data4 * 100);
            Count = (Globals.GameSpells[SpellNum].Data4 % Globals.GameSpells[SpellNum].Data2) - 1; //Subtract 1 since the first tick always occurs when the spell is cast.
        }

        public void Tick()
        {
            if (Interval >= Environment.TickCount)
            {
                if (Globals.GameSpells[SpellNum].HitAnimation > -1)
                {
                    PacketSender.SendAnimationToProximity(Globals.GameSpells[SpellNum].HitAnimation, 1, TargetID, Globals.Entities[TargetID].CurrentMap, 0, 0, Globals.Entities[TargetID].Dir); //Target Type 1 will be global entity
                }
                Globals.Entities[OwnerID].TryAttack(TargetID, false, SpellNum);
                Interval = Environment.TickCount + (Globals.GameSpells[SpellNum].Data4 * 100);
                Count--;

                if (Count <= 0)
                {
                    Globals.Entities[TargetID].DoT.Remove(this);
                }
            }
        }
    }

    public class StatusInstance
    {
        public int Type = -1;
        public string Data = "";
        public long Duration = 0;
        private int EntityID = -1;

        public StatusInstance(int entityID, int type, int duration, string data)
        {
            EntityID = entityID;
            Type = type;
            Duration = Environment.TickCount + duration;
            Data = data;
            PacketSender.SendEntityVitals(EntityID, (int)EntityTypes.GlobalEntity, Globals.Entities[EntityID]);
        }

        public void TryRemoveStatus()
        {
            if (Duration <= Environment.TickCount)
            {
                Globals.Entities[EntityID].Status.Remove(this);
            }
        }
    }
}

