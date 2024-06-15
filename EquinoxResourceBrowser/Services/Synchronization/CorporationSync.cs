using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services.Synchronization
{
    public class CorporationSync : BackgroundService
    {
        private readonly IEsiClient _esiClient;
        private readonly ILogger<CorporationSync> _logger;
        private readonly IDbContextFactory<ResourceContext> _ctxFactory;

        public CorporationSync(IEsiClient esiClient, ILogger<CorporationSync> logger, IDbContextFactory<ResourceContext> ctxFactory)
        {
            _esiClient = esiClient;
            _logger = logger;
            _ctxFactory = ctxFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Corporation synchronization started at: {Date}", DateTimeOffset.Now);

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
                _logger.LogInformation("Corporation synchronization is stopping at: {Date}", DateTimeOffset.Now);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var newRegionCount = await SynchronizeCorporations(stoppingToken);

            if (newRegionCount > 0)
            {
                _logger.LogInformation("{Count} new Alliances ingested", newRegionCount);
            }
            else
            {
                _logger.LogInformation("Failed to find any new Alliances to ingest");
            }
        }

        private async Task<int> SynchronizeCorporations(CancellationToken stoppingToken)
        {
            var totalCorporationsAdded = 0;
            try
            {
                await using var ctx = await _ctxFactory.CreateDbContextAsync(stoppingToken);

                var corporationsNeeded = await ctx.Sovereignties
                    .Where(s => s.CorporationId != null)
                    .Select(s => s.CorporationId!.Value)
                    .ToArrayAsync(stoppingToken);

                if (corporationsNeeded is null || corporationsNeeded.Length == 0)
                {
                    return totalCorporationsAdded;
                }

                var corporationsWeHave = await ctx.Corporations.Select(a => a.CorporationId).ToArrayAsync(stoppingToken);

                var corporationsToGet = corporationsNeeded.Except(corporationsWeHave).ToList();

                if (corporationsToGet is null || corporationsToGet.Count == 0)
                {
                    // Yay no alliances to get, we're up to date :)
                    return totalCorporationsAdded;
                }

                var corporationsToAdd = new List<Data.Models.Corporation>();
                foreach (var corporationId in corporationsToGet)
                {
                    try
                    {
                        var newCorporation = await _esiClient.GetCorporation(corporationId);

                        if (newCorporation is not null)
                        {
                            corporationsToAdd.Add(new Data.Models.Corporation
                            {
                                CorporationId = corporationId,
                                AllianceId = newCorporation.AllianceId,
                                Name = newCorporation.Name,
                                Description = newCorporation.Description,
                                DateFounded = newCorporation.CreationDate,
                                FactionId = newCorporation.FactionId,
                                Ticker = newCorporation.Ticker,
                                CreateTime = DateTime.UtcNow
                            });
                        }
                    }
                    catch (Exception aex)
                    {
                        _logger.LogError(aex, "An exception occurred ({ErrorMessage}) while getting corporation: {CorporationId}", aex.Message, corporationId);
                    }
                }

                if (corporationsToAdd.Count == 0)
                {
                    // Failed to get any alliances even though we should've...
                    return totalCorporationsAdded;
                }

                await ctx.Corporations.AddRangeAsync(corporationsToAdd, stoppingToken);
                totalCorporationsAdded = await ctx.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred ({ErrorMessage} while synchronizing corporations", ex.Message);
            }

            return totalCorporationsAdded;
        }
    }
}
