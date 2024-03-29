﻿using System.Diagnostics;
using System.Reflection;
using Microsoft.Diagnostics.Runtime;

namespace Intersect.Reflection;

public static partial class MemberInfoExtensions
{
    public static string GetFullName(this MemberInfo memberInfo)
    {
        if (memberInfo is Type type)
        {
            return type.FullName;
        }

        var declaringType = memberInfo.DeclaringType;

        return declaringType == null ? memberInfo.Name : $@"{declaringType.FullName}.{memberInfo.Name}";
    }

    public static string GetSignature(this MethodInfo methodInfo, bool fullyQualified = false)
    {
        Debug.Assert(methodInfo != null);

        var returnTypeName = fullyQualified ? methodInfo.ReturnType.FullName : methodInfo.ReturnType.Name;
        var declaringTypeName = fullyQualified ? methodInfo.DeclaringType.FullName : methodInfo.DeclaringType.Name;
        var parameterTypes = methodInfo.GetParameters().Select(parameter => parameter.ParameterType);
        var parameterTypeNames = parameterTypes.Select(
            parameterType => fullyQualified ? parameterType.FullName : parameterType.Name
        );

        return $"{returnTypeName} {declaringTypeName}.{methodInfo.Name}({string.Join(", ", parameterTypeNames)})";
    }

    public static string GetFullSignature(this MethodInfo methodInfo) => GetSignature(methodInfo, true);

    public static string GetSignature(this ClrMethod method, bool fullyQualified = false)
    {
        Debug.Assert(method != null);

        return method.Signature ?? string.Empty;
    }

    public static string GetFullSignature(this ClrMethod method) => method.GetSignature(true);
}