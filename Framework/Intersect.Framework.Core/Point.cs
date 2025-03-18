using System.Numerics;
using MessagePack;

namespace Intersect;

[MessagePackObject]
public partial struct Point
{
    [Key(0)]
    public int X { get; set; }

    [Key(1)]
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object obj)
    {
        if (obj is Point)
        {
            return (Point) obj == this;
        }

        return false;
    }

    public bool Equals(Point other)
    {
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }

    public override string ToString() => $"{X},{Y}";

    public static Point FromString(string val)
    {
        if (string.IsNullOrEmpty(val))
        {
            return Point.Empty;
        }

        var strs = val.Split(",".ToCharArray());
        var parts = new int[strs.Length];
        for (var i = 0; i < strs.Length; i++)
        {
            parts[i] = int.Parse(strs[i]);
        }

        return new Point(parts[0], parts[1]);
    }

    public static Point Empty => new();

    public static bool operator !=(Point left, Point right)
    {
        return left.X != right.X || left.Y != right.Y;
    }

    public static bool operator ==(Point left, Point right)
    {
        return left.X == right.X && left.Y == right.Y;
    }

    public static Point operator +(Point left, Point right) => new(left.X + right.X, left.Y + right.Y);

    public static Vector2 operator +(Point left, Vector2 right) => new(left.X + right.X, left.Y + right.Y);

    public static Vector2 operator +(Vector2 left, Point right) => new(left.X + right.X, left.Y + right.Y);

    public static Point operator -(Point left, Point right) => new(left.X - right.X, left.Y - right.Y);

    public static Point operator *(Point point, float scalar) => new((int)(point.X * scalar), (int)(point.Y * scalar));

    public static Point operator /(Point point, float scalar) => new((int)(point.X / scalar), (int)(point.Y / scalar));

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    public static implicit operator Point(Vector2 vector) => new((int)vector.X, (int)vector.Y);

    public static implicit operator Vector2(Point point) => new(point.X, point.Y);
}
