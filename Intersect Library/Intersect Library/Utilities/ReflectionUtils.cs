using System;
using System.IO;
using System.Reflection;
using System.Text;
using Intersect.Logging;

namespace Intersect.Utilities
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

        public static bool ExtractResource(string resourceName, string destinationName)
        {
            if (string.IsNullOrEmpty(destinationName)) throw new ArgumentNullException(nameof(destinationName));
            using (var desinationStream = new FileStream(destinationName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                return ExtractResource(resourceName, destinationName);
        }

        public static bool ExtractResource(string resourceName, Stream destinationStream)
        {
            if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException(nameof(resourceName));
            if (destinationStream == null) throw new ArgumentNullException(nameof(destinationStream));

            try
            {
                var executingAssembly = Assembly.GetExecutingAssembly();
                using (var resourceStream = executingAssembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null) throw new ArgumentNullException(nameof(resourceStream));
                    var data = new byte[resourceStream.Length];
                    resourceStream.Read(data, 0, (int) resourceStream.Length);
                    destinationStream.Write(data, 0, data.Length);
                }
                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                return false;
            }
        }
    }
}