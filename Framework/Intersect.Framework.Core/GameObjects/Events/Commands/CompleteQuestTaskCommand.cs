namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class CompleteQuestTaskCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.CompleteQuestTask;

    public Guid QuestId { get; set; }

    public Guid TaskId { get; set; }
}