using Intersect.Extensions;
using Intersect.Framework.Core.GameObjects.Crafting;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.Variables;
using Intersect.GameObjects;
using Intersect.GameObjects.Maps;

namespace Intersect.Enums;

public enum GameObjectType
{
    [GameObjectInfo(typeof(AnimationDescriptor), "animations")]
    Animation = 0,

    [GameObjectInfo(typeof(ClassDescriptor), "classes")]
    Class,

    [GameObjectInfo(typeof(ItemDescriptor), "items")]
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

    [GameObjectInfo(typeof(CraftingTableDescriptor), "crafting_tables")]
    CraftTables,

    [GameObjectInfo(typeof(CraftingRecipeDescriptor), "crafts")]
    Crafts,

    [GameObjectInfo(typeof(MapBase), "maps")]
    Map,

    [GameObjectInfo(typeof(EventDescriptor), "events")]
    Event,

    [GameObjectInfo(typeof(PlayerVariableDescriptor), "player_variables")]
    PlayerVariable,

    [GameObjectInfo(typeof(ServerVariableDescriptor), "server_variables")]
    ServerVariable,

    [GameObjectInfo(typeof(TilesetBase), "tilesets")]
    Tileset,

    [GameObjectInfo(typeof(TimeBase), "")]
    Time,

    [GameObjectInfo(typeof(GuildVariableDescriptor), "guild_variables")]
    GuildVariable,

    [GameObjectInfo(typeof(UserVariableDescriptor), "user_variables")]
    UserVariable,
}
