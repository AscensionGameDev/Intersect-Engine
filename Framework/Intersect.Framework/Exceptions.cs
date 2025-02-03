using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Intersect.Framework.Reflection;

namespace Intersect.Framework;

public static class Exceptions
{
    public static UnreachableException UnreachableInvalidEnum<TEnum>(TEnum value) where TEnum : struct, Enum =>
        new(
            $"Invalid {typeof(TEnum).GetName(qualified: true)} '{value}' ({value.ToUnderlyingType()})"
        );

    [DoesNotReturn]
    public static void ThrowUnreachableInvalidEnum<TEnum>(TEnum value) where TEnum : struct, Enum =>
        throw UnreachableInvalidEnum(value);
}