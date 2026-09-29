namespace Intersect.Framework.Core.GameObjects.Events.Commands;

/// <summary>
/// Unlocks a mount for the player, making it available to ride from the Mounts window. Does not mount it up.
/// </summary>
public partial class UnlockMountCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.UnlockMount;

    public Guid MountId { get; set; }
}
