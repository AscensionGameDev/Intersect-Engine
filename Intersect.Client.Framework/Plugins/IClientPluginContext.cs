using Intersect.Client.Framework.Content;
using Intersect.Client.Plugins.Interfaces;
using Intersect.Plugins;

namespace Intersect.Client.Plugins
{
    /// <summary>
    /// Declares the plugin API surface for the Intersect client
    /// </summary>
    public interface IClientPluginContext : IPluginContext<IClientPluginContext, IClientLifecycleHelper>
    {
        /// <summary>
        /// The <see cref="IContentManager"/> of the current plugin.
        /// </summary>
        IContentManager ContentManager { get; }
    }
}
