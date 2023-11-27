using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
    }
}
