using Intersect.GameObjects;
using Intersect.GameObjects.Crafting;
using Intersect.GameObjects.Events;
using Intersect.GameObjects.Maps.MapList;
using Intersect.Server.Maps;

using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.GameData
{
    public interface IGameContext : IDbContext
    {
        DbSet<AnimationBase> Animations { get; set; }

        DbSet<CraftBase> Crafts { get; set; }

        DbSet<CraftingTableBase> CraftingTables { get; set; }

        DbSet<ClassBase> Classes { get; set; }

        DbSet<EventBase> Events { get; set; }

        DbSet<ItemBase> Items { get; set; }

        DbSet<MapInstance> Maps { get; set; }

        DbSet<MapList> MapFolders { get; set; }

        DbSet<NpcBase> Npcs { get; set; }

        DbSet<ProjectileBase> Projectiles { get; set; }

        DbSet<QuestBase> Quests { get; set; }

        DbSet<ResourceBase> Resources { get; set; }

        DbSet<ShopBase> Shops { get; set; }

        DbSet<SpellBase> Spells { get; set; }

        DbSet<ServerVariableBase> ServerVariables { get; set; }

        DbSet<TilesetBase> Tilesets { get; set; }

        DbSet<TimeBase> Time { get; set; }
    }
}
