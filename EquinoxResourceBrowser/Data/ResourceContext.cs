using EquinoxResourceBrowser.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Data;

public class ResourceContext : DbContext
{
    public DbSet<Alliance> Alliances { get; set; }
    public DbSet<Constellation> Constellations { get; set; }
    public DbSet<Corporation> Corporations { get; set; }
    public DbSet<Faction> Factions { get; set; }
    public DbSet<Models.Type> Types { get; set; }
    public DbSet<Upgrade> Upgrades { get; set; }
    public DbSet<Planet> Planets { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<SolarSystem> SolarSystems { get; set; }
    public DbSet<Sovereignty> Sovereignties { get; set; }
    public DbSet<Stargate> Stargates { get; set; }
    public DbSet<Star> Stars { get; set; }

    public DbSet<VRegion> RegionResources { get; set; }

    public ResourceContext(DbContextOptions<ResourceContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<VRegion>(r =>
            {
                r.HasNoKey();
                r.ToView("View_Regions");
            });
    }
}
