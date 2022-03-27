using Intersect.Server.Database.PlayerData.Api;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities;

using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.PlayerData
{
    public interface IPlayerContext : IDbContext
    {
        DbSet<User> Users { get; set; }

        DbSet<Mute> Mutes { get; set; }

        DbSet<Ban> Bans { get; set; }

        DbSet<RefreshToken> RefreshTokens { get; set; }

        DbSet<Player> Players { get; set; }

        DbSet<BankSlot> Player_Bank { get; set; }

        DbSet<Friend> Player_Friends { get; set; }

        DbSet<HotbarSlot> Player_Hotbar { get; set; }

        DbSet<InventorySlot> Player_Items { get; set; }

        DbSet<Quest> Player_Quests { get; set; }

        DbSet<SpellSlot> Player_Spells { get; set; }

        DbSet<PlayerVariable> Player_Variables { get; set; }

        DbSet<Bag> Bags { get; set; }

        DbSet<BagSlot> Bag_Items { get; set; }

        DbSet<Guild> Guilds { get; set; }

        DbSet<GuildBankSlot> Guild_Bank { get; set; }
    }
}
