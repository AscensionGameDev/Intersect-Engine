using System;

namespace Intersect.Client.Framework.GenericClasses
{

    public struct Pointf
    {
        public static Pointf Empty => new Pointf();

        public static Pointf UnitX => new Pointf(1, 0);

        public static Pointf UnitY => new Pointf(0, 1);

        private const float TOLERANCE = 0.001f;

        public float X { get; set; }

        public float Y { get; set; }

        public Pointf(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Pointf point)
            {
                return point == this;
            }

            return false;
        }

        public bool Equals(Pointf other)
        {
            return Math.Abs(X - other.X) < TOLERANCE && Math.Abs(Y - other.Y) < TOLERANCE;
        }

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

        public Pointf StripX() => new Pointf(0, Y);

        public Pointf StripY() => new Pointf(X, 0);

        public static bool operator !=(Pointf left, Pointf right)
        {
            return Math.Abs(left.X - right.X) > TOLERANCE || Math.Abs(left.Y - right.Y) > TOLERANCE;
        }

        public static bool operator ==(Pointf left, Pointf right)
        {
            return Math.Abs(left.X - right.X) < TOLERANCE && Math.Abs(left.Y - right.Y) < TOLERANCE;
        }

        public static Pointf operator +(Pointf left, Pointf right) => new Pointf(left.X + right.X, left.Y + right.Y);

        public static Pointf operator -(Pointf left, Pointf right) => new Pointf(left.X - right.X, left.Y - right.Y);

        public static Pointf operator *(Pointf point, float scalar) => new Pointf(point.X * scalar, point.Y * scalar);

        public static Pointf operator /(Pointf point, float scalar) => new Pointf(point.X / scalar, point.Y / scalar);
    }

}
