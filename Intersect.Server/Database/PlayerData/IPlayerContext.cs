using Intersect.Server.Database.PlayerData.Api;
using Intersect.Server.Database.PlayerData.Players;
using Intersect.Server.Entities;

using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.PlayerData
{
    public interface IPlayerContext : IDbContext
    {
        DbSet<User> Users { get; }

        DbSet<Mute> Mutes { get; }

        DbSet<Ban> Bans { get; }

        DbSet<RefreshToken> RefreshTokens { get; }

        DbSet<Player> Players { get; }

        DbSet<BankSlot> Player_Bank { get; }

        DbSet<Friend> Player_Friends { get; }

        DbSet<HotbarSlot> Player_Hotbar { get; }

        DbSet<InventorySlot> Player_Items { get; }

        DbSet<Quest> Player_Quests { get; }

        DbSet<SpellSlot> Player_Spells { get; }

        DbSet<PlayerVariable> Player_Variables { get; }

        DbSet<Bag> Bags { get; }

        DbSet<BagSlot> Bag_Items { get; }

        DbSet<Guild> Guilds { get; }

        DbSet<GuildBankSlot> Guild_Bank { get; }

        DbSet<GuildVariable> Guild_Variables { get; }
    }
}
