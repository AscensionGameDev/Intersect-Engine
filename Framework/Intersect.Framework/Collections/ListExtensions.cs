namespace Intersect.Framework.Collections;

public static class ListExtensions
{
    public static void AddSorted<T>(this List<T> @this, T item) where T : IComparable<T>
    {
        if (@this.Count == 0)
        {
            @this.Add(item);
            return;
        }

        if (@this[^1].CompareTo(item) <= 0)
        {
            @this.Add(item);
            return;
        }

        if (@this[0].CompareTo(item) >= 0)
        {
            @this.Insert(0, item);
            return;
        }

        int index = @this.BinarySearch(item);
        if (index < 0)
        {
            index = ~index;
        }
        @this.Insert(index, item);
    }

    public static void Resort<T>(this List<T> @this, T item) where T : IComparable<T>
    {
        if (@this.Count < 1)
        {
            @this.AddSorted(item);
            return;
        }

        @this.Remove(item);
        @this.AddSorted(item);
    }
}