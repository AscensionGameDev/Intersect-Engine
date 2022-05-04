using System.Diagnostics.CodeAnalysis;

namespace Intersect.Time;

public struct FrameTime : IEquatable<FrameTime>
{
    public readonly TimeSpan Delta;

    public readonly TimeSpan Total;

    public FrameTime(TimeSpan delta, TimeSpan total)
    {
        Delta = delta;
        Total = total;
    }

    public override bool Equals([NotNullWhen(true)] object obj) => obj is FrameTime frameTime && Delta.Equals(frameTime.Delta);

    public bool Equals(FrameTime other) => Delta.Equals(other.Delta) && Total.Equals(other.Total);

    public override int GetHashCode() => HashCode.Combine(Delta, Total);

    public override string ToString() => $"[Delta = {Delta}, Total = {Total}]";

    public static bool operator ==(FrameTime left, FrameTime right) => Equals(left, right);

    public static bool operator !=(FrameTime left, FrameTime right) => !Equals(left, right);
}
