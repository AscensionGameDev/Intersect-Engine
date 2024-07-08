using Intersect.Logging;
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

    public IReadOnlyCollection<string> AppliedSchemaMigrations => _context.AppliedSchemaMigrations;

    public IReadOnlyCollection<string> AvailableSchemaMigrations => _context.PendingSchemaMigrations;

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
        var pendingDataMigrations = _context.PendingDataMigrations;
        List<MigrationMetadata> scheduledMigrations = new();

        var pendingSchemaMigrationNames = _context.PendingSchemaMigrations.ToList();
        var pendingSchemaMigrations = pendingSchemaMigrationNames
            .Select(pendingMigration => MigrationMetadata.CreateSchemaMigrationMetadata(pendingMigration, typeof(TContext)))
            .ToList();

        // Add schema migrations in their order and interleave any data migrations
        foreach (var pendingMigration in pendingSchemaMigrations)
        {
            scheduledMigrations.Add(pendingMigration);
            scheduledMigrations.AddRange(
                pendingDataMigrations
                    .Where(dataMigration =>
                        string.Equals(
                            pendingMigration.Name,
                            dataMigration.SchemaMigrations
                                .OrderBy(name => pendingSchemaMigrationNames.IndexOf(name))
                                .Last(),
                            StringComparison.Ordinal
                        )
                    )
            );
        }

        // Add remaining data migrations
        scheduledMigrations.AddRange(
            pendingDataMigrations.Where(
                pdm => !scheduledMigrations.Contains(pdm) && pdm.SchemaMigrationAttributes.Any(sma => sma.ApplyIfLast)
            )
        );

        var unscheduledDataMigrations = pendingDataMigrations
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
}
