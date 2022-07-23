using System;

using Intersect.Localization;

namespace Intersect.GameObjects.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EditorBooleanAttribute : EditorDisplayAttribute
    {
        public BooleanStyle Style { get; set; } = BooleanStyle.YesNo;
    }
}
