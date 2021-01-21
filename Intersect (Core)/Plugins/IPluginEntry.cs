
using Microsoft;

using System;

namespace Intersect.Plugins
{
    /// <summary>
    /// Defines the API that plugin entry points must implement
    /// in order to be discovered by the plugin loader.
    /// </summary>
    /// <see cref="Loaders.PluginLoader.LoadFrom(Core.IApplicationContext, System.Reflection.Assembly)"/>
    public interface IPluginEntry : IDisposable
    {
        /// <summary>
        /// Invoked during application bootstrapping before startup.
        /// </summary>
        /// <param name="context">the current plugin context</param>
        void OnBootstrap([ValidatedNotNull] IPluginBootstrapContext context);

        /// <summary>
        /// Invoked during application startup after basic initialization.
        /// </summary>
        /// <param name="context">the current plugin context</param>
        void OnStart([ValidatedNotNull] IPluginContext context);

        /// <summary>
        /// Invoked during application shutdown.
        /// </summary>
        /// <param name="context">the current plugin context</param>
        void OnStop([ValidatedNotNull] IPluginContext context);
    }
}
