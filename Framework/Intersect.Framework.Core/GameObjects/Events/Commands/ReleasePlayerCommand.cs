namespace Intersect.GameObjects.Events.Commands;

public partial class ReleasePlayerCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.ReleasePlayer;
}