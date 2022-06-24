using System;

namespace Intersect.Utilities
{

    public static partial class Nullability
    {

        public static T IsNotNull<T>(this T value)
        {
            if (value == null)
            {
                throw new InvalidOperationException();
            }

            return value;
        }

    }

}
