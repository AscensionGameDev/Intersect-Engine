using Intersect.Properties;
using Intersect.Reflection;

using System;
using System.Linq;
using System.Reflection;

namespace Intersect.Plugins.Loaders
{
    internal sealed partial class PluginLoader
    {
        /// <summary>
        /// The plugin entry type is defined as follows:
        /// - Is completely defined (not abstract, not generic, not an interface)
        /// - Implements <see cref="IPluginEntry"/> (or <see cref="IPluginEntry{TPluginContext}"/>, a sub-interface)
        /// - Has a default constructor (optional parameters are not a default constructor!)
        /// </summary>
        /// <param name="type">the <see cref="Type"/> to check</param>
        /// <returns>if <paramref name="type"/> is a valid plugin entry type</returns>
        internal static bool IsPluginEntryType(Type type)
        {
            // Abstract, interface and generic types are not valid virtual manifest types.
            if (type.IsAbstract || type.IsInterface || type.IsGenericType)
            {
                return false;
            }

            if (!typeof(IPluginEntry).IsAssignableFrom(type))
            {
                return false;
            }

            var constructor = type.GetConstructor(Array.Empty<Type>());
            return constructor != null;
        }

        internal static PluginReference CreatePluginReference(Assembly assembly)
        {
            var assemblyTypes = assembly.GetTypes();
            var entryType = assemblyTypes.FirstOrDefault(IsPluginEntryType);

            if (entryType == null)
            {
                throw new MissingPluginEntryException(assembly);
            }

            var entryTypeConstructor = entryType.GetConstructor(Array.Empty<Type>());
            if (entryTypeConstructor == null)
            {
                throw new ArgumentException(
                    ExceptionMessages.FoundPluginEntryTypeMissingDefaultConstructor, nameof(assembly)
                );
            }

            var configurationBaseType = typeof(PluginConfiguration);
            var configurationType = assembly.FindDefinedSubtypesOf(configurationBaseType).FirstOrDefault() ??
                                    configurationBaseType;

            return new PluginReference(assembly, configurationType, entryType);
        }
    }
}
