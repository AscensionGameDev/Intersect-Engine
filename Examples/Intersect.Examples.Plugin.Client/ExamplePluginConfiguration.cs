using Intersect.Plugins;

using Newtonsoft.Json;

namespace Intersect.Examples.Plugin.Client
{

    /// <summary>
    /// Example configuration class for a plugin.
    /// </summary>
    internal class ExamplePluginConfiguration : PluginConfiguration
    {

        /// <summary>
        /// Link to discord invite that should open when discord button is clicked
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DiscordInviteUrl { get; set; } = "https://discord.gg/fAwDR5v";

    }

}
