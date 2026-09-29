namespace Intersect.Enums;

public enum NpcMovement
{
    MoveRandomly = 0,

    TurnRandomly,

    StandStill,

    Static,

    /// <summary>Wanders within the NPC's configured patrol radius of its spawn point when it has no target.</summary>
    Patrol,
}
