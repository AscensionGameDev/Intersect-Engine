using System.Runtime.InteropServices;

namespace Intersect.Client.Framework.GenericClasses;

[StructLayout(LayoutKind.Explicit)]
public partial struct Rectangle : IEquatable<Rectangle>
{
    [FieldOffset(0)] public int X;
    [FieldOffset(4)] public int Y;
    [FieldOffset(8)] public int Width;
    [FieldOffset(12)] public int Height;

    [FieldOffset(0)] public Point Position;
    [FieldOffset(8)] public Point Size;

    public int Left => X;

    public int Top => Y;

    public int Bottom => Y + Height;

    public int Right => X + Width;

    public static Rectangle Empty => new();

    public Rectangle(int x, int y, int w, int h)
    {
        X = x;
        Y = y;
        Width = w;
        Height = h;
    }

    public Rectangle(Point position, Point size)
    {
        Position = position;
        Size = size;
    }

    public Rectangle(Rectangle other)
    {
        X = other.X;
        Y = other.Y;
        Width = other.Width;
        Height = other.Height;
    }

    public static Rectangle Intersect(Rectangle a, Rectangle b)
    {
        // MS.NET returns a non-empty rectangle if the two rectangles
        // touch each other
        if (!a.IntersectsWithInclusive(b))
        {
            return Empty;
        }

        return FromLtrb(
            Math.Max(a.Left, b.Left),
            Math.Max(a.Top, b.Top),
            Math.Min(a.Right, b.Right),
            Math.Min(a.Bottom, b.Bottom)
        );
    }

    public static Rectangle FromLtrb(int left, int top, int right, int bottom)
    {
        return new Rectangle(left, top, right - left, bottom - top);
    }

    public bool IntersectsWith(Rectangle rect)
    {
        return !(Left >= rect.Right || Right <= rect.Left || Top >= rect.Bottom || Bottom <= rect.Top);
    }

    private bool IntersectsWithInclusive(Rectangle r)
    {
        return !(Left > r.Right || Right < r.Left || Top > r.Bottom || Bottom < r.Top);
    }

    /// <summary>
    ///     Contains Method
    /// </summary>
    /// <remarks>
    ///     Checks if an x,y coordinate lies within this Rectangle.
    /// </remarks>
    public bool Contains(int x, int y)
    {
        return x >= Left && x < Right && y >= Top && y < Bottom;
    }

    /// <summary>
    ///     Contains Method
    /// </summary>
    /// <remarks>
    ///     Checks if a Point lies within this Rectangle.
    /// </remarks>
    public bool Contains(Point pt)
    {
        return Contains(pt.X, pt.Y);
    }

    public override string ToString() => $"{X},{Y},{Width},{Height}";

    public static string ToString(Rectangle rect) => rect.ToString();

    public static Rectangle FromString(string rect)
    {
        if (string.IsNullOrEmpty(rect))
        {
            return Rectangle.Empty;
        }

        var strs = rect.Split(",".ToCharArray());
        var parts = new int[strs.Length];
        for (var i = 0; i < strs.Length; i++)
        {
            parts[i] = int.Parse(strs[i]);
        }

        return new Rectangle(parts[0], parts[1], parts[2], parts[3]);
    }

    public bool Equals(Rectangle other) => X == other.X &&
                                           Y == other.Y &&
                                           Width == other.Width &&
                                           Height == other.Height &&
                                           Position.Equals(other.Position) &&
                                           Size.Equals(other.Size);

    public override bool Equals(object? obj) => obj is Rectangle other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height, Position, Size);

    public static bool operator ==(Rectangle lhs, Rectangle rhs) => lhs.Equals(rhs);

    public static bool operator !=(Rectangle lhs, Rectangle rhs) => !lhs.Equals(rhs);
}
