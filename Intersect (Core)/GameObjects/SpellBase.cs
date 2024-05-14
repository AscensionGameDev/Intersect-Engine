using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Intersect.Enums;
using Intersect.GameObjects.Conditions;
using Intersect.GameObjects.Events;
using Intersect.Models;
using Intersect.Utilities;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public partial class SpellBase : DatabaseObject<SpellBase>, IFolderable
    {
        [NotMapped]
        public long[] VitalCost = new long[Enum.GetValues<Vital>().Length];

        [JsonConstructor]
        public SpellBase(Guid id) : base(id)
        {
            Name = "New Spell";
        }

        public SpellBase()
        {
            Name = "New Spell";
        }

        public SpellType SpellType { get; set; }

        public string Description { get; set; } = "";

        public string Icon { get; set; } = "";

        //Animations
        [Column("CastAnimation")]
        public Guid CastAnimationId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public AnimationBase CastAnimation
        {
            get => AnimationBase.Get(CastAnimationId);
            set => CastAnimationId = value?.Id ?? Guid.Empty;
        }

        [Column("HitAnimation")]
        public Guid HitAnimationId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public AnimationBase HitAnimation
        {
            get => AnimationBase.Get(HitAnimationId);
            set => HitAnimationId = value?.Id ?? Guid.Empty;
        }

        [Column("TickAnimation")]
        public Guid TickAnimationId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public AnimationBase TickAnimation
        {
            get => AnimationBase.Get(TickAnimationId);
            set => TickAnimationId = value?.Id ?? Guid.Empty;
        }

        //Spell Times
        public int CastDuration { get; set; }

        public int CooldownDuration { get; set; }

        /// <summary>
        /// Defines which cooldown group this spell belongs to.
        /// </summary>
        public string CooldownGroup { get; set; } = string.Empty;

        /// <summary>
        /// Configures whether this should not trigger and be triggered by the global cooldown.
        /// </summary>
        public bool IgnoreGlobalCooldown { get; set; } = false;

        /// <summary>
        /// Configured whether the cooldown of this spell should be reduced by the players cooldown reduction
        /// </summary>
        public bool IgnoreCooldownReduction { get; set; } = false;

        //Spell Bound
        public bool Bound { get; set; }

        //Requirements
        [Column("CastRequirements")]
        [JsonIgnore]
        public string JsonCastRequirements
        {
            get => CastingRequirements.Data();
            set => CastingRequirements.Load(value);
        }

        [NotMapped]
        public ConditionLists CastingRequirements { get; set; } = new ConditionLists();

        public string CannotCastMessage { get; set; } = "";

        public string CastSpriteOverride { get; set; }

        //Combat Info
        public SpellCombatData Combat { get; set; } = new SpellCombatData();

        //Warp Info
        public SpellWarpData Warp { get; set; } = new SpellWarpData();

        //Dash Info
        public SpellDashOpts Dash { get; set; } = new SpellDashOpts();

        //Event Info
        [Column("Event")]
        public Guid EventId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public EventBase Event
        {
            get => EventBase.Get(EventId);
            set => EventId = value?.Id ?? Guid.Empty;
        }

        //Costs
        [Column("VitalCost")]
        [JsonIgnore]
        public string VitalCostJson
        {
            get => DatabaseUtils.SaveLongArray(VitalCost, Enum.GetValues<Vital>().Length);
            set => VitalCost = DatabaseUtils.LoadLongArray(value, Enum.GetValues<Vital>().Length);
        }

        /// <inheritdoc />
        public string Folder { get; set; } = "";

        /// <summary>
        /// Gets an array of all items sharing the provided cooldown group.
        /// </summary>
        /// <param name="cooldownGroup">The cooldown group to search for.</param>
        /// <returns>Returns an array of <see cref="ItemBase"/> containing all items with the supplied cooldown group.</returns>
        public static SpellBase[] GetCooldownGroup(string cooldownGroup)
        {
            cooldownGroup = cooldownGroup.Trim();

            // No point looking for nothing.
            if (string.IsNullOrWhiteSpace(cooldownGroup))
            {
                return Array.Empty<SpellBase>();
            }

            return Lookup
                .Where(i => ((SpellBase)i.Value).CooldownGroup.Trim() == cooldownGroup)
                .Select(i => (SpellBase)i.Value)
                .ToArray();
        }
    }

    [Owned]
    public partial class SpellCombatData
    {
        [NotMapped]
        public long[] VitalDiff = new long[Enum.GetValues<Vital>().Length];

        public int CritChance { get; set; }

        public double CritMultiplier { get; set; } = 1.5;

        public int DamageType { get; set; } = 1;

        public int HitRadius { get; set; }

        public bool Friendly { get; set; }

        public int CastRange { get; set; }

        //Extra Data, Teleport Coords, Custom Spells, Etc
        [Column("Projectile")]
        public Guid ProjectileId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public ProjectileBase Projectile
        {
            get => ProjectileBase.Get(ProjectileId);
            set => ProjectileId = value?.Id ?? Guid.Empty;
        }

        //Heal/Damage
        [Column("VitalDiff")]
        [JsonIgnore]
        public string VitalDiffJson
        {
            get => DatabaseUtils.SaveLongArray(VitalDiff, Enum.GetValues<Vital>().Length);
            set => VitalDiff = DatabaseUtils.LoadLongArray(value, Enum.GetValues<Vital>().Length);
        }

        //Buff/Debuff Data
        [Column("StatDiff")]
        [JsonIgnore]
        public string StatDiffJson
        {
            get => DatabaseUtils.SaveIntArray(StatDiff, Enum.GetValues<Stat>().Length);
            set => StatDiff = DatabaseUtils.LoadIntArray(value, Enum.GetValues<Stat>().Length);
        }

        [NotMapped]
        public int[] StatDiff { get; set; } = new int[Enum.GetValues<Stat>().Length];

        //Buff/Debuff Data
        [Column("PercentageStatDiff")]
        [JsonIgnore]
        public string PercentageStatDiffJson
        {
            get => DatabaseUtils.SaveIntArray(PercentageStatDiff, Enum.GetValues<Stat>().Length);
            set => PercentageStatDiff = DatabaseUtils.LoadIntArray(value, Enum.GetValues<Stat>().Length);
        }

        [NotMapped]
        public int[] PercentageStatDiff { get; set; } = new int[Enum.GetValues<Stat>().Length];

        public int Scaling { get; set; } = 0;

        public int ScalingStat { get; set; }

        public SpellTargetType TargetType { get; set; }

        public bool HoTDoT { get; set; }

        public int HotDotInterval { get; set; }

        public int Duration { get; set; }

        public SpellEffect Effect { get; set; }

        public string TransformSprite { get; set; }

        [Column("OnHit")]
        public int OnHitDuration { get; set; }

        [Column("Trap")]
        public int TrapDuration { get; set; }
    }

    [Owned]
    public partial class SpellWarpData
    {
        public Guid MapId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Dir { get; set; }
    }

    [Owned]
    public partial class SpellDashOpts
    {
        public bool IgnoreMapBlocks { get; set; }

        public bool IgnoreActiveResources { get; set; }

        public bool IgnoreInactiveResources { get; set; }

        public bool IgnoreZDimensionAttributes { get; set; }
    }
}
