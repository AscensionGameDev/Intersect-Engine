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
        base.CanBeAppliedTo(dbContext) &&
        SchemaMigrations.All(dbContext.AllSchemaMigrations.ToList().Contains) &&
        (SchemaMigrations.Any(dbContext.PendingSchemaMigrations.ToList().Contains) ||
         SchemaMigrationAttributes.Any(
             migration => migration.ApplyIfLast &&
                          string.Equals(
                              migration.SchemaMigrationName,
                              dbContext.AppliedSchemaMigrations.LastOrDefault(),
                              StringComparison.Ordinal
                          )
         ));
}
