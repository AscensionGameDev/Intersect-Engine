using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Extensions
{

    public static class EnumerableExtensions
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
        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            var result = source.SelectMany(selector);
            if (!result.Any())
            {
                return result;
            }
            return result.Concat(result.SelectManyRecursive(selector));
        }
    }
}
