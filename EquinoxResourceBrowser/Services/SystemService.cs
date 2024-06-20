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
            try
            {
                return await _ctx.SystemResources
                    .Where(s => s.ConstellationId == constellationId)
                    .OrderBy(s => s.Name)
                    .Select(s => new SystemDto
                    {
                        Id = s.SolarSystemId,
                        Name = s.Name,
                        TotalPower = s.TotalPower ?? 0,
                        TotalWorkforce = s.TotalWorkforce ?? 0,
                        SuperionicRate = s.TotalSuperionicIce ?? 0,
                        MagmaticRate = s.TotalMagmaticGas ?? 0,
                        SovereignAllianceId = s.SovereignAllianceId,
                        SovereignAllianceName = s.SovereignAllianceName,
                        SovereignCorporationId = s.SovereignCorporationId,
                        SovereignCorporationName = s.SovereignCorporationName,
                        SovereignFactionId = s.SovereignFactionId,
                        SovereignFactionName = s.SovereignFactionName
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred ({ErrorMessage}) while getting system resources for constellation: {ConstellationId}",
                    ex.Message, constellationId);
            }

            return [];
        }

        public async Task<ResourceDto> GetResourcesForSystem(int systemId)
        {
            var resource = new ResourceDto
            {
                Star = await (from st in _ctx.Stars
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
                              }).FirstOrDefaultAsync(),

                Planets = await (from p in _ctx.Planets
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
                                 }).ToListAsync()
            };

            return resource;
        }
    }
}
