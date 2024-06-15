using EquinoxResourceBrowser.Dtos;

namespace EquinoxResourceBrowser.Interfaces
{
    public interface ISystemService
    {
        Task<List<SystemDto>> GetSystemsForConstellation(int constellationId);
        Task<ResourceDto> GetResourcesForSystem(int systemId);
    }
}
