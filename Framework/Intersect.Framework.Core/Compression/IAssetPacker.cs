namespace Intersect.Compression;

public interface IAssetPacker : IDisposable
{
    /// <summary>
    /// The location given at the creation of this object defining where we can find our asset packs.
    /// </summary>
    string PackageLocation { get; }

    /// <summary>
    /// A list of all packages currently being cached.
    /// </summary>
    List<string> CachedPackages { get; }

    /// <summary>
    /// A list of all files contained within the asset packs.
    /// </summary>
    List<string> FileList { get; }

    /// <summary>
    /// Retrieve an asset from the asset packages.
    /// </summary>
    /// <param name="fileName">The asset to retrieve.</param>
    /// <returns>Returns a <see cref="MemoryStream"/> containing the requested file.</returns>
    MemoryStream GetAsset(string fileName);

    /// <summary>
    /// Checks whether or not an asset is contained within the loaded index file. This check is case insensitive!
    /// </summary>
    /// <param name="fileName">The file to look for.</param>
    /// <returns>Returns whether or not the file was found in the loaded Asset Packs.</returns>
    bool Contains(string fileName);

    /// <summary>
    /// Updates our cache timers and disposes of items no longer within the caching time limit.
    /// </summary>
    void UpdateCache();
}