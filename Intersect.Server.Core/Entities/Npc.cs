using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
using Intersect.Server.Maps;
using Intersect.Server.Networking;
using Intersect.Utilities;
using Stat = Intersect.Enums.Stat;

namespace Intersect.Server.Entities
{

    public partial class Npc : Entity
    {

        //Spell casting
        public long CastFreq;

        /// <summary>
        /// Damage Map - Keep track of who is doing the most damage to this npc and focus accordingly
        /// </summary>
        public ConcurrentDictionary<Entity, long> DamageMap = new ConcurrentDictionary<Entity, long>();

        public ConcurrentDictionary<Guid, bool> LootMap = new ConcurrentDictionary<Guid, bool>();

        public Guid[] LootMapCache = Array.Empty<Guid>();

        /// <summary>
        /// Returns the entity that ranks the highest on this NPC's damage map.
        /// </summary>
        public Entity DamageMapHighest
        {
            get
            {
                long damage = 0;
                Entity top = null;
                foreach (var pair in DamageMap)
                {
                    // Only include players on the current instance
                    if (pair.Value > damage && pair.Key.MapInstanceId == MapInstanceId)
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

        private int mTargetFailCounter = 0;
        private int mTargetFailMax = 10;

        private int mResetDistance = 0;
        private int mResetCounter = 0;
        private int mResetMax = 100;
        private bool mResetting = false;

        /// <summary>
        /// The map on which this NPC was "aggro'd" and started chasing a target.
        /// </summary>
        public MapController AggroCenterMap;

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

        public Npc(NpcBase myBase, bool despawnable = false) : base()
        {
            Name = myBase.Name;
            Sprite = myBase.Sprite;
            Color = myBase.Color;
            Level = myBase.Level;
            Immunities = myBase.Immunities;
            Base = myBase;
            Despawnable = despawnable;

            for (var i = 0; i < Enum.GetValues<Stat>().Length; i++)
            {
                BaseStats[i] = myBase.Stats[i];
                Stat[i] = new Combat.Stat((Stat)i, this);
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
                var slot = new InventorySlot(itemSlot);
                slot.Set(new Item(drop.ItemId, drop.Quantity));
                slot.DropChance = drop.Chance;
                Items.Add(slot);
                itemSlot++;
            }

            for (var i = 0; i < Enum.GetValues<Vital>().Length; i++)
            {
                SetMaxVital(i, myBase.MaxVital[i]);
                SetVital(i, myBase.MaxVital[i]);
            }

            Range = (byte)myBase.SightRange;
            mPathFinder = new Pathfinder(this);
        }

        public NpcBase Base { get; private set; }

        private bool IsStunnedOrSleeping => CachedStatuses.Any(PredicateStunnedOrSleeping);

        private bool IsUnableToCastSpells => CachedStatuses.Any(PredicateUnableToCastSpells);

        public override EntityType GetEntityType()
        {
            return EntityType.GlobalEntity;
        }

        public override void Die(bool generateLoot = true, Entity killer = null)
        {
            lock (EntityLock)
            {
                base.Die(generateLoot, killer);

                AggroCenterMap = null;
                AggroCenterX = 0;
                AggroCenterY = 0;
                AggroCenterZ = 0;

                if (MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var instance))
                {
                    instance.RemoveEntity(this);
                }
                PacketSender.SendEntityDie(this);
                PacketSender.SendEntityLeave(this);
            }
        }

        protected override bool ShouldDropItem(Entity killer, ItemBase itemDescriptor, Item item, float dropRateModifier, out Guid lootOwner)
        {
            lootOwner = (killer as Player)?.Id ?? Id;
            return base.ShouldDropItem(killer, itemDescriptor, item, dropRateModifier, out _);
        }

        public bool TargetHasStealth(Entity target)
        {
            return target == null || target.CachedStatuses.Any(s => s.Type == SpellEffect.Stealth);
        }

        //Targeting
        public void AssignTarget(Entity en)
        {
            var oldTarget = Target;

            // Are we resetting? If so, do not allow for a new target.
            var pathTarget = mPathFinder?.GetTarget();
            if (AggroCenterMap != null && pathTarget != null &&
                pathTarget.TargetMapId == AggroCenterMap.Id && pathTarget.TargetX == AggroCenterX && pathTarget.TargetY == AggroCenterY)
            {
                if (en == null)
                {
                    return;

                }
                else
                {
                    return;

                }
            }

            //Why are we doing all of this logic if we are assigning a target that we already have?
            if (en != null && en != Target)
            {
                // Can't assign a new target if taunted, unless we're resetting their target somehow.
                // Also make sure the taunter is in range.. If they're dead or gone, we go for someone else!
                if ((pathTarget != null && AggroCenterMap != null && (pathTarget.TargetMapId != AggroCenterMap.Id || pathTarget.TargetX != AggroCenterX || pathTarget.TargetY != AggroCenterY)))
                {
                    foreach (var status in CachedStatuses)
                    {
                        if (status.Type == SpellEffect.Taunt && en != status.Attacker && GetDistanceTo(status.Attacker) != 9999)
                        {
                            return;
                        }
                    }
                }

                if (en is Projectile projectile)
                {
                    if (projectile.Owner != this && !TargetHasStealth(projectile))
                    {
                        Target = projectile.Owner;
                    }
                }
                else
                {
                    if (en is Npc npc)
                    {
                        if (npc.Base == Base)
                        {
                            if (Base.AttackAllies == false)
                            {
                                return;
                            }
                        }
                    }

                    if (en is Player)
                    {
                        //TODO Make sure that the npc can target the player
                        if (this != en && !TargetHasStealth(en))
                        {
                            Target = en;
                        }
                    }
                    else
                    {
                        if (this != en && !TargetHasStealth(en))
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
            }
            else
            {
                Target = en;
            }

            if (Target != oldTarget)
            {
                CombatTimer = Timing.Global.Milliseconds + Options.CombatTime;
                PacketSender.SendNpcAggressionToProximity(this);
            }
            mTargetFailCounter = 0;
        }

        public void RemoveFromDamageMap(Entity en)
        {
            DamageMap.TryRemove(en, out _);
        }

        public void RemoveTarget()
        {
            AssignTarget(null);
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

            if (entity is EventPageInstance)
            {
                return false;
            }

            //Check if the attacker is stunned or blinded.
            foreach (var status in CachedStatuses)
            {
                if (status.Type == SpellEffect.Stun || status.Type == SpellEffect.Sleep)
                {
                    return false;
                }
            }

            if (TargetHasStealth(entity))
            {
                return false;
            }

            if (entity is Resource)
            {
                if (!entity.Passable)
                {
                    return false;
                }
            }
            else if (entity is Npc)
            {
                return CanNpcCombat(entity, spell != null && spell.Combat.Friendly) || entity == this;
            }
            else if (entity is Player player)
            {
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

            var deadAnimations = new List<KeyValuePair<Guid, Direction>>();
            var aliveAnimations = new List<KeyValuePair<Guid, Direction>>();

            //We were forcing at LEAST 1hp base damage.. but then you can't have guards that won't hurt the player.
            //https://www.ascensiongamedev.com/community/bug_tracker/intersect/npc-set-at-0-attack-damage-still-damages-player-by-1-initially-r915/
            if (IsAttacking)
            {
                return;
            }

            if (Base.AttackAnimation != null)
            {
                PacketSender.SendAnimationToProximity(
                    Base.AttackAnimationId, -1, Guid.Empty, target.MapId, (byte)target.X, (byte)target.Y,
                    Dir, target.MapInstanceId
                );
            }

            base.TryAttack(
                target, Base.Damage, (DamageType)Base.DamageType, (Stat)Base.ScalingStat, Base.Scaling,
                Base.CritChance, Base.CritMultiplier, deadAnimations, aliveAnimations
            );

            PacketSender.SendEntityAttack(this, CalculateAttackTime());
        }

        public bool CanNpcCombat(Entity enemy, bool friendly = false)
        {
            //Check for NpcVsNpc Combat, both must be enabled and the attacker must have it as an enemy or attack all types of npc.
            if (!friendly)
            {
                if (enemy != null && enemy is Npc enemyNpc && Base != null)
                {
                    if (enemyNpc.Base.NpcVsNpcEnabled == false)
                    {
                        return false;
                    }

                    if (Base.AttackAllies && enemyNpc.Base == Base)
                    {
                        return true;
                    }

                    for (var i = 0; i < Base.AggroList.Count; i++)
                    {
                        if (NpcBase.Get(Base.AggroList[i]) == enemyNpc.Base)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                if (enemy is Player)
                {
                    return true;
                }
            }
            else if (enemy is Npc enemyNpc && Base != null && enemyNpc.Base == Base && Base.AttackAllies == false)
            {
                return true;
            }
            else if (enemy is Player)
            {
                return false;
            }

            return false;
        }

        private static bool PredicateStunnedOrSleeping(Status status)
        {
            switch (status?.Type)
            {
                case SpellEffect.Sleep:
                case SpellEffect.Stun:
                    return true;

                case SpellEffect.Silence:
                case SpellEffect.None:
                case SpellEffect.Snare:
                case SpellEffect.Blind:
                case SpellEffect.Stealth:
                case SpellEffect.Transform:
                case SpellEffect.Cleanse:
                case SpellEffect.Invulnerable:
                case SpellEffect.Shield:
                case SpellEffect.OnHit:
                case SpellEffect.Taunt:
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
                case SpellEffect.Silence:
                case SpellEffect.Sleep:
                case SpellEffect.Stun:
                    return true;

                case SpellEffect.None:
                case SpellEffect.Snare:
                case SpellEffect.Blind:
                case SpellEffect.Stealth:
                case SpellEffect.Transform:
                case SpellEffect.Cleanse:
                case SpellEffect.Invulnerable:
                case SpellEffect.Shield:
                case SpellEffect.OnHit:
                case SpellEffect.Taunt:
                case null:
                    return false;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override bool IgnoresNpcAvoid => false;

        /// <inheritdoc />
        public override bool CanMoveInDirection(
            Direction direction,
            out MovementBlockerType blockerType,
            out EntityType entityType
        )
        {
            entityType = default;

            if (Base.Movement == (byte)NpcMovement.Static)
            {
                blockerType = MovementBlockerType.MapAttribute;
                return false;
            }

            if (
                !base.CanMoveInDirection(direction, out blockerType, out entityType)
                && blockerType == MovementBlockerType.Entity
                && Options.Instance.NpcOpts.IntangibleDuringReset
            )
            {
                if (mResetting)
                {
                    blockerType = MovementBlockerType.NotBlocked;
                }
            }

            if ((blockerType != MovementBlockerType.NotBlocked && blockerType != MovementBlockerType.Slide) ||
                !IsFleeing() ||
                !Options.Instance.NpcOpts.AllowResetRadius)
            {
                return blockerType == MovementBlockerType.NotBlocked;
            }

            var yOffset = 0;
            var xOffset = 0;
            var tile = new TileHelper(MapId, X, Y);
            switch (direction)
            {
                case Direction.Up:
                    yOffset--;
                    break;

                case Direction.Down:
                    yOffset++;
                    break;

                case Direction.Left:
                    xOffset--;
                    break;

                case Direction.Right:
                    xOffset++;
                    break;

                case Direction.UpLeft:
                    yOffset--;
                    xOffset--;
                    break;

                case Direction.UpRight:
                    yOffset--;
                    xOffset++;
                    break;

                case Direction.DownLeft:
                    yOffset++;
                    xOffset--;
                    break;

                case Direction.DownRight:
                    yOffset++;
                    xOffset++;
                    break;

                case Direction.None:
                default:
                    break;
            }

            // ReSharper disable once InvertIf
            if (tile.Translate(xOffset, yOffset))
            {
                // If this would move us past our reset radius then we cannot move.
                var dist = GetDistanceBetween(
                    AggroCenterMap,
                    tile.GetMap(),
                    AggroCenterX,
                    tile.GetX(),
                    AggroCenterY,
                    tile.GetY()
                );

                // ReSharper disable once InvertIf
                if (dist > Math.Max(Options.Npc.ResetRadius, Base.ResetRadius))
                {
                    blockerType = MovementBlockerType.MapAttribute;
                    return false;
                }
            }

            return blockerType == MovementBlockerType.NotBlocked;
        }

        private void TryCastSpells()
        {
            var target = Target;

            if (target == null || mPathFinder.GetTarget() == null)
            {
                return;
            }

            // Check if NPC is stunned/sleeping
            if (IsStunnedOrSleeping)
            {
                return;
            }

            //Check if NPC is casting a spell
            if (IsCasting)
            {
                return; //can't move while casting
            }

            if (CastFreq >= Timing.Global.Milliseconds)
            {
                return;
            }

            // Check if the NPC is able to cast spells
            if (IsUnableToCastSpells)
            {
                return;
            }

            if (Base.Spells is not { Count: > 0 })
            {
                return;
            }

            // Pick a random spell
            var spellIndex = Randomization.Next(0, Spells.Count);
            var spellId = Base.Spells[spellIndex];
            if (!SpellBase.TryGet(spellId, out var spellBase))
            {
                return;
            }

            if (spellBase.Combat == null)
            {
                Log.Warn($"Combat data missing for {spellBase.Id}.");
            }

            // Check if we are even allowed to cast this spell.
            if (!CanCastSpell(spellBase, target, true, out _))
            {
                return;
            }

            var targetType = spellBase.Combat?.TargetType ?? SpellTargetType.Single;
            var projectileBase = spellBase.Combat?.Projectile;

            if (spellBase.SpellType == SpellType.CombatSpell &&
                targetType == SpellTargetType.Projectile &&
                projectileBase != null &&
                InRangeOf(target, projectileBase.Range))
            {
                var dirToEnemy = DirectionToTarget(target);
                if (dirToEnemy != Dir)
                {
                    if (LastRandomMove >= Timing.Global.Milliseconds)
                    {
                        return;
                    }

                    //Face the target -- next frame fire -- then go on with life
                    ChangeDir(dirToEnemy); // Gotta get dir to enemy
                    LastRandomMove = Timing.Global.Milliseconds + Randomization.Next(1000, 3000);

                    return;
                }
            }

            CastTime = Timing.Global.Milliseconds + spellBase.CastDuration;

            if ((spellBase.Combat?.Friendly ?? false) && spellBase.SpellType != SpellType.WarpTo)
            {
                CastTarget = this;
            }
            else
            {
                CastTarget = target;
            }

            switch (Base.SpellFrequency)
            {
                case 0:
                    CastFreq = Timing.Global.Milliseconds + 30000;

                    break;

                case 1:
                    CastFreq = Timing.Global.Milliseconds + 15000;

                    break;

                case 2:
                    CastFreq = Timing.Global.Milliseconds + 8000;

                    break;

                case 3:
                    CastFreq = Timing.Global.Milliseconds + 4000;

                    break;

                case 4:
                    CastFreq = Timing.Global.Milliseconds + 2000;

                    break;
            }

            SpellCastSlot = spellIndex;

            if (spellBase.CastAnimationId != Guid.Empty)
            {
                PacketSender.SendAnimationToProximity(spellBase.CastAnimationId, 1, Id, MapId, 0, 0, Dir, MapInstanceId);

                //Target Type 1 will be global entity
            }

            PacketSender.SendEntityCastTime(this, spellId);
        }

        public bool IsFleeing()
        {
            if (Base.FleeHealthPercentage > 0)
            {
                var fleeHpCutoff = GetMaxVital(Vital.Health) * (Base.FleeHealthPercentage / 100f);
                if (GetVital(Vital.Health) < fleeHpCutoff)
                {
                    return true;
                }
            }
            return false;
        }

        // TODO: Improve NPC movement to be more fluid like a player
        //General Updating
        public override void Update(long timeMs)
        {
            var lockObtained = false;
            try
            {
                Monitor.TryEnter(EntityLock, ref lockObtained);
                if (lockObtained)
                {
                    var curMapLink = MapId;
                    base.Update(timeMs);
                    var tempTarget = Target;

                    foreach (var status in CachedStatuses)
                    {
                        if (status.Type == SpellEffect.Stun || status.Type == SpellEffect.Sleep)
                        {
                            return;
                        }
                    }

                    var fleeing = IsFleeing();

                    if (MoveTimer < Timing.Global.Milliseconds)
                    {
                        var targetMap = Guid.Empty;
                        var targetX = 0;
                        var targetY = 0;
                        var targetZ = 0;

                        //TODO Clear Damage Map if out of combat (target is null and combat timer is to the point that regen has started)
                        if (tempTarget != null && (Options.Instance.NpcOpts.ResetIfCombatTimerExceeded && Timing.Global.Milliseconds > CombatTimer))
                        {
                            if (CheckForResetLocation(true))
                            {
                                if (Target != tempTarget)
                                {
                                    PacketSender.SendNpcAggressionToProximity(this);
                                }
                                return;
                            }
                        }

                        // Are we resetting? If so, regenerate completely!
                        if (mResetting)
                        {
                            var distance = GetDistanceTo(AggroCenterMap, AggroCenterX, AggroCenterY);
                            // Have we reached our destination? If so, clear it.
                            if (distance < 1)
                            {
                                ResetAggroCenter(out targetMap);
                            }

                            Reset(Options.Instance.NpcOpts.ContinuouslyResetVitalsAndStatuses);
                            tempTarget = Target;

                            if (distance != mResetDistance)
                            {
                                mResetDistance = distance;
                            }
                            else
                            {
                                // Something is fishy here.. We appear to be stuck in a reset loop?
                                // Give it a few more attempts and reset the NPC's center if we're stuck!
                                mResetCounter++;
                                if (mResetCounter > mResetMax)
                                {
                                    ResetAggroCenter(out targetMap);
                                    mResetCounter = 0;
                                    mResetDistance = 0;
                                }
                            }

                        }

                        if (tempTarget != null && (tempTarget.IsDead() || !InRangeOf(tempTarget, Options.MapWidth * 2)))
                        {
                            TryFindNewTarget(Timing.Global.Milliseconds, tempTarget.Id);
                            tempTarget = Target;
                        }

                        //Check if there is a target, if so, run their ass down.
                        if (tempTarget != null)
                        {
                            if (!tempTarget.IsDead() && CanAttack(tempTarget, null))
                            {
                                targetMap = tempTarget.MapId;
                                targetX = tempTarget.X;
                                targetY = tempTarget.Y;
                                targetZ = tempTarget.Z;
                                foreach (var targetStatus in tempTarget.CachedStatuses)
                                {
                                    if (targetStatus.Type == SpellEffect.Stealth)
                                    {
                                        targetMap = Guid.Empty;
                                        targetX = 0;
                                        targetY = 0;
                                        targetZ = 0;
                                    }
                                }
                            }
                        }
                        else //Find a target if able
                        {
                            // Check if attack on sight or have other npc's to target
                            TryFindNewTarget(timeMs);
                            tempTarget = Target;
                        }

                        if (targetMap != Guid.Empty)
                        {
                            //Check if target map is on one of the surrounding maps, if not then we are not even going to look.
                            if (targetMap != MapId)
                            {
                                var found = false;
                                foreach (var map in MapController.Get(MapId).SurroundingMaps)
                                {
                                    if (map.Id == targetMap)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
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

                                if (tempTarget != Target)
                                {
                                    tempTarget = Target;
                                }
                            }

                        }

                        if (mPathFinder.GetTarget() != null && Base.Movement != (int)NpcMovement.Static)
                        {
                            TryCastSpells();
                            // TODO: Make resetting mobs actually return to their starting location.
                            if ((!mResetting && !IsOneBlockAway(
                                mPathFinder.GetTarget().TargetMapId, mPathFinder.GetTarget().TargetX,
                                mPathFinder.GetTarget().TargetY, mPathFinder.GetTarget().TargetZ
                            )) ||
                            (mResetting && GetDistanceTo(AggroCenterMap, AggroCenterX, AggroCenterY) != 0)
                            )
                            {
                                switch (mPathFinder.Update(timeMs))
                                {
                                    case PathfinderResult.Success:

                                        var dir = mPathFinder.GetMove();
                                        if (dir > Direction.None)
                                        {
                                            if (fleeing)
                                            {
                                                switch (dir)
                                                {
                                                    case Direction.Up:
                                                        dir = Direction.Down;

                                                        break;
                                                    case Direction.Down:
                                                        dir = Direction.Up;

                                                        break;
                                                    case Direction.Left:
                                                        dir = Direction.Right;

                                                        break;
                                                    case Direction.Right:
                                                        dir = Direction.Left;

                                                        break;
                                                    case Direction.UpLeft:
                                                        dir = Direction.UpRight;

                                                        break;
                                                    case Direction.UpRight:
                                                        dir = Direction.UpLeft;

                                                        break;
                                                    case Direction.DownRight:
                                                        dir = Direction.DownLeft;

                                                        break;
                                                    case Direction.DownLeft:
                                                        dir = Direction.DownRight;

                                                        break;
                                                }
                                            }

                                            if (CanMoveInDirection(dir, out var blockerType, out _) || blockerType == MovementBlockerType.Slide)
                                            {
                                                //check if NPC is snared or stunned
                                                foreach (var status in CachedStatuses)
                                                {
                                                    if (status.Type == SpellEffect.Stun ||
                                                        status.Type == SpellEffect.Snare ||
                                                        status.Type == SpellEffect.Sleep)
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

                                            // Are we resetting?
                                            if (mResetting)
                                            {
                                                // Have we reached our destination? If so, clear it.
                                                if (GetDistanceTo(AggroCenterMap, AggroCenterX, AggroCenterY) == 0)
                                                {
                                                    targetMap = Guid.Empty;

                                                    // Reset our aggro center so we can get "pulled" again.
                                                    AggroCenterMap = null;
                                                    AggroCenterX = 0;
                                                    AggroCenterY = 0;
                                                    AggroCenterZ = 0;
                                                    mPathFinder?.SetTarget(null);
                                                    mResetting = false;
                                                }
                                            }
                                        }

                                        break;
                                    case PathfinderResult.OutOfRange:
                                        TryFindNewTarget(timeMs, tempTarget?.Id ?? Guid.Empty, true);
                                        tempTarget = Target;
                                        targetMap = Guid.Empty;

                                        break;
                                    case PathfinderResult.NoPathToTarget:
                                        TryFindNewTarget(timeMs, tempTarget?.Id ?? Guid.Empty, true);
                                        tempTarget = Target;
                                        targetMap = Guid.Empty;

                                        break;
                                    case PathfinderResult.Failure:
                                        targetMap = Guid.Empty;
                                        TryFindNewTarget(timeMs, tempTarget?.Id ?? Guid.Empty, true);
                                        tempTarget = Target;

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
                                if (tempTarget != null && fleeing)
                                {
                                    var dir = DirectionToTarget(tempTarget);
                                    switch (dir)
                                    {
                                        case Direction.Up:
                                            dir = Direction.Down;

                                            break;
                                        case Direction.Down:
                                            dir = Direction.Up;

                                            break;
                                        case Direction.Left:
                                            dir = Direction.Right;

                                            break;
                                        case Direction.Right:
                                            dir = Direction.Left;

                                            break;
                                        case Direction.UpLeft:
                                            dir = Direction.UpRight;

                                            break;
                                        case Direction.UpRight:
                                            dir = Direction.UpLeft;
                                            break;

                                        case Direction.DownRight:
                                            dir = Direction.DownLeft;

                                            break;
                                        case Direction.DownLeft:
                                            dir = Direction.DownRight;

                                            break;
                                    }

                                    if (CanMoveInDirection(dir, out var blockerType, out _) || blockerType == MovementBlockerType.Slide)
                                    {
                                        //check if NPC is snared or stunned
                                        foreach (var status in CachedStatuses)
                                        {
                                            if (status.Type == SpellEffect.Stun ||
                                                status.Type == SpellEffect.Snare ||
                                                status.Type == SpellEffect.Sleep)
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
                                    if (tempTarget != null)
                                    {
                                        if (Dir != DirectionToTarget(tempTarget) && DirectionToTarget(tempTarget) != Direction.None)
                                        {
                                            ChangeDir(DirectionToTarget(tempTarget));
                                        }
                                        else
                                        {
                                            if (tempTarget.IsDisposed)
                                            {
                                                TryFindNewTarget(timeMs);
                                                tempTarget = Target;
                                            }
                                            else
                                            {
                                                if (CanAttack(tempTarget, null))
                                                {
                                                    TryAttack(tempTarget);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        CheckForResetLocation();

                        //Move randomly
                        if (targetMap != Guid.Empty)
                        {
                            return;
                        }

                        if (LastRandomMove >= Timing.Global.Milliseconds || IsCasting)
                        {
                            return;
                        }

                        if (Base.Movement == (int)NpcMovement.StandStill)
                        {
                            LastRandomMove = Timing.Global.Milliseconds + Randomization.Next(1000, 3000);

                            return;
                        }
                        else if (Base.Movement == (int)NpcMovement.TurnRandomly)
                        {
                            ChangeDir(Randomization.NextDirection());
                            LastRandomMove = Timing.Global.Milliseconds + Randomization.Next(1000, 3000);

                            return;
                        }

                        var i = Randomization.Next(0, 1);
                        if (i == 0)
                        {
                            var direction = Randomization.NextDirection();
                            if (CanMoveInDirection(direction))
                            {
                                //check if NPC is snared or stunned
                                foreach (var status in CachedStatuses)
                                {
                                    if (status.Type == SpellEffect.Stun ||
                                        status.Type == SpellEffect.Snare ||
                                        status.Type == SpellEffect.Sleep)
                                    {
                                        return;
                                    }
                                }

                                Move(direction, null);
                            }
                        }

                        LastRandomMove = Timing.Global.Milliseconds + Randomization.Next(1000, 3000);

                        if (fleeing)
                        {
                            LastRandomMove = Timing.Global.Milliseconds + (long)GetMovementTime();
                        }
                    }

                    //If we switched maps, lets update the maps
                    if (curMapLink != MapId)
                    {
                        if (curMapLink == Guid.Empty)
                        {
                            if (MapController.TryGetInstanceFromMap(curMapLink, MapInstanceId, out var instance))
                            {
                                instance.RemoveEntity(this);
                            }
                        }

                        if (MapId != Guid.Empty)
                        {
                            if (MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var instance))
                            {
                                instance.AddEntity(this);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (lockObtained)
                {
                    Monitor.Exit(EntityLock);
                }
            }
        }

        /// <summary>
        /// Resets the NPCs position to be "pulled" from
        /// </summary>
        /// <param name="targetMap">For referencing the map that the enemy's target WAS on before a reset.</param>
        private void ResetAggroCenter(out Guid targetMap)
        {
            targetMap = Guid.Empty;

            // Reset our aggro center so we can get "pulled" again.
            AggroCenterMap = null;
            AggroCenterX = 0;
            AggroCenterY = 0;
            AggroCenterZ = 0;
            mPathFinder?.SetTarget(null);
            mResetting = false;
        }

        private bool CheckForResetLocation(bool forceDistance = false)
        {
            // Check if we've moved out of our range we're allowed to move from after being "aggro'd" by something.
            // If so, remove target and move back to the origin point.
            if (Options.Npc.AllowResetRadius && AggroCenterMap != null && (GetDistanceTo(AggroCenterMap, AggroCenterX, AggroCenterY) > Math.Max(Options.Npc.ResetRadius, Math.Min(Base.ResetRadius, Math.Max(Options.MapWidth, Options.MapHeight))) || forceDistance))
            {
                Reset(Options.Npc.ResetVitalsAndStatusses);

                mResetCounter = 0;
                mResetDistance = 0;

                // Try and move back to where we came from before we started chasing something.
                mResetting = true;
                mPathFinder.SetTarget(new PathfinderTarget(AggroCenterMap.Id, AggroCenterX, AggroCenterY, AggroCenterZ));
                return true;
            }
            return false;
        }

        private void Reset(bool resetVitals, bool clearLocation = false)
        {
            // Remove our target.
            RemoveTarget();

            DamageMap.Clear();
            LootMap.Clear();
            LootMapCache = Array.Empty<Guid>();

            if (clearLocation)
            {
                mPathFinder.SetTarget(null);
                AggroCenterMap = null;
                AggroCenterX = 0;
                AggroCenterY = 0;
                AggroCenterZ = 0;
            }

            // Reset our vitals and statusses when configured.
            if (resetVitals)
            {
                Statuses.Clear();
                CachedStatuses = Statuses.Values.ToArray();
                DoT.Clear();
                CachedDots = DoT.Values.ToArray();
                for (var v = 0; v < Enum.GetValues<Vital>().Length; v++)
                {
                    RestoreVital((Vital)v);
                }
            }
        }

        // Completely resets an Npc to full health and its spawnpoint if it's current chasing something.
        public override void Reset()
        {
            if (AggroCenterMap != null)
            {
                Warp(AggroCenterMap.Id, AggroCenterX, AggroCenterY);
            }

            Reset(true, true);
        }

        public override void NotifySwarm(Entity attacker)
        {
            if (MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var instance))
            {
                foreach (var en in instance.GetEntities(true))
                {
                    if (en is Npc npc)
                    {
                        if (npc.Target == null && npc.Base == Base && npc.Base.Swarm)
                        {
                            if (npc.InRangeOf(attacker, npc.Base.SightRange))
                            {
                                npc.AssignTarget(attacker);
                            }
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
                    if (!Base.NpcVsNpcEnabled && !otherNpc.Base.NpcVsNpcEnabled)
                    {
                        return true;
                    }

                    return !otherNpc.CanNpcCombat(this);
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

        public void TryFindNewTarget(long timeMs, Guid avoidId = new Guid(), bool ignoreTimer = false, Entity attackedBy = null)
        {
            if (!ignoreTimer && FindTargetWaitTime > timeMs)
            {
                return;
            }

            // Are we resetting? If so, do not allow for a new target.
            var pathTarget = mPathFinder?.GetTarget();
            if (AggroCenterMap != null && pathTarget != null &&
                pathTarget.TargetMapId == AggroCenterMap.Id && pathTarget.TargetX == AggroCenterX && pathTarget.TargetY == AggroCenterY)
            {
                if (!Options.Instance.NpcOpts.AllowEngagingWhileResetting || attackedBy == null || attackedBy.GetDistanceTo(AggroCenterMap, AggroCenterX, AggroCenterY) > Math.Max(Options.Instance.NpcOpts.ResetRadius, Base.ResetRadius))
                {
                    return;
                }
                else
                {
                    //We're resetting and just got attacked, and we allow reengagement.. let's stop resetting and fight!
                    mPathFinder?.SetTarget(null);
                    mResetting = false;
                    AssignTarget(attackedBy);
                    return;
                }
            }

            var possibleTargets = new List<Entity>();
            var closestRange = Range + 1; //If the range is out of range we didn't find anything.
            var closestIndex = -1;
            var highestDmgIndex = -1;

            if (DamageMap.Count > 0)
            {
                // Go through all of our potential targets in order of damage done as instructed and select the first matching one.
                long highestDamage = 0;
                foreach (var en in DamageMap.ToArray())
                {
                    // Are we supposed to avoid this one?
                    if (en.Key.Id == avoidId)
                    {
                        continue;
                    }

                    // Is this entry dead?, if so skip it.
                    if (en.Key.IsDead())
                    {
                        continue;
                    }

                    // Is this entity on our instance anymore? If not skip it, but don't remove it in case they come back and need item drop determined
                    if (en.Key.MapInstanceId != MapInstanceId)
                    {
                        continue;
                    }

                    // Are we at a valid distance? (9999 means not on this map or somehow null!)
                    if (GetDistanceTo(en.Key) != 9999)
                    {
                        possibleTargets.Add(en.Key);

                        // Do we have the highest damage?
                        if (en.Value > highestDamage)
                        {
                            highestDmgIndex = possibleTargets.Count - 1;
                            highestDamage = en.Value;
                        }

                    }
                }
            }

            // Scan for nearby targets
            foreach (var instance in MapController.GetSurroundingMapInstances(MapId, MapInstanceId, true))
            {
                foreach (var entity in instance.GetCachedEntities())
                {
                    if (entity != null && !entity.IsDead() && entity != this && entity.Id != avoidId)
                    {
                        //TODO Check if NPC is allowed to attack player with new conditions
                        if (entity is Player player)
                        {
                            // Are we aggressive towards this player or have they hit us?
                            if (ShouldAttackPlayerOnSight(player) || (DamageMap.ContainsKey(entity) && entity.MapInstanceId == MapInstanceId))
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
                        else if (entity is Npc npc)
                        {
                            if (Base.Aggressive && Base.AggroList.Contains(npc.Base.Id))
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

            // Assign our target if we've found one!
            if (Base.FocusHighestDamageDealer && highestDmgIndex != -1)
            {
                // We're focussed on whoever has the most threat! o7
                AssignTarget(possibleTargets[highestDmgIndex]);
            }
            else if (Target != null && possibleTargets.Count > 0)
            {
                // Time to randomize who we target.. Since we don't actively care who we attack!
                // 10% chance to just go for someone else.
                if (Randomization.Next(1, 101) > 90)
                {
                    if (possibleTargets.Count > 1)
                    {
                        var target = Randomization.Next(0, possibleTargets.Count - 1);
                        AssignTarget(possibleTargets[target]);
                    }
                    else
                    {
                        AssignTarget(possibleTargets[0]);
                    }
                }
            }
            else if (Target == null && Base.Aggressive && closestIndex != -1)
            {
                // Aggressively attack closest person!
                AssignTarget(possibleTargets[closestIndex]);
            }
            else if (possibleTargets.Count > 0)
            {
                // Not aggressive but no target, so just try and attack SOMEONE on the damage table!
                if (possibleTargets.Count > 1)
                {
                    var target = Randomization.Next(0, possibleTargets.Count - 1);
                    AssignTarget(possibleTargets[target]);
                }
                else
                {
                    AssignTarget(possibleTargets[0]);
                }
            }
            else
            {
                // ??? What the frick is going on here?
                // We can't find a valid target somehow, keep it up a few times and reset if this keeps failing!
                mTargetFailCounter += 1;
                if (mTargetFailCounter > mTargetFailMax)
                {
                    CheckForResetLocation(true);
                }
            }

            FindTargetWaitTime = timeMs + FindTargetDelay;
        }

        public override void ProcessRegen()
        {
            if (Base == null)
            {
                return;
            }

            foreach (Vital vital in Enum.GetValues(typeof(Vital)))
            {
                if (!Enum.IsDefined(vital))
                {
                    continue;
                }

                var vitalId = (int)vital;
                var vitalValue = GetVital(vital);
                var maxVitalValue = GetMaxVital(vital);
                if (vitalValue >= maxVitalValue)
                {
                    continue;
                }

                var vitalRegenRate = Base.VitalRegen[vitalId] / 100f;
                var regenValue = (int)Math.Max(1, maxVitalValue * vitalRegenRate) *
                                 Math.Abs(Math.Sign(vitalRegenRate));

                AddVital(vital, regenValue);
            }
        }

        public override void Warp(Guid newMapId,
            float newX,
            float newY,
            Direction newDir,
            bool adminWarp = false,
            int zOverride = 0,
            bool mapSave = false,
            bool fromWarpEvent = false,
            MapInstanceType? mapInstanceType = null,
            bool fromLogin = false,
            bool forceInstanceChange = false)
        {
            if (!MapController.TryGetInstanceFromMap(newMapId, MapInstanceId, out var map))
            {
                return;
            }

            X = (int)newX;
            Y = (int)newY;
            Z = zOverride;
            Dir = newDir;
            if (newMapId != MapId)
            {
                if (MapController.TryGetInstanceFromMap(MapId, MapInstanceId, out var oldMap))
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
                PacketSender.SendEntityStats(this);
            }
        }

        /// <summary>
        /// Determines the aggression of this NPC towards a player.
        /// </summary>
        /// <param name="player">The player to check the relationship with.</param>
        /// <returns>The NPC's aggression towards the player.</returns>
        public NpcAggression GetAggression(Player player)
        {
            if (this.Target != null)
            {
                return NpcAggression.Aggressive;
            }

            var ally = IsAllyOf(player);
            var attackOnSight = ShouldAttackPlayerOnSight(player);
            var canPlayerAttack = CanPlayerAttack(player);

            if (ally && !canPlayerAttack)
            {
                return NpcAggression.Guard;
            }

            if (attackOnSight)
            {
                return NpcAggression.AttackOnSight;
            }

            if (!ally && !attackOnSight && canPlayerAttack)
            {
                return NpcAggression.AttackWhenAttacked;
            }

            if (!ally && !attackOnSight && !canPlayerAttack)
            {
                return NpcAggression.Neutral;
            }

            return NpcAggression.Neutral;
        }

        public override EntityPacket EntityPacket(EntityPacket packet = null, Player forPlayer = null)
        {
            if (packet == null)
            {
                packet = new NpcEntityPacket();
            }

            packet = base.EntityPacket(packet, forPlayer);

            var pkt = (NpcEntityPacket)packet;
            pkt.Aggression = GetAggression(forPlayer);

            return pkt;
        }

    }

}
