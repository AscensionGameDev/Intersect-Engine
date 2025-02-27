using Intersect.Enums;

namespace Intersect.GameObjects.Events.Commands;

public partial class SetAccessCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.SetAccess;

    public Access Access { get; set; }
}