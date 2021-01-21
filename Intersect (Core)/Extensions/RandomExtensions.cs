using System;

using MathNet.Numerics.Random;

namespace Intersect.Extensions
{

    public static class RandomExtensions
    {

        public static long NextLong(this Random random)
        {
            var buffer = new byte[8];
            random.NextBytes(buffer);

            return BitConverter.ToInt64(buffer, 0);
        }

        public static long NextLong(this Random random, long maximum)
        {
            if (maximum <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), @"'maximum' must be greater than 0.");
            }

            return NextLong(random) % maximum;
        }

        public static long NextLong(this Random random, long minimum, long maximum)
        {
            if (minimum >= maximum)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), @"'maximum' must be greater than 'minimum'.");
            }

            var value = NextULong(random, (ulong) (maximum - minimum));
            if (value < long.MaxValue)
            {
                return minimum + (long) value;
            }

            return maximum - (long) (ulong.MaxValue - value);
        }

        public static ulong NextULong(this Random random)
        {
            var buffer = new byte[8];
            random.NextBytes(buffer);

            return BitConverter.ToUInt64(buffer, 0);
        }

        public static ulong NextULong(this Random random, ulong maximum)
        {
            return NextULong(random) % maximum;
        }

        public static ulong NextULong(this Random random, ulong minimum, ulong maximum)
        {
            if (minimum >= maximum)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), $@"'{nameof(maximum)}' must be greater than '{nameof(minimum)}'.");
            }

            return NextULong(random, maximum - minimum) + minimum;
        }

        public static decimal NextDecimal(this Random random, decimal maximum)
        {
            if (maximum <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), $@"'{nameof(maximum)}' must be greater than 0.");
            }

            return random.NextDecimal() * maximum;
        }

        public static decimal NextDecimal(this Random random, decimal minimum, decimal maximum)
        {
            if (minimum >= maximum)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), $@"'{nameof(maximum)}' must be greater than '{nameof(minimum)}'.");
            }

            var midpoint = minimum / 2M + maximum / 2M;
            var halfRange = Math.Abs(maximum - midpoint);

            return (random.NextDecimal(2M) - 1M) * halfRange + midpoint;
        }

        public static double NextDouble(this Random random, double maximum)
        {
            if (maximum <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), $@"'{nameof(maximum)}' must be greater than 0.");
            }

            return random.NextDouble() * maximum;
        }

        public static double NextDouble(this Random random, double minimum, double maximum)
        {
            if (minimum >= maximum)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), $@"'{nameof(maximum)}' must be greater than '{nameof(minimum)}'.");
            }

            var midpoint = minimum / 2D + maximum / 2D;
            var halfRange = Math.Abs(maximum - midpoint);

            return (random.NextDouble(2D) - 1D) * halfRange + midpoint;
        }

        public static float NextFloat(this Random random)
        {
            return (float) random.NextDouble();
        }

        public static float NextFloat(this Random random, float maximum)
        {
            if (maximum <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), $@"'{nameof(maximum)}' must be greater than 0.");
            }

            return (float) NextDouble(random, maximum);
        }

        public static float NextFloat(this Random random, float minimum, float maximum)
        {
            if (minimum >= maximum)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), $@"'{nameof(maximum)}' must be greater than '{nameof(minimum)}'.");
            }

            return (float) NextDouble(random, minimum, maximum);
        }

    }

}
