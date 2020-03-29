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

    }

}
