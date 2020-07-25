using Intersect.Plugins.Interfaces;

using JetBrains.Annotations;

namespace Intersect.Plugins
{
    /// <summary>
    /// Defines the API of the plugin context during application bootstrapping.
    /// </summary>
    public interface IPluginBootstrapContext : IPluginBaseContext
    {
        /// <summary>
        /// The <see cref="ICommandLineHelper"/> of the current plugin.
        /// </summary>
        [NotNull]
        ICommandLineHelper CommandLine { get; }
    }
}
