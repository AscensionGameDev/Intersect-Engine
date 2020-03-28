using System;
using System.Collections.Generic;

using Intersect.Extensions;

using JetBrains.Annotations;

namespace Intersect.Collections
{

    public struct Sanitizer
    {

        private Dictionary<string, SanitizedValue<object>> mSanitizedValues;

        [CanBeNull]
        public Dictionary<string, SanitizedValue<object>> Sanitized => mSanitizedValues?.ToDictionary();

        public Sanitizer Add<T>([NotNull] string name, T before, T after)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (mSanitizedValues == null)
            {
                mSanitizedValues = new Dictionary<string, SanitizedValue<object>>(8);
            }

            mSanitizedValues.Add(name, new SanitizedValue<object>(before, after));

            return this;
        }

        [NotNull]
        public T IsNotNull<T>([NotNull] string name, [CanBeNull] T value, [NotNull] T defaultValue) where T : class
        {
            if (value == null)
            {
                Add(name, null, defaultValue);
            }

            return defaultValue;
        }

        [CanBeNull]
        public T IsNull<T>([NotNull] string name, [CanBeNull] T value) where T : class
        {
            if (value != null)
            {
                Add(name, value, null);
            }

            return null;
        }

        public T Is<T>([NotNull] string name, T actualValue, T expectedValue)
        {
            if (expectedValue == null && actualValue == null || (expectedValue?.Equals(actualValue) ?? false))
            {
                return actualValue;
            }

            Add(name, actualValue, expectedValue);

            return expectedValue;
        }

        public T IsNot<T>([NotNull] string name, T actualValue, T expectedValue)
        {
            if (expectedValue == null && actualValue != null || !(expectedValue?.Equals(actualValue) ?? false))
            {
                return actualValue;
            }

            Add(name, actualValue, expectedValue);

            return expectedValue;
        }

        public T MaximumExclusive<T>([NotNull] string name, T actualValue, [NotNull] T expectedMinimum)
            where T : IComparable<T>
        {
            if (expectedMinimum.CompareTo(actualValue) < 0)
            {
                return actualValue;
            }

            Add(name, actualValue, expectedMinimum);

            return expectedMinimum;
        }

        public T Maximum<T>([NotNull] string name, T actualValue, [NotNull] T expectedMinimum) where T : IComparable<T>
        {
            if (expectedMinimum.CompareTo(actualValue) <= 0)
            {
                return actualValue;
            }

            Add(name, actualValue, expectedMinimum);

            return expectedMinimum;
        }

        public T MinimumExclusive<T>([NotNull] string name, T actualValue, [NotNull] T expectedMaximum)
            where T : IComparable<T>
        {
            if (expectedMaximum.CompareTo(actualValue) > 0)
            {
                return actualValue;
            }

            Add(name, actualValue, expectedMaximum);

            return expectedMaximum;
        }

        public T Minimum<T>([NotNull] string name, T actualValue, [NotNull] T expectedMaximum) where T : IComparable<T>
        {
            if (expectedMaximum.CompareTo(actualValue) >= 0)
            {
                return actualValue;
            }

            Add(name, actualValue, expectedMaximum);

            return expectedMaximum;
        }

        public T Clamp<T>(
            [NotNull] string name,
            T actualValue,
            [NotNull] T expectedMinimum,
            [NotNull] T expectedMaximum
        ) where T : IComparable<T>
        {
            if (expectedMinimum.CompareTo(actualValue) > 0)
            {
                Add(name, actualValue, expectedMinimum);

                return expectedMinimum;
            }

            if (expectedMaximum.CompareTo(actualValue) >= 0)
            {
                return actualValue;
            }

            Add(name, actualValue, expectedMaximum);

            return expectedMaximum;
        }

        public T ClampExclusive<T>(
            [NotNull] string name,
            T actualValue,
            [NotNull] T expectedMinimum,
            [NotNull] T expectedMaximum
        ) where T : IComparable<T>
        {
            if (expectedMinimum.CompareTo(actualValue) >= 0)
            {
                Add(name, actualValue, expectedMinimum);

                return expectedMinimum;
            }

            if (expectedMaximum.CompareTo(actualValue) > 0)
            {
                return actualValue;
            }

            Add(name, actualValue, expectedMaximum);

            return expectedMaximum;
        }

        public T ClampExclusiveMinimum<T>(
            [NotNull] string name,
            T actualValue,
            [NotNull] T expectedMinimum,
            [NotNull] T expectedMaximum
        ) where T : IComparable<T>
        {
            if (expectedMinimum.CompareTo(actualValue) > 0)
            {
                Add(name, actualValue, expectedMinimum);

                return expectedMinimum;
            }

            if (expectedMaximum.CompareTo(actualValue) >= 0)
            {
                return actualValue;
            }

            Add(name, actualValue, expectedMaximum);

            return expectedMaximum;
        }

        public T ClampExclusiveMaximum<T>(
            [NotNull] string name,
            T actualValue,
            [NotNull] T expectedMinimum,
            [NotNull] T expectedMaximum
        ) where T : IComparable<T>
        {
            if (expectedMinimum.CompareTo(actualValue) >= 0)
            {
                Add(name, actualValue, expectedMinimum);

                return expectedMinimum;
            }

            if (expectedMaximum.CompareTo(actualValue) > 0)
            {
                return actualValue;
            }

            Add(name, actualValue, expectedMaximum);

            return expectedMaximum;
        }

    }

}
