namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class EndQuestCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.EndQuest;

    public Guid QuestId { get; set; }

    public bool SkipCompletionEvent { get; set; }
}