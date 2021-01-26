using Intersect.Server.Database.Logging.Entities;

using Microsoft.EntityFrameworkCore;

using System;

namespace Intersect.Server.Database.Logging
{
    public interface ILoggingContext : IDbContext, IDisposable
    {
        DbSet<RequestLog> RequestLogs { get; }

        DbSet<UserActivityHistory> UserActivityHistory { get; }

        DbSet<ChatHistory> ChatHistory { get; }
    }
}
