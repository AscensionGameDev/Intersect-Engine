using Intersect.Config;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Intersect.Server.Database;

public interface IDbContext
{
    DatabaseFacade Database { get; }

    DatabaseType DatabaseType { get; }

    int SaveChanges();

    int SaveChanges(bool acceptAllChangesOnSuccess);

    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
