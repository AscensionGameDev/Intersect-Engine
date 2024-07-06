namespace Intersect.Extensions
{

    public static partial class ArrayExtensions
    {

        public static TType[] Prepend<TType>(this TType[] values, params TType[] prependedValues)
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

        public static int IndexOf<T>(this T[] array, T value) => Array.IndexOf(array, value);
    }

}
