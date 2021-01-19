using Intersect.Plugins.Helpers;

namespace Intersect.Server.Plugins.Helpers
{
    /// <inheritdoc cref="IServerLifecycleHelper"/>
    internal sealed class ServerLifecycleHelper : ContextHelper<IServerPluginContext>, IServerLifecycleHelper
    {
        /// <inheritdoc />
        public ServerLifecycleHelper(IServerPluginContext context) : base(context)
        {
        }
    }
}
