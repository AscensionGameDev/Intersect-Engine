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

        [Required]
        [JsonProperty]
        private Guid AttackAnimationId { get; set; } //This gets stored in the EF database, and serialized to json whenever transferring to client or queried via the api or whatever..

        [NotMapped]
        [JsonIgnore]
        public AnimationBase AttackAnimation //This is what we use via code to access the animation easily (ie Item.Animation = whatever), this is easily readable. This property modifies
            //the AnimationId but doesn't actually get stored or sent.
        {
            get => AnimationBase.Lookup.Get<AnimationBase>(AttackAnimationId);
            set => AttackAnimationId = value?.Id ?? Guid.Empty;
        }

        public int BasePoints;
        public int[] BaseStat = new int[(int) Stats.StatCount];

        //Starting Vitals & Stats
        public int[] BaseVital = new int[(int) Vitals.VitalCount];

        public int CritChance;

        //Combat
        public int Damage = 1;

        public int DamageType;

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
        public int IncreasePercentage;

        [JsonIgnore]
        [NotNull]
        public ExperienceCurve ExperienceCurve { get; }

        //Locked - Can the class be chosen from character select?
        public int Locked;

        public int PointIncrease;
        public int Scaling = 100;
        public int ScalingStat;
        public int SpawnDir;

        //Spawn Info
        public int SpawnMap;

        public int SpawnX;
        public int SpawnY;

        //Starting Items
        [NotMapped]
        public List<ClassItem> Items = new List<ClassItem>();

        [Column("Items")]
        public string JsonItems
        {
            get => JsonConvert.SerializeObject(Items);
            set => Items = JsonConvert.DeserializeObject<List<ClassItem>>(value);
        }

        //Starting Spells
        [NotMapped]
        public List<ClassSpell> Spells = new List<ClassSpell>();

        [Column("Spells")]
        public string JsonSpells
        {
            get => JsonConvert.SerializeObject(Spells);
            set => Spells = JsonConvert.DeserializeObject<List<ClassSpell>>(value);
        }

        //Sprites
        [NotMapped]
        public List<ClassSprite> Sprites = new List<ClassSprite>();

        [Column("Sprites")]
        public string JsonSprites
        {
            get => JsonConvert.SerializeObject(Sprites);
            set => Sprites = JsonConvert.DeserializeObject<List<ClassSprite>>(value);
        }

        public int[] StatIncrease = new int[(int) Stats.StatCount];

        public int[] VitalIncrease = new int[(int) Vitals.VitalCount];

        //Regen Percentages
        public int[] VitalRegen = new int[(int) Vitals.VitalCount];

        [JsonConstructor]
        public ClassBase(int index) : base(index)
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
        public int Amount;

        [JsonProperty]
        public Guid ItemId { get; set; }

        [NotMapped]
        public ItemBase Item => ItemBase.Lookup.Get<ItemBase>(ItemId);

        [NotMapped]
        public int ItemNum
        {
            get => Item.Index;
            set => ItemId = ItemBase.Lookup.Get(value)?.Id ?? Guid.Empty;
        }
    }

    public class ClassSpell
    {
        public int Level;

        [JsonProperty]
        public Guid SpellId { get; set; }

        [NotMapped]
        public SpellBase Spell => SpellBase.Lookup.Get<SpellBase>(SpellId);

        [NotMapped]
        public int SpellNum
        {
            get => Spell.Index;
            set => SpellId = SpellBase.Lookup.Get(value)?.Id ?? Guid.Empty;
        }
    }

    public class ClassSprite
    {
        public string Face = "";
        public byte Gender;
        public string Sprite = "";
    }
}