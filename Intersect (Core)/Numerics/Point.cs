using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.Serialization;

namespace Intersect.Numerics;

[DataContract]
[DebuggerDisplay("{DebugDisplayString,nq}")]
public struct Point : IEquatable<Point>
{
    [DataMember]
    public int X;

    [DataMember]
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals([NotNullWhen(true)] object obj) => obj is Point point && Equals(point);

    public bool Equals(Point point) => X == point.X && Y == point.Y;

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString() => $"{{ X={X}, Y={Y} }}";

    public static Point FromString(string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return default;
        }

        var parts = str.Split(',').Select(int.Parse).ToArray();
        return new(parts[0], parts[1]);
    }

    public static bool operator ==(Point left, Point right) => left.Equals(right);

    public static bool operator !=(Point left, Point right) => !left.Equals(right);

    public static Point operator +(Point left, Point right) => new(left.X + right.X, left.Y + right.Y);

    public static Point operator -(Point left, Point right) => new(left.X + right.X, left.Y + right.Y);

    public static Point operator *(Point point, float scalar) => new((int)(point.X * scalar), (int)(point.Y * scalar));

    public static Point operator /(Point point, float scalar) => new((int)(point.X / scalar), (int)(point.Y / scalar));

    public static implicit operator Vector2(Point point) => new(point.X, point.Y);
}
