using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services.Synchronization;

public class SovereigntySync : BackgroundService
{
    private readonly IEsiClient _esiClient;
    private readonly ILogger<SovereigntySync> _logger;
    private readonly IDbContextFactory<ResourceContext> _ctxFactory;

    public SovereigntySync(IEsiClient esiClient, ILogger<SovereigntySync> logger, IDbContextFactory<ResourceContext> ctxFactory)
    {
        _esiClient = esiClient;
        _logger = logger;
        _ctxFactory = ctxFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Sovereignty synchronization started at: {Date}", DateTimeOffset.Now);

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(15));

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
            _logger.LogInformation("Sovereignty synchronization is stopping at: {Date}", DateTimeOffset.Now);
        }
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        var newRegionCount = await SynchronizeSov(stoppingToken);

        if (newRegionCount > 0)
        {
            _logger.LogInformation("Updated {Count} system's sovereignty", newRegionCount);
        }
        else
        {
            _logger.LogInformation("Failed to find any new sovereignty updates");
        }
    }

    private async Task<int> SynchronizeSov(CancellationToken stoppingToken)
    {
        var sovUpdateCount = 0;

        try
        {
            var esiSov = await _esiClient.GetSovereigntyMap();

            if (esiSov is null || esiSov.Length == 0)
            {
                _logger.LogWarning("Failed to get the Sovereignty Map, ESI might be down...");
                return sovUpdateCount;
            }

            await using var ctx = await _ctxFactory.CreateDbContextAsync(stoppingToken);
            var existingSov = await ctx.Sovereignties.ToArrayAsync(stoppingToken);

            if(existingSov is null || existingSov.Length == 0)
            {
                // Add everything
                var newSovMap = esiSov.Select(s => new Data.Models.Sovereignty
                {
                    AllianceId = s.AllianceId,
                    CorporationId = s.CorporationId,
                    FactionId = s.FactionId,
                    SolarSystemId = s.SolarSystemId,
                    CreateTime = DateTime.UtcNow
                });

                await ctx.Sovereignties.AddRangeAsync(newSovMap, stoppingToken);
            }
            else
            {
                // Add missing
                var sovToAdd = new List<Data.Models.Sovereignty>();
                foreach(var solarSystem in esiSov)
                {
                    var existingSystemSov = existingSov.FirstOrDefault(s => s.SolarSystemId == solarSystem.SolarSystemId);

                    if(existingSystemSov is null)
                    {
                        // Add to list to be later added in db
                        sovToAdd.Add(new Data.Models.Sovereignty
                        {
                            AllianceId = solarSystem.AllianceId,
                            CorporationId = solarSystem.CorporationId,
                            FactionId = solarSystem.FactionId,
                            SolarSystemId = solarSystem.SolarSystemId,
                            CreateTime = DateTime.UtcNow
                        });
                    }
                    else if (existingSystemSov.AllianceId != solarSystem.AllianceId
                            || existingSystemSov.CorporationId != solarSystem.CorporationId
                            || existingSystemSov.FactionId != solarSystem.FactionId)
                    {
                        existingSystemSov.AllianceId = solarSystem.AllianceId;
                        existingSystemSov.CorporationId = solarSystem.CorporationId;
                        existingSystemSov.FactionId = solarSystem.FactionId;

                        existingSystemSov.ModifiedTime = DateTime.UtcNow;
                    }
                }

                if(sovToAdd.Count > 0)
                {
                    await ctx.AddRangeAsync(sovToAdd, stoppingToken);
                }
            }

            sovUpdateCount = await ctx.SaveChangesAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred ({ErrorMessage} while synchronizing system sovereignty", ex.Message);
        }

        return sovUpdateCount;
    }
}
