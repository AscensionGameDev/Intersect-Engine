// using Intersect.Server.Migrations.Game;
//
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata;
//
// using SqlKata.Execution;

namespace Intersect.Server.Database.GameData.Migrations;

// [SchemaMigration(typeof(Net6Upgrade))]
public sealed partial class Net6UpgradeDataMigration : IDataMigration<GameContext>
{
    public void Down(DatabaseContextOptions databaseContextOptions)
    {
        throw new NotImplementedException();
    }

    public void Up(DatabaseContextOptions databaseContextOptions)
    {
        //foreach (var entityType in context.Model.GetEntityTypes())
        //{
        //    var tableName = entityType.GetTableName();
        //    var possiblyCorruptGuidColumns =
        //        entityType
        //            .GetNavigations()
        //            .OfType<IPropertyBase>()
        //            .Concat(entityType.GetProperties())
        //            .Where(property => property.ClrType == typeof(Guid))
        //            .Select(property => property.Name)
        //            .ToArray();
        //    var dbConnection = context.Database.GetDbConnection();
        //    var queryCompiler = context.DatabaseType.CreateQueryCompiler();
        //    var queryFactory = new QueryFactory(dbConnection, queryCompiler);
        //    var results = queryFactory
        //        .Query(tableName)
        //        .Select(possiblyCorruptGuidColumns)
        //        .Get()
        //        .Cast<IDictionary<string, object>>()
        //        .ToList();

        //    results.ToList();
        //}
    }
}
