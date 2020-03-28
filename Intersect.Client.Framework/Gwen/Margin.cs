using System;

namespace Intersect.Client.Framework.Gwen
{

    /// <summary>
    ///     Represents outer spacing.
    /// </summary>
    public struct Margin : IEquatable<Margin>
    {

        public int Top;

        public int Bottom;

        public int Left;

        public int Right;

        // common values
        public static Margin Zero = new Margin(0, 0, 0, 0);

        public static Margin One = new Margin(1, 1, 1, 1);

        public static Margin Two = new Margin(2, 2, 2, 2);

        public static Margin Three = new Margin(3, 3, 3, 3);

        public static Margin Four = new Margin(4, 4, 4, 4);

        public static Margin Five = new Margin(5, 5, 5, 5);

        public static Margin Six = new Margin(6, 6, 6, 6);

        public static Margin Seven = new Margin(7, 7, 7, 7);

        public static Margin Eight = new Margin(8, 8, 8, 8);

        public static Margin Nine = new Margin(9, 9, 9, 9);

        public static Margin Ten = new Margin(10, 10, 10, 10);

        public Margin(int left, int top, int right, int bottom)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public bool Equals(Margin other)
        {
            return other.Top == Top && other.Bottom == Bottom && other.Left == Left && other.Right == Right;
        }

        public static bool operator ==(Margin lhs, Margin rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Margin lhs, Margin rhs)
        {
            return !lhs.Equals(rhs);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj.GetType() != typeof(Margin))
            {
                return false;
            }

            return Equals((Margin) obj);
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

        public static string ToString(Margin mar)
        {
            return mar.Left + "," + mar.Top + "," + mar.Right + "," + mar.Bottom;
        }

        public static Margin FromString(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return Margin.Zero;
            }

            var strs = val.Split(",".ToCharArray());
            var parts = new int[strs.Length];
            for (var i = 0; i < strs.Length; i++)
            {
                parts[i] = int.Parse(strs[i]);
            }

            return new Margin(parts[0], parts[1], parts[2], parts[3]);
        }

    }

}
