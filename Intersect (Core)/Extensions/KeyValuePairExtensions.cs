using System.Collections.Generic;

namespace Intersect.Extensions
{
    public static partial class KeyValuePairExtensions
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue val)
        {
            key = pair.Key;
            val = pair.Value;
        }
    }
}
