using Intersect.Plugins.Interfaces;

namespace Intersect.Plugins
{
    /// <summary>
    /// Defines the API of the <typeparamref name="TPluginContext"/> with a
    /// specialized <see cref="ILifecycleHelper"/> during application runtime.
    /// </summary>
    /// <typeparam name="TPluginContext"><see cref="IPluginContext"/> extension type</typeparam>
    /// <typeparam name="TLifecycleHelper"><see cref="ILifecycleHelper"/> extension type used by <typeparamref name="TPluginContext"/></typeparam>
    public interface IPluginContext<TPluginContext, out TLifecycleHelper> : IPluginContext<TPluginContext>
        where TPluginContext : IPluginContext<TPluginContext> where TLifecycleHelper : ILifecycleHelper
    {
        /// <summary>
        /// The specialized <see cref="ILifecycleHelper"/> of the current plugin.
        /// </summary>
        new TLifecycleHelper Lifecycle { get; }
    }
}
