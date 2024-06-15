using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services.Synchronization
{
    public class RegionSync : BackgroundService
    {
        private readonly IEsiClient _esiClient;
        private readonly ILogger<RegionSync> _logger;
        private readonly IDbContextFactory<ResourceContext> _ctxFactory;

        public RegionSync(IEsiClient esiClient, ILogger<RegionSync> logger, IDbContextFactory<ResourceContext> ctxFactory)
        {
            _esiClient = esiClient;
            _logger = logger;
            _ctxFactory = ctxFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Region synchronization started at: {Date}", DateTimeOffset.Now);

            using var timer = new PeriodicTimer(TimeSpan.FromHours(3));

            await DoWork(stoppingToken);

            try
            {
                while(await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await DoWork(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Region synchronization is stopping at: {Date}", DateTimeOffset.Now);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var newRegionCount = await SynchronizeRegions(stoppingToken);

            if (newRegionCount > 0)
            {
                _logger.LogInformation("{Count} new regions ingested", newRegionCount);
            }
            else
            {
                _logger.LogInformation("Failed to find any new regions to ingest");
            }
        }

        private async Task<int> SynchronizeRegions(CancellationToken stoppingToken)
        {
            try
            {
                var esiRegions = await _esiClient.GetRegions();
                if(esiRegions is null || esiRegions.Length == 0)
                {
                    return 0;
                }

                // Can probably get away with ToArray'ing the whole table since there aren't that many regions
                // But this should probably be avoided elsewhere (*** cough *** planets *** cough ***)
                await using var ctx = await _ctxFactory.CreateDbContextAsync(stoppingToken);
                var existingRegions = await ctx.Regions.Select(r => r.RegionId).ToArrayAsync(stoppingToken);

                List<Data.Models.Region> newRegions;
                if(existingRegions is null || existingRegions.Length == 0)
                {
                    newRegions = await GetNewRegions(esiRegions, stoppingToken);
                }
                else
                {
                    // TODO: We need to compare to see if need to update (we don't care about string updates right now)
                    // This only considers it if we don't already have it
                    newRegions = await GetNewRegions(esiRegions.Except(existingRegions), stoppingToken);
                }

                int newlySavedRegions = 0;
                if (newRegions.Count > 0)
                {
                    await ctx.Regions.AddRangeAsync(newRegions, stoppingToken);
                    newlySavedRegions = await ctx.SaveChangesAsync(stoppingToken);
                }

                return newlySavedRegions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred ({ErrorMessage} while synchronizing regions", ex.Message);
            }

            return 0;
        }

        private async Task<List<Data.Models.Region>> GetNewRegions(IEnumerable<int> newRegionIds, CancellationToken stoppingToken)
        {
            // Get ALL from ESI and Send 'em all to db
            var newRegions = new List<Data.Models.Region>();
            foreach (var regionId in newRegionIds)
            {
                try
                {
                    if (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogCritical("Ingestion was cancelled");
                        break;
                    }

                    var esiRegion = await _esiClient.GetRegion(regionId);

                    if (esiRegion is null)
                    {
                        _logger.LogWarning("Region returned from ESI was null, this shouldn't happen...");
                        continue;
                    }

                    newRegions.Add(new Data.Models.Region
                    {
                        RegionId = esiRegion.Id,
                        Description = esiRegion.Description,
                        Name = esiRegion.Name,
                        CreateTime = DateTime.UtcNow
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurred ({ErrorMessage} while ingesting region: {RegionId}", ex.Message, regionId);
                }
            }

            return newRegions;
        }
    }
}
