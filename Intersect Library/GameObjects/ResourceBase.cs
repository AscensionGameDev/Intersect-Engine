using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.GameObjects.Conditions;
using Intersect.Models;
using Intersect.Utilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Intersect.Enums;

namespace Intersect.GameObjects
{
    [Owned]
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
        public Guid AnimationId { get; protected set; }
        [NotMapped]
        [JsonIgnore]
        public AnimationBase Animation
        {
            get => AnimationBase.Get(AnimationId);
            set => AnimationId = value?.Id ?? Guid.Empty;
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

        //Vital Regen %
        public int VitalRegen { get; set; }


        public int MaxHp { get; set; }
        public int MinHp { get; set; }
        public int SpawnDuration { get; set; }
        public int Tool { get; set; } = -1;
        public bool WalkableAfter { get; set; }
        public bool WalkableBefore { get; set; }

        [JsonConstructor]
        public ResourceBase(Guid id) : base(id)
        {
            Name = "New Resource";
            Initial = new ResourceState();
            Exhausted = new ResourceState();
        }

        //EF wants NO PARAMETERS!!!!!
        public ResourceBase()
        {
            Name = "New Resource";
            Initial = new ResourceState();
            Exhausted = new ResourceState();
        }

        public class ResourceDrop
        {
            public int Quantity;
            public double Chance;
            public Guid ItemId;
        }
    }
}