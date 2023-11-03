namespace Intersect.Enums;

public enum Direction
{
    None = -1,

    Up,

    Down,

    Left,

    Right,

    UpLeft,

    UpRight,

    DownRight,

    DownLeft,
}

public static class DirectionExtensions
{
    public static Direction GetOpposite(this Direction direction) =>
        direction switch
        {
            Direction.None => Direction.None,
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            Direction.UpLeft => Direction.DownRight,
            Direction.UpRight => Direction.DownLeft,
            Direction.DownRight => Direction.UpLeft,
            Direction.DownLeft => Direction.UpRight,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, $"Invalid {nameof(Direction)}"),
        };

    public static bool IsOppositeOf(this Direction direction, Direction otherDirection) =>
        otherDirection == direction.GetOpposite();
}