using System.Diagnostics;
using System.Reflection;

namespace Intersect.Framework.Reflection;

public static partial class MemberInfoExtensions
{
    public static string GetFullName(this MemberInfo memberInfo)
    {
        if (memberInfo is Type type)
        {
            return type.GetName(true);
        }

        var declaringType = memberInfo.DeclaringType;

        return declaringType == null ? memberInfo.Name : $@"{declaringType.FullName}.{memberInfo.Name}";
    }

    public static string GetSignature(this MethodInfo methodInfo, bool fullyQualified = false)
    {
        Debug.Assert(methodInfo != null);

        var returnTypeName = methodInfo.ReturnType.GetName(fullyQualified);
        var declaringTypeName = methodInfo.DeclaringType?.GetName(fullyQualified) ?? "???";
        var parameterTypes = methodInfo.GetParameters().Select(parameter => parameter.ParameterType);
        var parameterTypeNames = parameterTypes.Select(
            parameterType => fullyQualified ? parameterType.FullName : parameterType.Name
        );

        return $"{returnTypeName} {declaringTypeName}.{methodInfo.Name}({string.Join(", ", parameterTypeNames)})";
    }

    public static string GetFullSignature(this MethodInfo methodInfo) => methodInfo.GetSignature(true);
}
