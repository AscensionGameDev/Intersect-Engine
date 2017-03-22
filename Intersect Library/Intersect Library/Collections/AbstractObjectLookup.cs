using System.Collections.Generic;
using System.Collections.ObjectModel;
using Intersect.GameObjects;
using Intersect.Logging;

namespace Intersect.Collections
{
    public abstract class AbstractObjectLookup<TKey, TValue> where TValue : IGameObject<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> mMutableMap;

        public int Count => mMutableMap.Count;
        public IDictionary<TKey, TValue> ReadOnlyMap { get; }
        public ICollection<KeyValuePair<TKey, TValue>> Pairs => ReadOnlyMap;
        public ICollection<TKey> Keys => ReadOnlyMap.Keys;
        public ICollection<TValue> Values => ReadOnlyMap.Values;

        protected AbstractObjectLookup()
        {
            mMutableMap = new Dictionary<TKey, TValue>();
            ReadOnlyMap = new ReadOnlyDictionary<TKey, TValue>(mMutableMap);
        }

        protected abstract bool Validate(TKey key);

        public TValue Get(TKey key)
        {
            if (Validate(key) && !mMutableMap.TryGetValue(key, out TValue value))
            {
                return value;
            }

            return default(TValue);
        }

        public bool Add(TValue value)
        {
            if (value != null) return Add(value.Id, value);
            Log.Warn("Tried to add a null value to the collection.");
            return false;
        }

        public bool Add(TKey key, TValue value)
        {
            if (value == null)
            {
                Log.Warn("Tried to add a null value to the collection.");
                return false;
            }

            if (!Validate(key))
            {
                return false;
            }

            if (mMutableMap.ContainsKey(key))
            {
                Log.Warn("Collection already contains object with key '{0}'.", key);
                //return false;
                /* TODO: This really should return false and refuse to add, but the code this is replacing currently relies on us replacing it */
            }

            mMutableMap[key] = value;
            return true;
        }

        public bool Delete(TValue value)
        {
            return value != null && mMutableMap.Remove(value.Id);
        }

        public void Clear()
        {
            mMutableMap.Clear();
        }
    }
}
