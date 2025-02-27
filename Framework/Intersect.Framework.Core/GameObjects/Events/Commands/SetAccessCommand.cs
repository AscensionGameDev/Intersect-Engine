namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class SetAccessCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.SetAccess;

    public Access Access { get; set; }
}