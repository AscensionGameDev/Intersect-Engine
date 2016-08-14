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
using System.CodeDom;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Intersect_Library;
using Intersect_Library.GameObjects;
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
        public long SpawnTime;
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
        public long RegenTimer = Globals.System.GetTimeMs();
        public long CombatTimer = 0;
        public bool Dead = false;

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

        public long AttackTimer = 0;
        public bool Blocking = false;

        //Initialization
        public Entity(int index)
        {
            for (int I = 0; I < (int)Stats.StatCount; I++)
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

            SpawnTime = Globals.System.GetTimeMs();
        }

        public virtual void Update()
        {
            //Cast timers
            if (CastTime != 0 && CastTime < Globals.System.GetTimeMs())
            {
                CastTime = 0;
                CastSpell(Spells[SpellCastSlot].SpellNum, SpellCastSlot);
            }
            //DoT/HoT timers
            for (int i = 0; i < DoT.Count; i++)
            {
                DoT[i].Tick();
            }
            for (int i = 0; i < (int)Stats.StatCount; i++)
            {
                if (Stat[i].Update())
                {
                    SendStatUpdate(i);
                }
            }
            //Regen Timers
            if (Globals.System.GetTimeMs() > CombatTimer && Globals.System.GetTimeMs() > RegenTimer)
            {
                ProcessRegen();
                RegenTimer = Globals.System.GetTimeMs() + ServerOptions.RegenTime;
            }
            //Status timers
            int count = Status.Count;
            for (int i = 0; i < Status.Count; i++)
            {
                Status[i].TryRemoveStatus();
            }
            //If there is a removal of a status, update it client sided.
            if (count > Status.Count) { PacketSender.SendEntityVitals(this); }
        }

        public virtual void ProcessRegen()
        {

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
        public virtual int CanMove(int moveDir)
        {
            var xOffset = 0;
            var yOffset = 0;
            if (MoveTimer > Globals.System.GetTimeMs()) return -5;
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
                Attribute tileAttribute = MapInstance.GetMap(tile.GetMap()).Attributes[tile.GetX(), tile.GetY()];
                if (tileAttribute != null)
                {
                    if (tileAttribute.value == (int)MapAttributes.Blocked) return -2;
                    if (tileAttribute.value == (int)MapAttributes.NPCAvoid && this.GetType() == typeof(Npc)) return -2;
                    if (tileAttribute.value == (int)MapAttributes.ZDimension && tileAttribute.data2 > 0 &&
                        tileAttribute.data2 - 1 == CurrentZ) return -3;
                    if (tileAttribute.value == (int)MapAttributes.Slide) return -4;
                }
            }
            else
            {
                return -5; //Out of Bounds
            }

            var mapEntities = MapInstance.GetMap(tile.GetMap()).GetEntities();
            for (int i = 0; i < mapEntities.Count; i++)
            {
                Entity en = mapEntities[i];
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
                        //If determine if we should walk
                        var res = ((Resource) en);
                        if ((!res.IsDead() && !res.MyBase.WalkableBefore) || (res.IsDead() && !res.MyBase.WalkableAfter))
                        {
                            return (int) EntityTypes.Resource;
                        }
                    }
                    else if (en.GetType() == typeof(EventPageInstance))
                    {
                        return (int)EntityTypes.Event;
                    }
                }
            }

            return -1;
        }

        //Returns the amount of time required to traverse 1 tile
        public virtual float GetMovementTime()
        {
            var time = 1000f / (float)(1 + Math.Log(Stat[(int)Stats.Speed].Value()));
            if (Blocking) { time += time * (float)Options.BlockingSlow; }
            if (time > 1000f) time = 1000f;
            return time;
        }

        public virtual EntityTypes GetEntityType()
        {
            return EntityTypes.GlobalEntity;
        }

        public virtual void Move(int moveDir, Client client, bool DontUpdate = false)
        {
            var xOffset = 0;
            var yOffset = 0;
            Dir = moveDir;
            if (MoveTimer < Globals.System.GetTimeMs())
            {
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
                    if (CurrentMap != tile.GetMap())
                    {
                        var oldMap = MapInstance.GetMap(CurrentMap);
                        if (oldMap != null) oldMap.RemoveEntity(this);
                        var newMap = MapInstance.GetMap(tile.GetMap());
                        if (newMap != null) newMap.AddEntity(this);
                    }
                    CurrentMap = tile.GetMap();
                    if (DontUpdate == false)
                    {
                        if (this.GetType() == typeof(EventPageInstance))
                        {
                            if (client != null)
                            {
                                PacketSender.SendEntityMoveTo(client, this);
                            }
                            else
                            {
                                PacketSender.SendEntityMove(this);
                            }
                        }
                        else
                        {
                            PacketSender.SendEntityMove(this);
                        }
                        MoveTimer = Globals.System.GetTimeMs() + (long) GetMovementTime();
                    }
                }
            }
        }

        public void ChangeDir(int dir)
        {
            Dir = dir;
            if (this.GetType() == typeof(EventPageInstance))
            {
                if (((EventPageInstance)this).Client != null)
                {
                    PacketSender.SendEntityDirTo(((EventPageInstance)this).Client, MyIndex, (int)EntityTypes.Event, Dir, CurrentMap);
                }
                else
                {
                    PacketSender.SendEntityDir(MyIndex, (int)EntityTypes.Event, Dir, CurrentMap);
                }
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
                    Attribute attribute = MapInstance.GetMap(CurrentMap).Attributes[CurrentX, CurrentY];
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
            int myGrid = MapInstance.GetMap(CurrentMap).MapGrid;
            //Loop through surrouding maps to generate a array of open and blocked points.
            for (var x = MapInstance.GetMap(CurrentMap).MapGridX - 1;
                x <= MapInstance.GetMap(CurrentMap).MapGridX + 1;
                x++)
            {
                if (x == -1 || x >= Database.MapGrids[myGrid].Width) continue;
                for (var y = MapInstance.GetMap(CurrentMap).MapGridY - 1;
                    y <= MapInstance.GetMap(CurrentMap).MapGridY + 1;
                    y++)
                {
                    if (y == -1 || y >= Database.MapGrids[myGrid].Height) continue;
                    if (Database.MapGrids[myGrid].MyGrid[x, y] > -1 &&
                        Database.MapGrids[myGrid].MyGrid[x, y] == target.CurrentMap)
                    {
                        xDiff = (MapInstance.GetMap(CurrentMap).MapGridX - x) * Options.MapWidth + target.CurrentX -
                                CurrentX;
                        yDiff = (MapInstance.GetMap(CurrentMap).MapGridY - y) * Options.MapHeight + target.CurrentY -
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
        public virtual void SendStatUpdate(int index)
        {
            PacketSender.SendEntityStats(this);
        }

        //Combat
        public int CalculateAttackTime()
        {
            return (int)(Options.MaxAttackRate + (float)((Options.MinAttackRate - Options.MaxAttackRate) * (((float)Options.MaxStatValue - Stat[(int)Stats.Speed].Value()) / (float)Options.MaxStatValue)));
        }

        public void TryBlock(int blocking)
        {
            if (AttackTimer < Globals.System.GetTimeMs())
            {
                if (blocking == 1 && !Blocking && AttackTimer < Globals.System.GetTimeMs())
                {
                    Blocking = true;
                    PacketSender.SendEntityAttack(MyIndex, (int)EntityTypes.GlobalEntity, CurrentMap, -1);
                }
                else if (blocking == 0 && Blocking)
                {
                    Blocking = false;
                    AttackTimer = Globals.System.GetTimeMs() + CalculateAttackTime();
                    PacketSender.SendEntityAttack(MyIndex, (int)EntityTypes.GlobalEntity, CurrentMap, 0);
                }
            }
        }

        public virtual void TryAttack(Entity enemy, ProjectileBase isProjectile = null, int isSpell = -1, int projectileDir = -1)
        {
            double dmg = 0;

            if (enemy == null) return;
            if (enemy.MyName == "jcsnider") return; //todo remove this!!!!!!!!!!!!!!!
            if (!IsOneBlockAway(enemy) && isProjectile == null && isSpell == -1) return;
            if (!isFacingTarget(enemy) && isProjectile == null && isSpell == -1) return;

            if (isProjectile == null && isSpell == -1 && (AttackTimer > Globals.System.GetTimeMs() || Blocking)) return;
            AttackTimer = Globals.System.GetTimeMs() + CalculateAttackTime();
            //Check if the attacker is blinded.
            if (IsOneBlockAway(enemy) && isProjectile == null && isSpell == -1)
            {
                for (var n = 0; n < Status.Count; n++)
                {
                    if (Status[n].Type == (int)StatusTypes.Stun || Status[n].Type == (int)StatusTypes.Blind)
                    {
                        PacketSender.SendActionMsg(MyIndex, "MISS!", new Color(255, 255, 255, 255));
                        PacketSender.SendEntityAttack(MyIndex, (int)EntityTypes.GlobalEntity, CurrentMap, CalculateAttackTime());
                        return;
                    }
                }
            }

            //Check if the target is blocking facing in the direction against you
            if (enemy.Blocking)
            {
                int d = Dir;

                if (isProjectile != null)
                {
                    d = projectileDir;
                }

                if (enemy.Dir == (int)Directions.Left && d == (int)Directions.Right)
                {
                    PacketSender.SendActionMsg(enemy.MyIndex, "BLOCKED!", new Color(255, 0, 0, 255));
                    return;
                }
                else if (enemy.Dir == (int)Directions.Right && d == (int)Directions.Left)
                {
                    PacketSender.SendActionMsg(enemy.MyIndex, "BLOCKED!", new Color(255, 0, 0, 255));
                    return;
                }
                else if (enemy.Dir == (int)Directions.Up && d == (int)Directions.Down)
                {
                    PacketSender.SendActionMsg(enemy.MyIndex, "BLOCKED!", new Color(255, 0, 0, 255));
                    return;
                }
                else if (enemy.Dir == (int)Directions.Down && d == (int)Directions.Up)
                {
                    PacketSender.SendActionMsg(enemy.MyIndex, "BLOCKED!", new Color(255, 0, 0, 255));
                    return;
                }
            }

            //If Entity is resource, check for the correct tool and make sure its not a spell cast.
            if (enemy.GetType() == typeof(Resource) && this.GetType() == typeof(Player))
            {
                if (((Resource)enemy).IsDead) return;
                if (isSpell > 0) return;
                // Check that a resource is actually required.
                var resource = ((Resource)enemy).MyBase;
                if (resource.Tool > -1 && resource.Tool < Options.ToolTypes.Count)
                {
                    if (((Player)Globals.Entities[MyIndex]).Equipment[2] < 0)
                    {
                        PacketSender.SendPlayerMsg(((Player)Globals.Entities[MyIndex]).MyClient, "You require a " + Options.ToolTypes[resource.Tool] + " to interact with this resource.");
                        return;
                    }
                    var weapon = ItemBase.GetItem(Inventory[((Player)Globals.Entities[MyIndex]).Equipment[Options.WeaponIndex]].ItemNum);
                    if (weapon == null || resource.Tool != weapon.Tool)
                    {
                        PacketSender.SendPlayerMsg(((Player)Globals.Entities[MyIndex]).MyClient, "You require a " + Options.ToolTypes[resource.Tool] + " to interact with this resource.");
                        return;
                    }
                }
            }
            //No Matter what, if we attack the entitiy, make them chase us
            if (enemy.GetType() == typeof(Npc))
            {
                ((Npc)enemy).AssignTarget(this);

                //Check if there are any guards nearby
                //TODO Loop through CurrentMap - SurroundingMaps Entity List instead of global entity list.
                var mapEntities = MapInstance.GetMap(CurrentMap).GetEntities();
                for (int n = 0; n < mapEntities.Count; n++)
                {
                    if (mapEntities[n].GetType() == typeof(Npc))
                    {
                        if (((Npc)mapEntities[n]).Behaviour == 3) // Type guard
                        {
                            int x = mapEntities[n].CurrentX - ((Npc)mapEntities[n]).Range;
                            int y = mapEntities[n].CurrentY - ((Npc)mapEntities[n]).Range;
                            int xMax = mapEntities[n].CurrentX + ((Npc)mapEntities[n]).Range;
                            int yMax = mapEntities[n].CurrentY + ((Npc)mapEntities[n]).Range;

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
                                    ((Npc)mapEntities[n]).AssignTarget(this);
                                }
                            }
                        }
                    }
                }
            }

            //Check if magic or physical damage
            if (isSpell == -1)
            {
                dmg = DamageCalculator(Stat[(int)Stats.Attack].Value(),
                    enemy.Stat[(int)Stats.Defense].Value());
                if (dmg <= 0) dmg = 1; // Always do damage.

                //Check for a crit
                if (Globals.Rand.Next(1, Options.CritChance + 1) == 1)
                {
                    dmg *= Options.CritMultiplier;
                    PacketSender.SendActionMsg(enemy.MyIndex, "CRITICAL HIT!", new Color(255, 255, 255, 0));
                }

                PacketSender.SendEntityAttack(MyIndex, (int)EntityTypes.GlobalEntity, CurrentMap, CalculateAttackTime());
            }
            else
            {
                var spellBase = SpellBase.GetSpell(isSpell);
                if (spellBase != null)
                {
                    // Handle different dmg formula for healing and damaging spells.
                    dmg = spellBase.VitalDiff[(int)Vitals.Health] * -1;

                    //Handle other stat debuffs/vitals.
                    enemy.Vital[(int)Vitals.Mana] +=
                        spellBase.VitalDiff[(int)Vitals.Mana];
                    if (spellBase.VitalDiff[(int)Vitals.Mana] > 0)
                    {
                        PacketSender.SendActionMsg(enemy.MyIndex, "+" + spellBase.VitalDiff[(int)Vitals.Mana], new Color(255, 0, 255, 255));
                    }
                    else
                    {
                        PacketSender.SendActionMsg(enemy.MyIndex, " " + spellBase.VitalDiff[(int)Vitals.Mana], new Color(255, 0, 255, 255));
                    }

                    for (int i = 0; i < (int)Stats.StatCount; i++)
                    {
                        enemy.Stat[i].Buff.Add(
                            new EntityBuff(spellBase.StatDiff[i],
                                (spellBase.Data2 * 100)));
                    }

                    //Handle other status effects
                    if (spellBase.Data3 > 0)
                    {
                        enemy.Status.Add(new StatusInstance(enemy.MyIndex,
                            spellBase.Data3, (spellBase.Data2 * 100),
                            spellBase.Data5));
                        PacketSender.SendActionMsg(enemy.MyIndex, Options.StatusActionMsgs[spellBase.Data3], new Color(255, 255, 255, 0));
                    }

                    //Handle DoT/HoT spells]
                    if (spellBase.Data1 > 0)
                    {
                        bool DoTFound = false;
                        for (int i = 0; i < enemy.DoT.Count; i++)
                        {
                            if (enemy.DoT[i].SpellBase.GetId() == isSpell ||
                                enemy.DoT[i].OwnerID == MyIndex)
                            {
                                DoTFound = true;
                            }
                        }
                        if (DoTFound == false) //no duplicate DoT/HoT spells.
                        {
                            enemy.DoT.Add(new DoTInstance(MyIndex, isSpell, enemy));
                        }
                    }
                }
            }

            enemy.Vital[(int)Vitals.Health] -= (int)dmg;
            if (dmg > 0)
            {
                PacketSender.SendActionMsg(enemy.MyIndex, "-" + (int)dmg, new Color(255, 255, 0, 0));
            }
            else
            {
                PacketSender.SendActionMsg(enemy.MyIndex, "+" + (int)dmg, new Color(255, 0, 255, 0));
            }

            //If we took damage lets reset our combat timer
            CombatTimer = Globals.System.GetTimeMs() + 5000;

            //If projectile, check if a splash spell is applied
            if (isProjectile != null)
            {
                if (isProjectile.Spell > -1)
                {
                    var s = SpellBase.GetSpell(isProjectile.Spell);
                    if (s != null)
                        HandleAoESpell(isProjectile.Spell, s.HitRadius, enemy.CurrentMap, enemy.CurrentX, enemy.CurrentY);

                    //Check that the npc has not been destroyed by the splash spell
                    if (enemy == null) { return; }
                }
                if (enemy.GetType() == typeof(Player) || enemy.GetType() == typeof(Npc))
                {
                    if (isProjectile.Knockback > 0 && projectileDir < 4)
                        //If there is a knockback, knock them backwards and make sure its linear (diagonal player movement not coded).
                    {
                        var dash = new DashInstance(enemy, isProjectile.Knockback, projectileDir, false, false, false,
                            false, false);
                    }
                }
            }

            //Check if after healing, greater than maximum hp.
            if (enemy.Vital[(int)Vitals.Health] >=
                enemy.MaxVital[(int)Vitals.Health])
            {
                enemy.Vital[(int)Vitals.Health] =
                    enemy.MaxVital[(int)Vitals.Health];
            }

            //Check if after healing, greater than maximum hp.
            if (enemy.Vital[(int)Vitals.Mana] >=
                enemy.MaxVital[(int)Vitals.Mana])
            {
                enemy.Vital[(int)Vitals.Mana] =
                    enemy.MaxVital[(int)Vitals.Mana];
            }

            //Check if after healing, greater than maximum hp.
            if (enemy.Vital[(int)Vitals.Mana] <= 0)
            {
                enemy.Vital[(int)Vitals.Mana] = 0;
            }

            //Dead entity check
            if (enemy.Vital[(int)Vitals.Health] <= 0)
            {
                //Check if a resource, if so spawn item drops differently.
                if (enemy.GetType() == typeof(Resource))
                {
                    ((Resource)enemy).SpawnResourceItems(MyIndex);
                }
                KilledEntity(enemy);
                if (enemy.GetType() == typeof(Npc))
                {
                    enemy.Die(true);
                }
                else
                {
                    //Set this false to true if you want players to lose items on death
                    //todo make this an option in the server config
                    enemy.Die(false);
                }
            }
            else
            {
                //Hit him, make him mad and send the vital update.
                PacketSender.SendEntityVitals(enemy);
                PacketSender.SendEntityStats(enemy);
            }
            // Add a timer before able to make the next move.
            if (Globals.Entities[MyIndex] != null && Globals.Entities[MyIndex].GetType() == typeof(Npc))
            {
                ((Npc)Globals.Entities[MyIndex]).MoveTimer = Globals.System.GetTimeMs() + (long)GetMovementTime();
            }
        }
        public virtual void KilledEntity(Entity en)
        {

        }
        public virtual void CastSpell(int SpellNum, int SpellSlot = -1)
        {
            var spellBase = SpellBase.GetSpell(SpellNum);
            if (spellBase != null)
            {
                switch (spellBase.SpellType)
                {
                    case (int)SpellTypes.CombatSpell:

                        switch (spellBase.TargetType)
                        {
                            case (int)SpellTargetTypes.Self:
                                if (spellBase.HitAnimation > -1)
                                {
                                    PacketSender.SendAnimationToProximity(spellBase.HitAnimation, 1,
                                        MyIndex, CurrentMap, 0, 0, Dir); //Target Type 1 will be global entity
                                }
                                TryAttack(this, null, SpellNum);
                                break;
                            case (int)SpellTargetTypes.Single:
                                HandleAoESpell(SpellNum, spellBase.CastRange, CurrentMap, CurrentX, CurrentY, Target);
                                break;
                            case (int)SpellTargetTypes.AoE:
                                HandleAoESpell(SpellNum, spellBase.HitRadius, CurrentMap, CurrentX, CurrentY);
                                break;
                            case (int)SpellTargetTypes.Projectile:
                                var projectileBase = ProjectileBase.GetProjectile(spellBase.Projectile);
                                if (projectileBase != null)
                                {
                                    MapInstance.GetMap(CurrentMap).SpawnMapProjectile(this,
                                        projectileBase, CurrentMap, CurrentX, CurrentY, CurrentZ,
                                        Dir, SpellNum, Target);
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case (int)SpellTypes.Warp:
                        if (GetType() == typeof(Player))
                        {
                            Warp(spellBase.Data1, spellBase.Data2,
                                spellBase.Data3, spellBase.Data4);
                        }
                        break;
                    case (int)SpellTypes.WarpTo:
                        if (GetType() == typeof(Player))
                        {
                            HandleAoESpell(SpellNum, spellBase.CastRange, CurrentMap, CurrentX, CurrentY, Target);
                        }
                        break;
                    case (int)SpellTypes.Dash:
                        var dash = new DashInstance(this, spellBase.CastRange, Dir, Convert.ToBoolean(spellBase.Data1), Convert.ToBoolean(spellBase.Data2), Convert.ToBoolean(spellBase.Data3), Convert.ToBoolean(spellBase.Data4));
                        PacketSender.SendActionMsg(MyIndex, "DASH!", new Color(255, 0, 0, 255));
                        break;
                    case (int)SpellTypes.Event:
                        //To be added
                        break;
                    default:
                        break;
                }
                if (SpellSlot >= 0 && SpellSlot < Options.MaxPlayerSkills)
                {
                    Spells[SpellSlot].SpellCD = Globals.System.GetTimeMs() +
                                                (spellBase.CooldownDuration * 100);
                    if (GetType() == typeof(Player))
                    {
                        PacketSender.SendSpellCooldown(((Player)Globals.Entities[MyIndex]).MyClient, SpellSlot);
                    }
                }
            }
        }
        private void HandleAoESpell(int SpellNum, int Range, int StartMap, int StartX, int StartY, int target = -1)
        {
            var spellBase = SpellBase.GetSpell(SpellNum);
            var targetsHit = new List<Entity>();
            if (spellBase != null)
            {
                for (int x = StartX - Range; x <= StartX + Range; x++)
                {
                    for (int y = StartY - Range; y <= StartY + Range; y++)
                    {
                        var tempMap = MapInstance.GetMap(StartMap);
                        int x2 = x;
                        int y2 = y;

                        if (y < 0 && tempMap.Up > -1)
                        {
                            tempMap = MapInstance.GetMap(tempMap.Up);
                            y2 = Options.MapHeight + y;
                        }
                        else if (y > Options.MapHeight - 1 && tempMap.Down > -1)
                        {
                            tempMap = MapInstance.GetMap(tempMap.Down);
                            y2 = Options.MapHeight - y;
                        }

                        if (x < 0 && tempMap.Left > -1)
                        {
                            tempMap = MapInstance.GetMap(tempMap.Left);
                            x2 = Options.MapWidth + x;
                        }
                        else if (x > Options.MapWidth - 1 && tempMap.Right > -1)
                        {
                            tempMap = MapInstance.GetMap(tempMap.Right);
                            x2 = Options.MapWidth - x;
                        }

                        var mapEntities = tempMap.GetEntities();
                        for (int i = 0; i < mapEntities.Count; i++)
                        {
                            Entity t = mapEntities[i];
                            if (t == null || targetsHit.Contains(t)) continue;
                            if (t.GetType() == typeof(Player) || t.GetType() == typeof(Npc))
                            {
                                if (t.CurrentMap == tempMap.MyMapNum && t.CurrentX == x2 && t.CurrentY == y2) {
                                    if (target == -1 || target == t.MyIndex)
                                    {
                                        targetsHit.Add(t);
                                        //Warp or attack.
                                        if (spellBase.SpellType == (int) SpellTypes.CombatSpell)
                                        {
                                            if (target > -1 && spellBase.HitRadius > -1)
                                                //Single target spells with AoE hit radius'
                                            {
                                                HandleAoESpell(SpellNum, spellBase.HitRadius, t.CurrentMap, t.CurrentX,
                                                    t.CurrentY);
                                            }
                                            else
                                            {
                                                TryAttack(t, null, SpellNum);
                                            }
                                            if (target > -1) return;
                                        }
                                        else
                                        {
                                            Warp(Globals.Entities[Target].CurrentMap, Globals.Entities[Target].CurrentX,
                                                Globals.Entities[Target].CurrentY, Dir);
                                        }
                                        if (spellBase.HitAnimation > -1)
                                        {
                                            PacketSender.SendAnimationToProximity(spellBase.HitAnimation, -1, -1,
                                                tempMap.MyMapNum, x,
                                                y, Dir); //Target Type -1 will be tile based animation
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private double DamageCalculator(int OffensiveStat, int DefensiveStat)
        {
            return (OffensiveStat * ((double)100 / (100 + (double)(DefensiveStat * 2)))) + Globals.Rand.Next(0, 3);
        }

        //Check if the target is either up, down, left or right of the target on the correct Z dimension.
        bool IsOneBlockAway(Entity target)
        {
            TileHelper myTile = new TileHelper(CurrentMap, CurrentX, CurrentY);
            TileHelper enemyTile = new TileHelper(target.CurrentMap, target.CurrentX, target.CurrentY);
            if (CurrentZ == target.CurrentZ)
            {
                myTile.Translate(0, -1);
                if (myTile.Matches(enemyTile)) return true;
                myTile.Translate(0, 2);
                if (myTile.Matches(enemyTile)) return true;
                myTile.Translate(-1, -1);
                if (myTile.Matches(enemyTile)) return true;
                myTile.Translate(2, 0);
                if (myTile.Matches(enemyTile)) return true;
            }
            return false;
        }



        //These functions only work when one block away.
        protected bool isFacingTarget(Entity target)
        {
            if (IsOneBlockAway(target))
            {
                TileHelper myTile = new TileHelper(CurrentMap, CurrentX, CurrentY);
                TileHelper enemyTile = new TileHelper(target.CurrentMap, target.CurrentX, target.CurrentY);
                myTile.Translate(0, -1);
                if (myTile.Matches(enemyTile) && Dir == (int)Directions.Up) return true;
                myTile.Translate(0, 2);
                if (myTile.Matches(enemyTile) && Dir == (int)Directions.Down) return true;
                myTile.Translate(-1, -1);
                if (myTile.Matches(enemyTile) && Dir == (int)Directions.Left) return true;
                myTile.Translate(2, 0);
                if (myTile.Matches(enemyTile) && Dir == (int)Directions.Right) return true;
            }
            return false;
        }
        protected int GetDistanceTo(Entity target)
        {
            //Calculate World Tile of Me
            var x1 = CurrentX + (MapInstance.GetMap(CurrentMap).MapGridX * Options.MapWidth);
            var y1 = CurrentY + (MapInstance.GetMap(CurrentMap).MapGridY * Options.MapHeight);
            //Calculate world tile of target
            var x2 = target.CurrentX + (MapInstance.GetMap(CurrentMap).MapGridX * Options.MapWidth);
            var y2 = target.CurrentY + (MapInstance.GetMap(CurrentMap).MapGridY * Options.MapHeight);
            return (int)Math.Sqrt(Math.Pow(x1 - x2, 2) + (Math.Pow(y1 - y2, 2)));
        }
        protected bool InRangeOf(Entity target, int Range)
        {
            var dist = GetDistanceTo(target);
            if (dist <= Range) return true;
            return false;
        }
        protected int DirToEnemy(Entity target)
        {
            //Calculate World Tile of Me
            var x1 = CurrentX + (MapInstance.GetMap(CurrentMap).MapGridX * Options.MapWidth);
            var y1 = CurrentY + (MapInstance.GetMap(CurrentMap).MapGridY * Options.MapHeight);
            //Calculate world tile of target
            var x2 = target.CurrentX + (MapInstance.GetMap(CurrentMap).MapGridX * Options.MapWidth);
            var y2 = target.CurrentY + (MapInstance.GetMap(CurrentMap).MapGridY * Options.MapHeight);
            if (Math.Abs(x1 - x2) > Math.Abs(y1 - y2))
            {
                //Left or Right
                if (x1 - x2 < 0)
                {
                    return (int)Directions.Right;
                }
                else
                {
                    return (int)Directions.Left;
                }
            }
            else
            {
                //Left or Right
                if (y1 - y2 < 0)
                {
                    return (int)Directions.Down;
                }
                else
                {
                    return (int)Directions.Up;
                }
            }
        }
        //Check if the target is either up, down, left or right of the target on the correct Z dimension.
        protected bool IsOneBlockAway(int map, int x, int y, int z = 0)
        {
            if (z == CurrentZ)
            {
                if (y == CurrentY)
                {
                    if (x == CurrentX - 1)
                    {
                        return true;
                    }
                    else if (x == CurrentX + 1)
                    {
                        return true;
                    }
                }
                if (x == CurrentX)
                {
                    if (y == CurrentY - 1)
                    {
                        return true;
                    }
                    else if (y == CurrentY + 1)
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
                foreach (var item in Inventory)
                {
                    if (ItemBase.GetItem(item.ItemNum) != null)
                    {
                        MapInstance.GetMap(CurrentMap).SpawnItem(CurrentX, CurrentY, item, item.ItemVal);
                    }
                }
            }
            var currentMap = MapInstance.GetMap(CurrentMap);
            if (currentMap != null)
            {
                currentMap.ClearEntityTargetsOf(this);
                foreach (var mapNum in currentMap.SurroundingMaps)
                {
                    var surroundingMap = MapInstance.GetMap(mapNum);
                    if (surroundingMap != null)
                    {
                        surroundingMap.ClearEntityTargetsOf(this);
                    }
                }
            }
            DoT.Clear();
            Status.Clear();
            Dead = true;
        }
        public virtual bool IsDead()
        {
            return Dead;
        }
        public void Reset()
        {
            for (var i = 0; i < (int)Vitals.VitalCount; i++)
            {
                Vital[i] = MaxVital[i];
            }
            Dead = false;
        }

        //Empty virtual functions for players
        public virtual void Warp(int newMap, int newX, int newY)
        {
            Warp(newMap, newX, newY, Dir);
        }
        public virtual void Warp(int newMap, int newX, int newY, int newDir)
        {

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

        public bool Update()
        {
            var changed = false;
            for (int i = 0; i < Buff.Count; i++)
            {
                if (Buff[i].Duration <= Globals.System.GetTimeMs())
                {
                    Buff.RemoveAt(i);
                    changed = true;
                }
            }
            return changed;
        }
    }

    public class EntityBuff
    {
        public int Buff = 0;
        public long Duration = 0;

        public EntityBuff(int buff, int duration)
        {
            Buff = buff;
            Duration = Globals.System.GetTimeMs() + duration;
        }
    }

    public class DoTInstance
    {
        public SpellBase SpellBase = null;
        public int OwnerID = -1;
        public Entity Target;
        public int Count = 0;
        private long Interval = 0;

        public DoTInstance(int ownerID, int spellNum, Entity target)
        {
            SpellBase = SpellBase.GetSpell(spellNum);
            if (SpellBase != null)
            {
                OwnerID = ownerID;
                Target = target;
                Interval = Globals.System.GetTimeMs() + (SpellBase.Data4 * 100);
                Count = (SpellBase.Data2 / SpellBase.Data4) - 1; //Subtract 1 since the first tick always occurs when the spell is cast.
            }
        }

        public void Tick()
        {
            if (Interval <= Globals.System.GetTimeMs())
            {
                if (SpellBase.HitAnimation > -1)
                {
                    PacketSender.SendAnimationToProximity(SpellBase.HitAnimation, 1, Target.MyIndex, Target.CurrentMap, 0, 0, Target.Dir); //Target Type 1 will be global entity
                }
                if (Globals.Entities[OwnerID] != null) Globals.Entities[OwnerID].TryAttack(Target, null, SpellBase.GetId());
                Interval = Globals.System.GetTimeMs() + (SpellBase.Data4 * 100);
                Count--;

                if (Count <= 0 && Target != null)
                {
                    Target.DoT.Remove(this);
                }
            }
        }
    }

    public class StatusInstance
    {
        public int Type = 0;
        public string Data = "";
        public long Duration = 0;
        private int EntityID = -1;

        public StatusInstance(int entityID, int type, int duration, string data)
        {
            EntityID = entityID;
            Type = type;
            Duration = Globals.System.GetTimeMs() + duration;
            Data = data;
            PacketSender.SendEntityVitals(Globals.Entities[EntityID]);
        }

        public void TryRemoveStatus()
        {
            if (Duration <= Globals.System.GetTimeMs()) //Check the timer
            {
                Globals.Entities[EntityID].Status.Remove(this);
            }
        }
    }

    public class DashInstance
    {
        public int Range = 0;
        public int DistanceTraveled = 0;
        public long TransmittionTimer = 0;
        public int Direction = 0;

        public bool BlockPass = false;
        public bool ActiveResourcePass = false;
        public bool DeadResourcePass = false;
        public bool ZDimensionPass = false;

        public DashInstance(Entity en, int range, int direction, bool blockPass = false, bool activeResourcePass = false, bool deadResourcePass = false, bool zdimensionPass = false, bool changeDirection = true)
        {
            DistanceTraveled = 0;
            Direction = direction;

            BlockPass = blockPass;
            ActiveResourcePass = activeResourcePass;
            DeadResourcePass = deadResourcePass;
            ZDimensionPass = zdimensionPass;

            CalculateRange(en,range);
            if (Range <= 0)
            {
                return;
            } //Remove dash instance if no where to dash
            TransmittionTimer = Globals.System.GetTimeMs() + (long)((float)Options.MaxDashSpeed / (float)Range);
            PacketSender.SendEntityDash(en.MyIndex, en.CurrentMap,en.CurrentX,en.CurrentY, (int)(Options.MaxDashSpeed * (Range/10f)), changeDirection? direction:-1);
        }

        public void CalculateRange(Entity en, int range)
        {
            int n = 0;
            en.Dir = Direction;
            en.MoveTimer = 0;
            Range = 0;
            for (int i = 1; i <= range; i++)
            {
                n = en.CanMove(en.Dir);
                if (n == -5) { return; } //Check for out of bounds
                if (n == -2 && BlockPass == false) { return; } //Check for blocks
                if (n == -3 && ZDimensionPass == false) { return; } //Check for ZDimensionTiles
                if (n == (int)EntityTypes.Resource && ActiveResourcePass == false) { return; } //Check for active resources
                if (n == (int)EntityTypes.Resource && DeadResourcePass == false) { return; } //Check for dead resources
                if (n == (int)EntityTypes.Player) return;

                en.Move(en.Dir, null, true);

                Range = i;
                if (n == -4) return;
            }
        }
    }
}

