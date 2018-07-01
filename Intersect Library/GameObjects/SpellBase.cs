using System;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.GameObjects.Conditions;
using Intersect.Models;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class SpellBase : DatabaseObject<SpellBase>
    {
        //Animations
        [Column("CastAnimation")]
        public Guid CastAnimationId { get; protected set; }
        [NotMapped]
        [JsonIgnore]
        public AnimationBase CastAnimation
        {
            get => AnimationBase.Lookup.Get<AnimationBase>(CastAnimationId);
            set => CastAnimationId = value?.Id ?? Guid.Empty;
        }

        [Column("HitAnimation")]
        public Guid HitAnimationId { get; protected set; }
        [NotMapped]
        [JsonIgnore]
        public AnimationBase HitAnimation
        {
            get => AnimationBase.Lookup.Get<AnimationBase>(HitAnimationId);
            set => HitAnimationId = value?.Id ?? Guid.Empty;
        }

        //Spell Times
        public int CastDuration { get; set; }

        //Requirements
        [Column("CastRequirements")]
        [JsonIgnore]
        public string JsonCastRequirements
        {
            get => JsonConvert.SerializeObject(CastingReqs);
            set => CastingReqs = JsonConvert.DeserializeObject<ConditionLists>(value);
        }
        [NotMapped]
        public ConditionLists CastingReqs = new ConditionLists();

        public int CastRange { get; set; }
        public int CooldownDuration { get; set; }
        public int Cost { get; set; }

        //Damage
        public int CritChance { get; set; }

        public int DamageType { get; set; } = 1;
        public int Data1 { get; set; }
        public int Data2 { get; set; }
        public int Data3 { get; set; }
        public int Data4 { get; set; }
        public Guid Guid1 { get; set; }
        public Guid Guid2 { get; set; }
        public Guid Guid3 { get; set; }
        public Guid Guid4 { get; set; }
        public string Data5 { get; set; } = "";

        public string Desc { get; set; } = "";
        public int Friendly { get; set; }
        public int HitRadius { get; set; }
        public string Pic { get; set; } = "";
        
        //Extra Data, Teleport Coords, Custom Spells, Etc
        [Column("Projectile")]
        public Guid ProjectileId { get; protected set; }
        [NotMapped]
        [JsonIgnore]
        public ProjectileBase Projectile
        {
            get => ProjectileBase.Lookup.Get<ProjectileBase>(ProjectileId);
            set => ProjectileId = value?.Id ?? Guid.Empty;
        }

        public int Scaling { get; set; } = 100;
        public int ScalingStat { get; set; }
        public byte SpellType { get; set; }
        //Targetting Stuff
        public int TargetType { get; set; }

        //Buff/Debuff Data
        [Column("StatDiff")]
        [JsonIgnore]
        public string StatDiffJson
        {
            get => DatabaseUtils.SaveIntArray(StatDiff, (int)Stats.StatCount);
            set => StatDiff = DatabaseUtils.LoadIntArray(value, (int)Stats.StatCount);
        }
        [NotMapped]
        public int[] StatDiff { get; set; } = new int[(int)Stats.StatCount];

        //Costs
        [Column("VitalCost")]
        [JsonIgnore]
        public string VitalCostJson
        {
            get => DatabaseUtils.SaveIntArray(VitalCost, (int)Vitals.VitalCount);
            set => VitalCost = DatabaseUtils.LoadIntArray(value, (int)Vitals.VitalCount);
        }
        [NotMapped]
        public int[] VitalCost = new int[(int) Vitals.VitalCount];

        //Heal/Damage
        [Column("VitalDiff")]
        [JsonIgnore]
        public string VitalDiffJson
        {
            get => DatabaseUtils.SaveIntArray(VitalDiff, (int)Vitals.VitalCount);
            set => VitalDiff = DatabaseUtils.LoadIntArray(value, (int)Vitals.VitalCount);
        }
        [NotMapped]
        public int[] VitalDiff = new int[(int)Vitals.VitalCount];

        [JsonConstructor]
        public SpellBase(Guid id) : base(id)
        {
            Name = "New Spell";
        }

        public SpellBase()
        {
            Name = "New Spell";
        }
    }
}