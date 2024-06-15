using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services.Synchronization
{
    public class AllianceSync : BackgroundService
    {
        private readonly IEsiClient _esiClient;
        private readonly ILogger<AllianceSync> _logger;
        private readonly IDbContextFactory<ResourceContext> _ctxFactory;

        public AllianceSync(IEsiClient esiClient, ILogger<AllianceSync> logger, IDbContextFactory<ResourceContext> ctxFactory)
        {
            _esiClient = esiClient;
            _logger = logger;
            _ctxFactory = ctxFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Alliance synchronization started at: {Date}", DateTimeOffset.Now);

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
                _logger.LogInformation("Alliance synchronization is stopping at: {Date}", DateTimeOffset.Now);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var newRegionCount = await SynchronizeAlliances(stoppingToken);

            if (newRegionCount > 0)
            {
                _logger.LogInformation("{Count} new Alliances ingested", newRegionCount);
            }
            else
            {
                _logger.LogInformation("Failed to find any new Alliances to ingest");
            }
        }

        private async Task<int> SynchronizeAlliances(CancellationToken stoppingToken)
        {
            var totalAlliancesAdded = 0;
            try
            {
                await using var ctx = await _ctxFactory.CreateDbContextAsync(stoppingToken);

                var alliancesNeeded = await ctx.Sovereignties
                    .Where(s => s.AllianceId != null)
                    .Select(s => s.AllianceId!.Value)
                    .ToArrayAsync(stoppingToken);

                if(alliancesNeeded is null || alliancesNeeded.Length == 0)
                {
                    return totalAlliancesAdded;
                }

                var alliancesWeHave = await ctx.Alliances.Select(a => a.AllianceId).ToArrayAsync(stoppingToken);

                var alliancesToGet = alliancesNeeded.Except(alliancesWeHave).ToList();

                if(alliancesToGet is null || alliancesToGet.Count == 0)
                {
                    // Yay no alliances to get, we're up to date :)
                    return totalAlliancesAdded;
                }

                var alliancesToAdd = new List<Data.Models.Alliance>();
                foreach(var allianceId in alliancesToGet)
                {
                    try
                    {
                        var newAlliance = await _esiClient.GetAlliance(allianceId);

                        if(newAlliance is not null)
                        {
                            alliancesToAdd.Add(new Data.Models.Alliance
                            {
                                AllianceId = allianceId,
                                Name = newAlliance.Name,
                                CreatorCharacterId = newAlliance.CreatorCharacterId,
                                CreatorCorporationId = newAlliance.CreatorCorporationId,
                                ExecutorCorporationId = newAlliance.ExecutorCorporationId,
                                DateFounded = newAlliance.CreationDate,
                                FactionId = newAlliance.FactionId,
                                Ticker = newAlliance.Ticker,
                                CreateTime = DateTime.UtcNow
                            });
                        }
                    }
                    catch (Exception aex)
                    {
                        _logger.LogError(aex, "An exception occurred ({ErrorMessage}) while getting alliance: {AllianceId}", aex.Message, allianceId);
                    }
                }

                if (alliancesToAdd.Count == 0)
                {
                    // Failed to get any alliances even though we should've...
                    return totalAlliancesAdded;
                }

                await ctx.Alliances.AddRangeAsync(alliancesToAdd, stoppingToken);
                totalAlliancesAdded = await ctx.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred ({ErrorMessage} while synchronizing alliances", ex.Message);
            }

            return totalAlliancesAdded;
        }
    }
}
