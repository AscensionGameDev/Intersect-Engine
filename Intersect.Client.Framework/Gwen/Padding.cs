using System.Globalization;

namespace Intersect.Client.Framework.Gwen;


/// <summary>
///     Represents inner spacing.
/// </summary>
public partial struct Padding : IEquatable<Padding>
{
    public int Top;

    public int Bottom;

    public int Left;

    public int Right;

    // common values
    public static Padding Zero = new Padding(0, 0, 0, 0);

    public static Padding One = new Padding(1, 1, 1, 1);

    public static Padding Two = new Padding(2, 2, 2, 2);

    public static Padding TwoV = new Padding(0, 2);

    public static Padding Three = new Padding(3, 3, 3, 3);

    public static Padding Four = new Padding(4, 4, 4, 4);

    public static Padding Five = new Padding(5, 5, 5, 5);

    public static Padding FourH = new(4, 0);

    public Padding(int size) : this(size, size, size, size) { }

    public Padding(int horizontal, int vertical) : this(horizontal, vertical, horizontal, vertical) { }

    public Padding(int left, int top, int right, int bottom)
    {
        Top = top;
        Bottom = bottom;
        Left = left;
        Right = right;
    }

    public bool Equals(Padding other)
    {
        return other.Top == Top && other.Bottom == Bottom && other.Left == Left && other.Right == Right;
    }

    public static Padding operator +(Padding lhs, Padding rhs) => new(
        lhs.Left + rhs.Left,
        lhs.Top + rhs.Top,
        lhs.Right + rhs.Right,
        lhs.Bottom + rhs.Bottom
    );

    public static Padding operator -(Padding lhs, Padding rhs) => new(
        lhs.Left - rhs.Left,
        lhs.Top - rhs.Top,
        lhs.Right - rhs.Right,
        lhs.Bottom - rhs.Bottom
    );

    public static Padding operator *(Padding lhs, int rhs) => new(
        lhs.Left * rhs,
        lhs.Top * rhs,
        lhs.Right * rhs,
        lhs.Bottom * rhs
    );

    public static Padding operator /(Padding lhs, int rhs) => new(
        lhs.Left / rhs,
        lhs.Top / rhs,
        lhs.Right / rhs,
        lhs.Bottom / rhs
    );

    public static bool operator ==(Padding lhs, Padding rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(Padding lhs, Padding rhs)
    {
        return !lhs.Equals(rhs);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (obj.GetType() != typeof(Padding))
        {
            return false;
        }

        return Equals((Padding) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var result = Top;
            result = (result * 397) ^ Bottom;
            result = (result * 397) ^ Left;
            result = (result * 397) ^ Right;

            return result;
        }
    }

    public override string ToString() => $"{Left},{Top},{Right},{Bottom}";

    public static string ToString(Padding pad) => pad.ToString();

    public static Padding FromString(string rawPadding)
    {
        if (string.IsNullOrEmpty(rawPadding))
        {
            return default;
        }

        var parts = rawPadding.Split(',')
            .Select(rawPart => int.TryParse(rawPart, CultureInfo.InvariantCulture, out var part) ? part : 0)
            .ToArray();

        return parts.Length switch
        {
            < 1 => new Padding(),
            1 => new Padding(parts[0]),
            2 => new Padding(parts[0], parts[1], parts[0], parts[1]),
            3 => new Padding(parts[0], parts[1], parts[2], parts[1]),
            _ => new Padding(parts[0], parts[1], parts[2], parts[3]),
        };
    }
}
