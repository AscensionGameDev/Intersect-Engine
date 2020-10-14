using Intersect.Plugins.Interfaces;
using Intersect.Plugins.Manifests.Types;

using Newtonsoft.Json;

using Semver;

namespace Intersect.Plugins.Manifests
{

    /// <summary>
    /// Represents the structure of an embedded JSON manifest.
    /// </summary>
    public class JsonManifest : IManifestHelper
    {

        /// <inheritdoc />
        [JsonProperty]
        public string Name { get; private set; } = string.Empty;

        /// <inheritdoc />
        [JsonProperty]
        public string Key { get; private set; } = string.Empty;

        /// <inheritdoc />
        [JsonProperty]
        public SemVersion Version { get; private set; } = new SemVersion(1);

        /// <inheritdoc />
        [JsonProperty]
        public Authors Authors { get; private set; }

        /// <inheritdoc />
        [JsonProperty]
        public string Homepage { get; private set; } = string.Empty;

    }

}
