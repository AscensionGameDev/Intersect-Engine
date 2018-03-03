using System;
using JetBrains.Annotations;

namespace Intersect.Utilities
{
    public static class MathHelper
    {
        [Pure]
        public static decimal Clamp(decimal value, decimal minimum, decimal maximum)
            => Math.Min(Math.Max(value, minimum), maximum);

        [Pure]
        public static double Clamp(double value, double minimum, double maximum)
            => Math.Min(Math.Max(value, minimum), maximum);

        [Pure]
        public static long Clamp(long value, long minimum, long maximum)
            => Math.Min(Math.Max(value, minimum), maximum);

        [Pure]
        public static ulong Clamp(ulong value, ulong minimum, ulong maximum)
            => Math.Min(Math.Max(value, minimum), maximum);
    }
}
