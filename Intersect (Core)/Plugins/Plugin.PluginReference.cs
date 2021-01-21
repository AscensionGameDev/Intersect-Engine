
using System;
using System.IO;
using System.Reflection;

namespace Intersect.Plugins
{
    /// <summary>
    /// Reference container type of type, method handles, and other information gathered via reflection.
    /// </summary>
    internal sealed class PluginReference
    {
        /// <summary>
        /// Initializes a <see cref="PluginReference"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> this plugin belongs to.</param>
        /// <param name="configurationType">The plugin configuration <see cref="Type"/>, should descend from or be <see cref="PluginConfiguration"/>.</param>
        /// <param name="entryType">The <see cref="Type"/> of the entry point for the plugin, should descend from <see cref="IPluginEntry"/>.</param>
        internal PluginReference(
            Assembly assembly,
            Type configurationType,
            Type entryType
        )
        {
            Assembly = assembly;
            ConfigurationType = configurationType;
            EntryType = entryType;
        }

        /// <summary>
        /// The <see cref="Assembly"/> this plugin belongs to.
        /// </summary>
        internal Assembly Assembly { get; }

        /// <summary>
        /// The plugin configuration <see cref="Type"/>, should descend from or be <see cref="PluginConfiguration"/>.
        /// </summary>
        internal Type ConfigurationType { get; }

        /// <summary>
        /// The <see cref="Type"/> of the entry point for the plugin, should descend from <see cref="IPluginEntry"/>.
        /// </summary>
        internal Type EntryType { get; }

        /// <summary>
        /// The path to the configuration file for this plugin.
        /// </summary>
        internal string ConfigurationFile => Path.Combine(Directory, "config.json");

        /// <summary>
        /// The path to the directory this plugin is located in.
        /// </summary>
        internal string Directory => Path.GetDirectoryName(Assembly.Location) ??
                                     throw new InvalidOperationException(
                                         $"Error getting plugin directory for assembly {Assembly.FullName} ({Assembly.Location})."
                                     );

        /// <summary>
        /// Create an instance of the plugin entry type.
        /// </summary>
        /// <returns>an instance of <see cref="IPluginEntry"/> specific to this plugin</returns>
        internal IPluginEntry CreateInstance() =>
            Activator.CreateInstance(EntryType) as IPluginEntry ?? throw new InvalidOperationException();
    }
}
