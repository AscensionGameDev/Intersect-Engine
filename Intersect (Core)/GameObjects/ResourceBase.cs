using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.GameObjects.Conditions;
using Intersect.GameObjects.Events;
using Intersect.Models;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{

    [Owned]
    public class ResourceState
    {

        public string Graphic { get; set; } = null;

        public bool RenderBelowEntities { get; set; }

        public bool GraphicFromTileset { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

    }

    public class ResourceBase : DatabaseObject<ResourceBase>, IFolderable
    {

        [NotMapped] public List<ResourceDrop> Drops = new List<ResourceDrop>();

        [NotMapped] public ConditionLists HarvestingRequirements = new ConditionLists();

        public string CannotHarvestMessage { get; set; } = "";

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

        // Graphics
        public ResourceState Initial { get; set; }

        public ResourceState Exhausted { get; set; }

        [Column("Animation")]
        public Guid AnimationId { get; set; }

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

        //Requirements
        [Column("HarvestingRequirements")]
        [JsonIgnore]
        public string JsonHarvestingRequirements
        {
            get => HarvestingRequirements.Data();
            set => HarvestingRequirements.Load(value);
        }

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

        //Vital Regen %
        public int VitalRegen { get; set; }

        public int MaxHp { get; set; }

        public int MinHp { get; set; }

        public int SpawnDuration { get; set; }

        public int Tool { get; set; } = -1;

        public bool WalkableAfter { get; set; }

        public bool WalkableBefore { get; set; }

        /// <inheritdoc />
        public string Folder { get; set; } = "";

        public class ResourceDrop
        {

            public double Chance;

            public Guid ItemId;

            public int Quantity;

        }

    }

}
