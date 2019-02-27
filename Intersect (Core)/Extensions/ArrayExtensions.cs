using JetBrains.Annotations;
using System.Linq;

namespace Intersect.Extensions
{
    public static class ArrayExtensions
    {
        public static TType[] Prepend<TType>([NotNull] this TType[] values, [NotNull] params TType[] prependedValues)
        {
            return new TType[] { }.Concat(prependedValues).Concat(values).ToArray();
        }
    }
}
