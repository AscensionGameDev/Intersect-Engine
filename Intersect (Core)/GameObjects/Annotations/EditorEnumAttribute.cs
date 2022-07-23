using System;

namespace Intersect.GameObjects.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EditorEnumAttribute : EditorDisplayAttribute
    {
        public EditorEnumAttribute(Type enumType)
        {
            EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));

            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"{enumType.FullName} is not a enum type.", nameof(enumType));
            }
        }

        public Type EnumType { get; }
    }
}
