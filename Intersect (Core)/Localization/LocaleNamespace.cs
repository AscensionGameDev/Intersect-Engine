using System.Reflection;

using Intersect.Framework.Reflection;

namespace Intersect.Localization
{
    [Serializable]
    public abstract partial class LocaleNamespace : Localized
    {
        protected LocaleNamespace() => ValidateNamespaceType(GetType());

        public static void ValidateNamespaceType(Type type)
        {
            if (!type.Extends<LocaleNamespace>())
            {
                throw new ArgumentException(
                    $"{type.FullName} does not extend from {typeof(LocaleNamespace).FullName}.",
                    nameof(type)
                );
            }

            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            foreach (var fieldInfo in fieldInfos)
            {
                if (!fieldInfo.FieldType.Extends<Localized>())
                {
                    throw new ArgumentException(
                        $"{type.FullName} contains invalid field {fieldInfo.Name} of disallowed type {fieldInfo.FieldType.FullName}.",
                        nameof(type)
                    );
                }
            }
        }
    }
}
