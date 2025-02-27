namespace Intersect.GameObjects.Events;

public partial class QuestCompletedCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.QuestCompleted;

    public Guid QuestId { get; set; }
}