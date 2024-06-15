using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services.Synchronization
{
    public class StargateSync : BackgroundService
    {
        private readonly IEsiClient _esiClient;
        private readonly ILogger<StargateSync> _logger;
        private readonly IDbContextFactory<ResourceContext> _ctxFactory;

        public StargateSync(IEsiClient esiClient, ILogger<StargateSync> logger, IDbContextFactory<ResourceContext> ctxFactory)
        {
            _esiClient = esiClient;
            _logger = logger;
            _ctxFactory = ctxFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stargate synchronization started at: {Date}", DateTimeOffset.Now);

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
                _logger.LogInformation("Stargate synchronization is stopping at: {Date}", DateTimeOffset.Now);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            var newRegionCount = await SynchronizeStargates(stoppingToken);

            if (newRegionCount > 0)
            {
                _logger.LogInformation("{Count} new stargates ingested", newRegionCount);
            }
            else
            {
                _logger.LogInformation("Failed to find any new stargates to ingest");
            }
        }

        private async Task<int> SynchronizeStargates(CancellationToken stoppingToken)
        {
            var totalSavedGates = 0;

            try
            {
                await using var ctx = await _ctxFactory.CreateDbContextAsync(stoppingToken);
                // Get the ones without stargates first as we need those system's stargates more than those we already have stargates for
                var knownSystems = await (from s in ctx.SolarSystems
                                          join st in ctx.Stargates on s.SolarSystemId equals st.SolarSystemId into stargates
                                          from stargate in stargates.DefaultIfEmpty()
                                          group stargate by s.SolarSystemId into grouped
                                          orderby grouped.Count()
                                          select grouped.Key).ToArrayAsync(stoppingToken);

                if (knownSystems is null || knownSystems.Length == 0)
                {
                    return totalSavedGates;
                }

                foreach (var systemId in knownSystems)
                {
                    await using var transaction = await ctx.Database.BeginTransactionAsync(stoppingToken);

                    try
                    {
                        if (stoppingToken.IsCancellationRequested)
                        {
                            return totalSavedGates;
                        }

                        List<Data.Models.Stargate> newStargates = await GetStargatesForSystem(systemId, stoppingToken);

                        var existingStargates = await ctx.Stargates
                            .Where(p => p.SolarSystemId == systemId)
                            .Select(p => p.StargateId)
                            .ToListAsync(stoppingToken);

                        var stargatesToAdd = newStargates.ExceptBy(existingStargates, p => p.StargateId).ToArray();

                        if (stargatesToAdd.Length == 0)
                        {
                            _logger.LogDebug("Failed to find any new stargates for Solar System: {SystemId}", systemId);
                            continue;
                        }

                        await ctx.Stargates.AddRangeAsync(stargatesToAdd, stoppingToken);
                        var newlySavedSystems = await ctx.SaveChangesAsync(stoppingToken);
                        await transaction.CommitAsync(stoppingToken);

                        _logger.LogInformation("Saved {Count} new stargates for Solar System ID: {SystemId}", newlySavedSystems, systemId);

                        totalSavedGates += newlySavedSystems;
                    }
                    catch (Exception pex)
                    {
                        await transaction.RollbackAsync(stoppingToken);
                        _logger.LogError(pex, "An exception occurred ({ErrorMessage} while synchronizing stargates for Solar System: {SystemId}", pex.Message, systemId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred ({ErrorMessage} while synchronizing stargates", ex.Message);
            }

            return totalSavedGates;
        }

        private async Task<List<Data.Models.Stargate>> GetStargatesForSystem(int systemId, CancellationToken stoppingToken)
        {
            var esiSystem = await _esiClient.GetSolarSystem(systemId);

            if (esiSystem is null)
            {
                _logger.LogWarning("Solar System returned from ESI was null, this shouldn't happen...");
                return [];
            }

            var newStargates = new List<Data.Models.Stargate>();
            if(esiSystem.Stargates is not null)
            {
                foreach (var stargateId in esiSystem.Stargates)
                {
                    try
                    {
                        if (stoppingToken.IsCancellationRequested)
                        {
                            _logger.LogCritical("Ingestion was cancelled");
                            break;
                        }

                        var esiStargate = await _esiClient.GetStargate(stargateId);
                        if (esiStargate is null)
                        {
                            _logger.LogWarning("Stargate returned from ESI was null, this shouldn't happen...");
                            continue;
                        }

                        newStargates.Add(new Data.Models.Stargate
                        {
                            StargateId = esiStargate.StargateId,
                            SolarSystemId = systemId,
                            Name = esiStargate.Name,
                            DestinationStargateId = esiStargate.Destination.StargateId,
                            DestinationSystemId = esiStargate.Destination.SolarSystemId,
                            X = esiStargate.Position.X,
                            Y = esiStargate.Position.Y,
                            Z = esiStargate.Position.Z,
                            TypeId = esiStargate.TypeId,
                            CreateTime = DateTime.UtcNow
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An exception occurred ({ErrorMessage} while ingesting solar system: {SolarSystemId}", ex.Message, systemId);
                    }
                }
            }
            
            return newStargates;
        }
    }
}
