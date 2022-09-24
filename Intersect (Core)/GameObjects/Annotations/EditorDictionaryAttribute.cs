using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Intersect.Localization;

#if !DEBUG
using Intersect.Logging;
#endif

namespace Intersect.GameObjects.Annotations
{
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
                Log.Error(error);
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
                        if (!(value is string key))
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
                    throw new InvalidOperationException($"Unsupported type: {fieldInfo.FieldType.FullName}");
            }
        }
    }
}
