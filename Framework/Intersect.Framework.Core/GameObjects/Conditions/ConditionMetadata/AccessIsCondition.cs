using Intersect.Enums;

namespace Intersect.GameObjects.Events;

public partial class AccessIsCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.AccessIs;

    public Access Access { get; set; }
}