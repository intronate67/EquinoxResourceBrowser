using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Data.Models;
using EquinoxResourceBrowser.Dtos;
using EquinoxResourceBrowser.Dtos.Resources;
using EquinoxResourceBrowser.Enums;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services
{
    public class ResourceService : IResourceService
    {
        private readonly ILogger<ResourceService> _logger;
        private readonly ResourceContext _ctx;

        public ResourceService(ResourceContext ctx, ILogger<ResourceService> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public async Task<CalculatedResourceDto?> CalculateAvailableResources(int solarSystemId, RestrictionFilter filter = RestrictionFilter.AllConnected)
        {
            // Calculate connected solar systems based on the filter
            if (filter == RestrictionFilter.SystemOnly)
            {
                // Do not calculate connected systems as we only care about the single system
                return await (from s in _ctx.SolarSystems
                              join st in _ctx.Stars on s.SolarSystemId equals st.SolarSystemId
                              join p in _ctx.Planets on s.SolarSystemId equals p.SolarSystemId
                              where s.SolarSystemId == solarSystemId
                              group new { p } by new { s.SolarSystemId, s.StarId, st.Power } into grouping
                              select new CalculatedResourceDto
                              {
                                  SolarSystemId = grouping.Key.SolarSystemId,
                                  TotalPower = (grouping.Sum(g => g.p.Power) + grouping.Key.Power) ?? 0,
                                  TotalWorkforce = grouping.Sum(g => g.p.Workforce) ?? 0,
                              }).FirstOrDefaultAsync() ?? new CalculatedResourceDto
                              {
                                  SolarSystemId = solarSystemId,
                              };
            }

            if (filter == RestrictionFilter.ConstellationOnly || filter == RestrictionFilter.RegionOnly || filter == RestrictionFilter.AllConnected)
            {
                var result = await (from s in _ctx.SolarSystems
                                    join c in _ctx.Constellations on s.ConstellationId equals c.ConstellationId
                                    join st in _ctx.Stars on s.SolarSystemId equals st.SolarSystemId
                                    join p in _ctx.Planets on s.SolarSystemId equals p.SolarSystemId
                                    join sv in _ctx.Sovereignties on s.SolarSystemId equals sv.SolarSystemId
                                    where s.SolarSystemId == solarSystemId
                                    group new { p, sv } by new
                                    {
                                        s.SolarSystemId,
                                        s.ConstellationId,
                                        c.RegionId,
                                        s.StarId,
                                        st.Power
                                    } into grouping
                                    select new CalculatedResourceDto
                                    {
                                        SolarSystemId = grouping.Key.SolarSystemId,
                                        ConstellationId = grouping.Key.ConstellationId,
                                        RegionId = grouping.Key.RegionId,
                                        TotalPower = grouping.Sum(g => g.p.Power) + grouping.Key.Power ?? 0,
                                        TotalWorkforce = grouping.Sum(g => g.p.Workforce) ?? 0,
                                        AllianceId = grouping.Select(g => g.sv).First().AllianceId,
                                        CorporationId = grouping.Select(g => g.sv).First().CorporationId
                                    }).FirstOrDefaultAsync();

                if (result == null) return null;

                HashSet<int> visited = [solarSystemId];

                await TraverseConnectedSystems(solarSystemId, visited, result, filter, result.AllianceId, result.CorporationId);

                return result;
            }

            return null;
        }

        private async Task TraverseConnectedSystems(int currentSystemId, HashSet<int> visited,
            CalculatedResourceDto result, RestrictionFilter filter, int? sourceAllianceId, int? sourceCorpId)
        {
            visited.Add(currentSystemId);

            IQueryable<ConnectedSystem> connectedSystemsQuery;

            if (filter == RestrictionFilter.ConstellationOnly)
            {
                connectedSystemsQuery = from sg in _ctx.Stargates
                                        join dsg in _ctx.Stargates on sg.DestinationStargateId equals dsg.StargateId
                                        join s in _ctx.SolarSystems on dsg.SolarSystemId equals s.SolarSystemId
                                        join st in _ctx.Stars on s.SolarSystemId equals st.SolarSystemId
                                        join p in _ctx.Planets on s.SolarSystemId equals p.SolarSystemId
                                        join sv in _ctx.Sovereignties on s.SolarSystemId equals sv.SolarSystemId
                                        where sg.SolarSystemId == currentSystemId
                                              && s.ConstellationId == result.ConstellationId
                                              && (sv.AllianceId == sourceAllianceId || sv.CorporationId == sourceCorpId)
                                        select new ConnectedSystem
                                        {
                                            SolarSystem = s,
                                            Star = st,
                                            Planet = p,
                                            Sovereignty = sv
                                        };
            }
            else if (filter == RestrictionFilter.RegionOnly)
            {
                connectedSystemsQuery = from sg in _ctx.Stargates
                                        join dsg in _ctx.Stargates on sg.DestinationStargateId equals dsg.StargateId
                                        join s in _ctx.SolarSystems on dsg.SolarSystemId equals s.SolarSystemId
                                        join c in _ctx.Constellations on s.ConstellationId equals c.ConstellationId
                                        join st in _ctx.Stars on s.SolarSystemId equals st.SolarSystemId
                                        join p in _ctx.Planets on s.SolarSystemId equals p.SolarSystemId
                                        join sv in _ctx.Sovereignties on s.SolarSystemId equals sv.SolarSystemId
                                        where sg.SolarSystemId == currentSystemId
                                              && c.RegionId == result.RegionId
                                              && (sv.AllianceId == sourceAllianceId || sv.CorporationId == sourceCorpId)
                                        select new ConnectedSystem
                                        {
                                            SolarSystem = s,
                                            Star = st,
                                            Planet = p,
                                            Sovereignty = sv
                                        };
            }
            else // filter == RestrictionFilter.AllConnected
            {
                connectedSystemsQuery = from sg in _ctx.Stargates
                                        join dsg in _ctx.Stargates on sg.DestinationStargateId equals dsg.StargateId
                                        join s in _ctx.SolarSystems on dsg.SolarSystemId equals s.SolarSystemId
                                        join st in _ctx.Stars on s.SolarSystemId equals st.SolarSystemId
                                        join p in _ctx.Planets on s.SolarSystemId equals p.SolarSystemId
                                        join sv in _ctx.Sovereignties on s.SolarSystemId equals sv.SolarSystemId
                                        where sg.SolarSystemId == currentSystemId
                                              && (sv.AllianceId == sourceAllianceId || sv.CorporationId == sourceCorpId)
                                        select new ConnectedSystem
                                        {
                                            SolarSystem = s,
                                            Star = st,
                                            Planet = p,
                                            Sovereignty = sv
                                        };
            }

            var connectedSystems = await connectedSystemsQuery.ToListAsync();

            foreach (var connectedSystem in connectedSystems)
            {
                if (visited.Contains(connectedSystem.SolarSystem.SolarSystemId)) continue;

                // Do not add power since it cannot be transfered between solar systems
                //result.TotalPower += connectedSystem.Star.Power + connectedSystem.Planet.Power ?? 0;
                result.TotalWorkforce += connectedSystem.Planet.Workforce ?? 0;

                await TraverseConnectedSystems(connectedSystem.SolarSystem.SolarSystemId, visited,
                    result, filter, connectedSystem.Sovereignty.AllianceId, connectedSystem.Sovereignty.CorporationId);
            }
        }

        public async Task<bool> SavePlanets(IEnumerable<PlanetResource> planets, CancellationToken stoppingToken)
        {
            var success = false;
            await using var transaction = await _ctx.Database.BeginTransactionAsync(stoppingToken);
            try
            {
                var existingPlanets = await _ctx.Planets.ToArrayAsync(stoppingToken);
                if (existingPlanets is not null && existingPlanets.Length > 0)
                {
                    foreach (var planet in from ex in existingPlanets
                                           join res in planets on ex.PlanetId equals res.PlanetId
                                           select new { ex, res })
                    {
                        planet.ex.Power = planet.res.Power;
                        planet.ex.Workforce = planet.res.Workforce;
                        planet.ex.SuperionicRate = planet.res.SuperionicRate;
                        planet.ex.MagmaticRate = planet.res.MagmaticRate;
                        planet.ex.ModifiedTime = DateTime.UtcNow;
                    }
                }
                await _ctx.SaveChangesAsync(stoppingToken);
                await transaction.CommitAsync(stoppingToken);

                success = true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(stoppingToken);
                _logger.LogError(ex, "An exception occurred ({ErrorMessage}) while saving star.", ex.Message);
            }

            return success;
        }

        public async Task<bool> SaveStars(IEnumerable<StarResource> stars, CancellationToken stoppingToken)
        {
            var success = false;
            await using var transaction = await _ctx.Database.BeginTransactionAsync(stoppingToken);
            try
            {
                var existingStars = await _ctx.Stars.ToArrayAsync(stoppingToken);
                if(existingStars is not null && existingStars.Length > 0)
                {
                    foreach(var star in from ex in existingStars
                                        join res in stars on ex.StarId equals res.StarId
                                        select new { ex, res })
                    {
                        star.ex.Power = star.res.Power;
                        star.ex.ModifiedTime = DateTime.UtcNow;
                    }
                }
                await _ctx.SaveChangesAsync(stoppingToken);
                await transaction.CommitAsync(stoppingToken);

                success = true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(stoppingToken);
                _logger.LogError(ex, "An exception occurred ({ErrorMessage}) while saving star.", ex.Message);
            }

            return success;
        }

        public async Task<bool> SaveUpgrade(UpgradeResource upgrade, CancellationToken stoppingToken)
        {
            var success = false;
            try
            {
                var existing = _ctx.Upgrades.FirstOrDefault(u => u.TypeId == upgrade.TypeId);

                if(existing is null)
                {
                    await _ctx.Upgrades.AddAsync(new Data.Models.Upgrade
                    {
                        TypeId = upgrade.TypeId,
                        Power = upgrade.Power,
                        Workforce = upgrade.Workforce,
                        SuperionicRate = upgrade.SuperionicRate,
                        MagmaticRate = upgrade.MagmaticRate,
                        CreateTime = DateTime.UtcNow
                    }, stoppingToken);
                }
                else
                {
                    existing.TypeId = upgrade.TypeId;
                    existing.Power = upgrade.Power;
                    existing.Workforce = upgrade.Workforce;
                    existing.SuperionicRate = upgrade.SuperionicRate;
                    existing.MagmaticRate = upgrade.MagmaticRate;
                    existing.ModifiedTime = DateTime.UtcNow;
                }

                await _ctx.SaveChangesAsync(stoppingToken);

                success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred ({ErrorMessage}) while saving upgrade.", ex.Message);
            }

            return success;
        }

        private class ConnectedSystem
        {
            public SolarSystem SolarSystem { get; set; }
            public Star Star { get; set; }
            public Planet Planet { get; set; }
            public Sovereignty Sovereignty { get; set; }
        }
    }
}
