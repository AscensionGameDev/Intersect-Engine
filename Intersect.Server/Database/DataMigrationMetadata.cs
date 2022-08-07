using Intersect.Framework.Reflection;

namespace Intersect.Server.Database;

public class DataMigrationMetadata
{
    public Type ContextType { get; init; }

    public bool IsDataMigration { get; init; }

    public Type? MigrationType { get; init; }

    public string Name { get; init; }

    public IReadOnlyCollection<string> SchemaMigrations =>
        SchemaMigrationAttributes
            .Select(attribute => attribute.SchemaMigrationName)
            .ToList()
            .AsReadOnly();

    internal IReadOnlyCollection<SchemaMigrationAttribute> SchemaMigrationAttributes { get; init; }

    public bool CanBeAppliedTo<TContext>(IntersectDbContext<TContext> dbContext)
        where TContext : IntersectDbContext<TContext>
    {
        if (dbContext.GetType() != ContextType)
        {
            return false;
        }

        return SchemaMigrations.Any(dbContext.PendingMigrations.ToList().Contains)
            && SchemaMigrations.All(dbContext.AllMigrations.ToList().Contains);
    }

    public static DataMigrationMetadata Create(Type dataMigrationType)
    {
        var contextType = dataMigrationType.FindGenericTypeParameters(typeof(IDataMigration<>)).FirstOrDefault();
        if (contextType == default) {
            throw new ArgumentException($"Not a valid data migration type: {dataMigrationType.FullName}", nameof(dataMigrationType));
        }

        var schemaMigrationAttributes = dataMigrationType
            .GetCustomAttributes(typeof(SchemaMigrationAttribute), true)
            .OfType<SchemaMigrationAttribute>()
            .Where(attribute => attribute.DbContextType == contextType)
            .ToList()
            .AsReadOnly();

        return new DataMigrationMetadata
        {
            ContextType = contextType,
            IsDataMigration = true,
            MigrationType = dataMigrationType,
            Name = dataMigrationType.Name,
            SchemaMigrationAttributes = schemaMigrationAttributes,
        };
    }

    public static DataMigrationMetadata Create(string schemaMigrationName, Type contextType)
    {
        return new DataMigrationMetadata
        {
            ContextType = contextType,
            IsDataMigration = false,
            MigrationType = default,
            Name = schemaMigrationName,
            SchemaMigrationAttributes = new List<SchemaMigrationAttribute>().AsReadOnly(),
        };
    }
}
