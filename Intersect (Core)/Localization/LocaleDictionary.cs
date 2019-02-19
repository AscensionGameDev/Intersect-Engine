using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Localization
{
    public class LocaleDictionary<TKey, TValue> : Localized, IDictionary<TKey, TValue>
        where TKey : IComparable<TKey>
        where TValue : Localized
    {
        [NotNull] private readonly IDictionary<TKey, TValue> mDefaults;

        [NotNull] private readonly IDictionary<TKey, TValue> mValues;

        public LocaleDictionary(
            [NotNull] IDictionary<TKey, TValue> defaults,
            [CanBeNull] IDictionary<TKey, TValue> values = null
        )
        {
            mDefaults = new SortedDictionary<TKey, TValue>(defaults);
            mValues = values == null
                ? new SortedDictionary<TKey, TValue>()
                : new SortedDictionary<TKey, TValue>(values);
        }

        public TValue this[TKey key]
        {
            get => mValues.TryGetValue(key, out var backingValue)
                ? backingValue
                : mDefaults[key];

            set => mValues[key] = value ?? mDefaults[key];
        }

        public int Count => mDefaults.Count;

        public bool IsReadOnly => true;

        [NotNull]
        private ICollection<KeyValuePair<TKey, TValue>> Pairs =>
            Keys.Select(key =>
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (mValues.TryGetValue(key, out var value) ||
                    mDefaults.TryGetValue(key, out value))
                {
                    return new KeyValuePair<TKey, TValue>(key, value);
                }

                throw new InvalidOperationException();
            }).ToList();

        public ICollection<TKey> Keys => mDefaults.Keys;

        public ICollection<TValue> Values => Keys.Select(key =>
        {
            if (key == null)
            {
                throw new InvalidOperationException();
            }

            return this[key];
        }).ToList();

        public bool ContainsKey(TKey key)
        {
            return mDefaults.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            mValues.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            return mValues.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return mValues.TryGetValue(key, out value) || mDefaults.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            mValues.Add(item);
        }

        public void Clear()
        {
            mValues.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return mValues.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Pairs.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return mValues.Remove(item);
        }
    }
}