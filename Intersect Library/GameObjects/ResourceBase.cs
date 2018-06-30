using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.GameObjects.Conditions;
using Intersect.Models;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.GameObjects
{

    public class ResourceState
    {
        public string Graphic { get; set; } = null;
        public bool GraphicFromTileset { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }


    public class ResourceBase : DatabaseObject<ResourceBase>
    {
        // Graphics
        public ResourceState Initial { get; set; }
        public ResourceState Exhausted { get; set; }

        [Column("Animation")]
        public int AnimationId { get; protected set; }
        [NotMapped]
        [JsonIgnore]
        public AnimationBase Animation
        {
            get => AnimationBase.Lookup.Get<AnimationBase>(AnimationId);
            set => AnimationId = value?.Index ?? -1;
        }

        // Drops
        [Column("Drops")]
        [JsonIgnore]
        public string JsonDrops
        {
            get => JsonConvert.SerializeObject(Drops);
            set => Drops = JsonConvert.DeserializeObject<List<ResourceDrop>>(value);
        }
        [NotMapped]
        public List<ResourceDrop> Drops = new List<ResourceDrop>();

        //Requirements
        [Column("HarvestingRequirements")]
        [JsonIgnore]
        public string JsonHarvestingRequirements
        {
            get => JsonConvert.SerializeObject(HarvestingRequirements);
            set => HarvestingRequirements = JsonConvert.DeserializeObject<ConditionLists>(value);
        }
        [NotMapped]
        public ConditionLists HarvestingRequirements = new ConditionLists();


        public int MaxHp { get; set; }
        public int MinHp { get; set; }
        public int SpawnDuration { get; set; }
        public int Tool { get; set; } = -1;
        public bool WalkableAfter { get; set; }
        public bool WalkableBefore { get; set; }

        [JsonConstructor]
        public ResourceBase(int index) : base(index)
        {
            Name = "New Resource";
        }

        //EF wants NO PARAMETERS!!!!!
        public ResourceBase()
        {
            Name = "New Resource";
        }

        public class ResourceDrop
        {
            public int Amount;
            public double Chance;
            public int ItemNum;
        }

        public static ResourceBase Get(int index)
        {
            return ResourceBase.Lookup.Get<ResourceBase>(index);
        }
    }
}