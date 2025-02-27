namespace Intersect.GameObjects.Events.Commands;

public partial class ChangeFaceCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.ChangeFace;

    public string Face { get; set; } = string.Empty;
}