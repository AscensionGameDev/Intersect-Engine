using Intersect.Core;
using Intersect.Properties;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Intersect.Plugins.Loaders
{
    internal sealed partial class PluginLoader
    {
        /// <summary>
        /// Load a <see cref="Plugin"/> from the specified <see cref="Assembly"/> path.
        /// </summary>
        /// <param name="applicationContext">the <see cref="IApplicationContext"/> in which to load the plugin</param>
        /// <param name="assemblyPath">the path to the <see cref="Assembly"/> to load the <see cref="Plugin"/> from</param>
        /// <returns>a <see cref="Plugin"/> or null if one cannot be found</returns>
        [SuppressMessage(
            "Design", "CA1031:Do not catch general exception types", Justification = "Intentional catch-all-and-log."
        )]
        internal Plugin LoadFrom(IApplicationContext applicationContext, string assemblyPath)
        {
            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                if (assembly == null)
                {
                    throw new DllNotFoundException(ExceptionMessages.MissingPluginAssembly);
                }

                return LoadFrom(applicationContext, assembly);
            }
            catch (Exception exception)
            {
                applicationContext.Logger.Error(exception);
                return default;
            }
        }

        /// <summary>
        /// Load a <see cref="Plugin"/> from the specified <see cref="Assembly"/>.
        /// </summary>
        /// <param name="applicationContext">the <see cref="IApplicationContext"/> in which to load the plugin</param>
        /// <param name="assembly">the <see cref="Assembly"/> to load the <see cref="Plugin"/> from</param>
        /// <returns>a <see cref="Plugin"/> or null if one cannot be found</returns>
        internal Plugin LoadFrom(IApplicationContext applicationContext, Assembly assembly)
        {
            var manifest = ManifestLoader.FindManifest(assembly);
            if (manifest == null)
            {
                applicationContext.Logger.Warn($"Unable to find a manifest in '{assembly.FullName}' ({assembly.Location})");
                return default;
            }

            applicationContext.Logger.Info($"Loading plugin {manifest.Name} v{manifest.Version} ({manifest.Key}).");

            var pluginReference = CreatePluginReference(assembly);
            if (pluginReference != null)
            {
                return Plugin.Create(applicationContext, manifest, pluginReference);
            }

            applicationContext.Logger.Error($"Unable to find a plugin entry point in '{assembly.FullName}' ({assembly.Location})");
            return default;
        }
    }
}
