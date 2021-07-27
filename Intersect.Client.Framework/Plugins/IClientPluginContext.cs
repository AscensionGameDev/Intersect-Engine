using Intersect.Client.Framework.Audio;
using Intersect.Client.Framework.Content;
using Intersect.Client.Framework.Graphics;
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
        IGameRenderer GameRenderer { get; }

        /// <summary>
        /// The current <see cref="IGameAudioManager"/> of the current client instance.
        /// </summary>
        IGameAudioManager GameAudioManager { get; }
    }
}
