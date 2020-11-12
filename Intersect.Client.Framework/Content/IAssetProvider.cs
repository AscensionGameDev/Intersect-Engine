using System.Collections.Generic;
using System.IO;

namespace Intersect.Client.Framework.Content
{
    public interface IAssetProvider
    {
        bool Exists(AssetReference assetReference);

        List<AssetReference> IndexAssetsOfType(ContentType contentType);

        Stream OpenRead(AssetReference assetReference);

        AssetReference Resolve(ContentType contentType, string assetName, bool addExtension = true);
    }
}
