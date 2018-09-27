using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Characters;
using Intersect.Server.EventProcessing;
using Intersect.Server.General;
using Intersect.Server.Maps;
using Intersect.Server.Misc.Pathfinding;
using Intersect.Server.Networking;

namespace Intersect.Server.Entities
{
    public class Npc : EntityInstance
    {
        //Spell casting
        public long CastFreq;

        public bool Despawnable;

        //Moving
        public long LastRandomMove;

        public NpcBase Base;

        //Targetting
        public EntityInstance MyTarget;

        //Pathfinding
        private Pathfinder mPathFinder;

        private Task mPathfindingTask;
        public byte Range;

        //Respawn/Despawn
        public long RespawnTime;

        //Damage Map - Keep track of who is doing the most damage to this npc and focus accordingly
        public Dictionary<EntityInstance, long> DamageMap = new Dictionary<EntityInstance, long>();

        public Npc(NpcBase myBase, bool despawnable = false) : base()
        {
            Name = myBase.Name;
            Sprite = myBase.Sprite;
            Level = myBase.Level;
            Base = myBase;
            Despawnable = despawnable;

            for (int i= 0; i < (int) Stats.StatCount; i++)
            {
                BaseStats[i] = myBase.Stats[i];
                Stat[i] = new EntityStat((Stats)i,this);
            }

            var spellSlot = 0;
            for (int I = 0; I < Base.Spells.Count; I++)
            {
                var slot = new SpellSlot(spellSlot);
                slot.Set(new Spell(Base.Spells[I]));
                Spells.Add(slot);
                spellSlot++;
            }

            //Give NPC Drops
            var itemSlot = 0;
            foreach (var drop in myBase.Drops)
            {
                if (Globals.Rand.Next(1, 10001) <= drop.Chance * 100 && ItemBase.Get(drop.ItemId) != null)
                {
                    var slot = new InventorySlot(itemSlot);
                    Items.Add(slot);
                    slot.Set(new Item(drop.ItemId, drop.Quantity));
                    itemSlot++;
                }
            }

            for (int i = 0; i < (int) Vitals.VitalCount; i++)
            {
                SetMaxVital(i, myBase.MaxVital[i]);
                SetVital(i, myBase.MaxVital[i]);
            }
            Range = (byte) myBase.SightRange;
            mPathFinder = new Pathfinder(this);
        }

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.GlobalEntity;
        }

        public override void Die(int dropitems = 100, EntityInstance killer = null)
        {
            base.Die(dropitems, killer);
            MapInstance.Get(MapId).RemoveEntity(this);
            PacketSender.SendEntityLeave(Id, (int) EntityTypes.GlobalEntity, MapId);
        }

