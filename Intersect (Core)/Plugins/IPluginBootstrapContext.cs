using Intersect.Plugins.Interfaces;

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
        ICommandLineHelper CommandLine { get; }

        /// <summary>
        /// The <see cref="IPacketHelper"/> of the current plugin.
        /// </summary>
        IPacketHelper Packet { get; }
    }
}
