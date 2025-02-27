namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class SetClassCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.SetClass;

    public Guid ClassId { get; set; }
}