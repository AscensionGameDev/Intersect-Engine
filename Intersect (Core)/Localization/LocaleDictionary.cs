using System.Collections;

namespace Intersect.Localization;

public partial class LocaleDictionary<TKey, TValue> : Localized, IDictionary<TKey, TValue> where TValue : Localized
{
    private readonly IDictionary<TKey, TValue> _defaults;

    private readonly IDictionary<TKey, TValue> _overrides;

    private bool _frozen;

    public LocaleDictionary(
        IEnumerable<KeyValuePair<TKey, TValue>> defaults = null,
        IEnumerable<KeyValuePair<TKey, TValue>> overrides = null
    )
    {
        _defaults = defaults == null
            ? new SortedDictionary<TKey, TValue>()
            : new SortedDictionary<TKey, TValue>(
                defaults is IDictionary<TKey, TValue> dictionaryDefaults
                    ? dictionaryDefaults
                    : defaults.ToDictionary(pair => pair.Key, pair => pair.Value)
            );

        _overrides = overrides == null
            ? new SortedDictionary<TKey, TValue>()
            : new SortedDictionary<TKey, TValue>(
                overrides is IDictionary<TKey, TValue> dictionaryValues
                    ? dictionaryValues
                    : overrides.ToDictionary(pair => pair.Key, pair => pair.Value)
            );
    }

    private ICollection<KeyValuePair<TKey, TValue>> Pairs => Keys
        .Select(key =>
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (_overrides.TryGetValue(key, out var value) || _defaults.TryGetValue(key, out value))
                {
                    return new KeyValuePair<TKey, TValue>(key, value);
                }

                throw new InvalidOperationException();
            }
        )
        .ToList();

    public TValue this[TKey key]
    {
        get => _overrides.TryGetValue(key, out var backingValue) ? backingValue : _defaults[key];

        set
        {
            var target = (_frozen || _defaults.ContainsKey(key)) ? _overrides : _defaults;
            target[key] = value;
        }
    }

    public int Count => _defaults.Count;

    public bool IsReadOnly => true;

    public ICollection<TKey> Keys => _defaults.Keys;

    public ICollection<TValue> Values => Keys
        .Select(key => this[key ?? throw new InvalidOperationException()])
        .ToList();

    public void Add(TKey key, TValue value)
    {
        var source = (_frozen || ContainsKey(key)) ? _overrides : _defaults;
        source.Add(key, value);
    }

    public bool ContainsKey(TKey key) => _defaults.ContainsKey(key);

    public LocaleDictionary<TKey, TValue> Freeze()
    {
        _frozen = true;
        return this;
    }

    public bool Remove(TKey key) =>
        (_frozen || _defaults.Remove(key)) && _overrides.Remove(key);

    public bool TryGetValue(TKey key, out TValue value) =>
        _overrides.TryGetValue(key, out value) || _defaults.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Pairs.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(KeyValuePair<TKey, TValue> item) =>
        Add(item.Key, item.Value);

    public void Clear()
    {
        _overrides.Clear();

        if (!_frozen)
        {
            _defaults.Clear();
        }
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (_overrides.Contains(item))
        {
            return true;
        }

        return !_overrides.ContainsKey(item.Key) && _defaults.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
        Pairs.CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<TKey, TValue> item) =>
        (_frozen || _defaults.Remove(item)) && _overrides.Remove(item);
}
