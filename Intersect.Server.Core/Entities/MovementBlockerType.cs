namespace Intersect.Server.Entities;

public enum MovementBlockerType
{
    NotBlocked = 0,
    OutOfBounds,
    MapAttribute,
    Slide,
    Entity,
    ZDimension,
}