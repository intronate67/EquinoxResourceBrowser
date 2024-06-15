using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Dtos;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services;

public class RegionService : IRegionService
{
    private readonly ILogger<RegionService> _logger;
    private readonly ResourceContext _ctx;

    public RegionService(ILogger<RegionService> logger, ResourceContext ctx)
    {
        _logger = logger;
        _ctx = ctx;
    }

    public async Task<List<RegionDto>> GetAllRegions()
    {
        return await (from r in _ctx.Regions
                    join c in _ctx.Constellations on r.RegionId equals c.RegionId
                    join s in _ctx.SolarSystems on c.ConstellationId equals s.ConstellationId
                    join p in _ctx.Planets on s.SolarSystemId equals p.SolarSystemId
                    join st in _ctx.Stars on s.SolarSystemId equals st.SolarSystemId
                    group new { p, st } by new { r.RegionId, r.Name } into grouping
                    where (((grouping.Sum(r => r.p.Power) ?? 0) + (grouping.Select(g => g.st).First().Power ?? 0)) != 0
                        && (grouping.Sum(r => r.p.Workforce) ?? 0) != 0
                        && (grouping.Sum(r => r.p.SuperionicRate) ?? 0) != 0
                        && (grouping.Sum(r => r.p.MagmaticRate) ?? 0) != 0)
                    select new RegionDto
                    {
                        Id = grouping.Key.RegionId,
                        Name = grouping.Key.Name,
                        TotalPower = (grouping.Sum(r => r.p.Power) + (grouping.Select(g => g.st).First().Power ?? 0)) ?? 0,
                        TotalWorkforce = grouping.Sum(r => r.p.Workforce) ?? 0,
                        SuperionicRate = grouping.Sum(r => r.p.SuperionicRate) ?? 0,
                        MagmaticRate = grouping.Sum(r => r.p.MagmaticRate) ?? 0
                    }).ToListAsync();
    }
}
