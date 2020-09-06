using Newtonsoft.Json;

namespace Intersect.Plugins
{
    /// <summary>
    /// The basic plugin configuration class.
    /// </summary>
    public class PluginConfiguration
    {
        /// <summary>
        /// If this plugin is enabled or not.
        /// </summary>
        [JsonProperty]
        public bool IsEnabled { get; internal set; }
    }
}
