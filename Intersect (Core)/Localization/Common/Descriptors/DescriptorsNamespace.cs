using Intersect.Enums;
using Intersect.Localization.Common.Descriptors.Maps;

using Newtonsoft.Json;

namespace Intersect.Localization.Common.Descriptors;

public partial class DescriptorsNamespace : LocaleNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString Folder = @"Folder";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocalizedString FolderTooltipOfTheX = @"The folder {00:lower} is filed under.";

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly MapDescriptorNamespace Map = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocaleDictionary<GameObjectType, LocalizedPluralString> Names = new()
    {
        { GameObjectType.Animation, new("Animation") },
        { GameObjectType.Class, new("Class", "Classes") },
        { GameObjectType.Crafts, new("Craft") },
        { GameObjectType.CraftTables, new("Craft Table") },
        { GameObjectType.Event, new("Event") },
        { GameObjectType.GuildVariable, new("Guild Variable") },
        { GameObjectType.Item, new("Item") },
        { GameObjectType.Map, new("Map") },
        { GameObjectType.Npc, new("NPC") },
        { GameObjectType.PlayerVariable, new("Player Variable") },
        { GameObjectType.Projectile, new("Projectile") },
        { GameObjectType.Quest, new("Quest") },
        { GameObjectType.Resource, new("Resource") },
        { GameObjectType.ServerVariable, new("Server Variable") },
        { GameObjectType.Shop, new("Shop") },
        { GameObjectType.Spell, new("Spell") },
        { GameObjectType.Tileset, new("Tileset") },
        { GameObjectType.Time, new("Time") },
    };
}
