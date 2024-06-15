using EquinoxResourceBrowser.Esi.Models.Loyalty;
using EquinoxResourceBrowser.Esi.Models.Universe;
using Refit;

namespace EquinoxResourceBrowser.Interfaces
{
    public interface IEsiClient
    {
        [Get("/alliances/")]
        Task<int[]> GetAlliances();
        [Get("/alliances/{allianceId}/")]
        Task<Alliance> GetAlliance(int allianceId);
        [Get("/alliances/{allianceId}/corporations/")]
        Task<int[]> GetAllianceCorporations(int allianceId);
        [Get("/corporations/{corporationId}/")]
        Task<Corporation> GetCorporation(int corporationId);
        [Get("/universe/factions/")]
        Task<Faction[]> GetFactions();
        [Get("/universe/regions/")]
        Task<int[]> GetRegions();
        [Get("/universe/regions/{regionId}/")]
        Task<Region> GetRegion(int regionId);
        [Get("/universe/constellations/")]
        Task<int[]> GetConstellations();
        [Get("/universe/constellations/{constellationId}/")]
        Task<Constellation> GetConstellation(int constellationId);
        [Get("/universe/systems/")]
        Task<int[]> GetSolarSystems();
        [Get("/universe/systems/{systemId}/")]
        Task<SolarSystem> GetSolarSystem(int systemId);
        [Get("/universe/planets/{planetId}/")]
        Task<Planet> GetPlanet(int planetId);
        [Get("/universe/stars/{starId}/")]
        Task<Star> GetStar(int starId);
        [Get("/universe/stargates/{stargateId}/")]
        public Task<Stargate> GetStargate(int stargateId);
        [Get("/universe/types/{typeId}/")]
        Task<Esi.Models.Universe.Type> GetType(int typeId);
        [Get("/sovereignty/map/")]
        Task<SystemSovereignty[]> GetSovereigntyMap();
    }
}
