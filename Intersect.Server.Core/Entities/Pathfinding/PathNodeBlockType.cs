namespace Intersect.Server.Entities.Pathfinding;

public enum PathNodeBlockType
{
    Nonblocking,
    InvalidTile,
    OutOfRange,
    AttributeBlock,
    AttributeNpcAvoid,
    Npc,
    Player,
    Entity,
}