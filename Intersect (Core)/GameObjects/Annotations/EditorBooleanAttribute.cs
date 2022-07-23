using System;
using System.Reflection;

using Intersect.Localization;

namespace Intersect.GameObjects.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EditorBooleanAttribute : EditorDisplayAttribute
    {
        public BooleanStyle Style { get; set; } = BooleanStyle.YesNo;

        public override string Format(Type stringsType, object value)
        {
            if (stringsType == default)
            {
                throw new ArgumentNullException(nameof(stringsType));
            }

            var formatBooleanMethodInfo = stringsType.GetMethod("FormatBoolean", BindingFlags.Public | BindingFlags.Static);
            if (formatBooleanMethodInfo == default)
            {
                throw new InvalidOperationException($"{stringsType.FullName}.{nameof(stringsType)} is missing.");
            }

            return formatBooleanMethodInfo.Invoke(null, new[] { value, Style })?.ToString();
        }
    }
}
