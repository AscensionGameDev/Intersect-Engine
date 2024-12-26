using System.Diagnostics;

namespace Intersect.Enums;

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

    public static Point GetDeltaPoint(this Direction direction) =>
        direction switch
        {
            Direction.None => default,
            Direction.Down => new Point(0, 1),
            Direction.DownRight => new Point(1, 1),
            Direction.Right => new Point(1, 0),
            Direction.UpRight => new Point(1, -1),
            Direction.Up => new Point(0, -1),
            Direction.UpLeft => new Point(-1, -1),
            Direction.Left => new Point(-1, 0),
            Direction.DownLeft => new Point(-1, 1),
            _ => throw new UnreachableException($"Undefined direction: {direction} ({(int)direction})"),
        };

    public static bool IsDiagonal(this Direction direction) =>
        direction switch
        {
            Direction.DownLeft => true,
            Direction.DownRight => true,
            Direction.UpLeft => true,
            Direction.UpRight => true,
            _ => false,
        };

    public static Direction[] GetComponentDirections(this Direction direction) =>
        direction switch
        {
            Direction.DownRight => new[]{ Direction.Down, Direction.Right},
            Direction.DownLeft => new[]{ Direction.Down, Direction.Left},
            Direction.UpLeft => new[]{ Direction.Up, Direction.Left},
            Direction.UpRight => new[]{ Direction.Up, Direction.Right},
            _ => new [] { direction },
        };

    public static bool IsAdjacent(this Direction direction, Direction otherDirection) =>
        direction switch
        {
            Direction.None => false,
            Direction.Up => otherDirection switch
            {
                Direction.UpLeft or Direction.UpRight => true,
                _ => false,
            },
            Direction.Down => otherDirection switch
            {
                Direction.DownLeft or Direction.DownRight => true,
                _ => false,
            },
            Direction.Left => otherDirection switch
            {
                Direction.UpLeft or Direction.DownLeft => true,
                _ => false,
            },
            Direction.Right => otherDirection switch
            {
                Direction.DownRight or Direction.UpRight => true,
                _ => false,
            },
            Direction.UpLeft => otherDirection switch
            {
                Direction.Left or Direction.Up => true,
                _ => false,
            },
            Direction.UpRight => otherDirection switch
            {
                Direction.Up or Direction.Right => true,
                _ => false,
            },
            Direction.DownRight => otherDirection switch
            {
                Direction.Right or Direction.Down => true,
                _ => false,
            },
            Direction.DownLeft => otherDirection switch
            {
                Direction.Left or Direction.Down => true,
                _ => false,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
}