using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquinoxResourceBrowser.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddComputedResources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ComputedResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolarSystemId = table.Column<int>(type: "int", nullable: false),
                    Filter = table.Column<int>(type: "int", nullable: false),
                    AllianceId = table.Column<int>(type: "int", nullable: true),
                    CorporationId = table.Column<int>(type: "int", nullable: true),
                    TotalPower = table.Column<int>(type: "int", nullable: false),
                    TotalWorkforce = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComputedResources", x => new { x.Id, x.SolarSystemId, x.Filter });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComputedResources");
        }
    }
}
