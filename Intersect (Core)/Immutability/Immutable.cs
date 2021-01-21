using System;

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
                if (mInitialized)
                {
                    throw new InvalidOperationException(@"Trying to modify immutable value after initialization.");
                }

                mInitialized = true;
                mValue = value;
            }
        }

        public Immutable(TValue value)
        {
            mInitialized = true;
            mValue = value;
        }

        public static implicit operator TValue(Immutable<TValue> immutable)
        {
            return immutable.mValue;
        }

    }

}
