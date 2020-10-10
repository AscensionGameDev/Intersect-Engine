using System;
using System.Linq;

namespace Intersect.Extensions
{
    public static class ArrayExtensions
    {
        public static TType[] Prepend<TType>(this TType[] values, params TType[] prependedValues) =>
            Array.Empty<TType>().Concat(prependedValues).Concat(values).ToArray();

        public static void ValidateTypes(this object[] values, params Type[] types)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Length != types.Length)
            {
                var expectedTypes = string.Join(", ", types.Select(type => type.Name));
                var receivedTypes = string.Join(", ", values.Select(value => value?.GetType()?.Name ?? "null"));
                throw new ArgumentException(
                    $"Expected {types.Length} values, received {values.Length} (expected types: {expectedTypes}, received types: {receivedTypes})"
                );
            }
        }
    }
}
