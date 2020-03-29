using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

namespace Intersect.Server.Core.CommandParsing.Arguments
{

    public sealed class ArgumentValues : IEnumerable<object>
    {

        [NotNull] private readonly IList<object> mValues;

        public ArgumentValues([NotNull] string argumentName, [CanBeNull] params object[] values) : this(
            argumentName, values.AsEnumerable()
        )
        {
        }

        public ArgumentValues([NotNull] string argumentName, bool isImplicit, [CanBeNull] params object[] values) :
            this(argumentName, values.AsEnumerable(), isImplicit)
        {
        }

        public ArgumentValues(
            [NotNull] string argumentName,
            [CanBeNull] IEnumerable<object> values = null,
            bool isImplicit = false
        )
        {
            ArgumentName = argumentName;
            mValues = new List<object>(values ?? new object[0]);
            IsImplicit = isImplicit;
        }

        [NotNull]
        public string ArgumentName { get; }

        public object Value => mValues.FirstOrDefault();

        [NotNull]
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
