using System.Collections.Generic;

using Newtonsoft.Json;

namespace Intersect.Config
{
    /// <summary>
    /// Configuration options for items.
    /// </summary>
    public class ItemOptions
    {
        /// <summary>
        /// The available rarity tiers.
        /// </summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<string> RarityTiers { get; set; } = new List<string>
            {
                @"None",
                @"Common",
                @"Uncommon",
                @"Rare",
                @"Epic",
                @"Legendary",
            };
    }
}
