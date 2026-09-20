namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

/// <summary>
/// Passes when the player is currently inside a dungeon run: a specific dungeon, or any dungeon when
/// <see cref="DungeonId"/> is <see cref="Guid.Empty"/>.
/// </summary>
public partial class InDungeonCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.InDungeon;

    public Guid DungeonId { get; set; } = Guid.Empty;
}
