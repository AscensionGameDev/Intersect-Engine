using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Intersect.Collections
{

    [SuppressMessage("ReSharper", "JoinNullCheckWithUsage")]
    [SuppressMessage("ReSharper", "ArrangeAccessorOwnerBody")]
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {

        private readonly IDictionary<TKey, TValue> mInternalDictionary;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> internalDictionary)
        {
            mInternalDictionary = internalDictionary ?? throw new ArgumentNullException();
        }

        public int Count => mInternalDictionary?.Count ?? 0;

        public bool IsReadOnly => true;

        public ICollection<TKey> Keys => mInternalDictionary?.Keys ?? new TKey[0];

        public ICollection<TValue> Values => mInternalDictionary?.Values ?? new TValue[0];

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return mInternalDictionary?.GetEnumerator() ?? new Dictionary<TKey, TValue>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TValue this[TKey key]
        {
            get
            {
                Debug.Assert(mInternalDictionary != null, "mInternalDictionary != null");

                return mInternalDictionary[key];
            }
            set { throw new NotSupportedException(); }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Debug.Assert(mInternalDictionary != null, "mInternalDictionary != null");

            return mInternalDictionary.TryGetValue(key, out value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return mInternalDictionary?.Contains(item) ?? false;
        }

        public bool ContainsKey(TKey key)
        {
            return mInternalDictionary?.ContainsKey(key) ?? false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            mInternalDictionary?.CopyTo(array, arrayIndex);
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
