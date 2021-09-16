using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
using Intersect.Client.Framework.Input;
using Intersect.Client.Framework.Maps;
using Intersect.Client.Framework.Plugins.Interfaces;
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
        
        /// <summary>
        /// The <see cref="IClientNetworkHelper"/> of the current plugin.
        /// </summary>
        IClientNetworkHelper Network { get; }

        /// <summary>
        /// The current <see cref="IGameRenderer"/> of the current client instance.
        /// </summary>
        IGameRenderer Graphics { get; }

        /// <summary>
        /// The current <see cref="IAudioManager"/> of the current client instance.
        /// </summary>
        IAudioManager Audio { get; }

        /// <summary>
        /// The current <see cref="IGameInput"/> of the current client instance.
        /// </summary>
        IGameInput Input { get; }

        /// <summary>
        /// The current <see cref="IMapGrid"/> of the current client instance.
        /// </summary>
        IMapGrid MapGrid { get; }

        /// <summary>
        /// The current <see cref="Options"/> of the current client instance.
        /// </summary>
        Options Options { get; }
    }
}
