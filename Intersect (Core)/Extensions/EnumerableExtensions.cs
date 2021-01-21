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

    }

}
