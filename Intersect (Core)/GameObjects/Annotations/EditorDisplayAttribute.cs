using System;

namespace Intersect.GameObjects.Annotations
{
    public enum EmptyBehavior
    {
        NoChange,

        ShowNoneOnEmpty,

        ShowNoneOnNull,

        ShowNoneOnNullOrEmpty,
    }

    public enum StringBehavior
    {
        NoChange,

        Trim,
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EditorDisplayAttribute : Attribute
    {
        public EmptyBehavior EmptyBehavior { get; set; } = EmptyBehavior.NoChange;

        public EditorFieldType FieldType { get; set; } = EditorFieldType.Default;

        public StringBehavior StringBehavior { get; set; } = StringBehavior.NoChange;

        [Obsolete("We want to re-implement strings to be object-oriented.")]
        public virtual string Format(Type stringsType, object value)
        {
            string displayValue;
            switch (value)
            {
                case string stringValue:
                    displayValue = stringValue;

                    switch (StringBehavior)
                    {
                        case StringBehavior.NoChange:
                            break;

                        case StringBehavior.Trim:
                            displayValue = stringValue.Trim();
                            break;
                    }

                    break;

                default:
                    displayValue = value?.ToString();
                    break;
            }

            switch (EmptyBehavior)
            {
                case EmptyBehavior.NoChange:
                    break;

                case EmptyBehavior.ShowNoneOnEmpty:
                    if (displayValue != default && string.IsNullOrEmpty(displayValue))
                    {
                        displayValue = GetNone(stringsType);
                    }
                    break;

                case EmptyBehavior.ShowNoneOnNull:
                    if (displayValue == default)
                    {
                        displayValue = GetNone(stringsType);
                    }
                    break;

                case EmptyBehavior.ShowNoneOnNullOrEmpty:
                    if (string.IsNullOrEmpty(displayValue))
                    {
                        displayValue = GetNone(stringsType);
                    }
                    break;
            }

            return displayValue;
        }

        protected static string GetNone(Type stringsType)
        {
            if (stringsType == default)
            {
                throw new ArgumentNullException(nameof(stringsType));
            }

            var noneFieldInfo = stringsType.GetNestedType("General")?.GetField("None");
            if (noneFieldInfo == default)
            {
                throw new InvalidOperationException();
            }

            return noneFieldInfo?.GetValue(null)?.ToString();
        }
    }
}
