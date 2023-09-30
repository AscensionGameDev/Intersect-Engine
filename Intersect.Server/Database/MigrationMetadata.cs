using Intersect.Reflection;

namespace Intersect.Server.Database;

public abstract class MigrationMetadata
{
    public Type ContextType { get; init; }

    public abstract MigrationType MigrationType { get; }

    public string Name { get; init; }

    public virtual bool CanBeAppliedTo<TContext>(TContext dbContext)
        where TContext : IntersectDbContext<TContext> =>
        ContextType == typeof(TContext);

    public override string ToString() => $"MigrationType={MigrationType}, Name={Name}, Context={ContextType.FullName}";

    public static MigrationMetadata CreateDataMigrationMetadata(Type dataMigrationType)
    {
        var contextType = dataMigrationType.FindGenericTypeParameters(typeof(IDataMigration<>)).FirstOrDefault();
        if (contextType == default)
        {
            throw new ArgumentException($"Not a valid data migration type: {dataMigrationType.FullName}", nameof(dataMigrationType));
        }

        var schemaMigrationAttributes = dataMigrationType
            .GetCustomAttributes(typeof(SchemaMigrationAttribute), true)
            .OfType<SchemaMigrationAttribute>()
            .Where(attribute => attribute.DbContextType.Extends(contextType))
            .ToList()
            .AsReadOnly();

        return new DataMigrationMetadata
        {
            ContextType = contextType,
            MigratorType = dataMigrationType,
            Name = dataMigrationType.Name,
            SchemaMigrationAttributes = schemaMigrationAttributes,
        };
    }

    public static MigrationMetadata CreateSchemaMigrationMetadata(string schemaMigrationName, Type contextType)
    {
        return new SchemaMigrationMetadata
        {
            ContextType = contextType,
            Name = schemaMigrationName,
        };
    }
}
