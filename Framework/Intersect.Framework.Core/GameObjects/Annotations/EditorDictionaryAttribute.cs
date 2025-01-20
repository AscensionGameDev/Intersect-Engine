using System.Globalization;
using System.Reflection;
using Intersect.Framework.Reflection;
using Intersect.Localization;

#if !DEBUG

#endif

namespace Intersect.GameObjects.Annotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EditorDictionaryAttribute : EditorDisplayAttribute
{
    public EditorDictionaryAttribute(string name)
    {
        Group = default;
        Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
    }

    public EditorDictionaryAttribute(string group, string name) : this(name)
    {
        Group = !string.IsNullOrEmpty(group) ? group : throw new ArgumentNullException(nameof(group));
    }

    public string Group { get; }

    public string Name { get; }

    [Obsolete("We want to re-implement strings to be object-oriented.")]
    public override string Format(Type stringsType, object value)
    {
        if (stringsType == default)
        {
            throw new ArgumentNullException(nameof(stringsType));
        }

        var groupType = Group == default ? default : stringsType
            .GetNestedTypes(BindingFlags.Static | BindingFlags.Public)
            .FirstOrDefault(type => type.Name == Group);
        var parentType = groupType ?? stringsType;
        var fieldInfo = parentType.GetMember(Name).FirstOrDefault(member => member.MemberType == MemberTypes.Field) as FieldInfo;

        if (fieldInfo == default)
        {
            var error = new InvalidOperationException($"{parentType.FullName}.{Name} does not exist.");
#if DEBUG
            throw error;
#else
            LegacyLogging.Logger?.Error(error);
            return base.Format(stringsType, value);
#endif
        }

        var fieldValue = fieldInfo.GetValue(value);
        switch (fieldValue)
        {
            case Dictionary<int, LocalizedString> intKeyDictionary:
                try
                {
                    var key = Convert.ToInt32(value, CultureInfo.InvariantCulture);

                    if (!intKeyDictionary.TryGetValue(key, out var localizedString))
                    {
                        localizedString = $"KEY_MISSING=\"{key}\"";
                    }

                    return localizedString;
                }
                catch (Exception exception)
                {
                    throw new ArgumentException($"Expected an int or int-like but received a {value?.GetType()?.FullName}", nameof(value), exception);
                }

            case Dictionary<string, LocalizedString> stringKeyDictionary:
                {
                    if (value is not string key)
                    {
                        throw new ArgumentException($"Expected an int but received a {value?.GetType()?.FullName}", nameof(value));
                    }

                    if (!stringKeyDictionary.TryGetValue(key, out var localizedString))
                    {
                        throw new ArgumentOutOfRangeException($"Key missing: '{key}'");
                    }

                    return localizedString;
                }

            default:
                if (typeof(LocaleDictionary<,>).ExtendedBy(fieldValue?.GetType()) && value is Enum enumKey)
                {
                    return FormatLocaleDictionary(fieldValue, enumKey);
                }

                throw new InvalidOperationException($"Unsupported type: {fieldInfo.FieldType.FullName}");
        }
    }

    private delegate string? GetStringFromLocaleDictionaryGeneric(object? dictionaryObject, Enum genericKey);
    private static readonly Dictionary<Type, GetStringFromLocaleDictionaryGeneric> _cachedFormatters = new();
    private static readonly MethodInfo _methodInfoCreateWeaklyTypedDelegate = typeof(EditorDictionaryAttribute).GetMethod(nameof(CreateWeaklyTypedDelegate), BindingFlags.NonPublic | BindingFlags.Static);

    private static GetStringFromLocaleDictionaryGeneric CreateWeaklyTypedDelegate<TKey>()
    {
        return (dictionaryObject, genericKey) =>
        {
            if (dictionaryObject is not LocaleDictionary<TKey, LocalizedString> dictionary)
            {
                throw new ArgumentException($"Dictionary was a {dictionaryObject?.GetType().Name} but expected {typeof(LocaleDictionary<TKey, LocalizedString>).Name}", nameof(dictionaryObject));
            }

            if (genericKey is not TKey key)
            {
                throw new ArgumentException($"Key was a {genericKey.GetFullishName()} but expected {typeof(TKey).Name}.");
            }

            return Format<TKey>(dictionary, key);
        };
    }

    private static string? FormatLocaleDictionary(object? dictionaryObject, Enum key)
    {
        if (dictionaryObject == null)
        {
            return default;
        }

        var dictionaryType = dictionaryObject.GetType();
        if (!_cachedFormatters.TryGetValue(dictionaryType, out var formatter))
        {
            var createdDelegate = _methodInfoCreateWeaklyTypedDelegate.MakeGenericMethod(key.GetType()).Invoke(null, []);
            if (createdDelegate is not GetStringFromLocaleDictionaryGeneric genericFormatter)
            {
                throw new InvalidOperationException();
            }

            formatter = genericFormatter;
        }

        return formatter(dictionaryObject, key);
    }

    private static string? Format<TKey>(LocaleDictionary<TKey, LocalizedString> dictionary, TKey key)
    {
        if (!dictionary.TryGetValue(key, out var localizedString))
        {
            throw new ArgumentOutOfRangeException($"Key missing: '{key}'");
        }

        return localizedString;
    }
}
