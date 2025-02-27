namespace Intersect.GameObjects.Events;

public partial class QuestInProgressCondition : Condition
{
    public override ConditionType Type { get; } = ConditionType.QuestInProgress;

    public Guid QuestId { get; set; }

    public QuestProgressState Progress { get; set; } = QuestProgressState.OnAnyTask;

    public Guid TaskId { get; set; }
}