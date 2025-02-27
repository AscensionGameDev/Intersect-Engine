namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class ChangePlayerLabelCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.PlayerLabel;

    public string Value { get; set; }

    public int Position { get; set; } //0 = Above Player Name, 1 = Below Player Name

    public Color Color { get; set; }

    public bool MatchNameColor { get; set; }
}