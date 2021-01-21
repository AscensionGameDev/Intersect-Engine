using Intersect.Plugins.Interfaces;
using Intersect.Plugins.Manifests.Types;

using Semver;

namespace Intersect.Plugins
{
    internal class VirtualTestManifest : IManifestHelper
    {
        internal static readonly string Namespace = typeof(VirtualTestManifest).Namespace ?? "Intersect.Plugins";

        /// <inheritdoc />
        public string Name { get; } = "Test Manifest";

        /// <inheritdoc />
        public string Key { get; } = "AscensionGameDev.Intersect.Tests";

        /// <inheritdoc />
        public SemVersion Version { get; } = new SemVersion(1, 0, 0);

        /// <inheritdoc />
        public Authors Authors { get; } = new[]
        {
            new Author("Test Author"),
            new Author("Test Author With Email", "test@email.author"),
            new Author("Test Full Author", "test@full.author", "https://fullauthor.example.com")
        };

        /// <inheritdoc />
        public string Homepage { get; } = "https://github.com/AscensionGameDev/Intersect-Engine";
    }
}
