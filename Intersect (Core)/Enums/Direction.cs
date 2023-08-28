using System;

namespace Intersect.Enums
{
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
        public static Direction GetOpposite(this Direction direction)
        {
            switch (direction)
            {
                case Direction.None: return Direction.None;
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
                case Direction.UpLeft: return Direction.DownRight;
                case Direction.UpRight: return Direction.DownLeft;
                case Direction.DownRight: return Direction.UpLeft;
                case Direction.DownLeft: return Direction.UpRight;
                default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public static bool IsOppositeOf(this Direction direction, Direction otherDirection) =>
            otherDirection == direction.GetOpposite();
    }
}
