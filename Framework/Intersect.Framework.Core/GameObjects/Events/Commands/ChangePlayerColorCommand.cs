namespace Intersect.Framework.Core.GameObjects.Events.Commands;

/// <summary>
/// Defines the Event command partial class for the Change Player Color command.
/// </summary>
public partial class ChangePlayerColorCommand : EventCommand
{
    /// <summary>
    /// The <see cref="EventCommandType"/> of this command.
    /// </summary>
    public override EventCommandType Type { get; } = EventCommandType.ChangePlayerColor;

    /// <summary>
    /// The <see cref="Color"/> to apply to the player.
    /// </summary>
    public Color Color { get; set; } = new(255, 255, 255, 255);
}