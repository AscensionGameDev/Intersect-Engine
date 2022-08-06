using System.Collections;

namespace Intersect.Collections;

public sealed class ConverterList<TWrapped, TExposed> : IList<TExposed>
{
    private readonly List<TWrapped> _wrappedList;
    private readonly bool _enforceSorting;
    private readonly bool _enforceUniqueness;
    private readonly Func<TWrapped, TExposed, bool>? _equalityComparer;
    private readonly Func<TWrapped, TExposed> _retrievalConversion;
    private readonly Func<TExposed, TWrapped> _storageConversion;

    public ConverterList(
        List<TWrapped> wrappedList,
        Func<TWrapped, TExposed> retrievalConversion,
        Func<TExposed, TWrapped> storageConversion,
        Func<TWrapped, TExposed, bool>? equalityComparer = default,
        bool enforceSorting = false,
        bool enforceUniqueness = true
    )
    {
        _enforceSorting = enforceSorting;
        _enforceUniqueness = enforceUniqueness;
        _equalityComparer = equalityComparer;
        _retrievalConversion = retrievalConversion ?? throw new ArgumentNullException(nameof(retrievalConversion));
        _storageConversion = storageConversion ?? throw new ArgumentNullException(nameof(storageConversion));
        _wrappedList = wrappedList ?? throw new ArgumentNullException(nameof(wrappedList));
    }

    /// <inheritdoc/>
    public TExposed this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    /// <inheritdoc/>
    public int Count => _wrappedList.Count;

    bool ICollection<TExposed>.IsReadOnly => (_wrappedList as ICollection<TWrapped>).IsReadOnly;

    /// <inheritdoc/>
    public void Add(TExposed item)
    {
        var convertedItem = _storageConversion(item);

        if (_enforceUniqueness && _wrappedList.Contains(convertedItem))
        {
            return;
        }

        _wrappedList.Add(convertedItem);
    }

    public void AddRange(IEnumerable<TExposed> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    /// <inheritdoc/>
    public void Clear() => _wrappedList.Clear();

    /// <inheritdoc/>
    public bool Contains(TExposed item)
    {
        if (_equalityComparer == default)
        {
            var convertedItem = _storageConversion(item);
            return _wrappedList.Contains(convertedItem);
        }

        return _wrappedList.Any(wrapped => _equalityComparer(wrapped, item));
    }

    /// <inheritdoc/>
    public void CopyTo(TExposed[] array, int arrayIndex)
    {
        _wrappedList
            .Select(_retrievalConversion)
            .ToList()
            .CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public IEnumerator<TExposed> GetEnumerator()
    {
        var retrievalEnumerable = _wrappedList
            .Select(_retrievalConversion);

        if (_enforceSorting)
        {
            retrievalEnumerable = retrievalEnumerable.OrderBy(item => item);
        }

        return retrievalEnumerable.GetEnumerator();
    }

    /// <inheritdoc/>
    public int IndexOf(TExposed item)
    {
        if (_equalityComparer == default)
        {
            var convertedItem = _storageConversion(item);
            return _wrappedList.IndexOf(convertedItem);
        }

        return _wrappedList.FindIndex(wrapped => _equalityComparer(wrapped, item));
    }

    /// <inheritdoc/>
    public void Insert(int index, TExposed item)
    {
        var convertedItem = _storageConversion(item);

        if (_enforceUniqueness && _wrappedList.Contains(convertedItem))
        {
            return;
        }

        _wrappedList.Insert(index, convertedItem);
    }

    public void InsertRange(int index, IEnumerable<TExposed> items)
    {
        var itemsToInsert = items;

        if (_enforceUniqueness)
        {
            itemsToInsert = itemsToInsert
                .Where(item => !Contains(item));
        }

        var convertedItems = itemsToInsert.Select(_storageConversion);

        _wrappedList.InsertRange(
            index,
            convertedItems
        );
    }

    /// <inheritdoc/>
    public bool Remove(TExposed item)
    {
        var wrapped = _storageConversion(item);
        return _wrappedList.Remove(wrapped);
    }

    /// <inheritdoc/>
    public void RemoveAt(int index) =>
        _wrappedList.RemoveAt(index);

    public void RemoveRange(int index, int count) =>
        _wrappedList.RemoveRange(index, count);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
