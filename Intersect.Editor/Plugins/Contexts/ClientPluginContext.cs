using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Input;
using Intersect.Client.Framework.Maps;
using Intersect.Client.Framework.Plugins.Interfaces;
using Intersect.Client.Plugins;
using Intersect.Client.Plugins.Interfaces;
using Intersect.Editor.General;
using Intersect.Editor.Maps;
using Intersect.Editor.Plugins.Audio;
using Intersect.Editor.Plugins.Helpers;
using Intersect.Factories;
using Intersect.Plugins;
using Intersect.Plugins.Contexts;
using Intersect.Plugins.Interfaces;

namespace Intersect.Editor.Plugins.Contexts
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
                if (args.Length != 2)
                {
                    throw new ArgumentOutOfRangeException(nameof(args), $"{nameof(args)} should have 2 arguments.");
                }

                if (args[0] is not Plugin plugin)
                {
                    throw new ArgumentException($@"First argument needs to be non-null and of type {nameof(Plugin)}.");
                }

                if (args[1] is not IPacketHelper packetHelper)
                {
                    throw new ArgumentException($@"Second argument needs to be non-null and of type {nameof(IPacketHelper)}.");
                }

                return new ClientPluginContext(plugin, packetHelper);
            }
        }

        /// <inheritdoc />
        private ClientPluginContext(Plugin plugin, IPacketHelper packetHelper) : base(plugin)
        {
            Lifecycle = new ClientLifecycleHelper(this);
            Network = new ClientNetworkHelper(packetHelper);
        }

        /// <inheritdoc />
        public IContentManager ContentManager => Globals.ContentManager ??
                                                 throw new InvalidOperationException(
                                                     @"Tried accessing the content manager before it was created."
                                                 );

        /// <inheritdoc />
        public IGameRenderer Graphics => Core.Graphics.Renderer ??
                                                 throw new InvalidOperationException(
                                                     @"Tried accessing the game renderer before it was created."
                                                 );

        /// <inheritdoc />
        public IAudioManager Audio { get; } = new AudioManager();

        /// <inheritdoc />
        public override IClientLifecycleHelper Lifecycle { get; }

        /// <inheritdoc />
        public IClientNetworkHelper Network { get; }

        /// <inheritdoc />
        public IGameInput Input => Globals.InputManager ??
                                                 throw new InvalidOperationException(
                                                     @"Tried accessing the input manager before it was created."
                                                 );
        /// <inheritdoc />
        public Options Options => Options.Instance ??
                                                 throw new InvalidOperationException(
                                                     @"Tried accessing the options instance before it was created."
                                                 );

        public IMapGrid MapGrid { get; } = new MapGrid();
    }
}
