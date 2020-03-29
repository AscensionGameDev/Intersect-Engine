using System;

namespace Intersect.Client.Framework.GenericClasses
{

    public struct Pointf
    {

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
            if (obj is Pointf)
            {
                return (Pointf) obj == this;
            }

            return false;
        }

        public bool Equals(Pointf other)
        {
            return Math.Abs(X - other.X) < TOLERANCE && Math.Abs(Y - other.Y) < TOLERANCE;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static Pointf Empty => new Pointf();

        public static bool operator !=(Pointf left, Pointf right)
        {
            return Math.Abs(left.X - right.X) > TOLERANCE || Math.Abs(left.Y - right.Y) > TOLERANCE;
        }

        public static bool operator ==(Pointf left, Pointf right)
        {
            return Math.Abs(left.X - right.X) < TOLERANCE && Math.Abs(left.Y - right.Y) < TOLERANCE;
        }

    }

}
