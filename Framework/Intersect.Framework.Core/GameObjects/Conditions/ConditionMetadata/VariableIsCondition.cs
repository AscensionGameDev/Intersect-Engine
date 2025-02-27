using Intersect.Enums;
using Intersect.Framework.Core.GameObjects.Events;

namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

public partial class VariableIsCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.VariableIs;

    public VariableType VariableType { get; set; } = VariableType.PlayerVariable;

    public Guid VariableId { get; set; }

    public VariableComparison Comparison { get; set; } = new();
}