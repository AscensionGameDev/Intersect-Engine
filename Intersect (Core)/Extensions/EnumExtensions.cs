namespace Intersect.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Whether the given value is defined on its enum type.
    /// </summary>
    /// <typeparam name="T">the enum type</typeparam>
    /// <param name="enumValue">the value to check</param>
    /// <returns>if the value is valid for the given type</returns>
    public static bool IsDefined<T>(this T enumValue) where T : struct, Enum
    {
        return EnumValueCache<T>.DefinedValues.Contains(enumValue);
    }

    private static class EnumValueCache<T> where T : struct, Enum
    {
        public static readonly HashSet<T> DefinedValues = new(Enum.GetValues<T>());
    }
}
