namespace Intersect.GameObjects.Events.Commands;

public partial class HidePictureCommmand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.HidePicture;
}