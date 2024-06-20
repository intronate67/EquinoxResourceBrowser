using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquinoxResourceBrowser.Data.Migrations
{
    /// <inheritdoc />
    public partial class SystemView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"CREATE VIEW [View_Systems] AS
WITH PlanetResources AS (
	select
		s.SolarSystemId,
		s.Name,
		c.ConstellationId,
		c.RegionId,
		SUM(p.Power) as TotalPower,
		SUM(p.Workforce) as TotalWorkforce,
		SUM(p.SuperionicRate) as TotalSuperionicIce,
		SUM(p.MagmaticRate) as TotalMagmaticGas
	from SolarSystems s 
	INNER JOIN Constellations c on s.ConstellationId = c.ConstellationId
	INNER JOIN Planets p on s.SolarSystemId = p.SolarSystemId
	group by s.SolarSystemId, s.Name, c.ConstellationId, c.RegionId
),

StarPower AS (
	select
		s.SolarSystemId,
		st.Power as StarPower
	from SolarSystems s 
	INNER JOIN Constellations c on s.ConstellationId = c.ConstellationId
	INNER JOIN Stars st on s.SolarSystemId = st.SolarSystemId
)

select 
	pr.SolarSystemId,
	pr.Name,
	pr.ConstellationId,
	pr.RegionId,
	pr.TotalPower + sp.StarPower as TotalPower,
	pr.TotalWorkforce,
	pr.TotalSuperionicIce,
	pr.TotalMagmaticGas,
	sv.AllianceId as SovereignAllianceId,
	a.Name as SovereignAllianceName,
	sv.CorporationId as SovereignCorporationId,
	c.Name as SovereignCorporationName,
	sv.FactionId as SovereignFactionid,
	f.Name as SovereignFactionName
from PlanetResources pr
INNER JOIN StarPower sp on pr.SolarSystemId = sp.SolarSystemId
INNER JOIN Sovereignties sv on pr.SolarSystemId = sv.SolarSystemId
LEFT JOIN Alliances a on sv.AllianceId = a.AllianceId
LEFT JOIN Corporations c on sv.CorporationId = c.CorporationId
LEFT JOIN Factions f on sv.FactionId = f.FactionId;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP VIEW [View_Systems];");
        }
    }
}
