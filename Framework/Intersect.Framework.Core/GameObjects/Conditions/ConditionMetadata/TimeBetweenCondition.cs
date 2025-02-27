namespace Intersect.GameObjects.Events;

public partial class TimeBetweenCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.TimeBetween;

    public int[] Ranges { get; set; } = new int[2];
}