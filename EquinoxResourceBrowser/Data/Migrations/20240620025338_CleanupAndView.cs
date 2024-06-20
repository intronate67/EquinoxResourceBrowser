using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquinoxResourceBrowser.Data.Migrations
{
    /// <inheritdoc />
    public partial class CleanupAndView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComputedResources");

            migrationBuilder.Sql(@"CREATE VIEW [View_Regions] AS 
WITH SystemPlanetResources AS (
    SELECT 
        r.RegionId,
		r.Name,
        SUM(p.Power) AS TotalPower,
		SUM(p.Workforce) as TotalWorkforce,
		SUM(p.SuperionicRate) as TotalSuperionicIce,
		SUM(p.MagmaticRate) as TotalMagmaticGas
    FROM 
        SolarSystems AS s
	INNER JOIN Constellations c on s.ConstellationId = c.ConstellationId
	INNER JOIN Regions r on c.RegionId = r.RegionId
    LEFT JOIN 
        Planets AS p ON s.SolarSystemId = p.SolarSystemId
    GROUP BY 
        r.RegionId, r.Name
),
SystemStarPower AS (
    SELECT 
        r.RegionId,
		r.Name,
        SUM(st.Power) AS TotalPower
    FROM 
        SolarSystems AS s
	INNER JOIN Constellations c on s.ConstellationId = c.ConstellationId
	INNER JOIN Regions r on c.RegionId = r.RegionId
    LEFT JOIN 
        Stars AS st ON s.SolarSystemId = st.SolarSystemId
	GROUP BY 
        r.RegionId, r.Name
)
SELECT 
    ra.RegionId AS Id,
    ra.Name,
    ra.TotalPower + sp.TotalPower as TotalPower,
    ra.TotalWorkforce,
    ra.TotalSuperionicIce,
    ra.TotalMagmaticGas
FROM 
    SystemPlanetResources AS ra
JOIN SystemStarPower sp on ra.RegionId = sp.RegionId
WHERE 
    ra.TotalPower <> 0
    OR ra.TotalWorkforce <> 0
    OR ra.TotalSuperionicIce <> 0
    OR ra.TotalMagmaticGas <> 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [View_Regions];");
            migrationBuilder.CreateTable(
                name: "ComputedResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SolarSystemId = table.Column<int>(type: "int", nullable: false),
                    Filter = table.Column<int>(type: "int", nullable: false),
                    AllianceId = table.Column<int>(type: "int", nullable: true),
                    ConstellationId = table.Column<int>(type: "int", nullable: true),
                    CorporationId = table.Column<int>(type: "int", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegionId = table.Column<int>(type: "int", nullable: true),
                    TotalPower = table.Column<int>(type: "int", nullable: false),
                    TotalWorkforce = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComputedResources", x => new { x.Id, x.SolarSystemId, x.Filter });
                });
        }
    }
}
