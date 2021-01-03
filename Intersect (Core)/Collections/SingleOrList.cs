using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Collections
{

    public struct SingleOrList<TValue> : IList<TValue>
    {

        private readonly IList<TValue> mValues;

        public SingleOrList(params TValue[] values) : this(
            values as IEnumerable<TValue> ?? throw new InvalidOperationException()
        )
        {
        }

        public SingleOrList(IEnumerable<TValue> values)
        {
            mValues = values.ToList();
        }

        /// <inheritdoc />
        public IEnumerator<TValue> GetEnumerator()
        {
            return mValues.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(TValue item)
        {
            mValues.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            mValues.Clear();
        }

        /// <inheritdoc />
        public bool Contains(TValue item)
        {
            return mValues.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(TValue[] array, int arrayIndex)
        {
            mValues.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(TValue item)
        {
            return mValues.Remove(item);
        }

        /// <inheritdoc />
        public int Count => mValues.Count;

        /// <inheritdoc />
        public bool IsReadOnly => mValues.IsReadOnly;

        /// <inheritdoc />
        public int IndexOf(TValue item)
        {
            return mValues.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, TValue item)
        {
            mValues.Insert(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            mValues.RemoveAt(index);
        }

        /// <inheritdoc />
        public TValue this[int index]
        {
            get => mValues[index];
            set => mValues[index] = value;
        }

        public static implicit operator SingleOrList<TValue>(TValue value)
        {
            return new SingleOrList<TValue>(value);
        }

        public static implicit operator SingleOrList<TValue>(TValue[] values)
        {
            return new SingleOrList<TValue>(values);
        }

    }

}
