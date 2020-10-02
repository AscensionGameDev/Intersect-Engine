using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Intersect.Enums;
using Intersect.GameObjects;
using Intersect.Logging;
using Intersect.Network.Packets.Server;
using Intersect.Server.Database;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities.Combat;
using Intersect.Server.Entities.Events;
using Intersect.Server.Entities.Pathfinding;
using Intersect.Server.General;
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;

using JetBrains.Annotations;

namespace Intersect.Server.Entities
{

    public class Npc : Entity
    {

        //Spell casting
        public long CastFreq;

        /// <summary>
        /// Damage Map - Keep track of who is doing the most damage to this npc and focus accordingly
        /// </summary>
        public ConcurrentDictionary<Entity, long> DamageMap = new ConcurrentDictionary<Entity, long>();

        /// <summary>
        /// Returns the entity that ranks the highest on this NPC's damage map.
        /// </summary>
        public Entity DamageMapHighest { 
            get {
                long damage = 0;
                Entity top = null;
                foreach (var pair in DamageMap)
                {
                    if (pair.Value > damage)
                    {
                        top = pair.Key;
                        damage = pair.Value;
                    }
                }
                return top;
            } 
        }

        public bool Despawnable;

        //Moving
        public long LastRandomMove;

        //Pathfinding
        private Pathfinder mPathFinder;

        private Task mPathfindingTask;

        public byte Range;

        //Respawn/Despawn
        public long RespawnTime;

        public long FindTargetWaitTime;
        public int FindTargetDelay = 500;


        /// <summary>
        /// The map on which this NPC was "aggro'd" and started chasing a target.
        /// </summary>
        public MapInstance AggroCenterMap;

        /// <summary>
        /// The X value on which this NPC was "aggro'd" and started chasing a target.
        /// </summary>
        public int AggroCenterX;

        /// <summary>
        /// The Y value on which this NPC was "aggro'd" and started chasing a target.
        /// </summary>
        public int AggroCenterY;

        /// <summary>
        /// The Z value on which this NPC was "aggro'd" and started chasing a target.
        /// </summary>
        public int AggroCenterZ;

        public Npc([NotNull] NpcBase myBase, bool despawnable = false) : base()
        {
            Name = myBase.Name;
            Sprite = myBase.Sprite;
            Level = myBase.Level;
            Base = myBase;
            Despawnable = despawnable;

            for (var i = 0; i < (int) Stats.StatCount; i++)
            {
                BaseStats[i] = myBase.Stats[i];
                Stat[i] = new Stat((Stats) i, this);
            }

            var spellSlot = 0;
            for (var I = 0; I < Base.Spells.Count; I++)
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
                //if (Globals.Rand.Next(1, 10001) <= drop.Chance * 100 && ItemBase.Get(drop.ItemId) != null)
                //{
                var slot = new InventorySlot(itemSlot);
                slot.Set(new Item(drop.ItemId, drop.Quantity, true));
                slot.DropChance = drop.Chance;
                Items.Add(slot);
                itemSlot++;

                //}
            }

            for (var i = 0; i < (int) Vitals.VitalCount; i++)
            {
                SetMaxVital(i, myBase.MaxVital[i]);
                SetVital(i, myBase.MaxVital[i]);
            }

            Range = (byte) myBase.SightRange;
            mPathFinder = new Pathfinder(this);
        }

        [NotNull]
        public NpcBase Base { get; private set; }

        private bool IsStunnedOrSleeping => Statuses.Values.Any(PredicateStunnedOrSleeping);

        private bool IsUnableToCastSpells => Statuses.Values.Any(PredicateUnableToCastSpells);

        public override EntityTypes GetEntityType()
        {
            return EntityTypes.GlobalEntity;
        }

        public override void Die(int dropitems = 100, Entity killer = null)
        {
            base.Die(dropitems, killer);

            AggroCenterMap = null;
            AggroCenterX = 0;
            AggroCenterY = 0;
            AggroCenterZ = 0;

            MapInstance.Get(MapId).RemoveEntity(this);
            PacketSender.SendEntityDie(this);
            PacketSender.SendEntityLeave(this);
        }

