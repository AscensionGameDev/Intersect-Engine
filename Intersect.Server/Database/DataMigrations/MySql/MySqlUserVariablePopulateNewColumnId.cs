namespace Intersect.Server.Database.DataMigrations.MySql;

[SchemaMigration(typeof(Migrations.MySql.Player.UserVariables_AddStandaloneIdPK), ApplyIfLast = true)]
public sealed class MySqlUserVariablePopulateNewColumnId : UserVariablePopulateNewColumnId
{
}