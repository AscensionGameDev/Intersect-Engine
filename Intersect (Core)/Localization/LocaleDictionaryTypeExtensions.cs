using Intersect.Framework.Reflection;

namespace Intersect.Localization;

public static class LocaleDictionaryTypeExtensions
{
    public static Type GetLocaleDictionaryKeyType(this Type type) => type.GetLocaleDictionaryTypes().Key;

    public static KeyValuePair<Type, Type> GetLocaleDictionaryTypes(this Type type)
    {
        var localeDictionaryParameters = type.FindGenericTypeParameters(typeof(LocaleDictionary<,>));
        return new(localeDictionaryParameters[0], localeDictionaryParameters[1]);
    }

    public static bool IsLocaleDictionary(this Type type) =>
        type.Extends(typeof(LocaleDictionary<,>));

    public static bool IsLocalizedStringDictionary(this Type type)
    {
        var localeDictionaryParameters = type.FindGenericTypeParameters(typeof(LocaleDictionary<,>), false);
        return localeDictionaryParameters?.LastOrDefault() == typeof(LocalizedString);
    }
}
