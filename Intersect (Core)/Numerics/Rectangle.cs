using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Intersect.Numerics;

[DataContract]
[DebuggerDisplay("{DebugDisplayString,nq}")]
public struct Rectangle : IEquatable<Rectangle>
{
    [DataMember]
    public int X;

    [DataMember]
    public int Y;

    [DataMember]
    public int Width;

    [DataMember]
    public int Height;

    public int Left => X;

    public int Right => X + Width;

    public int Top => Y;

    public int Bottom => Y + Height;

    public Rectangle(int width, int height)
        : this(0, 0, width, height)
    {
    }

    public Rectangle(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public Rectangle(int x, int y, Point size)
        : this(x, y, size.X, size.Y)
    {
    }

    public Rectangle(Point position, int width, int height)
        : this(position.X, position.Y, width, height)
    {
    }

    public Rectangle(Point position, Point size)
        : this(position.X, position.Y, size.X, size.Y)
    {
    }

    /// <summary>
    /// Checks if the point represented by <paramref name="x"/> and <paramref name="y"/> lies within this <see cref="Rectangle"/>.
    /// </summary>
    /// <param name="x">the X coordinate of the point</param>
    /// <param name="y">the Y coordinate of the point</param>
    /// <returns></returns>
    public bool Contains(int x, int y) => x >= Left && x < Right && y >= Top && y < Bottom;

    /// <summary>
    /// Checks if the given <see cref="Point"/> lies within this <see cref="Rectangle"/>.
    /// </summary>
    /// <param name="point">a point to check if it is contained</param>
    /// <returns></returns>
    public bool Contains(Point point) => Contains(point.X, point.Y);

    public override bool Equals([NotNullWhen(true)] object obj) => obj is Rectangle rectangle && Equals(rectangle);

    public bool Equals(Rectangle rectangle) => X == rectangle.X && Y == rectangle.Y && Width == rectangle.Width && Height == rectangle.Height;

    public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

    public Rectangle Intersect(Rectangle rectangle)
    {
        if (!IntersectsExclusive(rectangle))
        {
            return default;
        }

        return FromLtrb(
            Math.Max(Left, rectangle.Left),
            Math.Max(Top, rectangle.Top),
            Math.Min(Right, rectangle.Right),
            Math.Min(Bottom, rectangle.Bottom)
        );
    }

    public bool Intersects(Rectangle rectangle) =>
        !(Left >= rectangle.Right || Right <= rectangle.Left || Top >= rectangle.Bottom || Bottom <= rectangle.Top);

    public bool IntersectsExclusive(Rectangle rectangle) =>
        !(Left > rectangle.Right || Right < rectangle.Left || Top > rectangle.Bottom || Bottom < rectangle.Top);

    public override string ToString() => $"{{ {X}, {Y}, {Width}, {Height} }}";

    public static Rectangle FromLtrb(int left, int top, int right, int bottom) => new(left, top, right - left, bottom - top);

    public static Rectangle FromString(string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return default;
        }

        var parts = str.Split(',').Select(int.Parse).ToArray();
        return new(parts[0], parts[1], parts[2], parts[3]);
    }

    public static bool operator ==(Rectangle left, Rectangle right) => left.Equals(right);

    public static bool operator !=(Rectangle left, Rectangle right) => !left.Equals(right);

    public static Rectangle operator +(Rectangle left, Rectangle right) => new(left.X + right.X, left.Y + right.Y, left.Width + right.Width, left.Height + right.Height);

    public static Rectangle operator -(Rectangle left, Rectangle right) => new(left.X + right.X, left.Y + right.Y, left.Width + right.Width, left.Height + right.Height);

    public static Rectangle operator *(Rectangle rectangle, float scalar) => new((int)(rectangle.X * scalar), (int)(rectangle.Y * scalar), (int)(rectangle.Width * scalar), (int)(rectangle.Height * scalar));

    public static Rectangle operator /(Rectangle rectangle, float scalar) => new((int)(rectangle.X / scalar), (int)(rectangle.Y / scalar), (int)(rectangle.Width / scalar), (int)(rectangle.Height / scalar));
}
