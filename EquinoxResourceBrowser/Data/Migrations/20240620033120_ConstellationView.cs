using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquinoxResourceBrowser.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConstellationView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"CREATE VIEW [View_Constellations] AS 
WITH ConstellationPlanetResources AS (
	select
		c.ConstellationId,
		c.Name,
		c.RegionId,
		SUM(p.Power) as TotalPower,
		SUM(p.Workforce) as TotalWorkforce,
		SUM(p.SuperionicRate) as TotalSuperionicIce,
		SUM(p.MagmaticRate) as TotalMagmaticGas
	from Constellations c
	INNER JOIN SolarSystems s on c.ConstellationId = s.ConstellationId
	INNER JOIN Planets p on s.SolarSystemId = p.SolarSystemId
	group by c.ConstellationId, c.Name, c.RegionId
),

ConstellationStarResources AS (
	select
		c.ConstellationId,
		c.Name,
		c.RegionId,
		SUM(st.Power) as TotalPower
	from Constellations c
	INNER JOIN SolarSystems s on c.ConstellationId = s.ConstellationId
	INNER JOIN Stars st on s.SolarSystemId = st.SolarSystemId
	group by c.ConstellationId, c.Name, c.RegionId
)

select 
	cp.ConstellationId,
	cp.Name,
	cp.RegionId,
	cp.TotalPower + cs.TotalPower as TotalPower,
	cp.TotalWorkforce,
	cp.TotalSuperionicIce,
	cp.TotalMagmaticGas
FROM ConstellationPlanetResources cp
INNER JOIN ConstellationStarResources cs on cp.ConstellationId = cs.ConstellationId;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP VIEW [View_Constellations];");
        }
    }
}
