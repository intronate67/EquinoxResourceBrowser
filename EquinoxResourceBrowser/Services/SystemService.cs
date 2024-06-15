using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Dtos;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services
{
    public class SystemService : ISystemService
    {
        private readonly ILogger<SystemService> _logger;
        private readonly ResourceContext _ctx;

        public SystemService(ILogger<SystemService> logger, ResourceContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        public async Task<List<SystemDto>> GetSystemsForConstellation(int constellationId)
        {
            return await (from c in _ctx.Constellations
                          join s in _ctx.SolarSystems on c.ConstellationId equals s.ConstellationId
                          join sv in _ctx.Sovereignties on s.SolarSystemId equals sv.SolarSystemId
                          join p in _ctx.Planets on s.SolarSystemId equals p.SolarSystemId
                          join st in _ctx.Stars on s.SolarSystemId equals st.SolarSystemId
                          join a in _ctx.Alliances on sv.AllianceId equals a.AllianceId into g_all
                          from all in g_all.DefaultIfEmpty()
                          join co in _ctx.Corporations on sv.CorporationId equals co.CorporationId into g_corp
                          from corp in g_corp.DefaultIfEmpty()
                          join f in _ctx.Factions on sv.FactionId equals f.FactionId into g_fact
                          from fact in g_fact.DefaultIfEmpty()
                          where c.ConstellationId == constellationId
                          group new { s, p, sv } by new 
                          { 
                              s.SolarSystemId,
                              s.Name,
                              sv.AllianceId,
                              AllianceName = all.Name,
                              sv.CorporationId,
                              CorporationName = corp.Name,
                              sv.FactionId,
                              FactionName = fact.Name,
                              s.StarId,
                              st.Power
                          } into grouping
                          select new SystemDto
                          {
                              Id = grouping.Key.SolarSystemId,
                              Name = grouping.Key.Name,
                              SovereignAllianceId = grouping.Key.AllianceId,
                              SovereignAllianceName = grouping.Key.AllianceName,
                              SovereignCorporationId = grouping.Key.CorporationId,
                              SovereignCorporationName = grouping.Key.CorporationName,
                              SovereignFactionId = grouping.Key.FactionId,
                              SovereignFacionName = grouping.Key.FactionName,
                              TotalPower = (grouping.Sum(r => r.p.Power) + grouping.Key.Power) ?? 0,
                              TotalWorkforce = grouping.Sum(r => r.p.Workforce) ?? 0,
                              SuperionicRate = grouping.Sum(r => r.p.SuperionicRate) ?? 0,
                              MagmaticRate = grouping.Sum(r => r.p.MagmaticRate) ?? 0
                          }).ToListAsync();
        }

        public async Task<ResourceDto> GetResourcesForSystem(int systemId)
        {
            var resource = new ResourceDto();
            resource.Star = await (from st in _ctx.Stars
                        join t in _ctx.Types on st.TypeId equals t.TypeId
                        join s in _ctx.SolarSystems on st.SolarSystemId equals s.SolarSystemId
                        join sv in _ctx.Sovereignties on s.SolarSystemId equals sv.SolarSystemId
                        where s.SolarSystemId == systemId
                        select new StarDto
                        {
                            SolarSystemId = s.SolarSystemId,
                            StarId = st.StarId,
                            TypeId = st.TypeId,
                            Power = st.Power.GetValueOrDefault()
                        }).FirstOrDefaultAsync();

            resource.Planets = await (from p in _ctx.Planets
                           join t in _ctx.Types on p.TypeId equals t.TypeId
                           join s in _ctx.SolarSystems on p.SolarSystemId equals s.SolarSystemId
                           where s.SolarSystemId == systemId
                           select new PlanetDto
                           {
                               SolarSystemId = s.SolarSystemId,
                               PlanetId = p.PlanetId,
                               Name = p.Name,
                               TypeId = p.TypeId,
                               Power = p.Power.GetValueOrDefault(),
                               Workforce = p.Workforce,
                               SuperionicRate = p.SuperionicRate,
                               MagmaticRate = p.MagmaticRate
                           }).ToListAsync();

            return resource;
        }
    }
}
