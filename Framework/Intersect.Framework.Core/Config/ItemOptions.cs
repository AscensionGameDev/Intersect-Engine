using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Intersect.Config;

/// <summary>
/// Configuration options for items.
/// </summary>
public class ItemOptions
{
    /// <summary>
    /// The available rarity tiers.
    /// </summary>
    /// <remarks>This is not intended to be a localized string, please see Strings for localization.</remarks>
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> RarityTiers { get; set; } =
    [
        @"None",
        @"Common",
        @"Uncommon",
        @"Rare",
        @"Epic",
        @"Legendary",
    ];

    public bool TryGetRarityName(int rarityLevel, [NotNullWhen(true)] out string? rarityName)
    {
        rarityName = RarityTiers.Skip(rarityLevel).FirstOrDefault();
        return rarityName != default;
    }
}
