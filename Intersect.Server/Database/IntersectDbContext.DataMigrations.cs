using System.Reflection;
using Intersect.Reflection;

namespace Intersect.Server.Database;

public partial class IntersectDbContext<TDbContext>
{
    private static IReadOnlyCollection<Type> FindDataMigrationsForContextType()
    {
        var assembly = typeof(TDbContext).Assembly;
        var dataMigrationTypes = assembly.GetTypes()
            .Where(
                type => type.IsClass &&
                        !type.IsAbstract &&
                        !type.IsGenericType &&
                        type.FindGenericTypeParameters(typeof(IDataMigration<>), false).FirstOrDefault() ==
                        typeof(TDbContext)
            ).ToList().AsReadOnly();
        return dataMigrationTypes;
    }

    private static IReadOnlyCollection<DataMigrationMetadata> FindAllDataMigrations() =>
        FindDataMigrationsForContextType().Select(MigrationMetadata.CreateDataMigrationMetadata).ToList().AsReadOnly();

    private static IReadOnlyCollection<DataMigrationMetadata> FilterPendingMigrations(
        IEnumerable<DataMigrationMetadata> dataMigrations,
        TDbContext context
    )
    {
        return dataMigrations.Where(dataMigrationMetadata => dataMigrationMetadata.CanBeAppliedTo(context))
            .ToList()
            .AsReadOnly();
    }

    private IReadOnlyCollection<DataMigrationMetadata> FindPendingDataMigrations(IEnumerable<DataMigrationMetadata>? dataMigrations = default) =>
        FilterPendingMigrations(dataMigrations ?? FindAllDataMigrations(), this as TDbContext)
            .OrderBy(
                dataMigrationMetadata => dataMigrationMetadata.SchemaMigrations.Aggregate(
                    0,
                    (currentIndex, migrationName) => Math.Max(
                        currentIndex,
                        this.AllSchemaMigrations.ToList().IndexOf(migrationName)
                    )
                )
            )
            .ToList()
            .AsReadOnly();
}