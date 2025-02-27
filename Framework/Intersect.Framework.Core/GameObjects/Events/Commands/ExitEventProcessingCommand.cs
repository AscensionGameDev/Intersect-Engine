namespace Intersect.GameObjects.Events.Commands;

public partial class ExitEventProcessingCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.ExitEventProcess;
}