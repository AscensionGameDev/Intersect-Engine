using Intersect.Enums;

namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

public partial class LevelOrStatCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.LevelOrStat;

    public bool ComparingLevel { get; set; }

    public Stat Stat { get; set; }

    public VariableComparator Comparator { get; set; } = VariableComparator.Equal;

    public int Value { get; set; }

    public bool IgnoreBuffs { get; set; }
}