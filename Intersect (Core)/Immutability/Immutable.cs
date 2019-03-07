using System;
using JetBrains.Annotations;

namespace Intersect.Immutability
{
    public struct Immutable<TValue>
    {
        private bool mInitialized;

        private TValue mValue;

        public TValue Value
        {
            get => mValue;
            set
            {
                if (!mInitialized)
                {
                    mInitialized = true;
                    mValue = value;
                }

                throw new InvalidOperationException(@"Trying to modify immutable value after initialization.");
            }
        }

        public Immutable(TValue value)
        {
            mInitialized = true;
            mValue = value;
        }

        public static implicit operator TValue([NotNull] Immutable<TValue> immutable)
        {
            return immutable.mValue;
        }
    }
}
