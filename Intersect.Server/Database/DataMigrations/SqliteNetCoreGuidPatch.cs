using System.Diagnostics;
using Dapper;
using Intersect.Config;
using Intersect.Logging;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SqlKata.Execution;

namespace Intersect.Server.Database.DataMigrations;

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

            var notNullLookup = columnsInDatabase.ToDictionary(
                column => column.Name,
                column => column.NotNull
            );

            var searchIdColumns = new[] { "Id" };
            if (!guidColumnNames.Contains("Id"))
            {
                searchIdColumns = guidColumnNames;
            }

            HashSet<string> skippedKeys = new();
            var numberOfRows = queryFactory.Query(entityTable).Select("*").AsCount().First<int>();
            var convertedRowCount = 0;
            const int take = 100;
            var numberOfSegments = numberOfRows / take + (numberOfRows % take == 0 ? 0 : 1);
            for (var skip = 0; skip < numberOfRows; skip += take)
            {
                var segmentNumber = 1 + skip / take;
                var startTimeSegment = DateTime.UtcNow;
                var rows = queryFactory
                    .Query(entityTable)
                    .Select(guidColumnNames)
                    .Skip(skip)
                    .Take(take)
                    .Get()
                    .Select(row => (IDictionary<string, object>)row)
                    .ToArray();
                var convertedRows = rows
                    .Select(
                        row =>
                        {
                            var convertedRow = row.Where(
                                    pair =>
                                    {
                                        var (key, _) = pair;
                                        if (!skippedKeys.Contains(key) && !(key.StartsWith('"') && key.EndsWith('"')))
                                        {
                                            return true;
                                        }

                                        skippedKeys.Add(key);
                                        skippedKeys.Add(key[1..^1]);
                                        return false;

                                    }
                                )
                                .ToDictionary(
                                    column => column.Key,
                                    column =>
                                    {
                                        switch (column.Value)
                                        {
                                            case string @string when Guid.TryParse(@string, out _):
                                                return string.Equals(
                                                    @string.ToUpperInvariant(),
                                                    @string,
                                                    StringComparison.Ordinal
                                                )
                                                    ? null
                                                    : @string.ToUpperInvariant();

                                            case null:
                                                return column.Value;
                                            case byte[] data:
                                                switch (data.Length)
                                                {
                                                    case 0:
                                                        return null;
                                                    case 16:
                                                    {
                                                        var guid = new Guid(data);
                                                        return Guid.Empty != guid ||
                                                               notNullLookup.TryGetValue(column.Key, out var notNull) &&
                                                               notNull
                                                            ? guid.ToString().ToUpperInvariant()
                                                            : null;
                                                    }
                                                    default:
                                                        return column.Value;
                                                }

                                            default:
                                                var columnKey = column.Key;
                                                throw new InvalidOperationException(
                                                    $"Expected byte[], received: '{column.Value}' for '{columnKey}'"
                                                );
                                        }
                                    }
                                );

                            var searchValues = searchIdColumns
                                .Select(searchIdColumn => (searchIdColumn, row[searchIdColumn]))
                                .ToArray();
                            return (convertedRow, searchValues);
                        }
                    )
                    .Where(
                        convertedRowEntry =>
                        {
                            var (convertedRow, _) = convertedRowEntry;
                            return convertedRow.Values.Any(value => value is not null);
                        })
                    .ToArray();

                if (convertedRows.Length > 0)
                {
                    dbConnection.Open();
                    var transaction = dbConnection.BeginTransaction();
                    var segmentConvertedRowCount = 0;
                    foreach (var (convertedRow, searchIds) in convertedRows)
                    {
                        try
                        {
                            var query = queryFactory.Query(entityTable);
                            foreach (var (columnName, searchId) in searchIds)
                            {
                                if (skippedKeys.Contains(columnName))
                                {
                                    Log.Warn($"Dropping '{columnName}' as a query constraint while updating '{entityTable}'");
                                    continue;
                                }

                                query = query.Where(columnName, searchId);
                            }

                            var result = query.Update(convertedRow, transaction: transaction);

                            if (Log.Default.Configuration.LogLevel >= LogLevel.Debug)
                            {
                                Log.Debug($"Processed row {convertedRowCount++}/{numberOfRows} in '{entityTable}' (segment {segmentConvertedRowCount++}/{convertedRows.Length}), {result} rows changed.");
                            }
                        }
                        catch
                        {
#if DEBUG
                            Debugger.Break();
#endif
                        }
                    }
                    transaction.Commit();
                    dbConnection.Close();

                    if (Log.Default.Configuration.LogLevel >= LogLevel.Debug)
                    {
                        Log.Debug(
                            $"Completed updating segment {segmentNumber}/{numberOfSegments} in {(DateTime.UtcNow - startTimeSegment).TotalMilliseconds}ms ('{entityTable}', {convertedRows.Length} rows updated)"
                        );
                    }
                }
                else
                {
                    Log.Debug($"Skipped segment {segmentNumber}/{numberOfSegments} because it has no changed rows ('{entityTable}')");
                }
            }

            Log.Verbose($"Completed updating table in {(DateTime.UtcNow - startTimeTable).TotalMilliseconds}ms  ('{entityTable}', {convertedRowCount} rows updated)");
        }

        Log.Verbose($"Completed updating database in {(DateTime.UtcNow - startTimeDatabase).TotalMilliseconds}ms");
    }
}
