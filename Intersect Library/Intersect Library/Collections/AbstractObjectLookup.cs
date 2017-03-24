using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Intersect.GameObjects;
using Intersect.Logging;

namespace Intersect.Collections
{
    public abstract class AbstractObjectLookup<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TValue : IGameObject<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> mMutableMap;
        private readonly ConstructorInfo mConstructor;

        protected AbstractObjectLookup()
        {
            mMutableMap = new Dictionary<TKey, TValue>();
            ReadOnlyMap = new ReadOnlyDictionary<TKey, TValue>(mMutableMap);

            mConstructor = ValueType.GetConstructor(new[] {KeyType});
            if (mConstructor == null)
            {
                //throw new ArgumentNullException($"Missing constructor with parameter '{KeyType.Name}'.");
            }
        }

        public Type KeyType => typeof(TKey);
        public Type ValueType => typeof(TValue);
        public virtual int Count => mMutableMap?.Count ?? -1;
        public virtual IDictionary<TKey, TValue> ReadOnlyMap { get; }
        public virtual ICollection<KeyValuePair<TKey, TValue>> Pairs => ReadOnlyMap;
        public virtual ICollection<TKey> Keys => ReadOnlyMap?.Keys;
        public virtual ICollection<TValue> Values => ReadOnlyMap?.Values;

        protected abstract bool Validate(TKey key);

        public virtual TValue this[TKey key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        public virtual TValue Get(TKey key)
        {
            if (!Validate(key)) return default(TValue);

            if (mMutableMap == null)
            {
                throw new AccessViolationException("Internal map is null despite being set in the constructor.");
            }

            return mMutableMap.TryGetValue(key, out TValue value) ? value : default(TValue);
        }

        private bool Add(TValue value)
        {
            if (value != null) return Add(value.Id, value);
            Log.Warn("Tried to add a null value to the collection.");
            return false;
        }

        private bool Add(TKey key, TValue value)
        {
            if (value == null)
            {
                Log.Warn("Tried to add a null value to the collection.");
                return false;
            }

            if (!Validate(key)) return false;

            if (mMutableMap == null)
            {
                throw new AccessViolationException("Internal map is null despite being set in the constructor.");
            }

            if (!mMutableMap.ContainsKey(key)) return Set(key, value);

            Log.Error("Collection already contains object with key '{0}'.", key);
            return false;
        }

        private TValue AddNew(TKey key)
        {
            var value = (TValue) mConstructor?.Invoke(new object[] {key});
            if (value == null) throw new ArgumentNullException($"Failed to create instance of '{ValueType.Name}'.");
            return Add(key, value) ? value : default(TValue);
        }

        public virtual bool Set(TKey key, TValue value)
        {
            if (value == null)
            {
                Log.Warn("Tried to set a null value for key '{0}'.", key);
                return false;
            }

            if (!Validate(key)) return false;

            if (mMutableMap == null)
            {
                throw new AccessViolationException("Internal map is null despite being set in the constructor.");
            }

            mMutableMap[key] = value;
            return true;
        }

        public virtual bool Delete(TValue value) => value != null && (mMutableMap?.Remove(value.Id) ?? false);

        public virtual void Clear() => mMutableMap?.Clear();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            if (Pairs == null)
            {
                throw new AccessViolationException("Lookup pairs somehow null.");
            }

            return Pairs.GetEnumerator();
        }
    }
}