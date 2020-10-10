using Intersect.Extensions;
using Intersect.Factories;
using Intersect.Plugins;
using Intersect.Plugins.Contexts;
using Intersect.Server.Core;
using Intersect.Server.Plugins.Helpers;

namespace Intersect.Server.Plugins
{
    /// <summary>
    /// Implementation of <see cref="IServerPluginContext"/>.
    /// </summary>
    internal sealed class ServerPluginContext : PluginContext<ServerPluginContext, IServerLifecycleHelper>,
        IServerPluginContext
    {
        /// <summary>
        /// <see cref="IPluginContext"/> factory that creates instances of <see cref="IServerPluginContext"/>.
        /// </summary>
        internal sealed class Factory : IFactory<IPluginContext>
        {
            /// <inheritdoc />
            public IPluginContext Create(params object[] args)
            {
                args.ValidateTypes(typeof(IServerContext), typeof(Plugin));
                return new ServerPluginContext(args[0] as IServerContext, args[1] as Plugin);
            }
        }

        private IServerContext ServerContext { get; }

        /// <inheritdoc />
        public ServerPluginContext(IServerContext serverContext, Plugin plugin) : base(plugin)
        {
            ServerContext = serverContext;
            Lifecycle = new ServerLifecycleHelper(this);
        }

        /// <inheritdoc />
        public override IServerLifecycleHelper Lifecycle { get; }
    }
}
