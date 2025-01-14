namespace Intersect.Server.Database;

public sealed class CleanDatabaseMigrationMetadata : MigrationMetadata
{
    public override MigrationType MigrationType => MigrationType.Clean;
}