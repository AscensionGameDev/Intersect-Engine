using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

using Intersect.Numerics;

namespace Intersect.Client.Framework.Graphics;

using Point = Numerics.Point;

[DataContract]
[DebuggerDisplay("{DebugDisplayString,nq}")]
public struct Viewport : IEquatable<Viewport>
{
    [DataMember]
    public readonly int X;

    [DataMember]
    public readonly int Y;

    [DataMember]
    public readonly int Width;

    [DataMember]
    public readonly int Height;

    [DataMember]
    public readonly float MinimumDepth;

    [DataMember]
    public readonly float MaximumDepth;

    public Viewport(int width, int height)
        : this(0, 0, width, height)
    {
    }

    public Viewport(Point size)
        : this(size.X, size.Y)
    {
    }

    public Viewport(int x, int y, int width, int height)
        : this(x, y, width, height, 0f, 1f)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public Viewport(int x, int y, Point size)
        : this(x, y, size.X, size.Y)
    {
    }

    public Viewport(Point position, Point size)
        : this(position.X, position.Y, size.X, size.Y)
    {
    }

    public Viewport(Rectangle rectangle)
        : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
    {
    }

    public Viewport(int x, int y, int width, int height, float mininumDepth, float maximumDepth)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        MinimumDepth = mininumDepth;
        MaximumDepth = maximumDepth;
    }

    public float AspectRatio => Width / (float)Height;

    public Rectangle Bounds => new(Position, Size);

    public Point Position => new(X, Y);

    public Point Size => new(Width, Height);

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Viewport viewport && Equals(viewport);

    public bool Equals(Viewport viewport) => X == viewport.X && Y == viewport.Y && Width == viewport.Width && Height == viewport.Height && Math.Abs(MinimumDepth - viewport.MinimumDepth) < 0.001 && Math.Abs(MaximumDepth - viewport.MaximumDepth) < 0.001;

    public override int GetHashCode() =>
        HashCode.Combine(X, Y, Width, Height, MinimumDepth, MaximumDepth);

    public override string ToString() =>
        $"{{ X={X}, Y={Y}, Width={Width}, Height={Height}, MinimumDepth={MinimumDepth}, MaximumDepth={MaximumDepth} }}";
}
