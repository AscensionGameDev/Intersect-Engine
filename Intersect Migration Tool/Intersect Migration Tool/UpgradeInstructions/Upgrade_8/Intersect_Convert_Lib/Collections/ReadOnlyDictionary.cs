using System;
using System.Collections;
using System.Collections.Generic;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_8.Intersect_Convert_Lib.Collections
{
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> mInternalDictionary;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> internalDictionary)
        {
            if (internalDictionary == null) throw new ArgumentNullException();

            mInternalDictionary = internalDictionary;
        }

        public int Count => mInternalDictionary.Count;
        public bool IsReadOnly => true;
        public ICollection<TKey> Keys => mInternalDictionary.Keys;
        public ICollection<TValue> Values => mInternalDictionary.Values;

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => mInternalDictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public TValue this[TKey key]
        {
            get { return mInternalDictionary[key]; }
            set { throw new NotSupportedException(); }
        }

        public bool TryGetValue(TKey key, out TValue value) => mInternalDictionary.TryGetValue(key, out value);

        public bool Contains(KeyValuePair<TKey, TValue> item) => mInternalDictionary.Contains(item);

        public bool ContainsKey(TKey key) => mInternalDictionary.ContainsKey(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            mInternalDictionary.CopyTo(array, arrayIndex);
        }

        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }
    }
}