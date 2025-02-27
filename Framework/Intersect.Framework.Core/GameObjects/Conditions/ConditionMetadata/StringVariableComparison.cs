using Intersect.Enums;

namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

public partial class StringVariableComparison : VariableComparison
{
    public StringVariableComparator Comparator { get; set; } = StringVariableComparator.Equal;

    public string Value { get; set; }
}