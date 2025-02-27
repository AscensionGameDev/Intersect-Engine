using Intersect.Framework.Core.GameObjects.Crafting;
using Intersect.Framework.Core.GameObjects.Events;
using Intersect.Framework.Core.GameObjects.Items;
using Intersect.Framework.Core.GameObjects.Maps.MapList;
using Intersect.Framework.Core.GameObjects.NPCs;
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

    DbSet<ProjectileBase> Projectiles { get; set; }

    DbSet<QuestBase> Quests { get; set; }

    DbSet<ResourceBase> Resources { get; set; }

    DbSet<ShopBase> Shops { get; set; }

    DbSet<SpellBase> Spells { get; set; }

    DbSet<ServerVariableDescriptor> ServerVariables { get; set; }

    DbSet<TilesetBase> Tilesets { get; set; }

    DbSet<TimeBase> Time { get; set; }
}
