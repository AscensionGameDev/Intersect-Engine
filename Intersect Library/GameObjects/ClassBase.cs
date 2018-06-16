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

        //Starting Items
        public List<ClassItem> Items = new List<ClassItem>();

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

        //Starting Spells
        public List<ClassSpell> Spells = new List<ClassSpell>();

        //Sprites
        public List<ClassSprite> Sprites = new List<ClassSprite>();

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
        public int ItemNum;
    }

    public class ClassSpell
    {
        public int Level;
        public int SpellNum;
    }

    public class ClassSprite
    {
        public string Face = "";
        public byte Gender;
        public string Sprite = "";
    }
}