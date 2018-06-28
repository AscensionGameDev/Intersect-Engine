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

        [Required]
        [JsonProperty]
        private Guid AnimationId { get; set; } //This gets stored in the EF database, and serialized to json whenever transferring to client or queried via the api or whatever..

        [NotMapped]
        [JsonIgnore]
        public AnimationBase Animation //This is what we use via code to access the animation easily (ie Item.Animation = whatever), this is easily readable. This property modifies
        //the AnimationId but doesn't actually get stored or sent.
        {
            get => AnimationBase.Lookup.Get<AnimationBase>(AnimationId);
            set => AnimationId = value?.Id ?? Guid.Empty;
        }

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
        public string StatsJson
        {
            get => DatabaseUtils.SaveIntArray(StatsGiven, (int)Stats.StatCount);
            set => StatsGiven = DatabaseUtils.LoadIntArray(value, (int)Stats.StatCount);
        }

        [NotMapped]
        public int[] StatsGiven { get; set; } = new int[(int)Stats.StatCount];

        [Column("UsageRequirements")]
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

        public bool IsStackable()
        {
            return (ItemType == (int) ItemTypes.Currency || Stackable) && ItemType != (int)ItemTypes.Equipment && ItemType != (int)ItemTypes.Bag;
        }
    }
}