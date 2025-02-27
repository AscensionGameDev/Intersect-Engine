namespace Intersect.GameObjects.Events.Commands;

public partial class StartCommmonEventCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.StartCommonEvent;

    public bool AllInInstance { get; set;  }

    public bool AllowInOverworld { get; set; }

    public Guid EventId { get; set; }
}