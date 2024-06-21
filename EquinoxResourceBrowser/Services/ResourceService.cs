using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Dtos;
using EquinoxResourceBrowser.Dtos.Resources;
using EquinoxResourceBrowser.Enums;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

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
            var rootSystem = await _ctx.SystemResources.Where(s => s.SolarSystemId == solarSystemId).Select(s => new CalculatedResourceDto
            {
                SolarSystemId = solarSystemId,
                ConstellationId = s.ConstellationId,
                RegionId = s.RegionId,
                AllianceId = s.SovereignAllianceId,
                CorporationId = s.SovereignCorporationId,
                TotalPower = s.TotalPower ?? 0,
                TotalWorkforce = s.TotalWorkforce ?? 0,
            }).FirstOrDefaultAsync();

            if (rootSystem is null || filter == RestrictionFilter.SystemOnly)
            {
                // Do not calculate connected systems as we only care about the single system
                return rootSystem;
            }

            List<NodePart> nodeParts = [];

            if (filter == RestrictionFilter.ConstellationOnly)
            {
                nodeParts = await (from sr in _ctx.SystemResources
                                   join sg in _ctx.Stargates on sr.SolarSystemId equals sg.SolarSystemId
                                   where sr.ConstellationId == rootSystem.ConstellationId
                                   select new NodePart
                                   {
                                       Resource = new CalculatedResourceDto
                                       {
                                           SolarSystemId = sr.SolarSystemId,
                                           ConstellationId = sr.ConstellationId,
                                           RegionId = sr.RegionId,
                                           AllianceId = sr.SovereignAllianceId,
                                           CorporationId = sr.SovereignCorporationId,
                                           TotalPower = sr.TotalPower ?? 0,
                                           TotalWorkforce = sr.TotalWorkforce ?? 0,
                                       },
                                       Edge = sg.DestinationSystemId
                                   }).ToListAsync();
            }
            else if (filter == RestrictionFilter.RegionOnly)
            {
                nodeParts = await (from sr in _ctx.SystemResources
                                   join sg in _ctx.Stargates on sr.SolarSystemId equals sg.SolarSystemId
                                   where sr.RegionId == rootSystem.RegionId
                                   select new NodePart
                                   {
                                       Resource = new CalculatedResourceDto
                                       {
                                           SolarSystemId = sr.SolarSystemId,
                                           ConstellationId = sr.ConstellationId,
                                           RegionId = sr.RegionId,
                                           AllianceId = sr.SovereignAllianceId,
                                           CorporationId = sr.SovereignCorporationId,
                                           TotalPower = sr.TotalPower ?? 0,
                                           TotalWorkforce = sr.TotalWorkforce ?? 0,
                                       },
                                       Edge = sg.DestinationSystemId
                                   }).ToListAsync();
            }
            else if (filter == RestrictionFilter.AllConnected)
            {
                nodeParts = await (from sr in _ctx.SystemResources
                                   join sg in _ctx.Stargates on sr.SolarSystemId equals sg.SolarSystemId
                                   select new NodePart
                                   {
                                       Resource = new CalculatedResourceDto
                                       {
                                           SolarSystemId = sr.SolarSystemId,
                                           ConstellationId = sr.ConstellationId,
                                           RegionId = sr.RegionId,
                                           AllianceId = sr.SovereignAllianceId,
                                           CorporationId = sr.SovereignCorporationId,
                                           TotalPower = sr.TotalPower ?? 0,
                                           TotalWorkforce = sr.TotalWorkforce ?? 0,
                                       },
                                       Edge = sg.DestinationSystemId
                                   }).ToListAsync();
            }

            var nodes = nodeParts
                    .GroupBy(np => np.Resource, new ResourceComparer())
                    .Select(g => new Node
                    {
                        Resource = g.Key,
                        Edges = g.Select(s => s.Edge)
                    }).ToList();

            // Start traversal with conditions (only connected (edges) systems (within the specified filter) owned (alliances/corporation) by the same people)
            HashSet<int> visited = [rootSystem.SolarSystemId];
            TraverseSystems(rootSystem, nodes, visited);

            return rootSystem;
        }

        private void TraverseSystems(CalculatedResourceDto rootSystem, List<Node> nodes, HashSet<int> visited)
        {
            var currentNode = nodes.FirstOrDefault(n => n.Resource.SolarSystemId == rootSystem.SolarSystemId);

            if (currentNode == null) return;

            foreach (var edge in currentNode.Edges)
            {
                if (!visited.Contains(edge))
                {
                    var connectedNode = nodes.FirstOrDefault(n => n.Resource.SolarSystemId == edge);
                    if (connectedNode != null &&
                        ((rootSystem.AllianceId != null && connectedNode.Resource.AllianceId == rootSystem.AllianceId) ||
                        (rootSystem.CorporationId != null && connectedNode.Resource.CorporationId == rootSystem.CorporationId)))
                    {
                        rootSystem.TotalWorkforce += connectedNode.Resource.TotalWorkforce;

                        visited.Add(edge);
                        TraverseSystems(connectedNode.Resource, nodes, visited);
                    }
                }
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
                if (existingStars is not null && existingStars.Length > 0)
                {
                    foreach (var star in from ex in existingStars
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

                if (existing is null)
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

        private class Node
        {
            public CalculatedResourceDto Resource { get; set; } = default!;
            public IEnumerable<int> Edges { get; set; } = [];
        }

        private class NodePart
        {
            public CalculatedResourceDto Resource { get; set; } = default!;
            public int Edge { get; set; }
        }

        private class ResourceComparer : EqualityComparer<CalculatedResourceDto>
        {
            public override bool Equals(CalculatedResourceDto? x, CalculatedResourceDto? y)
            {
                return x?.SolarSystemId == y?.SolarSystemId;
            }

            public override int GetHashCode([DisallowNull] CalculatedResourceDto obj)
            {
                return obj.SolarSystemId.GetHashCode();
            }
        }
    }
}
