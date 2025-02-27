using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Conditions;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Models;
using Intersect.Utilities;
using Newtonsoft.Json;

namespace Intersect.GameObjects;

public partial class SpellDescriptor : DatabaseObject<SpellDescriptor>, IFolderable
{
    private long[] _vitalCost = new long[Enum.GetValues<Vital>().Length];

    [NotMapped]
    public long[] VitalCost
    {
        get => _vitalCost;
        set => _vitalCost = value;
    }

    [JsonConstructor]
    public SpellDescriptor(Guid id) : base(id)
    {
        Name = "New Spell";
    }

    public SpellDescriptor()
    {
        Name = "New Spell";
    }

    public SpellType SpellType { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    //Animations
    [Column("CastAnimation")]
    public Guid CastAnimationId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public AnimationDescriptor CastAnimation
    {
        get => AnimationDescriptor.Get(CastAnimationId);
        set => CastAnimationId = value?.Id ?? Guid.Empty;
    }

    [Column("HitAnimation")]
    public Guid HitAnimationId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public AnimationDescriptor HitAnimation
    {
        get => AnimationDescriptor.Get(HitAnimationId);
        set => HitAnimationId = value?.Id ?? Guid.Empty;
    }

    [Column("TickAnimation")]
    public Guid TickAnimationId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public AnimationDescriptor TickAnimation
    {
        get => AnimationDescriptor.Get(TickAnimationId);
        set => TickAnimationId = value?.Id ?? Guid.Empty;
    }

    //Spell Times
    public int CastDuration { get; set; }

    public int CooldownDuration { get; set; }

    /// <summary>
    /// Defines which cooldown group this spell belongs to.
    /// </summary>
    public string CooldownGroup { get; set; } = string.Empty;

    /// <summary>
    /// Configures whether this should not trigger and be triggered by the global cooldown.
    /// </summary>
    public bool IgnoreGlobalCooldown { get; set; } = false;

    /// <summary>
    /// Configured whether the cooldown of this spell should be reduced by the players cooldown reduction
    /// </summary>
    public bool IgnoreCooldownReduction { get; set; } = false;

    //Spell Bound
    public bool Bound { get; set; }

    //Requirements
    [Column("CastRequirements")]
    [JsonIgnore]
    public string JsonCastRequirements
    {
        get => CastingRequirements.Data();
        set => CastingRequirements.Load(value);
    }

    [NotMapped]
    public ConditionLists CastingRequirements { get; set; } = new();

    public string CannotCastMessage { get; set; } = string.Empty;

    public string CastSpriteOverride { get; set; }

    //Combat Info
    public SpellCombatDescriptor Combat { get; set; } = new();

    //Warp Info
    public SpellWarpDescriptor Warp { get; set; } = new();

    //Dash Info
    public SpellDashDescriptor Dash { get; set; } = new();

    //Event Info
    [Column("Event")]
    public Guid EventId { get; set; }

    [NotMapped]
    [JsonIgnore]
    public EventDescriptor Event
    {
        get => EventDescriptor.Get(EventId);
        set => EventId = value?.Id ?? Guid.Empty;
    }

    //Costs
    [Column("VitalCost")]
    [JsonIgnore]
    public string VitalCostJson
    {
        get => DatabaseUtils.SaveLongArray(_vitalCost, Enum.GetValues<Vital>().Length);
        set => DatabaseUtils.LoadLongArray(ref _vitalCost, value, Enum.GetValues<Vital>().Length);
    }

    /// <inheritdoc />
    public string Folder { get; set; } = string.Empty;

    /// <summary>
    /// Gets an array of all items sharing the provided cooldown group.
    /// </summary>
    /// <param name="cooldownGroup">The cooldown group to search for.</param>
    /// <returns>Returns an array of <see cref="ItemDescriptor"/> containing all items with the supplied cooldown group.</returns>
    public static SpellDescriptor[] GetCooldownGroup(string cooldownGroup)
    {
        cooldownGroup = cooldownGroup.Trim();

        // No point looking for nothing.
        if (string.IsNullOrWhiteSpace(cooldownGroup))
        {
            return [];
        }

        return Lookup
            .Where(i => ((SpellDescriptor)i.Value).CooldownGroup.Trim() == cooldownGroup)
            .Select(i => (SpellDescriptor)i.Value)
            .ToArray();
    }
}