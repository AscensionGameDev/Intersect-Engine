using System.Reflection;

namespace Intersect.Framework.Annotations;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Struct | AttributeTargets.Enum)]
public sealed class IgnoreAttribute : CachedExistenceAttribute<IgnoreAttribute>
{
    public static bool IsIgnored(MemberInfo memberInfo) => ExistsOn(memberInfo);
}