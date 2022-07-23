using System;
using System.Reflection;

using Intersect.Localization;

namespace Intersect.GameObjects.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class EditorLabelAttribute : Attribute
    {
        public EditorLabelAttribute(string name)
        {
            Group = default;
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentNullException(nameof(name));
        }

        public EditorLabelAttribute(string group, string name) : this(name)
        {
            Group = !string.IsNullOrEmpty(group) ? group : throw new ArgumentNullException(nameof(group));
        }

        public string Group { get; }

        public string Name { get; }

        [Obsolete("We want to re-implement strings to be object-oriented.")]
        public string Evaluate(Type stringsType)
        {
            if (stringsType == default)
            {
                throw new ArgumentNullException(nameof(stringsType));
            }

            var groupType = stringsType.GetNestedType(Group, BindingFlags.Public | BindingFlags.Static);
            if (groupType == default)
            {
                throw new InvalidOperationException($"'{stringsType.FullName}.{Group}' does not exist.");
            }

            var fieldInfo = groupType.GetField(Name);
            if (fieldInfo == default)
            {
                throw new InvalidOperationException($"'{groupType.FullName}.{Name}' does not exist.");
            }

            return fieldInfo.GetValue(null)?.ToString();
        }
    }
}
