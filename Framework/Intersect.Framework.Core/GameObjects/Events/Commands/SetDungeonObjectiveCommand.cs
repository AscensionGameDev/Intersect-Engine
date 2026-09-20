namespace Intersect.Framework.Core.GameObjects.Events.Commands;

/// <summary>
/// Changes the objective line shown in the dungeon HUD of everyone in the player's current dungeon.
/// </summary>
public partial class SetDungeonObjectiveCommand : EventCommand
{
    public override EventCommandType Type { get; } = EventCommandType.SetDungeonObjective;

    public string Objective { get; set; } = string.Empty;
}
