using System.Reflection;

namespace Intersect.Framework.Annotations;

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Struct |
    AttributeTargets.Enum
)]
public sealed class RequiresRestartAttribute : CachedExistenceAttribute<IgnoreAttribute>
{
    public static bool RequiresRestart(MemberInfo memberInfo) => ExistsOn(
        memberInfo,
        allowInheritedFromDeclaringType: !DoesNotRequireRestartAttribute.DoesNotRequireRestart(memberInfo)
    );
}