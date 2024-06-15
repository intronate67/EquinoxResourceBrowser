using EquinoxResourceBrowser.Dtos;

namespace EquinoxResourceBrowser.Interfaces
{
    public interface IConstellationService
    {
        Task<List<ConstellationDto>> GetConstellationsForRegion(int regionId);
    }
}
