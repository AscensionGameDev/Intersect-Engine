
using System;
using System.IO;

using Intersect.Plugins;

namespace Intersect.Client.Framework.Content
{

    public interface IContentManager
    {

        TAsset Load<TAsset>(ContentTypes contentType, string assetPath) where TAsset : class, IAsset;

        TAsset Load<TAsset>(ContentTypes contentType, string assetName, Func<Stream> createStream)
            where TAsset : class, IAsset;

        TAsset LoadEmbedded<TAsset>(
            IPluginContext context,
            ContentTypes contentType,
            string assetName
        ) where TAsset : class, IAsset;

    }

}
