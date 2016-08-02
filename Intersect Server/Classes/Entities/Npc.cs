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
using Intersect_Library;
using Intersect_Library.GameObjects;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Maps;
using Intersect_Server.Classes.Misc;
using Intersect_Server.Classes.Misc.Pathfinding;
using Intersect_Server.Classes.Networking;
using Intersect_Server.Classes.Spells;
using System.Collections.Generic;

namespace Intersect_Server.Classes.Entities
{
    public class Npc : Entity
    {
        //Targetting
        public Entity MyTarget = null;
        public NpcBase MyBase = null;

        //Pathfinding
        private Pathfinder pathFinder;

        //Temporary Values
        private int _curMapLink = -1;

        //Moving
        public long LastRandomMove;

        //Respawn
        public long RespawnTime;

        //Behaviour
        public byte Behaviour = 0;
        public byte Range = 0;

        //Spell casting
        public long CastFreq = 0;

        public Npc(int index, NpcBase myBase)
            : base(index)
        {
            MyName = myBase.Name;
            MySprite = myBase.Sprite;
            MyBase = myBase;

            for (int I = 0; I < (int)Stats.StatCount; I++)
            {
                Stat[I] = new EntityStat(myBase.Stat[I]);
            }

            for (int I = 0; I < MyBase.Spells.Count; I++)
            {
                Spells.Add(new SpellInstance(MyBase.Spells[I]));
            }

            myBase.MaxVital.CopyTo(Vital, 0);
            myBase.MaxVital.CopyTo(MaxVital, 0);
            Behaviour = myBase.Behavior;
            Range = (byte)myBase.SightRange;
            pathFinder = new Pathfinder(this);
        }

        public override void Die(bool dropitems = false)
        {
            base.Die(dropitems);

            MapInstance.GetMap(CurrentMap).RemoveEntity(this);
            PacketSender.SendEntityLeave(MyIndex, (int)EntityTypes.GlobalEntity, CurrentMap);
            Globals.Entities[MyIndex] = null;
        }

        //Targeting
        public void AssignTarget(int Target)
        {
            if (Globals.Entities[Target].GetType() == typeof(Projectile))
            {
                MyTarget = ((Projectile)Globals.Entities[Target]).Owner;
            }
            else
            {
                MyTarget = Globals.Entities[Target];
            }
        }

