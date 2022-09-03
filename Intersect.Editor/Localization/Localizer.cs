using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Intersect.GameObjects.Annotations;
using Intersect.Localization;
using Intersect.Logging;

namespace Intersect.Editor.Localization
{
    public class EditorProperty : IComparable<EditorProperty>
    {
        public EditorProperty(PropertyInfo propertyInfo, EditorDisplayAttribute displayAttribute, EditorLabelAttribute labelAttribute)
        {
            DisplayAttribute = displayAttribute ?? throw new ArgumentNullException(nameof(displayAttribute));
            LabelAttribute = labelAttribute ?? throw new ArgumentNullException(nameof(labelAttribute));
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
        }

        public EditorDisplayAttribute DisplayAttribute { get; }

        public EditorLabelAttribute LabelAttribute { get; }

        public PropertyInfo PropertyInfo { get; }

        public int CompareTo(EditorProperty other)
        {
            if (other == default)
            {
                return -1;
            }

            var thisFieldType = DisplayAttribute.FieldType;
            var otherFieldType = other.DisplayAttribute.FieldType;

            if (thisFieldType == otherFieldType)
            {
                return string.Compare(PropertyInfo.Name, other.PropertyInfo.Name, StringComparison.Ordinal);
            }

            return thisFieldType == EditorFieldType.Pivot ? -1 : 1;
        }

        public object GetValue(object parent) => PropertyInfo.GetValue(parent);

        public T GetValue<T>(object parent) => PropertyInfo.GetValue(parent) is T value ? value : default;

        public static bool TryCreate(PropertyInfo propertyInfo, out EditorProperty editorProperty)
        {
            if (propertyInfo == default)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var attributes = Attribute.GetCustomAttributes(propertyInfo, true);
            var labelAttribute = attributes.FirstOrDefault(attribute => attribute is EditorLabelAttribute) as EditorLabelAttribute;
            var displayAttribute = attributes.FirstOrDefault(attribute => attribute is EditorDisplayAttribute) as EditorDisplayAttribute;

            if (labelAttribute == default || displayAttribute == default)
            {
                // TODO: Re-enable this, disabled for spamming the logs
                // Log.Warn($"{propertyInfo.DeclaringType.FullName}.{propertyInfo.Name} must have both a label and display attribute.");
                editorProperty = default;
                return false;
            }

            editorProperty = new EditorProperty(
                displayAttribute: displayAttribute,
                labelAttribute: labelAttribute,
                propertyInfo: propertyInfo
            );

            return true;
        }
    }

    public class Localizer
    {
        private readonly List<Assembly> _indexedAssemblies;
        private readonly List<Type> _indexedTypes;
        private readonly Dictionary<Type, List<EditorProperty>> _indexedEditorProperties;

        public Localizer()
        {
            _indexedAssemblies = new List<Assembly>();
            _indexedTypes = new List<Type>();
            _indexedEditorProperties = new Dictionary<Type, List<EditorProperty>>();

            IndexAssembly(typeof(Localized).Assembly);
        }

        public void IndexAssembly(Assembly assembly)
        {
            if (assembly == default)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (_indexedAssemblies.Contains(assembly))
            {
                Log.Debug($"Skipping re-index of {assembly.FullName}");
                return;
            }

            foreach (var type in assembly.GetTypes())
            {
                if (_indexedTypes.Contains(type))
                {
                    Log.Debug($"Skipping re-index of {type.FullName}");
                    continue;
                }

                var editorPropertiesQuery = type
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .SelectMany(
                        propertyInfo => EditorProperty.TryCreate(propertyInfo, out var editorProperty)
                            ? new[] { editorProperty }
                            : Array.Empty<EditorProperty>()
                    );

                _indexedEditorProperties[type] = editorPropertiesQuery.ToList();
            }
        }

        [Obsolete("We want to re-implement strings to be object-oriented.")]
        public List<KeyValuePair<string, string>> Localize(Type stringsType, object value)
        {
            if (value == default || !_indexedEditorProperties.TryGetValue(value.GetType(), out var editorProperties))
            {
                return new List<KeyValuePair<string, string>>();
            }

            return editorProperties
                .Select(editorProperty =>
                {
                    var labelAttribute = editorProperty.LabelAttribute;
                    var label = labelAttribute.Evaluate(typeof(Strings));

                    var displayAttribute = editorProperty.DisplayAttribute;
                    var propertyValue = editorProperty.GetValue(value);
                    var displayValue = displayAttribute.Format(typeof(Strings), propertyValue);
                    return new KeyValuePair<string, string>(label, displayValue);
                })
                .ToList();
        }
    }
}
