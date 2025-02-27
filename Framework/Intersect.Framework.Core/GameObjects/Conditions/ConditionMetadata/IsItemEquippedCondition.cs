namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

public partial class IsItemEquippedCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.IsItemEquipped;

    public Guid ItemId { get; set; }
}