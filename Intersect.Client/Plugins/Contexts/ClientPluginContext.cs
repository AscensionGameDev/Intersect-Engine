using Intersect.Client.Core;
using Intersect.Client.Framework.Content;
using Intersect.Client.Plugins.Helpers;
using Intersect.Client.Plugins.Interfaces;
using Intersect.Extensions;
using Intersect.Factories;
using Intersect.Plugins;
using Intersect.Plugins.Contexts;

using System;

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
                args.ValidateTypes(typeof(IClientContext), typeof(Plugin));
                return new ClientPluginContext(args[0] as IClientContext, args[1] as Plugin);
            }
        }

        /// <inheritdoc />
        public override IClientLifecycleHelper Lifecycle { get; }

        private IClientContext ClientContext { get; }

        /// <inheritdoc />
        private ClientPluginContext(IClientContext clientContext, Plugin plugin) : base(plugin)
        {
            ClientContext = clientContext;
            Lifecycle = new ClientLifecycleHelper(this);
        }

        /// <inheritdoc />
        public IContentManager ContentManager => ClientContext.GameContext.ContentManager ??
                                                 throw new InvalidOperationException(
                                                     @"Tried accessing the content manager before it was created."
                                                 );
    }
}
