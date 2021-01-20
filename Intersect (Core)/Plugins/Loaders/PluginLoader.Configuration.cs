using Intersect.Core;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Intersect.Plugins.Loaders
{
    internal partial class PluginLoader
    {
        /// <summary>
        /// Loads configuration for the provided <see cref="Plugin"/>s.
        /// </summary>
        /// <param name="applicationContext">the <see cref="IApplicationContext"/> in which to load plugin configurations</param>
        /// <param name="plugins">the <see cref="Plugin"/>s to load configuration for</param>
        internal void LoadConfigurations(
            IApplicationContext applicationContext,
            IEnumerable<Plugin> plugins
        ) => plugins.ToList()
            .ForEach(
                plugin =>
                {
                    if (plugin != null)
                    {
                        plugin.Configuration = LoadConfiguration(applicationContext, plugin);
                    }
                }
            );

        /// <summary>
        /// Loads configuration for the provided <see cref="Plugin"/>.
        /// </summary>
        /// <param name="applicationContext">the <see cref="IApplicationContext"/> in which to load plugin configuration</param>
        /// <param name="plugin">the <see cref="Plugin"/> to load configuration for</param>
        /// <returns>the <see cref="PluginConfiguration"/> that was loaded (or defaults set) for <paramref name="plugin"/></returns>
        [SuppressMessage(
            "Design", "CA1031:Do not catch general exception types", Justification = "Intentional catch-all-and-logs."
        )]
        internal PluginConfiguration LoadConfiguration(
            IApplicationContext applicationContext,
            Plugin plugin
        )
        {
            var configurationFilePath = plugin.Reference.ConfigurationFile;
            var configuration = new PluginConfiguration {IsEnabled = true};
            string serializedOldConfiguration = null;
            try
            {
                try
                {
                    if (File.Exists(configurationFilePath))
                    {
                        serializedOldConfiguration = File.ReadAllText(configurationFilePath, Encoding.UTF8);
                        configuration = JsonConvert.DeserializeObject(
                                            serializedOldConfiguration, plugin.Reference.ConfigurationType,
                                            new JsonSerializerSettings
                                            {
                                                DefaultValueHandling = DefaultValueHandling.Include
                                            }
                                        ) as PluginConfiguration ??
                                        configuration;
                    }
                }
                catch (Exception exception)
                {
                    applicationContext.Logger.Warn(
                        exception,
                        $"Failed to load plugin configuration from '{configurationFilePath}', using default values."
                    );
                }

                var serializedUpdatedConfiguration = JsonConvert.SerializeObject(configuration, Formatting.Indented);

                var backupConfigurationFilePath = Path.Combine(
                    Path.GetDirectoryName(configurationFilePath),
                    $"{Path.GetFileNameWithoutExtension(configurationFilePath)}.{DateTime.Now:yyyyMMddHHmmss}.backup.json"
                );

                try
                {
                    if (serializedOldConfiguration == null ||
                        !string.Equals(
                            serializedOldConfiguration, serializedUpdatedConfiguration, StringComparison.Ordinal
                        ))
                    {
                        // If the file existed and has changed make a time-stamped backup of the configuration file
                        File.Copy(configurationFilePath, backupConfigurationFilePath);
                    }
                }
                catch (Exception exception)
                {
                    applicationContext.Logger.Warn(
                        exception,
                        $"Failed to backup existing configuration from {configurationFilePath} to {backupConfigurationFilePath}"
                    );
                }

                File.WriteAllText(configurationFilePath, serializedUpdatedConfiguration, Encoding.UTF8);
            }
            catch (Exception exception)
            {
                applicationContext.Logger.Warn(
                    exception, $"Failed to save plugin configuration to '{configurationFilePath}'."
                );
            }

            return configuration;
        }
    }
}
