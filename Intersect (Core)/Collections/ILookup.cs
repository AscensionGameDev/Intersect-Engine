using System;
using System.Collections.Generic;

namespace Intersect.Collections
{

    public interface ILookup<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {

        Type KeyType { get; }

        Type ValueType { get; }

        int Count { get; }

        IDictionary<TKey, TValue> Clone { get; }

        ICollection<KeyValuePair<TKey, TValue>> Pairs { get; }

        ICollection<TKey> Keys { get; }

        ICollection<TValue> Values { get; }

        TValue Get(TKey key);

        TObject Get<TObject>(TKey key) where TObject : TValue;

        bool TryGetValue<TObject>(TKey key, out TObject value) where TObject : TValue;

        bool TryGetValue(TKey key, out TValue value);

        bool Add(TValue value);

        TValue AddNew(Type type, TKey key);

        bool Set(TKey key, TValue value);

        bool Delete(TValue value);

        bool DeleteAt(TKey key);

        void Clear();

    }

}
