using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquinoxResourceBrowser.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddResourceCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConstellationId",
                table: "ComputedResources",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "ComputedResources",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConstellationId",
                table: "ComputedResources");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "ComputedResources");
        }
    }
}
