using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.Models;
using Intersect.Server.Maps;

using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.GameData;

public interface IGameContext : IDbContext
{
    DbSet<AnimationBase> Animations { get; }

    DbSet<ContentString> ContentStrings { get; }

    DbSet<CraftBase> Crafts { get; }

    DbSet<CraftingTableBase> CraftingTables { get; }

    DbSet<ClassBase> Classes { get; }

    DbSet<EventBase> Events { get; }

    DbSet<Folder> Folders { get; }

    DbSet<ItemBase> Items { get; }

    DbSet<LocaleContentString> LocaleContentStrings { get; }

    DbSet<MapController> Maps { get; }

    DbSet<NpcBase> Npcs { get; }

    DbSet<ProjectileBase> Projectiles { get; }

    DbSet<QuestBase> Quests { get; }

    DbSet<ResourceBase> Resources { get; }

    DbSet<ShopBase> Shops { get; }

    DbSet<SpellBase> Spells { get; }

    DbSet<PlayerVariableBase> PlayerVariables { get; }

    DbSet<ServerVariableBase> ServerVariables { get; }

    DbSet<GuildVariableBase> GuildVariables { get; }

    DbSet<TilesetBase> Tilesets { get; }

    DbSet<TimeBase> Time { get; }
}
