using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services.Synchronization
{
    public class SolarSystemSync : BackgroundService
    {
        private readonly IEsiClient _esiClient;
        private readonly ILogger<SolarSystemSync> _logger;
        private readonly IDbContextFactory<ResourceContext> _ctxFactory;

        public SolarSystemSync(IEsiClient esiClient, ILogger<SolarSystemSync> logger, IDbContextFactory<ResourceContext> ctxFactory)
        {
            _esiClient = esiClient;
            _logger = logger;
            _ctxFactory = ctxFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Solar System synchronization started at: {Date}", DateTimeOffset.Now);

            using var timer = new PeriodicTimer(TimeSpan.FromHours(3));

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
                _logger.LogInformation("Solar System synchronization is stopping at: {Date}", DateTimeOffset.Now);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var newRegionCount = await SynchronizeSystems(stoppingToken);

            if (newRegionCount > 0)
            {
                _logger.LogInformation("{Count} new solar system(s) ingested", newRegionCount);
            }
            else
            {
                _logger.LogInformation("Failed to find any new solar systems to ingest");
            }
        }

        private async Task<int> SynchronizeSystems(CancellationToken stoppingToken)
        {
            try
            {
                var esiSystems = await _esiClient.GetSolarSystems();
                if (esiSystems is null || esiSystems.Length == 0)
                {
                    return 0;
                }

                // Can probably get away with ToArray'ing the whole table since there aren't that many regions
                // But this should probably be avoided elsewhere (*** cough *** planets *** cough ***)
                await using var ctx = await _ctxFactory.CreateDbContextAsync(stoppingToken);
                var existingSystems = await ctx.SolarSystems.Select(r => r.SolarSystemId).ToArrayAsync(stoppingToken);

                List<Data.Models.SolarSystem> newSystems;
                if (existingSystems is null || existingSystems.Length == 0)
                {
                    newSystems = await GetNewSystems(esiSystems, stoppingToken);
                }
                else
                {
                    // TODO: We need to compare to see if need to update (we don't care about string updates right now)
                    // This only considers it if we don't already have it
                    newSystems = await GetNewSystems(esiSystems.Except(existingSystems), stoppingToken);
                }

                int newlySavedSystems = 0;
                if (newSystems.Count > 0)
                {
                    await ctx.SolarSystems.AddRangeAsync(newSystems, stoppingToken);
                    newlySavedSystems = await ctx.SaveChangesAsync(stoppingToken);
                }

                return newlySavedSystems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred ({ErrorMessage} while synchronizing solar systems", ex.Message);
            }

            return 0;
        }

        private async Task<List<Data.Models.SolarSystem>> GetNewSystems(IEnumerable<int> newSystemIds, CancellationToken stoppingToken)
        {
            var newSystems = new List<Data.Models.SolarSystem>();
            foreach (var systemId in newSystemIds)
            {
                try
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogCritical("Ingestion was cancelled");
                        break;
                    }

                    var esiSystem = await _esiClient.GetSolarSystem(systemId);

                    if (esiSystem is null)
                    {
                        _logger.LogWarning("Solar System returned from ESI was null, this shouldn't happen...");
                        continue;
                    }

                    newSystems.Add(new Data.Models.SolarSystem
                    {
                        SolarSystemId = esiSystem.Id,
                        SecurityClass = esiSystem.SecurityClass ?? "",
                        SecurityStatus = esiSystem.SecurityStatus,
                        StarId = esiSystem.StarId,
                        ConstellationId = esiSystem.ConstellationId,
                        Name = esiSystem.Name,
                        CreateTime = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurred ({ErrorMessage} while ingesting solar system: {SolarSystemId}", ex.Message, systemId);
                }
            }

            return newSystems;
        }
    }
}
