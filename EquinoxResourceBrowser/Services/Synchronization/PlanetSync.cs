using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services.Synchronization
{
    public class PlanetSync : BackgroundService
    {
        private readonly IEsiClient _esiClient;
        private readonly ILogger<PlanetSync> _logger;
        private readonly IDbContextFactory<ResourceContext> _ctxFactory;

        public PlanetSync(IEsiClient esiClient, ILogger<PlanetSync> logger, IDbContextFactory<ResourceContext> ctxFactory)
        {
            _esiClient = esiClient;
            _logger = logger;
            _ctxFactory = ctxFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Planet synchronization started at: {Date}", DateTimeOffset.Now);

            using var timer = new PeriodicTimer(TimeSpan.FromHours(1));

            await DoWork(stoppingToken);

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await DoWork(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Planet synchronization is stopping at: {Date}", DateTimeOffset.Now);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var newRegionCount = await SynchronizePlanets(stoppingToken);

            if (newRegionCount > 0)
            {
                _logger.LogInformation("{Count} new planets ingested", newRegionCount);
            }
            else
            {
                _logger.LogInformation("Failed to find any new planets to ingest");
            }
        }

        private async Task<int> SynchronizePlanets(CancellationToken stoppingToken)
        {
            var totalSavedPlanets = 0;

            try
            {
                await using var ctx = await _ctxFactory.CreateDbContextAsync(stoppingToken);
                // Get the ones without planets first as we need those system's planets more than those we already have planets for
                var knownSystems = await (from s in ctx.SolarSystems
                            join p in ctx.Planets on s.SolarSystemId equals p.SolarSystemId into ps
                            from planet in ps.DefaultIfEmpty()
                            group planet by s.SolarSystemId into grouped
                            orderby grouped.Count()
                            select grouped.Key).ToArrayAsync(stoppingToken);

                if (knownSystems is null || knownSystems.Length == 0)
                {
                    return totalSavedPlanets;
                }

                foreach (var systemId in knownSystems)
                {
                    await using var transaction = await ctx.Database.BeginTransactionAsync(stoppingToken);

                    try
                    {
                        if (stoppingToken.IsCancellationRequested)
                        {
                            return totalSavedPlanets;
                        }

                        List<Data.Models.Planet> newPlanets = await GetPlanetsForSystem(systemId, stoppingToken);

                        var existingPlanets = await ctx.Planets
                            .Where(p => p.SolarSystemId == systemId)
                            .Select(p => p.PlanetId)
                            .ToListAsync(stoppingToken);

                        var planetsToAdd = newPlanets.ExceptBy(existingPlanets, p => p.PlanetId).ToArray();

                        if (planetsToAdd.Length == 0)
                        {
                            _logger.LogDebug("Failed to find any new planets for Solar System: {SystemId}", systemId);
                            continue;
                        }

                        await ctx.Planets.AddRangeAsync(planetsToAdd, stoppingToken);
                        var newlySavedSystems = await ctx.SaveChangesAsync(stoppingToken);
                        await transaction.CommitAsync(stoppingToken);

                        _logger.LogInformation("Saved {Count} new planets for Solar System ID: {SystemId}", newlySavedSystems, systemId);

                        totalSavedPlanets += newlySavedSystems;
                    }
                    catch (Exception pex)
                    {
                        await transaction.RollbackAsync(stoppingToken);
                        _logger.LogError(pex, "An exception occurred ({ErrorMessage} while synchronizing planets for Solar System: {SystemId}", pex.Message, systemId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred ({ErrorMessage} while synchronizing planets", ex.Message);
            }

            return totalSavedPlanets; 
        }

        private async Task<List<Data.Models.Planet>> GetPlanetsForSystem(int systemId, CancellationToken stoppingToken)
        {
            var esiSystem = await _esiClient.GetSolarSystem(systemId);

            if (esiSystem is null)
            {
                _logger.LogWarning("Solar System returned from ESI was null, this shouldn't happen...");
                return [];
            }

            var newPlanets = new List<Data.Models.Planet>();
            foreach (var planet in esiSystem.Planets)
            {
                try
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogCritical("Ingestion was cancelled");
                        break;
                    }

                    var esiPlanet = await _esiClient.GetPlanet(planet.PlanetId);
                    if (esiPlanet is null)
                    {
                        _logger.LogWarning("Planet returned from ESI was null, this shouldn't happen...");
                        continue;
                    }

                    newPlanets.Add(new Data.Models.Planet
                    {
                        SolarSystemId = systemId,
                        Name = esiPlanet.Name,
                        PlanetId = planet.PlanetId,
                        TypeId = esiPlanet.TypeId,
                        CreateTime = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurred ({ErrorMessage} while ingesting solar system: {SolarSystemId}", ex.Message, systemId);
                }
            }

            return newPlanets;
        }
    }
}
