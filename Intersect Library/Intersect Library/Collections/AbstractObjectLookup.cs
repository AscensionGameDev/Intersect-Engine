using System.Collections.Generic;
using System.Collections.ObjectModel;
using Intersect.GameObjects;
using Intersect.Logging;

namespace Intersect.Collections
{
    public abstract class AbstractObjectLookup<K, V> where V : IGameObject<K, V>
    {
        private Dictionary<K, V> mMutableMap;
        private ReadOnlyDictionary<K, V> mReadOnlyMap;

        public int Count { get { return mMutableMap.Count; } }
        public IDictionary<K, V> ReadOnlyMap { get { return mReadOnlyMap; } }
        public ICollection<K> Keys { get { return ReadOnlyMap.Keys; } }
        public ICollection<V> Objects { get { return ReadOnlyMap.Values; } }

        public AbstractObjectLookup()
        {
            this.mMutableMap = new Dictionary<K, V>();
            this.mReadOnlyMap = new ReadOnlyDictionary<K, V>(this.mMutableMap);
        }

        protected abstract bool Validate(K key);

        public V Get(K key)
        {
            V value;

            if (!Validate(key) || !mMutableMap.TryGetValue(key, out value))
            {
                return default(V);
            }

            return value;
        }

        public bool Add(V value)
        {
            if (value == null)
            {
                Log.Warn("Tried to add a null value to the collection.");
                return false;
            }

            return Add(value.Id, value);
        }

        public bool Add(K key, V value)
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

        public bool Delete(V value)
        {
            if (value != null)
            {
                return mMutableMap.Remove(value.Id);
            }

            return false;
        }

        public void Clear()
        {
            mMutableMap.Clear();
        }
    }
}