        public bool CanAttack()
        {
            //Check if the attacker is stunned or blinded.
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == (int)StatusTypes.Stun)
                {
                    return false;
                }
            }
            return true;
        }

        public void RemoveTarget()
        {
            MyTarget = null;
        }

        private void TryCastSpells()
        {
            //check if NPC is snared or stunned
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == (int)StatusTypes.Stun ||
                    Status[n].Type == (int)StatusTypes.Snare)
                {
                    return;
                }
            }
            //Check if NPC is dashing
            if (Dashing != null)
            {
                return;
            }
            //Check if NPC is casting a spell
            if (CastTime > Globals.System.GetTimeMs())
            {
                return; //can't move while casting
            }
            else if (CastFreq < Globals.System.GetTimeMs())//Try to cast a new spell
            {
                var CC = false;
                //Check if the NPC is silenced or stunned
                for (var n = 0; n < Status.Count; n++)
                {
                    if (Status[n].Type == (int)StatusTypes.Silence || Status[n].Type == (int)StatusTypes.Stun)
                    {
                        CC = true;
                        break;
                    }
                }

                if (CC == false)
                {
                    if (MyBase.Spells.Count > 0)
                    {
                        var s = Globals.Rand.Next(0, MyBase.Spells.Count); //Pick a random spell
                        var spell = SpellBase.GetSpell((MyBase.Spells[s]));
                        if (spell.SpellType == (int)SpellTypes.CombatSpell && spell.TargetType == (int)SpellTargetTypes.Projectile && InRangeOf(MyTarget,spell.CastRange))
                        {
                            if (DirToEnemy(MyTarget) != Dir)
                            {
                                if (LastRandomMove >= Globals.System.GetTimeMs()) return;
                                var dirToEnemy = DirToEnemy(MyTarget);
                                if (dirToEnemy != -1)
                                {
                                    //Face the target -- next frame fire -- then go on with life
                                    ChangeDir(dirToEnemy); // Gotta get dir to enemy
                                    LastRandomMove = Globals.System.GetTimeMs() + Globals.Rand.Next(1000, 3000);
                                }
                                return;
                            }
                        }

                        if (spell.VitalCost[(int)Vitals.Mana] <= Vital[(int)Vitals.Mana])
                        {
                            if (spell.VitalCost[(int)Vitals.Health] <= Vital[(int)Vitals.Health])
                            {
                                if (Spells[s].SpellCD < Globals.System.GetTimeMs())
                                {
                                    Vital[(int)Vitals.Mana] = Vital[(int)Vitals.Mana] -
                                                               spell.VitalCost[(int)Vitals.Mana];
                                    Vital[(int)Vitals.Health] = Vital[(int)Vitals.Health] -
                                                                 spell.VitalCost[(int)Vitals.Health
                                                                     ];
                                    CastTime = Globals.System.GetTimeMs() + (spell.CastDuration * 100);

                                    switch (MyBase.SpellFrequency)
                                    {
                                        case 0:
                                            CastFreq = Globals.System.GetTimeMs() + 30000;
                                            break;
                                        case 1:
                                            CastFreq = Globals.System.GetTimeMs() + 15000;
                                            break;
                                        case 2:
                                            CastFreq = Globals.System.GetTimeMs() + 8000;
                                            break;
                                        case 3:
                                            CastFreq = Globals.System.GetTimeMs() + 4000;
                                            break;
                                        case 4:
                                            CastFreq = Globals.System.GetTimeMs() + 2000;
                                            break;
                                    }

                                    SpellCastSlot = s;

                                    if (spell.CastAnimation > -1)
                                    {
                                        PacketSender.SendAnimationToProximity(spell.CastAnimation, 1,
                                            MyIndex, CurrentMap, 0, 0, Dir);
                                        //Target Type 1 will be global entity
                                    }

                                    PacketSender.SendEntityVitals(MyIndex, (int)Vitals.Health,
                                        Globals.Entities[MyIndex]);
                                    PacketSender.SendEntityVitals(MyIndex, (int)Vitals.Mana,
                                        Globals.Entities[MyIndex]);
                                    PacketSender.SendEntityCastTime(MyIndex, (MyBase.Spells[s]));
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void Update()
        {
            base.Update();

            //Process dash spells
            if (Dashing != null)
            {
                Dashing.Update();
                return;
            }
            if (MoveTimer < Globals.System.GetTimeMs())
            {
                var targetMap = -1;
                var targetX = 0;
                var targetY = 0;
                //Check if there is a target, if so, run their ass down.
                if (MyTarget != null)
                {
                    if (!MyTarget.IsDead())
                    {
                        targetMap = MyTarget.CurrentMap;
                        targetX = MyTarget.CurrentX;
                        targetY = MyTarget.CurrentY;
                        for (var n = 0; n < MyTarget.Status.Count; n++)
                        {
                            if (MyTarget.Status[n].Type == (int)StatusTypes.Stealth)
                            {
                                targetMap = -1;
                                targetX = 0;
                                targetY = 0;
                            }
                        }
                    }
                    else
                    {
                        MyTarget = null;
                    }
                }
                else //Find a target if able
                {
                    if (Behaviour == (int)NpcBehavior.AttackOnSight || MyBase.AggroList.Count > -1) // Check if attack on sight or have other npc's to target
                    {
                        var maps = MapInstance.GetMap(CurrentMap).GetSurroundingMaps(true);
                        var possibleTargets = new List<Entity>();
                        int closestRange = Range+1; //If the range is out of range we didn't find anything.
                        int closestIndex = -1;
                        foreach (var map in maps)
                        {
                            foreach (var entity in map.GetEntities())
                            {
                                if (entity.IsDead() == false && entity != this)
                                {
                                    if ((entity.GetType() == typeof(Player)) || (entity.GetType() == typeof(Npc) && MyBase.AggroList.Contains(((Npc)entity).MyBase.GetId())))
                                    {
                                        var dist = GetDistanceTo(entity);
                                        if (dist <= Range && dist < closestRange)
                                        {
                                            possibleTargets.Add(entity);
                                            closestIndex = possibleTargets.Count - 1;
                                            closestRange = dist;
                                        }
                                    }
                                }
                            }
                        }
                        if (closestIndex != -1)
                        {
                            MyTarget = possibleTargets[closestIndex];
                        }
                    }
                }

                if (targetMap > -1)
                {
                    //Check if target map is on one of the surrounding maps, if not then we are not even going to look.
                    if (targetMap != CurrentMap)
                    {
                        if (MapInstance.GetMap(CurrentMap).SurroundingMaps.Count > 0)
                        {
                            for (var x = 0; x < MapInstance.GetMap(CurrentMap).SurroundingMaps.Count; x++)
                            {
                                if (MapInstance.GetMap(CurrentMap).SurroundingMaps[x] == targetMap)
                                {
                                    break;
                                }
                                if (x == MapInstance.GetMap(CurrentMap).SurroundingMaps.Count - 1)
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

                if (targetMap > -1)
                {
                    if (pathFinder.GetTarget() != null)
                    {
                        if (targetMap != pathFinder.GetTarget().TargetMap || targetX != pathFinder.GetTarget().TargetX || targetY != pathFinder.GetTarget().TargetY)
                        {
                            pathFinder.SetTarget(null);
                        }
                    }

                    if (pathFinder.GetTarget() != null)
                    {
                        TryCastSpells();
                        if (!IsOneBlockAway(pathFinder.GetTarget().TargetMap, pathFinder.GetTarget().TargetX, pathFinder.GetTarget().TargetY))
                        {
                            var dir = pathFinder.GetMove();
                            if (dir > -1)
                            {
                                if (CanMove(dir) == -1 || CanMove(dir) == -4)
                                {
                                    Move(dir, null);
                                    pathFinder.RemoveMove();
                                }
                                else
                                {
                                    pathFinder.SetTarget(null);
                                }
                            }
                            else
                            {
                                targetMap = -1;
                            }
                        }
                        else
                        {
                            if (Dir != DirToEnemy(MyTarget) && DirToEnemy(MyTarget) != -1)
                            {
                                ChangeDir(DirToEnemy(MyTarget));
                            }
                            else
                            {
                                if (CanAttack()) TryAttack(MyTarget.MyIndex, null, -1, -1);
                            }
                        }
                    }
                    else
                    {
                        pathFinder.SetTarget(new PathfinderTarget(targetMap, targetX, targetY));
                        if (Dir != DirToEnemy(MyTarget) && DirToEnemy(MyTarget) != -1)
                        {
                            ChangeDir(DirToEnemy(MyTarget));
                        }
                        else
                        {
                            if (CanAttack()) TryAttack(MyTarget.MyIndex, null, -1, -1);
                        }
                    }
                }

                //Move randomly
                if (targetMap != -1) return;
                if (LastRandomMove >= Globals.System.GetTimeMs()) return;
                if (MyBase.Behavior == (int)NpcBehavior.Guard)
                {
                    LastRandomMove = Globals.System.GetTimeMs() + Globals.Rand.Next(1000, 3000);
                    return;
                }
                var i = Globals.Rand.Next(0, 1);
                if (i == 0)
                {
                    i = Globals.Rand.Next(0, 4);
                    if (CanMove(i) == -1)
                    {
                        //check if NPC is snared or stunned
                        for (var n = 0; n < Status.Count; n++)
                        {
                            if (Status[n].Type == (int)StatusTypes.Stun || Status[n].Type == (int)StatusTypes.Snare)
                            {
                                return;
                            }
                        }
                        //Check if NPC is dashing
                        if (Dashing != null)
                        {
                            return;
                        }
                        Move(i, null);
                    }
                }
                LastRandomMove = Globals.System.GetTimeMs() + Globals.Rand.Next(1000, 3000);
            }
            //If we switched maps, lets update the maps
            if (_curMapLink != CurrentMap)
            {
                if (_curMapLink != -1)
                {
                    MapInstance.GetMap(_curMapLink).RemoveEntity(this);
                }
                if (CurrentMap > -1)
                {
                    MapInstance.GetMap(CurrentMap).AddEntity(this);
                }
                _curMapLink = CurrentMap;
            }
        }
    }
}