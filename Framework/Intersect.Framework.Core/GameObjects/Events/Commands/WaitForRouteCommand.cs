namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class WaitForRouteCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.WaitForRouteCompletion;

    public Guid TargetId { get; set; }
}