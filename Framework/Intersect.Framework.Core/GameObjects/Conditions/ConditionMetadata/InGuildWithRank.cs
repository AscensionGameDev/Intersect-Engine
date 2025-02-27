namespace Intersect.GameObjects.Events;

/// <summary>
/// Defines the condition class used when checking whether a player is in a guild with at least a specified rank
/// </summary>
public partial class InGuildWithRank : Condition
{
    /// <summary>
    /// Defines the type of condition
    /// </summary>
    public override ConditionType Type { get; } = ConditionType.InGuildWithRank;

    /// <summary>
    /// The guild rank the condition checks for as a minimum
    /// </summary>
    public int Rank { get; set; }
}