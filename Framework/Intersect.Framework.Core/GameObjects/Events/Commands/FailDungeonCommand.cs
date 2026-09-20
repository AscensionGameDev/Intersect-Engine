namespace Intersect.Framework.Core.GameObjects.Events.Commands;

/// <summary>
/// Marks the dungeon the player is currently in as failed: everyone is sent back out without rewards.
/// </summary>
public partial class FailDungeonCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.FailDungeon;
}
