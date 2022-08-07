using Intersect.Framework.Reflection;
using Intersect.Logging;

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Database;

public sealed class MigrationScheduler<TContext>
    where TContext : IntersectDbContext<TContext>
{
    private readonly TContext _context;
    private List<DataMigrationMetadata>? _scheduledMigrations;

    public MigrationScheduler(TContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IReadOnlyCollection<string> AvailableDataMigrations => throw new NotImplementedException();

    public IReadOnlyCollection<string> AppliedSchemaMigrations => _context.AppliedMigrations;

    public IReadOnlyCollection<string> AvailableSchemaMigrations => _context.PendingMigrations;

    public void ApplyScheduledMigrations()
    {
        if (_scheduledMigrations == default)
        {
            throw new InvalidOperationException($"Has {nameof(SchedulePendingMigrations)} been called?");
        }

        if (_scheduledMigrations.Count < 1)
        {
            return;
        }

        var migrator = _context.Database.GetService<IMigrator>();

        foreach (var scheduledMigration in _scheduledMigrations)
        {
            if (scheduledMigration.IsDataMigration)
            {
                var dataMigration = Activator.CreateInstance(scheduledMigration.MigrationType) as IDataMigration<TContext>;
                if (dataMigration == default)
                {
                    throw new InvalidOperationException($"Failed to create instance of data migration: {scheduledMigration.MigrationType.FullName}");
                }

                dataMigration.Up(_context);
                _context.ChangeTracker.DetectChanges();
                var changes = _context.SaveChanges();

                Log.Info($"{scheduledMigration.MigrationType.FullName}: {changes} changes applied.");
            }
            else
            {
                migrator.Migrate(scheduledMigration.Name);
            }
        }
    }

    public MigrationScheduler<TContext> SchedulePendingMigrations()
    {
        var dataMigrations = FindValidDataMigrations(_context);
        List<DataMigrationMetadata> scheduledMigrations = new();

        var pendingMigrations = _context.PendingMigrations.ToList();
        var pendingMigrationMetadata = pendingMigrations
            .Select(pendingMigration => DataMigrationMetadata.Create(pendingMigration, typeof(TContext)))
            .ToList();

        foreach (var pendingMigration in pendingMigrationMetadata)
        {
            scheduledMigrations.Add(pendingMigration);
            scheduledMigrations.AddRange(
                dataMigrations.Where(dataMigration =>
                    string.Equals(
                        pendingMigration.Name,
                        dataMigration.SchemaMigrations
                            .OrderBy(name => pendingMigrations.IndexOf(name))
                            .Last(),
                        StringComparison.Ordinal
                    )
                )
            );
        }

        var unscheduledDataMigrations = dataMigrations
            .Where(metadata => !scheduledMigrations.Contains(metadata))
            .ToList();

        if (unscheduledDataMigrations.Count > 0)
        {
            var exception = new InvalidOperationException("Found migrations that could not be scheduled.");
            exception.Data.Add("UnscheduledDataMigrations", unscheduledDataMigrations);
            throw exception;
        }

        _scheduledMigrations = scheduledMigrations;

        return this;
    }

    public static List<DataMigrationMetadata> FindValidDataMigrations(TContext context) =>
        FindDataMigrationsForContextType()
            .Select(DataMigrationMetadata.Create)
            .Where(index => index.CanBeAppliedTo(context))
            .OrderBy(
                index => index.SchemaMigrations.Aggregate(
                    0,
                    (currentIndex, migrationName) => Math.Max(
                        currentIndex,
                        context.AllMigrations.ToList().IndexOf(migrationName)
                    )
                )
            )
            .ToList();

    public static ICollection<Type> FindDataMigrationsForContextType()
    {
        var assembly = typeof(TContext).Assembly;
        var dataMigrationTypes = assembly.GetTypes()
            .Where(
                type => type.IsClass
                    && !type.IsAbstract
                    && !type.IsGenericType
                    && type.FindGenericTypeParameters(
                        typeof(IDataMigration<>),
                        false
                    ).FirstOrDefault() == typeof(TContext)
            );
        return dataMigrationTypes.ToList().AsReadOnly();
    }
}
