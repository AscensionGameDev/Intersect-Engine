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
    }

    //EF wants NO PARAMETERS!!!!!
    public ResourceDescriptor()
    {
        Name = "New Resource";
    }

    public bool UseExplicitMaxHealthForResourceStates { get; set; }

    [NotMapped, JsonIgnore]
    public Dictionary<Guid, ResourceStateDescriptor> States { get; set; } = [];

    [Column(nameof(States))]
    public string JsonStates
    {
        get => JsonConvert.SerializeObject(States);
        set => States = JsonConvert.DeserializeObject<Dictionary<Guid, ResourceStateDescriptor>>(value);
    }

    [Column(nameof(DeathAnimation))]
    public Guid DeathAnimationId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public AnimationDescriptor DeathAnimation
    {
        get => AnimationDescriptor.Get(DeathAnimationId);
        set => DeathAnimationId = value?.Id ?? Guid.Empty;
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
