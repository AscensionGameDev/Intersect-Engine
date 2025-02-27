namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class ExitEventProcessingCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.ExitEventProcess;
}