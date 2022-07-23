using System;

namespace Intersect.GameObjects.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EditorTimeAttribute : EditorFormattedAttribute
    {
        public EditorTimeAttribute() : base("FormatTimeMilliseconds")
        {
        }
    }
}
