using Intersect.Enums;

namespace Intersect.GameObjects.Events;

public partial class StringVariableComparison : VariableComparison
{
    public StringVariableComparator Comparator { get; set; } = StringVariableComparator.Equal;

    public string Value { get; set; }
}