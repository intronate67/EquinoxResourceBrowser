using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services.Synchronization
{
    public class TypeSync : BackgroundService
    {
        private readonly IEsiClient _esiClient;
        private readonly ILogger<TypeSync> _logger;
        private readonly IDbContextFactory<ResourceContext> _ctxFactory;

        public TypeSync(IEsiClient esiClient, ILogger<TypeSync> logger, IDbContextFactory<ResourceContext> ctxFactory)
        {
            _esiClient = esiClient;
            _logger = logger;
            _ctxFactory = ctxFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Type synchronization started at: {Date}", DateTimeOffset.Now);

            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(30));

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
                _logger.LogInformation("Type synchronization is stopping at: {Date}", DateTimeOffset.Now);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var newTypeCount = await SynchronizeTypes(stoppingToken);

            if (newTypeCount > 0)
            {
                _logger.LogInformation("{Count} new types ingested", newTypeCount);
            }
            else
            {
                _logger.LogInformation("Failed to find any new types to ingest");
            }
        }

        private async Task<int> SynchronizeTypes(CancellationToken stoppingToken)
        {
            var totalTypesAdded = 0;
            try
            {
                await using var ctx = await _ctxFactory.CreateDbContextAsync(stoppingToken);
                var typesWeNeed = await (from p in ctx.Planets
                                   select p.TypeId)
                                   .Union(from s in ctx.Stars
                                          select s.TypeId)
                                   .Union(from u in ctx.Upgrades
                                          select u.TypeId)
                                   .Distinct()
                                   .ToArrayAsync(stoppingToken);

                if(typesWeNeed is null || typesWeNeed.Length == 0)
                {
                    _logger.LogWarning("Failed to find any types in the DB, other jobs probably haven't run yet or are failing to injest.");
                    return totalTypesAdded;
                }

                var existingTypes = await ctx.Types.Select(t => t.TypeId).ToArrayAsync(stoppingToken);

                var typesToGet = typesWeNeed.Except(existingTypes).ToList();
                if(typesToGet is null || typesToGet.Count == 0)
                {
                    // No new types to grab, all good :)
                    return totalTypesAdded;
                }

                var typesToAdd = new List<Data.Models.Type>();
                foreach(var typeId in typesToGet)
                {
                    try
                    {
                        var newType = await _esiClient.GetType(typeId);

                        if(newType is not null)
                        {
                            typesToAdd.Add(new Data.Models.Type
                            {
                                TypeId = newType.Id,
                                Name = newType.Name,
                                Description = newType.Description,
                                CreateTime = DateTime.UtcNow
                            });
                        }
                    }
                    catch (Exception tex)
                    {
                        _logger.LogError(tex, "An exception occurred ({ErrorMessage}) while getting type: {TypeId}", tex.Message, typeId);
                    }
                }

                if(typesToAdd.Count == 0)
                {
                    // Failed to get any types even though we shoul've...
                    return totalTypesAdded;
                }

                await ctx.Types.AddRangeAsync(typesToAdd, stoppingToken);
                totalTypesAdded = await ctx.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred ({ErrorMessage} while synchronizing types", ex.Message);
            }

            return totalTypesAdded;
        }
    }
}
