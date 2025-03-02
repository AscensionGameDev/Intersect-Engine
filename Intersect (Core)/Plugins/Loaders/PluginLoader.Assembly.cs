using Intersect.Core;
using Intersect.Properties;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;

namespace Intersect.Plugins.Loaders;

internal sealed partial class PluginLoader
{
    private sealed class PluginLoadContext(FileInfo pluginFileInfo) : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _dependencyResolver = new(pluginFileInfo.FullName);

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var assemblyPath = _dependencyResolver.ResolveAssemblyToPath(assemblyName);

            Assembly? assembly;
            if (assemblyPath != null)
            {
                try
                {
                    assembly = LoadFromAssemblyPath(assemblyPath);
                    return assembly;
                }
                catch (Exception exception)
                {
                    ApplicationContext.CurrentContext.Logger.LogError(
                        exception,
                        "Failed to resolve '{AssemblyName}' from plugin: {PluginPath}",
                        assemblyName,
                        Path.GetRelativePath(Environment.CurrentDirectory, pluginFileInfo.FullName)
                    );
                }
            }

            try
            {
                assembly = Assembly.Load(assemblyName);
                ApplicationContext.CurrentContext.Logger.LogDebug(
                    "Found '{AssemblyName}' using fallback resolution",
                    assemblyName
                );
                return assembly;
            }
            catch (Exception exception)
            {
                ApplicationContext.CurrentContext.Logger.LogError(
                    exception,
                    "Failed to resolve '{AssemblyName}' using fallback resolution",
                    assemblyName
                );

                return null;
            }
        }

        protected override IntPtr LoadUnmanagedDll(string nativeLibraryName)
        {
            var nativeLibraryPath = _dependencyResolver.ResolveUnmanagedDllToPath(nativeLibraryName);
            if (nativeLibraryPath != null)
            {
                return LoadUnmanagedDllFromPath(nativeLibraryPath);
            }

            ApplicationContext.CurrentContext.Logger.LogError(
                "Failed to resolve '{AssemblyName}' from plugin: {PluginPath}",
                nativeLibraryName,
                Path.GetRelativePath(Environment.CurrentDirectory, pluginFileInfo.FullName)
            );

            // TODO: Fallback for native libraries
            return default;
        }
    }

    /// <summary>
    /// Load a <see cref="Plugin"/> from the specified <see cref="Assembly"/> path.
    /// </summary>
    /// <param name="applicationContext">the <see cref="IApplicationContext"/> in which to load the plugin</param>
    /// <param name="assemblyPath">the path to the <see cref="Assembly"/> to load the <see cref="Plugin"/> from</param>
    /// <returns>a <see cref="Plugin"/> or null if one cannot be found</returns>
    internal Plugin LoadFrom(IApplicationContext applicationContext, string assemblyPath)
    {
        try
        {
            FileInfo assemblyFileInfo = new(assemblyPath);
            PluginLoadContext pluginLoadContext = new(assemblyFileInfo);
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyPath);

            var assembly = pluginLoadContext.LoadFromAssemblyName(assemblyName);
            return LoadFrom(applicationContext, assembly);
        }
        catch (Exception exception)
        {
            applicationContext.Logger.LogError(exception, "Error loading plugin from {AssemblyPath}", assemblyPath);
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
            applicationContext.Logger.LogWarning($"Unable to find a manifest in '{assembly.FullName}' ({assembly.Location})");
            return default;
        }

        applicationContext.Logger.LogInformation($"Loading plugin {manifest.Name} v{manifest.Version} ({manifest.Key}).");

        var pluginReference = CreatePluginReference(assembly);
        if (pluginReference != null)
        {
            return Plugin.Create(applicationContext, manifest, pluginReference);
        }

        applicationContext.Logger.LogError($"Unable to find a plugin entry point in '{assembly.FullName}' ({assembly.Location})");
        return default;
    }
}
