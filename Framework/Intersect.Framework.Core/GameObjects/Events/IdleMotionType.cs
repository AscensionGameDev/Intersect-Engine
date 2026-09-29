namespace Intersect.Framework.Core.GameObjects.Events;

public enum IdleMotionType
{
    None = 0,

    // Smooth vertical up/down drift (floating items, ghosts).
    Float = 1,

    // Sharp vertical bounce that always returns to the base position (balls, crops).
    Bounce = 2,

    // Rotation back and forth around the sprite's base (trees, plants swaying in the wind).
    Sway = 3,
}
