namespace Intersect.GameObjects.Events.Commands;

public partial class ShowTextCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.ShowText;

    public string Text { get; set; } = string.Empty;

    public string Face { get; set; } = string.Empty;
}