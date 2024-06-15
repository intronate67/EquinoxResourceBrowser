using EquinoxResourceBrowser.Dtos;
using EquinoxResourceBrowser.Dtos.Resources;
using EquinoxResourceBrowser.Enums;

namespace EquinoxResourceBrowser.Interfaces
{
    public interface IResourceService
    {
        Task<CalculatedResourceDto?> CalculateAvailableResources(int solarSystemId,
            RestrictionFilter filter = RestrictionFilter.AllConnected);
        Task<bool> SaveUpgrade(UpgradeResource upgrade, CancellationToken stoppingToken);
        Task<bool> SaveStars(IEnumerable<StarResource> star, CancellationToken stoppingToken);
        Task<bool> SavePlanets(IEnumerable<PlanetResource> planets, CancellationToken stoppingToken);
    }
}
