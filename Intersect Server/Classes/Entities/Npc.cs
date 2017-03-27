using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intersect;
using Intersect.GameObjects;
using Intersect_Server.Classes.General;
using Intersect_Server.Classes.Maps;
using Intersect_Server.Classes.Misc;
using Intersect_Server.Classes.Misc.Pathfinding;
using Intersect_Server.Classes.Networking;
using Intersect_Server.Classes.Spells;

namespace Intersect_Server.Classes.Entities
{
    public class Npc : Entity
    {
        //Behaviour
        public byte Behaviour = 0;

        //Spell casting
        public long CastFreq = 0;
        public bool Despawnable;

        //Moving
        public long LastRandomMove;
        public NpcBase MyBase = null;
        //Targetting
        public Entity MyTarget = null;

        //Pathfinding
        private Pathfinder pathFinder;
        private Task pathfindingTask;
        public byte Range = 0;

        //Respawn/Despawn
        public long RespawnTime;

        public Npc(int index, NpcBase myBase, bool despawnable = false)
            : base(index)
        {
            MyName = myBase.Name;
            MySprite = myBase.Sprite;
            MyBase = myBase;
            Despawnable = despawnable;

            for (int I = 0; I < (int) Stats.StatCount; I++)
            {
                Stat[I] = new EntityStat(myBase.Stat[I], I);
            }

            for (int I = 0; I < MyBase.Spells.Count; I++)
            {
                Spells.Add(new SpellInstance(MyBase.Spells[I]));
            }

            myBase.MaxVital.CopyTo(Vital, 0);
            myBase.MaxVital.CopyTo(MaxVital, 0);
            Behaviour = myBase.Behavior;
            Range = (byte) myBase.SightRange;
            pathFinder = new Pathfinder(this);
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.GlobalEntity;
        }

        public override void Die(bool dropitems = false, Entity killer = null)
        {
            base.Die(dropitems, killer);
            MapInstance.GetMap(CurrentMap).RemoveEntity(this);
            PacketSender.SendEntityLeave(MyIndex, (int) EntityTypes.GlobalEntity, CurrentMap);
            Globals.Entities[MyIndex] = null;
        }

        //Targeting
        public void AssignTarget(Entity en)
        {
            if (en.GetType() == typeof(Projectile))
            {
                if (((Projectile) en).Owner != this) MyTarget = ((Projectile) en).Owner;
            }
            else
            {
                if (this != en) MyTarget = en;
            }
            PacketSender.SendNpcAggressionToProximity(this);
        }

        public void RemoveTarget()
        {
            MyTarget = null;
            PacketSender.SendNpcAggressionToProximity(this);
        }

