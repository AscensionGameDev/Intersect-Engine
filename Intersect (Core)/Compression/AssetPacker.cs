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
	public sealed class AssetPacker : IDisposable
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
		public bool Contains(string fileName) => findAsset(fileName) != null ? true : false;

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
					pack.Value.StreamCache = null;
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
		/// <param name="batchSize">The maximum amount of files to pack up in each asset pack.</param>
		public static void PackageAssets(string inputDir, string inputFilter, string outputDir, string indexName, string packPrefix, string packExt, int batchSize)
		{
			// We can't do less than one file at a time.
			if (batchSize < 1)
			{
				batchSize = 1;
			}

			var fileBatches = Directory.GetFiles(inputDir, inputFilter).Split(batchSize).ToArray();
			var index = new List<PackageIndexEntry>();
			for (var n = 0; n < fileBatches.Length; n++)
			{
				using (var stream = GzipCompression.CreateCompressedFileStream(Path.Combine(outputDir, string.Format("{00}{01}{02}", packPrefix, n, packExt))))
				{
					int streamOffset = 0;
					foreach (var item in fileBatches[n])
					{
						using (var file = File.OpenRead(item))
						{
							file.CopyTo(stream);
							index.Add(new PackageIndexEntry() { FileName = Path.GetFileName(item), PackName = string.Format("{00}{01}{02}", packPrefix, n, packExt), FileStartByte = streamOffset, FileEndByte = streamOffset + (int)file.Length });
							streamOffset += (int)file.Length;
						}
					}
				}
			}
			GzipCompression.WriteCompressedString(Path.Combine(outputDir, indexName), JsonConvert.SerializeObject(index));
		}
		#endregion

		#region Internal classes
		private class PackageCacheEntry
		{
			public long LastAccessTime { get; set; }
			public MemoryStream StreamCache { get; set; }
		}
		private class PackageIndexEntry
		{
			public string FileName { get; set; }
			public string PackName { get; set; }
			public int FileStartByte { get; set; }
			public int FileEndByte { get; set; }
		}
		#endregion

	}
}
