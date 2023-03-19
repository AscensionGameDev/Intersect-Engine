namespace Intersect.Enums
{
    /// <summary>
    /// This enumeration represents the content of in-game tiles.
    /// The negative values indicate tiles that have specific attributes, while the positive values indicate
    /// tiles that contain certain entities.
    /// </summary>
    public enum TileType
    {
        OutOfBounds = -4,

        Slide = -3,

        ZDimension = -2,

        Block = -1,

        Clear = 0,

        WithPlayer = 1,

        WithResource = 2,

        WithEvent = 3
    }
}
