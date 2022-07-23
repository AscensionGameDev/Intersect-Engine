using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Intersect.Localization;

#if !DEBUG
using Intersect.Logging;
#endif

namespace Intersect.GameObjects.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EditorFormattedAttribute : EditorDisplayAttribute
    {
        public EditorFormattedAttribute(string name)
        {
            Group = default;
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
        }

        public EditorFormattedAttribute(string group, string name) : this(name)
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
                .GetNestedTypes(BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .FirstOrDefault(type => type.Name == Group);
            var parentType = (groupType ?? stringsType);
            var members = parentType.GetMember(Name);

            if (members.Length == 0)
            {
                var error = new InvalidOperationException($"{parentType.FullName}.{Name} does not exist.");
#if DEBUG
                throw error;
#else
                Log.Error(error);
                return base.Format(stringsType, value);
#endif
            }

            var firstMember = members.First();
            switch (firstMember)
            {
                case MethodInfo _:
                    return InvokeFormatterMethod(members.Cast<MethodInfo>(), value)?.ToString();

                case FieldInfo fieldInfo:
                    if (fieldInfo.FieldType != typeof(LocalizedString))
                    {
                        throw new InvalidOperationException($"Expected {typeof(LocalizedString).FullName} but the field is {fieldInfo.FieldType.FullName}");
                    }
                    return (fieldInfo.GetValue(null) as LocalizedString ?? throw new InvalidOperationException()).ToString(value);

                default:
                    throw new InvalidOperationException($"Unsupported member type {firstMember.MemberType}: {firstMember.DeclaringType.FullName}.{firstMember.Name}");
            }
        }

        private static object InvokeFormatterMethod(IEnumerable<MethodInfo> methodInfos, object value)
        {
            var formatterMethodInfo = methodInfos
                .OrderBy(methodInfo => methodInfo.GetParameters().Length)
                .FirstOrDefault(methodInfo => methodInfo.GetParameters().Length == 1 || methodInfo.GetParameters().Count(parameter => !parameter.HasDefaultValue) == 1);

            if (formatterMethodInfo == default)
            {
                var firstMember = methodInfos.First();
                throw new InvalidOperationException($"No override that has 1 required parameter exists for {firstMember.DeclaringType.FullName}.{firstMember.Name}");
            }

            if (formatterMethodInfo.GetParameters().Length == 1)
            {
                return formatterMethodInfo.Invoke(default, new[] { value });
            }

            return formatterMethodInfo.Invoke(
                default,
                new[] { value }.Concat(
                    formatterMethodInfo.GetParameters()
                    .Where(parameter => parameter.HasDefaultValue)
                    .Select(parameter => parameter.RawDefaultValue))
                .ToArray()
            );
        }
    }
}
