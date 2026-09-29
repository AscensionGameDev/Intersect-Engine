using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class AddResourceIdleMotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "IdleMotionAmplitude",
                table: "Resources",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "IdleMotionPivotPercent",
                table: "Resources",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "IdleMotionSpeed",
                table: "Resources",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "IdleMotionType",
                table: "Resources",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdleMotionAmplitude",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "IdleMotionPivotPercent",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "IdleMotionSpeed",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "IdleMotionType",
                table: "Resources");
        }
    }
}
