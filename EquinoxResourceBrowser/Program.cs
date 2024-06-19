using EquinoxResourceBrowser.Components;
using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Interfaces;
using EquinoxResourceBrowser.Services;
using EquinoxResourceBrowser.Services.Synchronization;
using Microsoft.EntityFrameworkCore;
using Refit;

namespace EquinoxResourceBrowser;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddRazorComponents();

        string connectionString;

        if (builder.Environment.IsDevelopment())
        {
            connectionString = builder.Configuration.GetConnectionString("ResourceDb")
            ?? throw new InvalidOperationException("Connection string 'ResourceDb' not found.");
        }
        else
        {
            connectionString = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")
            ?? throw new InvalidOperationException("Connection string 'AZURE_SQL_CONNECTIONSTRING' not found.");
        }

        builder.Services.AddDbContextFactory<ResourceContext>(options => options.UseSqlServer(connectionString));

        builder.Services.AddRefitClient<IEsiClient>()
            .ConfigureHttpClient(client =>
            {
                var baseUrl = builder.Configuration["EsiUrl"]
                    ?? throw new InvalidOperationException("Configuration value 'EsiUrl' not found");
                client.BaseAddress = new Uri(baseUrl);

                client.DefaultRequestHeaders.UserAgent
                    .TryParseAdd("Equinox Resource Browser; Contact: admin@killboard.space; IGN: Wiener Johnson");
            }).AddStandardResilienceHandler();
        
        builder.Services.AddMemoryCache();

        //builder.Services.AddHostedService<RegionSync>();
        //builder.Services.AddHostedService<ConstellationSync>();
        //builder.Services.AddHostedService<SolarSystemSync>();
        //builder.Services.AddHostedService<PlanetSync>();
        //builder.Services.AddHostedService<StarSync>();
        builder.Services.AddHostedService<SovereigntySync>();
        //builder.Services.AddHostedService<TypeSync>();
        builder.Services.AddHostedService<AllianceSync>();
        builder.Services.AddHostedService<CorporationSync>();
        //builder.Services.AddHostedService<FactionSync>();
        //builder.Services.AddHostedService<StargateSync>();

        builder.Services.AddScoped<IConstellationService, ConstellationService>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<IRegionService, RegionService>();
        builder.Services.AddScoped<IResourceService, ResourceService>();
        builder.Services.AddScoped<ISearchService, SearchService>();
        builder.Services.AddScoped<ISystemService, SystemService>();
        builder.Services.AddScoped<IUpgradeService, UpgradeService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
