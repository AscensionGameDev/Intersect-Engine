using System;

namespace Intersect.Client.Framework.GenericClasses
{

    public struct FloatRect
    {

        private float mX;

        private float mY;

        private float mWidth;

        private float mHeight;

        public float X
        {
            get => mX;
            set => mX = value;
        }

        public float Y
        {
            get => mY;
            set => mY = value;
        }

        public float Width
        {
            get => mWidth;
            set => mWidth = value;
        }

        public float Height
        {
            get => mHeight;
            set => mHeight = value;
        }

        public float Left => X;

        public float Top => Y;

        public float Bottom => Y + Height;

        public float Right => X + Width;

        public static FloatRect Empty => new FloatRect();

        public FloatRect(float x, float y, float w, float h)
        {
            mX = x;
            mY = y;
            mWidth = w;
            mHeight = h;
        }

        public static FloatRect Intersect(FloatRect a, FloatRect b)
        {
            // MS.NET returns a non-empty rectangle if the two rectangles
            // touch each other
            if (!a.IntersectsWithInclusive(b))
            {
                return Empty;
            }

            return FloatRect.FromLtrb(
                Math.Max(a.Left, b.Left), Math.Max(a.Top, b.Top), Math.Min(a.Right, b.Right),
                Math.Min(a.Bottom, b.Bottom)
            );
        }

        public static FloatRect FromLtrb(float left, float top, float right, float bottom)
        {
            return new FloatRect(left, top, right - left, bottom - top);
        }

        public bool IntersectsWith(FloatRect rect)
        {
            return !(Left >= rect.Right || Right <= rect.Left || Top >= rect.Bottom || Bottom <= rect.Top);
        }

        private bool IntersectsWithInclusive(FloatRect r)
        {
            return !(Left > r.Right || Right < r.Left || Top > r.Bottom || Bottom < r.Top);
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
        ///     Checks if an x,y coordinate lies within this Rectangle.
        /// </remarks>
        public bool Contains(float x, float y)
        {
            return x >= Left && x < Right && y >= Top && y < Bottom;
        }

        /// <summary>
        ///     Contains Method
        /// </summary>
        /// <remarks>
        ///     Checks if a Pofloat lies within this Rectangle.
        /// </remarks>
        public bool Contains(Point pt)
        {
            return Contains(pt.X, pt.Y);
        }

    }

}
