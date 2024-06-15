using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services.Synchronization
{
    public class StarSync : BackgroundService
    {
        private readonly IEsiClient _esiClient;
        private readonly ILogger<StarSync> _logger;
        private readonly IDbContextFactory<ResourceContext> _ctxFactory;

        public StarSync(IEsiClient esiClient, ILogger<StarSync> logger, IDbContextFactory<ResourceContext> ctxFactory)
        {
            _esiClient = esiClient;
            _logger = logger;
            _ctxFactory = ctxFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Star synchronization started at: {Date}", DateTimeOffset.Now);

            using var timer = new PeriodicTimer(TimeSpan.FromHours(2));

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
                _logger.LogInformation("Star synchronization is stopping at: {Date}", DateTimeOffset.Now);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var newStarCount = await SynchronizeStars(stoppingToken);

            if (newStarCount > 0)
            {
                _logger.LogInformation("{Count} new stars ingested", newStarCount);
            }
            else
            {
                _logger.LogInformation("Failed to find any new stars to ingest");
            }
        }

        private async Task<int> SynchronizeStars(CancellationToken stoppingToken)
        {
            var totalSavedStars = 0;

            try
            {
                await using var ctx = await _ctxFactory.CreateDbContextAsync(stoppingToken);

                var knownStars = await ctx.SolarSystems
                    .Where(s => s.StarId.HasValue)
                    .Select(s => s.StarId!.Value)
                    .ToArrayAsync(stoppingToken);

                if(knownStars is null || knownStars.Length == 0)
                {
                    return 0;
                }

                var existingStars = await ctx.Stars.Select(s => s.StarId).ToArrayAsync(stoppingToken);
                var newStarIds = knownStars.Except(existingStars).ToList();

                for (int i = 0; i < newStarIds.Count; i += 100)
                {
                    var starBatch = newStarIds.Skip(i).Take(100);

                    await using var transaction = await ctx.Database.BeginTransactionAsync(stoppingToken);
                    try
                    {
                        var newStars = new List<Data.Models.Star>();

                        foreach (var starId in starBatch)
                        {
                            if (stoppingToken.IsCancellationRequested)
                            {
                                await transaction.RollbackAsync(stoppingToken);
                                return totalSavedStars;
                            }

                            var newStar = await GetStarForSystem(starId, stoppingToken);

                            if (newStar is not null)
                            {
                                newStars.Add(newStar);
                            }
                        }

                        if(newStars.Count > 0)
                        {
                            await ctx.Stars.AddRangeAsync(newStars, stoppingToken);
                            totalSavedStars += await ctx.SaveChangesAsync(stoppingToken);
                            await transaction.CommitAsync(stoppingToken);

                            _logger.LogInformation("Saved {Count} new stars", newStars.Count);
                        }
                    }
                    catch (Exception pex)
                    {
                        await transaction.RollbackAsync(stoppingToken);
                        _logger.LogError(pex, "An error occurred while processing a batch of stars");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred ({ErrorMessage} while synchronizing stars", ex.Message);
            }

            return totalSavedStars;
        }

        private async Task<Data.Models.Star?> GetStarForSystem(int starId, CancellationToken stoppingToken)
        {
            var esiStar = await _esiClient.GetStar(starId);

            if (esiStar is null)
            {
                _logger.LogWarning("Star returned from ESI was null, this shouldn't happen...");
                return null;
            }

            return new Data.Models.Star
            {
                StarId = starId,
                SolarSystemId = esiStar.SolarSystemId,
                SpectralClass = esiStar.SpectralClass,
                Age = esiStar.Age,
                Luminosity = esiStar.Luminosity,
                Name = esiStar.Name,
                Radius = esiStar.Radius,
                Temperature = esiStar.Temperature,
                TypeId = esiStar.TypeId,
                CreateTime = DateTime.UtcNow
            };
        }
    }
}
