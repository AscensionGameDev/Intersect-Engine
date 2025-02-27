namespace Intersect.Framework.Core.GameObjects.Events.Commands;

public partial class DespawnNpcCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.DespawnNpc;

    //No parameters, only despawns npcs that have been spawned via events for the player
}