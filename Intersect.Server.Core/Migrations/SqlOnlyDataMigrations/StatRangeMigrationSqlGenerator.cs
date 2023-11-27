using Intersect.Config;
using Intersect.Server.Database;
using SqlKata;

namespace Intersect.Server.Migrations.SqlOnlyDataMigrations;

public sealed class StatRangeMigrationSqlGenerator : IDataMigrationSqlGenerator
{
    private static readonly string[] EquipmentPropertiesColumns = {
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

    private static readonly string[] ItemDescriptorColumns = {
        "Id",
        "(StatGrowth * -1)",
        "StatGrowth",
        "(StatGrowth * -1)",
        "StatGrowth",
        "(StatGrowth * -1)",
        "StatGrowth",
        "(StatGrowth * -1)",
        "StatGrowth",
        "(StatGrowth * -1)",
        "StatGrowth",
    };

    public IEnumerable<string> Generate(DatabaseType databaseType)
    {
        var compiler = (this as IDataMigrationSqlGenerator).PickCompiler(databaseType);

        var selectStatGrowthItems = new Query("Items").SelectRaw(string.Join(", ", ItemDescriptorColumns))
            .Where("ItemType", "=", 1)
            .Where("StatGrowth", "!=", 0);

        var insertIntoItemsEquipmentProperties =
            new Query("Items_EquipmentProperties").AsInsert(EquipmentPropertiesColumns, selectStatGrowthItems);

        var sqlInsert = compiler.Compile(insertIntoItemsEquipmentProperties).ToString();

        return new[] { sqlInsert };
    }
}
