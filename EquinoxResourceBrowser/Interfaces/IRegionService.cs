using EquinoxResourceBrowser.Dtos;

namespace EquinoxResourceBrowser.Interfaces;

public interface IRegionService
{
    Task<List<RegionDto>> GetAllRegions();
}
