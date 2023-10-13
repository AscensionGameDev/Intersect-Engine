namespace Intersect.Server.Database;

public class SchemaMigrationMetadata : MigrationMetadata
{
    public override MigrationType MigrationType => MigrationType.Schema;
}
