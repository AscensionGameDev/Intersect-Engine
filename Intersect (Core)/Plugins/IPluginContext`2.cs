using Intersect.Plugins.Interfaces;

using JetBrains.Annotations;

namespace Intersect.Plugins
{
    /// <summary>
    /// Defines the API of the <typeparamref name="TPluginContext"/> with a
    /// specialized <see cref="ILifecycleHelper"/> during application runtime.
    /// </summary>
    /// <typeparam name="TPluginContext">a <see cref="IPluginContext"/> subtype</typeparam>
    /// <typeparam name="TLifecycleHelper">a <see cref="ILifecycleHelper"/> subtype</typeparam>
    public interface IPluginContext<TPluginContext, out TLifecycleHelper> : IPluginContext<TPluginContext>
        where TPluginContext : IPluginContext<TPluginContext> where TLifecycleHelper : ILifecycleHelper
    {
        /// <summary>
        /// The specialized <see cref="ILifecycleHelper"/> of the current plugin.
        /// </summary>
        [NotNull]
        new TLifecycleHelper Lifecycle { get; }
    }
}
