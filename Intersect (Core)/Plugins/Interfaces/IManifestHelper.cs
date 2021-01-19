using Intersect.Plugins.Manifests.Types;

using Semver;

namespace Intersect.Plugins.Interfaces
{
    /// <summary>
    /// Defines the API for accessing plugin manifest information.
    /// </summary>
    public interface IManifestHelper
    {
        /// <summary>
        /// The end-user visible name of the plugin
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The unique key of the plugin (e.g. AscensionGameDev.Intersect.Client.DiscordButton)
        /// </summary>
        string Key { get; }

        /// <summary>
        /// The version of the plugin
        /// </summary>
        SemVersion Version { get; }

        /// <summary>
        /// The <see cref="Author"/>(s) of the plugin
        /// </summary>
        Authors Authors { get; }

        /// <summary>
        /// The homepage for the plugin
        /// </summary>
        string Homepage { get; }
    }
}
