using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using Intersect.Extensions;

namespace Intersect.Compression
{
	/// <summary>
	/// Allows for packaging up assets and extracting them from packaged files.
	/// </summary>
	public sealed partial class AssetPacker : IDisposable
	{

		/// <summary>
		/// The location given at the creation of this object defining where we can find our asset packs.
		/// </summary>
		public string PackageLocation { get => packageLocation; }

		/// <summary>
		/// A list of all packages currently being cached.
		/// </summary>
		public List<string> CachedPackages { get => packageCache.Keys.ToList(); }

		/// <summary>
		/// A list of all files contained within the asset packs.
		/// </summary>
		public List<string> FileList { get => fileIndex.Select(i => i.FileName).ToList(); }

		// The index used to retrieve data from our cached packages.
		private List<PackageIndexEntry> fileIndex;

		// The location used to load asset packages from.
		private string packageLocation;

		// Our package cache as well as the timer used to keep track of what to cache.
		private Dictionary<string, PackageCacheEntry> packageCache = new Dictionary<string, PackageCacheEntry>();

		// Our timer defining how long to keep items cached in memory.
		private long cacheTimeOutTimer = 2 * TimeSpan.TicksPerMinute;

		/// <summary>
		/// Create a new instance of <see cref="AssetPacker"/>
		/// </summary>
		/// <param name="indexFile">The index file to load assets from.</param>
		/// <param name="packLocation">The location to load asset packages from.</param>
		public AssetPacker(string indexFile, string packLocation)
		{
			fileIndex = JsonConvert.DeserializeObject<List<PackageIndexEntry>>(GzipCompression.ReadDecompressedString(indexFile));
			packageLocation = packLocation;

			// Load all pack names from this asset file so we can start keeping track of our caches.
			foreach (var pack in fileIndex.Select(i => i.PackName).Distinct().ToArray())
			{
				packageCache.Add(pack, new PackageCacheEntry());
			}
		}

		/// <summary>
		/// Retrieve an asset from the asset packages.
		/// </summary>
		/// <param name="fileName">The asset to retrieve.</param>
		/// <returns>Returns a <see cref="MemoryStream"/> containing the requested file.</returns>
		public MemoryStream GetAsset(string fileName)
		{
			// Find out where to locate our data.
			var asset = findAsset(fileName);

			// Is this file included in our packages?
			if (asset == null)
			{
				return null;
			}

			// Set our access timer.
			packageCache[asset.PackName].LastAccessTime = DateTime.Now.Ticks;

			// Do we already have this data package cached?
			if (packageCache[asset.PackName].StreamCache == null)
			{
				using (var stream = GzipCompression.CreateDecompressedFileStream(Path.Combine(packageLocation, asset.PackName)))
				{
					packageCache[asset.PackName].StreamCache = new MemoryStream();
					stream.CopyTo(packageCache[asset.PackName].StreamCache);
				}
			}

			// Create our buffer.
			var bytes = new byte[asset.FileEndByte - asset.FileStartByte];

			// Search for the file in our cached package.
			packageCache[asset.PackName].StreamCache.Seek(asset.FileStartByte, SeekOrigin.Begin);
			packageCache[asset.PackName].StreamCache.Read(bytes, 0, bytes.Length);

			// Create a new memorystream and write our data to it, then return it.
			var output = new MemoryStream();
			output.Write(bytes, 0, bytes.Length);
			output.Position = 0;
			return output;
		}

		/// <summary>
		/// Checks whether or not an asset is contained within the loaded index file. This check is case insensitive!
		/// </summary>
		/// <param name="fileName">The file to look for.</param>
		/// <returns>Returns whether or not the file was found in the loaded Asset Packs.</returns>
		public bool Contains(string fileName) => findAsset(fileName) != default;

		/// <summary>
		/// Updates our cache timers and disposes of items no longer within the caching time limit.
		/// </summary>
		public void UpdateCache()
		{
			// Cycle through all our packages and check if they've been accessed within our timeout timer. If not, dispose of them.
			foreach (var pack in packageCache)
			{
				if (pack.Value.StreamCache != null && (DateTime.Now.Ticks - pack.Value.LastAccessTime) > cacheTimeOutTimer)
				{
					pack.Value.StreamCache.Dispose();
					pack.Value.StreamCache = default;
				}
			}
		}

