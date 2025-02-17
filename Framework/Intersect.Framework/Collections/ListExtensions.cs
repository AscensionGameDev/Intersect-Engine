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

    private sealed class KeyComparer<TItem, TKey>(Func<TItem?, TKey?> keySelector)
        : IComparer<TItem> where TKey : IComparable<TKey>
    {
        public int Compare(TItem? x, TItem? y)
        {
            var keyX = keySelector(x);
            var keyY = keySelector(y);

            if (keyX is not null)
            {
                return keyX.CompareTo(keyY);
            }

            if (keyY is null)
            {
                return 0;
            }

            return -keyY.CompareTo(keyX);
        }
    }

    public static void AddSorted<TItem, TKey>(this List<TItem> @this, TItem item, Func<TItem?, TKey?> keySelector) where TKey : IComparable<TKey>
    {
        if (@this.Count == 0)
        {
            @this.Add(item);
            return;
        }

        KeyComparer<TItem, TKey> comparer = new(keySelector);

        if (comparer.Compare(@this[^1], item) <= 0)
        {
            @this.Add(item);
            return;
        }

        if (comparer.Compare(@this[0], item) >= 0)
        {
            @this.Insert(0, item);
            return;
        }

        int index = @this.BinarySearch(item, comparer);
        if (index < 0)
        {
            index = ~index;
        }
        @this.Insert(index, item);
    }

    public static void Resort<TItem, TKey>(this List<TItem> @this, TItem item, Func<TItem?, TKey?> keySelector)
        where TKey : IComparable<TKey>
    {
        if (@this.Count < 1)
        {
            @this.AddSorted(item, keySelector);
            return;
        }

        @this.Remove(item);
        @this.AddSorted(item, keySelector);
    }
}