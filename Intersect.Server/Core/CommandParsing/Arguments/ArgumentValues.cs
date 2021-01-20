using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Intersect.Server.Core.CommandParsing.Arguments
{

    public sealed class ArgumentValues : IEnumerable<object>
    {

        private readonly IList<object> mValues;

        public ArgumentValues(string argumentName, params object[] values) : this(
            argumentName, values.AsEnumerable()
        )
        {
        }

        public ArgumentValues(string argumentName, bool isImplicit, params object[] values) :
            this(argumentName, values.AsEnumerable(), isImplicit)
        {
        }

        public ArgumentValues(
            string argumentName,
            IEnumerable<object> values = null,
            bool isImplicit = false
        )
        {
            ArgumentName = argumentName;
            mValues = new List<object>(values ?? Array.Empty<object>());
            IsImplicit = isImplicit;
        }

        public string ArgumentName { get; }

        public object Value => mValues.FirstOrDefault();

        public IList<object> Values => mValues.ToImmutableList() ?? throw new InvalidOperationException();

        public bool IsEmpty => mValues.Count < 1;

        public bool IsImplicit { get; }

        public IEnumerator<object> GetEnumerator()
        {
            return mValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TValue ToTypedValue<TValue>(int index = 0)
        {
            if (mValues.ElementAtOrDefault(index) is TValue typedValue)
            {
                return typedValue;
            }

            return default(TValue);
        }

        public IList<TValue> ToTypedValues<TValue>()
        {
            return mValues.Select(value => value is TValue typedValue ? typedValue : default(TValue)).ToImmutableList();
        }

    }

}
