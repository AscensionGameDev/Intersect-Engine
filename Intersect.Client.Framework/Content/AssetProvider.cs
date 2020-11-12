using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Intersect.Client.Framework.Content
{
    public abstract class AssetProvider : IAssetProvider
    {
        /// <inheritdoc />
        public abstract bool Exists(AssetReference assetReference);

        /// <inheritdoc />
        public List<AssetReference> IndexAssetsOfType(ContentType contentType)
        {
            var assetNames = FindAssetsOfType(contentType);
            var assetReferences = assetNames.Select(assetName => AsReference(contentType, assetName)).ToList();
            return assetReferences;
        }

        /// <inheritdoc />
        public abstract Stream OpenRead(AssetReference assetReference);

        /// <inheritdoc />
        public abstract AssetReference Resolve(ContentType contentType, string assetName, bool addExtension = true);

        protected abstract AssetReference AsReference(ContentType contentType, string fullName, bool strict = true);

        protected abstract IEnumerable<string> FindAssetsOfType(ContentType contentType);
    }
}
