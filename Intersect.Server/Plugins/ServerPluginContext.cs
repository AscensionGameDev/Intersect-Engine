using System;

using Intersect.Factories;
using Intersect.Plugins;
using Intersect.Plugins.Contexts;
using Intersect.Plugins.Interfaces;
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
                if (args.Length < 1)
                {
                    throw new ArgumentException($@"Need to provide an instance of {nameof(IManifestHelper)}.");
                }

                if (!(args[0] is Plugin plugin))
                {
                    throw new ArgumentException($@"First argument needs to be non-null and of type {nameof(Plugin)}.");
                }

                return new ServerPluginContext(plugin);
            }
        }

        /// <inheritdoc />
        public ServerPluginContext(Plugin plugin) : base(plugin)
        {
            Lifecycle = new ServerLifecycleHelper(this);
        }

        /// <inheritdoc />
        public override IServerLifecycleHelper Lifecycle { get; }
    }
}
