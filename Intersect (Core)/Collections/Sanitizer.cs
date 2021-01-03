using System;
using System.Collections.Generic;

using Intersect.Extensions;

namespace Intersect.Collections
{

    public struct Sanitizer
    {

        private Dictionary<string, SanitizedValue<object>> mSanitizedValues;

        public Dictionary<string, SanitizedValue<object>> Sanitized => mSanitizedValues?.ToDictionary();

        public Sanitizer Add<T>(string name, T before, T after)
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

        public T IsNotNull<T>(string name, T value, T defaultValue) where T : class
        {
            if (value == null)
            {
                Add(name, null, defaultValue);
            }

            return defaultValue;
        }

        public T IsNull<T>(string name, T value) where T : class
        {
            if (value != null)
            {
                Add(name, value, null);
            }

            return null;
        }

        public T Is<T>(string name, T actualValue, T expectedValue)
        {
            if (expectedValue == null && actualValue == null || (expectedValue?.Equals(actualValue) ?? false))
            {
                return actualValue;
            }

            Add(name, actualValue, expectedValue);

            return expectedValue;
        }

        public T IsNot<T>(string name, T actualValue, T expectedValue)
        {
            if (expectedValue == null && actualValue != null || !(expectedValue?.Equals(actualValue) ?? false))
            {
                return actualValue;
            }

            Add(name, actualValue, expectedValue);

            return expectedValue;
        }

        public T MaximumExclusive<T>(string name, T actualValue, T expectedMinimum)
            where T : IComparable<T>
        {
            if (expectedMinimum.CompareTo(actualValue) < 0)
            {
                return actualValue;
            }

            Add(name, actualValue, expectedMinimum);

            return expectedMinimum;
        }

        public T Maximum<T>(string name, T actualValue, T expectedMinimum) where T : IComparable<T>
        {
            if (expectedMinimum.CompareTo(actualValue) <= 0)
            {
                return actualValue;
            }

            Add(name, actualValue, expectedMinimum);

            return expectedMinimum;
        }

        public T MinimumExclusive<T>(string name, T actualValue, T expectedMaximum)
            where T : IComparable<T>
        {
            if (expectedMaximum.CompareTo(actualValue) > 0)
            {
                return actualValue;
            }

            Add(name, actualValue, expectedMaximum);

            return expectedMaximum;
        }

        public T Minimum<T>(string name, T actualValue, T expectedMaximum) where T : IComparable<T>
        {
            if (expectedMaximum.CompareTo(actualValue) >= 0)
            {
                return actualValue;
            }

            Add(name, actualValue, expectedMaximum);

            return expectedMaximum;
        }

        public T Clamp<T>(
            string name,
            T actualValue,
            T expectedMinimum,
            T expectedMaximum
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
            string name,
            T actualValue,
            T expectedMinimum,
            T expectedMaximum
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
            string name,
            T actualValue,
            T expectedMinimum,
            T expectedMaximum
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
            string name,
            T actualValue,
            T expectedMinimum,
            T expectedMaximum
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
