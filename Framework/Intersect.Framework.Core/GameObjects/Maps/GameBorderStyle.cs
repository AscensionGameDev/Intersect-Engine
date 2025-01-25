namespace Intersect.Framework.Core.GameObjects.Maps;

public enum GameBorderStyle
{
    /// <summary>
    /// No transitions between adjacent maps but the camera is locked to map boundaries if the map has adjacent maps.
    /// </summary>
    Seamless = 0,

    /// <summary>
    /// Camera is clamped to the boundaries of the current map, which causes a transition when moving to adjacent maps.
    /// </summary>
    /// <remarks>
    /// This only applies if the effective resolution (zoom shrinks this) is below the pixel dimensions of the map.
    /// </remarks>
    Seamed = 1,

    /// <summary>
    /// No transitions between adjacent maps, and the camera is not locked to map boundaries ("black" borders).
    /// </summary>
    SeamlessUnbounded = 2,
}