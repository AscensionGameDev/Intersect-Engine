using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.Models;
using Intersect.Server.Utilities;
using Intersect.Utilities;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class ClassBase : DatabaseObject<ClassBase>
    {
        public const long DEFAULT_BASE_EXPERIENCE = 100;
        public const long DEFAULT_EXPERIENCE_INCREASE = 50;

        public int AttackAnimation { get; set; }
        public int BasePoints { get; set; }

        public int CritChance { get; set; }

        //Combat
        public int Damage { get; set; } = 1;

        public int DamageType { get; set; }

        [JsonIgnore]
        private long mBaseExp;

        public long BaseExp
        {
            get => mBaseExp;
            set
            {
                mBaseExp = Math.Max(0, value);
                ExperienceCurve.BaseExperience = Math.Max(1, mBaseExp);
            }
        }

        [JsonIgnore]
        private long mExpIncrease;

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
        public int IncreasePercentage { get; set; }

        [JsonIgnore]
        [NotMapped]
        [NotNull]
        public ExperienceCurve ExperienceCurve { get; }

        //Locked - Can the class be chosen from character select?
        public bool Locked { get; set; }

        public int PointIncrease { get; set; }
        public int Scaling { get; set; } = 100;
        public int ScalingStat { get; set; }

        //Spawn Info
        public int SpawnMap { get; set; }
        public int SpawnX { get; set; }
        public int SpawnY { get; set; }
        public int SpawnDir { get; set; }

        //Base Stats
        [Column("BaseStats")]
        [JsonIgnore]
        public string JsonBaseStats
        {
            get => JsonConvert.SerializeObject(BaseStat);
            protected set => BaseStat = JsonConvert.DeserializeObject<int[]>(value);
        }
        [NotMapped]
        public int[] BaseStat = new int[(int)Stats.StatCount];

        //Base Vitals
        [Column("BaseVitals")]
        [JsonIgnore]
        public string JsonBaseVitals
        {
            get => JsonConvert.SerializeObject(BaseVital);
            protected set => BaseVital = JsonConvert.DeserializeObject<int[]>(value);
        }
        [NotMapped]
        public int[] BaseVital = new int[(int)Vitals.VitalCount];

        //Starting Items
        [Column("Items")]
        [JsonIgnore]
        public string JsonItems
        {
            get => JsonConvert.SerializeObject(Items);
            protected set => Items = JsonConvert.DeserializeObject<List<ClassItem>>(value);
        }
        [NotMapped]
        public List<ClassItem> Items = new List<ClassItem>();


        //Starting Spells
        [Column("Spells")]
        [JsonIgnore]
        public string JsonSpells
        {
            get => JsonConvert.SerializeObject(Spells);
            protected set => Spells = JsonConvert.DeserializeObject<List<ClassSpell>>(value);
        }
        [NotMapped]
        public List<ClassSpell> Spells = new List<ClassSpell>();

        //Sprites
        [JsonIgnore]
        [Column("Sprites")]
        public string JsonSprites
        {
            get => JsonConvert.SerializeObject(Sprites);
            protected set => Sprites = JsonConvert.DeserializeObject<List<ClassSprite>>(value);
        }
        [NotMapped]
        public List<ClassSprite> Sprites = new List<ClassSprite>();

        //Stat Increases (per level)
        [JsonIgnore]
        [Column("StatIncreases")]
        public string StatIncreaseJson
        {
            get => JsonConvert.SerializeObject(StatIncrease, Formatting.None);
            protected set => StatIncrease = JsonConvert.DeserializeObject<int[]>(value);
        }
        [NotMapped]
        public int[] StatIncrease = new int[(int) Stats.StatCount];

        //Vital Increases (per level0
        [JsonIgnore]
        [Column("VitalIncreases")]
        public string VitalIncreaseJson
        {
            get => JsonConvert.SerializeObject(VitalIncrease, Formatting.None);
            protected set => VitalIncrease = JsonConvert.DeserializeObject<int[]>(value);
        }
        [NotMapped]
        public int[] VitalIncrease = new int[(int) Vitals.VitalCount];

        //Vital Regen %
        [JsonIgnore]
        [Column("VitalRegen")]
        public string RegenJson
        {
            get => JsonConvert.SerializeObject(VitalRegen, Formatting.None);
            protected set => VitalRegen = JsonConvert.DeserializeObject<int[]>(value);
        }
        [NotMapped]
        public int[] VitalRegen = new int[(int) Vitals.VitalCount];

        [JsonConstructor]
        public ClassBase(int index) : base(index)
        {
            Name = "New Class";

            ExperienceCurve = new ExperienceCurve();
            BaseExp = DEFAULT_BASE_EXPERIENCE;
            ExpIncrease = DEFAULT_EXPERIENCE_INCREASE;
        }

        //Parameterless constructor for EF
        public ClassBase()
        {
            Name = "New Class";

            ExperienceCurve = new ExperienceCurve();
            BaseExp = DEFAULT_BASE_EXPERIENCE;
            ExpIncrease = DEFAULT_EXPERIENCE_INCREASE;
        }

        [Pure]
        public long ExperienceToNextLevel(int level)
            => ExperienceCurve.Calculate(level);
    }

    public class ClassItem
    {
        [JsonProperty]
        public int Item { get; set; }
        public int Amount { get; set; }
        
        public ItemBase Get()
        {
            return ItemBase.Lookup.Get<ItemBase>(Item);
        }
    }

    public class ClassSpell
    {
        [JsonProperty]
        public int Spell { get; set; }
        public int Level { get; set; }

        public SpellBase Get()
        {
            return SpellBase.Lookup.Get<SpellBase>(Spell);
        }
    }

    public class ClassSprite
    {
        public string Face = "";
        public byte Gender;
        public string Sprite = "";
    }
}