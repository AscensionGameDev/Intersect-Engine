namespace Intersect.Framework.Collections;

public static class CollectionExtensions
{
    public static void AddRange<TValue>(this ICollection<TValue> collection, IEnumerable<TValue> values)
    {
        foreach (var value in values)
        {
            if (collection.Contains(value))
            {
                continue;
            }

            collection.Add(value);
        }
    }

    public static bool RemoveRange<TValue>(this ICollection<TValue> collection, IEnumerable<TValue> values) =>
        values.All(value => collection.Remove(value));
}
