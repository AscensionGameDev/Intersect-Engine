using MessagePack;

namespace Intersect
{
    [MessagePackObject]
    public struct Point
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

        public static Point Empty => new Point();

        public static bool operator !=(Point left, Point right)
        {
            return left.X != right.X || left.Y != right.Y;
        }

        public static bool operator ==(Point left, Point right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static string ToString(Point pnt)
        {
            return pnt.X + "," + pnt.Y;
        }

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

    }

}
