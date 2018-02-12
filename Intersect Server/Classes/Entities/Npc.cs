using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Classes.General;
using Intersect.Server.Classes.Maps;
using Intersect.Server.Classes.Misc.Pathfinding;
using Intersect.Server.Classes.Networking;
using Intersect.Server.Classes.Spells;
using Intersect.Server.Classes.Items;

namespace Intersect.Server.Classes.Entities
{
    public class Npc : Entity
    {
        //Behaviour
        public byte Behaviour;

        //Spell casting
        public long CastFreq;

        public bool Despawnable;

        //Moving
        public long LastRandomMove;

        public NpcBase MyBase;

        //Targetting
        public Entity MyTarget;

        //Pathfinding
        private Pathfinder mPathFinder;

        private Task mPathfindingTask;
        public byte Range;

        //Respawn/Despawn
        public long RespawnTime;

        public Npc(int index, NpcBase myBase, bool despawnable = false)
            : base(index)
        {
            MyName = myBase.Name;
            MySprite = myBase.Sprite;
            Level = myBase.Level;
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

            //Give NPC Drops
            for (var n = 0; n < Options.MaxNpcDrops; n++)
            {
                if (Globals.Rand.Next(1, 101) <= myBase.Drops[n].Chance)
                {
                    Inventory.Add(new ItemInstance(myBase.Drops[n].ItemNum,
                        myBase.Drops[n].Amount, -1));
                }
            }

            myBase.MaxVital.CopyTo(Vital, 0);
            myBase.MaxVital.CopyTo(MaxVital, 0);
            Behaviour = myBase.Behavior;
            Range = (byte) myBase.SightRange;
            mPathFinder = new Pathfinder(this);
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.GlobalEntity;
        }

        public override void Die(int dropitems = 100, Entity killer = null)
        {
            base.Die(dropitems, killer);
            MapInstance.Lookup.Get<MapInstance>(CurrentMap).RemoveEntity(this);
            PacketSender.SendEntityLeave(MyIndex, (int) EntityTypes.GlobalEntity, CurrentMap);
            Globals.Entities[MyIndex] = null;
        }

