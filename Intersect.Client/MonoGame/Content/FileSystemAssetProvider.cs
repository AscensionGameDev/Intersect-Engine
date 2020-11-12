using System;

using Intersect.Client.Framework.Content;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Intersect.Client.MonoGame.Content
{
    public class FileSystemAssetProvider : AssetProvider
    {
        #region Public API

        /// <inheritdoc />
        public override bool Exists(AssetReference assetReference) => File.Exists(assetReference.ResolvedPath);

        /// <inheritdoc />
        public override Stream OpenRead(AssetReference assetReference) => File.OpenRead(assetReference.ResolvedPath);

        /// <inheritdoc />
        public override AssetReference Resolve(ContentType contentType, string assetName, bool addExtension = true)
        {
            var cleanAssetName = Path.GetFileNameWithoutExtension(assetName);
            var assetNameWithExtension = addExtension ? $"{assetName}.{contentType.GetExtension()}" : assetName;
            return AsReference(contentType, cleanAssetName, ResolveAssetPath(contentType, assetNameWithExtension));
        }

        #endregion Public API

        #region Protected API

        /// <inheritdoc />
        protected override AssetReference AsReference(ContentType contentType, string fullName, bool strict = true) =>
            AsReference(contentType, Path.GetFileNameWithoutExtension(fullName), fullName);

        /// <inheritdoc />
        protected override IEnumerable<string> FindAssetsOfType(ContentType contentType)
        {
            var directoryPath = EnsureDirectory(contentType);
            var extension = $".{contentType.GetExtension()}";
            return Directory.EnumerateFiles(directoryPath, $"*{extension}").Select(CleanPath);
        }

        #endregion Protected API

        #region Internal Helper Functions

        private AssetReference AsReference(ContentType contentType, string assetName, string fullName) =>
            new AssetReference(null, contentType, assetName, fullName);

        private string EnsureDirectory(ContentType contentType)
        {
            var resolvedPath = ResolveDirectory(contentType);

            if (!Directory.Exists(resolvedPath))
            {
                Directory.CreateDirectory(resolvedPath);
            }

            return resolvedPath;
        }

        private string CleanPath(string path) => path.Replace('\\', '/');

        private string ResolveAssetPath(ContentType contentType, string fullName) =>
            CleanPath(Path.Combine(ResolveDirectory(contentType), fullName));

        private string ResolveDirectory(ContentType contentType) =>
            CleanPath(Path.Combine("resources", contentType.GetDirectory()));

        #endregion Internal Helper Functions
    }
}
