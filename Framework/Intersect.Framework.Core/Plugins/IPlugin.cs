using Intersect.Plugins.Interfaces;

namespace Intersect.Plugins;

public interface IPlugin
{
    /// <summary>
    /// The <see cref="IManifestHelper"/> that describes this <see cref="Plugin"/>.
    /// </summary>
    IManifestHelper Manifest { get; }

    /// <summary>
    /// The <see cref="ILoggingHelper"/> for this <see cref="Plugin"/>.
    /// </summary>
    ILoggingHelper Logging { get; }

    /// <summary>
    /// The <see cref="PluginConfiguration"/> for this <see cref="Plugin"/>.
    /// </summary>
    PluginConfiguration Configuration { get; }

    /// <inheritdoc cref="PluginConfiguration.IsEnabled" />
    bool IsEnabled { get; internal set; }

    /// <inheritdoc cref="IManifestHelper.Key" />
    string Key { get; }

    /// <inheritdoc />
    int GetHashCode();
}