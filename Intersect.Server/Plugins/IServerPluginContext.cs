using Intersect.Plugins;
using Intersect.Server.Plugins.Helpers;

namespace Intersect.Server.Plugins
{
    /// <summary>
    /// Declares the plugin API surface for the Intersect server
    /// </summary>
    public interface IServerPluginContext : IPluginContext<IServerPluginContext, IServerLifecycleHelper>
    {

    }
}
