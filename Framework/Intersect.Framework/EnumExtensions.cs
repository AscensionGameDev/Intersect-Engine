using System.Diagnostics;
using Intersect.Framework.Reflection;

namespace Intersect.Framework;

public static class EnumExtensions
{
    public static TEnum[] GetFlags<TEnum>(this TEnum @enum) where TEnum : struct, Enum =>
        Enum.GetValues<TEnum>().Where(flag => !flag.Equals(default(TEnum)) && @enum.HasFlag(flag)).ToArray();

    public static object ToUnderlyingType<TEnum>(this TEnum @enum) where TEnum : struct, Enum
    {
        var enumUnderlyingType = typeof(TEnum).GetEnumUnderlyingType();
        if (enumUnderlyingType == typeof(int))
        {
            return Convert.ToInt32(@enum);
        }

        if (enumUnderlyingType == typeof(uint))
        {
            return Convert.ToUInt32(@enum);
        }

        if (enumUnderlyingType == typeof(long))
        {
            return Convert.ToInt64(@enum);
        }

        if (enumUnderlyingType == typeof(ulong))
        {
            return Convert.ToUInt64(@enum);
        }

        if (enumUnderlyingType == typeof(short))
        {
            return Convert.ToInt16(@enum);
        }

        if (enumUnderlyingType == typeof(ushort))
        {
            return Convert.ToUInt16(@enum);
        }

        if (enumUnderlyingType == typeof(byte))
        {
            return Convert.ToByte(@enum);
        }

        if (enumUnderlyingType == typeof(sbyte))
        {
            return Convert.ToSByte(@enum);
        }

        throw new UnreachableException($"Invalid underlying enum type {enumUnderlyingType.GetName(qualified: true)}");
    }
}