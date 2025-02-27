namespace Intersect.GameObjects.Events.Commands;

public partial class GoToLabelCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.GoToLabel;

    public string Label { get; set; }
}