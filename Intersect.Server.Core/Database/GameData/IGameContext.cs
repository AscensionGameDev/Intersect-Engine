using Intersect.Framework.Core.GameObjects.Animations;
using Intersect.Framework.Core.GameObjects.Crafting;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.Mapping.Tilesets;
using Intersect.Framework.Core.GameObjects.Maps.MapList;
using Intersect.Framework.Core.GameObjects.NPCs;
using Intersect.Framework.Core.GameObjects.PlayerClass;
using Intersect.Framework.Core.GameObjects.Resources;
using Intersect.Framework.Core.GameObjects.Skills;
using Intersect.Framework.Core.GameObjects.Variables;
using Intersect.GameObjects;
using Intersect.Server.Maps;
using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.GameData;

public interface IGameContext : IDbContext
{
    DbSet<AnimationDescriptor> Animations { get; set; }

    DbSet<CraftingRecipeDescriptor> Crafts { get; set; }

    DbSet<CraftingTableDescriptor> CraftingTables { get; set; }

    DbSet<ClassDescriptor> Classes { get; set; }

    DbSet<EventDescriptor> Events { get; set; }

    DbSet<ItemDescriptor> Items { get; set; }

    DbSet<EquipmentProperties> Items_EquipmentProperties { get; set; }

    DbSet<MapController> Maps { get; set; }

    DbSet<MapList> MapFolders { get; set; }

    DbSet<NPCDescriptor> Npcs { get; set; }

    DbSet<ProjectileDescriptor> Projectiles { get; set; }

    DbSet<QuestDescriptor> Quests { get; set; }

    DbSet<ResourceDescriptor> Resources { get; set; }

    DbSet<ShopDescriptor> Shops { get; set; }

    DbSet<SpellDescriptor> Spells { get; set; }

    DbSet<ServerVariableDescriptor> ServerVariables { get; set; }

    DbSet<TilesetDescriptor> Tilesets { get; set; }

    DbSet<DaylightCycleDescriptor> Time { get; set; }

    DbSet<SkillDescriptor> Skills { get; set; }
}
