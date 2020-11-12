using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Intersect.Client.Framework.Content
{
    public class EmbeddedAssetProvider : AssetProvider
    {
        #region Static Asset Matchers

        private static Dictionary<ContentType, Regex> FuzzyMatchers { get; }

        private static Dictionary<ContentType, Regex> StrictMatchers { get; }

        static EmbeddedAssetProvider()
        {
            FuzzyMatchers = Enum.GetValues(typeof(ContentType))
                .Cast<ContentType>()
                .ToDictionary(
                    contentType => contentType,
                    contentType => new Regex(
                        $"^.+\\.resources\\.{contentType.GetDirectory()}\\.(.+)\\.[^\\.]+$", RegexOptions.Compiled
                    )
                );

            StrictMatchers = Enum.GetValues(typeof(ContentType))
                .Cast<ContentType>()
                .ToDictionary(
                    contentType => contentType,
                    contentType => new Regex(
                        $"^.+\\.resources\\.{contentType.GetDirectory()}\\.(.+)\\.{contentType.GetExtension()}$",
                        RegexOptions.Compiled
                    )
                );
        }

        #endregion Static Asset Matchers

        #region Properties and Constructors

        private Assembly Assembly { get; }

        public EmbeddedAssetProvider(Assembly assembly)
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        #endregion Properties and Constructors

        #region Public API

        /// <inheritdoc />
        public override bool Exists(AssetReference assetReference) =>
            Assembly.GetManifestResourceInfo(assetReference.ResolvedPath) != null;

        /// <inheritdoc />
        public override Stream OpenRead(AssetReference assetReference) =>
            Assembly.GetManifestResourceStream(assetReference.ResolvedPath);

        /// <inheritdoc />
        public override AssetReference Resolve(ContentType contentType, string assetName, bool addExtension = true)
        {
            var partialAssetName = $".resources.{contentType.GetDirectory()}.{assetName}";
            if (addExtension)
            {
                partialAssetName = $"{partialAssetName}.{contentType.GetExtension()}";
            }

            var manifestAssetNames = Assembly.GetManifestResourceNames();
            var matchingAssetNames = manifestAssetNames
                .Where(manifestAssetName => assetName.EndsWith(partialAssetName, StringComparison.Ordinal))
                .ToList();

            if (matchingAssetNames.Count < 1)
            {
                throw new Exception($"Found no matching assets for {assetName} ({contentType}).");
            }

            if (matchingAssetNames.Count > 1)
            {
                throw new Exception(
                    $"Found multiple matching assets for {assetName} ({contentType}):\n{string.Join("\n", matchingAssetNames)}"
                );
            }

            var matchingAssetName = matchingAssetNames.FirstOrDefault();
            return AsReference(contentType, matchingAssetName, !addExtension);
        }

        #endregion Public API

        #region Protected API

        /// <inheritdoc />
        protected override AssetReference AsReference(ContentType contentType, string fullName, bool strict = true)
        {
            var matcher = (strict ? StrictMatchers : FuzzyMatchers)[contentType];
            var match = matcher.Match(fullName);

            if (!match.Success)
            {
                throw new ArgumentException("Invalid asset name.", nameof(fullName));
            }

            return new AssetReference(Assembly, contentType, match.Groups[1].Value, fullName);
        }

        /// <inheritdoc />
        protected override IEnumerable<string> FindAssetsOfType(ContentType contentType)
        {
            var manifestAssetNames = Assembly.GetManifestResourceNames();
            var matcher = StrictMatchers[contentType];
            var matchingAssetNames = manifestAssetNames.Where(manifestAssetName => matcher.IsMatch(manifestAssetName))
                .ToList();

            return matchingAssetNames;
        }

        #endregion Protected API
    }
}
