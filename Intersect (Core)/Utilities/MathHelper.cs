using System;

using JetBrains.Annotations;

namespace Intersect.Utilities
{

    public static class MathHelper
    {

        [Pure]
        public static decimal Clamp(decimal value, decimal minimum, decimal maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        [Pure]
        public static double Clamp(double value, double minimum, double maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        [Pure]
        public static sbyte Clamp(sbyte value, sbyte minimum, sbyte maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        [Pure]
        public static short Clamp(short value, short minimum, short maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        [Pure]
        public static int Clamp(int value, int minimum, int maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        [Pure]
        public static long Clamp(long value, long minimum, long maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        [Pure]
        public static byte Clamp(byte value, byte minimum, byte maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        [Pure]
        public static ushort Clamp(ushort value, ushort minimum, ushort maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        [Pure]
        public static uint Clamp(uint value, uint minimum, uint maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        [Pure]
        public static ulong Clamp(ulong value, ulong minimum, ulong maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

    }

}
