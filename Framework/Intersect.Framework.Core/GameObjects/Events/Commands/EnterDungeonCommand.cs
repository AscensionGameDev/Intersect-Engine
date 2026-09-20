namespace Intersect.Framework.Core.GameObjects.Events.Commands;

/// <summary>
/// Starts a dungeon run for the player (and their party/guild depending on the dungeon's access type).
/// </summary>
public partial class EnterDungeonCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.EnterDungeon;

    public Guid DungeonId { get; set; }
}
