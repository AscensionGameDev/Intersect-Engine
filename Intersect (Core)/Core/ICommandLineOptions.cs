using System.Collections.Generic;

namespace Intersect.Core
{
    /// <summary>
    /// Declares the common basic command line options for all applications.
    /// </summary>
    public interface ICommandLineOptions
    {
        /// <summary>
        /// The override working directory.
        /// </summary>
        string WorkingDirectory { get; }

        /// <summary>
        /// If the application is in plugin development mode.
        ///
        /// Toggle to allow the plugin loader to load plugins without them being
        /// in their own named sub-directory of one of the plugin directories.
        ///
        /// e.g. instead of /path/to/plugins/MyPlugin/MyPlugin.dll it allows
        /// /path/to/plugin/MyPlugin/bin/Debug/MyPlugin.dll to be loaded
        /// </summary>
        bool IsInPluginDevelopmentMode { get; }

        /// <summary>
        /// Additional plugin directories besides <c>/working/directory/plugins</c>.
        /// </summary>
        IEnumerable<string> PluginDirectories { get; }
    }
}
