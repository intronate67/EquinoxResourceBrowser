using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EquinoxResourceBrowser.Data;

public class DesignTimeResourceContext : IDesignTimeDbContextFactory<ResourceContext>
{
    public ResourceContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ResourceContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=resource_context;Integrated Security=True;TrustServerCertificate=True;Command Timeout=300");

        return new ResourceContext(optionsBuilder.Options);
    }
}