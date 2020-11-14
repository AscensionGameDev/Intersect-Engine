using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Client.Framework.Content
{
    /// <summary>
    /// Asset reference management and lookup by content type.
    /// </summary>
    public class AssetLookup
    {
        private readonly IDictionary<ContentType, IDictionary<string, IAsset>> mAssetDictionaries;

        public AssetLookup()
        {
            mAssetDictionaries = new ConcurrentDictionary<ContentType, IDictionary<string, IAsset>>();
        }

        protected string CreateAssetKey(string assetName) => assetName?.ToUpperInvariant();

        protected IDictionary<string, IAsset> FindDictionaryFor(ContentType contentType)
        {
            if (mAssetDictionaries.TryGetValue(contentType, out var assetDictionary))
            {
                return assetDictionary;
            }

            assetDictionary = new ConcurrentDictionary<string, IAsset>();
            mAssetDictionaries[contentType] = assetDictionary;
            return assetDictionary;
        }

        public IAsset Find(ContentType contentType, string assetName)
        {
            var assetKey = CreateAssetKey(assetName);
            var assetDictionary = FindDictionaryFor(contentType);
            if (assetDictionary.TryGetValue(assetKey, out var asset))
            {
                return asset;
            }

            return default;
        }

        public TAsset Find<TAsset>(ContentType contentType, string assetName) => string.IsNullOrWhiteSpace(assetName)
            ? default
            : (TAsset) Find(contentType, assetName);

        public IEnumerable<TAsset> FindAll<TAsset>(ContentType contentType, Func<TAsset, bool> predicate)
            where TAsset : IAsset
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var assets = GetAssets<TAsset>(contentType);
            return assets.Where(predicate);
        }

        public TAsset Find<TAsset>(ContentType contentType, Func<TAsset, bool> predicate) where TAsset : IAsset
        {
            var assets = FindDictionaryFor(contentType).Values.Cast<TAsset>();
            var asset = assets.FirstOrDefault(predicate);
            return asset;
        }

        public void Add<TAsset>(ContentType contentType, params TAsset[] assets) where TAsset : IAsset =>
            Add(contentType, assets as IEnumerable<TAsset>);

        public void Add<TAsset>(ContentType contentType, IEnumerable<TAsset> assets) where TAsset : IAsset
        {
            if (assets == null)
            {
                return;
            }

            var assetDictionary = FindDictionaryFor(contentType);
            foreach (var asset in assets)
            {
                if (string.IsNullOrWhiteSpace(asset.Name))
                {
                    throw new ArgumentException("One or more assets have null or whitespace names.", nameof(assets));
                }

                var assetKey = CreateAssetKey(asset.Name);
                if (!assetDictionary.ContainsKey(assetKey))
                {
                    assetDictionary[assetKey] = asset;
                }
            }
        }

        public void Remove(ContentType contentType, params IAsset[] assets) =>
            Remove(contentType, assets as IEnumerable<IAsset>);

        public void Remove(ContentType contentType, IEnumerable<IAsset> assets) =>
            Remove(contentType, assets.Select(asset => asset.Name));

        public void Remove(ContentType contentType, params string[] assetNames) =>
            Remove(contentType, assetNames as IEnumerable<string>);

        public void Remove(ContentType contentType, IEnumerable<string> assetNames)
        {
            if (assetNames == null)
            {
                return;
            }

            var assetDictionary = FindDictionaryFor(contentType);
            foreach (var assetName in assetNames)
            {
                if (string.IsNullOrWhiteSpace(assetName))
                {
                    throw new ArgumentException("One or more assets names are null or whitespace.", nameof(assetNames));
                }

                var assetKey = CreateAssetKey(assetName);
                if (assetDictionary.ContainsKey(assetKey))
                {
                    assetDictionary.Remove(assetKey);
                }
            }
        }

        public void Clear(ContentType contentType) => FindDictionaryFor(contentType).Clear();

        public void ClearAll() => mAssetDictionaries.Keys.ToList().ForEach(Clear);

        public bool Contains(ContentType contentType, string assetName) =>
            FindDictionaryFor(contentType)?.ContainsKey(CreateAssetKey(assetName)) ?? false;

        public IEnumerable<TAsset> GetAssets<TAsset>(ContentType contentType) where TAsset : IAsset =>
            FindDictionaryFor(contentType)?.Values.Cast<TAsset>();

        public IEnumerable<AssetReference> ListAssets(ContentType contentType) =>
            FindDictionaryFor(contentType)?.Values.Select(asset => asset.Reference);

        public IList<string> GetAvailableAssetNamesFor(ContentType contentType) =>
            FindDictionaryFor(contentType)?.Keys.ToList() ?? new List<string>();
    }
}
