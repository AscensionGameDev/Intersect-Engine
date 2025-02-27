namespace Intersect.GameObjects.Events.Commands;

public partial class ChangeSpriteCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.ChangeSprite;

    public string Sprite { get; set; } = string.Empty;
}