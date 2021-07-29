using Intersect.Plugins;

namespace Intersect.Client.Framework.Content
{

    public interface IContentManager
    {
        /// <summary>
        /// Find a specified Asset from within the Content Manager.
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="contentType">The type of asset to try and search for.</param>
        /// <param name="assetName">The name/alias of the asset to try and search for.</param>
        /// <returns>Returns the specified <see cref="TAsset"/> if it exists, if not returns <see cref="null"/></returns>
        TAsset Find<TAsset>(ContentTypes contentType, string assetName) where TAsset : class, IAsset;

        /// <summary>
        /// Load an asset from disk by the specified file.
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="contentType">The type of asset to load.</param>
        /// <param name="assetPath">The path of the asset file to load.</param>
        /// <param name="assetAlias">The alias to give to the asset for future reference.</param>
        /// <returns>Returns an instance of <see cref="TAsset"/></returns>
        TAsset Load<TAsset>(ContentTypes contentType, string assetPath, string assetAlias) where TAsset : class, IAsset;

        /// <summary>
        /// Load an asset embedded from within the binary.
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="context">The <see cref="IPluginContext"/> to load the embedded asset from.</param>
        /// <param name="contentType">The type of asset to load./param>
        /// <param name="assetName">The name of the file within the binary's manifest file.</param>
        /// <returns>Returns an instance of <see cref="TAsset"/></returns>
        TAsset LoadEmbedded<TAsset>(
            IPluginContext context,
            ContentTypes contentType,
            string assetName
        ) where TAsset : class, IAsset;

        /// <summary>
        /// Load an asset embedded from within the binary.
        /// </summary>
        /// <typeparam name="TAsset"></typeparam>
        /// <param name="context">The <see cref="IPluginContext"/> to load the embedded asset from.</param>
        /// <param name="contentType">The type of asset to load./param>
        /// <param name="assetName">The name of the file within the binary's manifest file.</param>
        /// <param name="assetAlias">The alias to give to the asset for future reference.</param>
        /// <returns>Returns an instance of <see cref="TAsset"/></returns>
        TAsset LoadEmbedded<TAsset>(
            IPluginContext context,
            ContentTypes contentType,
            string assetName,
            string assetAlias
        ) where TAsset : class, IAsset;

    }

}
