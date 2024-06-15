using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Dtos;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services
{
    public class ConstellationService : IConstellationService
    {
        private readonly ILogger<ConstellationService> _logger;
        private readonly ResourceContext _ctx;

        public ConstellationService(ILogger<ConstellationService> logger, ResourceContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        public async Task<List<ConstellationDto>> GetConstellationsForRegion(int regionId)
        {
            return await (from r in _ctx.Regions
                          join c in _ctx.Constellations on r.RegionId equals c.RegionId
                          join s in _ctx.SolarSystems on c.ConstellationId equals s.ConstellationId
                          join p in _ctx.Planets on s.SolarSystemId equals p.SolarSystemId
                          join st in _ctx.Stars on s.SolarSystemId equals st.SolarSystemId
                          where r.RegionId == regionId
                          group new { p, st } by new { c.ConstellationId, c.Name} into grouping
                          select new ConstellationDto
                          {
                              Id = grouping.Key.ConstellationId,
                              Name = grouping.Key.Name,
                              TotalPower = (grouping.Sum(r => r.p.Power) + grouping.Select(g => g.st).First().Power) ?? 0,
                              TotalWorkforce = grouping.Sum(r => r.p.Workforce) ?? 0,
                              SuperionicRate = grouping.Sum(r => r.p.SuperionicRate) ?? 0,
                              MagmaticRate = grouping.Sum(r => r.p.MagmaticRate) ?? 0
                          }).ToListAsync();
        }
    }
}
