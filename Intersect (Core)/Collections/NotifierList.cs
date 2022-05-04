using System.Collections;

namespace Intersect.Collections;

public interface INotifierListMutationObserver<TValue>
{
    void OnAdd(NotifierList<TValue> sender, ref TValue value);

    void OnClear(NotifierList<TValue> sender, TValue[] values);

    void OnInsert(NotifierList<TValue> sender, int index, ref TValue value);

    void OnRemove(NotifierList<TValue> sender, ref TValue value, bool result);

    void OnRemoveAt(NotifierList<TValue> sender, int index, ref TValue value);

    void OnSet(NotifierList<TValue> sender, int index, ref TValue value);
}

public class NotifierList<TValue> : ICollection<TValue>, IEnumerable<TValue>, IEnumerable, IList<TValue>, IReadOnlyCollection<TValue>, IReadOnlyList<TValue>, ICollection, IList
{
    private readonly INotifierListMutationObserver<TValue> _mutationObserver;
    private readonly List<TValue> _values;

    public NotifierList(INotifierListMutationObserver<TValue> mutationObserver)
        : this(mutationObserver, 0)
    {
    }

    public NotifierList(INotifierListMutationObserver<TValue> mutationObserver, int capacity)
    {
        _mutationObserver = mutationObserver ?? throw new ArgumentNullException(nameof(mutationObserver));
        _values = new List<TValue>(capacity);
    }

    public NotifierList(INotifierListMutationObserver<TValue> mutationObserver, params TValue[] values)
        : this(mutationObserver, values as IEnumerable<TValue>)
    {
    }

    public NotifierList(INotifierListMutationObserver<TValue> mutationObserver, IEnumerable<TValue> values)
    {
        _mutationObserver = mutationObserver ?? throw new ArgumentNullException(nameof(mutationObserver));
        _values = new List<TValue>(values);
    }

    public TValue this[int index]
    {
        get => _values[index];
        set
        {
            _values[index] = value;
            _mutationObserver.OnSet(this, index, ref value);
        }
    }

    object IList.this[int index]
    {
        get => this[index];
        set
        {
            if (value is not TValue typedValue)
            {
                throw new InvalidCastException(string.Format("Attempted to insert an incompatible value of type {0} into an IList<{1}>.", value?.GetType(), typeof(TValue)));
            }

            this[index] = typedValue;
        }
    }

    public int Count => _values.Count;

    public bool IsReadOnly => (_values as ICollection<TValue>).IsReadOnly;

    bool IList.IsFixedSize => (_values as IList).IsFixedSize;

    bool ICollection.IsSynchronized => (_values as ICollection).IsSynchronized;

    object ICollection.SyncRoot => (_values as ICollection).SyncRoot;

    public void Add(TValue item)
    {
        _values.Add(item);
        _mutationObserver.OnAdd(this, ref item);
    }

    public void Clear()
    {
        var values = _values.ToArray();
        _values.Clear();
        _mutationObserver.OnClear(this, values);
    }

    public bool Contains(TValue item) => _values.Contains(item);

    public void CopyTo(TValue[] array, int arrayIndex) => _values.CopyTo(array, arrayIndex);

    public IEnumerator<TValue> GetEnumerator() => _values.GetEnumerator();

    public int IndexOf(TValue item) => _values.IndexOf(item);

    public void Insert(int index, TValue item)
    {
        _values.Insert(index, item);
        _mutationObserver.OnInsert(this, index, ref item);
    }

    public bool Remove(TValue item)
    {
        var result = _values.Remove(item);
        _mutationObserver.OnRemove(this, ref item, result);
        return result;
    }

    public void RemoveAt(int index)
    {
        var value = _values[index];
        _values.RemoveAt(index);
        _mutationObserver.OnRemoveAt(this, index, ref value);
    }

    int IList.Add(object value)
    {
        if (value is TValue typedValue)
        {
            var index = (_values as IList).Add(value);
            _mutationObserver.OnAdd(this, ref typedValue);
            return index;
        }

        throw new ArgumentException(string.Format("Attempted to insert an incompatible value of type {0} into an IList<{1}>.", value?.GetType(), typeof(TValue)), nameof(value));
    }

    bool IList.Contains(object value) => (_values as IList).Contains(value);

    void ICollection.CopyTo(Array array, int index) => (_values as ICollection).CopyTo(array, index);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    int IList.IndexOf(object value) => (_values as IList).IndexOf(value);

    void IList.Insert(int index, object value)
    {
        if (value is TValue typedValue)
        {
            Insert(index, typedValue);
            return;
        }

        throw new ArgumentException(string.Format("Attempted to insert an incompatible value of type {0} into an IList<{1}>.", value?.GetType(), typeof(TValue)), nameof(value));
    }

    void IList.Remove(object value)
    {
        if (value is TValue typedValue)
        {
            _ = Remove(typedValue);
            return;
        }

        throw new ArgumentException(string.Format("Attempted to insert an incompatible value of type {0} into an IList<{1}>.", value?.GetType(), typeof(TValue)), nameof(value));
    }
}
