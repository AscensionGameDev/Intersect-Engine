using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Extensions;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Events;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Maps;
using Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.GameObjects.Switches_and_Variables;

namespace Intersect.Migration.UpgradeInstructions.Upgrade_11.Intersect_Convert_Lib.Enums
{
    public enum GameObjectType
    {
        [GameObjectInfo(typeof(AnimationBase), "animations")] Animation = 0,

        [GameObjectInfo(typeof(ClassBase), "classes")] Class,

        [GameObjectInfo(typeof(ItemBase), "items")] Item,

        [GameObjectInfo(typeof(NpcBase), "npcs")] Npc,

        [GameObjectInfo(typeof(ProjectileBase), "projectiles")] Projectile,

        [GameObjectInfo(typeof(QuestBase), "quests")] Quest,

        [GameObjectInfo(typeof(ResourceBase), "resources")] Resource,

        [GameObjectInfo(typeof(ShopBase), "shops")] Shop,

        [GameObjectInfo(typeof(SpellBase), "spells")] Spell,

        [GameObjectInfo(typeof(BenchBase), "crafts")] Bench,

        [GameObjectInfo(typeof(MapBase), "maps")] Map,

        [GameObjectInfo(typeof(EventBase), "events")] CommonEvent,

        [GameObjectInfo(typeof(PlayerSwitchBase), "player_switches")] PlayerSwitch,

        [GameObjectInfo(typeof(PlayerVariableBase), "player_variables")] PlayerVariable,

        [GameObjectInfo(typeof(ServerSwitchBase), "server_switches")] ServerSwitch,

        [GameObjectInfo(typeof(ServerVariableBase), "server_variables")] ServerVariable,

        [GameObjectInfo(typeof(TilesetBase), "tilesets")] Tileset,

        [GameObjectInfo(typeof(TimeBase), "")] Time
    }
}