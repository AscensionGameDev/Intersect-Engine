using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations;
using SqlKata;
using SqlKata.Compilers;

namespace Intersect.Server.Migrations.SqlGeneration;
public class StatRangeMigrationSql
{
    private readonly string[] StatColumns = new[] {
                "DescriptorId",
                "StatRange_Attack_LowRange",
                "StatRange_Attack_HighRange",
                "StatRange_AbilityPower_LowRange",
                "StatRange_AbilityPower_HighRange",
                "StatRange_Defense_LowRange",
                "StatRange_Defense_HighRange",
                "StatRange_MagicResist_LowRange",
                "StatRange_MagicResist_HighRange",
                "StatRange_Speed_LowRange",
                "StatRange_Speed_HighRange",
            };

    private readonly string[] ItemColumns = new[]
    {
                "Id",
                "StatGrowth",
                "StatGrowth",
                "StatGrowth",
                "StatGrowth",
                "StatGrowth",
                "StatGrowth",
                "StatGrowth",
                "StatGrowth",
                "StatGrowth",
                "StatGrowth",
            };

    private Compiler mMySqlCompiler { get; set; }
    private Compiler mSqliteCompiler { get; set; }

    public StatRangeMigrationSql()
    {
        mMySqlCompiler = new MySqlCompiler();
        mSqliteCompiler = new SqliteCompiler();
    }

    public string BuildSqliteSql()
    {
        return BuildSql(mSqliteCompiler);
    }

    public string BuildMysqlSql()
    {
        return BuildSql(mMySqlCompiler);
    }

    private string BuildSql(Compiler compiler)
    {
        var statGrowthItems = new Query("Items")
                .Select(ItemColumns)
                .Where("ItemType", "=", 1);

        var populateQuery = new Query("Items_EquipmentProperties")
            .AsInsert(StatColumns, statGrowthItems);

        return compiler.Compile(populateQuery).ToString();
    }
}
