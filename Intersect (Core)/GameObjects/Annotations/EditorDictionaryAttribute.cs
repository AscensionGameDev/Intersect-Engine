using System.Globalization;
using System.Reflection;

using Intersect.Framework.Reflection;
using Intersect.Generic;
using Intersect.Localization;
using Intersect.Localization.Common;

namespace Intersect.GameObjects.Annotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class EditorDictionaryAttribute : EditorDisplayAttribute
{
    private string _legacyGroupName;
    private FieldInfo _targetFieldInfo;
    private Type _targetKeyType;

    public EditorDictionaryAttribute(string name)
    {
        Group = default;
        Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
    }

    public EditorDictionaryAttribute(string group, string name) : this(name)
    {
        Group = !string.IsNullOrEmpty(group) ? group : throw new ArgumentNullException(nameof(group));
    }

    public EditorDictionaryAttribute(Type namespaceType, string dictionaryName)
    {
        Namespace = namespaceType ?? throw new ArgumentNullException(nameof(namespaceType));

        if (string.IsNullOrWhiteSpace(dictionaryName))
        {
            throw new ArgumentNullException(nameof(dictionaryName));
        }

        Name = dictionaryName;

        _targetFieldInfo = Namespace.GetField(Name, BindingFlags.Public | BindingFlags.Instance);

        if (_targetFieldInfo == default)
        {
            throw new ArgumentException(
                $"Unable to find {Name} on {Namespace.FullName}.",
                nameof(dictionaryName)
            );
        }

        var localeDictionaryTypes = _targetFieldInfo.FieldType.GetLocaleDictionaryTypes();
        if (!localeDictionaryTypes.Value.Extends<LocalizedString>())
        {
            throw new ArgumentException(
                $"{_targetFieldInfo.FieldType.FullName} must extend {typeof(LocaleDictionary<,>).FullName} where the second parameter is {typeof(LocalizedString).FullName} or a subclass.",
                nameof(dictionaryName)
            );
        }

        _targetKeyType = localeDictionaryTypes.Key;
    }

    public string Group
    {
        get => Namespace?.FullName ?? _legacyGroupName ?? throw new NullReferenceException("Attribute initialized incorrectly.");
        private set => _legacyGroupName = value;
    }

    public string Name { get; }

    public Type Namespace { get; }

    public override string Format(RootNamespace rootNamespace, object value)
    {
        if (_targetFieldInfo == default)
        {
            throw new InvalidOperationException();
        }

        if (value?.GetType() != _targetKeyType)
        {
            throw new ArgumentException($"Passed value must be a {_targetKeyType}.", nameof(value));
        }

        var dictionary = rootNamespace.Localized[_targetFieldInfo];
        var localizedString = GenericDictionaryAccessor.Get<LocalizedString>(dictionary, value);
        return localizedString;
    }

    public override string Format(Type stringsType, object value)
    {
        if (stringsType == default)
        {
            throw new ArgumentNullException(nameof(stringsType));
        }

        var fieldInfo = _targetFieldInfo;
        if (fieldInfo == default)
        {
            var groupType = Group == default ? default : stringsType
                .GetNestedTypes(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(type => type.Name == Group);
            var parentType = groupType ?? stringsType;
            fieldInfo = parentType.GetMember(Name).FirstOrDefault(member => member.MemberType == MemberTypes.Field) as FieldInfo;

            if (fieldInfo == default)
            {
                var error = new InvalidOperationException($"{parentType.FullName}.{Name} does not exist.");
#if DEBUG
                throw error;
#else
                Log.Error(error);
                return base.Format(stringsType, value);
#endif
            }
        }

        var fieldValue = fieldInfo.GetValue(null);
        switch (fieldValue)
        {
            case Dictionary<int, LocalizedString> intKeyDictionary:
                try
                {
                    var key = Convert.ToInt32(value, CultureInfo.InvariantCulture);

                    if (!intKeyDictionary.TryGetValue(key, out var localizedString))
                    {
                        throw new ArgumentOutOfRangeException($"Key missing: {key}");
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
                {
                    throw new InvalidOperationException($"Unsupported type: {fieldInfo.FieldType.FullName}");
                }
        }
    }
}
