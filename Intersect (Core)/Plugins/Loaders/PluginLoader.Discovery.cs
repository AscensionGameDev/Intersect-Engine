using Intersect.Core;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Intersect.Plugins.Loaders
{
    /// <summary>
    /// Utility class used for finding, loading, and configuring application plugins.
    /// </summary>
    internal partial class PluginLoader
    {
        /// <summary>
        /// Discover plugins for the current <see cref="IApplicationContext"/> in the specified directories.
        /// Note: Yes, the <see cref="IApplicationContext.StartupOptions"/> exists with <see cref="ICommandLineOptions.PluginDirectories"/>,
        ///     but <paramref name="pluginDirectories"/> exists here because <see cref="PluginService"/> adds a "built-in" directory that is
        ///     scanned regardless of the command line options.
        /// </summary>
        /// <param name="applicationContext">the <see cref="IApplicationContext"/> to discover plugins for</param>
        /// <param name="pluginDirectories">the directories to scan for plugins in</param>
        /// <returns><see cref="IDictionary{TKey, TValue}"/> of plugins, keyed by <see cref="Plugin.Key"/></returns>
        internal IDictionary<string, Plugin> DiscoverPlugins(
            IApplicationContext applicationContext,
            IEnumerable<string> pluginDirectories
        )
        {
            var discoveredPlugins = pluginDirectories.SelectMany(
                pluginDirectory =>
                {
                    Debug.Assert(pluginDirectory != null, nameof(pluginDirectory) + " != null");
                    return DiscoverPlugins(applicationContext, pluginDirectory) ?? Array.Empty<Plugin>();
                }
            );

            var plugins = new Dictionary<string, Plugin>();

            foreach (var discoveredPlugin in discoveredPlugins)
            {
                Debug.Assert(discoveredPlugin != null, $"{nameof(discoveredPlugin)} != null");
                var key = discoveredPlugin.Manifest.Key;
                if (!plugins.TryGetValue(discoveredPlugin.Manifest.Key, out var existingPlugin) ||
                    existingPlugin == default ||
                    existingPlugin.Manifest.Version < discoveredPlugin.Manifest.Version)
                {
                    plugins[key] = discoveredPlugin;
                }
            }

            return plugins;
        }

        /// <summary>
        /// Discovers plugins in a specific <paramref name="pluginDirectory"/>.
        /// </summary>
        /// <param name="applicationContext">the <see cref="IApplicationContext"/> to discover plugins for</param>
        /// <param name="pluginDirectory">the directory to scan for plugins in</param>
        /// <returns><see cref="IEnumerable{T}"/> of plugins</returns>
        internal IEnumerable<Plugin> DiscoverPlugins(
            IApplicationContext applicationContext,
            string pluginDirectory
        )
        {
            if (Directory.Exists(pluginDirectory))
            {
                var pluginFiles = new List<string>();

                pluginFiles.AddRange(
                    Directory.EnumerateDirectories(pluginDirectory)
                        .Select(directoryPath => new DirectoryInfo(directoryPath))
                        .Select(directoryInfo => Path.Combine(directoryInfo.FullName, $"{directoryInfo.Name}.dll"))
                        .Where(File.Exists)
                );

                return pluginFiles.Select(
                        file =>
                        {
                            Debug.Assert(file != null, nameof(file) + " != null");
                            return LoadFrom(applicationContext, file);
                        }
                    )
                    .Where(plugin => plugin != default);
            }

            applicationContext.Logger.Warn(
                $@"Directory was specified as a plugin directory but does not exist: '{pluginDirectory}'"
            );

            return default;
        }
    }
}
