using Intersect.Extensions;
using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.Framework.Core.GameObjects.Crafting;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.Mapping.Tilesets;
using Intersect.Framework.Core.GameObjects.Maps;
using Intersect.Framework.Core.GameObjects.NPCs;
using Intersect.Framework.Core.GameObjects.PlayerClass;
using Intersect.Framework.Core.GameObjects.Resources;
using Intersect.Framework.Core.GameObjects.Skills;
using Intersect.Framework.Core.GameObjects.Variables;
using Intersect.GameObjects;

namespace Intersect.Enums;

public enum GameObjectType
{
    [GameObjectInfo(typeof(AnimationDescriptor), "animations")]
    Animation = 0,

    [GameObjectInfo(typeof(ClassDescriptor), "classes")]
    Class,

    [GameObjectInfo(typeof(ItemDescriptor), "items")]
    Item,

    [GameObjectInfo(typeof(NPCDescriptor), "npcs")]
    Npc,

    [GameObjectInfo(typeof(ProjectileDescriptor), "projectiles")]
    Projectile,

    [GameObjectInfo(typeof(QuestDescriptor), "quests")]
    Quest,

    [GameObjectInfo(typeof(ResourceDescriptor), "resources")]
    Resource,

    [GameObjectInfo(typeof(ShopDescriptor), "shops")]
    Shop,

    [GameObjectInfo(typeof(SpellDescriptor), "spells")]
    Spell,

    [GameObjectInfo(typeof(CraftingTableDescriptor), "crafting_tables")]
    CraftTables,

    [GameObjectInfo(typeof(CraftingRecipeDescriptor), "crafts")]
    Crafts,

    [GameObjectInfo(typeof(MapDescriptor), "maps")]
    Map,

    [GameObjectInfo(typeof(EventDescriptor), "events")]
    Event,

    [GameObjectInfo(typeof(PlayerVariableDescriptor), "player_variables")]
    PlayerVariable,

    [GameObjectInfo(typeof(ServerVariableDescriptor), "server_variables")]
    ServerVariable,

    [GameObjectInfo(typeof(TilesetDescriptor), "tilesets")]
    Tileset,

    [GameObjectInfo(typeof(DaylightCycleDescriptor), "")]
    Time,

    [GameObjectInfo(typeof(GuildVariableDescriptor), "guild_variables")]
    GuildVariable,

    [GameObjectInfo(typeof(UserVariableDescriptor), "user_variables")]
    UserVariable,

    [GameObjectInfo(typeof(SkillDescriptor), "skills")]
    Skill,
}
