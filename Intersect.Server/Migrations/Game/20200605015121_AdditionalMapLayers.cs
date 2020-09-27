using Microsoft.EntityFrameworkCore.Migrations;

namespace Intersect.Server.Migrations.Game
{
    public partial class AdditionalMapLayers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MapLayers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MapLayerRegionID = table.Column<int>(nullable: false),
                    IntersectLayerID = table.Column<int>(nullable: false),
                    OldLayerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapLayers", x => x.ID);
                });

            migrationBuilder.Sql("INSERT INTO MapLayers (ID, MapLayerRegionID, IntersectLayerID, OldLayerID) VALUES (0, 0, 0, -1)");
            migrationBuilder.Sql("INSERT INTO MapLayers (ID, MapLayerRegionID, IntersectLayerID, OldLayerID) VALUES (1, 0, 1, -1)");
            migrationBuilder.Sql("INSERT INTO MapLayers (ID, MapLayerRegionID, IntersectLayerID, OldLayerID) VALUES (2, 0, 2, -1)");
            migrationBuilder.Sql("INSERT INTO MapLayers (ID, MapLayerRegionID, IntersectLayerID, OldLayerID) VALUES (3, 1, 3, -1)");
            migrationBuilder.Sql("INSERT INTO MapLayers (ID, MapLayerRegionID, IntersectLayerID, OldLayerID) VALUES (4, 2, 4, -1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapLayers");
        }
    }
}
