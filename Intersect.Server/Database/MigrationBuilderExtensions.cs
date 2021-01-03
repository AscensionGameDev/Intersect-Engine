using System;
using System.Linq;

using Intersect.Config;

using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace Intersect.Server.Database
{

    public static class MigrationBuilderExtensions
    {

        public static DatabaseOptions.DatabaseType GetDatabaseType(this MigrationBuilder migrationBuilder)
        {
            switch (migrationBuilder.ActiveProvider)
            {
                default:
                    if (migrationBuilder.ActiveProvider?.ToLowerInvariant().Contains("sqlite") ?? false)
                    {
                        return DatabaseOptions.DatabaseType.SQLite;
                    }

                    return DatabaseOptions.DatabaseType.MySQL;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="migrationBuilder"></param>
        /// <param name="conditionalQueries"></param>
        /// <param name="suppressTransaction"></param>
        /// <returns></returns>
        /// <see cref="MigrationBuilder.Sql(string, bool)"/>
        public static OperationBuilder<SqlOperation> Sql(
            this MigrationBuilder migrationBuilder,
            params (DatabaseOptions.DatabaseType DatabaseType, string Sql)[] conditionalQueries
        )
        {
            return migrationBuilder.Sql(
                conditionalQueries
                    .Select(conditionalQuery => (conditionalQuery.DatabaseType, conditionalQuery.Sql, false))
                    .ToArray()
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="migrationBuilder"></param>
        /// <param name="conditionalQueries"></param>
        /// <returns></returns>
        /// <see cref="MigrationBuilder.Sql(string, bool)"/>
        public static OperationBuilder<SqlOperation> Sql(
            this MigrationBuilder migrationBuilder,
            params (DatabaseOptions.DatabaseType DatabaseType, string Sql, bool SuppressTransaction)[]
                conditionalQueries
        )
        {
            OperationBuilder<SqlOperation> operationBuilder = null;
            var databaseType = migrationBuilder.GetDatabaseType();
            foreach (var conditionalQuery in conditionalQueries)
            {
                if (databaseType == conditionalQuery.DatabaseType)
                {
                    operationBuilder = migrationBuilder.Sql(conditionalQuery.Sql, conditionalQuery.SuppressTransaction);
                }
            }

            if (operationBuilder == null)
            {
                throw new ArgumentException(@"No queries were executed.", nameof(conditionalQueries));
            }

            return operationBuilder;
        }

    }

}
