using System;
using System.Globalization;
using System.Linq;

namespace Intersect.Client.Framework.Gwen
{

    /// <summary>
    ///     Represents inner spacing.
    /// </summary>
    public partial struct Padding : IEquatable<Padding>
    {

        public readonly int Top;

        public readonly int Bottom;

        public readonly int Left;

        public readonly int Right;

        // common values
        public static Padding Zero = new Padding(0, 0, 0, 0);

        public static Padding One = new Padding(1, 1, 1, 1);

        public static Padding Two = new Padding(2, 2, 2, 2);

        public static Padding Three = new Padding(3, 3, 3, 3);

        public static Padding Four = new Padding(4, 4, 4, 4);

        public static Padding Five = new Padding(5, 5, 5, 5);

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

        public static Padding FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return Zero;
            }

            var parts = str.Split(',').Select(part => int.Parse(part, CultureInfo.InvariantCulture)).ToArray();
            switch (parts.Length)
            {
                case 1:
                    return new Padding(parts[0], parts[0], parts[0], parts[0]);

                case 2:
                    return new Padding(parts[0], parts[1], parts[0], parts[1]);

                case 3:
                    return new Padding(parts[0], parts[1], parts[0], parts[2]);

                case 4:
                    return new Padding(parts[0], parts[1], parts[2], parts[3]);

                default:
                    throw new ArgumentException($"Expected 1-4 comma-separated numbers, receive {parts.Length}.", nameof(str));
            }
        }

    }

}
