namespace Intersect.Server.Database;

public class DataMigrationMetadata : MigrationMetadata
{
    public override MigrationType MigrationType => MigrationType.Data;

    public Type? MigratorType { get; init; }

    public IReadOnlyCollection<string> SchemaMigrations =>
        SchemaMigrationAttributes
            .Select(attribute => attribute.SchemaMigrationName)
            .ToList()
            .AsReadOnly();

    internal IReadOnlyCollection<SchemaMigrationAttribute> SchemaMigrationAttributes { get; init; }

    public override bool CanBeAppliedTo<TContext>(TContext dbContext) =>
        base.CanBeAppliedTo(dbContext)
        && SchemaMigrations.Any(dbContext.PendingMigrations.ToList().Contains)
        && SchemaMigrations.All(dbContext.AllMigrations.ToList().Contains);
}
