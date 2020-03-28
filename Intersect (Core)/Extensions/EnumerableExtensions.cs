using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace Intersect.Extensions
{

    public static class EnumerableExtensions
    {

        [NotNull]
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
            [NotNull] this IEnumerable<KeyValuePair<TKey, TValue>> enumerable,
            [CanBeNull] Func<KeyValuePair<TKey, TValue>, bool> where = null
        )
        {
            var range = enumerable;
            if (where != null)
            {
                range = range.Where(where);
            }

            return range.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

    }

}
