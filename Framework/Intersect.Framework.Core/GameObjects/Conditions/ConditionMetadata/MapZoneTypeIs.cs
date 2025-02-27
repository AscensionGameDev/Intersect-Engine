using Intersect.Enums;

namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

/// <summary>
/// Defines the condition class used when checking whether a player is on a specific map zone type.
/// </summary>
public partial class MapZoneTypeIs : Condition
{
    /// <summary>
    /// Defines the type of condition.
    /// </summary>
    public override ConditionType Type { get; } = ConditionType.MapZoneTypeIs;

    /// <summary>
    /// Defines the map Zone Type to compare to.
    /// </summary>
    public MapZone ZoneType { get; set; }
}