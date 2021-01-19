using System.Collections;
using System.Collections.Generic;

namespace Intersect.Collections
{
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private IList<T> backingList;

        public T this[int index] => backingList[index];

        public int Count => backingList.Count;

        public IEnumerator<T> GetEnumerator() => backingList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => backingList.GetEnumerator();

        public ReadOnlyList(IList<T> backingList) =>
            this.backingList = backingList;
    }

    public static class ListExtensions
    {
        public static IReadOnlyList<T> WrapReadOnly<T>(this IList<T> list) =>
            new ReadOnlyList<T>(list);
    }
}
