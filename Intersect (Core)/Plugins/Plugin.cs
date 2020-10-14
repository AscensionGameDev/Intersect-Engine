using Intersect.Core;
using Intersect.Plugins.Helpers;
using Intersect.Plugins.Interfaces;

using JetBrains.Annotations;

namespace Intersect.Plugins
{
    /// <summary>
    /// Representation of a loaded plugin descriptor.
    /// </summary>
    public sealed class Plugin
    {
        /// <summary>
        /// Create a <see cref="Plugin"/> instance for the given context, manifest and reference.
        /// </summary>
        /// <param name="applicationContext">the <see cref="IApplicationContext"/> the plugin is running in</param>
        /// <param name="manifest">the <see cref="IManifestHelper"/> that describes this plugin</param>
        /// <param name="reference">the <see cref="PluginReference"/> with pre-searched reflection information</param>
        /// <returns></returns>
        internal static Plugin Create(
            [NotNull] IApplicationContext applicationContext,
            [NotNull] IManifestHelper manifest,
            [NotNull] PluginReference reference
        ) => new Plugin(manifest, new LoggingHelper(applicationContext.Logger, manifest), reference);

        // ReSharper disable once NotNullMemberIsNotInitialized
        // Plugin instance is created at the Discovery phase and Configuration is loaded afterwards
        private Plugin(
            [NotNull] IManifestHelper manifest,
            [NotNull] ILoggingHelper logging,
            [NotNull] PluginReference reference
        )
        {
            Manifest = manifest;
            Logging = logging;
            Reference = reference;
        }

        /// <summary>
        /// The <see cref="IManifestHelper"/> that describes this <see cref="Plugin"/>.
        /// </summary>
        [NotNull]
        public IManifestHelper Manifest { get; }

        /// <summary>
        /// The <see cref="ILoggingHelper"/> for this <see cref="Plugin"/>.
        /// </summary>
        [NotNull]
        public ILoggingHelper Logging { get; }

        /// <summary>
        /// The <see cref="PluginConfiguration"/> for this <see cref="Plugin"/>.
        /// </summary>
        [NotNull]
        public PluginConfiguration Configuration { get; internal set; }

        /// <summary>
        /// The <see cref="PluginReference"/> to reflection information for this plugin.
        /// </summary>
        [NotNull]
        internal PluginReference Reference { get; }

        /// <inheritdoc cref="PluginConfiguration.IsEnabled" />
        public bool IsEnabled
        {
            get => Configuration.IsEnabled;
            internal set => Configuration.IsEnabled = value;
        }

        /// <inheritdoc cref="IManifestHelper.Key" />
        [NotNull]
        public string Key => Manifest.Key;

        /// <inheritdoc />
        public override int GetHashCode() => Manifest.Key.GetHashCode();
    }
}
