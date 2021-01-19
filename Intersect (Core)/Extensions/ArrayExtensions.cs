using System.Linq;

namespace Intersect.Extensions
{

    public static class ArrayExtensions
    {

        public static TType[] Prepend<TType>(this TType[] values, params TType[] prependedValues)
        {
            return new TType[] { }.Concat(prependedValues).Concat(values).ToArray();
        }

    }

}
