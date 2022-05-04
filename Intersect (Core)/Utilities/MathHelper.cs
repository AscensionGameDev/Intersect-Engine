namespace Intersect.Utilities
{

    public static partial class MathHelper
    {
        public static decimal Clamp(decimal value, decimal minimum, decimal maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        public static double Clamp(double value, double minimum, double maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        public static float Clamp(float value, float minimum, float maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        public static sbyte Clamp(sbyte value, sbyte minimum, sbyte maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        public static byte Clamp(byte value, byte minimum, byte maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        public static short Clamp(short value, short minimum, short maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        public static ushort Clamp(ushort value, ushort minimum, ushort maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        public static int Clamp(int value, int minimum, int maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        public static uint Clamp(uint value, uint minimum, uint maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        public static long Clamp(long value, long minimum, long maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="minimum">The minimum value. If <paramref name="value"/> is less than <paramref name="minimum"/>, <paramref name="minimum"/> will be returned.</param>
        /// <param name="maximum">The maximum value. If <paramref name="value"/> is greater than <paramref name="maximum"/>, <paramref name="maximum"/> will be returned.</param>
        /// <returns>The clamped value.</returns>
        public static ulong Clamp(ulong value, ulong minimum, ulong maximum)
        {
            return Math.Min(Math.Max(value, minimum), maximum);
        }

        /// <summary>
        /// Linearly interpolates between two values.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Destination value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <returns>Interpolated value.</returns>
        /// <remarks>This method performs the linear interpolation based on the following formula:
        /// <code>value1 + (value2 - value1) * amount</code>.
        /// Passing amount a value of 0 will cause value1 to be returned, a value of 1 will cause value2 to be returned.
        /// See <see cref="MathHelper.LerpPrecise(double, double, double)"/> for a less efficient version with more precision around edge cases.
        /// </remarks>
        public static double Lerp(double value1, double value2, double amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        /// <summary>
        /// Linearly interpolates between two values.
        /// This method is a less efficient, more precise version of <see cref="MathHelper.Lerp"/>.
        /// See remarks for more info.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Destination value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <returns>Interpolated value.</returns>
        /// <remarks>This method performs the linear interpolation based on the following formula:
        /// <code>((1 - amount) * value1) + (value2 * amount)</code>.
        /// Passing amount a value of 0 will cause value1 to be returned, a value of 1 will cause value2 to be returned.
        /// This method does not have the floating point precision issue that <see cref="MathHelper.Lerp"/> has.
        /// i.e. If there is a big gap between value1 and value2 in magnitude (e.g. value1=10000000000000000, value2=1),
        /// right at the edge of the interpolation range (amount=1), <see cref="MathHelper.Lerp"/> will return 0 (whereas it should return 1).
        /// This also holds for value1=10^17, value2=10; value1=10^18,value2=10^2... so on.
        /// For an in depth explanation of the issue, see below references:
        /// Relevant Wikipedia Article: https://en.wikipedia.org/wiki/Linear_interpolation#Programming_language_support
        /// Relevant StackOverflow Answer: http://stackoverflow.com/questions/4353525/floating-point-linear-interpolation#answer-23716956
        /// </remarks>
        public static double LerpPrecise(double value1, double value2, double amount)
        {
            return ((1 - amount) * value1) + (value2 * amount);
        }

        /// <summary>
        /// Linearly interpolates between two values.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Destination value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <returns>Interpolated value.</returns>
        /// <remarks>This method performs the linear interpolation based on the following formula:
        /// <code>value1 + (value2 - value1) * amount</code>.
        /// Passing amount a value of 0 will cause value1 to be returned, a value of 1 will cause value2 to be returned.
        /// See <see cref="MathHelper.LerpPrecise(float, float, float)"/> for a less efficient version with more precision around edge cases.
        /// </remarks>
        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        /// <summary>
        /// Linearly interpolates between two values.
        /// This method is a less efficient, more precise version of <see cref="MathHelper.Lerp"/>.
        /// See remarks for more info.
        /// </summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Destination value.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <returns>Interpolated value.</returns>
        /// <remarks>This method performs the linear interpolation based on the following formula:
        /// <code>((1 - amount) * value1) + (value2 * amount)</code>.
        /// Passing amount a value of 0 will cause value1 to be returned, a value of 1 will cause value2 to be returned.
        /// This method does not have the floating point precision issue that <see cref="MathHelper.Lerp"/> has.
        /// i.e. If there is a big gap between value1 and value2 in magnitude (e.g. value1=10000000000000000, value2=1),
        /// right at the edge of the interpolation range (amount=1), <see cref="MathHelper.Lerp"/> will return 0 (whereas it should return 1).
        /// This also holds for value1=10^17, value2=10; value1=10^18,value2=10^2... so on.
        /// For an in depth explanation of the issue, see below references:
        /// Relevant Wikipedia Article: https://en.wikipedia.org/wiki/Linear_interpolation#Programming_language_support
        /// Relevant StackOverflow Answer: http://stackoverflow.com/questions/4353525/floating-point-linear-interpolation#answer-23716956
        /// </remarks>
        public static float LerpPrecise(float value1, float value2, float amount)
        {
            return ((1 - amount) * value1) + (value2 * amount);
        }
    }
}