        //Targeting
        public void AssignTarget(EntityInstance en)
        {
            if (en.GetType() == typeof(Projectile))
            {
                if (((Projectile) en).Owner != this) MyTarget = ((Projectile) en).Owner;
            }
            else
            {
                if (en.GetType() == typeof(Npc))
                {
                    if (((Npc)en).Base == Base)
                    {
                        if (Base.AttackAllies == false) return;
                    }
                }
                if (en.GetType() == typeof(Player))
                {
                    //TODO Make sure that the npc can target the player
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
            if (MyTarget != null)
            {
                if (DamageMap.ContainsKey(MyTarget))
                    DamageMap.Remove(MyTarget);
            }
            MyTarget = null;
            PacketSender.SendNpcAggressionToProximity(this);
        }

        public override bool CanAttack(EntityInstance en, SpellBase spell)
        {
            if (!base.CanAttack(en, spell)) return false;
            if (en.GetType() == typeof(EventPageInstance)) return false;
            //Check if the attacker is stunned or blinded.
            var statuses = Statuses.Values.ToArray();
            foreach (var status in statuses)
            {
                if (status.Type == StatusTypes.Stun)
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
                return CanNpcCombat(en, spell != null && spell.Combat.Friendly) || en == this;
            }
            else if (en.GetType() == typeof(Player))
            {
                var player = (Player)en;
                var friendly = spell != null && spell.Combat.Friendly;
                if (friendly && IsFriend(player)) return true;
                if (!friendly && !IsFriend(player)) return true;
                return false;
            }
            return true;
        }

        public override void TryAttack(EntityInstance enemy)
        {
            if (enemy.IsDisposed) return;
            if (!CanAttack(enemy, null)) return;
            if (!IsOneBlockAway(enemy)) return;
            if (!IsFacingTarget(enemy)) return;

            var deadAnimations = new List<KeyValuePair<Guid, int>>();
            var aliveAnimations = new List<KeyValuePair<Guid, int>>();

            if (Base.AttackAnimation != null)
            {
                deadAnimations.Add(new KeyValuePair<Guid, int>(Base.AttackAnimationId, Dir));
                aliveAnimations.Add(new KeyValuePair<Guid, int>(Base.AttackAnimationId, Dir));
            }

            //We were forcing at LEAST 1hp base damage.. but then you can't have guards that won't hurt the player.
            //https://www.ascensiongamedev.com/community/bug_tracker/intersect/npc-set-at-0-attack-damage-still-damages-player-by-1-initially-r915/
            if (AttackTimer < Globals.System.GetTimeMs())
            {
                base.TryAttack(enemy, Base.Damage, (DamageType) Base.DamageType,
                    (Stats) Base.ScalingStat,
                    Base.Scaling, Base.CritChance, Base.CritMultiplier, deadAnimations, aliveAnimations);
                PacketSender.SendEntityAttack(this, (int) EntityTypes.GlobalEntity, MapId, CalculateAttackTime());
            }
        }

        public bool CanNpcCombat(EntityInstance enemy, bool friendly = false)
        {
            //Check for NpcVsNpc Combat, both must be enabled and the attacker must have it as an enemy or attack all types of npc.
            if (!friendly)
            {
                if (enemy != null && enemy.GetType() == typeof(Npc) && Base != null)
                {
                    if (((Npc) enemy).Base.NpcVsNpcEnabled == false)
                        return false;

                    if (Base.AttackAllies && ((Npc) enemy).Base == Base) return true;

                    for (int i = 0; i < Base.AggroList.Count; i++)
                    {
                        if (NpcBase.Get(Base.AggroList[i]) == ((Npc) enemy).Base)
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
                if (enemy != null && enemy.GetType() == typeof(Npc) && Base != null && ((Npc)enemy).Base == Base && Base.AttackAllies == false)
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
                if (status.Type == StatusTypes.Stun)
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
                    if (status.Type == StatusTypes.Silence || status.Type == StatusTypes.Stun)
                    {
                        cc = true;
                        break;
                    }
                }

                if (cc == false)
                {
                    if (Base.Spells.Count > 0)
                    {
                        var s = Globals.Rand.Next(0, Base.Spells.Count); //Pick a random spell
                        var spell = SpellBase.Get((Base.Spells[s]));
                        var range = spell.Combat.CastRange;
                        if (spell != null)
                        {
                            var projectileBase = spell.Combat.Projectile;
                            if (spell.SpellType == SpellTypes.CombatSpell && spell.Combat.TargetType == SpellTargetTypes.Projectile && projectileBase != null && InRangeOf(MyTarget, projectileBase.Range))
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

                            if (spell.VitalCost[(int) Vitals.Mana] <= GetVital(Vitals.Mana))
                            {
                                if (spell.VitalCost[(int) Vitals.Health] <= GetVital(Vitals.Health))
                                {
                                    if (Spells[s].SpellCd < Globals.System.GetTimeMs())
                                    {
                                        if (spell.Combat.TargetType == SpellTargetTypes.Self || spell.Combat.TargetType == SpellTargetTypes.AoE || InRangeOf(MyTarget, range))
                                        {
                                            CastTime = Globals.System.GetTimeMs() + spell.CastDuration;
                                            SubVital(Vitals.Mana, spell.VitalCost[(int) Vitals.Mana]);
                                            SubVital(Vitals.Health,spell.VitalCost[(int)Vitals.Health]);
                                            if (spell.Combat.Friendly && spell.SpellType != SpellTypes.WarpTo)
                                            {
                                                CastTarget = this;
                                            }
                                            else
                                            {
                                                CastTarget = MyTarget;
                                            }

                                            switch (Base.SpellFrequency)
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

                                            if (spell.CastAnimationId != Guid.Empty)
                                            {
                                                PacketSender.SendAnimationToProximity(spell.CastAnimationId, 1,
                                                    Id, MapId, 0, 0, Dir);
                                                //Target Type 1 will be global entity
                                            }

                                            PacketSender.SendEntityVitals(this);
                                            PacketSender.SendEntityCastTime(this, (Base.Spells[s]));
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
            var curMapLink = MapId;
            base.Update(timeMs);
            //TODO Clear Damage Map if out of combat (target is null and combat timer is to the point that regen has started)
            if (Target == null && Globals.System.GetTimeMs() > CombatTimer && Globals.System.GetTimeMs() > RegenTimer)
            {
                DamageMap.Clear();
            }

            var fleeing = false;
            if (Base.FleeHealthPercentage > 0)
            {
                var fleeHpCutoff = GetMaxVital(Vitals.Health) * ((float)Base.FleeHealthPercentage / 100f);
                if (GetVital(Vitals.Health) < fleeHpCutoff)
                {
                    fleeing = true;
                }
            }

            if (MoveTimer < Globals.System.GetTimeMs())
            {
                var targetMap = Guid.Empty;
                var targetX = 0;
                var targetY = 0;
                //Check if there is a target, if so, run their ass down.
                if (MyTarget != null)
                {
                    if (!MyTarget.IsDead() && CanAttack(MyTarget,null))
                    {
                        targetMap = MyTarget.MapId;
                        targetX = MyTarget.X;
                        targetY = MyTarget.Y;
                        var targetStatuses = MyTarget.Statuses.Values.ToArray();
                        foreach (var targetStatus in targetStatuses)
                        {
                            if (targetStatus.Type == StatusTypes.Stealth)
                            {
                                targetMap = Guid.Empty;
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
                    long dmg = 0;
                    EntityInstance tgt = null;
                    foreach (var pair in DamageMap)
                    {
                        if (pair.Value > dmg)
                        {
                            dmg = pair.Value;
                            tgt = pair.Key;
                        }
                    }

                    if (tgt != null)
                    {
                        AssignTarget(tgt);
                    }
                    else
                    {
                        // Check if attack on sight or have other npc's to target
                        TryFindNewTarget();
                    }
                }

                if (targetMap != Guid.Empty)
                {
                    //Check if target map is on one of the surrounding maps, if not then we are not even going to look.
                    if (targetMap != MapId)
                    {
                        if (MapInstance.Get(MapId).SurroundingMaps.Count > 0)
                        {
                            for (var x = 0;
                                x < MapInstance.Get(MapId).SurroundingMaps.Count;
                                x++)
                            {
                                if (MapInstance.Get(MapId).SurroundingMaps[x] == targetMap)
                                {
                                    break;
                                }
                                if (x == MapInstance.Get(MapId).SurroundingMaps.Count - 1)
                                {
                                    targetMap = Guid.Empty;
                                }
                            }
                        }
                        else
                        {
                            targetMap = Guid.Empty;
                        }
                    }
                }

                if (targetMap != Guid.Empty)
                {
                    if (mPathFinder.GetTarget() != null)
                    {
                        if (targetMap != mPathFinder.GetTarget().TargetMapId ||
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
                        if (!IsOneBlockAway(mPathFinder.GetTarget().TargetMapId, mPathFinder.GetTarget().TargetX,
                            mPathFinder.GetTarget().TargetY))
                        {
                            switch (mPathFinder.Update(timeMs))
                            {
                                case PathfinderResult.Success:
                                    var dir = mPathFinder.GetMove();
                                    if (dir > -1)
                                    {
                                        if (fleeing)
                                        {
                                            switch (dir)
                                            {
                                                case 0:
                                                    dir = 1;
                                                    break;
                                                case 1:
                                                    dir = 0;
                                                    break;
                                                case 2:
                                                    dir = 3;
                                                    break;
                                                case 3:
                                                    dir = 2;
                                                    break;
                                            }
                                        }
                                        if (CanMove(dir) == -1 || CanMove(dir) == -4)
                                        {
                                            //check if NPC is snared or stunned
                                            var statuses = Statuses.Values.ToArray();
                                            foreach (var status in statuses)
                                            {
                                                if (status.Type == StatusTypes.Stun || status.Type == StatusTypes.Snare)
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
                                    targetMap = Guid.Empty;
                                    break;
                                case PathfinderResult.NoPathToTarget:
                                    TryFindNewTarget((MyTarget != null ? MyTarget.Id : Guid.Empty));
                                    targetMap = Guid.Empty;
                                    break;
                                case PathfinderResult.Failure:
                                    targetMap = Guid.Empty;
                                    RemoveTarget();
                                    break;
                                case PathfinderResult.Wait:
                                    targetMap = Guid.Empty;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else
                        {
                            var fleed = false;
                            if (fleeing)
                            {
                                var dir = DirToEnemy(MyTarget);
                                switch (dir)
                                {
                                    case 0:
                                        dir = 1;
                                        break;
                                    case 1:
                                        dir = 0;
                                        break;
                                    case 2:
                                        dir = 3;
                                        break;
                                    case 3:
                                        dir = 2;
                                        break;
                                }
                                if (CanMove(dir) == -1 || CanMove(dir) == -4)
                                {
                                    //check if NPC is snared or stunned
                                    var statuses = Statuses.Values.ToArray();
                                    foreach (var status in statuses)
                                    {
                                        if (status.Type == StatusTypes.Stun || status.Type == StatusTypes.Snare)
                                        {
                                            return;
                                        }
                                    }
                                    Move(dir, null);
                                    fleed = true;
                                }
                            }
                            if (!fleed)
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
                }

                //Move randomly
                if (targetMap != Guid.Empty) return;
                if (LastRandomMove >= Globals.System.GetTimeMs()) return;
                if (Base.Movement == (int)NpcMovement.StandStill)
                {
                    LastRandomMove = Globals.System.GetTimeMs() + Globals.Rand.Next(1000, 3000);
                    return;
                }
                else if (Base.Movement == (int)NpcMovement.TurnRandomly)
                {
                    ChangeDir(Globals.Rand.Next(0, 4));
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
                            if (status.Type == StatusTypes.Stun || status.Type == StatusTypes.Snare)
                            {
                                return;
                            }
                        }
                        Move(i, null);
                    }
                }
                LastRandomMove = Globals.System.GetTimeMs() + Globals.Rand.Next(1000, 3000);

                if (fleeing) LastRandomMove = Globals.System.GetTimeMs() + (long)GetMovementTime();
            }
            //If we switched maps, lets update the maps
            if (curMapLink != MapId)
            {
                if (curMapLink == Guid.Empty)
                {
                    MapInstance.Get(curMapLink).RemoveEntity(this);
                }
                if (MapId != Guid.Empty)
                {
                    MapInstance.Get(MapId).AddEntity(this);
                }
            }
        }

        public override void NotifySwarm(EntityInstance attacker)
        {
            var mapEntities = MapInstance.Get(MapId).GetEntities(true);
            foreach (var en in mapEntities)
            {
                if (en.GetType() == typeof(Npc))
                {
                    var npc = (Npc)en;
                    if (npc.MyTarget == null & npc.Base.Swarm && npc.Base == Base)
                    {
                        if (npc.InRangeOf(attacker, npc.Base.SightRange))
                        {
                            npc.AssignTarget(attacker);
                        }
                    }
                }
            }
        }

        public bool CanPlayerAttack(Player en)
        {
            //Check to see if the npc is a friend/protector...
            if (IsFriend(en)) return false;

            //If not then check and see if player meets the conditions to attack the npc...
            if (Base.PlayerCanAttackConditions.Lists.Count == 0 || Conditions.MeetsConditionLists(Base.PlayerCanAttackConditions, en, null)) return true;

            return false;
        }

        public bool IsFriend(EntityInstance entity)
        {
            if (entity.GetType() == typeof(Npc))
            {
                if (((Npc)entity).Base == Base) return true;
            }
            else if (entity.GetType() == typeof(Player))
            {
                var player = (Player)entity;
                if (Base.PlayerFriendConditions.Lists.Count == 0) return false;
                if (Conditions.MeetsConditionLists(Base.PlayerFriendConditions,player,null))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ShouldAttackPlayerOnSight(Player en)
        {
            if (Base.Aggressive)
            {
                if (Base.AttackOnSightConditions.Lists.Count > 0 && Conditions.MeetsConditionLists(Base.AttackOnSightConditions, en, null)) return false;
                return true;
            }
            else
            {
                if (Base.AttackOnSightConditions.Lists.Count > 0 && Conditions.MeetsConditionLists(Base.AttackOnSightConditions, en, null)) return true;
            }
            return false;
        }

        private void TryFindNewTarget(Guid avoidId = new Guid())
        {
            var maps = MapInstance.Get(MapId).GetSurroundingMaps(true);
            var possibleTargets = new List<EntityInstance>();
            int closestRange = Range + 1; //If the range is out of range we didn't find anything.
            int closestIndex = -1;
            foreach (var map in maps)
            {
                foreach (var entity in map.GetEntities())
                {
                    if (entity != null && entity.IsDead() == false && entity != this && entity.Id != avoidId)
                    {
                        //TODO Check if NPC is allowed to attack player with new conditions
                        if ((entity.GetType() == typeof(Player))) {
                            if (ShouldAttackPlayerOnSight((Player)entity))
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
                        else if (entity.GetType() == typeof(Npc)) {
                            if (Base.Aggressive && Base.AggroList.Contains(((Npc)entity).Base.Id)) {
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
            }
            if (closestIndex != -1)
            {
                AssignTarget(possibleTargets[closestIndex]);
            }
        }

        public override void ProcessRegen()
        {
            if (Base == null) return;

            foreach (Vitals vital in Enum.GetValues(typeof(Vitals)))
            {
                if (vital >= Vitals.VitalCount) continue;

                var vitalId = (int)vital;
                var vitalValue = GetVital(vital);
                var maxVitalValue = GetMaxVital(vital);
                if (vitalValue >= maxVitalValue) continue;

                var vitalRegenRate = Base.VitalRegen[vitalId] / 100f;
                var regenValue = (int)Math.Max(1, maxVitalValue * vitalRegenRate) * Math.Abs(Math.Sign(vitalRegenRate));
                AddVital(vital, regenValue);
            }
        }

        public override void Warp(Guid newMapId, int newX, int newY, int newDir, bool adminWarp = false, int zOverride = 0, bool mapSave = false)
        {
            var map = MapInstance.Get(newMapId);
            if (map == null)
            {
                return;
            }
            X = newX;
            Y = newY;
            Z = zOverride;
            Dir = newDir;
            if (newMapId != MapId )
            {
                var oldMap = MapInstance.Get(MapId);
                if (oldMap != null)
                {
                    oldMap.RemoveEntity(this);
                }
                PacketSender.SendEntityLeave(Id, (int)EntityTypes.GlobalEntity, MapId);
                MapId = newMapId;
                PacketSender.SendEntityDataToProximity(this);
                PacketSender.SendEntityPositionToAll(this);
            }
            else
            {
                PacketSender.SendEntityPositionToAll(this);
                PacketSender.SendEntityVitals(this);
                PacketSender.SendEntityStats(this);
            }
        }
    }
}