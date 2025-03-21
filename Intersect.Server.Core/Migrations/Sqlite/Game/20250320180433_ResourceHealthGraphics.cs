using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class ResourceHealthGraphics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HealthGraphics",
                table: "Resources",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Resources
                SET HealthGraphics = json_object(
                    'Exhauted', json_object(
                        'Graphic', Exhausted_Graphic,
                        'GraphicType', Exhausted_GraphicFromTileset,
                        'RenderBelowEntities', Exhausted_RenderBelowEntities,
                        'Height', Exhausted_Height,
                        'Width', Exhausted_Width,
                        'X', Exhausted_X,
                        'Y', Exhausted_Y,
                        'MinHp', 0,
                        'MaxHp', 0
                    ),
                    'Initial', json_object(
                        'Graphic', Initial_Graphic,
                        'GraphicType', Initial_GraphicFromTileset,
                        'RenderBelowEntities', Initial_RenderBelowEntities,
                        'Height', Initial_Height,
                        'Width', Initial_Width,
                        'X', Initial_X,
                        'Y', Initial_Y,
                        'MinHp', 100,
                        'MaxHp', 100
                    )
                );
            ");

            migrationBuilder.AddColumn<bool>(
                name: "UseExplicitMaxHealthForResourceStates",
                table: "Resources",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.DropColumn(
                name: "Exhausted_Graphic",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Exhausted_GraphicFromTileset",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Exhausted_Height",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Exhausted_RenderBelowEntities",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Exhausted_Width",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Exhausted_X",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Exhausted_Y",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Initial_GraphicFromTileset",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Initial_Height",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Initial_RenderBelowEntities",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Initial_Width",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Initial_X",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Initial_Y",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "Initial_Graphic",
                table: "Resources");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseExplicitMaxHealthForResourceStates",
                table: "Resources");

            migrationBuilder.RenameColumn(
                name: "HealthGraphics",
                table: "Resources",
                newName: "Initial_Graphic");

            migrationBuilder.AddColumn<string>(
                name: "Exhausted_Graphic",
                table: "Resources",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Exhausted_GraphicFromTileset",
                table: "Resources",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Exhausted_Height",
                table: "Resources",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Exhausted_RenderBelowEntities",
                table: "Resources",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Exhausted_Width",
                table: "Resources",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Exhausted_X",
                table: "Resources",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Exhausted_Y",
                table: "Resources",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Initial_GraphicFromTileset",
                table: "Resources",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Initial_Height",
                table: "Resources",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Initial_RenderBelowEntities",
                table: "Resources",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Initial_Width",
                table: "Resources",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Initial_X",
                table: "Resources",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Initial_Y",
                table: "Resources",
                type: "INTEGER",
                nullable: true);
        }
    }
}
