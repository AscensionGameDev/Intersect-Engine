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

    public class ItemBase : DatabaseObject<ItemBase>, IFolderable
    {

        [NotMapped] public ConditionLists UsageRequirements = new ConditionLists();

        public ItemBase()
        {
            Initialize();
        }

        [JsonConstructor]
        public ItemBase(Guid id) : base(id)
        {
            Initialize();
        }

        [Column("Animation")]
        public Guid AnimationId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public AnimationBase Animation
        {
            get => AnimationBase.Get(AnimationId);
            set => AnimationId = value?.Id ?? Guid.Empty;
        }

        [Column("AttackAnimation")]
        [JsonProperty]
        public Guid AttackAnimationId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public AnimationBase AttackAnimation
        {
            get => AnimationBase.Get(AttackAnimationId);
            set => AttackAnimationId = value?.Id ?? Guid.Empty;
        }

        [Column("EquipmentAnimation")]
        public Guid EquipmentAnimationId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public AnimationBase EquipmentAnimation
        {
            get => AnimationBase.Get(EquipmentAnimationId);
            set => EquipmentAnimationId = value?.Id ?? Guid.Empty;
        }

        public bool Bound { get; set; }

        public int CritChance { get; set; }

        public double CritMultiplier { get; set; } = 1.5;

        public int Cooldown { get; set; }

        /// <summary>
        /// Defines which cooldown group this item belongs to.
        /// </summary>
        public string CooldownGroup { get; set; } = string.Empty;

        /// <summary>
        /// Configures whether this should not trigger and be triggered by the global cooldown.
        /// </summary>
        public bool IgnoreGlobalCooldown { get; set; } = false;

        /// <summary>
        /// Configured whether the cooldown of this item should be reduced by the players cooldown reduction
        /// </summary>
        public bool IgnoreCooldownReduction { get; set; } = false;

        public int Damage { get; set; }

        public int DamageType { get; set; }

        public int AttackSpeedModifier { get; set; }

        public int AttackSpeedValue { get; set; }

        public ConsumableData Consumable { get; set; }

        public int EquipmentSlot { get; set; }

        public bool TwoHanded { get; set; }

        public EffectData Effect { get; set; }

        public int SlotCount { get; set; }

        [Column("Spell")]
        [JsonProperty]
        public Guid SpellId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public SpellBase Spell
        {
            get => SpellBase.Get(SpellId);
            set => SpellId = value?.Id ?? Guid.Empty;
        }

        public bool QuickCast { get; set; }

        [Column("DestroySpell")]
        public bool SingleUse { get; set; } = true;

        [Column("Event")]
        [JsonProperty]
        public Guid EventId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public EventBase Event
        {
            get => EventBase.Get(EventId);
            set => EventId = value?.Id ?? Guid.Empty;
        }

        public string Description { get; set; } = "";

        public string FemalePaperdoll { get; set; } = "";

        public ItemTypes ItemType { get; set; }

        public string MalePaperdoll { get; set; } = "";

        public string Icon { get; set; } = "";

        /// <summary>
        /// The database compatible version of <see cref="Color"/>
        /// </summary>
        [Column("Color")]
        [JsonIgnore]
        public string JsonColor
        {
            get => JsonConvert.SerializeObject(Color);
            set => Color = !string.IsNullOrWhiteSpace(value) ? JsonConvert.DeserializeObject<Color>(value) : Color.White;
        }

        /// <summary>
        /// Defines the ARGB color settings for this Item.
        /// </summary>
        [NotMapped]
        public Color Color { get; set; }

        public int Price { get; set; }

        public int Rarity { get; set; }

        [Column("Projectile")]
        [JsonProperty]
        public Guid ProjectileId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public ProjectileBase Projectile
        {
            get => ProjectileBase.Get(ProjectileId);
            set => ProjectileId = value?.Id ?? Guid.Empty;
        }

        public int Scaling { get; set; }

        public int ScalingStat { get; set; }

        public int Speed { get; set; }

        public bool Stackable { get; set; }

        public int StatGrowth { get; set; }

        public int Tool { get; set; } = -1;

        [Column("VitalsGiven")]
        [JsonIgnore]
        public string VitalsJson
        {
            get => DatabaseUtils.SaveIntArray(VitalsGiven, (int) Vitals.VitalCount);
            set => VitalsGiven = DatabaseUtils.LoadIntArray(value, (int) Vitals.VitalCount);
        }

        [NotMapped]
        public int[] VitalsGiven { get; set; }

        [Column("VitalsRegen")]
        [JsonIgnore]
        public string VitalsRegenJson
        {
            get => DatabaseUtils.SaveIntArray(VitalsRegen, (int) Vitals.VitalCount);
            set => VitalsRegen = DatabaseUtils.LoadIntArray(value, (int) Vitals.VitalCount);
        }

        [NotMapped]
        public int[] VitalsRegen { get; set; }

        [Column("PercentageVitalsGiven")]
        [JsonIgnore]
        public string PercentageVitalsJson
        {
            get => DatabaseUtils.SaveIntArray(PercentageVitalsGiven, (int) Vitals.VitalCount);
            set => PercentageVitalsGiven = DatabaseUtils.LoadIntArray(value, (int) Vitals.VitalCount);
        }

        [NotMapped]
        public int[] PercentageVitalsGiven { get; set; }

        [Column("StatsGiven")]
        [JsonIgnore]
        public string StatsJson
        {
            get => DatabaseUtils.SaveIntArray(StatsGiven, (int) Stats.StatCount);
            set => StatsGiven = DatabaseUtils.LoadIntArray(value, (int) Stats.StatCount);
        }

        [NotMapped]
        public int[] StatsGiven { get; set; }

        [Column("PercentageStatsGiven")]
        [JsonIgnore]
        public string PercentageStatsJson
        {
            get => DatabaseUtils.SaveIntArray(PercentageStatsGiven, (int) Stats.StatCount);
            set => PercentageStatsGiven = DatabaseUtils.LoadIntArray(value, (int) Stats.StatCount);
        }

        [NotMapped]
        public int[] PercentageStatsGiven { get; set; }

        [Column("UsageRequirements")]
        [JsonIgnore]
        public string JsonUsageRequirements
        {
            get => UsageRequirements.Data();
            set => UsageRequirements.Load(value);
        }

        [JsonIgnore, NotMapped]
        public bool IsStackable => (ItemType == ItemTypes.Currency || Stackable) &&
                                   ItemType != ItemTypes.Equipment &&
                                   ItemType != ItemTypes.Bag;

        /// <inheritdoc />
        public string Folder { get; set; } = "";

        /// <summary>
        /// Gets an array of all items sharing the provided cooldown group.
        /// </summary>
        /// <param name="cooldownGroup">The cooldown group to search for.</param>
        /// <returns>Returns an array of <see cref="ItemBase"/> containing all items with the supplied cooldown group.</returns>
        public static ItemBase[] GetCooldownGroup(string cooldownGroup)
        {
            cooldownGroup = cooldownGroup.Trim();

            // No point looking for nothing.
            if (string.IsNullOrWhiteSpace(cooldownGroup))
            {
                return Array.Empty<ItemBase>();
            }

            return Lookup.Where(i => ((ItemBase)i.Value).CooldownGroup.Trim() == cooldownGroup).Select(i => (ItemBase)i.Value).ToArray();
        }

        private void Initialize()
        {
            Name = "New Item";
            Speed = 10; // Set to 10 by default.
            StatsGiven = new int[(int) Stats.StatCount];
            PercentageStatsGiven = new int[(int) Stats.StatCount];
            VitalsGiven = new int[(int) Vitals.VitalCount];
            VitalsRegen = new int[(int) Vitals.VitalCount];
            PercentageVitalsGiven = new int[(int) Vitals.VitalCount];
            Consumable = new ConsumableData();
            Effect = new EffectData();
            Color = new Color(255, 255, 255, 255);
        }

    }

    [Owned]
    public class ConsumableData
    {

        public ConsumableType Type { get; set; }

        public int Value { get; set; }

        public int Percentage { get; set; }

    }

    [Owned]
    public class EffectData
    {

        public EffectType Type { get; set; }

        public int Percentage { get; set; }

    }

}
