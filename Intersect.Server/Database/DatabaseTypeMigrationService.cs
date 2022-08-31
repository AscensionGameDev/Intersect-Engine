using System.Reflection;
using Intersect.Framework.Reflection;
using Microsoft.EntityFrameworkCore;
using Intersect.Server.Localization;

namespace Intersect.Server.Database;

public class DatabaseTypeMigrationService
{
    private static readonly MethodInfo _methodInfoMigrateDbSet = typeof(DatabaseTypeMigrationService)
        .GetMethod(nameof(MigrateDbSet), BindingFlags.NonPublic | BindingFlags.Static)
        ?? throw new InvalidOperationException();

    public async Task<bool> TryMigrate<TContext>(DatabaseContextOptions fromOptions, DatabaseContextOptions toOptions)
        where TContext : IntersectDbContext<TContext>
    {
        await using var fromContext = IntersectDbContext<TContext>.Create(fromOptions);
        await using var toContext = IntersectDbContext<TContext>.Create(toOptions);

        if (toContext.IsEmpty())
        {
            _ = await toContext.Database.EnsureDeletedAsync();
            await toContext.Database.MigrateAsync();
        }
        else
        {
            Console.WriteLine(Strings.Migration.mysqlnotempty);
            return false;
        }

        var dbSetInfos = typeof(TContext)
            .GetProperties()
            .Where(propertyInfo => propertyInfo.PropertyType.Extends(typeof(DbSet<>)));

        foreach (var dbSetInfo in dbSetInfos)
        {
            var fromDbSet = dbSetInfo.GetValue(fromContext);
            if (fromDbSet == default)
            {
                throw new InvalidOperationException();
            }

            var toDbSet = dbSetInfo.GetValue(toContext);
            if (toDbSet == default)
            {
                throw new InvalidOperationException();
            }

            var migrateDbSetMethod = _methodInfoMigrateDbSet.MakeGenericMethod(dbSetInfo.PropertyType);
            var migrateTask = migrateDbSetMethod.Invoke(null, new[] { fromDbSet, toDbSet }) as Task;
            await (migrateTask ?? throw new InvalidOperationException());
        }

        toContext.ChangeTracker.DetectChanges();
        await toContext.SaveChangesAsync();
        return true;
    }


    private static async Task MigrateDbSet<T>(DbSet<T> oldDbSet, DbSet<T> newDbSet) where T : class
    {
        var skip = 0;
        var remaining = await oldDbSet.CountAsync();
        while (remaining > 0)
        {
            var take = Math.Min(remaining, 1000);
            await newDbSet.AddRangeAsync(oldDbSet.Skip(skip).Take(take));
            remaining -= take;
            skip += take;
        }
    }
}
