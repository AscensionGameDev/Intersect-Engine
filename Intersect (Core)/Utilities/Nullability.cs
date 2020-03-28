using System;

using JetBrains.Annotations;

namespace Intersect.Utilities
{

    public static class Nullability
    {

        [NotNull]
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
