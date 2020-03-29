using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;

using Intersect.Logging;

namespace Intersect.Utilities
{

    public static class ReflectionUtils
    {

        public static string StringifyParameter(ParameterInfo parameter)
        {
            return parameter == null
                ? @"[NAMEOF_NULL_PARAMETER: TYPEOF_NULL_PARAMETER]"
                : $"[{parameter.Name}: {parameter.ParameterType.Name}]";
        }

        public static string StringifyConstructor(ConstructorInfo constructor)
        {
            if (constructor == null)
            {
                return "<NULL_CONSTRUCTOR>";
            }

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
            if (type == null)
            {
                return "<NULL_TYPE>";
            }

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
            if (string.IsNullOrEmpty(destinationName))
            {
                throw new ArgumentNullException(nameof(destinationName));
            }

            using (var destinationStream = new FileStream(destinationName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                return ExtractResource(resourceName, destinationStream);
            }
        }

        public static bool ExtractCosturaResource(string resourceName, string destinationName)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException(nameof(resourceName));
            }

            if (string.IsNullOrEmpty(destinationName))
            {
                throw new ArgumentNullException(nameof(destinationName));
            }

            if (!resourceName.StartsWith("costura."))
            {
                return false;
            }

            if (!resourceName.EndsWith(".compressed"))
            {
                return false;
            }

            using (var destinationStream = new FileStream(destinationName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                return ExtractCompressedResource(resourceName, destinationStream);
            }
        }

        public static bool ExtractCompressedResource(string resourceName, Stream destinationStream)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException(nameof(resourceName));
            }

            if (destinationStream == null)
            {
                throw new ArgumentNullException(nameof(destinationStream));
            }

            try
            {
                var executingAssembly = Assembly.GetEntryAssembly();
                using (var compressedResourceStream = executingAssembly?.GetManifestResourceStream(resourceName))
                {
                    if (compressedResourceStream == null)
                    {
                        throw new ArgumentNullException(nameof(compressedResourceStream));
                    }

                    using (var resourceStream = new DeflateStream(compressedResourceStream, CompressionMode.Decompress))
                    {
                        StreamUtils.Pipe(resourceStream, destinationStream);
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                Log.Error($"resourceName: '{resourceName}'");

                return false;
            }
        }

        public static bool ExtractResource(string resourceName, Stream destinationStream)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException(nameof(resourceName));
            }

            if (destinationStream == null)
            {
                throw new ArgumentNullException(nameof(destinationStream));
            }

            try
            {
                var executingAssembly = Assembly.GetEntryAssembly();
                using (var resourceStream = executingAssembly?.GetManifestResourceStream(resourceName))
                {
                    StreamUtils.Pipe(resourceStream, destinationStream);
                }

                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                Log.Error($"resourceName: '{resourceName}'");

                return false;
            }
        }

    }

}
