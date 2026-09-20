namespace Intersect.Framework.Core.GameObjects.Events.Commands;

/// <summary>
/// Marks the dungeon the player is currently in as completed: rewards everyone and sends them back out.
/// </summary>
public partial class CompleteDungeonCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.CompleteDungeon;
}
