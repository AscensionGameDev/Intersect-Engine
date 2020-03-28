using System;

namespace Intersect.Client.Framework.GenericClasses
{

    public struct Rectangle
    {

        private int mX;

        private int mY;

        private int mWidth;

        private int mHeight;

        public int X
        {
            get => mX;
            set => mX = value;
        }

        public int Y
        {
            get => mY;
            set => mY = value;
        }

        public int Width
        {
            get => mWidth;
            set => mWidth = value;
        }

        public int Height
        {
            get => mHeight;
            set => mHeight = value;
        }

        public int Left => X;

        public int Top => Y;

        public int Bottom => Y + Height;

        public int Right => X + Width;

        public static Rectangle Empty => new Rectangle();

        public Rectangle(int x, int y, int w, int h)
        {
            mX = x;
            mY = y;
            mWidth = w;
            mHeight = h;
        }

        public static Rectangle Intersect(Rectangle a, Rectangle b)
        {
            // MS.NET returns a non-empty rectangle if the two rectangles
            // touch each other
            if (!a.IntersectsWithInclusive(b))
            {
                return Empty;
            }

            return Rectangle.FromLtrb(
                Math.Max(a.Left, b.Left), Math.Max(a.Top, b.Top), Math.Min(a.Right, b.Right),
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

        public void Reset()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
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

        public static string ToString(Rectangle rect)
        {
            return rect.X + "," + rect.Y + "," + rect.Width + "," + rect.Height;
        }

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

    }

}
