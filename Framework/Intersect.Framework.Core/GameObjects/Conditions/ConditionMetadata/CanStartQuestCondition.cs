namespace Intersect.GameObjects.Events;

public partial class CanStartQuestCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.CanStartQuest;

    public Guid QuestId { get; set; }
}