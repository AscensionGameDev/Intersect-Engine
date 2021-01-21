using System;

namespace Intersect.Client.Framework.Graphics
{

    public struct Resolution
    {

        public static readonly Resolution Empty = default;

        private static readonly char[] Separators = { 'x', ',', ' ', '/', '-', '_', '.', '~' };

        public readonly ushort X;

        public readonly ushort Y;

        public Resolution(long x = 800, long y = 600)
        {
            X = (ushort)(x & 0xFFFF);
            Y = (ushort)(y & 0xFFFF);
        }

        public Resolution(ulong x = 800, ulong y = 600)
        {
            X = (ushort)(x & 0xFFFF);
            Y = (ushort)(y & 0xFFFF);
        }

        public Resolution(Resolution resolution, long overrideX = 0, long overrideY = 0)
            : this(
                overrideX > 0 ? overrideX : resolution.X, overrideY > 0 ? overrideY : resolution.Y
            )
        { }

        public Resolution(Resolution resolution, Resolution? overrideResolution = null)
        {
            var x = overrideResolution?.X ?? resolution.X;
            X = x > 0 ? x : resolution.X;

            var y = overrideResolution?.Y ?? resolution.Y;
            Y = y > 0 ? y : resolution.Y;
        }

        public override bool Equals(object obj) => obj is Resolution resolution && Equals(resolution);

        public bool Equals(Resolution other) => X == other.X && Y == other.Y;

        public override int GetHashCode() => (X << 16) & Y;

        public override string ToString() => $"{X},{Y}";

        public static Resolution Parse(string resolution)
        {
            var split = resolution?.Split(Separators);
            string xString = split?[0], yString = split?[1];

            if (string.IsNullOrWhiteSpace(xString))
            {
                throw new ArgumentNullException(nameof(xString));
            }

            if (string.IsNullOrWhiteSpace(yString))
            {
                throw new ArgumentNullException(nameof(xString));
            }

            var x = ushort.Parse(xString);
            var y = ushort.Parse(yString);
            return new Resolution(x, y);
        }

        public static bool TryParse(string resolutionString, out Resolution resolution)
        {
            try
            {
                resolution = Parse(resolutionString);
                return true;
            }
            catch
            {
                // Ignore
                resolution = default;
                return false;
            }
        }

        public static bool operator ==(Resolution left, Resolution right) =>
            left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Resolution left, Resolution right) =>
            left.X != right.X && left.Y != right.Y;
    }

}
