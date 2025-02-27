namespace Intersect.GameObjects.Events;

public partial class ClassIsCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.ClassIs;

    public Guid ClassId { get; set; }
}