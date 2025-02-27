namespace Intersect.GameObjects.Events;

public partial class MapIsCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.MapIs;

    public Guid MapId { get; set; }
}