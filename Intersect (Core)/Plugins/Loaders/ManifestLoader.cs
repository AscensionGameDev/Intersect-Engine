using Intersect.Logging;
using Intersect.Plugins.Interfaces;
using Intersect.Plugins.Manifests;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Intersect.Plugins.Loaders
{
    /// <summary>
    /// Utility class for loading manifests from <see cref="Assembly"/>s.
    /// </summary>
    public static class ManifestLoader
    {
        private static readonly Type ManifestType = typeof(IManifestHelper);

        /// <summary>
        /// Delegate signature for manifest loading functions.
        /// </summary>
        /// <param name="assembly">the <see cref="Assembly"/> to pull the manifest from</param>
        /// <returns>an instance of <see cref="IManifestHelper"/> or null if not found or an error occurred</returns>
        public delegate IManifestHelper ManifestLoaderDelegate(Assembly assembly);

        /// <summary>
        /// Currently registered manifest loading functions.
        /// </summary>
        public static readonly List<ManifestLoaderDelegate> ManifestLoaderDelegates = new List<ManifestLoaderDelegate>
        {
            LoadJsonManifestFrom,
            LoadVirtualManifestFrom
        };

        /// <summary>
        /// Locates manifests in the assembly with priority order declared by <see cref="ManifestLoaderDelegates"/>.
        /// </summary>
        /// <param name="assembly">the <see cref="Assembly"/> to pull the manifest from</param>
        /// <returns>an instance of <see cref="IManifestHelper"/> or null if not found or an error occurred</returns>
        [SuppressMessage(
            "Design", "CA1031:Do not catch general exception types",
            Justification = "This method is supposed to be safe and not leak exceptions upwards, only log them."
        )]
        public static IManifestHelper FindManifest(Assembly assembly)
        {
            if (ManifestLoaderDelegates.Count < 1)
            {
                throw new InvalidOperationException(
                    $"{nameof(ManifestLoaderDelegates)} was initialized with no pre-registered delegates, or the pre-defined delegates were removed and no alternatives were added."
                );
            }

            try
            {
                // This SHOULD NOT be converted into a LINQ expression.
                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var manifestLoaderDelegate in ManifestLoaderDelegates)
                {
                    var manifest = manifestLoaderDelegate?.Invoke(assembly);
                    if (manifest != null)
                    {
                        return manifest;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Exception thrown by manifest loader delegate.");
            }

            return default;
        }

        /// <summary>
        /// Loads a manifest from the <c>manifest.json</c> file embedded in the root of the assembly.
        /// </summary>
        /// <param name="assembly">the <see cref="Assembly"/> to pull the manifest from</param>
        /// <returns>an instance of <see cref="IManifestHelper"/> or null if not found or an error occurred</returns>
        public static IManifestHelper LoadJsonManifestFrom(Assembly assembly)
        {
            try
            {
                var manifestInfo = assembly.GetManifestResourceInfo(@"manifest.json");
                if (manifestInfo == null)
                {
                    return null;
                }

                using (var manifestStream = assembly.GetManifestResourceStream(@"manifest.json"))
                {
                    if (manifestStream == null)
                    {
                        throw new InvalidDataException("Manifest resource stream null when info exists.");
                    }

                    using (var manifestReader = new StreamReader(manifestStream))
                    {
                        var manifestSource = manifestReader.ReadToEnd();

                        if (string.IsNullOrWhiteSpace(manifestSource))
                        {
                            throw new InvalidDataException("Manifest is empty or failed to load and is null.");
                        }

                        return JsonConvert.DeserializeObject<JsonManifest>(manifestSource);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to load manifest.json from {assembly.FullName}.", exception);
            }
        }

        internal static bool IsVirtualManifestType(Type type)
        {
            // Abstract, interface and generic types are not valid virtual manifest types.
            if (type.IsAbstract || type.IsInterface || type.IsGenericType)
            {
                return false;
            }

            if (!ManifestType.IsAssignableFrom(type))
            {
                var mismatchedType = type.GetInterfaces()
                    .FirstOrDefault(
                        interfaceType => string.Equals(
                            ManifestType.FullName, interfaceType.FullName, StringComparison.Ordinal
                        )
                    );

                if (mismatchedType != null)
                {
                    Log.Debug($"Expected: {ManifestType.AssemblyQualifiedName} in {ManifestType.Assembly.Location}");
                    Log.Debug($"Loaded: {mismatchedType.AssemblyQualifiedName} in {mismatchedType.Assembly.Location}");

                    if (!string.Equals(
                        ManifestType.Assembly.Location, mismatchedType.Assembly.Location, StringComparison.Ordinal
                    ))
                    {
                        Log.Warn(
                            $"Manifest loaded the core library from the wrong location." +
                            $"\n\tExpected: {ManifestType.Assembly.Location}" +
                            $"\n\t  Actual: {mismatchedType.Assembly.Location}"
                        );
                    }
                }

                return false;
            }

            if (type.IsValueType)
            {
                return true;
            }

            var constructor = type.GetConstructor(Array.Empty<Type>());
            if (constructor != null)
            {
                return true;
            }

            Log.Debug($"'{type.Name}' is missing a default constructor.");
            return false;
        }

        /// <summary>
        /// Loads a manifest from an assembly that is defined as an explicit class.
        /// </summary>
        /// <param name="assembly">the <see cref="Assembly"/> to pull the manifest from</param>
        /// <returns>an instance of <see cref="IManifestHelper"/> or null if not found or an error occurred</returns>
        public static IManifestHelper LoadVirtualManifestFrom(Assembly assembly)
        {
            if (default == assembly)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            try
            {
                var assemblyTypes = assembly.GetTypes();
                var manifestType = assemblyTypes.FirstOrDefault(IsVirtualManifestType);
                if (default != manifestType)
                {
                    return Activator.CreateInstance(manifestType) as IManifestHelper;
                }
            }
            catch (Exception exception)
            {
                Debugger.Break();
                throw new Exception($"Failed to load virtual manifest from {assembly.FullName}.", exception);
            }

            return default;
        }
    }
}
