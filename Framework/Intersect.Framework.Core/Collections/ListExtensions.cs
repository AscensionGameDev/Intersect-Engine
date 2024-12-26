namespace Intersect.Collections;

public static partial class ListExtensions
{
    public static IReadOnlyList<T> WrapReadOnly<T>(this IList<T> list) =>
        new ReadOnlyList<T>(list);
}