		/// <summary>
		/// Find the location of an item in our asset packs.
		/// </summary>
		/// <param name="fileName">The asset file to locate.</param>
		/// <returns>Returns a <see cref="PackageIndexEntry"/> containing relevant information regarding the asset's location in our asset packages.</returns>
		private PackageIndexEntry findAsset(string fileName)
		{
			return fileIndex.Where(d => d.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
		}

		#region IDisposable Implementation
		/// <summary>
		/// Disposes of all cached data in the cache.
		/// </summary>
		public void Dispose()
		{
			foreach (var pack in packageCache)
			{
				if (pack.Value.StreamCache != null)
				{
					pack.Value.StreamCache.Dispose();
					pack.Value.StreamCache = null;
				}
			}
		}
		#endregion

		#region Static Methods
		/// <summary>
		/// Packages up assets according to our settings.
		/// </summary>
		/// <param name="inputDir">The directory to search for assets to package up in.</param>
		/// <param name="inputFilter">The filter to use when searching for assets to pack up.</param>
		/// <param name="outputDir">The directory to place output assets packs and index files.</param>
		/// <param name="indexName">The filename for the index file.</param>
		/// <param name="packPrefix">The file prefix for each asset pack.</param>
		/// <param name="packExt">The file extension for each asset pack.</param>
		/// <param name="batchSize">The maximum size in Megabytes of combined files to pack up in each asset pack.</param>
		public static void PackageAssets(string inputDir, string inputFilter, string outputDir, string indexName, string packPrefix, string packExt, int batchSize)
		{
			// We can't do less than one MB at a time.
			if (batchSize < 1)
			{
				batchSize = 1;
			}

			// Convert to Bytes.
			batchSize = (batchSize * 1024) * 1024;

			// Get our asset file layout so we can just loop through this to create our packs.
			var packs = GeneratePackLayout(inputDir, inputFilter, packPrefix, packExt, batchSize);

			var index = new List<PackageIndexEntry>();
			foreach (var pack in packs)
			{
				using (var stream = GzipCompression.CreateCompressedFileStream(Path.Combine(outputDir, pack.Key)))
				{
					int streamOffset = 0;
					foreach (var item in pack.Value)
					{
						using (var file = File.OpenRead(item.FullPath))
						{
							file.CopyTo(stream);
							index.Add(new PackageIndexEntry() { FileName = Path.GetFileName(item.Name), PackName = pack.Key, FileStartByte = streamOffset, FileEndByte = streamOffset + (int)file.Length });
							streamOffset += (int)file.Length;
						}
					}
				}
			}

			GzipCompression.WriteCompressedString(Path.Combine(outputDir, indexName), JsonConvert.SerializeObject(index));
		}

		private static Dictionary<string, List<PackFileEntry>> GeneratePackLayout(string inputDir, string inputFilter, string packPrefix, string packExt, long batchSize)
        {
			var cache = new Dictionary<string, List<PackFileEntry>>();
			var currentPack = 0;
			long currentSize = 0;

			foreach (var file in new DirectoryInfo(inputDir).GetFiles(inputFilter).OrderByDescending(f => f.Length))
            {
				// Does this file make us go over our determined batch size, but is it not our first file?
				if (currentSize + file.Length > batchSize && currentSize != 0)
                {
					// It's going over and is NOT our first file! Uh-Oh! Move on to the next pack!
					currentSize = 0;
					currentPack++;
                }

				// Get our pack name for simplicity.
				var packname = $"{packPrefix}{currentPack}{packExt}";

				// Check if we already have this pack name in our cache, if not add it!
				if (!cache.ContainsKey(packname))
                {
					cache.Add(packname, new List<PackFileEntry>());
                }

				// Add the file to our cache here.
				cache[packname].Add(new PackFileEntry() { Name = file.Name, FullPath = file.FullName });
				currentSize += file.Length;
            }

			return cache;
        }
		#endregion

		#region Internal classes
		private partial class PackageCacheEntry
		{
			public long LastAccessTime { get; set; }
			public MemoryStream StreamCache { get; set; }
		}

		private partial class PackageIndexEntry
		{
			public string FileName { get; set; }
			public string PackName { get; set; }
			public int FileStartByte { get; set; }
			public int FileEndByte { get; set; }
		}

		private partial class PackFileEntry 
		{ 
			public string Name { get; set; }
			public string FullPath { get; set; }
		}
		#endregion

	}
}
