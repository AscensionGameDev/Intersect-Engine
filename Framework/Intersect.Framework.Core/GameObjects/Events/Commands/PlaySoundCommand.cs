namespace Intersect.GameObjects.Events.Commands;

public partial class PlaySoundCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.PlaySound;

    public string File { get; set; } = string.Empty;
}