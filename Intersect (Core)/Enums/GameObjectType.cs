using Intersect.Extensions;
using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps;

namespace Intersect.Enums
{

    public enum GameObjectType
    {

        [GameObjectInfo(typeof(AnimationBase), "animations")]
        Animation = 0,

        [GameObjectInfo(typeof(ClassBase), "classes")]
        Class,

        [GameObjectInfo(typeof(ItemBase), "items")]
        Item,

        [GameObjectInfo(typeof(NpcBase), "npcs")]
        Npc,

        [GameObjectInfo(typeof(ProjectileBase), "projectiles")]
        Projectile,

        [GameObjectInfo(typeof(QuestBase), "quests")]
        Quest,

        [GameObjectInfo(typeof(ResourceBase), "resources")]
        Resource,

        [GameObjectInfo(typeof(ShopBase), "shops")]
        Shop,

        [GameObjectInfo(typeof(SpellBase), "spells")]
        Spell,

        [GameObjectInfo(typeof(CraftingTableBase), "crafting_tables")]
        CraftTables,

        [GameObjectInfo(typeof(CraftBase), "crafts")]
        Crafts,

        [GameObjectInfo(typeof(MapBase), "maps")]
        Map,

        [GameObjectInfo(typeof(EventBase), "events")]
        Event,

        [GameObjectInfo(typeof(PlayerVariableBase), "player_variables")]
        PlayerVariable,

        [GameObjectInfo(typeof(ServerVariableBase), "server_variables")]
        ServerVariable,

        [GameObjectInfo(typeof(TilesetBase), "tilesets")]
        Tileset,

        [GameObjectInfo(typeof(TimeBase), "")] Time,

        [GameObjectInfo(typeof(GuildVariableBase), "guild_variables")]
        GuildVariable,

    }

}
