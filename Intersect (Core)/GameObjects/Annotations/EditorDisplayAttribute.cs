using System;

namespace Intersect.GameObjects.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EditorDisplayAttribute : Attribute
    {
        public EditorFieldType FieldType { get; set; } = EditorFieldType.Default;

        [Obsolete("We want to re-implement strings to be object-oriented.")]
        public virtual string Format(Type stringsType, object value) => value?.ToString();
    }
}
