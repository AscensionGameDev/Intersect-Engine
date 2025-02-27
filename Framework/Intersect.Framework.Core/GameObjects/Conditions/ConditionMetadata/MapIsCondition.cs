namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

public partial class MapIsCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.MapIs;

    public Guid MapId { get; set; }
}