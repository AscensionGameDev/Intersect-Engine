namespace Intersect.GameObjects.Events.Commands;

public partial class HidePlayerCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.HidePlayer;
}