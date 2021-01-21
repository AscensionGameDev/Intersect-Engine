using Intersect.Plugins.Interfaces;

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Intersect.Plugins.Helpers
{
    /// <inheritdoc />
    internal sealed class EmbeddedResourceHelper : IEmbeddedResourceHelper
    {
        private Assembly Assembly { get; }

        internal EmbeddedResourceHelper(Assembly assembly)
        {
            Assembly = assembly;
        }

        /// <inheritdoc />
        public bool Exists(string resourceName) => Assembly.GetManifestResourceInfo(Resolve(resourceName)) != null;

        /// <inheritdoc />
        public ManifestResourceInfo GetInfo(string resourceName) =>
            Assembly.GetManifestResourceInfo(Resolve(resourceName)) ??
            throw new InvalidOperationException("Resource exists but info is null.");

        /// <inheritdoc />
        public Stream Read(string resourceName) =>
            Assembly.GetManifestResourceStream(Resolve(resourceName)) ??
            throw new InvalidOperationException("Resource exists but stream is null.");

        /// <inheritdoc />
        public string Resolve(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentNullException(
                    nameof(resourceName), $@"{nameof(resourceName)} cannot be null, empty, or whitespace."
                );
            }

            var partialManifestResourceName = resourceName.Replace("/", ".");
            var manifestResourceName = Assembly.GetManifestResourceNames()
                .Where(
                    potentialManifestResourceName => potentialManifestResourceName.EndsWith(partialManifestResourceName)
                )
                .OrderBy(potentialManifestResourceName => potentialManifestResourceName.Length)
                .FirstOrDefault();

            if (manifestResourceName == default)
            {
                throw new FileNotFoundException($@"Unable to find resource: {resourceName}");
            }

            return manifestResourceName;
        }

        /// <inheritdoc />
        public bool TryGetInfo(string resourceName, out ManifestResourceInfo resourceInfo)
        {
            try
            {
                resourceInfo = GetInfo(resourceName);
                return true;
            }
            catch
            {
                resourceInfo = default;
                return false;
            }
        }

        /// <inheritdoc />
        public bool TryRead(string resourceName, out Stream stream)
        {
            try
            {
                stream = Read(resourceName);
                return true;
            }
            catch
            {
                stream = default;
                return false;
            }
        }

        /// <inheritdoc />
        public bool TryResolve(string resourceName, out string manifestResourceName)
        {
            try
            {
                manifestResourceName = Resolve(resourceName);
                return true;
            }
            catch
            {
                manifestResourceName = default;
                return false;
            }
        }
    }
}
