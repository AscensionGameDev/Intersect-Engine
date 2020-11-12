using System;
using System.Reflection;

namespace Intersect.Client.Framework.Content
{
    /// <summary>
    /// Resolved reference to a loadable asset.
    /// </summary>
    public struct AssetReference : IEquatable<AssetReference>
    {
        /// <summary>
        /// The <see cref="Assembly"/> the asset is in if it is an embedded asset.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// The content type of the asset.
        /// </summary>
        public ContentType ContentType { get; }

        /// <summary>
        /// The non-path name of the asset.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The resolved path of the asset.
        /// </summary>
        public string ResolvedPath { get; }

        /// <summary>
        /// The resolved path of the asset without the extension.
        /// </summary>
        public string ResolvedPathWithoutExtension { get; }

        public AssetReference(Assembly assembly, ContentType contentType, string name, string resolvedPath)
        {
            Assembly = assembly;
            ContentType = contentType;
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : name;
            ResolvedPath = string.IsNullOrWhiteSpace(resolvedPath)
                ? throw new ArgumentNullException(nameof(resolvedPath))
                : resolvedPath;

            var lastIndex = ResolvedPath.LastIndexOf('.');
            ResolvedPathWithoutExtension = ResolvedPath.Substring(0, lastIndex < 0 ? ResolvedPath.Length : lastIndex);
        }

        public override bool Equals(object obj) => obj is AssetReference assetReference && Equals(assetReference);

        public bool Equals(AssetReference other) => string.Equals(
            ResolvedPath, other.ResolvedPath, StringComparison.Ordinal
        );

        public override int GetHashCode() => ResolvedPath.GetHashCode();

        public override string ToString() =>
            $"{(Assembly == null ? $"{Assembly.GetName().Name}, " : "")}{ResolvedPath}";

        public static bool operator ==(AssetReference left, AssetReference right) => left.Equals(right);

        public static bool operator !=(AssetReference left, AssetReference right) => !left.Equals(right);
    }
}
