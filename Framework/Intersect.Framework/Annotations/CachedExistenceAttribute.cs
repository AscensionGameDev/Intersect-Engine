using System.Collections.Concurrent;
using System.Reflection;

namespace Intersect.Framework.Annotations;

public abstract class CachedExistenceAttribute<TAttribute> : Attribute
    where TAttribute : CachedExistenceAttribute<TAttribute>
{
    // ReSharper disable once StaticMemberInGenericType
    // We're specifically trying to take advantage of static-per-generic-argument
    private static readonly ConcurrentDictionary<MemberInfo, bool> CachedExistence = [];

    protected static bool ExistsOn(MemberInfo memberInfo, bool allowInheritedFromDeclaringType = true)
    {
        if (CachedExistence.TryGetValue(memberInfo, out var existsOn))
        {
            return existsOn;
        }

        existsOn = memberInfo.GetCustomAttribute<TAttribute>() != null;
        if (!existsOn)
        {
            switch (memberInfo)
            {
                case PropertyInfo propertyInfo:
                    existsOn |= ExistsOn(propertyInfo.PropertyType);
                    break;
                case FieldInfo fieldInfo:
                    existsOn |= ExistsOn(fieldInfo.FieldType);
                    break;
            }
        }

        if (!existsOn && allowInheritedFromDeclaringType && memberInfo.DeclaringType is {} declaringType)
        {
            existsOn = ExistsOn(declaringType);
        }

        CachedExistence[memberInfo] = existsOn;
        return existsOn;
    }
}
