namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class StopSoundsCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.StopSounds;
}