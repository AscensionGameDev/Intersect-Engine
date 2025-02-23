using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Intersect.Client.Framework.GenericClasses;

[DataContract]
[DebuggerDisplay("{DebugDisplayString,nq}")]
[StructLayout(LayoutKind.Explicit)]
public partial struct Pointf : IEquatable<Pointf>
{
    [FieldOffset(0)]
    public float X;

    [FieldOffset(sizeof(float))]
    public float Y;

    public static Pointf Empty => new();

    public static Pointf UnitX => new(1, 0);

    public static Pointf UnitY => new(0, 1);

    public static Pointf One => new(1, 1);

    private const float Tolerance = 0.001f;

    public Pointf()
    {
    }

    public Pointf(float value) : this(value, value) { }

    public Pointf(float x, float y)
    {
        X = x;
        Y = y;
    }

    public Pointf(double x, double y) : this((float)x, (float)y)
    {
    }

    public override bool Equals(object? obj) => obj is Pointf point && Equals(point);

    public bool Equals(Pointf other) => this == other;

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static implicit operator Pointf(Point point) => new(point.X, point.Y);

    public static implicit operator Point(Pointf point) => new((int)point.X, (int)point.Y);

    public static bool operator ==(Pointf left, Pointf right) => left.X.Equals(right.X) && left.Y.Equals(right.Y);

    public static bool operator !=(Pointf left, Pointf right) => !left.X.Equals(right.X) || !left.Y.Equals(right.Y);

    public static Pointf operator +(Pointf left, Pointf right) => new(left.X + right.X, left.Y + right.Y);

    public static Pointf operator -(Pointf left, Pointf right) => new(left.X - right.X, left.Y - right.Y);

    public static Pointf operator *(Pointf lhs, Pointf rhs) => new(lhs.X * rhs.X, lhs.Y * rhs.Y);

    public static Pointf operator *(Pointf point, float scalar) => new(point.X * scalar, point.Y * scalar);

    public static Pointf operator *(float scalar, Pointf point) => new(point.X * scalar, point.Y * scalar);

    public static Pointf operator /(Pointf point, float scalar) => new(point.X / scalar, point.Y / scalar);

    internal string DebugDisplayString => $"{X}, {Y}";

    public override string ToString() => $"({X}, {Y})";
}
