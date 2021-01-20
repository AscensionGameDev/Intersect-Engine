using Intersect.Core;
using Intersect.Factories;
using Intersect.Plugins.Loaders;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Intersect.Plugins
{
    /// <summary>
    /// Implementation of <see cref="IPluginService"/>.
    /// </summary>
    internal sealed class PluginService : ApplicationService<IPluginService, PluginService>, IPluginService
    {
        private static readonly string BuiltInPluginDirectory = Path.Combine("resources", "plugins");

        /// <summary>
        /// Initializes the <see cref="PluginService"/>.
        /// </summary>
        public PluginService()
        {
            if (FactoryRegistry<IPluginBootstrapContext>.Factory == null)
            {
                throw new InvalidOperationException(
                    $@"Factory has not been registered for {nameof(IPluginBootstrapContext)}."
                );
            }

            if (FactoryRegistry<IPluginContext>.Factory == null)
            {
                throw new InvalidOperationException($@"Factory has not been registered for {nameof(IPluginContext)}.");
            }

            PluginDirectories = new List<string>
            {
                BuiltInPluginDirectory
            };

            Plugins = new ConcurrentDictionary<string, Plugin>();
            Instances = new ConcurrentDictionary<Plugin, PluginInstance>();
            Loader = new PluginLoader();
        }

        /// <summary>
        /// Map of all loaded <see cref="Plugin"/>s by their name.
        /// </summary>
        private ConcurrentDictionary<string, Plugin> Plugins { get; }

        /// <summary>
        /// Map of all <see cref="PluginInstance"/>s by their describing <see cref="Plugin"/>.
        /// </summary>
        private ConcurrentDictionary<Plugin, PluginInstance> Instances { get; }

        /// <summary>
        /// Reference to the <see cref="PluginLoader"/> used for discovering and loading <see cref="Plugin"/>s and their configuration.
        /// </summary>
        private PluginLoader Loader { get; }

        /// <inheritdoc />
        public override bool IsEnabled => true;

        /// <inheritdoc />
        public override bool Bootstrap(IApplicationContext applicationContext)
        {
            try
            {
                // Configure the plugin server
                PluginDirectories.AddRange(
                    applicationContext.StartupOptions.PluginDirectories ?? Array.Empty<string>()
                );

                // Discover plugins
                var discoveredPlugins = Loader.DiscoverPlugins(applicationContext, PluginDirectories);

                applicationContext.Logger.Info(
                    $"Discovered {discoveredPlugins.Count} plugins:\n{string.Join("\n", discoveredPlugins.Select(plugin => plugin.Key))}"
                );

                // Load configuration for plugins
                Loader.LoadConfigurations(applicationContext, discoveredPlugins.Values);

                // Register discovered plugins
                RegisterPlugins(discoveredPlugins);

                // Create plugin instances
                CreateInstances();

                // Run bootstrap plugin
                RunOnAllInstances(applicationContext, OnBootstrap);

                return true;
            }
            catch (Exception exception)
            {
                throw new ServiceLifecycleFailureException(ServiceLifecycleStage.Bootstrap, Name, exception);
            }
        }

        /// <inheritdoc />
        protected override void TaskStart(IApplicationContext applicationContext) =>
            RunOnAllInstances(applicationContext, OnStart);

        /// <inheritdoc />
        protected override void TaskStop(IApplicationContext applicationContext)
        {
            RunOnAllInstances(applicationContext, OnStop);

            Instances.Clear();
            Plugins.Clear();
        }

        /// <inheritdoc />
        public Plugin this[string pluginKey] =>
            Plugins.TryGetValue(pluginKey, out var plugin) ? plugin : null;

        /// <inheritdoc />
        public List<string> PluginDirectories { get; }

        /// <inheritdoc />
        public bool IsPluginEnabled(string pluginKey) => this[pluginKey]?.IsEnabled ?? false;

        /// <inheritdoc />
        public bool EnablePlugin(string pluginKey)
        {
            var plugin = this[pluginKey];
            if (plugin?.IsEnabled ?? true)
            {
                return plugin?.IsEnabled ?? false;
            }

            return plugin.IsEnabled = true;
        }

        /// <inheritdoc />
        public bool DisablePlugin(string pluginKey)
        {
            var plugin = this[pluginKey];
            if (!(plugin?.IsEnabled ?? false))
            {
                return !(plugin?.IsEnabled ?? false);
            }

            plugin.IsEnabled = false;
            return true;
        }

        private void RegisterPlugins(IDictionary<string, Plugin> plugins)
        {
            foreach (var pluginEntry in plugins)
            {
                Debug.Assert(pluginEntry.Key != null, "pluginEntry.Key != null");
                if (!Plugins.TryAdd(pluginEntry.Key, pluginEntry.Value))
                {
                    throw new Exception($@"Failed to register plugin: {pluginEntry.Key}");
                }
            }
        }

        private void CreateInstances()
        {
            foreach (var pluginEntry in Plugins)
            {
                var plugin = pluginEntry.Value;
                Debug.Assert(plugin != null, nameof(plugin) + " != null");

                var instance = PluginInstance.Create(plugin);
                if (!Instances.TryAdd(plugin, instance))
                {
                    throw new Exception($@"Failed to add plugin instance: {plugin.Key}");
                }
            }
        }

        [SuppressMessage(
            "Design", "CA1031:Do not catch general exception types",
            Justification = "This is meant to catch, log, and display errors from individual plugins, " +
                            "otherwise the application will crash and burn before logging can occur. " +
                            "Not all exceptions here are fatal to the engine core itself."
        )]
        private void RunOnAllInstances(
            IApplicationContext applicationContext,
            Action<KeyValuePair<Plugin, PluginInstance>> action
        ) =>
            Instances.Where(instance => instance.Key?.IsEnabled ?? false)
                .ToList()
                .ForEach(
                    pair =>
                    {
                        try
                        {
                            action(pair);
                        }
                        catch (Exception exception)
                        {
                            // TODO: Implement something like this for displaying errors to the user
                            // var manifestName = pair.Key.Manifest.Name;
                            // var lifecycleState = action == OnStop
                            //     ? applicationContext.Strings.Errors.PluginLifecycleShutdownLowercase
                            //     : applicationContext.Strings.Errors.PluginLifecycleStartupLowercase;
                            //
                            // applicationContext.UserDisplay.Error(
                            //     applicationContext.Strings.Errors.PluginLifecycleFailed.ToString(lifecycleState)
                            // );

                            applicationContext.Logger.Error(
                                exception, $"Failed to invoke {action.Method?.Name} for {pair.Key.Key}."
                            );
                        }
                    }
                );

        private void OnBootstrap(KeyValuePair<Plugin, PluginInstance> instancePair)
        {
            Debug.Assert(instancePair.Key != null, $@"{nameof(instancePair)}.Key != null");
            Debug.Assert(instancePair.Value != null, $@"{nameof(instancePair)}.Value != null");

            var instance = instancePair.Value;
            var entry = instance.Entry;
            entry.OnBootstrap(instance.BootstrapContext);
        }

        private void OnStart(KeyValuePair<Plugin, PluginInstance> instancePair)
        {
            Debug.Assert(instancePair.Key != null, $@"{nameof(instancePair)}.Key != null");
            Debug.Assert(instancePair.Value != null, $@"{nameof(instancePair)}.Value != null");

            var instance = instancePair.Value;
            var entry = instance.Entry;
            entry.OnStart(instance.Context);
        }

        private void OnStop(KeyValuePair<Plugin, PluginInstance> instancePair)
        {
            Debug.Assert(instancePair.Key != null, $@"{nameof(instancePair)}.Key != null");
            Debug.Assert(instancePair.Value != null, $@"{nameof(instancePair)}.Value != null");

            var instance = instancePair.Value;
            var entry = instance.Entry;
            entry.OnStop(instance.Context);
        }
    }
}
