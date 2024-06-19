using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Dtos;
using EquinoxResourceBrowser.Enums;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace EquinoxResourceBrowser.Services
{
    public class SearchService : ISearchService
    {
        private readonly ILogger<SearchService> _logger;
        private readonly IMemoryCache _memCache;
        private readonly ResourceContext _ctx;

        public SearchService(ILogger<SearchService> logger, IMemoryCache memCache, ResourceContext ctx)
        {
            _logger = logger;
            _memCache = memCache;
            _ctx = ctx;
        }

        public async Task<List<SearchResultDto>> LoadUniverse()
        {
            if (!_memCache.TryGetValue<List<SearchResultDto>>("search_results", out var result) || result?.Count == 0)
            {
                // TODO: Figure out a way to reduce the constellations and regions to only ones w/ workforce and power
                result = await (from s in _ctx.SolarSystems
                                join c in _ctx.Constellations on s.ConstellationId equals c.ConstellationId
                                where s.SecurityStatus <= 0.0
                                select new SearchResultDto
                                {
                                    Id = s.SolarSystemId,
                                    Name = s.Name,
                                    ConstellationId = s.ConstellationId,
                                    RegionId = c.RegionId,
                                    Type = SearchResultType.SolarSystem
                                }).Union(_ctx.Constellations.Select(s => new SearchResultDto
                                {
                                    Id = s.ConstellationId,
                                    Name = s.Name,
                                    ConstellationId = s.ConstellationId,
                                    RegionId = s.RegionId,
                                    Type = SearchResultType.Constellation
                                })).Union(_ctx.Regions.Select(s => new SearchResultDto
                                {
                                    Id = s.RegionId,
                                    Name = s.Name,
                                    ConstellationId = null,
                                    RegionId = s.RegionId,
                                    Type = SearchResultType.Region
                                }))
                                .OrderBy(s => s.Type)
                                .ThenBy(s => s.Name)
                                .ToListAsync();

                _memCache.Set("search_results", result);
            }

            return result ?? [];
        }

    }
}
