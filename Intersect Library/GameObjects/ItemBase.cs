using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.GameObjects.Conditions;
using Intersect.Models;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public class ItemBase : DatabaseObject<ItemBase>
    {
        [Column("Animation")]
        [Required]
        [JsonProperty]
        public int AnimationId { get; protected set; }
        [NotMapped]
        [JsonIgnore]
        public AnimationBase Animation
        {
            get => AnimationBase.Lookup.Get<AnimationBase>(AnimationId);
            set => AnimationId = value?.Index ?? -1;
        }

        [Column("AttackAnimation")]
        [Required]
        [JsonProperty]
        public int AttackAnimationId { get; protected set; }
        [NotMapped]
        [JsonIgnore]
        public AnimationBase AttackAnimation
        {
            get => AnimationBase.Lookup.Get<AnimationBase>(AttackAnimationId);
            set => AttackAnimationId = value?.Index ?? -1;
        }

        public bool Bound { get; set; }
        public int CritChance { get; set; }
        public int Damage { get; set; }
        public int DamageType { get; set; }
        public int Data1 { get; set; }
        public int Data2 { get; set; }
        public int Data3 { get; set; }
        public int Data4 { get; set; }

        public string Desc { get; set; } = "";
        public string FemalePaperdoll { get; set; } = "";
        public int ItemType { get; set; }
        public string MalePaperdoll { get; set; } = "";
        public string Pic { get; set; } = "";
        public int Price { get; set; }
        public int Projectile { get; set; } = -1;
        public int Scaling { get; set; }
        public int ScalingStat { get; set; }
        public int Speed { get; set; }
        public bool Stackable { get; set; }
        public int StatGrowth { get; set; }
        public int Tool { get; set; } = -1;

        [Column("StatsGiven")]
        [JsonIgnore]
        public string StatsJson
        {
            get => DatabaseUtils.SaveIntArray(StatsGiven, (int)Stats.StatCount);
            set => StatsGiven = DatabaseUtils.LoadIntArray(value, (int)Stats.StatCount);
        }
        [NotMapped]
        public int[] StatsGiven { get; set; } = new int[(int)Stats.StatCount];


        [Column("UsageRequirements")]
        [JsonIgnore]
        public string JsonUsageRequirements
        {
            get => JsonConvert.SerializeObject(UsageRequirements);
            set => UsageRequirements = JsonConvert.DeserializeObject<ConditionLists>(value);
        }
        [NotMapped]
        public ConditionLists UsageRequirements = new ConditionLists();


        [JsonConstructor]
        public ItemBase(int index) : base(index)
        {
            Name = "New Item";
            Speed = 10; // Set to 10 by default.
            StatsGiven = new int[(int)Stats.StatCount];
        }

        public ItemBase()
        {
            Name = "New Item";
            Speed = 10; // Set to 10 by default.
            StatsGiven = new int[(int)Stats.StatCount];
        }

        public bool IsStackable()
        {
            return (ItemType == (int) ItemTypes.Currency || Stackable) && ItemType != (int)ItemTypes.Equipment && ItemType != (int)ItemTypes.Bag;
        }
    }
}