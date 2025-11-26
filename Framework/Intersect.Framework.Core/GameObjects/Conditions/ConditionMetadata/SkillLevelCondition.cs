using System;
using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Events;

namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

public partial class SkillLevelCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.SkillLevel;

    public Guid SkillId { get; set; }

    public VariableComparator Comparator { get; set; } = VariableComparator.Equal;

    public int Value { get; set; }
}

