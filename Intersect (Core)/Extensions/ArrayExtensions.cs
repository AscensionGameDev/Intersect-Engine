using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace Intersect.Extensions
{

    public static class ArrayExtensions
    {

        [NotNull]
        public static TType[] Prepend<TType>([NotNull] this TType[] values, [NotNull] params TType[] prependedValues)
        {
            return new TType[] { }.Concat(prependedValues).Concat(values).ToArray();
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }

    }

}
