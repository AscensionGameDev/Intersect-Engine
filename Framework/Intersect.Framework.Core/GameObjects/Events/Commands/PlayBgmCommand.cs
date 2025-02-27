namespace Intersect.GameObjects.Events.Commands;

public partial class PlayBgmCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.PlayBgm;

    public string File { get; set; } = string.Empty;
}