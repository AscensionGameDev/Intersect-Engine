namespace Intersect.Config
{
    /// <summary>
    /// Contains configurable options pertaining to the way loot (item) drops are handled by the engine.
    /// </summary>
    public class LootOptions
    {

        /// <summary>
        /// Defines how long (in ms) loot will be available for picking up on the map.
        /// </summary>
        public int ItemDespawnTime = 15000;

        /// <summary>
        /// Defines how long (in ms) an item drop will be ''owned'' by a player and their party.
        /// </summary>
        public int ItemOwnershipTime = 5000;

        /// <summary>
        /// Defines whether players can see items they do not ''own'' on the map.
        /// </summary>
        public bool ShowUnownedItems = false;

        /// <summary>
        /// Defines whether or not spawning items on the map from anything but the map attribute should consolidate it into one item.
        /// When false, will drop multiple items with a quantity of 1, rather than a single item with a higher quantity for non-stackable items.
        /// </summary>
        public bool ConsolidateMapDrops = true;
    }
}
