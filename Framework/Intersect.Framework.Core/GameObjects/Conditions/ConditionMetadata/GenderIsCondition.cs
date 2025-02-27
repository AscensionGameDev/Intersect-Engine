using Intersect.Enums;

namespace Intersect.GameObjects.Events;

public partial class GenderIsCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.GenderIs;

    public Gender Gender { get; set; } = Gender.Male;
}