namespace Intersect.GameObjects.Events.Commands;

public partial class SetMoveRouteCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.SetMoveRoute;

    public EventMoveRoute Route { get; set; } = new();
}