using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services.Synchronization
{
    public class FactionSync : BackgroundService
    {
        private readonly IEsiClient _esiClient;
        private readonly ILogger<FactionSync> _logger;
        private readonly IDbContextFactory<ResourceContext> _ctxFactory;

        public FactionSync(IEsiClient esiClient, ILogger<FactionSync> logger, IDbContextFactory<ResourceContext> ctxFactory)
        {
            _esiClient = esiClient;
            _logger = logger;
            _ctxFactory = ctxFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Faction synchronization started at: {Date}", DateTimeOffset.Now);

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
                _logger.LogInformation("Faction synchronization is stopping at: {Date}", DateTimeOffset.Now);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var newFactionCount = await SynchronizeFactions(stoppingToken);

            if (newFactionCount > 0)
            {
                _logger.LogInformation("{Count} new Factions ingested", newFactionCount);
            }
            else
            {
                _logger.LogInformation("Failed to find any new Factions to ingest");
            }
        }

        private async Task<int> SynchronizeFactions(CancellationToken stoppingToken)
        {
            var totalFactionsAdded = 0;
            try
            {
                var esiFactions = await _esiClient.GetFactions();

                if (esiFactions is null || esiFactions.Length == 0)
                {
                    return totalFactionsAdded;
                }

                await using var ctx = await _ctxFactory.CreateDbContextAsync(stoppingToken);
                var existingFactions = await ctx.Factions.Select(f => f.FactionId).ToArrayAsync(stoppingToken);

                IEnumerable<Data.Models.Faction> factionsToAdd;
                if(existingFactions is null || existingFactions.Length == 0)
                {
                    factionsToAdd = esiFactions
                        .Select(f => new Data.Models.Faction
                        {
                            FactionId = f.FactionId,
                            Name = f.Name,
                            Description = f.Description,
                            CorporationId = f.CorporationId,
                            MilitiaCorporationId = f.MilitiaCorporationId,
                            SizeFactor = f.SizeFactor,
                            SolarSystemId = f.SolarSystemId,
                            IsUnique = f.IsUnique,
                            StationCount = f.StationCount,
                            StationSystemCount = f.StationSystemCount,
                            CreateTime = DateTime.UtcNow
                        });
                }
                else
                {
                    factionsToAdd = esiFactions.ExceptBy(existingFactions, f => f.FactionId)
                        .Select(f => new Data.Models.Faction
                        {
                            FactionId = f.FactionId,
                            Name = f.Name,
                            Description = f.Description,
                            CorporationId = f.CorporationId,
                            MilitiaCorporationId = f.MilitiaCorporationId,
                            SizeFactor = f.SizeFactor,
                            SolarSystemId = f.SolarSystemId,
                            IsUnique = f.IsUnique,
                            StationCount = f.StationCount,
                            StationSystemCount = f.StationSystemCount,
                            CreateTime = DateTime.UtcNow
                        });
                }

                if (factionsToAdd.Any())
                {
                    await ctx.Factions.AddRangeAsync(factionsToAdd, stoppingToken);
                    totalFactionsAdded = await ctx.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception)
            {
            }

            return totalFactionsAdded;
        }
    }
}
