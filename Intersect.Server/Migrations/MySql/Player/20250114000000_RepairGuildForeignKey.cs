using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.MySql.Player
{
    /// <inheritdoc />
    public partial class MySqlGuildForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Players_Guilds_DbGuildId",
            //     table: "Players");

            migrationBuilder.Sql(
                """
                SET @__delete__if_exists_FK_Players_Guilds_DbGuildId=IF(
                	(
                		SELECT true
                		FROM information_schema.TABLE_CONSTRAINTS
                		WHERE CONSTRAINT_SCHEMA = DATABASE()
                			AND TABLE_NAME = 'Players'
                			AND CONSTRAINT_NAME = 'FK_Players_Guilds_DbGuildId'
                			AND CONSTRAINT_TYPE = 'FOREIGN KEY'
                	) = true,
                	'ALTER TABLE Players DROP FOREIGN KEY `FK_Players_Guilds_DbGuildId`',
                	'SELECT 1'
                );

                PREPARE __delete__if_exists_FK_Players_Guilds_DbGuildId_statement FROM @__delete__if_exists_FK_Players_Guilds_DbGuildId;
                EXECUTE __delete__if_exists_FK_Players_Guilds_DbGuildId_statement;
                DEALLOCATE PREPARE __delete__if_exists_FK_Players_Guilds_DbGuildId_statement;
                """
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Guilds_DbGuildId",
                table: "Players",
                column: "DbGuildId",
                principalTable: "Guilds",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Do nothing, ideally this would have existed before
        }
    }
}
