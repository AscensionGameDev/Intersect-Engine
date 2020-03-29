using System;
using System.Reflection;

using JetBrains.Annotations;

namespace Intersect.Reflection
{

    public static class MemberInfoExtensions
    {

        public static string GetFullName([NotNull] this MemberInfo memberInfo)
        {
            if (memberInfo is Type type)
            {
                return type.FullName;
            }

            var declaringType = memberInfo.DeclaringType;

            return declaringType == null ? memberInfo.Name : $@"{declaringType.FullName}.{memberInfo.Name}";
        }

    }

}
