namespace Intersect.GameObjects.Events.Commands;

public partial class LabelCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.Label;

    public string Label { get; set; }
}