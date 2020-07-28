using Intersect.Logging;

using JetBrains.Annotations;

namespace Intersect.Plugins
{
    /// <summary>
    /// Convenience abstract class that defines commonly used properties for certain plugin helpers.
    /// </summary>
    public abstract class PluginHelper
    {
        /// <summary>
        /// The <see cref="Logger"/> for this helper to use.
        /// </summary>
        [NotNull]
        protected Logger Logger { get; }

        /// <summary>
        /// Initializes this <see cref="PluginHelper"/>.
        /// </summary>
        /// <param name="logger">The <see cref="Logger"/> for this helper to use.</param>
        protected PluginHelper([NotNull] Logger logger)
        {
            Logger = logger;
        }
    }
}
