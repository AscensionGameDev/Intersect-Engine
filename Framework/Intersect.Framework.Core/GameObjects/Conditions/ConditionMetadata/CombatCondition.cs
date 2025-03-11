namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

public partial class CombatCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.IsInCombat;
}
