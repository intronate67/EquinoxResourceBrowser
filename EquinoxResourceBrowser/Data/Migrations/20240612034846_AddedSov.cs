using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquinoxResourceBrowser.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSov : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Power",
                table: "Stars",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MagmaticRate",
                table: "Planets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Power",
                table: "Planets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuperionicRate",
                table: "Planets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Workforce",
                table: "Planets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Sovereignties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolarSystemId = table.Column<int>(type: "int", nullable: false),
                    AllianceId = table.Column<int>(type: "int", nullable: true),
                    CorporationId = table.Column<int>(type: "int", nullable: true),
                    FactionId = table.Column<int>(type: "int", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sovereignties", x => new { x.Id, x.SolarSystemId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sovereignties");

            migrationBuilder.DropColumn(
                name: "Power",
                table: "Stars");

            migrationBuilder.DropColumn(
                name: "MagmaticRate",
                table: "Planets");

            migrationBuilder.DropColumn(
                name: "Power",
                table: "Planets");

            migrationBuilder.DropColumn(
                name: "SuperionicRate",
                table: "Planets");

            migrationBuilder.DropColumn(
                name: "Workforce",
                table: "Planets");
        }
    }
}
