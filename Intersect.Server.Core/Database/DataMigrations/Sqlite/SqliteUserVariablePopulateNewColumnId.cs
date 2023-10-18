namespace Intersect.Server.Database.DataMigrations.Sqlite;

[SchemaMigration(typeof(Migrations.Sqlite.Player.UserVariables_AddStandaloneIdPK), ApplyIfLast = true)]
public sealed class SqliteUserVariablePopulateNewColumnId : UserVariablePopulateNewColumnId
{

}