using Intersect.Enums;
using Intersect.Localization.Common.Descriptors.Maps;

using Newtonsoft.Json;

namespace Intersect.Localization.Common.Descriptors;

public partial class DescriptorsNamespace : LocaleNamespace
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly MapDescriptorNamespace Map = new();

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public readonly LocaleDictionary<GameObjectType, LocalizedPluralString> Names = new()
    {
        { GameObjectType.Animation, new("Animation", "Animations") },
        { GameObjectType.Class, new("Class", "Classes") },
        { GameObjectType.Crafts, new("Craft", "Crafts") },
        { GameObjectType.CraftTables, new("Craft Table", "Craft Tables") },
        { GameObjectType.Event, new("Event", "Events") },
        { GameObjectType.GuildVariable, new("Guild Variable", "Guild Variables") },
        { GameObjectType.Item, new("Item", "Items") },
        { GameObjectType.Map, new("Map", "Maps") },
        { GameObjectType.Npc, new("NPC", "NPCs") },
        { GameObjectType.PlayerVariable, new("Player Variable", "Player Variables") },
        { GameObjectType.Projectile, new("Projectile", "Projectiles") },
        { GameObjectType.Quest, new("Quest", "Quests") },
        { GameObjectType.Resource, new("Resource", "Resources") },
        { GameObjectType.ServerVariable, new("Server Variable", "Server Variables") },
        { GameObjectType.Shop, new("Shop", "Shops") },
        { GameObjectType.Spell, new("Spell", "Spells") },
        { GameObjectType.Tileset, new("Tileset", "Tilesets") },
        { GameObjectType.Time, new("Time", "Times") },
    };
}
