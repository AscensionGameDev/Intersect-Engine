using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.Framework.Core.GameObjects.Conditions;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Models;
using Newtonsoft.Json;

namespace Intersect.Framework.Core.GameObjects.Resources;

public partial class ResourceDescriptor : DatabaseObject<ResourceDescriptor>, IFolderable
{
    [NotMapped]
    public List<Drop> Drops { get; set; } = [];

    [NotMapped]
    public ConditionLists HarvestingRequirements { get; set; } = new();

    public string CannotHarvestMessage { get; set; } = string.Empty;

    [JsonConstructor]
    public ResourceDescriptor(Guid id) : base(id)
    {
        Name = "New Resource";
        HealthGraphics = [];
    }

    //EF wants NO PARAMETERS!!!!!
    public ResourceDescriptor()
    {
        Name = "New Resource";
        HealthGraphics = [];
    }

    public bool UseExplicitMaxHealthForResourceStates { get; set; }

    [NotMapped, JsonIgnore]
    public Dictionary<string, ResourceStateDescriptor> HealthGraphics { get; set; }

    [Column("HealthGraphics")]
    public string JsonHealthGraphics
    {
        get => JsonConvert.SerializeObject(HealthGraphics);
        set => HealthGraphics = JsonConvert.DeserializeObject<Dictionary<string, ResourceStateDescriptor>>(value);
    }

    [Column("Animation")]
    public Guid AnimationId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public AnimationDescriptor Animation
    {
        get => AnimationDescriptor.Get(AnimationId);
        set => AnimationId = value?.Id ?? Guid.Empty;
    }

    // Drops
    [Column("Drops")]
    [JsonIgnore]
    public string JsonDrops
    {
        get => JsonConvert.SerializeObject(Drops);
        set => Drops = JsonConvert.DeserializeObject<List<Drop>>(value);
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
    public EventDescriptor Event
    {
        get => EventDescriptor.Get(EventId);
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
    public string Folder { get; set; } = string.Empty;
}
