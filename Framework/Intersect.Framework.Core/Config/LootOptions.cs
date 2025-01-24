namespace Intersect.Config;

/// <summary>
/// Contains configurable options pertaining to the way loot (item) drops are handled by the engine.
/// </summary>
public partial class LootOptions
{
    /// <summary>
    /// Defines how long (in ms) loot will be available for picking up on the map.
    /// </summary>
    public int ItemDespawnTime { get; set; } = 15000;

    /// <summary>
    /// Defines how long (in ms) an item drop will be ''owned'' by a player and their party.
    /// </summary>
    public int ItemOwnershipTime { get; set; } = 5000;

    /// <summary>
    /// Defines whether players can see items they do not ''own'' on the map.
    /// </summary>
    public bool ShowUnownedItems { get; set; } = false;

    /// <summary>
    /// Defines whether spawning items on the map from anything but the map attribute should consolidate it into one item.
    /// When false, will drop multiple items with a quantity of 1, rather than a single item with a higher quantity for non-stackable items.
    /// </summary>
    public bool ConsolidateMapDrops { get; set; } = true;

    /// <summary>
    /// Configures whether the loot window feature of the client is allowed to be active.
    /// Note, there's no such setting on the client so enabling it here enables it there.
    /// </summary>
    public bool EnableLootWindow { get; set; } = true;

    /// <summary>
    /// Configures the maximum amount of items to be displayed on the client's Loot Window.
    /// </summary>
    public byte MaximumLootWindowItems { get; set; } = 10;

    /// <summary>
    /// Configures the maximum amount of tiles around you to search for loot with the Loot Window.
    /// </summary>
    public byte MaximumLootWindowDistance { get; set; } = 3;

    /// <summary>
    /// When killing an NPC which spawns individualized loot for all of its attackers, include loot for all their party members, even if they didn't participate (deal damage) to the npc
    /// </summary>
    public bool IndividualizedLootAutoIncludePartyMembers { get; set; } = false;
}
