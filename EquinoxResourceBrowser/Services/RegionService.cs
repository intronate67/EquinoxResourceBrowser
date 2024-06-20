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
        try
        {
            return await _ctx.RegionResources.OrderBy(r => r.Name).Select(r => new RegionDto
            {
                Id = r.Id,
                Name = r.Name,
                TotalPower = r.TotalPower,
                TotalWorkforce = r.TotalWorkforce,
                SuperionicRate = r.TotalSuperionicIce,
                MagmaticRate = r.TotalMagmaticGas
            }).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred ({ErrorMessage}) while getting region resources", ex.Message);
        }

        return [];
    }
}