        //Targeting
        public void AssignTarget(Entity en)
        {
            //Can't assign a new target if taunted, unless we're resetting their target somehow.
            var pathTarget = mPathFinder.GetTarget();
            if (en != null || (pathTarget != null && (pathTarget.TargetMapId != AggroCenterMap.Id ||  pathTarget.TargetX != AggroCenterX || pathTarget.TargetY != AggroCenterY)))
            {
                var statuses = Statuses.Values.ToArray();
                foreach (var status in statuses)
                {
                    if (status.Type == StatusTypes.Taunt)
                    {
                        return;
                    }
                }
            }
            
            if (en.GetType() == typeof(Projectile))
            {
                if (((Projectile) en).Owner != this)
                {
                    Target = ((Projectile) en).Owner;
                }
            }
            else
            {
                if (en.GetType() == typeof(Npc))
                {
                    if (((Npc) en).Base == Base)
                    {
                        if (Base.AttackAllies == false)
                        {
                            return;
                        }
                    }
                }

                if (en.GetType() == typeof(Player))
                {
                    //TODO Make sure that the npc can target the player
                    if (this != en)
                    {
                        Target = en;
                    }
                }
                else
                {
                    if (this != en)
                    {
                        Target = en;
                    }
                }
            }

            // Are we configured to handle resetting NPCs after they chase a target for a specified amount of tiles?
            if (Options.Npc.AllowResetRadius)
            {
                // Are we configured to allow new reset locations before they move to their original location, or do we simply not have an original location yet?
                if (Options.Npc.AllowNewResetLocationBeforeFinish || AggroCenterMap == null)
                {
                    AggroCenterMap = Map;
                    AggroCenterX = X;
                    AggroCenterY = Y;
                    AggroCenterZ = Z;
                }
            }

            PacketSender.SendNpcAggressionToProximity(this);
        }

        public void RemoveTarget()
        {
            if (Target != null)
            {
                if (DamageMap.ContainsKey(Target))
                {
                    DamageMap.TryRemove(Target, out var val);
                }
            }

            Target = null;
 
            PacketSender.SendNpcAggressionToProximity(this);
        }

        public override int CalculateAttackTime()
        {
            if (Base.AttackSpeedModifier == 1) //Static
            {
                return Base.AttackSpeedValue;
            }

            return base.CalculateAttackTime();
        }

