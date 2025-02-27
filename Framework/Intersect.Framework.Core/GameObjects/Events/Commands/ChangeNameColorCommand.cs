namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class ChangeNameColorCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.ChangeNameColor;

    public Color Color { get; set; }

    public bool Override { get; set; }

    public bool Remove { get; set; }
}