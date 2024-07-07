using Intersect.Server.Database.Logging.Entities;

using Microsoft.EntityFrameworkCore;

namespace Intersect.Server.Database.Logging;

public interface ILoggingContext : IDbContext, IDisposable
{
    DbSet<RequestLog> RequestLogs { get; }

    DbSet<UserActivityHistory> UserActivityHistory { get; }

    DbSet<ChatHistory> ChatHistory { get; }

    DbSet<TradeHistory> TradeHistory { get; }
    
    DbSet<GuildHistory> GuildHistory { get; }

    void Seed();
}
