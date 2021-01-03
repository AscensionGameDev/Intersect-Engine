using System.Reflection;

using Intersect.Plugins.Helpers;
using Intersect.Plugins.Interfaces;

namespace Intersect.Plugins.Contexts
{
    /// <summary>
    /// Common implementation class for typed plugin contexts.
    /// </summary>
    /// <typeparam name="TPluginContext"><see cref="IPluginContext"/> extension type</typeparam>
    /// <typeparam name="TLifecycleHelper"><see cref="ILifecycleHelper"/> extension type used by <typeparamref name="TPluginContext"/></typeparam>
    public abstract class
        PluginContext<TPluginContext, TLifecycleHelper> : IPluginContext<TPluginContext, TLifecycleHelper>
        where TPluginContext : IPluginContext<TPluginContext> where TLifecycleHelper : ILifecycleHelper
    {

        private Plugin Plugin { get; }

        /// <inheritdoc />
        public Assembly Assembly => Plugin.Reference.Assembly;

        /// <inheritdoc />
        public PluginConfiguration Configuration => Plugin.Configuration;

        /// <inheritdoc />
        public IEmbeddedResourceHelper EmbeddedResources { get; }

        ILifecycleHelper IPluginContext.Lifecycle => Lifecycle;

        /// <inheritdoc cref="IPluginContext.Lifecycle" />
        public abstract TLifecycleHelper Lifecycle { get; }

        /// <inheritdoc />
        public ILoggingHelper Logging => Plugin.Logging;

        /// <inheritdoc />
        public IManifestHelper Manifest => Plugin.Manifest;

        /// <summary>
        /// Instantiates a <see cref="PluginContext{TPluginContext, TLifecycleHelper}"/>.
        /// </summary>
        /// <param name="plugin">the <see cref="Plugin"/> this context will be used for</param>
        protected PluginContext(Plugin plugin)
        {
            Plugin = plugin;
            EmbeddedResources = new EmbeddedResourceHelper(Assembly);
        }

        /// <inheritdoc />
        public TConfiguration GetTypedConfiguration<TConfiguration>() where TConfiguration : PluginConfiguration =>
            Configuration as TConfiguration;

    }

}
