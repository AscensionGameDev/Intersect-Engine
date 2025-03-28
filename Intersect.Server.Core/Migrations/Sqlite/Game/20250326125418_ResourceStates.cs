using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intersect.Server.Migrations.Sqlite.Game
{
    /// <inheritdoc />
    public partial class ResourceStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "States",
                table: "Resources",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql($@"
                UPDATE Resources
                SET States = json_object(
                    '00000000-0000-0000-0000-000000000001', json_object(
                        'Id', '00000000-0000-0000-0000-000000000001',
                        'Name', 'Exhausted',
                        'TextureName', Exhausted_Graphic,
                        'TextureType', Exhausted_GraphicFromTileset,
                        'RenderBelowEntities', Exhausted_RenderBelowEntities,
                        'Width', Exhausted_Width,
                        'Height', Exhausted_Height,
                        'X', Exhausted_X,
                        'Y', Exhausted_Y,
                        'MinimumHealth', 0,
                        'MaximumHealth', 0
                    ),
                    '00000000-0000-0000-0000-000000000002', json_object(
                        'Id', '00000000-0000-0000-0000-000000000002',
                        'Name', 'Initial',
                        'TextureName', Initial_Graphic,
                        'TextureType', Initial_GraphicFromTileset,
                        'RenderBelowEntities', Initial_RenderBelowEntities,
                        'Width', Initial_Width,
                        'Height', Initial_Height,
                        'X', Initial_X,
                        'Y', Initial_Y,
                        'MinimumHealth', 1,
                        'MaximumHealth', 100
                    )
                );
            ");

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
                name: "Initial_Graphic",
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

            migrationBuilder.RenameColumn(
                name: "Animation",
                table: "Resources",
                newName: "DeathAnimation");

            migrationBuilder.AddColumn<bool>(
                name: "UseExplicitMaxHealthForResourceStates",
                table: "Resources",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseExplicitMaxHealthForResourceStates",
                table: "Resources");

            migrationBuilder.RenameColumn(
                name: "States",
                table: "Resources",
                newName: "Initial_Graphic");

            migrationBuilder.RenameColumn(
                name: "DeathAnimation",
                table: "Resources",
                newName: "Animation");

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
