namespace Intersect.Framework.Core.GameObjects.Conditions.ConditionMetadata;

public partial class QuestCompletedCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.QuestCompleted;

    public Guid QuestId { get; set; }
}