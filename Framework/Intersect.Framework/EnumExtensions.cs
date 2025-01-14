namespace Intersect.Framework;

public static class EnumExtensions
{
    public static TEnum[] GetFlags<TEnum>(this TEnum @enum) where TEnum : struct, Enum =>
        Enum.GetValues<TEnum>().Where(flag => !flag.Equals(default(TEnum)) && @enum.HasFlag(flag)).ToArray();
}