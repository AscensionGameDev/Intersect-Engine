using Dapper;
using Intersect.Config;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SqlKata.Execution;

namespace Intersect.Server.Database;

public sealed class SqliteNet6GuidPatch
{
    private class TableInfoColumn
    {
        public int Cid { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool NotNull { get; set; }
    }

    public static bool ShouldBeAppliedTo<TContext>(TContext context)
        where TContext : IntersectDbContext<TContext>
    {
        if (context.DatabaseType != DatabaseType.Sqlite)
        {
            return false;
        }

        var dbConnection = context.Database.GetDbConnection();
        var queryCompiler = context.DatabaseType.CreateQueryCompiler();
        var queryFactory = new QueryFactory(dbConnection, queryCompiler);
        var hasNet6MigrationsApplied = queryFactory
            .Query("__EFMigrationsHistory")
            .Select("MigrationId", "ProductVersion")
            .WhereStarts("ProductVersion", "6")
            .AsCount()
            .First<int>();
        return hasNet6MigrationsApplied < 1;
    }

    public static void ApplyTo<TContext>(TContext context)
        where TContext : IntersectDbContext<TContext>
    {
        var originalConnectionStringBuilder = context.ContextOptions.ConnectionStringBuilder;
        if (originalConnectionStringBuilder is not SqliteConnectionStringBuilder sqliteConnectionStringBuilder)
        {
            throw new InvalidOperationException();
        }

        sqliteConnectionStringBuilder.ForeignKeys = false;
        var recreatedContext = IntersectDbContext<TContext>.Create(
            context.ContextOptions with { ConnectionStringBuilder = sqliteConnectionStringBuilder }
        );

        var queryCompiler = recreatedContext.DatabaseType.CreateQueryCompiler();
        var dbConnection = recreatedContext.Database.GetDbConnection();
        var queryFactory = new QueryFactory(dbConnection, queryCompiler);
        var tablesInDatabase =
            queryFactory
                .Query("sqlite_master")
                .Select("tbl_name")
                .Where("type", "table")
                .Get<string>()
                .ToList();
        foreach (var entityType in context.Model.GetEntityTypes())
        {
            var entityTable = entityType.GetTableName();
            if (!tablesInDatabase.Contains(entityTable))
            {
                continue;
            }

            var columnsInDatabase = dbConnection
                .Query<TableInfoColumn>($"PRAGMA table_info({entityType.GetTableName()});")
                .ToList();
            var storeObject = StoreObjectIdentifier.Table(entityType.GetTableName()!, entityType.GetSchema());
            var guidColumnNames = columnsInDatabase
                .Where(
                    column => column.Type == "BLOB" && entityType.GetProperties().All(
                        property => property.GetColumnName(storeObject) != column.Name
                    ) || entityType.GetProperties().Any(
                        property => property.GetColumnName(storeObject) == column.Name &&
                                    property.ClrType == typeof(Guid)
                    )
                )
                .Select(column => column.Name)
                .ToArray();
            var notNullLookup = columnsInDatabase.ToDictionary(
                column => column.Name,
                column => column.NotNull
            );

            var numberOfRows = queryFactory.Query(entityTable).Select("*").AsCount().First<int>();
            const int take = 100;
            for (var skip = 0; skip < numberOfRows; skip += take)
            {
                var rows = queryFactory
                    .Query(entityTable)
                    .Select(guidColumnNames)
                    .Skip(skip)
                    .Take(take)
                    .Get()
                    .Select(row => (IDictionary<string, object>)row)
                    .ToList();
                var convertedRows = rows
                    .Select(
                        row =>
                        {
                            var convertedRow = row.ToDictionary(
                                column => column.Key,
                                column =>
                                {
                                    if (column.Value is string @string && Guid.TryParse(@string, out _))
                                    {
                                        return @string.ToUpperInvariant();
                                    }

                                    if (column.Value is not byte[] data)
                                    {
                                        throw new InvalidOperationException(
                                            $"Expected byte[], received: '{column.Value}'");
                                    }

                                    var value = data.Length switch
                                    {
                                        0 => null,
                                        16 => new Guid(data),
                                        36 => column.Value,
                                        _ => throw new InvalidOperationException(
                                            $"Expected blob of 0/16/36 bytes, received {data.Length}"
                                        )
                                    };

                                    if (value is not Guid guid)
                                    {
                                        return value;
                                    }

                                    return Guid.Empty != guid ||
                                           notNullLookup.TryGetValue(column.Key, out var notNull) && notNull
                                        ? guid.ToString().ToUpperInvariant()
                                        : null;
                                }
                            );
                            return (convertedRow, row["Id"]);
                        }
                    )
                    .ToList();
                foreach (var (convertedRow, searchId) in convertedRows)
                {
                    var result = queryFactory
                        .Query(entityTable)
                        .Where("Id", searchId)
                        .AsUpdate(convertedRow)
                        .Get();
                    result.ToString();
                }

                string.Empty.ToString();
            }

            string.Empty.ToString();
        }
    }
}
