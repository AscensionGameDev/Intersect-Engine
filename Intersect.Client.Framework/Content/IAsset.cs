namespace Intersect.Client.Framework.Content
{
    /// <summary>
    /// Declares the API for assets.
    /// </summary>
    public interface IAsset
    {
        /// <summary>
        /// The name of the asset.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The reference to the asset including type, name and resolved path.
        /// </summary>
        AssetReference Reference { get; }
    }
}
