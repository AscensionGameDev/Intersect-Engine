using System.Reflection;

namespace Intersect.Client.ThirdParty;

public partial class Steam
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class SupportedAttribute : Attribute
    {
        public static bool IsPresent(Assembly? assembly = default)
        {
            return assembly == default
                ? AppDomain.CurrentDomain.GetAssemblies().Any(IsPresent)
                : assembly.GetCustomAttribute<SupportedAttribute>() != default;
        }
    }
}