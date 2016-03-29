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
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Misc;

namespace Intersect_Server.Classes
{
    public class Npc : Entity
    {
        //Targetting
        public Entity MyTarget = null;

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

        public Npc(int index, NpcStruct myBase)
            : base(index)
        {
            MyName = myBase.Name;
            MySprite = myBase.Sprite;

            for (int I = 0; I < (int)Enums.Stats.StatCount; I++)
            {
                Stat[I] = new EntityStat(myBase.Stat[I]);
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

            Globals.GameMaps[CurrentMap].RemoveEntity(this);
            PacketSender.SendEntityLeave(MyIndex, (int)Enums.EntityTypes.GlobalEntity, CurrentMap);
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

        public override void Update()
        {
            base.Update();
            if (MoveTimer < Environment.TickCount)
            {
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
                else //Find a target if able
                {
                    if (Behaviour == 1) // Check if attack on sight
                    {
                        int x = CurrentX - Range;
                        int y = CurrentY - Range;
                        int xMax = CurrentX + Range;
                        int yMax = CurrentY + Range;

                        //Check that not going out of the map boundaries
                        if (x < 0) x = 0;
                        if (y < 0) y = 0;
                        if (xMax >= Options.MapWidth) xMax = Options.MapWidth;
                        if (yMax >= Options.MapHeight) yMax = Options.MapHeight;

                        //TODO base this off of the entity array of surrounding maps, not the whole global list.
                        for (int n = 0; n < Globals.GameMaps[CurrentMap].Entities.Count; n++)
                        {
                            if (Globals.GameMaps[CurrentMap].Entities[n].GetType() == typeof(Player))
                            {
                                if (x < Globals.GameMaps[CurrentMap].Entities[n].CurrentX && xMax > Globals.GameMaps[CurrentMap].Entities[n].CurrentX)
                                {
                                    if (y < Globals.GameMaps[CurrentMap].Entities[n].CurrentY && yMax > Globals.GameMaps[CurrentMap].Entities[n].CurrentY)
                                    {
                                        // In range, so make a target
                                        MyTarget = Globals.GameMaps[CurrentMap].Entities[n];
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (targetMap > -1)
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
                        var dir = pathFinder.GetMove();
                        if (dir > -1)
                        {
                            if (CanMove(dir) == -1)
                            {
                                Move(dir, null);
                                pathFinder.RemoveMove();
                            }
                            else
                            {
                                pathFinder.SetTarget(null);
                            }
                        }
                    }
                else
                    {
                        pathFinder.SetTarget(new PathfinderTarget(targetMap, targetX, targetY));
                        TryAttack(MyTarget.MyIndex);
                    }
                }

                //Move randomly
                if (targetMap != -1) return;
                if (LastRandomMove >= Environment.TickCount) return;
                var i = Globals.Rand.Next(0, 1);
                if (i == 0)
                {
                    i = Globals.Rand.Next(0, 4);
                    if (CanMove(i) == -1)
                    {
                        Move(i, null);
                    }
                }
                LastRandomMove = Environment.TickCount + Globals.Rand.Next(1000, 3000);
            }
            //If we switched maps, lets update the maps
            if (_curMapLink != CurrentMap)
            {
                if (_curMapLink != -1)
                {
                    Globals.GameMaps[_curMapLink].RemoveEntity(this);
                }
                if (CurrentMap > -1)
                {
                    Globals.GameMaps[CurrentMap].AddEntity(this);
                }
                _curMapLink = CurrentMap;
            }
        }
    }


}