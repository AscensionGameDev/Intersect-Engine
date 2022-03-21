using System.Runtime.CompilerServices;

namespace Intersect.Framework.Randomization;

/// <summary>
/// Extensions for <see cref="System.Random"/>.
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    /// Generates a new <see cref="byte"/> within the given inclusive bounds.
    /// </summary>
    /// <param name="random">the <see cref="Random"/> instance to use to generate the <see cref="byte"/></param>
    /// <param name="maxValue">the maximum value to generate, default <see cref="byte.MaxValue" /></param>
    /// <param name="minValue">the minimum value to generate, default <see cref="byte.MinValue" /></param>
    /// <returns>the random <see cref="byte"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte NextByte(this Random random, byte maxValue = byte.MaxValue, byte minValue = byte.MinValue)
    {
        var buffer = new byte[sizeof(byte)];
        random.NextBytes(buffer);
        var value = buffer[0];
        return Math.Min(Math.Max(minValue, value), maxValue);
    }

    /// <summary>
    /// Generates a new <see cref="char"/> within the given inclusive bounds.
    /// </summary>
    /// <param name="random">the <see cref="Random"/> instance to use to generate the <see cref="char"/></param>
    /// <param name="maxValue">the maximum value to generate, default <see cref="char.MaxValue" /></param>
    /// <param name="minValue">the minimum value to generate, default <see cref="char.MinValue" /></param>
    /// <returns>the random <see cref="char"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char NextChar(this Random random, char maxValue = char.MaxValue, char minValue = char.MinValue)
    {
        return (char)NextUShort(random, (ushort)maxValue, (ushort)minValue);
    }

    /// <summary>
    /// Generates a new <see cref="float"/> within the given inclusive bounds.
    /// </summary>
    /// <param name="random">the <see cref="Random"/> instance to use to generate the <see cref="float"/></param>
    /// <param name="maxValue">the maximum value to generate, default <see cref="float.MaxValue" /></param>
    /// <param name="minValue">the minimum value to generate, default <see cref="float.MinValue" /></param>
    /// <returns>the random <see cref="float"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float NextFloat(this Random random, float maxValue = float.MaxValue, float minValue = float.MinValue)
    {
        var buffer = new byte[sizeof(float)];
        random.NextBytes(buffer);
        var value = BitConverter.ToSingle(buffer);
        return Math.Min(Math.Max(minValue, value), maxValue);
    }

    /// <summary>
    /// Generates a new <see cref="int"/> within the given inclusive bounds.
    /// </summary>
    /// <param name="random">the <see cref="Random"/> instance to use to generate the <see cref="int"/></param>
    /// <param name="maxValue">the maximum value to generate, default <see cref="int.MaxValue" /></param>
    /// <param name="minValue">the minimum value to generate, default <see cref="int.MinValue" /></param>
    /// <returns>the random <see cref="int"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int NextInt(this Random random, int maxValue = int.MaxValue, int minValue = int.MinValue)
    {
        var buffer = new byte[sizeof(int)];
        random.NextBytes(buffer);
        var value = BitConverter.ToInt32(buffer);
        return Math.Min(Math.Max(minValue, value), maxValue);
    }

    /// <summary>
    /// Generates a new <see cref="long"/> within the given inclusive bounds.
    /// </summary>
    /// <param name="random">the <see cref="Random"/> instance to use to generate the <see cref="long"/></param>
    /// <param name="maxValue">the maximum value to generate, default <see cref="long.MaxValue" /></param>
    /// <param name="minValue">the minimum value to generate, default <see cref="long.MinValue" /></param>
    /// <returns>the random <see cref="long"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long NextLong(this Random random, long maxValue = long.MaxValue, long minValue = long.MinValue)
    {
        var buffer = new byte[sizeof(long)];
        random.NextBytes(buffer);
        var value = BitConverter.ToInt64(buffer);
        return Math.Min(Math.Max(minValue, value), maxValue);
    }

    /// <summary>
    /// Generates a new <see cref="sbyte"/> within the given inclusive bounds.
    /// </summary>
    /// <param name="random">the <see cref="Random"/> instance to use to generate the <see cref="sbyte"/></param>
    /// <param name="maxValue">the maximum value to generate, default <see cref="sbyte.MaxValue" /></param>
    /// <param name="minValue">the minimum value to generate, default <see cref="sbyte.MinValue" /></param>
    /// <returns>the random <see cref="sbyte"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte NextSByte(this Random random, sbyte maxValue = sbyte.MaxValue, sbyte minValue = sbyte.MinValue)
    {
        var buffer = new byte[sizeof(sbyte)];
        random.NextBytes(buffer);
        var value = unchecked((sbyte)buffer[0]);
        return Math.Min(Math.Max(minValue, value), maxValue);
    }

    /// <summary>
    /// Generates a new <see cref="short"/> within the given inclusive bounds.
    /// </summary>
    /// <param name="random">the <see cref="Random"/> instance to use to generate the <see cref="short"/></param>
    /// <param name="maxValue">the maximum value to generate, default <see cref="short.MaxValue" /></param>
    /// <param name="minValue">the minimum value to generate, default <see cref="short.MinValue" /></param>
    /// <returns>the random <see cref="short"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short NextShort(this Random random, short maxValue = short.MaxValue, short minValue = short.MinValue)
    {
        var buffer = new byte[sizeof(short)];
        random.NextBytes(buffer);
        var value = BitConverter.ToInt16(buffer);
        return Math.Min(Math.Max(minValue, value), maxValue);
    }

    /// <summary>
    /// Generates a new <see cref="uint"/> within the given inclusive bounds.
    /// </summary>
    /// <param name="random">the <see cref="Random"/> instance to use to generate the <see cref="uint"/></param>
    /// <param name="maxValue">the maximum value to generate, default <see cref="uint.MaxValue" /></param>
    /// <param name="minValue">the minimum value to generate, default <see cref="uint.MinValue" /></param>
    /// <returns>the random <see cref="uint"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint NextUInt(this Random random, uint maxValue = uint.MaxValue, uint minValue = uint.MinValue)
    {
        var buffer = new byte[sizeof(uint)];
        random.NextBytes(buffer);
        var value = BitConverter.ToUInt32(buffer);
        return Math.Min(Math.Max(minValue, value), maxValue);
    }

    /// <summary>
    /// Generates a new <see cref="ulong"/> within the given inclusive bounds.
    /// </summary>
    /// <param name="random">the <see cref="Random"/> instance to use to generate the <see cref="ulong"/></param>
    /// <param name="maxValue">the maximum value to generate, default <see cref="ulong.MaxValue" /></param>
    /// <param name="minValue">the minimum value to generate, default <see cref="ulong.MinValue" /></param>
    /// <returns>the random <see cref="ulong"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong NextULong(this Random random, ulong maxValue = ulong.MaxValue, ulong minValue = ulong.MinValue)
    {
        var buffer = new byte[sizeof(ulong)];
        random.NextBytes(buffer);
        var value = BitConverter.ToUInt64(buffer);
        return Math.Min(Math.Max(minValue, value), maxValue);
    }

    /// <summary>
    /// Generates a new <see cref="ushort"/> within the given inclusive bounds.
    /// </summary>
    /// <param name="random">the <see cref="Random"/> instance to use to generate the <see cref="ushort"/></param>
    /// <param name="maxValue">the maximum value to generate, default <see cref="ushort.MaxValue" /></param>
    /// <param name="minValue">the minimum value to generate, default <see cref="ushort.MinValue" /></param>
    /// <returns>the random <see cref="ushort"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort NextUShort(this Random random, ushort maxValue = ushort.MaxValue, ushort minValue = ushort.MinValue)
    {
        var buffer = new byte[sizeof(ushort)];
        random.NextBytes(buffer);
        var value = BitConverter.ToUInt16(buffer);
        return Math.Min(Math.Max(minValue, value), maxValue);
    }
}
