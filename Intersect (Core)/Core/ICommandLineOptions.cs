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
        /// Additional plugin directories besides <c>/working/directory/plugins</c>.
        /// </summary>
        IEnumerable<string> PluginDirectories { get; }
    }
}
