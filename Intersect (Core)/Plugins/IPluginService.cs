
using System.Collections.Generic;

using Intersect.Core;

namespace Intersect.Plugins
{
    /// <summary>
    /// Declares the API surface for the plugin service.
    /// </summary>
    public interface IPluginService : IApplicationService
    {
        /// <summary>
        /// Indexer for looking up loaded <see cref="Plugin"/>s by their key.
        /// </summary>
        /// <param name="pluginKey">the </param>
        /// <returns></returns>
        Plugin this[string pluginKey] { get; }

        /// <summary>
        /// The directories to look for plugins in.
        /// </summary>
        List<string> PluginDirectories { get; }

        /// <summary>
        /// Checks if the plugin for the given key is enabled.
        /// </summary>
        /// <param name="pluginKey">the plugin key string</param>
        /// <returns>if the plugin is enabled (false if it is not even registered)</returns>
        bool IsPluginEnabled(string pluginKey);

        /// <summary>
        /// Enable the <see cref="Plugin"/> with the given key.
        /// </summary>
        /// <param name="pluginKey">the plugin key string</param>
        /// <returns>true if the plugin exists and is now enabled, false otherwise</returns>
        bool EnablePlugin(string pluginKey);

        /// <summary>
        /// Disable the <see cref="Plugin"/> with the given key.
        /// </summary>
        /// <param name="pluginKey">the plugin key string</param>
        /// <returns>true if the plugin exists and is now disabled, false otherwise</returns>
        bool DisablePlugin(string pluginKey);
    }
}
