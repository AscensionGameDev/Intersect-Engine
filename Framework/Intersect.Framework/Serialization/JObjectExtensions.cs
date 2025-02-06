using Newtonsoft.Json.Linq;

namespace Intersect.Framework.Serialization;

public static class JObjectExtensions
{
    public static bool TryGet<TEnum>(this JObject @object, string propertyName, out TEnum propertyValue)
        where TEnum : struct, Enum =>
        TryGet(@object, propertyName, default, out propertyValue);

    public static bool TryGet<TEnum>(this JObject @object, string propertyName, TEnum defaultValue, out TEnum propertyValue)
        where TEnum : struct, Enum
    {
        if (!@object.TryGetValue(propertyName, out var token))
        {
            propertyValue = defaultValue;
            return false;
        }

        if (token is not JValue { Type: JTokenType.String } value)
        {
            propertyValue = defaultValue;
            return false;
        }

        var rawEnum = value.Value<string>();
        if (Enum.TryParse(rawEnum, out propertyValue))
        {
            return true;
        }

        propertyValue = defaultValue;
        return false;

    }
}