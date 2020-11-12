using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Intersect.Client.Framework.Graphics;

namespace Intersect.Client.Framework.Content
{
    public interface IContentManager
    {
        void Clear();

        void Clear(ContentType contentType);

        TAsset Find<TAsset>(ContentType contentType, string assetName) where TAsset : class, IAsset;

        TAsset Find<TAsset>(AssetReference assetReference) where TAsset : class, IAsset;

        IEnumerable<TAsset> FindAll<TAsset>(ContentType contentType) where TAsset : class, IAsset;

        IFont FindFont(string fontName, int fontSize);

        ITexture FindTexture(TextureType textureType, string assetName);

        void IndexAvailableAssets();

        void IndexEmbeddedAssets(Assembly assembly);

        IEnumerable<AssetReference> ListAvailableAssets(ContentType contentType);

        IEnumerable<AssetReference> ListAvailableTextures(TextureType textureType);

        Stream OpenRead(AssetReference assetReference);

        AssetReference Resolve(ContentType contentType, string assetName);

        AssetReference ResolveEmbedded(ContentType contentType, Assembly assembly, string assetName);
    }
}
