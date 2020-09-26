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
            var assetDictionary = FindDictionaryFor(contentType);
            if (assetDictionary.TryGetValue(assetName, out var asset))
            {
                return asset;
            }

            return default;
        }

        public TAsset Find<TAsset>(ContentType contentType, string assetName) => (TAsset) Find(contentType, assetName);

        public TAsset Find<TAsset>(ContentType contentType, Func<TAsset, bool> predicate) where TAsset : IAsset =>
            FindDictionaryFor(contentType).Values.Select(asset => (TAsset) asset).FirstOrDefault(predicate);

        public void Add(ContentType contentType, params IAsset[] assets) =>
            Add(contentType, assets as IEnumerable<IAsset>);

        public void Add(ContentType contentType, IEnumerable<IAsset> assets)
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

                if (!assetDictionary.ContainsKey(asset.Name))
                {
                    assetDictionary[asset.Name] = asset;
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

                if (assetDictionary.ContainsKey(assetName))
                {
                    assetDictionary.Remove(assetName);
                }
            }
        }

        public void Clear(ContentType contentType) => FindDictionaryFor(contentType).Clear();

        public void ClearAll() => mAssetDictionaries.Keys.ToList().ForEach(Clear);

        public bool Contains(ContentType contentType, string assetName) => FindDictionaryFor(contentType)?.ContainsKey(assetName) ?? false;

        public IList<string> GetAvailableAssetNamesFor(ContentType contentType) =>
            FindDictionaryFor(contentType)?.Keys.ToList() ?? new List<string>();
    }
}
