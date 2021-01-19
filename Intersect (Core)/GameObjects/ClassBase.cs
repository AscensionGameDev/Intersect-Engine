using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Enums;
using Intersect.GameObjects.Maps;
using Intersect.Models;
using Intersect.Server.Utilities;
using Intersect.Utilities;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{

    public class ClassBase : DatabaseObject<ClassBase>, IFolderable
    {

        public const long DEFAULT_BASE_EXPERIENCE = 100;

        public const long DEFAULT_EXPERIENCE_INCREASE = 50;

        [NotMapped] public int[] BaseStat = new int[(int) Stats.StatCount];

        [NotMapped] public int[] BaseVital = new int[(int) Vitals.VitalCount];

        [NotMapped] public Dictionary<int, long> ExperienceOverrides = new Dictionary<int, long>();

        [NotMapped] public List<ClassItem> Items = new List<ClassItem>();

        [JsonIgnore] private long mBaseExp;

        [JsonIgnore] private long mExpIncrease;

        [NotMapped] public List<ClassSpell> Spells = new List<ClassSpell>();

        [NotMapped] public List<ClassSprite> Sprites = new List<ClassSprite>();

        [NotMapped] public int[] StatIncrease = new int[(int) Stats.StatCount];

        [NotMapped] public int[] VitalIncrease = new int[(int) Vitals.VitalCount];

        [NotMapped] public int[] VitalRegen = new int[(int) Vitals.VitalCount];

        [JsonConstructor]
        public ClassBase(Guid id) : base(id)
        {
            Name = "New Class";

            ExperienceCurve = new ExperienceCurve();
            ExperienceCurve.Calculate(1);
            BaseExp = DEFAULT_BASE_EXPERIENCE;
            ExpIncrease = DEFAULT_EXPERIENCE_INCREASE;
        }

        //Parameterless constructor for EF
        public ClassBase()
        {
            Name = "New Class";

            ExperienceCurve = new ExperienceCurve();
            ExperienceCurve.Calculate(1);
            BaseExp = DEFAULT_BASE_EXPERIENCE;
            ExpIncrease = DEFAULT_EXPERIENCE_INCREASE;
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

        public int BasePoints { get; set; }

        public int CritChance { get; set; }

        public double CritMultiplier { get; set; } = 1.5;

        //Combat
        public int Damage { get; set; } = 1;

        public int DamageType { get; set; }

        public int AttackSpeedModifier { get; set; }

        public int AttackSpeedValue { get; set; }

        public long BaseExp
        {
            get => mBaseExp;
            set
            {
                mBaseExp = Math.Max(0, value);
                ExperienceCurve.BaseExperience = Math.Max(1, mBaseExp);
            }
        }

        public long ExpIncrease
        {
            get => mExpIncrease;
            set
            {
                mExpIncrease = Math.Max(0, value);
                ExperienceCurve.Gain = 1 + value / 100.0;
            }
        }

        //Level Up Info
        public bool IncreasePercentage { get; set; }

        [JsonIgnore]
        [NotMapped]
        public ExperienceCurve ExperienceCurve { get; }

        //Locked - Can the class be chosen from character select?
        public bool Locked { get; set; }

        public int PointIncrease { get; set; }

        public int Scaling { get; set; } = 100;

        public int ScalingStat { get; set; }

        //Spawn Info
        [Column("SpawnMap")]
        [JsonProperty]
        public Guid SpawnMapId { get; set; }

        [NotMapped]
        [JsonIgnore]
        public MapBase SpawnMap
        {
            get => MapBase.Get(SpawnMapId);
            set => SpawnMapId = value?.Id ?? Guid.Empty;
        }

        public int SpawnX { get; set; }

        public int SpawnY { get; set; }

        public int SpawnDir { get; set; }

        //Base Stats
        [Column("BaseStats")]
        [JsonIgnore]
        public string JsonBaseStats
        {
            get => DatabaseUtils.SaveIntArray(BaseStat, (int) Stats.StatCount);
            set => BaseStat = DatabaseUtils.LoadIntArray(value, (int) Stats.StatCount);
        }

        //Base Vitals
        [Column("BaseVitals")]
        [JsonIgnore]
        public string JsonBaseVitals
        {
            get => DatabaseUtils.SaveIntArray(BaseVital, (int) Vitals.VitalCount);
            set => BaseVital = DatabaseUtils.LoadIntArray(value, (int) Vitals.VitalCount);
        }

        //Starting Items
        [Column("Items")]
        [JsonIgnore]
        public string JsonItems
        {
            get => JsonConvert.SerializeObject(Items);
            protected set => Items = JsonConvert.DeserializeObject<List<ClassItem>>(value);
        }

        //Starting Spells
        [Column("Spells")]
        [JsonIgnore]
        public string JsonSpells
        {
            get => JsonConvert.SerializeObject(Spells);
            protected set => Spells = JsonConvert.DeserializeObject<List<ClassSpell>>(value);
        }

        //Sprites
        [JsonIgnore]
        [Column("Sprites")]
        public string JsonSprites
        {
            get => JsonConvert.SerializeObject(Sprites);
            protected set => Sprites = JsonConvert.DeserializeObject<List<ClassSprite>>(value);
        }

        //Stat Increases (per level)
        [JsonIgnore]
        [Column("StatIncreases")]
        public string StatIncreaseJson
        {
            get => DatabaseUtils.SaveIntArray(StatIncrease, (int) Stats.StatCount);
            set => StatIncrease = DatabaseUtils.LoadIntArray(value, (int) Stats.StatCount);
        }

        //Vital Increases (per level0
        [JsonIgnore]
        [Column("VitalIncreases")]
        public string VitalIncreaseJson
        {
            get => DatabaseUtils.SaveIntArray(VitalIncrease, (int) Vitals.VitalCount);
            set => VitalIncrease = DatabaseUtils.LoadIntArray(value, (int) Vitals.VitalCount);
        }

        //Vital Regen %
        [JsonIgnore]
        [Column("VitalRegen")]
        public string RegenJson
        {
            get => DatabaseUtils.SaveIntArray(VitalRegen, (int) Vitals.VitalCount);
            set => VitalRegen = DatabaseUtils.LoadIntArray(value, (int) Vitals.VitalCount);
        }

        [JsonIgnore]
        [Column("ExperienceOverrides")]
        public string ExpOverridesJson
        {
            get => JsonConvert.SerializeObject(ExperienceOverrides);
            set
            {
                ExperienceOverrides = JsonConvert.DeserializeObject<Dictionary<int, long>>(value ?? "");
                if (ExperienceOverrides == null)
                {
                    ExperienceOverrides = new Dictionary<int, long>();
                }
            }
        }

        /// <inheritdoc />
        public string Folder { get; set; } = "";

        public long ExperienceToNextLevel(int level)
        {
            if (ExperienceOverrides.ContainsKey(level))
            {
                return ExperienceOverrides[level];
            }

            return ExperienceCurve.Calculate(level);
        }

    }

    public class ClassItem
    {

        [JsonProperty]
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public ItemBase Get()
        {
            return ItemBase.Get(Id);
        }

    }

    public class ClassSpell
    {

        [JsonProperty]
        public Guid Id { get; set; }

        public int Level { get; set; }

        public SpellBase Get()
        {
            return SpellBase.Get(Id);
        }

    }

    public class ClassSprite
    {

        public string Face = "";

        public Gender Gender;

        public string Sprite = "";

    }

}
