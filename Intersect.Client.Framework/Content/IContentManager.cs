using JetBrains.Annotations;

using System;
using System.IO;

using Intersect.Plugins;

namespace Intersect.Client.Framework.Content
{

    public interface IContentManager
    {

        TAsset Load<TAsset>(ContentTypes contentType, [NotNull] string assetPath) where TAsset : class, IAsset;

        TAsset Load<TAsset>(ContentTypes contentType, [NotNull] string assetName, [NotNull] Func<Stream> createStream)
            where TAsset : class, IAsset;

        TAsset LoadEmbedded<TAsset>(
            [NotNull] IPluginContext context,
            ContentTypes contentType,
            [NotNull] string assetName
        ) where TAsset : class, IAsset;

    }

}
