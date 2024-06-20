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
            try
            {
                return await _ctx.ConstellationResources
                    .Where(c => c.RegionId == regionId)
                    .OrderBy(c => c.Name)
                    .Select(c => new ConstellationDto
                    {
                        Id = c.ConstellationId,
                        Name = c.Name,
                        TotalPower = c.TotalPower ?? 0,
                        TotalWorkforce = c.TotalWorkforce ?? 0,
                        SuperionicRate = c.TotalSuperionicIce ?? 0,
                        MagmaticRate = c.TotalMagmaticGas ?? 0
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred ({ErrorMessage}) while getting constellation resources for region: {RegionId}",
                    ex.Message, regionId);
            }

            return [];
        }
    }
}
