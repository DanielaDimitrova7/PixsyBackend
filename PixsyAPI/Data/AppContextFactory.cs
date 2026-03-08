using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PixsyAPI.Data;

public sealed class PixsyDbContextFactory : IDesignTimeDbContextFactory<PixsyDbContext>
{
    public PixsyDbContext CreateDbContext(string[] args)
    {
        var basePath = ResolveBasePath();

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing.");

        var optionsBuilder = new DbContextOptionsBuilder<PixsyDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new PixsyDbContext(optionsBuilder.Options);
    }

    private static string ResolveBasePath()
    {
        var current = Directory.GetCurrentDirectory();

        if (File.Exists(Path.Combine(current, "appsettings.json")))
            return current;

        var candidate = Path.Combine(current, "PixsyAPI");
        if (File.Exists(Path.Combine(candidate, "appsettings.json")))
            return candidate;

        return AppContext.BaseDirectory;
    }
}
