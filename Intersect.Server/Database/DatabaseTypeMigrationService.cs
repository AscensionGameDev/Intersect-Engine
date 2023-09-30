using System.Reflection;
using Intersect.Logging;
using Intersect.Reflection;
using Microsoft.EntityFrameworkCore;
using Intersect.Server.Localization;

namespace Intersect.Server.Database;

public class DatabaseTypeMigrationService
{
    private static readonly MethodInfo MethodInfoMigrateDbSet = typeof(DatabaseTypeMigrationService)
                                                                     .GetMethod(nameof(MigrateDbSet),
                                                                         BindingFlags.NonPublic | BindingFlags.Static)
                                                                 ?? throw new InvalidOperationException();

    private async Task<bool> CheckIfNotEmpty<TContext>(DatabaseContextOptions options)
        where TContext : IntersectDbContext<TContext>
    {
        await using var context = IntersectDbContext<TContext>.Create(options);
        if (context.IsEmpty())
        {
            try
            {
                _ = await context.Database.EnsureDeletedAsync();
                await context.Database.MigrateAsync();
                return false;
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                throw;
            }
        }

        Log.Error(Strings.Migration.MySqlNotEmpty);
        return true;
    }

    public async Task<bool> TryMigrate<TContext>(DatabaseContextOptions fromOptions, DatabaseContextOptions toOptions)
        where TContext : IntersectDbContext<TContext>
    {
        if (await CheckIfNotEmpty<TContext>(toOptions))
        {
            return false;
        }

        var dbSetInfos = typeof(TContext)
            .GetProperties()
            .Where(propertyInfo => propertyInfo.PropertyType.Extends(typeof(DbSet<>)));

        foreach (var dbSetInfo in dbSetInfos)
        {
            Log.Info(Strings.Migration.MigratingDbSet.ToString(dbSetInfo.Name));

            try
            {
                var dbSetContainedType = dbSetInfo.PropertyType.FindGenericTypeParameters(typeof(DbSet<>)).First();
                var migrateDbSetMethod = MethodInfoMigrateDbSet.MakeGenericMethod(typeof(TContext), dbSetContainedType);
                var migrateTask =
                    migrateDbSetMethod.Invoke(null, new object[] { fromOptions, toOptions, dbSetInfo }) as Task;
                await (migrateTask ?? throw new InvalidOperationException());
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                throw;
            }
        }

        return true;
    }


    private static async Task MigrateDbSet<TContext, T>(
        DatabaseContextOptions fromOptions,
        DatabaseContextOptions toOptions,
        PropertyInfo dbSetInfo
    )
        where TContext : IntersectDbContext<TContext>
        where T : class
    {
        await using var fromContext = IntersectDbContext<TContext>.Create(fromOptions with
        {
            DisableAutoInclude = true
        });
        await using var toContext = IntersectDbContext<TContext>.Create(toOptions with
        {
            DisableAutoInclude = true,
            EnableDetailedErrors = true,
            EnableSensitiveDataLogging = true
        });

        if (dbSetInfo.GetValue(fromContext) is not DbSet<T> fromDbSet)
        {
            throw new InvalidOperationException();
        }

        if (dbSetInfo.GetValue(toContext) is not DbSet<T> toDbSet)
        {
            throw new InvalidOperationException();
        }

        foreach (var item in fromDbSet)
        {
            fromContext.Entry(item).State = EntityState.Detached;
            toDbSet.Add(item);
        }

        // var skip = 0;
        // var remaining = await fromDbSet.CountAsync();
        // while (remaining > 0)
        // {
        //     var take = Math.Min(remaining, 1000);
        //
        //     try
        //     {
        //         await toDbSet.AddRangeAsync(fromDbSet.AsNoTracking().Skip(skip).Take(take));
        //     }
        //     catch (Exception exception)
        //     {
        //         Log.Error(exception);
        //         throw;
        //     }
        //
        //     remaining -= take;
        //     skip += take;
        // }

        try
        {
            toContext.ChangeTracker.DetectChanges();
            await toContext.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception);
            throw;
        }
    }
}
