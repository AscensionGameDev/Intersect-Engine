using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.Serialization;

namespace Intersect.Numerics;

[DataContract]
[DebuggerDisplay("{DebugDisplayString,nq}")]
public struct Pointf : IEquatable<Pointf>
{
    [DataMember]
    public float X;

    [DataMember]
    public float Y;

    public Pointf(float x, float y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals([NotNullWhen(true)] object obj) => obj is Pointf point && Equals(point);

    public bool Equals(Pointf point) => X == point.X && Y == point.Y;

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString() => $"{{ X={X}, Y={Y} }}";

    public static Pointf FromString(string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return default;
        }

        var parts = str.Split(',').Select(float.Parse).ToArray();
        return new(parts[0], parts[1]);
    }

    public static bool operator ==(Pointf left, Pointf right) => left.Equals(right);

    public static bool operator !=(Pointf left, Pointf right) => !left.Equals(right);

    public static Pointf operator +(Pointf left, Pointf right) => new(left.X + right.X, left.Y + right.Y);

    public static Pointf operator -(Pointf left, Pointf right) => new(left.X + right.X, left.Y + right.Y);

    public static Pointf operator *(Pointf point, float scalar) => new(point.X * scalar, point.Y * scalar);

    public static Pointf operator /(Pointf point, float scalar) => new(point.X / scalar, point.Y / scalar);

    public static implicit operator Pointf(Point point) => new(point.X, point.Y);

    public static implicit operator Vector2(Pointf point) => new(point.X, point.Y);
}
