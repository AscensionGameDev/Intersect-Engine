using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Utilities
{

    public static class ValueUtils
    {

        public static void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }

        public static bool SetDefault<T>(bool condition, out T value)
        {
            value = default(T);

            return condition;
        }

        /// <summary>
        /// Computes the aggregate hash code for <paramref name="values"/>.
        /// </summary>
        /// <param name="values">the set of values</param>
        /// <returns>the aggregate hash code</returns>
        public static int ComputeHashCode(params object[] values) => ComputeHashCode<object>(values);

        /// <summary>
        /// Computes the aggregate hash code for <paramref name="values"/>.
        /// </summary>
        /// <typeparam name="TValue">the value type</typeparam>
        /// <param name="values">the enumerable set of values</param>
        /// <returns>the aggregate hash code</returns>
        public static int ComputeHashCode<TValue>(IEnumerable<TValue> values) =>
            values?.Aggregate(
                0, (current, value) => unchecked(current * (int) 0xA5555529 + (value?.GetHashCode() ?? 0))
            ) ??
            0;

        /// <summary>
        /// Compares two <see cref="IEnumerable{T}"/> if <see cref="string"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="stringComparison">the <see cref="StringComparison"/> mode to use</param>
        /// <returns>the comparison between the two <see cref="string"/> enumerables</returns>
        public static int Compare(
            IEnumerable<string> a,
            IEnumerable<string> b,
            StringComparison stringComparison = StringComparison.CurrentCulture
        )
        {
            if (a == null)
            {
                return b == null ? 0 : -1;
            }

            if (b == null)
            {
                return 1;
            }

            int comparison;

            using (var enumeratorA = a.GetEnumerator())
            {
                using (var enumeratorB = b.GetEnumerator())
                {
                    do
                    {
                        var aHas = enumeratorA.MoveNext();
                        var bHas = enumeratorB.MoveNext();

                        if (!aHas)
                        {
                            return bHas ? -1 : 0;
                        }

                        if (!bHas)
                        {
                            return 1;
                        }

                        comparison = string.Compare(enumeratorA.Current, enumeratorB.Current, stringComparison);
                    } while (comparison == 0);
                }
            }

            return comparison;
        }

    }

}
