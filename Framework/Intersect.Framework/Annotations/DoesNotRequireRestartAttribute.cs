using System.Reflection;

namespace Intersect.Framework.Annotations;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Struct |
    AttributeTargets.Enum
)]
public sealed class DoesNotRequireRestartAttribute : CachedExistenceAttribute<IgnoreAttribute>
{
    public static bool DoesNotRequireRestart(MemberInfo memberInfo) => ExistsOn(memberInfo);
}