        //Targeting
        public void AssignTarget(Entity en)
        {
            if (MyBase.Behavior == (int) NpcBehavior.Friendly) return;
            if (en.GetType() == typeof(Projectile))
            {
                if (((Projectile) en).Owner != this) MyTarget = ((Projectile) en).Owner;
            }
            else
            {
                if (en.GetType() == typeof(Npc))
                {
                    if (((Npc)en).MyBase == MyBase)
                    {
                        if (MyBase.AttackAllies == false) return;
                    }
                }
                if (en.GetType() == typeof(Player))
                {
                    if (this != en) MyTarget = en;
                }
                else
                {
                    if (this != en) MyTarget = en;
                }
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
            if (en.GetType() == typeof(Npc) && ((Npc) en).MyBase.Behavior == (int) NpcBehavior.Friendly) return false;
            if (en.GetType() == typeof(EventPageInstance)) return false;
            //Check if the attacker is stunned or blinded.
            var statuses = Statuses.Values.ToArray();
            foreach (var status in statuses)
            {
                if (status.Type == (int) StatusTypes.Stun)
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
                return CanNpcCombat(en, spell != null && spell.Friendly == 1) || en == this;
            }
            return true;
        }

        public override void TryAttack(Entity enemy)
        {
            if (enemy.IsDisposed) return;
            if (!CanAttack(enemy, null)) return;
            if (!IsOneBlockAway(enemy)) return;
            if (!IsFacingTarget(enemy)) return;

            var deadAnimations = new List<KeyValuePair<int, int>>();
            var aliveAnimations = new List<KeyValuePair<int, int>>();

            if (MyBase.AttackAnimation > -1)
            {
                deadAnimations.Add(new KeyValuePair<int, int>(MyBase.AttackAnimation, Dir));
                aliveAnimations.Add(new KeyValuePair<int, int>(MyBase.AttackAnimation, Dir));
            }

            //We were forcing at LEAST 1hp base damage.. but then you can't have guards that won't hurt the player.
            //https://www.ascensiongamedev.com/community/bug_tracker/intersect/npc-set-at-0-attack-damage-still-damages-player-by-1-initially-r915/
            base.TryAttack(enemy, MyBase.Damage, (DamageType) MyBase.DamageType,
                (Stats) MyBase.ScalingStat,
                MyBase.Scaling, MyBase.CritChance, Options.CritMultiplier, deadAnimations, aliveAnimations);
            PacketSender.SendEntityAttack(this, (int) EntityTypes.GlobalEntity, CurrentMap, CalculateAttackTime());
        }

        public bool CanNpcCombat(Entity enemy, bool friendly = false)
        {
            //Check for NpcVsNpc Combat, both must be enabled and the attacker must have it as an enemy or attack all types of npc.
            if (!friendly)
            {
                if (enemy != null && enemy.GetType() == typeof(Npc) && MyBase != null)
                {
                    if (((Npc) enemy).MyBase.NpcVsNpcEnabled == false)
                        return false;

                    if (MyBase.AttackAllies && ((Npc) enemy).MyBase == MyBase) return true;

                    for (int i = 0; i < MyBase.AggroList.Count; i++)
                    {
                        if (NpcBase.Lookup.Get<NpcBase>(MyBase.AggroList[i]) == ((Npc) enemy).MyBase)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else if (enemy != null && enemy.GetType() == typeof(Player))
                {
                    return true;
                }
            }
            else
            {
                if (enemy != null && enemy.GetType() == typeof(Npc) && MyBase != null && ((Npc)enemy).MyBase == MyBase && MyBase.AttackAllies == false)
                {
                    return true;
                }
                else if (enemy != null && enemy.GetType() == typeof(Player))
                {
                    return false;
                }
            }
            return false;
        }

        private void TryCastSpells()
        {
            //check if NPC is stunned
            var statuses = Statuses.Values.ToArray();
            foreach (var status in statuses)
            {
                if (status.Type == (int) StatusTypes.Stun)
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
                var cc = false;
                //Check if the NPC is silenced or stunned
                foreach (var status in statuses)
                {
                    if (status.Type == (int) StatusTypes.Silence || status.Type == (int) StatusTypes.Stun)
                    {
                        cc = true;
                        break;
                    }
                }

                if (cc == false)
                {
                    if (MyBase.Spells.Count > 0)
                    {
                        var s = Globals.Rand.Next(0, MyBase.Spells.Count); //Pick a random spell
                        var spell = SpellBase.Lookup.Get<SpellBase>((MyBase.Spells[s]));
                        var range = spell.CastRange;
                        if (spell != null)
                        {
                            var projectileBase = ProjectileBase.Lookup.Get<ProjectileBase>(spell.Projectile);
                            if (spell.SpellType == (int) SpellTypes.CombatSpell &&
                                spell.TargetType == (int) SpellTargetTypes.Projectile && projectileBase != null &&
                                InRangeOf(MyTarget, projectileBase.Range))
                            {
                                range = projectileBase.Range;
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
                                    if (Spells[s].SpellCd < Globals.System.GetTimeMs())
                                    {
                                        if (spell.TargetType == (int)SpellTargetTypes.Self || spell.TargetType == (int)SpellTargetTypes.AoE || InRangeOf(MyTarget, range))
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
        }

        //General Updating
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
                        var targetStatuses = MyTarget.Statuses.Values.ToArray();
                        foreach (var targetStatus in targetStatuses)
                        {
                            if (targetStatus.Type == (int) StatusTypes.Stealth)
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
                        TryFindNewTarget();
                    }
                }

                if (targetMap > -1)
                {
                    //Check if target map is on one of the surrounding maps, if not then we are not even going to look.
                    if (targetMap != CurrentMap)
                    {
                        if (MapInstance.Lookup.Get<MapInstance>(CurrentMap).SurroundingMaps.Count > 0)
                        {
                            for (var x = 0;
                                x < MapInstance.Lookup.Get<MapInstance>(CurrentMap).SurroundingMaps.Count;
                                x++)
                            {
                                if (MapInstance.Lookup.Get<MapInstance>(CurrentMap).SurroundingMaps[x] == targetMap)
                                {
                                    break;
                                }
                                if (x == MapInstance.Lookup.Get<MapInstance>(CurrentMap).SurroundingMaps.Count - 1)
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
                    if (mPathFinder.GetTarget() != null)
                    {
                        if (targetMap != mPathFinder.GetTarget().TargetMap ||
                            targetX != mPathFinder.GetTarget().TargetX ||
                            targetY != mPathFinder.GetTarget().TargetY)
                        {
                            mPathFinder.SetTarget(null);
                        }
                    }

                    if (mPathFinder.GetTarget() == null)
                    {
                        mPathFinder.SetTarget(new PathfinderTarget(targetMap, targetX, targetY));
                    }

                    if (mPathFinder.GetTarget() != null)
                    {
                        TryCastSpells();
                        if (!IsOneBlockAway(mPathFinder.GetTarget().TargetMap, mPathFinder.GetTarget().TargetX,
                            mPathFinder.GetTarget().TargetY))
                        {
                            switch (mPathFinder.Update(timeMs))
                            {
                                case PathfinderResult.Success:
                                    var dir = mPathFinder.GetMove();
                                    if (dir > -1)
                                    {
                                        if (CanMove(dir) == -1 || CanMove(dir) == -4)
                                        {
                                            //check if NPC is snared or stunned
                                            var statuses = Statuses.Values.ToArray();
                                            foreach (var status in statuses)
                                            {
                                                if (status.Type == (int) StatusTypes.Stun ||
                                                    status.Type == (int) StatusTypes.Snare)
                                                {
                                                    return;
                                                }
                                            }
                                            Move(dir, null);
                                        }
                                        else
                                        {
                                            mPathFinder.PathFailed(timeMs);
                                        }
                                    }
                                    break;
                                case PathfinderResult.OutOfRange:
                                    RemoveTarget();
                                    targetMap = -1;
                                    break;
                                case PathfinderResult.NoPathToTarget:
                                    TryFindNewTarget((MyTarget != null ? MyTarget.MyIndex : -1));
                                    targetMap = -1;
                                    break;
                                case PathfinderResult.Failure:
                                    targetMap = -1;
                                    RemoveTarget();
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
                        var statuses = Statuses.Values.ToArray();
                        foreach (var status in statuses)
                        {
                            if (status.Type == (int) StatusTypes.Stun || status.Type == (int) StatusTypes.Snare)
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
                    MapInstance.Lookup.Get<MapInstance>(curMapLink).RemoveEntity(this);
                }
                if (CurrentMap > -1)
                {
                    MapInstance.Lookup.Get<MapInstance>(CurrentMap).AddEntity(this);
                }
            }
        }

        private void TryFindNewTarget(int avoidIndex = -1)
        {
            var maps = MapInstance.Lookup.Get<MapInstance>(CurrentMap).GetSurroundingMaps(true);
            var possibleTargets = new List<Entity>();
            int closestRange = Range + 1; //If the range is out of range we didn't find anything.
            int closestIndex = -1;
            foreach (var map in maps)
            {
                foreach (var entity in map.GetEntities())
                {
                    if (entity != null && entity.IsDead() == false && entity != this && entity.MyIndex != avoidIndex)
                    {
                        if ((entity.GetType() == typeof(Player)) &&
                            Behaviour == (int)NpcBehavior.AttackOnSight ||
                            (entity.GetType() == typeof(Npc) &&
                             MyBase.AggroList.Contains(((Npc)entity).MyBase.Index)))
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

        public override void ProcessRegen()
        {
            //For now give npcs/resources 10% health back every regen tick... in the future we should put per-npc and per-resource regen settings into their respective editors.
            foreach (Vitals vital in Enum.GetValues(typeof(Vitals)))
            {
                Debug.Assert(Vital != null, "Vital != null");
                Debug.Assert(MaxVital != null, "MaxVital != null");

                if (vital >= Vitals.VitalCount) continue;

                var vitalId = (int)vital;
                var vitalValue = Vital[vitalId];
                var maxVitalValue = MaxVital[vitalId];
                if (vitalValue >= maxVitalValue) continue;

                var regenValue = (int)Math.Max(1, maxVitalValue * .1f);
                AddVital(vital, regenValue);
            }
        }
    }
}