namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class ShowPictureCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.ShowPicture;

    /// <summary>
    /// Picture filename to show.
    /// </summary>
    public string File { get; set; } = string.Empty;

    /// <summary>
    /// How the picture is rendered on the screen.
    /// </summary>
    public int Size { get; set; } //Original = 0, Full Screen, Half Screen, Stretch To Fit  //TODO Enum this?

    /// <summary>
    /// If true the picture will close upon being clicked
    /// </summary>
    public bool Clickable { get; set; }

    /// <summary>
    /// If not 0 the picture will go away after shown for the time below
    /// </summary>
    public int HideTime { get; set; } = 0;

    /// <summary>
    /// If true this event won't continue with commands until this picture is closed.
    /// </summary>
    public bool WaitUntilClosed { get; set; }
}