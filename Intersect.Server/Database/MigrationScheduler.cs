using Intersect.Logging;
using Intersect.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Database;

public sealed class MigrationScheduler<TContext>
    where TContext : IntersectDbContext<TContext>
{
    private readonly TContext _context;
    private List<MigrationMetadata>? _scheduledMigrations;

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

        if (SqliteNetCoreGuidPatch.ShouldBeAppliedTo(_context))
        {
            Log.Verbose("Applying .NET Core patch...");
            SqliteNetCoreGuidPatch.ApplyTo(_context);
            Log.Verbose("Finished applying .NET Core patch.");
        }

        var migrator = _context.Database.GetService<IMigrator>();

        var scheduleSegment = _scheduledMigrations;
        while (scheduleSegment.Any())
        {
            var targetMigration = scheduleSegment
                .TakeWhile(metadata => metadata is SchemaMigrationMetadata)
                .LastOrDefault();

            if (targetMigration is SchemaMigrationMetadata)
            {
                migrator.Migrate(targetMigration.Name);

                scheduleSegment = scheduleSegment
                    .SkipWhile(metadata => metadata is SchemaMigrationMetadata)
                    .ToList();
                continue;
            }

            targetMigration = scheduleSegment.FirstOrDefault();

            if (targetMigration is not DataMigrationMetadata scheduledDataMigration)
            {
                continue;
            }

            if (scheduledDataMigration.MigratorType == default)
            {
                throw new InvalidOperationException($"Missing MigratorType for {scheduledDataMigration.Name}");
            }

            if (Activator.CreateInstance(scheduledDataMigration.MigratorType) is not IDataMigration<TContext> dataMigration)
            {
                throw new InvalidOperationException($"Failed to create instance of data migration: {scheduledDataMigration.MigratorType.FullName}");
            }

            dataMigration.Up(_context.ContextOptions);

            scheduleSegment = scheduleSegment
                .Skip(1)
                .ToList();;
        }
    }

    public MigrationScheduler<TContext> SchedulePendingMigrations()
    {
        var dataMigrations = FindValidDataMigrations(_context);
        List<MigrationMetadata> scheduledMigrations = new();

        var pendingMigrations = _context.PendingMigrations.ToList();
        var pendingMigrationMetadata = pendingMigrations
            .Select(pendingMigration => MigrationMetadata.CreateSchemaMigrationMetadata(pendingMigration, typeof(TContext)))
            .ToList();

        foreach (var pendingMigration in pendingMigrationMetadata)
        {
            scheduledMigrations.Add(pendingMigration);
            scheduledMigrations.AddRange(
                dataMigrations
                    .OfType<DataMigrationMetadata>()
                    .Where(dataMigration =>
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

    public static List<MigrationMetadata> FindValidDataMigrations(TContext context) =>
        FindDataMigrationsForContextType()
            .Select(MigrationMetadata.CreateDataMigrationMetadata)
            .Cast<DataMigrationMetadata>()
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
            .Cast<MigrationMetadata>()
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
