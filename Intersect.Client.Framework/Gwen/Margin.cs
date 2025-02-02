using System.Globalization;
using System.Runtime.InteropServices;

#pragma warning disable CS0660, CS0661
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
namespace Intersect.Client.Framework.Gwen;


/// <summary>
///     Represents outer spacing.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public partial struct Margin : IEquatable<Margin>
{
    public int Top;
    public int Right;
    public int Bottom;
    public int Left;

    // common values
    public static Margin Zero = new(0, 0, 0, 0);

    public static Margin One = new(1, 1, 1, 1);

    public static Margin Two = new(2, 2, 2, 2);

    public static Margin Three = new(3, 3, 3, 3);

    public static Margin Four = new(4, 4, 4, 4);

    public static Margin Five = new(5, 5, 5, 5);

    public static Margin Six = new(6, 6, 6, 6);

    public static Margin Seven = new(7, 7, 7, 7);

    public static Margin Eight = new(8, 8, 8, 8);

    public static Margin Nine = new(9, 9, 9, 9);

    public static Margin Ten = new(10, 10, 10, 10);

    public Margin(int size) : this(size, size, size, size) { }

    public Margin(int horizontal, int vertical) : this(horizontal, vertical, horizontal, vertical) { }

    public Margin(int left, int top, int right, int bottom)
    {
        Top = top;
        Bottom = bottom;
        Left = left;
        Right = right;
    }

    public bool Equals(Margin other) =>
        other.Top == Top && other.Bottom == Bottom && other.Left == Left && other.Right == Right;

    public static bool operator ==(Margin lhs, Margin rhs) => lhs.Equals(rhs);

    public static bool operator !=(Margin lhs, Margin rhs) => !lhs.Equals(rhs);

    public override bool Equals(object? other) => other is Margin margin && Equals(margin);

    public override string ToString() => string.Join(',', Left, Top, Right, Bottom);

    public static Margin FromString(string rawMargin)
    {
        if (string.IsNullOrEmpty(rawMargin))
        {
            return default;
        }

        var parts = rawMargin.Split(',')
            .Select(rawPart => int.TryParse(rawPart, CultureInfo.InvariantCulture, out var part) ? part : 0)
            .ToArray();

        return parts.Length switch
        {
            < 1 => new Margin(),
            1 => new Margin(parts[0]),
            2 => new Margin(parts[0], parts[1], parts[0], parts[1]),
            3 => new Margin(parts[0], parts[1], parts[2], parts[1]),
            _ => new Margin(parts[0], parts[1], parts[2], parts[3]),
        };
    }
}
