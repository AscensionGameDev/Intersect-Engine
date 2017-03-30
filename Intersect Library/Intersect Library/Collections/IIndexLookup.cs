using System;
using System.Collections.Generic;
using Intersect.Models;

namespace Intersect.Collections
{
    public interface IIndexLookup<TValue> : IGameObjectLookup<TValue> where TValue : IIndexedGameObject
    {
        Type IndexKeyType { get; }
        IDictionary<int, TValue> IndexClone { get; }
        ICollection<KeyValuePair<int, TValue>> IndexPairs { get; }
        ICollection<int> IndexKeys { get; }
        ICollection<TValue> IndexValues { get; }

        int NextIndex { get; }
        TValue Get(int index);
        TObject Get<TObject>(int index) where TObject : TValue;
        bool TryGetValue(int index, out TValue value);
        bool TryGetValue<TObject>(int index, out TObject value) where TObject : TValue;
        TValue AddNew(Type type, int index);
        TValue AddNew(Type type, Guid key, int index);
        bool Set(int index, TValue value);
        bool DeleteAt(int key);
    }

    public interface IIndexLookup : IIndexLookup<IIndexedGameObject>
    {

    }
}