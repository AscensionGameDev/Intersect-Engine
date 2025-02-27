using Intersect.Enums;

namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

public partial class AccessIsCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.AccessIs;

    public Access Access { get; set; }
}