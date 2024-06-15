using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services.Synchronization
{
    public class ConstellationSync : BackgroundService
    {
        private readonly IEsiClient _esiClient;
        private readonly ILogger<ConstellationSync> _logger;
        private readonly IDbContextFactory<ResourceContext> _ctxFactory;

        public ConstellationSync(IEsiClient esiClient, ILogger<ConstellationSync> logger, IDbContextFactory<ResourceContext> ctxFactory)
        {
            _esiClient = esiClient;
            _logger = logger;
            _ctxFactory = ctxFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Constellation synchronization started at: {Date}", DateTimeOffset.Now);

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
                _logger.LogInformation("Constellation synchronization is stopping at: {Date}", DateTimeOffset.Now);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var newRegionCount = await SynchronizeConstellations(stoppingToken);

            if (newRegionCount > 0)
            {
                _logger.LogInformation("{Count} new constellations ingested", newRegionCount);
            }
            else
            {
                _logger.LogInformation("Failed to find any new constellations to ingest");
            }
        }

        private async Task<int> SynchronizeConstellations(CancellationToken stoppingToken)
        {
            try
            {
                var esiConstellations = await _esiClient.GetConstellations();
                if (esiConstellations is null || esiConstellations.Length == 0)
                {
                    return 0;
                }

                // Can probably get away with ToArray'ing the whole table since there aren't that many regions
                // But this should probably be avoided elsewhere (*** cough *** planets *** cough ***)
                await using var ctx = await _ctxFactory.CreateDbContextAsync(stoppingToken);
                var existingConstellations = await ctx.Constellations.Select(r => r.ConstellationId).ToArrayAsync(stoppingToken);

                List<Data.Models.Constellation> newConstellations;
                if (existingConstellations is null || existingConstellations.Length == 0)
                {
                    newConstellations = await GetNewConstellations(esiConstellations, stoppingToken);
                }
                else
                {
                    // TODO: We need to compare to see if need to update (we don't care about string updates right now)
                    // This only considers it if we don't already have it
                    newConstellations = await GetNewConstellations(esiConstellations.Except(existingConstellations), stoppingToken);
                }

                int newlySavedConstellations = 0;
                if (newConstellations.Count > 0)
                {
                    await ctx.Constellations.AddRangeAsync(newConstellations, stoppingToken);
                    newlySavedConstellations = await ctx.SaveChangesAsync(stoppingToken);
                }

                return newlySavedConstellations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred ({ErrorMessage} while synchronizing constellations", ex.Message);
            }

            return 0;
        }

        private async Task<List<Data.Models.Constellation>> GetNewConstellations(IEnumerable<int> newConstellationIds, CancellationToken stoppingToken)
        {
            // Get ALL from ESI and Send 'em all to db
            var newRegions = new List<Data.Models.Constellation>();
            foreach (var constellationId in newConstellationIds)
            {
                try
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogCritical("Ingestion was cancelled");
                        break;
                    }

                    var esiConstellation = await _esiClient.GetConstellation(constellationId);

                    if (esiConstellation is null)
                    {
                        _logger.LogWarning("Constellation returned from ESI was null, this shouldn't happen...");
                        continue;
                    }

                    newRegions.Add(new Data.Models.Constellation
                    {
                        ConstellationId = esiConstellation.Id,
                        RegionId = esiConstellation.RegionId,
                        Name = esiConstellation.Name,
                        CreateTime = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurred ({ErrorMessage} while ingesting constellation: {ConstellationId}", ex.Message, constellationId);
                }
            }

            return newRegions;
        }
    }
}
