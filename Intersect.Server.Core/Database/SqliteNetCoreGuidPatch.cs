using Dapper;
using Intersect.Config;
using Intersect.Logging;
using Intersect.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SqlKata.Execution;

namespace Intersect.Server.Database;

public sealed class SqliteNetCoreGuidPatch
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
        if (context.DatabaseType != DatabaseType.Sqlite || context.IsEmpty)
        {
            return false;
        }

        var dbConnection = context.Database.GetDbConnection();
        var queryCompiler = context.DatabaseType.CreateQueryCompiler();
        var queryFactory = new QueryFactory(dbConnection, queryCompiler);
        var hasNetCoreMigrationsApplied = queryFactory
            .Query("__EFMigrationsHistory")
            .Select("MigrationId")
            .Where("MigrationId", "20230930000000_Net7Upgrade")
            .AsCount()
            .First<int>();
        return hasNetCoreMigrationsApplied < 1;
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
        using var recreatedContext = IntersectDbContext<TContext>.Create(
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

        var startTimeDatabase = DateTime.UtcNow;
        foreach (var entityType in context.Model.GetEntityTypes())
        {
            var startTimeTable = DateTime.UtcNow;
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

            if (entityTable == "Players")
            {
                guidColumnNames = guidColumnNames.Append("DbGuildId").Distinct().ToArray();
            }

            var searchColumnNames = new[] { "Id" };
            if (entityTable == "User_Variables")
            {
                searchColumnNames = new[] { "UserId", "VariableId" };
            }

            if (searchColumnNames.Length < 1)
            {
                throw new InvalidOperationException("There should be at least 1 search column");
            }

            var notNullLookup = columnsInDatabase.ToDictionary(
                column => column.Name,
                column => column.NotNull
            );

            var numberOfRows = queryFactory.Query(entityTable).Select("*").AsCount().First<int>();
            var convertedRowCount = 0;
            const int take = 100;
            for (var skip = 0; skip < numberOfRows; skip += take)
            {
                var startTimeSegment = DateTime.UtcNow;
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
                                    switch (column.Value)
                                    {
                                        case string @string
                                            when Guid.TryParse(@string, out _): return @string.ToUpperInvariant();
                                        case null: return column.Value;
                                        case byte[] data:
                                            switch (data.Length)
                                            {
                                                case 0: return null;
                                                case 16:
                                                {
                                                    var guid = new Guid(data);
                                                    if (Guid.Empty != guid ||
                                                        notNullLookup.TryGetValue(column.Key, out var notNull) &&
                                                        notNull)
                                                    {
                                                        return guid.ToString().ToUpperInvariant();
                                                    }
                                                    else
                                                    {
                                                        return (string)null;
                                                    }
                                                }
                                                default: return column.Value;
                                            }

                                        default:
                                            throw new InvalidOperationException(
                                                $"Expected byte[], received: '{column.Value}'");
                                    }
                                }
                            );
                            return (convertedRow, searchColumnNames.Select(columnName => row[columnName]).ToArray());
                        }
                    )
                    .ToList();

                dbConnection.Open();
                var transaction = dbConnection.BeginTransaction();
                var segmentConvertedRowCount = 0;
                foreach (var (convertedRow, searchValues) in convertedRows)
                {
                    var query = queryFactory.Query(entityTable);
                    if (searchValues.Length != searchColumnNames.Length)
                    {
                        throw new InvalidOperationException(
                            $"Expected {searchColumnNames.Length} values but received {searchValues.Length}"
                        );
                    }

                    for (var searchColumnIndex = 0; searchColumnIndex < searchColumnNames.Length; ++searchColumnIndex)
                    {
                        query = query.Where(searchColumnNames[searchColumnIndex], searchValues[searchColumnIndex]);
                    }

                    var result = query.Update(convertedRow, transaction: transaction);

                    // Only debug logging below here in this loop
                    // If more logic needs to be added this should be inverted
                    // and the logging logic put back inside the if statement
                    if (Log.Default.Configuration.LogLevel < LogLevel.Debug)
                    {
                        continue;
                    }

                    var searchValuesString = string.Join(
                        ", ",
                        searchValues.Select(
                            searchValue => searchValue is byte[] searchIdBytes
                                ? new Guid(searchIdBytes).ToString()
                                : searchValue.ToString()
                        )
                    );

                    var searchValuesTypeString = string.Join(
                        ", ",
                        searchValues.Select(searchValue => searchValue?.GetFullishName())
                    );

                    Log.Debug($"Processed row {convertedRowCount++}/{numberOfRows} in '{entityTable}' (segment {segmentConvertedRowCount++}/{convertedRows.Count}) ({searchValuesString} was {searchValuesTypeString}), {result} rows changed.");
                }
                transaction.Commit();
                dbConnection.Close();

                if (Log.Default.Configuration.LogLevel >= LogLevel.Debug)
                {
                    Log.Debug(
                        $"Completed updating segment in {(DateTime.UtcNow - startTimeSegment).TotalMilliseconds}ms ('{entityTable}', {convertedRows.Count} rows updated)"
                    );
                }
            }

            Log.Verbose($"Completed updating table in {(DateTime.UtcNow - startTimeTable).TotalMilliseconds}ms  ('{entityTable}', {numberOfRows} rows updated)");
        }

        Log.Verbose($"Completed updating database in {(DateTime.UtcNow - startTimeDatabase).TotalMilliseconds}ms");
    }
}
