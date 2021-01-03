using System;

using Intersect.Client.Framework.Content;
using Intersect.Client.General;
using Intersect.Client.Plugins.Helpers;
using Intersect.Client.Plugins.Interfaces;
using Intersect.Factories;
using Intersect.Plugins;
using Intersect.Plugins.Contexts;
using Intersect.Plugins.Interfaces;

namespace Intersect.Client.Plugins.Contexts
{
    /// <summary>
    /// Implementation of <see cref="IClientPluginContext"/>.
    /// </summary>
    internal sealed class ClientPluginContext : PluginContext<ClientPluginContext, IClientLifecycleHelper>,
        IClientPluginContext
    {
        /// <summary>
        /// <see cref="IPluginContext"/> factory that creates instances of <see cref="IClientPluginContext"/>.
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

                return new ClientPluginContext(plugin);
            }
        }

        /// <inheritdoc />
        public override IClientLifecycleHelper Lifecycle { get; }

        /// <inheritdoc />
        private ClientPluginContext(Plugin plugin) : base(plugin)
        {
            Lifecycle = new ClientLifecycleHelper(this);
        }

        /// <inheritdoc />
        public IContentManager ContentManager => Globals.ContentManager ??
                                                 throw new InvalidOperationException(
                                                     @"Tried accessing the content manager before it was created."
                                                 );
    }
}
