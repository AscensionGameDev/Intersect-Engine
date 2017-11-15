using System;
using System.Reflection;
using System.Text;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_9.Intersect_Convert_Lib.Utilities
{
    public static class ReflectionUtils
    {
        public static string StringifyParameter(ParameterInfo parameter) =>
            parameter == null
                ? @"[NAMEOF_NULL_PARAMETER: TYPEOF_NULL_PARAMETER]"
                : $"[{parameter.Name}: {parameter.ParameterType.Name}]";

        public static string StringifyConstructor(ConstructorInfo constructor)
        {
            if (constructor == null) return "<NULL_CONSTRUCTOR>";

            var parameters = constructor.GetParameters();
            var builder = new StringBuilder();
            foreach (var parameter in parameters)
            {
                builder.Append(StringifyParameter(parameter));
                builder.Append(",");
            }

            return builder.ToString();
        }

        public static string StringifyConstructors(Type type)
        {
            if (type == null) return "<NULL_TYPE>";

            var constructors = type.GetConstructors();
            var builder = new StringBuilder();
            foreach (var constructor in constructors)
            {
                builder.AppendLine(StringifyConstructor(constructor));
            }

            return builder.ToString();
        }
    }
}