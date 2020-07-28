using Intersect.Plugins.Helpers;

using JetBrains.Annotations;

namespace Intersect.Server.Plugins.Helpers
{
    /// <inheritdoc cref="IServerLifecycleHelper"/>
    internal sealed class ServerLifecycleHelper : ContextHelper<IServerPluginContext>, IServerLifecycleHelper
    {
        /// <inheritdoc />
        public ServerLifecycleHelper([NotNull] IServerPluginContext context) : base(context)
        {
        }
    }
}
