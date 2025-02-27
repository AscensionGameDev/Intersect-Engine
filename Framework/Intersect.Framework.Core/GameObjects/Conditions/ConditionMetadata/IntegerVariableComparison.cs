using Intersect.Enums;

namespace Intersect.GameObjects.Events;

public partial class IntegerVariableComparison : VariableComparison
{
    public VariableComparator Comparator { get; set; } = VariableComparator.Equal;

    public VariableType CompareVariableType { get; set; } = VariableType.PlayerVariable;

    public Guid CompareVariableId { get; set; }

    public long Value { get; set; }

    public bool TimeSystem { get; set; }
}