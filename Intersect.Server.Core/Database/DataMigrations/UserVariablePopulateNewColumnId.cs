using System.Diagnostics;
using Intersect.Config;
using Intersect.Core;
using Intersect.Server.Database.PlayerData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using SqlKata.Extensions;

namespace Intersect.Server.Database.DataMigrations;

/// <see cref="T:Intersect.Server.Database.DataMigrations.Sqlite.SqliteUserVariablePopulateNewColumnId"/>
/// <see cref="T:Intersect.Server.Database.DataMigrations.MySql.MySqlUserVariablePopulateNewColumnId"/>
public abstract class UserVariablePopulateNewColumnId : IDataMigration<PlayerContext>
{
    public void Down(DatabaseContextOptions databaseContextOptions)
    {
    }

    public void Up(DatabaseContextOptions databaseContextOptions)
    {
        const string tableName = "User_Variables";

        using var context = PlayerContext.Create(databaseContextOptions);
        var databaseName = databaseContextOptions.DatabaseType == DatabaseType.SQLite
            ? default
            : databaseContextOptions.ConnectionStringBuilder["Database"].ToString();
        var queryCompiler = context.DatabaseType.CreateQueryCompiler();
        var dbConnection = context.Database.GetDbConnection();
        var queryFactory = new QueryFactory(dbConnection, queryCompiler);
        var numberOfTables = queryFactory.Query()
            .ForSqlite(
                q => q.SelectRaw(
                    "tbl_name FROM sqlite_master WHERE type = 'table' AND tbl_name LIKE 'User_Variables';"
                )
            )
            .ForMySql(q => q.SelectRaw($"INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = '{databaseName}' AND TABLE_NAME LIKE '{tableName}'"))
            .AsCount()
            .First<int>();

        if (numberOfTables < 1)
        {
            ApplicationContext.Context.Value?.Logger.LogWarning($"No 'User_Variables' table.");
            return;
        }

        var numberOfRows = queryFactory.Query(tableName).Where("Id", Guid.Empty.ToString()).AsCount().First<int>();
        const string columnUserId = "UserId";
        const string columnVariableId = "VariableId";
        var searchIdColumns = new[] { columnUserId, columnVariableId };

        const int take = 100;

        var startTimeTable = DateTime.UtcNow;
        var numberOfSegments = numberOfRows / take + (numberOfRows % take == 0 ? 0 : 1);
        var segmentNumber = 0;
        var convertedRowCount = 0;
        for (var skip = 0; skip < numberOfRows; skip += take)
        {
            ++segmentNumber;

            var startTimeSegment = DateTime.UtcNow;

            var rowsToConvert = queryFactory.Query(tableName)
                .Where("Id", Guid.Empty.ToString())
                .Select(searchIdColumns)
                .Skip(skip)
                .Take(take)
                .Get<PseudoUserVariable>()
                .ToArray();

            dbConnection.Open();
            var transaction = dbConnection.BeginTransaction();
            for (var indexInSegment = 0; indexInSegment < rowsToConvert.Length; ++indexInSegment)
            {
                var convertedRow = rowsToConvert[indexInSegment];
                try
                {
                    var query = queryFactory.Query(tableName)
                        .Where(columnUserId, convertedRow.UserId)
                        .Where(columnVariableId, convertedRow.VariableId);

                    var result = query.Update(convertedRow, transaction: transaction);

                    if (Debugger.IsAttached)
                    {
                        ApplicationContext.Context.Value?.Logger.LogDebug(
                            $"Processed row {++convertedRowCount}/{numberOfRows} in '{tableName}' (segment {indexInSegment++}/{rowsToConvert.Length}), {result} rows changed."
                        );
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

            if (Debugger.IsAttached)
            {
                ApplicationContext.Context.Value?.Logger.LogDebug(
                    $"Completed updating segment {segmentNumber}/{numberOfSegments} in {(DateTime.UtcNow - startTimeSegment).TotalMilliseconds}ms ('{tableName}', {rowsToConvert.Length} rows updated)"
                );
            }
        }

        ApplicationContext.Context.Value?.Logger.LogDebug($"Completed updating table in {(DateTime.UtcNow - startTimeTable).TotalMilliseconds}ms  ('{tableName}', {convertedRowCount} rows updated)");
    }

    private sealed class PseudoUserVariable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid VariableId { get; set; }
    }
}
