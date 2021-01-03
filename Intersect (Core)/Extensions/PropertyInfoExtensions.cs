using System.Globalization;
using System.Reflection;

namespace Intersect.Extensions
{

    public static class PropertyInfoExtensions
    {

        public static bool TryGetValue<TValue>(
            this PropertyInfo propertyInfo,
            object target,
            out TValue value
        )
        {
            if (propertyInfo.GetValue(target) is TValue typedValue)
            {
                value = typedValue;

                return true;
            }

            value = default(TValue);

            return false;
        }

        public static bool TryGetValue<TValue>(
            this PropertyInfo propertyInfo,
            object target,
            object[] index,
            out TValue value
        )
        {
            if (propertyInfo.GetValue(target, index) is TValue typedValue)
            {
                value = typedValue;

                return true;
            }

            value = default(TValue);

            return false;
        }

        public static bool TryGetValue<TValue>(
            this PropertyInfo propertyInfo,
            object target,
            BindingFlags invokeAttr,
            Binder binder,
            object[] index,
            CultureInfo culture,
            out TValue value
        )
        {
            if (propertyInfo.GetValue(target, invokeAttr, binder, index, culture) is TValue typedValue)
            {
                value = typedValue;

                return true;
            }

            value = default(TValue);

            return false;
        }

    }

}