        public override bool CanAttack(Entity entity, SpellBase spell)
        {
            if (!base.CanAttack(entity, spell))
            {
                return false;
            }

            if (entity.GetType() == typeof(EventPageInstance))
            {
                return false;
            }

            //Check if the attacker is stunned or blinded.
            var statuses = Statuses.Values.ToArray();
            foreach (var status in statuses)
            {
                if (status.Type == StatusTypes.Stun || status.Type == StatusTypes.Sleep)
                {
                    return false;
                }
            }

            if (entity.GetType() == typeof(Resource))
            {
                if (!entity.Passable)
                {
                    return false;
                }
            }
            else if (entity.GetType() == typeof(Npc))
            {
                return CanNpcCombat(entity, spell != null && spell.Combat.Friendly) || entity == this;
            }
            else if (entity.GetType() == typeof(Player))
            {
                var player = (Player) entity;
                var friendly = spell != null && spell.Combat.Friendly;
                if (friendly && IsAllyOf(player))
                {
                    return true;
                }

                if (!friendly && !IsAllyOf(player))
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        public override void TryAttack(Entity target)
        {
            if (target.IsDisposed)
            {
                return;
            }

            if (!CanAttack(target, null))
            {
                return;
            }

            if (!IsOneBlockAway(target))
            {
                return;
            }

            if (!IsFacingTarget(target))
            {
                return;
            }

            var deadAnimations = new List<KeyValuePair<Guid, sbyte>>();
            var aliveAnimations = new List<KeyValuePair<Guid, sbyte>>();

            //We were forcing at LEAST 1hp base damage.. but then you can't have guards that won't hurt the player.
            //https://www.ascensiongamedev.com/community/bug_tracker/intersect/npc-set-at-0-attack-damage-still-damages-player-by-1-initially-r915/
            if (AttackTimer < Globals.Timing.Milliseconds)
            {
                if (Base.AttackAnimation != null)
                {
                    PacketSender.SendAnimationToProximity(
                        Base.AttackAnimationId, -1, Guid.Empty, target.MapId, (byte) target.X, (byte) target.Y,
                        (sbyte) Dir
                    );
                }

                base.TryAttack(
                    target, Base.Damage, (DamageType) Base.DamageType, (Stats) Base.ScalingStat, Base.Scaling,
                    Base.CritChance, Base.CritMultiplier, deadAnimations, aliveAnimations
                );

                PacketSender.SendEntityAttack(this, CalculateAttackTime());
            }
        }

        public bool CanNpcCombat(Entity enemy, bool friendly = false)
        {
            //Check for NpcVsNpc Combat, both must be enabled and the attacker must have it as an enemy or attack all types of npc.
            if (!friendly)
            {
                if (enemy != null && enemy.GetType() == typeof(Npc) && Base != null)
                {
                    if (((Npc) enemy).Base.NpcVsNpcEnabled == false)
                    {
                        return false;
                    }

                    if (Base.AttackAllies && ((Npc) enemy).Base == Base)
                    {
                        return true;
                    }

                    for (var i = 0; i < Base.AggroList.Count; i++)
                    {
                        if (NpcBase.Get(Base.AggroList[i]) == ((Npc) enemy).Base)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                if (enemy != null && enemy.GetType() == typeof(Player))
                {
                    return true;
                }
            }
            else
            {
                if (enemy != null &&
                    enemy.GetType() == typeof(Npc) &&
                    Base != null &&
                    ((Npc) enemy).Base == Base &&
                    Base.AttackAllies == false)
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

        private static bool PredicateStunnedOrSleeping(Status status)
        {
            switch (status?.Type)
            {
                case StatusTypes.Sleep:
                case StatusTypes.Stun:
                    return true;

                case StatusTypes.Silence:
                case StatusTypes.None:
                case StatusTypes.Snare:
                case StatusTypes.Blind:
                case StatusTypes.Stealth:
                case StatusTypes.Transform:
                case StatusTypes.Cleanse:
                case StatusTypes.Invulnerable:
                case StatusTypes.Shield:
                case StatusTypes.OnHit:
                case StatusTypes.Taunt:
                case null:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static bool PredicateUnableToCastSpells(Status status)
        {
            switch (status?.Type)
            {
                case StatusTypes.Silence:
                case StatusTypes.Sleep:
                case StatusTypes.Stun:
                    return true;

                case StatusTypes.None:
                case StatusTypes.Snare:
                case StatusTypes.Blind:
                case StatusTypes.Stealth:
                case StatusTypes.Transform:
                case StatusTypes.Cleanse:
                case StatusTypes.Invulnerable:
                case StatusTypes.Shield:
                case StatusTypes.OnHit:
                case StatusTypes.Taunt:
                case null:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void TryCastSpells()
        {
            // Check if NPC is stunned/sleeping
            if (IsStunnedOrSleeping)
            {
                return;
            }

            //Check if NPC is casting a spell
            if (CastTime > Globals.Timing.Milliseconds)
            {
                return; //can't move while casting
            }

            if (CastFreq >= Globals.Timing.Milliseconds)
            {
                return;
            }

            // Check if the NPC is able to cast spells
            if (IsUnableToCastSpells)
            {
                return;
            }

            if (Base.Spells == null || Base.Spells.Count <= 0)
            {
                return;
            }

            // Pick a random spell
            var spellIndex = Randomization.Next(0, Spells.Count);
            var spellId = Base.Spells[spellIndex];
            var spellBase = SpellBase.Get(spellId);
            if (spellBase == null)
            {
                return;
            }

            if (spellBase.Combat == null)
            {
                Log.Warn($"Combat data missing for {spellBase.Id}.");
            }

            var range = spellBase.Combat?.CastRange ?? 0;
            var targetType = spellBase.Combat?.TargetType ?? SpellTargetTypes.Single;
            var projectileBase = spellBase.Combat?.Projectile;

            if (spellBase.SpellType == SpellTypes.CombatSpell &&
                targetType == SpellTargetTypes.Projectile &&
                projectileBase != null &&
                InRangeOf(Target, projectileBase.Range))
            {
                range = projectileBase.Range;
                var dirToEnemy = DirToEnemy(Target);
                if (dirToEnemy != Dir)
                {
                    if (LastRandomMove >= Globals.Timing.Milliseconds)
                    {
                        return;
                    }

                    //Face the target -- next frame fire -- then go on with life
                    ChangeDir(dirToEnemy); // Gotta get dir to enemy
                    LastRandomMove = Globals.Timing.Milliseconds + Randomization.Next(1000, 3000);

                    return;
                }
            }

            if (spellBase.VitalCost == null)
            {
                return;
            }

            if (spellBase.VitalCost[(int) Vitals.Mana] > GetVital(Vitals.Mana))
            {
                return;
            }

            if (spellBase.VitalCost[(int) Vitals.Health] > GetVital(Vitals.Health))
            {
                return;
            }

            var spell = Spells[spellIndex];
            if (spell == null)
            {
                return;
            }

            if (SpellCooldowns.ContainsKey(spell.SpellId) && SpellCooldowns[spell.SpellId] >= Globals.Timing.MillisecondsUTC)
            {
                return;
            }

            if (!InRangeOf(Target, range))
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (targetType)
                {
                    case SpellTargetTypes.Self:
                    case SpellTargetTypes.AoE:
                        return;
                }
            }

            CastTime = Globals.Timing.Milliseconds + spellBase.CastDuration;

            if (spellBase.VitalCost[(int) Vitals.Mana] > 0)
            {
                SubVital(Vitals.Mana, spellBase.VitalCost[(int) Vitals.Mana]);
            }
            else
            {
                AddVital(Vitals.Mana, -spellBase.VitalCost[(int) Vitals.Mana]);
            }

            if (spellBase.VitalCost[(int) Vitals.Health] > 0)
            {
                SubVital(Vitals.Health, spellBase.VitalCost[(int) Vitals.Health]);
            }
            else
            {
                AddVital(Vitals.Health, -spellBase.VitalCost[(int) Vitals.Health]);
            }

            if ((spellBase.Combat?.Friendly ?? false) && spellBase.SpellType != SpellTypes.WarpTo)
            {
                CastTarget = this;
            }
            else
            {
                CastTarget = Target;
            }

            switch (Base.SpellFrequency)
            {
                case 0:
                    CastFreq = Globals.Timing.Milliseconds + 30000;

                    break;

                case 1:
                    CastFreq = Globals.Timing.Milliseconds + 15000;

                    break;

                case 2:
                    CastFreq = Globals.Timing.Milliseconds + 8000;

                    break;

                case 3:
                    CastFreq = Globals.Timing.Milliseconds + 4000;

                    break;

                case 4:
                    CastFreq = Globals.Timing.Milliseconds + 2000;

                    break;
            }

            SpellCastSlot = spellIndex;

            if (spellBase.CastAnimationId != Guid.Empty)
            {
                PacketSender.SendAnimationToProximity(spellBase.CastAnimationId, 1, Id, MapId, 0, 0, (sbyte) Dir);

                //Target Type 1 will be global entity
            }

            PacketSender.SendEntityVitals(this);
            PacketSender.SendEntityCastTime(this, spellId);
        }

        // TODO: Improve NPC movement to be more fluid like a player
        //General Updating
        public override void Update(long timeMs)
        {
            var curMapLink = MapId;
            base.Update(timeMs);

            var statuses = Statuses.Values.ToArray();
            foreach (var status in statuses)
            {
                if (status.Type == StatusTypes.Stun || status.Type == StatusTypes.Sleep)
                {
                    return;
                }
            }

            //TODO Clear Damage Map if out of combat (target is null and combat timer is to the point that regen has started)
            if (Target == null && Globals.Timing.Milliseconds > CombatTimer && Globals.Timing.Milliseconds > RegenTimer)
            {
                DamageMap.Clear();
            }

            var fleeing = false;
            if (Base.FleeHealthPercentage > 0)
            {
                var fleeHpCutoff = GetMaxVital(Vitals.Health) * ((float) Base.FleeHealthPercentage / 100f);
                if (GetVital(Vitals.Health) < fleeHpCutoff)
                {
                    fleeing = true;
                }
            }

            if (MoveTimer < Globals.Timing.Milliseconds)
            {
                var targetMap = Guid.Empty;
                var targetX = 0;
                var targetY = 0;
                var targetZ = 0;

                //Check if there is a target, if so, run their ass down.
                if (Target != null)
                {
                    if (!Target.IsDead() && CanAttack(Target, null))
                    {
                        targetMap = Target.MapId;
                        targetX = Target.X;
                        targetY = Target.Y;
                        targetZ = Target.Z;
                        var targetStatuses = Target.Statuses.Values.ToArray();
                        foreach (var targetStatus in targetStatuses)
                        {
                            if (targetStatus.Type == StatusTypes.Stealth)
                            {
                                targetMap = Guid.Empty;
                                targetX = 0;
                                targetY = 0;
                                targetZ = 0;
                            }
                        }
                    }
                    else
                    {
                        if (CastTime <= 0)
                        {
                            RemoveTarget();
                        }
                    }
                }
                else //Find a target if able
                {
                    long dmg = 0;
                    Entity tgt = null;
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
                        TryFindNewTarget(timeMs);
                    }
                }

                if (targetMap != Guid.Empty)
                {
                    //Check if target map is on one of the surrounding maps, if not then we are not even going to look.
                    if (targetMap != MapId)
                    {
                        if (MapInstance.Get(MapId).SurroundingMaps.Count > 0)
                        {
                            for (var x = 0; x < MapInstance.Get(MapId).SurroundingMaps.Count; x++)
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
                        mPathFinder.SetTarget(new PathfinderTarget(targetMap, targetX, targetY, targetZ));
                    }

                }

                if (mPathFinder.GetTarget() != null)
                {
                    TryCastSpells();
                    if (!IsOneBlockAway(
                        mPathFinder.GetTarget().TargetMapId, mPathFinder.GetTarget().TargetX,
                        mPathFinder.GetTarget().TargetY, mPathFinder.GetTarget().TargetZ
                    ))
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
                                        statuses = Statuses.Values.ToArray();
                                        foreach (var status in statuses)
                                        {
                                            if (status.Type == StatusTypes.Stun ||
                                                status.Type == StatusTypes.Snare ||
                                                status.Type == StatusTypes.Sleep)
                                            {
                                                return;
                                            }
                                        }

                                        Move((byte)dir, null);
                                    }
                                    else
                                    {
                                        mPathFinder.PathFailed(timeMs);
                                    }

                                    // Have we reached our destination? If so, clear it.
                                    var tloc = mPathFinder.GetTarget();
                                    if (tloc.TargetMapId == MapId && tloc.TargetX == X && tloc.TargetY == Y)
                                    {
                                        targetMap = Guid.Empty;

                                        // Reset our aggro center so we can get "pulled" again.
                                        AggroCenterMap = null;
                                        AggroCenterX = 0;
                                        AggroCenterY = 0;
                                        AggroCenterZ = 0;
                                    }
                                }

                                break;
                            case PathfinderResult.OutOfRange:
                                RemoveTarget();
                                targetMap = Guid.Empty;

                                break;
                            case PathfinderResult.NoPathToTarget:
                                TryFindNewTarget(timeMs, Target?.Id ?? Guid.Empty);
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
                        if (Target != null && fleeing)
                        {
                            var dir = DirToEnemy(Target);
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
                                statuses = Statuses.Values.ToArray();
                                foreach (var status in statuses)
                                {
                                    if (status.Type == StatusTypes.Stun ||
                                        status.Type == StatusTypes.Snare ||
                                        status.Type == StatusTypes.Sleep)
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
                            if (Target != null)
                            {
                                if (Dir != DirToEnemy(Target) && DirToEnemy(Target) != -1)
                                {
                                    ChangeDir(DirToEnemy(Target));
                                }
                                else
                                {
                                    if (Target.IsDisposed)
                                    {
                                        Target = null;
                                    }
                                    else
                                    {
                                        if (CanAttack(Target, null))
                                        {
                                            TryAttack(Target);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }



                //Move randomly
                if (targetMap != Guid.Empty)
                {
                    return;
                }

                if (LastRandomMove >= Globals.Timing.Milliseconds || CastTime > 0)
                {
                    return;
                }

                if (Base.Movement == (int) NpcMovement.StandStill)
                {
                    LastRandomMove = Globals.Timing.Milliseconds + Randomization.Next(1000, 3000);

                    return;
                }
                else if (Base.Movement == (int) NpcMovement.TurnRandomly)
                {
                    ChangeDir((byte)Randomization.Next(0, 4));
                    LastRandomMove = Globals.Timing.Milliseconds + Randomization.Next(1000, 3000);

                    return;
                }

                var i = Randomization.Next(0, 1);
                if (i == 0)
                {
                    i = Randomization.Next(0, 4);
                    if (CanMove(i) == -1)
                    {
                        //check if NPC is snared or stunned
                        statuses = Statuses.Values.ToArray();
                        foreach (var status in statuses)
                        {
                            if (status.Type == StatusTypes.Stun ||
                                status.Type == StatusTypes.Snare ||
                                status.Type == StatusTypes.Sleep)
                            {
                                return;
                            }
                        }

                        Move((byte) i, null);
                    }
                }

                LastRandomMove = Globals.Timing.Milliseconds + Randomization.Next(1000, 3000);

                if (fleeing)
                {
                    LastRandomMove = Globals.Timing.Milliseconds + (long) GetMovementTime();
                }
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

            // Check if we've moved out of our range we're allowed to move from after being "aggro'd" by something.
            // If so, remove target and move back to the origin point.
            if (Options.Npc.AllowResetRadius && AggroCenterMap != null && GetDistanceTo(AggroCenterMap, AggroCenterX, AggroCenterY) > Options.Npc.ResetRadius)
            {
                // Remove our target.
                RemoveTarget();

                // Reset our vitals and statusses when configured.
                if (Options.Npc.ResetVitalsAndStatusses)
                {
                    Statuses.Clear();
                    DoT.Clear();
                    for (var v = 0; v < (int)Vitals.VitalCount; v++)
                    {
                        RestoreVital((Vitals)v);
                    }
                }

                // Try and move back to where we came from before we started chasing something.
                mPathFinder.SetTarget(new PathfinderTarget(AggroCenterMap.Id, AggroCenterX, AggroCenterY, AggroCenterZ));
            }
        }

        public override void NotifySwarm(Entity attacker)
        {
            var mapEntities = MapInstance.Get(MapId).GetEntities(true);
            foreach (var en in mapEntities)
            {
                if (en.GetType() == typeof(Npc))
                {
                    var npc = (Npc) en;
                    if (npc.Target == null & npc.Base.Swarm && npc.Base == Base)
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
            if (IsAllyOf(en))
            {
                return false;
            }

            //If not then check and see if player meets the conditions to attack the npc...
            if (Base.PlayerCanAttackConditions.Lists.Count == 0 ||
                Conditions.MeetsConditionLists(Base.PlayerCanAttackConditions, en, null))
            {
                return true;
            }

            return false;
        }

        public override bool IsAllyOf(Entity otherEntity)
        {
            switch (otherEntity)
            {
                case Npc otherNpc:
                    return Base == otherNpc.Base;
                case Player otherPlayer:
                    var conditionLists = Base.PlayerFriendConditions;
                    if ((conditionLists?.Count ?? 0) == 0)
                    {
                        return false;
                    }

                    return Conditions.MeetsConditionLists(conditionLists, otherPlayer, null);
                default:
                    return base.IsAllyOf(otherEntity);
            }
        }

        public bool ShouldAttackPlayerOnSight(Player en)
        {
            if (IsAllyOf(en))
            {
                return false;
            }

            if (Base.Aggressive)
            {
                if (Base.AttackOnSightConditions.Lists.Count > 0 &&
                    Conditions.MeetsConditionLists(Base.AttackOnSightConditions, en, null))
                {
                    return false;
                }

                return true;
            }
            else
            {
                if (Base.AttackOnSightConditions.Lists.Count > 0 &&
                    Conditions.MeetsConditionLists(Base.AttackOnSightConditions, en, null))
                {
                    return true;
                }
            }

            return false;
        }

        private void TryFindNewTarget(long timeMs, Guid avoidId = new Guid())
        {
            if (FindTargetWaitTime > timeMs)
            {
                return;
            }

            var maps = MapInstance.Get(MapId).GetSurroundingMaps(true);
            var possibleTargets = new List<Entity>();
            var closestRange = Range + 1; //If the range is out of range we didn't find anything.
            var closestIndex = -1;
            foreach (var map in maps)
            {
                foreach (var en in map.GetEntitiesDictionary())
                {
                    var entity = en.Value;
                    if (entity != null && entity.IsDead() == false && entity != this && entity.Id != avoidId)
                    {
                        //TODO Check if NPC is allowed to attack player with new conditions
                        if (entity.GetType() == typeof(Player))
                        {
                            if (ShouldAttackPlayerOnSight((Player) entity))
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
                        else if (entity.GetType() == typeof(Npc))
                        {
                            if (Base.Aggressive && Base.AggroList.Contains(((Npc) entity).Base.Id))
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
            }

            if (closestIndex != -1)
            {
                AssignTarget(possibleTargets[closestIndex]);
            }

            FindTargetWaitTime = timeMs + FindTargetDelay;
        }

        public override void ProcessRegen()
        {
            if (Base == null)
            {
                return;
            }

            foreach (Vitals vital in Enum.GetValues(typeof(Vitals)))
            {
                if (vital >= Vitals.VitalCount)
                {
                    continue;
                }

                var vitalId = (int) vital;
                var vitalValue = GetVital(vital);
                var maxVitalValue = GetMaxVital(vital);
                if (vitalValue >= maxVitalValue)
                {
                    continue;
                }

                var vitalRegenRate = Base.VitalRegen[vitalId] / 100f;
                var regenValue = (int) Math.Max(1, maxVitalValue * vitalRegenRate) *
                                 Math.Abs(Math.Sign(vitalRegenRate));

                AddVital(vital, regenValue);
            }
        }

        public override void Warp(
            Guid newMapId,
            byte newX,
            byte newY,
            byte newDir,
            bool adminWarp = false,
            byte zOverride = 0,
            bool mapSave = false
        )
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
            if (newMapId != MapId)
            {
                var oldMap = MapInstance.Get(MapId);
                if (oldMap != null)
                {
                    oldMap.RemoveEntity(this);
                }

                PacketSender.SendEntityLeave(this);
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

        public int GetAggression(Player player)
        {
            //Determines the aggression level of this npc to send to the player
            if (this.Target != null)
            {
                return -1;
            }
            else
            {
                //Guard = 3
                //Will attack on sight = 1
                //Will attack if attacked = 0
                //Can't attack nor can attack = 2
                var ally = IsAllyOf(player);
                var attackOnSight = ShouldAttackPlayerOnSight(player);
                var canPlayerAttack = CanPlayerAttack(player);
                if (ally && !canPlayerAttack)
                {
                    return 3;
                }

                if (attackOnSight)
                {
                    return 1;
                }

                if (!ally && !attackOnSight && canPlayerAttack)
                {
                    return 0;
                }

                if (!ally && !attackOnSight && !canPlayerAttack)
                {
                    return 2;
                }
            }

            return 2;
        }

        public override EntityPacket EntityPacket(EntityPacket packet = null, Player forPlayer = null)
        {
            if (packet == null)
            {
                packet = new NpcEntityPacket();
            }

            packet = base.EntityPacket(packet, forPlayer);

            var pkt = (NpcEntityPacket) packet;
            pkt.Aggression = GetAggression(forPlayer);

            return pkt;
        }

    }

}