        public override bool CanAttack(Entity en, SpellBase spell)
        {
            //Check if the attacker is stunned or blinded.
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == (int) StatusTypes.Stun)
                {
                    return false;
                }
            }
            if (en.GetType() == typeof(Resource))
            {
                return false;
            }
            else if (en.GetType() == typeof(Npc))
            {
                return CanNpcCombat(en) || en == this;
            }
            return true;
        }

        public override void TryAttack(Entity enemy)
        {
            if (enemy.IsDisposed) return;
            if (!CanAttack(enemy, null)) return;
            if (!IsOneBlockAway(enemy)) return;
            if (!isFacingTarget(enemy)) return;

            var deadAnimations = new List<KeyValuePair<int, int>>();
            var aliveAnimations = new List<KeyValuePair<int, int>>();

            if (MyBase.AttackAnimation > -1)
            {
                deadAnimations.Add(new KeyValuePair<int, int>(MyBase.AttackAnimation, Dir));
                aliveAnimations.Add(new KeyValuePair<int, int>(MyBase.AttackAnimation, Dir));
            }

            base.TryAttack(enemy, MyBase.Damage == 0 ? 1 : MyBase.Damage, (DamageType) MyBase.DamageType,
                (Stats) MyBase.ScalingStat,
                MyBase.Scaling, MyBase.CritChance, Options.CritMultiplier, deadAnimations, aliveAnimations);
            PacketSender.SendEntityAttack(this, (int) EntityTypes.GlobalEntity, CurrentMap, CalculateAttackTime());
        }

        private bool CanNpcCombat(Entity enemy)
        {
            //Check for NpcVsNpc Combat, both must be enabled and the attacker must have it as an enemy or attack all types of npc.
            if (enemy != null && enemy.GetType() == typeof(Npc) && MyBase != null)
            {
                if (((Npc) enemy).MyBase.NpcVsNpcEnabled == false || ((Npc) enemy).MyBase.NpcVsNpcEnabled == false)
                    return false;

                if (MyBase.AttackAllies == true) return true;

                for (int i = 0; i < MyBase.AggroList.Count; i++)
                {
                    if (NpcBase.GetNpc(MyBase.AggroList[i]) == ((Npc) enemy).MyBase)
                    {
                        return true;
                    }
                }
                return false;
            }
            else if (enemy.GetType() == typeof(Player))
            {
                return true;
            }
            return false;
        }

        private void TryCastSpells()
        {
            //check if NPC is stunned
            for (var n = 0; n < Status.Count; n++)
            {
                if (Status[n].Type == (int) StatusTypes.Stun)
                {
                    return;
                }
            }
            //Check if NPC is casting a spell
            if (CastTime > Globals.System.GetTimeMs())
            {
                return; //can't move while casting
            }
            else if (CastFreq < Globals.System.GetTimeMs()) //Try to cast a new spell
            {
                var CC = false;
                //Check if the NPC is silenced or stunned
                for (var n = 0; n < Status.Count; n++)
                {
                    if (Status[n].Type == (int) StatusTypes.Silence || Status[n].Type == (int) StatusTypes.Stun)
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
                        if (spell != null)
                        {
                            if (spell.SpellType == (int) SpellTypes.CombatSpell &&
                                spell.TargetType == (int) SpellTargetTypes.Projectile &&
                                InRangeOf(MyTarget, spell.CastRange))
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

                            if (spell.VitalCost[(int) Vitals.Mana] <= Vital[(int) Vitals.Mana])
                            {
                                if (spell.VitalCost[(int) Vitals.Health] <= Vital[(int) Vitals.Health])
                                {
                                    if (Spells[s].SpellCD < Globals.System.GetTimeMs())
                                    {
                                        Vital[(int) Vitals.Mana] = Vital[(int) Vitals.Mana] -
                                                                   spell.VitalCost[(int) Vitals.Mana];
                                        Vital[(int) Vitals.Health] = Vital[(int) Vitals.Health] -
                                                                     spell.VitalCost[(int) Vitals.Health
                                                                     ];
                                        CastTime = Globals.System.GetTimeMs() + (spell.CastDuration * 100);
                                        if (spell.Friendly == 1)
                                        {
                                            CastTarget = this;
                                        }
                                        else
                                        {
                                            CastTarget = MyTarget;
                                        }

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

                                        PacketSender.SendEntityVitals(Globals.Entities[MyIndex]);
                                        PacketSender.SendEntityVitals(Globals.Entities[MyIndex]);
                                        PacketSender.SendEntityCastTime(this, (MyBase.Spells[s]));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void Update(long timeMs)
        {
            var curMapLink = CurrentMap;
            base.Update(timeMs);
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
                            if (MyTarget.Status[n].Type == (int) StatusTypes.Stealth)
                            {
                                targetMap = -1;
                                targetX = 0;
                                targetY = 0;
                            }
                        }
                    }
                    else
                    {
                        RemoveTarget();
                    }
                }
                else //Find a target if able
                {
                    if (Behaviour == (int) NpcBehavior.AttackOnSight || MyBase.AggroList.Count > -1)
                        // Check if attack on sight or have other npc's to target
                    {
                        var maps = MapInstance.GetMap(CurrentMap).GetSurroundingMaps(true);
                        var possibleTargets = new List<Entity>();
                        int closestRange = Range + 1; //If the range is out of range we didn't find anything.
                        int closestIndex = -1;
                        foreach (var map in maps)
                        {
                            foreach (var entity in map.GetEntities())
                            {
                                if (entity.IsDead() == false && entity != this)
                                {
                                    if ((entity.GetType() == typeof(Player)) &&
                                        Behaviour == (int) NpcBehavior.AttackOnSight ||
                                        (entity.GetType() == typeof(Npc) &&
                                         MyBase.AggroList.Contains(((Npc) entity).MyBase.Id)))
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
                            AssignTarget(possibleTargets[closestIndex]);
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
                        if (targetMap != pathFinder.GetTarget().TargetMap || targetX != pathFinder.GetTarget().TargetX ||
                            targetY != pathFinder.GetTarget().TargetY)
                        {
                            pathFinder.SetTarget(null);
                        }
                    }

                    if (pathFinder.GetTarget() == null)
                    {
                        pathFinder.SetTarget(new PathfinderTarget(targetMap, targetX, targetY));
                    }

                    if (pathFinder.GetTarget() != null)
                    {
                        TryCastSpells();
                        if (!IsOneBlockAway(pathFinder.GetTarget().TargetMap, pathFinder.GetTarget().TargetX,pathFinder.GetTarget().TargetY))
                        {
                            switch (pathFinder.Update(timeMs))
                            {
                                case PathfinderResult.Success:
                                    var dir = pathFinder.GetMove();
                                    if (dir > -1)
                                    {
                                        if (CanMove(dir) == -1 || CanMove(dir) == -4)
                                        {
                                            //check if NPC is snared or stunned
                                            for (var n = 0; n < Status.Count; n++)
                                            {
                                                if (Status[n].Type == (int) StatusTypes.Stun ||
                                                    Status[n].Type == (int) StatusTypes.Snare)
                                                {
                                                    return;
                                                }
                                            }
                                            Move(dir, null);
                                        }
                                        else
                                        {
                                            pathFinder.PathFailed(timeMs);
                                        }
                                    }
                                    break;
                                case PathfinderResult.OutOfRange:
                                    MyTarget = null;
                                    targetMap = -1;
                                    break;
                                case PathfinderResult.NoPathToTarget:
                                    targetMap = -1;
                                    break;
                                case PathfinderResult.Failure:
                                    targetMap = -1;
                                    MyTarget = null;
                                    break;
                                case PathfinderResult.Wait:
                                    targetMap = -1;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
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
                                if (MyTarget.IsDisposed)
                                {
                                    MyTarget = null;
                                }
                                else
                                {
                                    if (CanAttack(MyTarget, null)) TryAttack(MyTarget);
                                }
                            }
                        }
                    }
                }

                //Move randomly
                if (targetMap != -1) return;
                if (LastRandomMove >= Globals.System.GetTimeMs()) return;
                if (MyBase.Behavior == (int) NpcBehavior.Guard)
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
                            if (Status[n].Type == (int) StatusTypes.Stun || Status[n].Type == (int) StatusTypes.Snare)
                            {
                                return;
                            }
                        }
                        Move(i, null);
                    }
                }
                LastRandomMove = Globals.System.GetTimeMs() + Globals.Rand.Next(1000, 3000);
            }
            //If we switched maps, lets update the maps
            if (curMapLink != CurrentMap)
            {
                if (curMapLink != -1)
                {
                    MapInstance.GetMap(curMapLink).RemoveEntity(this);
                }
                if (CurrentMap > -1)
                {
                    MapInstance.GetMap(CurrentMap).AddEntity(this);
                }
            }
        }
    }
}