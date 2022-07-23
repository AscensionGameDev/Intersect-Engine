using System;
using System.Linq;
using System.Reflection;

using Intersect.Collections;
using Intersect.GameObjects.Maps;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Models;

namespace Intersect.GameObjects.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EditorReferenceAttribute : EditorDisplayAttribute
    {
        private readonly PropertyInfo _referenceProperty;

        public EditorReferenceAttribute(Type descriptorType, string referenceName)
        {
            DescriptorType = descriptorType ?? throw new ArgumentNullException(nameof(descriptorType));
            ReferenceName = !string.IsNullOrEmpty(referenceName) ? referenceName : throw new ArgumentNullException(nameof(referenceName));

            if (DescriptorType.BaseType.GetGenericTypeDefinition() != typeof(DatabaseObject<>))
            {
                throw new ArgumentException($"Invalid descriptor type '{DescriptorType.FullName}'.", nameof(descriptorType));
            }

            _referenceProperty = DescriptorType.GetProperty(ReferenceName);
            if (_referenceProperty == default)
            {
                throw new ArgumentException($"'{referenceName}' does not exist on {descriptorType.FullName}.", nameof(referenceName));
            }
        }

        public Type DescriptorType { get; }

        public string ReferenceName { get; }

        public override string Format(Type stringsType, object value)
        {
            if (!(value is Guid guid))
            {
                throw new ArgumentException($"Invalid value type {value?.GetType().FullName}", nameof(value));
            }

            if (DescriptorType == typeof(MapBase))
            {
                var mapName = MapList.OrderedMaps.FirstOrDefault(map => map.MapId == guid)?.Name;
                if (mapName != default)
                {
                    return mapName;
                }
            }
            else
            {
                var lookup = DescriptorType
                .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .FirstOrDefault(propertyInfo => propertyInfo.PropertyType == typeof(DatabaseObjectLookup))?
                .GetValue(null) as DatabaseObjectLookup;

                if (lookup == default)
                {
                    throw new InvalidOperationException();
                }

                if (lookup.TryGetValue(guid, out IDatabaseObject databaseObject))
                {
                    return databaseObject.Name;
                }
            }

            if (stringsType == default)
            {
                throw new ArgumentNullException(nameof(stringsType));
            }

            var noneFieldInfo = stringsType.GetNestedType("General")?.GetField("None");
            if (noneFieldInfo == default)
            {
                throw new InvalidOperationException();
            }

            return noneFieldInfo?.GetValue(null)?.ToString();
        }
    }
}
