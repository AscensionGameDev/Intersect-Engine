using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Intersect.Numerics;

[DataContract]
[DebuggerDisplay("{DebugDisplayString,nq}")]
public struct FloatRect : IEquatable<FloatRect>
{
    [DataMember]
    public float X;

    [DataMember]
    public float Y;

    [DataMember]
    public float Width;

    [DataMember]
    public float Height;

    public float Left => X;

    public float Right => X + Width;

    public float Top => Y;

    public float Bottom => Y + Height;

    public FloatRect(float width, float height)
        : this(0, 0, width, height)
    {
    }

    public FloatRect(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public FloatRect(float x, float y, Pointf size)
        : this(x, y, size.X, size.Y)
    {
    }

    public FloatRect(Pointf position, float width, float height)
        : this(position.X, position.Y, width, height)
    {
    }

    public FloatRect(Pointf position, Pointf size)
        : this(position.X, position.Y, size.X, size.Y)
    {
    }

    /// <summary>
    /// Checks if the point represented by <paramref name="x"/> and <paramref name="y"/> lies within this <see cref="Rectangle"/>.
    /// </summary>
    /// <param name="x">the X coordinate of the point</param>
    /// <param name="y">the Y coordinate of the point</param>
    /// <returns></returns>
    public bool Contains(float x, float y) => x >= Left && x < Right && y >= Top && y < Bottom;

    /// <summary>
    /// Checks if the given <see cref="Point"/> lies within this <see cref="Rectangle"/>.
    /// </summary>
    /// <param name="point">a point to check if it is contained</param>
    /// <returns></returns>
    public bool Contains(Point point) => Contains(point.X, point.Y);

    public override bool Equals([NotNullWhen(true)] object obj) => obj is FloatRect rectangle && Equals(rectangle);

    public bool Equals(FloatRect rectangle) => X == rectangle.X && Y == rectangle.Y && Width == rectangle.Width && Height == rectangle.Height;

    public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

    public FloatRect Intersect(FloatRect rectangle)
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

    public bool Intersects(FloatRect rectangle) =>
        !(Left >= rectangle.Right || Right <= rectangle.Left || Top >= rectangle.Bottom || Bottom <= rectangle.Top);

    public bool IntersectsExclusive(FloatRect rectangle) =>
        !(Left > rectangle.Right || Right < rectangle.Left || Top > rectangle.Bottom || Bottom < rectangle.Top);

    public override string ToString() => $"{{ {X}, {Y}, {Width}, {Height} }}";

    public static FloatRect FromLtrb(float left, float top, float right, float bottom) => new(left, top, right - left, bottom - top);

    public static FloatRect FromString(string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return default;
        }

        var parts = str.Split(',').Select(float.Parse).ToArray();
        return new(parts[0], parts[1], parts[2], parts[3]);
    }

    public static bool operator ==(FloatRect left, FloatRect right) => left.Equals(right);

    public static bool operator !=(FloatRect left, FloatRect right) => !left.Equals(right);

    public static FloatRect operator +(FloatRect left, FloatRect right) => new(left.X + right.X, left.Y + right.Y, left.Width + right.Width, left.Height + right.Height);

    public static FloatRect operator -(FloatRect left, FloatRect right) => new(left.X + right.X, left.Y + right.Y, left.Width + right.Width, left.Height + right.Height);

    public static FloatRect operator *(FloatRect rectangle, float scalar) => new((rectangle.X * scalar), (rectangle.Y * scalar), (rectangle.Width * scalar), (rectangle.Height * scalar));

    public static FloatRect operator /(FloatRect rectangle, float scalar) => new((rectangle.X / scalar), (rectangle.Y / scalar), (rectangle.Width / scalar), (rectangle.Height / scalar));

    public static implicit operator FloatRect(Rectangle rectangle) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
}
