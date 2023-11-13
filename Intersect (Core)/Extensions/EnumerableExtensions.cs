using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using Intersect.Enums;
using Intersect.Models;
using static Intersect.Attributes.Attributes;

namespace Intersect.Extensions
{

    public static partial class EnumerableExtensions
    {

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> enumerable,
            Func<KeyValuePair<TKey, TValue>, bool> where = null
        )
        {
            var range = enumerable;
            if (where != null)
            {
                range = range.Where(where);
            }

            return range.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        // Not a very clean solution, but will do for now.
        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector, CancellationToken cancellationToken = default)
        {
            if (cancellationToken == default || cancellationToken.IsCancellationRequested)
            {
                return Enumerable.Empty<T>();
            }

            var result = source.SelectMany(selector);
            if (cancellationToken.IsCancellationRequested)
            {
                return Enumerable.Empty<T>();
            }

            return result.Any() ? result.Concat(result.SelectManyRecursive(selector, cancellationToken)) : result;
        }

        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return string.Empty;
        }

        public static string[] GetDescriptions(Type enumType, string except = null)
        {
            return Enum.GetValues(enumType)
                .Cast<Enum>()
                .Select(value => value.GetDescription())
                .Where(value => string.IsNullOrEmpty(except) || value != except)
                .ToArray();
        }

        public static GameObjectType GetRelatedTable(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    RelatedTable attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(RelatedTable)) as RelatedTable;
                    if (attr != null)
                    {
                        return attr.TableType;
                    }
                }
            }
            throw new ArgumentException($"{nameof(value)} did not have a valid RelatedTable attribute to pull from");
        }

        public static VariableType GetRelatedVariableType(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    RelatedVariableType attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(RelatedVariableType)) as RelatedVariableType;
                    if (attr != null)
                    {
                        return attr.VariableType;
                    }
                }
            }
            throw new ArgumentException($"{nameof(value)} did not have a valid RelatedVariableType attribute to pull from");
        }
    }
}
