﻿using System.Reflection;

using Intersect.Plugins.Interfaces;

using JetBrains.Annotations;

namespace Intersect.Plugins
{
    /// <summary>
    /// Defines the common required elements for a context between <see cref="IPluginContext"/> and <see cref="IPluginBootstrapContext"/>.
    /// </summary>
    public interface IPluginBaseContext
    {
        /// <summary>
        /// The <see cref="System.Reflection.Assembly"/> of the current plugin.
        /// </summary>
        [NotNull]
        Assembly Assembly { get; }

        /// <summary>
        /// The <see cref="PluginConfiguration"/> of the current plugin.
        /// </summary>
        [NotNull]
        PluginConfiguration Configuration { get; }

        /// <summary>
        /// The <see cref="IEmbeddedResourceHelper"/> for the current plugin.
        /// </summary>
        [NotNull]
        IEmbeddedResourceHelper EmbeddedResources { get; }

        /// <summary>
        /// The <see cref="ILoggingHelper"/> for the current plugin.
        /// </summary>
        [NotNull]
        ILoggingHelper Logging { get; }

        /// <summary>
        /// The <see cref="IManifestHelper"/> for the current plugin.
        /// </summary>
        [NotNull]
        IManifestHelper Manifest { get; }

        /// <summary>
        /// Gets <see cref="Configuration"/> as a <typeparamref name="TConfiguration"/>.
        /// </summary>
        /// <typeparam name="TConfiguration">a subtype of <see cref="PluginConfiguration"/></typeparam>
        /// <returns><see cref="Configuration"/> if it is a <typeparamref name="TConfiguration"/> or null otherwise</returns>
        TConfiguration GetTypedConfiguration<TConfiguration>() where TConfiguration : PluginConfiguration;
    }
}
