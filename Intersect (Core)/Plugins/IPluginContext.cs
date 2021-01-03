using Intersect.Plugins.Interfaces;

namespace Intersect.Plugins
{
    /// <summary>
    /// Defines the API of the plugin context during application runtime.
    /// </summary>
    public interface IPluginContext : IPluginBaseContext
    {
        /// <summary>
        /// The <see cref="ILifecycleHelper"/> of the current plugin.
        /// </summary>
        ILifecycleHelper Lifecycle { get; }
    }
}
