using Intersect.Enums;

namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

public partial class BooleanVariableComparison : VariableComparison
{
    public VariableType CompareVariableType { get; set; } = VariableType.PlayerVariable;

    public Guid CompareVariableId { get; set; }

    public bool ComparingEqual { get; set; }

    public bool Value { get; set; }
}